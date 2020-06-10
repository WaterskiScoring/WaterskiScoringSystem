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

using WaterskiScoringSystem.Common;


namespace WaterskiScoringSystem.Tools {
	class EwscMonitor {
        private static String mySanctionNum;

        private static DataRow myTourRow;

        private static String ewcsUri = "http://ewscdata.com:40000/";
		private static Quobject.SocketIoClientDotNet.Client.Socket socketClient = null;


		public static void startEwscMonitoring() {
            getSanctionNum();

            Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
                    { "loggingdetail", "no" }
                    , { "mode", "Tournament" }
                    , { "eventid", mySanctionNum }
                    , { "provider", Properties.Settings.Default.AppTitle }
                    , { "application", Properties.Settings.Default.AppVersion }
                    , { "version", Properties.Settings.Default.AppVersion }
                    , { "username", "mawsa@comcast.net" }
                };
        String jsonData = JsonConvert.SerializeObject(sendConnectionMsg);

            socketClient = IO.Socket(ewcsUri);

            socketClient.On(Socket.EVENT_CONNECT, () => {
                Console.WriteLine(String.Format("Connected to {0}", ewcsUri));
                socketClient.Emit("manual_connection_parameter", jsonData);
                Console.WriteLine(String.Format("manual_connection_parameter confirm sent with parameters... {0}", jsonData));
            });

            socketClient.On("connect_confirm", (data) => {
                Console.WriteLine(String.Format("connect_confirm: {0}", data));
            });

            socketClient.On("boat_times", (data) => {
                Console.WriteLine(String.Format("boat_times {0}", data));
            });

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

    }
}
