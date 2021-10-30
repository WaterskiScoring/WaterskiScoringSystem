using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WaterskiScoringSystem.Common {
	class CheckOfficials {
		private DataTable myOfficialAsgmtDataTable = null;
		private DataTable myDriverAsgmtDataTable = null;

		public void readOfficialAssignments( String inSanctionNum, String inEvent, String inAgeGroup, String inEventGroup, String inRound ) {
			String curEventGroup = inEventGroup;
			if ( inSanctionNum.Substring( 2, 1 ).Equals( "U" ) ) {
				if ( inAgeGroup.Equals( "CM" ) ) {
					curEventGroup = "MEN A";
				} else if ( inAgeGroup.Equals( "CW" ) ) {
					curEventGroup = "WOMEN A";
				} else if ( inAgeGroup.Equals( "BW" ) ) {
					curEventGroup = "WOMEN B";
				} else if ( inAgeGroup.Equals( "BW" ) ) {
					curEventGroup = "WOMEN B";
				}
			}
			myOfficialAsgmtDataTable = getOfficialWorkAsgmt( inSanctionNum, inEvent, curEventGroup, inRound );
			myDriverAsgmtDataTable = getDriverAsgmt( inSanctionNum, inEvent, curEventGroup, inRound );
		}

		public DataTable officialAsgmtDataTable {
			get {
				return myOfficialAsgmtDataTable;
			}
		}

		public DataTable driverAsgmtDataTable {
			get {
				return myDriverAsgmtDataTable;
			}
		}
		
		public int officialAsgmtCount {
			get {
				return myOfficialAsgmtDataTable.Rows.Count;
			}
		}

		public int driverAsgmtCount {
			get {
				return myDriverAsgmtDataTable.Rows.Count;
			}
		}

		public bool officialAsgmtAvailable {
			get {
				return ( myOfficialAsgmtDataTable.Rows.Count > 0 );
			}
		}

		public DataTable getOfficialWorkAsgmt( String inSanctionNum, String inEvent, String inEventGroup, String inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt" );
			curSqlStmt.Append( ", O.StartTime, O.EndTime, O.Notes, T.SkierName AS MemberName " );
			curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
			curSqlStmt.Append( "     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) T " );
			curSqlStmt.Append( "        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId " );
			curSqlStmt.Append( "     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt " );
			curSqlStmt.Append( "WHERE O.SanctionId = '" + inSanctionNum + "' " );
			if ( inEvent != null ) curSqlStmt.Append( "  AND O.Event = '" + inEvent + "' " );
			if ( inEventGroup != null && inEventGroup.Length > 0 ) curSqlStmt.Append( "  AND O.EventGroup = '" + inEventGroup + "' " );
			if ( inRound != null ) curSqlStmt.Append( "  AND O.Round = " + inRound + " " );
			curSqlStmt.Append( "ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName" );
			return myOfficialAsgmtDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
		}
		
		private static DataTable getDriverAsgmt( String inSanctionNum, String inEvent, String inEventGroup, String inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt" );
			curSqlStmt.Append( ", O.StartTime, O.EndTime, O.Notes, T.SkierName AS MemberName " );
			curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
			curSqlStmt.Append( "     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) T " );
			curSqlStmt.Append( "        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId " );
			curSqlStmt.Append( "     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt " );
			curSqlStmt.Append( "WHERE O.SanctionId = '" + inSanctionNum + "' " );
			curSqlStmt.Append( "  AND O.WorkAsgmt = 'Driver' " );
			if ( inEvent != null ) curSqlStmt.Append( "  AND O.Event = '" + inEvent + "' " );
			if ( inEventGroup != null && inEventGroup.Length > 0 ) curSqlStmt.Append( "  AND O.EventGroup = '" + inEventGroup + "' " );
			if ( inRound != null ) curSqlStmt.Append( "  AND O.Round = " + inRound + " " );
			curSqlStmt.Append( "ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}
	}
}
