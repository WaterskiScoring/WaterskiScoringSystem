using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Slalom {
	class SlalomEventData {
		public static String mySanctionNum;
		public static String myTourClass;
		public static String myTourRules;

		public static DataRow myTourRow;
		public static DataRow myClassCRow;
		public static DataRow myClassERow;
		public static DataRow myClassRowTour;

		public static DataTable myTimesDataTable;
		public static DataTable myBoatPathDevMax;
		public static DataTable myDivisionIntlDataTable;
		public static DataTable mySlalomAltScoreMethodDataTable;

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
			if ( (byte)myTourRow["SlalomRounds"] <= 0 ) {
				MessageBox.Show( "The Slalom event is not defined for the active tournament" );
				return false;
			}

			myTourRules = (String)myTourRow["Rules"];
			myTourClass = myTourRow["Class"].ToString().ToUpper();

			mySkierClassList = new ListSkierClass();
			mySkierClassList.ListSkierClassLoad();

			myClassRowTour = mySkierClassList.SkierClassDataTable.Select( "ListCode = '" + myTourRow["EventScoreClass"].ToString().ToUpper() + "'" )[0];
			myClassCRow = mySkierClassList.SkierClassDataTable.Select( "ListCode = 'C'" )[0];
			myClassERow = mySkierClassList.SkierClassDataTable.Select( "ListCode = 'E'" )[0];

			//Instantiate object for checking for records
			myCheckEventRecord = new CheckEventRecord( myTourRow );

			//Age group list
			myAgeDivList = new AgeGroupDropdownList( myTourRow );

			//Retrieve boat times
			myTimesDataTable = getBoatTimes();

			//Retrieve boat path deviation criteria
			myBoatPathDevMax = getBoatPathDevMax();

			return true;
		}

		/*
		 * Use class R times for all Classes greater than class C (e.g. class E, L, X, etc..)
		 * Use class C times for all Classes less than or equal to class C (e.g. class C, N, F, G)
		 */
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

		public static bool isCollegiateEvent() {
			return HelperFunctions.isCollegiateEvent( myTourRules );
		}
		public static bool isIwwfEvent() {
			return HelperFunctions.isIwwfEvent( myTourRules );
		}

		public static Boolean isDivisionIntl( String inAgeGroup ) {
			if ( myDivisionIntlDataTable == null ) {
				StringBuilder curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT Distinct ListName, ListCode as Division, CodeValue as DivisionName " );
				curSqlStmt.Append( "FROM CodeValueList " );
				curSqlStmt.Append( "WHERE ListName = 'IwwfAgeGroup'" );
				myDivisionIntlDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			}
			
			if ( myDivisionIntlDataTable == null ) return false;
			DataRow[] curRowsFound = myDivisionIntlDataTable.Select( "Division = '" + inAgeGroup + "'" );
			if ( curRowsFound.Length > 0 ) return true;
			return false;
		}

		public static String[] buildScoreExport( String curRound, String curEventGroup, String curFilterCmd ) {
			String[] curSelectCommand = new String[10];
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
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Slalom' "
				+ "Where XT.SanctionId = '" + SlalomEventData.mySanctionNum + "' ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "Select * from EventReg XT "
				+ " Where SanctionId = '" + SlalomEventData.mySanctionNum + "'"
				+ " And Event = 'Slalom'";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "Select * from EventRunOrder XT "
				+ " Where SanctionId = '" + SlalomEventData.mySanctionNum + "'"
				+ " And Event = 'Slalom' And Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "SELECT XT.* FROM SlalomScore XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Slalom' "
				+ "Where XT.SanctionId = '" + SlalomEventData.mySanctionNum + "' And Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "SELECT XT.* FROM SlalomRecap XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Slalom' "
				+ "Where XT.SanctionId = '" + SlalomEventData.mySanctionNum + "' And Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			//----------------------------------------
			//Export data related to officials
			//----------------------------------------
			curIdx++;
			curSelectCommand[curIdx] = "SELECT XT.* FROM TourReg XT "
				+ "INNER JOIN OfficialWorkAsgmt ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Slalom' AND ER.Round in (" + curRound + ", 25) "
				+ "Where XT.SanctionId = '" + SlalomEventData.mySanctionNum + "' ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "SELECT XT.* FROM OfficialWork XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Slalom' "
				+ "Where XT.SanctionId = '" + SlalomEventData.mySanctionNum + "' And XT.LastUpdateDate is not null ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";
			curSelectCommand[curIdx] = curSelectCommand[curIdx] + "Union "
				+ "SELECT XT.* FROM OfficialWork XT "
				+ "INNER JOIN OfficialWorkAsgmt ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Slalom' AND ER.Round in (" + curRound + ", 25) "
				+ "Where XT.SanctionId = '" + SlalomEventData.mySanctionNum + "' And XT.LastUpdateDate is not null ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "Select * from OfficialWorkAsgmt "
				+ " Where SanctionId = '" + SlalomEventData.mySanctionNum + "' And Event = 'Slalom' And Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";

			//----------------------------------------
			//Export data provided by boat path measurement system using Waterski Connect
			//----------------------------------------
			curIdx++;
			curSelectCommand[curIdx] = "SELECT BT.* FROM BoatTime BT "
				+ "INNER JOIN  SlalomScore XT on BT.SanctionId = XT.SanctionId AND BT.MemberId = XT.MemberId AND BT.Round = XT.Round AND BT.Event = 'Slalom' "
				+ "INNER JOIN EventReg ER on BT.SanctionId = ER.SanctionId AND BT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Slalom' "
				+ "Where BT.SanctionId = '" + SlalomEventData.mySanctionNum + "' AND BT.Event = 'Slalom' And BT.Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			curIdx++;
			curSelectCommand[curIdx] = "SELECT BT.* FROM BoatPath BT "
				+ "INNER JOIN  SlalomScore XT on BT.SanctionId = XT.SanctionId AND BT.MemberId = XT.MemberId AND BT.Round = XT.Round AND BT.Event = 'Slalom' "
				+ "INNER JOIN EventReg ER on BT.SanctionId = ER.SanctionId AND BT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Slalom' "
				+ "Where BT.SanctionId = '" + SlalomEventData.mySanctionNum + "' AND BT.Event = 'Slalom' And BT.Round in (" + curRound + ", 25) ";
			if ( HelperFunctions.isObjectPopulated( tmpFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + tmpFilterCmd + " ";
			if ( HelperFunctions.isObjectPopulated( curFilterCmd ) ) curSelectCommand[curIdx] = curSelectCommand[curIdx] + "And " + curFilterCmd + " ";

			return curSelectCommand;
		}

		/*
		 * Determine if skier division and class qualify the skier to use the alternate scoring method.
		 * This alternate scoring method no longer requires that the skier be scored at long line when at less than max speed
		 * Rule 10.06 for ski year 2017
		 * Also Collegiate divisions
		 */
		public static Boolean isQualifiedAltScoreMethod( String inAgeGroup, String inSkierClass ) {
			if ( mySlalomAltScoreMethodDataTable == null ) {
				StringBuilder curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT Distinct ListName, ListCode, SortSeq, CodeValue as Description " );
				curSqlStmt.Append( "FROM CodeValueList " );
				curSqlStmt.Append( "WHERE ListName = 'SlalomAltScoreMethodQual'" );
				mySlalomAltScoreMethodDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			}
			if ( mySlalomAltScoreMethodDataTable == null ) return false;

			DataRow[] curRowsFound = mySlalomAltScoreMethodDataTable.Select( "ListCode = '" + inAgeGroup + "'" );
			if ( curRowsFound.Length > 0 ) return true;

			curRowsFound = mySlalomAltScoreMethodDataTable.Select( "ListCode = '" + inAgeGroup + "-" + inSkierClass + "'" );
			if ( curRowsFound.Length > 0 ) return true;
			return false;
		}

		/*
		 * Determine required number of judges for event
		 */
		public static Int16 getJudgeCountByClass() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select ListCode, CodeValue, MaxValue, MinValue FROM CodeValueList " );
			curSqlStmt.Append( "Where ListName = 'SlalomJudgesNum' And ListCode = '" + myTourClass + "' ORDER BY SortSeq" );
			DataTable curNumJudgesDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			if ( curNumJudgesDataTable.Rows.Count > 0 ) {
				return Convert.ToInt16( (Decimal)curNumJudgesDataTable.Rows[0]["MaxValue"] );
			}
			return 1;
		}

		private static DataTable getTourData( String inSanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, SanctionEditCode, ContactMemberId, Name" );
			curSqlStmt.Append( ", Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, SlalomRounds, Rules, EventDates, EventLocation " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			return curDataTable;
		}

		public static DataTable getBoatTimes() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode, ListCodeNum, SortSeq, CodeValue, MinValue, MaxValue, CodeDesc " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = 'SlalomBoatTime' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public static DataTable getBoatPathDevMax() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode as Buoy, ListCodeNum as BuoyNum, CodeValue, MinValue as BuoyDev, MaxValue as CumDev, CodeDesc " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = 'SlalomBoatPathDevMax' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}
		
		public static DataRow getBoatPathDevMaxRow( int inBuoyNum, String inSkierClass ) {
			if ( inBuoyNum > 6 ) return null;
			DataRow[] curFilterRows;
			if ( inBuoyNum == 0 ) {
				curFilterRows = myBoatPathDevMax.Select( String.Format( "Buoy = 'GATE-{0}'", inSkierClass ) );
			} else {
				curFilterRows = myBoatPathDevMax.Select( String.Format( "Buoy = 'B{0}-{1}'", inBuoyNum, inSkierClass ) );
			}
			if ( curFilterRows.Length > 0 ) return curFilterRows[0];

			String curMsg = String.Format( "Slalom: ScoreEntry: getBoatPathDevMaxRow: "
				+ "Boat path deviation max entry not for buoy '{0}' and skier class '{1}'"
				, inBuoyNum, inSkierClass );
			Log.WriteFile( curMsg );
			MessageBox.Show( curMsg );
			return null;
		}

		public static DataRow getSlalomBoatPathRerideRopeMin( String curSkierClass ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode as Class, CodeValue, MinValue as MinRopeLength, CodeDesc " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = 'SlalomBoatPathRerideRopeMin' " );
			curSqlStmt.Append( String.Format( "AND ListCode = '{0}'", curSkierClass ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable != null && curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];
			return null; ;
		}

		public static Int16 getMaxSpeedOrigData( String inAgeGroup, Int16 inMaxSpeedKph ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode as Division, ListCodeNum, CodeValue as MaxSpeedDesc, MinValue as MaxSpeedMph, MaxValue as MaxSpeedKph " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName like '%SlalomMaxDiv%' AND ListCode = '" + inAgeGroup + "' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			DataTable curMaxSpeedDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curMaxSpeedDataTable.Rows.Count > 0 ) return Convert.ToInt16( (Decimal)curMaxSpeedDataTable.Rows[0]["MaxSpeedKph"] );
			
			return getMaxSpeedData( inAgeGroup, inMaxSpeedKph );
		}

		public static Int16 getMaxSpeedData( String inAgeGroup, Int16 inMaxSpeedKph ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode as Division, ListCodeNum, CodeValue as MaxSpeedDesc, MinValue as MaxSpeedMph, MaxValue as MaxSpeedKph " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName like '%SlalomMax' AND ListCode = '" + inAgeGroup + "' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			DataTable curMaxSpeedDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curMaxSpeedDataTable.Rows.Count > 0 ) return Convert.ToInt16( (Decimal)curMaxSpeedDataTable.Rows[0]["MaxSpeedKph"] );

			return inMaxSpeedKph;
		}

		public static DataTable getMinSpeedData( String inAgeGroup ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );

			curSqlStmt.Append( "SELECT ListCode as Division, ListCodeNum, CodeValue as MaxSpeedDesc, MinValue as NumPassMinSpeed, MaxValue as MaxSpeedKph " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName like '%SlalomMin' AND ListCode = '" + inAgeGroup + "' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );

			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public static DataRow getPassRow( Int16 inSpeed, decimal inPassLine ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT " );
			curSqlStmt.Append( "SS.MaxValue as SpeedKph, SS.MinValue as SpeedMph, SS.CodeValue as SpeedMphDesc, SS.ListCode as SpeedKphDesc" );
			curSqlStmt.Append( ", SL.MaxValue as LineLengthMeters, SL.MinValue as LineLengthOff, SL.CodeValue as LineLengthOffDesc, SL.ListCode as LineLengthMetersDesc" );
			curSqlStmt.Append( ", (12 - SS.SortSeq + 1) as SlalomSpeedNum, (SL.SortSeq - 1) as SlalomLineNum " );
			curSqlStmt.Append( "FROM CodeValueList as SS, CodeValueList as SL " );
			curSqlStmt.Append( "Where SS.ListName = 'SlalomSpeeds' AND SL.ListName = 'SlalomLines' " );
			curSqlStmt.Append( String.Format( "AND SL.MaxValue = {0} AND SS.MaxValue = {1}.0 ", inPassLine, inSpeed ) );
			curSqlStmt.Append( "Order by SS.SortSeq, SL.SortSeq" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];

			MessageBox.Show( String.Format( "Slalom pass not found for KPH={0} Meters={1}", inSpeed, inPassLine ) );
			return null;
		}
		
		public static DataRow getPassRow( int inSpeedNum, int inLineNum ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT " );
			curSqlStmt.Append( "SS.MaxValue as SpeedKph, SS.MinValue as SpeedMph, SS.CodeValue as SpeedMphDesc, SS.ListCode as SpeedKphDesc" );
			curSqlStmt.Append( ", SL.MaxValue as LineLengthMeters, SL.MinValue as LineLengthOff, SL.CodeValue as LineLengthOffDesc, SL.ListCode as LineLengthMetersDesc" );
			curSqlStmt.Append( ", (12 - SS.SortSeq + 1) as SlalomSpeedNum, (SL.SortSeq - 1) as SlalomLineNum " );
			curSqlStmt.Append( "FROM CodeValueList as SS, CodeValueList as SL " );
			curSqlStmt.Append( "Where SS.ListName = 'SlalomSpeeds' AND SL.ListName = 'SlalomLines' " );
			curSqlStmt.Append( String.Format( "AND SL.SortSeq = {0} AND SS.SortSeq = {1} ", (inLineNum + 1), ( 12 - inSpeedNum + 1 ) ) );
			curSqlStmt.Append( "Order by SS.SortSeq, SL.SortSeq" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];

			MessageBox.Show( String.Format( "Slalom pass not found for KPH SortSeq={0} MetersSortSeq={1}", inSpeedNum, inLineNum ) );
			return null;
		}

	}
}
