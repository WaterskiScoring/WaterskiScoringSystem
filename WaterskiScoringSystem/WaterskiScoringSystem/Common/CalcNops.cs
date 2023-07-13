using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace WaterskiScoringSystem.Common {
    class CalcNops {
        private static readonly CalcNops myInstance = new CalcNops();
        //private List<ListItem> RatingList = new List<ListItem>();
        private DataTable myDataTable;
        private String mySanctionNum = "";

        private CalcNops() {
            // Load data rating values
            //RatingList.Add( new ListItem( "Rating2nd", "2nd Class" ) );
            //RatingList.Add( new ListItem( "Rating1st", "1st Class" ) );
            //RatingList.Add( new ListItem( "RatingExp", "Expert" ) );
            //RatingList.Add( new ListItem( "RatingMst", "Master" ) );
            //RatingList.Add( new ListItem( "RatingEp", "EP" ) );
            //RatingList.Add( new ListItem( "RatingOpen", "Open" ) );
        }

        public static CalcNops Instance {
            get {
                return myInstance;
            }
        }

        public void LoadDataForTour() {
            String curSanctionNum = Properties.Settings.Default.AppSanctionNum;
            if (curSanctionNum != null) {
                if (curSanctionNum.Length > 5) {
                    mySanctionNum = curSanctionNum;
                    Byte curSkiYear = Convert.ToByte(curSanctionNum.Substring(0, 2));
                    myDataTable = getNopsDataBySkiYear(curSkiYear);
                    if (myDataTable.Rows.Count == 0) {
                        MessageBox.Show("NOPS factors not available for tournament ski year " + curSanctionNum.Substring(0, 2)
                            + "\n Current year NOPS factors being used.");
                        mySanctionNum = "";
                        myDataTable = getNopsDataCurSkiYear();
                    }
                } else {
                    LoadDataCurrent();
                }
            } else {
                LoadDataCurrent();
            }
        }

        public void LoadDataCurrent() {
            mySanctionNum = "";
            myDataTable = getNopsDataCurSkiYear();
        }

        public DataTable NopsDataTable {
            get {
                return myDataTable;
            }
        }

        /*
         * Calculate NOPS Values and detemine rating corresponding to score.
         * This calculation is perfomred for each event supplied in the ScoreEntry list
         */
        public void calcNops( String inAgeGroup, ScoreEntry inScoreEntry ) {
            String curDateString = DateTime.Now.ToString( "yy" );
            Byte curSkiYear = Convert.ToByte( curDateString );
            if ( mySanctionNum != null ) {
                if ( mySanctionNum.Length > 5 ) {
                    curSkiYear = Convert.ToByte( mySanctionNum.Substring( 0, 2 ) );
                }
            }

            DataRow[] findRows = myDataTable.Select("AgeGroup = '" + inAgeGroup + "' and Event = '" + inScoreEntry.Event + "'");
            if ( findRows.Length > 0 ) {
                DataRow curRow = findRows[0];

                if (inScoreEntry.Event.Equals( "Slalom" )) {
                    inScoreEntry.Nops = calcSlalomNops( inScoreEntry.Score, curRow );
                    inScoreEntry.Rating = getRating( inScoreEntry.Score, curRow );
                } else if (inScoreEntry.Event.Equals( "Trick" )) {
                    inScoreEntry.Nops = calcTrickNops( inScoreEntry.Score, curRow );
                    inScoreEntry.Rating = getRating( inScoreEntry.Score, curRow );
                } else if (inScoreEntry.Event.Equals( "Jump" )) {
                    inScoreEntry.Nops = calcJumpNops( inScoreEntry.Score, curRow );
                    inScoreEntry.Rating = getRating( inScoreEntry.Score, curRow );
                }
            } else {
                inScoreEntry.Nops = 0;
                inScoreEntry.Rating = "";
            }
        }

        /*
         * Calculate NOPS Values and detemine rating corresponding to score.
         * This calculation is perfomred for each event supplied in the ScoreEntry list
         */
        public void calcNops( String inAgeGroup, List<ScoreEntry> inScoreList ) {
            DataRow[] findRows;
            DataRow curRow;

            String curDateString = DateTime.Now.ToString( "yy" );
            Byte curSkiYear = Convert.ToByte( curDateString );
            if (mySanctionNum != null) {
                if ( mySanctionNum.Length > 5 ) {
                    curSkiYear = Convert.ToByte( mySanctionNum.Substring( 0, 2 ) );
                }
            }

            foreach ( ScoreEntry curEntry in inScoreList ) {
                if ( curEntry.Event.Equals( "Overall" ) ) {
                    if ( inScoreList.Count > 1 ) {
                        curEntry.Nops = calcOverallNops( inScoreList );
                        curEntry.Score = curEntry.Nops;
                        curEntry.Rating = "";
                    } else {
                        curEntry.Nops = curEntry.Score;
                        curEntry.Rating = "";
                    }
                } else {
                    findRows = myDataTable.Select( "AgeGroup = '" + inAgeGroup + "' and Event = '" + curEntry.Event + "'" );
                    if ( findRows.Length > 0 ) {
                        curRow = findRows[0];
                        if (curEntry.Event.Equals( "Slalom" )) {
                            curEntry.Nops = calcSlalomNops( curEntry.Score, curRow );
                            curEntry.Rating = getRating( curEntry.Score, curRow );
                        } else if (curEntry.Event.Equals( "Trick" )) {
                            curEntry.Nops = calcTrickNops( curEntry.Score, curRow );
                            curEntry.Rating = getRating( curEntry.Score, curRow );
                        } else if (curEntry.Event.Equals( "Jump" )) {
                            curEntry.Nops = calcJumpNops( curEntry.Score, curRow );
                            curEntry.Rating = getRating( curEntry.Score, curRow );
                        }
                    } else {
                        curEntry.Nops = 0;
                        curEntry.Rating = "";
                    }
                }
            }
        }

        /*
         * Calculate Slalom NOPS Value
         */
        public Decimal calcSlalomNops( Decimal inScore, DataRow inRow ) {
            Decimal curNops = 0, curBase, curBasePts, curExp, curRatingRec, curRatingMedian;

            curBase = (Decimal)inRow["Base"];
            curBasePts = (Decimal)inRow["OverallBase"];
            //curExp = (Decimal)inRow["OverallExp"];
            curRatingRec = (Decimal)inRow["RatingRec"];
            curRatingMedian = (Decimal)inRow["RatingMedian"];

            try {
                //INT(0.5+1000 * LN( (500-(6*J2)) / (1500-(6*J2)) ) / ( LN( (G2-6) / (H2-6) ) ) ) / 1000
                curExp = Math.Round( Convert.ToDecimal( Math.Log( Convert.ToDouble( ( 500 - ( 6 * curBasePts ) ) / ( 1500 - ( 6 * curBasePts ) ) ), Math.E )
                    / Math.Log( Convert.ToDouble( ( curRatingMedian - 6 ) / ( curRatingRec - 6 ) ), Math.E ) ), 3 );
            } catch {
                curExp = 0;
            }

            if (curBase > 0 && curRatingMedian > 0 && curExp > 0) {
                if ( inScore < 6 ) {
                    curNops = inScore * curBasePts;
                } else {
                    //(6*SLM_Pts)+((1500-(6*SLM_Pts))*((Slalom_Score-6)/(Slm_Recd-6))^SLM_Exp)
                    //INT( 0.5 + 10 * 
                    //IF(Slalom_Score<6,(Slalom_Score*SLM_Pts),
                    //(6*SLM_Pts) + ((1500-(6*SLM_Pts)) * ((Slalom_Score-6) / (Slm_Recd-6)) ^ SLM_Exp)) 
                    //)/10
                    curNops = Math.Round( 
                        (Decimal)( ( 6 * curBasePts )
                        + ( curBase - ( 6 * curBasePts ) )
                        * Convert.ToDecimal( Math.Pow( (Double)( ( inScore - 6 ) / ( curRatingRec - 6 ) ), (Double)curExp ) ) )
                        , 1);
                }
            }

            return curNops;
        }

        /*
         * Calculate Trick NOPS Value
         */
        public Decimal calcTrickNops( Decimal inScore, DataRow inRow ) {
            Decimal curNops = 0, curBase, curExp, curRatingRec, curRatingMedian;
            Decimal curDec5 = 5, curDec15 = 15;

            curBase = (Decimal)inRow["Base"];
            //curExp = (Decimal)inRow["OverallExp"];
            curRatingRec = (Decimal)inRow["RatingRec"];
            curRatingMedian = (Decimal)inRow["RatingMedian"];

            try {
                //INT(0.5+1000 * LN(5/15) / LN(G6/H6)) / 1000
                curExp = Math.Round( Convert.ToDecimal( Math.Log( Convert.ToDouble( curDec5 / curDec15 ), Math.E )
                    / Math.Log( Convert.ToDouble( curRatingMedian / curRatingRec ), Math.E ) ), 3 );
            } catch (Exception e) {
                String curMsg = e.Message;
                curExp = 0;
            }

            if (curBase > 0 && curRatingMedian > 0 && curExp > 0) {
                //INT(0.5+15000*((Trick_Score/Trk_Recd)^TRK_Exp))/10
                if (inScore > 0) {
                    curNops = Math.Round( (Decimal)( ( curBase * Convert.ToDecimal( Math.Pow( (Double)( inScore / curRatingRec ), (Double)curExp ) ) ) / 10 ), 1 );
                } else {
                    curNops = 0;
                }
            }

            return curNops;
        }

        /*
         * Calculate Jump NOPS Value
         * IF(Jump_Score<(0.15*JMP_Recd),0,700*(((Jump_Score-(0.15*JMP_Recd))/(JMP_Base-(0.15*JMP_Recd)))^JMP_Exp))
         *  700*
         *  ((( Jump_Score- (0.15*JMP_Recd ) )
         *  / (JMP_Base-(0.15*JMP_Recd)))^JMP_Exp)
         * 
         * IF(Jump_Score<(0.15*JMP_Recd),0,
         * 700*(((Jump_Score-(0.15*JMP_Recd))/(JMP_Base-(0.15*JMP_Recd)))^JMP_Exp))
         * 
         * Rating
         * IF(OR(Division="B1",Division="G1"),"--",IF(Jump_Score<D11,"None",IF(Jump_Score>=H11,"Open",LOOKUP(Jump_Score,Jump,Ratings))))
         */
        public Decimal calcJumpNops( Decimal inScore, DataRow inRow ) {
            Decimal curNops = 0, curBase, curExp, curRatingRec, curRatingMedian, curMinScore;
            Decimal curDec5 = 5, curDec15 = 15;
            Decimal cur15Percent = Convert.ToDecimal("0.15");

            curBase = (Decimal)inRow["Base"];
            curMinScore = (Decimal)inRow["Adj"];
            //curExp = (Decimal)inRow["OverallExp"];
            curRatingRec = (Decimal)inRow["RatingRec"];
            curRatingMedian = (Decimal)inRow["RatingMedian"];

            try {
                //INT(0.5+1000*LN(5/15) / LN( (G7-(0.15*H7))/(H7-(0.15*H7))) ) /1000
                curExp = Math.Round( Convert.ToDecimal( Math.Log( Convert.ToDouble( curDec5 / curDec15 ), Math.E )
                    / Math.Log( Convert.ToDouble( ( curRatingMedian - ( cur15Percent * curRatingRec ) ) / ( curRatingRec - ( cur15Percent * curRatingRec ) ) ), Math.E )
                    ), 3 );
            } catch (Exception e) {
                String curMsg = e.Message;
                curExp = 0;
            }

            if (curBase > 0 && curRatingMedian > 0 && curExp > 0) {
                if ( ( inScore < curMinScore ) ) {
                    curNops = 0;
                } else {
                    //INT(0.5 + 10 * IF(Jump_Score<(0.15*JMP_Recd),0,
                    // 1500 * (( (Jump_Score-(0.15*JMP_Recd)) / (JMP_Recd-(0.15*JMP_Recd)))^JMP_Exp)))/10
                    curNops = Math.Round( 
                        (Decimal)(curBase * Convert.ToDecimal( Math.Pow( (Double)( ( inScore - curMinScore ) / ( curRatingRec - curMinScore ) ), (Double)curExp ) ) )
                        , 1 );
                }
            }
            return curNops;
        }

        /*
         * Calculate Overall NOPS Value
         * =IF(OR(Division="G1",Division="B1"),SUM(F3:F4),SUM(F3:F5))
         * 
         * Overall Rating
         * IF(OA_Actual<OA_Reqd,"--",IF(Overall_Score<E12,"None",LOOKUP(Overall_Score,Overall,OARatings)))
         */
        public Decimal calcOverallNops( List<ScoreEntry> inScoreList ) {
            Decimal curNops;

            curNops = 0;
            foreach ( ScoreEntry curEntry in inScoreList ) {
                if ( !( curEntry.Event.Equals( "Overall" ) ) ) {
                    curNops = curNops + curEntry.Nops;
                }
            }

            return curNops;
        }

        /*
         * Determine rating corresponding to score and event
         * Event information is provided in DataRowView object
         */
        public String getRating( Decimal inScore, DataRow inRow ) {
            //Decimal curRatingValue;
            String myRating = "";
            /*
            for ( int idx = 0; idx < RatingList.Count; idx++ ) {
                curRatingValue = (Decimal)inRow[RatingList[idx].ItemName];
                if ( inScore < curRatingValue ) { break; }
                if ( curRatingValue > 0 ) {
                    myRating = RatingList[idx].ItemValue;
                }
            }
             */
            return myRating;
        }

        private DataTable getNopsDataCurSkiYear() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT  N.PK, SkiYear, Event, AgeGroup, N.SortSeq, Base, Adj, " );
            curSqlStmt.Append( "RatingOpen, RatingRec, RatingMedian, OverallBase, OverallExp, EventsReqd " );
            curSqlStmt.Append( "FROM NopsData N " );
            curSqlStmt.Append( "INNER JOIN CodeValueList V ON N.AgeGroup = V.ListCode " );
            curSqlStmt.Append( "WHERE V.ListName = 'AWSAAgeGroup' " );
            curSqlStmt.Append( "AND N.SkiYear IN (SELECT MAX(SkiYear) AS Expr1 FROM NopsData AS N2) "  );
            curSqlStmt.Append( "ORDER BY N.SkiYear DESC, V.SortSeq, N.SortSeq " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getNopsDataBySkiYear( Byte inSkiYear ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct N.PK, SkiYear, Event, AgeGroup, V.SortSeq as SortSeqDiv, N.SortSeq, Base, Adj, " );
            curSqlStmt.Append( "RatingOpen, RatingRec, RatingMedian, OverallBase, OverallExp, EventsReqd " );
            curSqlStmt.Append( "FROM NopsData N " );
            curSqlStmt.Append( "INNER JOIN CodeValueList V ON N.AgeGroup = V.ListCode " );
            curSqlStmt.Append( "WHERE V.ListName = 'AWSAAgeGroup' " );
            curSqlStmt.Append( "AND N.SkiYear = " + inSkiYear.ToString() );
            curSqlStmt.Append( "ORDER BY N.SkiYear DESC, V.SortSeq, N.SortSeq " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
