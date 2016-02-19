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
            if ( mySanctionNum.Length < 6 ) {
                return false;
            }

            try {
                #region Add tournament registrations to OfficialWork table if one does not already exist
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "INSERT INTO OfficialWork " );
                curSqlStmt.Append( "(SanctionId, MemberId" );
                curSqlStmt.Append( ", JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating" );
                curSqlStmt.Append( ", ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating" );
                curSqlStmt.Append( ", DriverSlalomRating, DriverTrickRating, DriverJumpRating" );
                curSqlStmt.Append( ", SafetyOfficialRating, TechOfficialRating, AnncrOfficialRating " );
                curSqlStmt.Append( ") SELECT Distinct " );
                curSqlStmt.Append( "  TourReg.SanctionId, TourReg.MemberId" );
                curSqlStmt.Append( ", JudgeSlalomList.CodeValue AS JudgeSlalomRatingDesc" );
                curSqlStmt.Append( ", JudgeTrickList.CodeValue AS JudgeTrickRatingDesc" );
                curSqlStmt.Append( ", JudgeJumpList.CodeValue AS JudgeJumpRatingDesc" );
                curSqlStmt.Append( ", ScorerSlalomList.CodeValue AS ScorerSlalomRatingDesc" );
                curSqlStmt.Append( ", ScorerTrickList.CodeValue AS ScorerTrickRatingDesc" );
                curSqlStmt.Append( ", ScorerJumpList.CodeValue AS ScorerJumpRatingDesc" );
                curSqlStmt.Append( ", DriverSlalomList.CodeValue AS DriverSlalomRatingDesc" );
                curSqlStmt.Append( ", DriverTrickList.CodeValue AS DriverTrickRatingDesc" );
                curSqlStmt.Append( ", DriverJumpList.CodeValue AS DriverJumpRatingDesc" );
                curSqlStmt.Append( ", SafetyList.CodeValue AS SafetyOfficialRatingDesc" );
                curSqlStmt.Append( ", TechList.CodeValue AS TechOfficialRatingDesc" );
                curSqlStmt.Append( ", AnncrList.CodeValue AS AnncrOfficialRatingDesc " );
                curSqlStmt.Append( "FROM TourReg " );
                curSqlStmt.Append( "	LEFT OUTER JOIN MemberList ON MemberList.MemberId = TourReg.MemberId  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS JudgeSlalomList ON MemberList.JudgeSlalomRating = JudgeSlalomList.ListCode AND JudgeSlalomList.ListName = 'JudgeRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS JudgeTrickList ON MemberList.JudgeTrickRating = JudgeTrickList.ListCode AND JudgeTrickList.ListName = 'JudgeRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS JudgeJumpList ON MemberList.JudgeJumpRating = JudgeJumpList.ListCode AND JudgeJumpList.ListName = 'JudgeRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS ScorerSlalomList ON MemberList.ScorerSlalomRating = ScorerSlalomList.ListCode AND ScorerSlalomList.ListName = 'ScorerRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS ScorerTrickList ON MemberList.ScorerTrickRating = ScorerTrickList.ListCode AND ScorerTrickList.ListName = 'ScorerRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS ScorerJumpList ON MemberList.ScorerJumpRating = ScorerJumpList.ListCode AND ScorerJumpList.ListName = 'ScorerRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS DriverSlalomList ON MemberList.DriverSlalomRating = DriverSlalomList.ListCode AND DriverSlalomList.ListName = 'DriverRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS DriverTrickList ON MemberList.DriverTrickRating = DriverTrickList.ListCode AND DriverTrickList.ListName = 'DriverRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS DriverJumpList ON MemberList.DriverJumpRating = DriverJumpList.ListCode AND DriverJumpList.ListName = 'DriverRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS SafetyList ON MemberList.SafetyOfficialRating = SafetyList.ListCode AND SafetyList.ListName = 'SafetyRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS TechList ON MemberList.TechOfficialRating = TechList.ListCode AND TechList.ListName = 'TechRating'  " );
                curSqlStmt.Append( "	LEFT OUTER JOIN CodeValueList AS AnncrList ON MemberList.AnncrOfficialRating = AnncrList.ListCode AND AnncrList.ListName = 'AnnouncerRating'  " );
                curSqlStmt.Append( "WHERE TourReg.SanctionId = '" + mySanctionNum + "'" );
                curSqlStmt.Append( "  AND not exists (Select 1 from OfficialWork O Where O.SanctionId = TourReg.SanctionId and O.MemberId = TourReg.MemberId)" );
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
                curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Slalom') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Boat Judge', 'Event Judge'))))" );
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
                curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Trick') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Boat Judge', 'Event Judge'))))" );
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
                curSqlStmt.Append( "     WHERE (OfficialWorkAsgmt.Event = 'Jump') AND (OfficialWorkAsgmt.WorkAsgmt IN ('Boat Judge', 'Event Judge'))))" );
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

            } catch ( Exception ex ) {
                String ExcpMsg = ex.Message;
                if (curSqlStmt != null) {
                    ExcpMsg += "\n" + curSqlStmt.ToString() ;
                }
                MessageBox.Show( "Error: Adding to official work record "
                    + "\n\nError: " + ExcpMsg
                    );
                return false;
            } finally {
            }
            return true;

        }


    }
}
