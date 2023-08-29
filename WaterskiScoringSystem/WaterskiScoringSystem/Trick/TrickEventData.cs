using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Trick {
	class TrickEventData {
		public static String mySanctionNum;
		public static String myTourClass;
		public static String myTourRules;

		public static Int16 myNumJudges;

		public static DataRow myTourRow;
		public static DataRow myClassCRow;
		public static DataRow myClassERow;

		public static ListSkierClass mySkierClassList;
		public static CheckEventRecord myCheckEventRecord;
		public static AgeGroupDropdownList myAgeDivList;

		public static bool setEventData() {
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			DataTable curTourDataTable = getTourData( mySanctionNum );
			if ( curTourDataTable.Rows.Count <= 0 ) {
				MessageBox.Show( "An active tournament is not properly defined" );
				return false;
			}
			myTourRow = curTourDataTable.Rows[0];
			if ( (byte)myTourRow["TrickRounds"] <= 0 ) {
				MessageBox.Show( "The Trick event is not defined for the active tournament" );
				return false;
			}

			myTourRules = (String)myTourRow["Rules"];
			myTourClass = myTourRow["Class"].ToString().ToUpper();

			mySkierClassList = new ListSkierClass();
			mySkierClassList.ListSkierClassLoad();

			myClassCRow = mySkierClassList.SkierClassDataTable.Select( "ListCode = 'C'" )[0];
			myClassERow = mySkierClassList.SkierClassDataTable.Select( "ListCode = 'E'" )[0];

			//Instantiate object for checking for records
			myCheckEventRecord = new CheckEventRecord( myTourRow );

			//Age group list
			myAgeDivList = new AgeGroupDropdownList( myTourRow );

			myNumJudges = getNumTrickJudges();

			return true;
		}

		public static bool isCollegiateEvent() {
			return HelperFunctions.isCollegiateEvent( myTourRules );
		}
		public static bool isIwwfEvent() {
			return HelperFunctions.isIwwfEvent( myTourRules );
		}

		public static DataRow getSkierClass( String inEventClass ) {
			String curSkierClass = myTourClass;
			DataRow curClassRowSkier = getClassRow( inEventClass );
			if ( curClassRowSkier == null ) return getClassRow( myTourClass );
			if ( (Decimal)curClassRowSkier["ListCodeNum"] > (Decimal)myClassCRow["ListCodeNum"] ) return getClassRow( "R" );
			return curClassRowSkier;
		}

		public static DataRow getClassRow( String inClass ) {
			DataRow[] curRowsFound = mySkierClassList.SkierClassDataTable.Select( String.Format( "ListCode = '{0}'", inClass ) );
			if ( curRowsFound.Length > 0 ) return curRowsFound[0];
			return null;
		}

		public static String[] buildScoreExport( String curRound, String curEventGroup, String inFilterCmd ) {
			String[] curSelectCommand = new String[8];
			String curFilterCmd = inFilterCmd;
			if ( curFilterCmd.Contains( "Div =" ) ) {
				curFilterCmd = curFilterCmd.Replace( "Div =", "XT.AgeGroup =" );

			} else if ( curFilterCmd.Contains( "AgeGroup not in" ) ) {
				curFilterCmd = curFilterCmd.Replace( "AgeGroup not in", "XT.AgeGroup not in" );

			} else if ( curFilterCmd.Contains( "AgeGroup =" ) ) {
				curFilterCmd = curFilterCmd.Replace( "AgeGroup =", "XT.AgeGroup =" );
			}

			String tmpFilterCmd = "";
			if ( !( curEventGroup.ToLower().Equals( "all" ) ) ) tmpFilterCmd = "And EventGroup = '" + curEventGroup + "' ";

			int curIdx = 0;
			curSelectCommand[curIdx] = "SELECT XT.* FROM TourReg XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Trick' "
				+ "Where XT.SanctionId = '" + TrickEventData.mySanctionNum + "' ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "Select * from EventReg XT "
				+ " Where SanctionId = '" + TrickEventData.mySanctionNum + "'"
				+ " And Event = 'Trick'";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "Select * from EventRunOrder XT "
				+ " Where SanctionId = '" + TrickEventData.mySanctionNum + "'"
				+ " And Event = 'Trick' And Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "SELECT XT.* FROM TrickScore XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Trick' "
				+ "Where XT.SanctionId = '" + TrickEventData.mySanctionNum + "' And Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "SELECT XT.* FROM TrickPass XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Trick' "
				+ "Where XT.SanctionId = '" + TrickEventData.mySanctionNum + "' And Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			//----------------------------------------
			//Export data related to officials
			//----------------------------------------
			curIdx++;
			curSelectCommand[curIdx] = "SELECT XT.* FROM TourReg XT "
				+ "INNER JOIN OfficialWorkAsgmt ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Trick' AND ER.Round in (" + curRound + ", 25) "
				+ "Where XT.SanctionId = '" + TrickEventData.mySanctionNum + "' ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "SELECT XT.* FROM OfficialWork XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Trick' "
				+ "Where XT.SanctionId = '" + TrickEventData.mySanctionNum + "' And XT.LastUpdateDate is not null ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";
			curSelectCommand[curIdx] = curSelectCommand[curIdx] + "Union "
				+ "SELECT XT.* FROM OfficialWork XT "
				+ "INNER JOIN OfficialWorkAsgmt ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Trick' AND ER.Round in (" + curRound + ", 25) "
				+ "Where XT.SanctionId = '" + TrickEventData.mySanctionNum + "' And XT.LastUpdateDate is not null ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "Select * from OfficialWorkAsgmt "
				+ " Where SanctionId = '" + TrickEventData.mySanctionNum + "' And Event = 'Trick' And Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			return curSelectCommand;
		}

		private static DataTable getTourData( String inSanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, SanctionEditCode, ContactMemberId, Name" );
			curSqlStmt.Append( ", Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
			DataTable curDataTable = Common.DataAccess.getDataTable( curSqlStmt.ToString() );
			return curDataTable;
		}

		private static Int16 getNumTrickJudges() {
			//Determine required number of judges for event
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select ListCode, CodeValue, MaxValue, MinValue FROM CodeValueList " );
			curSqlStmt.Append( "Where ListName = 'TrickJudgesNum' And ListCode = '" + TrickEventData.myTourClass + "' ORDER BY SortSeq" );
			DataTable curNumJudgesDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curNumJudgesDataTable.Rows.Count > 0 ) return Convert.ToInt16( (Decimal)curNumJudgesDataTable.Rows[0]["MaxValue"] );
			return 1;
		}

	}
}
