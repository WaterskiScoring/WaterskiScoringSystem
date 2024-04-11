using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Jump {
	class JumpEventData {
		public static String mySanctionNum;
		public static String myTourClass;
		public static String myTourRules;

		public static DataRow myTourRow;
		public static DataRow myClassCRow;
		public static DataRow myClassERow;
		public static DataRow myClassRowTour;

		public static DataTable myMinSpeedDataTable;
		public static DataTable myMaxSpeedDataTable;
		public static DataTable myMaxRampDataTable;
		public static DataTable myTimesDataTable;
		public static DataTable myBoatPathDevMax;
		public static DataTable myJump3TimesDivDataTable;

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
			if ( (byte)myTourRow["JumpRounds"] <= 0 ) {
				MessageBox.Show( "The jump event is not defined for the active tournament" );
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

			//Retrieve maximum speeds by age group
			myMaxSpeedDataTable = getMaxSpeedData();

			//Retrieve minimum speeds by class
			myMinSpeedDataTable = getMinSpeedData();

			//Retrieve maximum ramp heights by age group
			myMaxRampDataTable = getMaxRampData();

			//Retrieve boat path deviation criteria
			myBoatPathDevMax = getBoatPathDevMax();

			//Retrieve list of divisions and scores that require 3 segment validation for L and R tournaments
			try {
				DataTable curTourClassDataTable = mySkierClassList.getTourClassList();
				if ( (Decimal)myClassRowTour["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"] ) {
					myJump3TimesDivDataTable = getJump3TimesDivData();
				} else {
					myJump3TimesDivDataTable = null;
				}
			} catch {
				myJump3TimesDivDataTable = null;
			}

			return true;
		}

		/* 
		 * For class L,R tournaments get IWWF equivalent divisions
		 */
		public static DataRow[] getJump3TimesDiv( String inDiv, Int16 inSkiYearAge ) {
			if ( myJump3TimesDivDataTable == null ) return new DataRow[0];

			DataRow[] returnJump3TimesDiv = null;
			returnJump3TimesDiv = myJump3TimesDivDataTable.Select( "ListCode = '" + inDiv + "'" );
			if ( returnJump3TimesDiv.Length > 0 ) return returnJump3TimesDiv;

			ArrayList curIwwfEligDiv = new ArrayList();
			Int16 curSkiYearAge = inSkiYearAge;
			if ( curSkiYearAge == 0 ) curSkiYearAge = (short)myAgeDivList.getMinAgeForDiv( inDiv );
			if ( curSkiYearAge > 0 ) curIwwfEligDiv = myAgeDivList.getDivListForAgeIwwf( curSkiYearAge, inDiv );

			if ( curIwwfEligDiv.Count > 0 ) {
				// Search eligible divisions for elite division with shortest required distance
				foreach ( String curEligDiv in curIwwfEligDiv ) {
					String curKey = String.Format( "ListCode = '{0}'", curEligDiv.Substring( 0, 2 ) );
					DataRow[] curRowsFound = myJump3TimesDivDataTable.Select( curKey );
					if ( curRowsFound.Length > 0 ) {
						if ( returnJump3TimesDiv.Length > 0 ) {
							if ( (Decimal)returnJump3TimesDiv[0]["MaxValue"] > (Decimal)curRowsFound[0]["MaxValue"] ) {
								returnJump3TimesDiv = myJump3TimesDivDataTable.Select( curKey );
							}
						} else {
							returnJump3TimesDiv = myJump3TimesDivDataTable.Select( curKey );
						}
					}
				}
				return returnJump3TimesDiv;
			}

			return new DataRow[0];
		}

		/*
		 * Use class R times for all Classes greater than class C (e.g. class E, L, X, etc..)
		 * Use class C times for all Classes less than or equal to class C (e.g. class C, N, F, G)
		 */
		public static DataRow getSkierClass( String inEventClass ) {
			String curSkierClass = myTourClass;
			DataRow curClassRowSkier = getClassRow(inEventClass);
			if ( curClassRowSkier == null ) return getClassRow( myTourClass );
			if ( (Decimal)curClassRowSkier["ListCodeNum"] > (Decimal)myClassCRow["ListCodeNum"] ) return getClassRow( "R" );
			return curClassRowSkier;
		}

		public static DataRow getClassRow( String inClass ) {
			DataRow[] curRowsFound = mySkierClassList.SkierClassDataTable.Select( String.Format( "ListCode = '{0}'", inClass ) );
			if ( curRowsFound.Length > 0 ) return curRowsFound[0];
			return null;
		}

		public static Int16 getMaxDivSpeed( String inAgeGroup ) {
			DataRow[] curFoundRows = JumpEventData.myMaxSpeedDataTable.Select( String.Format( "ListCode = '{0}'", inAgeGroup ) );
			if ( curFoundRows.Length == 0 ) {
				MessageBox.Show( String.Format( "JumpEventData:getMaxDivSpeed: Max speed entry not found for divsion {0}", inAgeGroup ) );
				return 0;
			}
			return Convert.ToInt16( (Decimal)curFoundRows[0]["MaxValue"] );
		}

		public static bool isCollegiateEvent() {
			return HelperFunctions.isCollegiateEvent( myTourRules );
		}
		public static bool isIwwfEvent() {
			return HelperFunctions.isIwwfEvent( myTourRules );
		}

		public static DataTable getDataForMeters() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue as TriangleFeet FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName in ('JumpMeter6Tol', 'JumpTriangle', 'JumpTriangleZero') " );
			curSqlStmt.Append( "And ListCode = '" + JumpEventData.myTourClass + "' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );

		}

		public static String[] buildScoreExport(String curRound, String curEventGroup, bool isNcwsa ) {
			String[] curSelectCommand = new String[11];
			String tmpFilterCmd = "", tmpFilterCmd2 = "";
			if ( isNcwsa ) {
				if ( !( curEventGroup.ToLower().Equals( "all" ) ) ) {
					tmpFilterCmd = "And XT.AgeGroup = '" + curEventGroup + "' ";
					tmpFilterCmd2 = "And XT.EventGroup = '" + HelperFunctions.getEventGroupOfficialAsgmtNcwsa( curEventGroup ) + "' ";

				}
			} else {
				if ( !( curEventGroup.ToLower().Equals( "all" ) ) ) {
					tmpFilterCmd = "And EventGroup = '" + curEventGroup + "' ";
					tmpFilterCmd2 = tmpFilterCmd;
				}
			}

			int curIdx = 0;
			curSelectCommand[curIdx] = String.Format( "SELECT XT.* FROM TourReg XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
				+ "Where XT.SanctionId = '{0}' {1} ", JumpEventData.mySanctionNum, tmpFilterCmd );

			curIdx++;
			curSelectCommand[curIdx] = String.Format( "Select * from EventReg XT Where SanctionId = '{0}' And Event = 'Jump' {1} ", JumpEventData.mySanctionNum, tmpFilterCmd );

			curIdx++;
			curSelectCommand[curIdx] = String.Format( "Select * from EventRunOrder XT Where SanctionId = '{0}' And Event = 'Jump' And Round in ({1}, 25) {2} ", JumpEventData.mySanctionNum, curRound, tmpFilterCmd );

			curIdx++;
			curSelectCommand[curIdx] = String.Format( "SELECT XT.* FROM JumpScore XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
				+ "Where XT.SanctionId = '{0}' And Round in ({1}, 25) {2} ", JumpEventData.mySanctionNum, curRound, tmpFilterCmd );

			curIdx++;
			curSelectCommand[curIdx] = String.Format( "SELECT XT.* FROM JumpRecap XT "
				+ "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
				+ "Where XT.SanctionId = '{0}' And Round in ({1}, 25) {2} ", JumpEventData.mySanctionNum, curRound, tmpFilterCmd );

			//----------------------------------------
			//Export data related to officials
			//----------------------------------------
			curIdx++;
			curSelectCommand[curIdx] = String.Format( "SELECT TR.* FROM TourReg TR "
				+ "INNER JOIN OfficialWorkAsgmt XT on XT.SanctionId = TR.SanctionId AND XT.MemberId = TR.MemberId AND XT.Event = 'Jump' "
				+ "Where TR.SanctionId = '{0}' And Round in ({1}, 25) {2} ", JumpEventData.mySanctionNum, curRound, tmpFilterCmd2 );

			curIdx++;
			curSelectCommand[curIdx] = String.Format( "SELECT OW.* FROM OfficialWork OW "
				+ "INNER JOIN EventReg XT on XT.SanctionId = OW.SanctionId AND XT.MemberId = OW.MemberId AND XT.Event = 'Jump' "
				+ "Where OW.SanctionId = '{0}' And OW.LastUpdateDate is not null {1}", JumpEventData.mySanctionNum, tmpFilterCmd )
				+ String.Format( " Union "
				+ "SELECT OW.* FROM OfficialWork OW "
				+ "INNER JOIN OfficialWorkAsgmt XT on XT.SanctionId = OW.SanctionId AND XT.MemberId = OW.MemberId AND XT.Event = 'Jump' "
				+ "Where OW.SanctionId = '{0}' And OW.LastUpdateDate is not null And Round in ({1}, 25) {2} ", JumpEventData.mySanctionNum, curRound, tmpFilterCmd2 );

			curIdx++;
			curSelectCommand[curIdx] = String.Format( "Select * FROM OfficialWorkAsgmt XT Where XT.SanctionId = '{0}' And Event = 'Jump' And Round in ({1}, 25) {2} "
				, JumpEventData.mySanctionNum, curRound, tmpFilterCmd2 );

			//----------------------------------------
			//Export data provided by boat path measurement system using Waterski Connect
			//----------------------------------------
			curIdx++;
			curSelectCommand[curIdx] = String.Format( "SELECT BT.* FROM BoatTime BT "
				+ "INNER JOIN  JumpScore XT on BT.SanctionId = XT.SanctionId AND BT.MemberId = XT.MemberId AND BT.Round = XT.Round AND BT.Event = 'Jump' "
				+ "INNER JOIN EventReg ER on BT.SanctionId = ER.SanctionId AND BT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
				+ "Where BT.SanctionId = '{0}' AND BT.Event = 'Jump' And BT.Round in ({1}, 25) {2} ", JumpEventData.mySanctionNum, curRound, tmpFilterCmd );

			curIdx++;
			curSelectCommand[curIdx] = String.Format( "SELECT BT.* FROM BoatPath BT "
				+ "INNER JOIN  JumpScore XT on BT.SanctionId = XT.SanctionId AND BT.MemberId = XT.MemberId AND BT.Round = XT.Round AND BT.Event = 'Jump' "
				+ "INNER JOIN EventReg ER on BT.SanctionId = ER.SanctionId AND BT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
				+ "Where BT.SanctionId = '{0}' AND BT.Event = 'Jump' And BT.Round in ({1}, 25) {2} ", JumpEventData.mySanctionNum, curRound, tmpFilterCmd );

			curIdx++;
			curSelectCommand[curIdx] = String.Format( "SELECT BT.* FROM JumpMeasurement BT "
				+ "INNER JOIN  JumpScore XT on BT.SanctionId = XT.SanctionId AND BT.MemberId = XT.MemberId AND BT.Round = XT.Round AND BT.Event = 'Jump' "
				+ "INNER JOIN EventReg ER on BT.SanctionId = ER.SanctionId AND BT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
				+ "Where BT.SanctionId = '{0}' AND BT.Event = 'Jump' And BT.Round in ({1}, 25) {2} ", JumpEventData.mySanctionNum, curRound, tmpFilterCmd );

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
		
		private static DataTable getMinSpeedData() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT MIN(SUBSTRING(ListCode, 1, 2)) AS Speed" );
			curSqlStmt.Append( " FROM CodeValueList" );
			curSqlStmt.Append( " WHERE ListName = 'JumpBoatTime3Seg' AND ListCode LIKE '%-52M'" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static DataTable getMaxSpeedData() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName LIKE '%JumpMax' " );
			curSqlStmt.Append( "ORDER BY SortSeq " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static DataTable getMaxRampData() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName like '%RampMax' " );
			curSqlStmt.Append( "ORDER BY SortSeq " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static DataTable getBoatTimes() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode, CodeValue, MinValue, MaxValue, CodeDesc " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = 'JumpBoatTime3Seg' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );

		}

		private static DataTable getJump3TimesDivData() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode, MaxValue " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = 'IwwfJump3TimesDiv' " );
			curSqlStmt.Append( "ORDER BY SortSeq " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static DataTable getBoatPathDevMax() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode as Buoy, ListCodeNum as BuoyNum, CodeValue as CodeValueDesc, MaxValue as MaxDev " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = 'JumpBoatPathDevMax' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

	}
}
