using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace WaterskiScoringSystem.Common {
	class CheckOfficials {

		public bool checkOfficialAssignments(String inSanctionNum, String inEvent, String inEventGroup, String inRound ) {
			DataTable curDataTable = getOfficialWorkAsgmt( inSanctionNum, inEvent, inEventGroup, inRound );
			if ( curDataTable.Rows.Count > 0 ) {
				return true;
			} else {
				return false;
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
			if ( inEvent != null ) {
				curSqlStmt.Append( "  AND O.Event = '" + inEvent + "' " );
			}
			if ( inEventGroup != null ) {
				if ( inEventGroup.Length > 0 ) {
					curSqlStmt.Append( "  AND O.EventGroup = '" + inEventGroup + "' " );
				}
			}
			if ( inRound != null ) {
				curSqlStmt.Append( "  AND O.Round = " + inRound + " " );
			}
			curSqlStmt.Append( "ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}


	}
}
