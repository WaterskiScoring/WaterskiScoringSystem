using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
	class ExportPerfDataIwwf {
        private DataRow myTourRow;
        
        private int myTourRounds = 0;
        private int mySlalomRounds = 0;
        private int myTrickRounds = 0;
        private int myJumpRounds = 0;
        private int myRoundsMax = 6;
        
        private String myTourFed = "";
        private String mySiteCode = "";

        private TourProperties myTourProperties;
        private ListSkierClass mySkierClassList;
        private CalcEventPlcmt myCalcEventPlcmt;
        private ProgressWindow myProgressInfo;

        public ExportPerfDataIwwf() {
            myCalcEventPlcmt = new CalcEventPlcmt();
        }

        public Boolean exportTourPerfData( String inSanctionId ) {
            String curMethodName = "ExportPerfDataIwwf:exportTourPerfData: ";
            String curMemberId = "", curReadyToSki = "", curAgeGroup = "";
            StreamWriter outBuffer = null;
            String curFilename = "", curRules = "";
            myTourProperties = TourProperties.Instance;

            myTourRow = getTourData( inSanctionId );
            if ( myTourRow == null ) return false;
            curRules = (String)myTourRow["Rules"];
            
            String curEventLocation = HelperFunctions.getDataRowColValue( myTourRow, "EventLocation", "" );
            mySiteCode = "Not Available";
            int curDelimStart = curEventLocation.IndexOf( "(" );
            if ( curDelimStart > 0 ) {
                int curDelimEnd = curEventLocation.IndexOf( ")" );
                mySiteCode = curEventLocation.Substring( curDelimStart + 1, curDelimEnd - curDelimStart - 1 );
            }

            curFilename = inSanctionId.Trim() + "RS.txt";
            outBuffer = getExportFile( curFilename );
            if ( outBuffer == null ) return false;

            Log.WriteFile( curMethodName + "Export iwwf performance data file begin: " + curFilename );

            //Build file header line and write to file
            buildHeaderInfo( inSanctionId );

            //Initialize output buffer
            myProgressInfo = new ProgressWindow();
            myProgressInfo.setProgessMsg( "Processing Skier Performances for IWWF ranking list" );
            myProgressInfo.Show();
            myProgressInfo.Refresh();
            myProgressInfo.setProgressMax( 10 );

            DataTable curMemberDataTable = getMemberData( inSanctionId );
            if ( curMemberDataTable == null ) return false;

            String curPlcmtMethod = "score", curPlcmtOverallOrg = "agegroup";
            String curDataType = "Round" , curPlcmtOrg = "Div", curPointsMethod = "NOPS";

            CalcScoreSummary curCalcSummary = new CalcScoreSummary();
            DataTable curSummaryDataTable = null;

            DataTable myMemberData = curCalcSummary.getMemberData( inSanctionId );
            myProgressInfo.setProgressValue( 1 );
            myProgressInfo.Refresh();

            DataTable curSlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
            myProgressInfo.setProgressValue( 2 );
            myProgressInfo.Refresh();
            DataTable curTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
            myProgressInfo.setProgressValue( 3 );
            myProgressInfo.Refresh();
            DataTable curJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
            myProgressInfo.setProgressValue( 4 );
            myProgressInfo.Refresh();
            curSummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, curSlalomDataTable, curTrickDataTable, curJumpDataTable, curDataType, curPlcmtOverallOrg );
            myProgressInfo.setProgressValue( 5 );
            myProgressInfo.Refresh();

            mySkierClassList = new ListSkierClass();
            mySkierClassList.ListSkierClassLoad();

            DataRow[] curScoreRows;
            String curOutline;
            int curRowCount = 0, curRowsSlalomSelected = 0, curRowsTrickSelected = 0, curRowsJumpSelected = 0;
            myProgressInfo.setProgressMax( curMemberDataTable.Rows.Count );
            foreach ( DataRow curMemberRow in curMemberDataTable.Rows ) {
                curRowCount++;
                myProgressInfo.setProgressValue( curRowCount );
                myProgressInfo.Refresh();

                curMemberId = curMemberRow["MemberId"].ToString();
                curAgeGroup = curMemberRow["AgeGroup"].ToString();
                curReadyToSki = curMemberRow["ReadyToSki"].ToString();
                curScoreRows = curSummaryDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );

                if ( curReadyToSki.Equals( "Y" ) ) {
                    if ( curScoreRows.Length > 0 ) {
                        foreach ( DataRow curScoreRow in curScoreRows ) {
                            curOutline = checkWriteSkierData( inSanctionId, curMemberRow, curScoreRow, "Slalom" );
                            if ( curOutline != null ) {
                                curRowsSlalomSelected++;
                                outBuffer.WriteLine( curOutline );
                            }
                            
                            curOutline = checkWriteSkierData( inSanctionId, curMemberRow, curScoreRow, "Trick" );
                            if ( curOutline != null ) {
                                curRowsTrickSelected++;
                                outBuffer.WriteLine( curOutline );
                            }

                            curOutline = checkWriteSkierData( inSanctionId, curMemberRow, curScoreRow, "Jump" );
                            if ( curOutline != null ) {
                                curRowsJumpSelected++;
                                outBuffer.WriteLine( curOutline );
                            }
                        }
                    }
                }
            }
            outBuffer.Close();

            myProgressInfo.Close();
            if ( curMemberDataTable.Rows.Count > 0 ) {
                MessageBox.Show( String.Format( "{0} Members, {1} Slalom Rows Selected, {2} Trick Rows Selected, {3} Jump Rows Selected"
                    , curRowCount, curRowsSlalomSelected, curRowsTrickSelected, curRowsJumpSelected) );
            } else {
                MessageBox.Show( "No rows found" );
            }
            Log.WriteFile( curMethodName + "Export iwwf performance data file complete: " + curFilename );

            return true;
        }

        //Write skier identification information
        private void buildHeaderInfo( String inSanctionId ) {
            myTourFed = myTourRow["Federation"].ToString().ToUpper();
            HelperFunctions.getDataRowColValue( myTourRow, "Federation", "USA" ).ToUpper();

            int.TryParse( HelperFunctions.getDataRowColValue( myTourRow, "SlalomRounds", "0" ), out mySlalomRounds );
            if ( mySlalomRounds > myRoundsMax ) mySlalomRounds = myRoundsMax;
            int.TryParse( HelperFunctions.getDataRowColValue( myTourRow, "TrickRounds", "0" ), out myTrickRounds );
            if ( myTrickRounds > myRoundsMax ) myTrickRounds = myRoundsMax;
            int.TryParse( HelperFunctions.getDataRowColValue( myTourRow, "JumpRounds", "0" ), out myJumpRounds );
            if ( myJumpRounds > myRoundsMax ) myJumpRounds = myRoundsMax;

            if ( mySlalomRounds > myTourRounds ) { myTourRounds = mySlalomRounds; }
            if ( myTrickRounds > myTourRounds ) { myTourRounds = myTrickRounds; }
            if ( myJumpRounds > myTourRounds ) { myTourRounds = myJumpRounds; }
        }

        private String checkWriteSkierData( String inSanctionId, DataRow curMemberRow, DataRow curScoreRow, String curEvent ) {
            String curReadyForPlcmt = (String)curScoreRow["ReadyForPlcmt" + curEvent];
            String curEventClass = HelperFunctions.getDataRowColValue( curScoreRow, "EventClass" + curEvent, "" ).Trim();

            if ( HelperFunctions.isObjectPopulated( curEventClass )
                && HelperFunctions.isValueTrue( curReadyForPlcmt )
                && ( curEventClass.Equals( "L" ) || curEventClass.Equals( "R" ) )
                ) {
                return writeSkierPerfData( inSanctionId, curMemberRow, curScoreRow, curEvent );
            }
            return null;
        }

        //Write skier performance summary information
        private String writeSkierPerfData( String inSanctionId, DataRow curMemberRow, DataRow curScoreRow, String curEvent ) {
            String curMethodName = "ExportPerfDataIwwf:writeSkierPerfData: ";
            int curSkiYear, curSkiYearAge;
            decimal curScore = 0;
            StringBuilder outLine = new StringBuilder( "" );
            String curSkierLastName, curSkierFirstName;
            String curSkierName = HelperFunctions.getDataRowColValue( curMemberRow, "SkierName", "" );
            String[] curNameList = curSkierName.Split( ',' );

            String[] curSkierNameParts = getSkierName( curMemberRow );
            if ( curSkierNameParts == null || curSkierNameParts.Length != 2 ) {
                String curMsg = String.Format( "{0} Bypassing skier with member Id {1}, has missing or invalid name", curMethodName, HelperFunctions.getDataRowColValue( curMemberRow, "MemberId", "" ) );
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return null;
            }
            curSkierLastName = curSkierNameParts[0].Trim();
            curSkierFirstName = curSkierNameParts[1].Trim();

            outLine.Append( curSkierLastName + ";" );
            outLine.Append( curSkierFirstName + ";" );
            outLine.Append( HelperFunctions.getDataRowColValue( curMemberRow, "MemberId", "" ) + ";;" );
            outLine.Append( HelperFunctions.getDataRowColValue( curMemberRow, "Federation", "" ) + ";" );
            outLine.Append( HelperFunctions.getDataRowColValue( curMemberRow, "Gender", "" ) + ";" );
            outLine.Append( HelperFunctions.getDataRowColValue( curMemberRow, "SanctionId", "" ) + ";" );

            String curReadyForPlcmt = (String)curScoreRow["ReadyForPlcmt" + curEvent];

            if ( curEvent.Equals("Slalom")) {
                decimal.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "ScoreSlalom", "0" ), out curScore );
                outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "ScoreSlalom", "" ) + ";;;" );
                outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "FinalPassScore", "" ) + ";" );
            }
            if ( curEvent.Equals( "Trick" ) ) {
                decimal.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "ScoreTrick", "0" ), out curScore );
                outLine.Append( ";" + HelperFunctions.getDataRowColValue( curScoreRow, "ScoreTrick", "" ) + ";;;" );
            }
            if ( curEvent.Equals( "Jump" ) ) {
                decimal.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "ScoreMeters", "0" ), out curScore );
                outLine.Append( ";;" + HelperFunctions.getDataRowColValue( curScoreRow, "ScoreMeters", "" ) + ";" );
                outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "ScoreFeet", "" ) + ";" );
            }
            if ( curScore == 0m ) return null;

            int.TryParse( inSanctionId.Substring( 0, 2 ), out curSkiYear );
            int.TryParse( HelperFunctions.getDataRowColValue( curMemberRow, "SkiYearAge", "0" ), out curSkiYearAge );
            outLine.Append( (2000 + curSkiYear - curSkiYearAge - 1).ToString() + ";" );
            outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "EventClass" + curEvent, "" ).Trim() + ";" );
            outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "Round" + curEvent, "" ) + ";" );
            String curDiv = HelperFunctions.getDataRowColValue( curScoreRow, "AgeGroup", "" );
            outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "AgeGroup", "" ) + ";" );

            if ( curEvent.Equals( "Slalom" ) ) {
                outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "FinalLen", "" ) + ";" );
                outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "FinalSpeedKph", "" ) + ";" );
            }
            if ( curEvent.Equals( "Trick" ) ) {
                outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "ScorePass1", "" ) + ";" );
                outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "ScorePass2", "" ) + ";" );
            }
            if ( curEvent.Equals( "Jump" ) ) {
                Decimal curRampHeight = Convert.ToDecimal( HelperFunctions.getDataRowColValue( curScoreRow, "RampHeight", "0" ) );
                if ( curRampHeight == 5.00M ) {
                    outLine.Append( ".235");
                } else if ( curRampHeight == 5.50M ) {
                    outLine.Append( ".255" );
                } else if ( curRampHeight == 6.00M ) {
                    outLine.Append( ".271" );
                } else if ( curRampHeight == 4.00M ) {
                    outLine.Append( ".215" );
                } else if ( curRampHeight == 4.50M ) {
                    outLine.Append( ".215" );
                } else {
                    outLine.Append( ".235" );
                }
                outLine.Append( ";" );
                outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "SpeedKphJump", "" ) + ";" );
            }
            DateTime curEventDate;
            DateTime.TryParse( HelperFunctions.getDataRowColValue( myTourRow, "EventDates", "" ), out curEventDate );
            outLine.Append( curEventDate.ToString( "yyyyMMdd" ) + ";" );

            String curSeniorOrJunior = "";
            if ( curDiv.Equals( "OM" ) || curDiv.Equals( "OM" ) ) {
                curSeniorOrJunior = "";
            } else if ( curSkiYearAge < 18 ) curSeniorOrJunior = "J";
            outLine.Append( curSeniorOrJunior + ";" );

            outLine.Append( "Y;" );
            if ( curEvent.Equals( "Slalom" ) && curScore < 6 ) outLine.Append( "*" );
            outLine.Append( ";" );

            outLine.Append( HelperFunctions.getDataRowColValue( curScoreRow, "Plcmt" + curEvent, "" ) + ";" );
            outLine.Append( getIwwfAthleteId( curMemberRow ) + ";" );
            outLine.Append( mySiteCode );

            return outLine.ToString();
        }

        private String[] getSkierName( DataRow curDataRow ) {
            String curSkierName = HelperFunctions.getDataRowColValue( curDataRow, "SkierName", "" );
            if ( HelperFunctions.isObjectEmpty( curSkierName ) ) return null;
            HelperFunctions.stringReplace( curSkierName, HelperFunctions.SingleQuoteDelim, "" );
            return curSkierName.Split( ',' );
        }

        private String getIwwfAthleteId( DataRow curDataRow ) {
            String curFedCode = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" ).ToUpper();
            if ( HelperFunctions.isObjectEmpty( curFedCode ) || curFedCode.Equals( "USA" ) ) {
                return "USA" + HelperFunctions.getDataRowColValue( curDataRow, "MemberId", "" );
            } else {
                return curFedCode + HelperFunctions.getDataRowColValue( curDataRow, "ForeignFederationID", "" );
            }
        }

        private DataRow getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactMemberId, ContactAddress, ContactPhone, ContactEmail, CP.SkierName AS ContactName " );
            curSqlStmt.Append( ", ChiefJudgeMemberId, ChiefJudgeAddress, ChiefJudgePhone, ChiefJudgeEmail, CJ.SkierName AS ChiefJudgeName" );
            curSqlStmt.Append( ", ChiefDriverMemberId, ChiefDriverAddress, ChiefDriverPhone, ChiefDriverEmail, CD.SkierName AS ChiefDriverName " );
            curSqlStmt.Append( ", SafetyDirMemberId, SafetyDirAddress, SafetyDirPhone, SafetyDirEmail, SD.SkierName AS SafetyDirName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CP " );
            curSqlStmt.Append( "    ON CP.SanctionId = T.SanctionId AND CP.MemberId = T.ContactMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CJ " );
            curSqlStmt.Append( "    ON CJ.SanctionId = T.SanctionId AND CJ.MemberId = T.ChiefJudgeMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CD " );
            curSqlStmt.Append( "    ON CD.SanctionId = T.SanctionId AND CD.MemberId = T.ChiefDriverMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) SD " );
            curSqlStmt.Append( "    ON SD.SanctionId = T.SanctionId AND SD.MemberId = T.SafetyDirMemberId " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return null;
            return curDataTable.Rows[0];
        }

        private DataTable getMemberData( String inSanctionId ) {
            String selectStmt = "SELECT T.SanctionId, Federation, ForeignFederationID, T.MemberId, SkierName, "
                + "Gender, SkiYearAge, State, ReadyToSki, L.CodeValue AS Region, "
                + "RS.TeamCode AS SlalomTeam, RT.TeamCode AS TrickTeam, RJ.TeamCode AS JumpTeam, "
                + "RS.AgeGroup AS SlalomGroup, RT.AgeGroup AS TrickGroup, RJ.AgeGroup AS JumpGroup, "
                + "RS.EventClass AS SlalomClass, RT.EventClass AS TrickClass, RJ.EventClass AS JumpClass, "
                + "T.AgeGroup as AgeGroup, RS.AgeGroup AS SlalomAgeGroup, RT.AgeGroup AS TrickAgeGroup, RJ.AgeGroup AS JumpAgeGroup "
                + "FROM TourReg AS T "
                + "  LEFT OUTER JOIN EventReg AS RS ON T.SanctionId = RS.SanctionId AND T.MemberId = RS.MemberId AND T.AgeGroup = RS.AgeGroup AND RS.Event = 'Slalom'"
                + "  LEFT OUTER JOIN EventReg AS RT ON T.SanctionId = RT.SanctionId AND T.MemberId = RT.MemberId AND T.AgeGroup = RT.AgeGroup AND RT.Event = 'Trick'"
                + "  LEFT OUTER JOIN EventReg AS RJ ON T.SanctionId = RJ.SanctionId AND T.MemberId = RJ.MemberId AND T.AgeGroup = RJ.AgeGroup AND RJ.Event = 'Jump'"
                + "  LEFT OUTER JOIN CodeValueList AS L ON ListName = 'StateRegion' AND ListCode = State "
                + "WHERE (T.SanctionId = '" + inSanctionId + "')"
                + "ORDER BY SkierName, T.AgeGroup"
            ;
            DataTable curDataTable = DataAccess.getDataTable( selectStmt );
            return curDataTable;
        }

        private String getIwwfSlalomMin( String inDiv ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, CodeValue, MinValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'IWWFSlalomMin' " );
            curSqlStmt.Append( "  AND ListCode = '" + inDiv + "' " );
            curSqlStmt.Append( "ORDER BY SortSeq" );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                return ( (Decimal)curDataTable.Rows[0]["MaxValue"] ).ToString( "00" );
            } else {
                return "25";
            }
        }

        private StreamWriter getExportFile( String inFileName ) {
            StreamWriter outBuffer = null;

            SaveFileDialog curFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            curFileDialog.InitialDirectory = curPath;
            curFileDialog.FileName = inFileName;

            try {
                if ( curFileDialog.ShowDialog() == DialogResult.OK ) {
                    String curFileName = curFileDialog.FileName;
                    if ( curFileName != null ) {
                        if ( Path.GetExtension( curFileName ) == null ) curFileName += ".txt";
                        outBuffer = File.CreateText( curFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }
    }
}
