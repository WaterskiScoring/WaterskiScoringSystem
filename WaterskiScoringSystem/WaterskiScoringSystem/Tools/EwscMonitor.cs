using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Quobject.SocketIoClientDotNet.Client;
using Quobject.EngineIoClientDotNet;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WaterskiScoringSystem.Common;
using ICSharpCode.SharpZipLib.Zip;

namespace WaterskiScoringSystem.Tools {
	class EwscMonitor {
		private static String mySanctionNum;
		private static DataRow myTourRow;
		private static Boolean myConnectActive = false;

		public static String EwcsWebLocation = "";
		private static String EwcsWebLocationDefault = "http://ewscdata.com:40000/";
		//private static String EwcsWebLocationDefault = "http://waterskiconnect.com:40000/";
		private static Quobject.SocketIoClientDotNet.Client.Socket socketClient = null;

		public static void execEwscMonitoring() {
			if (EwscMonitor.EwcsWebLocation.Length > 1) return;
			EwscMonitor.EwcsWebLocation = EwcsWebLocationDefault;
			String returnValue = startEwscMonitoring();
			MessageBox.Show("WaterSkiConnect: Results=" + returnValue);
		}

		private static String startEwscMonitoring() {
			String curMethodName = "startEwscMonitoring: ";
			getSanctionNum();

			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "loggingdetail", "no" }
					, { "mode", "Tournament" }
					, { "eventid", mySanctionNum }
					, { "provider", "Mass Water Ski Association" }
					, { "application", Properties.Settings.Default.AppTitle }
					, { "version", Properties.Settings.Default.AppVersion }
					, { "username", "mawsa@comcast.net" }
				};
			String jsonData = JsonConvert.SerializeObject(sendConnectionMsg);

			Log.WriteFile(curMethodName + String.Format("Connected to {0}", EwcsWebLocation));
			socketClient = IO.Socket(EwcsWebLocation);

			socketClient.On(Socket.EVENT_CONNECT, () => {
				Log.WriteFile(curMethodName + String.Format("Connection: {0}", jsonData));
				socketClient.Emit("manual_connection_parameter", jsonData);
			});

			startClientListeners();

			int count = 0;
			while (count >= 0) {
				System.Threading.Thread.Sleep(2000);

				if (count > 5) {
					int returnMsg = checkForMsgToSend();
					if (returnMsg > 0) {
						EwscMonitor.EwcsWebLocation = "";
						return "Exit WaterSkiConnect";
					}
					count = 0;
				}

				count++;
			}
			String returnValue = "";
			return returnValue;
		}

		private static void startClientListeners() {
			socketClient.On("connect_confirm", (data) => {
				showMsg("connect_confirm", String.Format("connect_confirm: {0}", data));
			});

			socketClient.On("boat_times", (data) => {
				showMsg("boat_times", String.Format("boat_times {0}", data));
			});

			socketClient.On("scoring_result", (data) => {
				showMsg("scoring_result", String.Format("scoring_result {0}", data));
			});

			socketClient.On("boatpath_data", (data) => {
				showMsg("boatpath_data", String.Format("boatpath_data {0}", data));
			});

			socketClient.On("trickscoring_detail", (data) => {
				showMsg("trickscoring_detail", String.Format("trickscoring_detail {0}", data));
			});

			socketClient.On("jumpmeasurement_score", (data) => {
				showMsg("jumpmeasurement_score", String.Format("jumpmeasurement_score {0}", data));
			});
		}

		private static void showMsg(String msgType, String msg) {
			addEwscListenMsg(msgType, msg);
			if (msgType.Equals("connect_confirm")) {
				if (myConnectActive == false) {
					myConnectActive = true;
					MessageBox.Show(msg);
				}
			} else {
				//MessageBox.Show(msg);
			}
		}

		/*
         * Check for messages to be sent
         */
		private static int checkForMsgToSend() {
			DataTable curDataTable = getEwscMsg();
			foreach (DataRow curDataRow in curDataTable.Rows) {
				if (((String)curDataRow["MsgType"]).Equals("Exit")) {
					myConnectActive = false;
					removeEwscMsg((int)curDataRow["PK"]);
					return 1;
				}
				socketClient.Emit((String)curDataRow["MsgType"], (String)curDataRow["MsgData"]);
				removeEwscMsg((int)curDataRow["PK"]);
			}
			return 0;
		}

		public static void showPin() {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("Select SanctionId, MsgType, MsgData, CreateDate From EwscListenMsg ");
			curSqlStmt.Append("Where SanctionId = '" + mySanctionNum + "' ");
			curSqlStmt.Append("AND MsgType = 'connect_confirm' ");
			curSqlStmt.Append("Order by CreateDate DESC");
			DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
			String msg = "Information is not available";
			if ( curDataTable.Rows.Count > 0 ) {
				msg = (String)curDataTable.Rows[0]["MsgData"];
			}
			MessageBox.Show(msg);
		}

		public static Boolean sendBoatData(String boatId, String boatManufacturer, String boatModel
			, Int16 boatYear, String boatColor, String boatComment) {
			Dictionary<string, string> sendMsg = new Dictionary<string, string> {
					{ "boatId", boatId }
					, { "boatManufacturer", boatManufacturer }
					, { "boatModel", boatModel }
					, { "boatYear", boatYear.ToString() }
					, { "boatColor", boatColor }
					, { "boatComment", boatComment }
				};
			addEwscMsg("boat_data", JsonConvert.SerializeObject(sendMsg));
			return true;
		}

		public static Boolean sendAthleteData(String athleteId, String athleteName, String athleteEvent, String athleteCountry, String athleteRegion, String eventGroup
			, String round, Int16 passNumber, Int16 speed, String rope, String split) {
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "athleteId", athleteId }
					, { "athleteName", athleteName }
					, { "athleteEvent", athleteEvent }
					, { "athleteCountry", athleteCountry.ToUpper() }
					, { "athleteRegion", athleteRegion.ToUpper() }
					, { "round", round }
					, { "passNumber", passNumber }
					, { "speed", speed }
					, { "rope", rope }
					, { "split", split }
				};

			DataTable curDataTable = getDriverAssignment(athleteEvent, eventGroup, round);
			if ( curDataTable.Rows.Count > 0 ) {
				DataRow curDataRow = curDataTable.Rows[(curDataTable.Rows.Count - 1)];
				sendMsg.Add("driver", buildOfficialEntry(curDataRow));
			}

			addEwscMsg("athlete_data", JsonConvert.SerializeObject(sendMsg));
			return true;
		}

		public static Boolean sendAthleteScore(String athleteId, String athleteName, String athleteEvent, String athleteCountry, String athleteRegion
			, Int16 passNumber, Int16 speed, String rope, String score) {
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "athleteId", athleteId }
					, { "athleteName", athleteName }
					, { "athleteEvent", athleteEvent }
					, { "athleteCountry", athleteCountry.ToUpper() }
					, { "athleteRegion", athleteRegion.ToUpper() }
					, { "passNumber", passNumber }
					, { "speed", speed }
					, { "rope", rope }
					, { "score", score + "@" + speed.ToString() + "/" + rope }
				};
			addEwscMsg("scoring_score", JsonConvert.SerializeObject(sendMsg));
			return true;
		}

		public static Boolean sendRunningOrder(String curEvent, String eventGroup, Int16 round, String curSortCommand) {
			DataTable curDataTable = getRunningOrder(curEvent, eventGroup, round, curSortCommand);
			Dictionary<string, object>[] startListAthletes = new Dictionary<string, object>[curDataTable.Rows.Count];

			int curRow = 0;
			foreach ( DataRow curDataRow in curDataTable.Rows ) {
				String curFederation = (String)curDataRow["Federation"];
				if (curFederation.Length == 0) curFederation = (String)curDataRow["TourFederation"];

				String curValue = (String)curDataRow["memberId"];
				curValue = (String)curDataRow["SkierName"];
				curValue = curFederation.ToUpper();
				curValue = (String)curDataRow["State"];
				int curIntValue = Convert.ToInt32(curRow + 1);
				curIntValue = Convert.ToInt32(curDataTable.Rows.Count - curRow);
				curIntValue = Convert.ToInt32(curRow + 1);
				decimal curNumValue = (decimal)curDataRow["RankingScore"];
				curNumValue = Convert.ToDecimal(0);

				//	, { "division", (String)curDataRow["DivDesc"] }
				startListAthletes[curRow] = new Dictionary<string, object> {
                    { "athleteId", (String)curDataRow["memberId"] }
                    , { "athleteName", (String)curDataRow["SkierName"] }
					, { "athleteCountry", curFederation.ToUpper() }
                    , { "athleteRegion", (String)curDataRow["State"] }
                    , { "position_starting", Convert.ToInt32(curRow + 1) }
                    , { "position_seed", Convert.ToInt32(curDataTable.Rows.Count - curRow) }
                    , { "position_current", Convert.ToInt32(curRow + 1) }
                    , { "score_seed", (decimal)curDataRow["RankingScore"] }
                    , { "score_current", Convert.ToDecimal(0) }
                };

				curRow++;
			}

			Dictionary<string, object> startListMsg = new Dictionary<string, object> {
					{ "startlistName", "EventGroup " + eventGroup }
					, { "round", round }
					, { "eventName", "Slalom" }
					, { "division", eventGroup }
					, { "group", eventGroup }
					, { "lake", "" }
					, { "current_athlete_index", 0 }
					, { "current_athlete_name", "" }
					, { "startlist_athletes", startListAthletes }
				};

			addEwscMsg("start_list", JsonConvert.SerializeObject(startListMsg));
            return true;
        }

		public static Boolean sendOfficialsAssignments(String curEvent, String eventGroup, Int16 round ) {
			int towerJudgeIdx = 1;
			Boolean officialsAvailable = false;
			Dictionary<string, object> curEventOfficials = new Dictionary<string, object>();
			
			DataTable curDataTable = getOfficialAssignments(curEvent, eventGroup, round);
            foreach (DataRow curDataRow in curDataTable.Rows) {
				String curOfficialAsgmt = (String)curDataRow["WorkAsgmt"];
				if (curOfficialAsgmt.Equals("Driver")) {
					curEventOfficials.Add("driver", buildOfficialEntry( curDataRow ));
					officialsAvailable = true;

				} else if (curOfficialAsgmt.Equals("Event Judge")) {
					String judgeSuffix = "";
					if (towerJudgeIdx == 1) {
						judgeSuffix = "1a";
					} else if (towerJudgeIdx == 2) {
						judgeSuffix = "1b";
					} else if (towerJudgeIdx == 3) {
						judgeSuffix = "2a";
					} else if (towerJudgeIdx == 4) {
						judgeSuffix = "2b";
					}
					if (towerJudgeIdx <= 4) {
						curEventOfficials.Add("towerJudge" + judgeSuffix, buildOfficialEntry(curDataRow));
						officialsAvailable = true;
					}
					towerJudgeIdx++;

				} else if (curOfficialAsgmt.Equals("Boat Judge")) {
					curEventOfficials.Add("boatJudge", buildOfficialEntry(curDataRow));
					officialsAvailable = true;

				} else if (curOfficialAsgmt.Equals("Scorer")) {
					curEventOfficials.Add("scorer", buildOfficialEntry(curDataRow));
					officialsAvailable = true;
				}
			}
			if (officialsAvailable) {
				//String msg = JsonConvert.SerializeObject(curEventOfficials);
				addEwscMsg("officials_data", JsonConvert.SerializeObject(curEventOfficials));
				return true;
			}

			return false;

		}

		private static Dictionary<string, object> buildOfficialEntry(DataRow curDataRow) {
			String curFederation = "";
			try {
				curFederation = (String)curDataRow["Federation"];
				if (curFederation.Length == 0) curFederation = (String)curDataRow["TourFederation"];
			} catch {
				if (curFederation.Length == 0) curFederation = (String)curDataRow["TourFederation"];
			}
			Dictionary<string, object> curOfficial = new Dictionary<string, object> {
					{ "officialId", (String)curDataRow["MemberId"] }
					, { "officialName", (String)curDataRow["MemberName"] }
					, { "officialCountry", curFederation.ToUpper() }
					, { "officialRegion", (String)curDataRow["State"] } };
			return curOfficial;
		}

		public static Boolean sendExit() {
			DialogResult msgResp =
				MessageBox.Show("You have asked to close the connection to WaterSkiConnect", "Close Confirm",
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1);
			if (msgResp == DialogResult.OK) {
				addEwscMsg("Exit", "Exit WaterSkiConnect");
			}
			return true;
        }

        private static DataTable getRunningOrder(String curEvent, String eventGroup, Int16 round, String curSortCmd ) {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("SELECT E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, E.EventGroup");
			curSqlStmt.Append(", E.RankingScore, E.RankingRating, E.AgeGroup as Div, CodeValue as DivDesc ");
			curSqlStmt.Append(", T.State, T.City, T.Federation, X.Federation as TourFederation");
			curSqlStmt.Append(", COALESCE (S.Status, 'TBD') AS Status, S.Score");
			curSqlStmt.Append(", E.RunOrder, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
			curSqlStmt.Append("FROM EventReg E ");
			curSqlStmt.Append("     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup ");
			curSqlStmt.Append("     INNER JOIN Tournament AS X on X.SanctionId = E.SanctionId ");
			curSqlStmt.Append("     LEFT OUTER JOIN SlalomScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + round.ToString() + " ");
			curSqlStmt.Append("     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event ");
			curSqlStmt.Append("     LEFT OUTER JOIN (Select Distinct ListCode, CodeValue From CodeValueList Where ListName in ('AWSAAgeGroup', 'IwwfAgeGroup', 'CWSAAgeGroup', 'NcwsaAgeGroup')) ");
			curSqlStmt.Append("          DL ON DL.ListCode = E.AgeGroup ");
			curSqlStmt.Append("WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + curEvent + "' ");
			if (curSortCmd.Length > 0) curSqlStmt.Append("ORDER BY " + curSortCmd);

			DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
			curDataTable.DefaultView.Sort = curSortCmd;
			return curDataTable.DefaultView.ToTable();
		}

		private static DataTable getOfficialAssignments( String inEvent, String inEventGroup, Int16 inRound ) {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt");
			curSqlStmt.Append(", T.SkierName AS MemberName, T.State, T.Federation, X.Federation as TourFederation ");
			curSqlStmt.Append("FROM OfficialWorkAsgmt O ");
			curSqlStmt.Append("     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName, State, Federation From TourReg ) T ");
			curSqlStmt.Append("        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId ");
			curSqlStmt.Append("     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt ");
			curSqlStmt.Append("     INNER JOIN Tournament AS X on X.SanctionId = O.SanctionId ");
			curSqlStmt.Append("WHERE O.SanctionId = '" + mySanctionNum + "' ");
			curSqlStmt.Append("  AND O.Event = '" + inEvent + "' ");
			curSqlStmt.Append("  AND O.EventGroup = '" + inEventGroup + "' ");
			curSqlStmt.Append("  AND O.Round = " + inRound + " ");
			curSqlStmt.Append("ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName");
			return DataAccess.getDataTable(curSqlStmt.ToString());
		}

		private static DataTable getDriverAssignment(String inEvent, String inEventGroup, String inRound) {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt");
			curSqlStmt.Append(", T.SkierName AS MemberName, T.State, T.Federation, X.Federation as TourFederation ");
			curSqlStmt.Append("FROM OfficialWorkAsgmt O ");
			curSqlStmt.Append("     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName, State, Federation From TourReg ) T ");
			curSqlStmt.Append("        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId ");
			curSqlStmt.Append("     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt ");
			curSqlStmt.Append("     INNER JOIN Tournament AS X on X.SanctionId = O.SanctionId ");
			curSqlStmt.Append("WHERE O.SanctionId = '" + mySanctionNum + "' ");
			curSqlStmt.Append("  AND O.Event = '" + inEvent + "' ");
			curSqlStmt.Append("  AND O.EventGroup = '" + inEventGroup + "' ");
			curSqlStmt.Append("  AND O.Round = " + inRound + " ");
			curSqlStmt.Append("  AND O.WorkAsgmt = 'Driver' ");
			curSqlStmt.Append("ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName");
			return DataAccess.getDataTable(curSqlStmt.ToString());
		}

		private static void getSanctionNum() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if (mySanctionNum == null) {
                String msg = "An active tournament must be selected from the Administration menu Tournament List option";
                MessageBox.Show(msg);
                throw new Exception(msg);

            } else if (mySanctionNum.Length < 6) {
                String msg = "An active tournament must be selected from the Administration menu Tournament List option";
                MessageBox.Show(msg);
                throw new Exception(msg);

            } else {
                //Retrieve selected tournament attributes
                DataTable curTourDataTable = getTourData();
                myTourRow = curTourDataTable.Rows[0];
            }
        }

        private static void addEwscMsg(String msgType, String msgData ) {
            char[] singleQuoteDelim = new char[] { '\'' };
            String curMsgData = stringReplace(msgData, singleQuoteDelim, "''");

            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("Insert EwscMsg ( ");
            curSqlStmt.Append("SanctionId, MsgType, MsgData, CreateDate ");
            curSqlStmt.Append(") Values ( ");
            curSqlStmt.Append("'" + mySanctionNum + "'");
            curSqlStmt.Append(", '" + msgType + "'");
            curSqlStmt.Append(", '" + msgData + "'");
            curSqlStmt.Append(", getdate()");
            curSqlStmt.Append(")");
            int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
			Log.WriteFile(curSqlStmt.ToString());
		}

		//
		private static void addEwscListenMsg(String msgType, String msgData) {
			char[] singleQuoteDelim = new char[] { '\'' };
			String curMsgData = stringReplace(msgData, singleQuoteDelim, "''");

			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("Insert EwscListenMsg ( ");
			curSqlStmt.Append("SanctionId, MsgType, MsgData, CreateDate ");
			curSqlStmt.Append(") Values ( ");
			curSqlStmt.Append("'" + mySanctionNum + "'");
			curSqlStmt.Append(", '" + msgType + "'");
			curSqlStmt.Append(", '" + msgData + "'");
			curSqlStmt.Append(", getdate()");
			curSqlStmt.Append(")");
			int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
			Log.WriteFile(curSqlStmt.ToString());
		}

		private static void removeEwscMsg( int pkid ) {
            StringBuilder curSqlStmt = new StringBuilder("Delete FROM EwscMsg Where PK = " + pkid);
            int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
        }

        private static DataTable getEwscMsg() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT PK, SanctionId, MsgType, MsgData ");
            curSqlStmt.Append("FROM EwscMsg M ");
            curSqlStmt.Append("WHERE M.SanctionId = '" + mySanctionNum + "' ");
            return DataAccess.getDataTable(curSqlStmt.ToString());
        }

        private static DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation");
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation");
            curSqlStmt.Append(", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' ");
            return DataAccess.getDataTable(curSqlStmt.ToString());
        }

        public static String stringReplace(String inValue, char[] inCurValue, String inReplValue) {
            StringBuilder curNewValue = new StringBuilder("");

            String[] curValues = inValue.Split(inCurValue);
            if (curValues.Length > 1) {
                int curCount = 0;
                foreach (String curValue in curValues) {
                    curCount++;
                    if (curCount < curValues.Length) {
                        curNewValue.Append(curValue + inReplValue);
                    } else {
                        curNewValue.Append(curValue);
                    }
                }
            } else {
                curNewValue.Append(inValue);
            }

            return curNewValue.ToString();
        }
    }
}
