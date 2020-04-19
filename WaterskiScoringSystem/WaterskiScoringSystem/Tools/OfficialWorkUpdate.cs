using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class OfficialWorkUpdate {
        private String mySanctionNum;

        public OfficialWorkUpdate() {
        }

        public bool updateOfficialWorkRecord(  ) {
            String curMethodName = "OfficialWorkUpdate:updateOfficialWorkRecord";
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            mySanctionNum = Properties.Settings.Default.AppSanctionNum.Trim();
            if ( mySanctionNum.Length < 6 ) return false;

			#region Add tournament registrations to OfficialWork table if one does not already exist
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "INSERT INTO OfficialWork ( " );
			curSqlStmt.Append( "SanctionId, MemberId" );
			curSqlStmt.Append( ", JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating" );
			curSqlStmt.Append( ", ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating" );
			curSqlStmt.Append( ", DriverSlalomRating, DriverTrickRating, DriverJumpRating" );
			curSqlStmt.Append( ", SafetyOfficialRating, TechOfficialRating, AnncrOfficialRating " );
			curSqlStmt.Append( ") SELECT Distinct " );
			curSqlStmt.Append( "  TR.SanctionId, TR.MemberId" );
			curSqlStmt.Append( ", Coalesce( ML.JudgeSlalomRating, '' ) as JudgeSlalomRating" );
			curSqlStmt.Append( ", Coalesce( ML.JudgeTrickRating, '' ) as JudgeTrickRating" );
			curSqlStmt.Append( ", Coalesce( ML.JudgeJumpRating, '' ) as JudgeJumpRating" );
			curSqlStmt.Append( ", Coalesce( ML.ScorerSlalomRating, '' ) as ScorerSlalomRating" );
			curSqlStmt.Append( ", Coalesce( ML.ScorerTrickRating, '' ) as ScorerTrickRating" );
			curSqlStmt.Append( ", Coalesce( ML.ScorerJumpRating, '' ) as ScorerJumpRating" );
			curSqlStmt.Append( ", Coalesce( ML.DriverSlalomRating, '' ) as DriverSlalomRating" );
			curSqlStmt.Append( ", Coalesce( ML.DriverTrickRating, '' ) as DriverTrickRating" );
			curSqlStmt.Append( ", Coalesce( ML.DriverJumpRating, '' ) as DriverJumpRating" );
			curSqlStmt.Append( ", Coalesce( ML.SafetyOfficialRating, '' ) as SafetyOfficialRating" );
			curSqlStmt.Append( ", Coalesce( ML.TechOfficialRating, '' ) as TechOfficialRating" );
			curSqlStmt.Append( ", Coalesce( ML.AnncrOfficialRating, '' ) as AnncrOfficialRating " );
			curSqlStmt.Append( "FROM TourReg TR " );
			curSqlStmt.Append( "	INNER JOIN MemberList ML ON ML.MemberId = TR.MemberId  " );
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "  AND not exists (Select 1 from OfficialWork O Where O.SanctionId = TR.SanctionId and O.MemberId = TR.MemberId) " );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			if ( rowsProc > 0 ) Log.WriteFile( curMethodName + ":Rows added to OfficialWork table " + rowsProc.ToString() );
			#endregion

			#region Update credits
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Delete OfficialWorkAsgmt " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "AND MemberId is null or MemberId = '' " );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update slalom judge credits
			//No HQ credit for 'Event Judge Asst', 'Rope Handler'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET JudgeSlalomCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Slalom') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Boat Judge', 'Event Judge', 'Event ACJ'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update trick judge credits
			//No HQ credit for 'Event Judge Asst', 'Rope Handler'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET JudgeTrickCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Trick') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Boat Judge', 'Event Judge', 'Event ACJ'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update jump judge credits
			//No HQ credit for 'Event Judge Asst', 'Rope Handler'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET JudgeJumpCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Jump') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Boat Judge', 'Event Judge', 'Event ACJ'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update slalom driver credits
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET DriverSlalomCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Slalom') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Driver'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update trick driver credits
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET DriverTrickCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Trick') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Driver'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update jump driver credits
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET DriverJumpCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Jump') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Driver'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update slalom scorer credits
			//No HQ credit for 'Score Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET ScoreSlalomCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Slalom') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Scorer'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update trick scorer credits
			//No HQ credit for 'Score Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET ScoreTrickCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Trick') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Scorer'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update jump scorer credits
			//No HQ credit for 'Score Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET ScoreJumpCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Jump') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Scorer'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update slalom safety credits
			//No HQ credit for 'Safety Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET SafetySlalomCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Slalom') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Safety'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update trick safety credits
			//No HQ credit for 'Safety Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET SafetyTrickCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Trick') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Safety'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update jump safety credits
			//No HQ credit for 'Safety Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET SafetyJumpCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Jump') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Safety'))))" );
			#endregion

			#region Update slalom announcer credits
			//No HQ credit for 'Announcer Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET AnncrSlalomCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Slalom') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Announcer'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update trick announcer credits
			//No HQ credit for 'Announcer Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET AnncrTrickCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Trick') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Announcer'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update jump announcer credits
			//No HQ credit for 'Announcer Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET AnncrJumpCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Jump') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Announcer'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update slalom technical credits
			//No HQ credit for 'Technical Controller Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET TechSlalomCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Slalom') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Technical Controller'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update trick technical credits
			//No HQ credit for 'Technical Controller Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET TechTrickCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Trick') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Technical Controller'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Update jump technical credits
			//No HQ credit for 'Technical Controller Asst'
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "UPDATE    OfficialWork " );
			curSqlStmt.Append( "SET TechJumpCredit = 'Y'" );
			curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') AND (MemberId IN " );
			curSqlStmt.Append( "    (SELECT OfficialWorkAsgmt.MemberId FROM OfficialWorkAsgmt INNER JOIN " );
			curSqlStmt.Append( "        OfficialWork AS OfficialWork_1 ON OfficialWorkAsgmt.SanctionId = OfficialWork_1.SanctionId " );
			curSqlStmt.Append( "        AND OfficialWorkAsgmt.MemberId = OfficialWork_1.MemberId " );
			curSqlStmt.Append( "        AND OfficialWork_1.SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Jump') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Technical Controller'))))" );
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			return true;

        }


    }
}
