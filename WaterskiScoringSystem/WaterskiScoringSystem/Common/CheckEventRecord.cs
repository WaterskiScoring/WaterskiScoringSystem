using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Common {
    class CheckEventRecord {
        AgeGroupDropdownList myAgeGroupList = null;
        String myTourRules = "";

        public CheckEventRecord(DataRow inTourRow ) {
            myAgeGroupList = new AgeGroupDropdownList(inTourRow);
            myTourRules = (String) inTourRow["Rules"];
        }

        public String checkRecordSlalom( String inDiv, String inScore, short inSkiYearAge, String inGender ) {
            StringBuilder curReturnMsg = new StringBuilder("");
            StringBuilder curSqlStmt = new StringBuilder("");

            #region Check to see if score is equal to or great than divisions current record score
            curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc ");
            curSqlStmt.Append("FROM CodeValueList ");
            curSqlStmt.Append("WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')");
            curSqlStmt.Append("  AND ListCode = '" + inDiv + "-S' ");
            DataTable curRecordDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curRecordDataTable.Rows.Count > 0 ) {
                if ( curRecordDataTable.Rows[0]["MaxValue"] != System.DBNull.Value ) {
                    Decimal curRecordScore = (Decimal) curRecordDataTable.Rows[0]["MaxValue"];
                    Decimal curScore = 0;
                    try {
                        curScore = Convert.ToDecimal(inScore);
                    } catch {
                        curScore = 0;
                    }
                    if ( curScore >= curRecordScore ) {
                        curReturnMsg.Append("Current skier score of " + curScore.ToString("##0.00"));
                        curReturnMsg.Append("\nMatches or exceeds the " + (String) curRecordDataTable.Rows[0]["CodeValue"] + " division's current record ");
                        String curRecordInfo = (String) curRecordDataTable.Rows[0]["CodeDesc"];
                        int curDelim = curRecordInfo.IndexOf('/');
                        if ( curDelim > 5 ) {
                            if ( curRecordInfo.Substring(curDelim - 1).Equals(" ") ) {
                                curDelim--;
                            } else {
                                curDelim--;
                                curDelim--;
                            }
                            int curDelim2 = curRecordInfo.IndexOf(' ', curDelim + 1);
                            curReturnMsg.Append("\n" + curRecordScore.ToString("##0.00") + " buoys, ");
                            curReturnMsg.Append(" " + curRecordInfo.Substring(0, curDelim));
                            curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(curDelim + 1, curDelim2 - curDelim - 1));
                            curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim2 + 1));
                        } else {
                            curReturnMsg.Append("\n" + curRecordScore.ToString("##0.00") + " buoys");
                            curReturnMsg.Append("\nRecord information " + curRecordInfo);
                        }
                        curReturnMsg.Append("\n\nNote: Records can't be set with a speed greater than the historical division maximum");
                    }
                }
            }
            #endregion

            #region Check scores for comparable age divisions 
            Decimal curOrigPassNumMinSpeed = 0;
            if ( myAgeGroupList.isDivisionIntl(inDiv) ) {
                curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue");
                curSqlStmt.Append(" FROM CodeValueList");
                curSqlStmt.Append(" WHERE ListName = 'IWWFSlalomMin' AND ListCode = '" + inDiv + "'");
                curSqlStmt.Append(" ORDER BY SortSeq");
                DataTable curMinSpeedDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                if ( curMinSpeedDataTable.Rows.Count > 0 ) {
                    curOrigPassNumMinSpeed = Convert.ToInt16((Decimal) curMinSpeedDataTable.Rows[0]["MinValue"]);
                    curOrigPassNumMinSpeed--;
                }
            }
            #endregion

            #region Check scores for comparable age divisions 
            String curGender = inGender;
            if ( inGender == null || inGender.Equals("") ) {
                curGender = myAgeGroupList.getGenderOfAgeDiv(inDiv);
            }
            ArrayList curDropdownList = myAgeGroupList.getComparableDivListForAge(inDiv, inSkiYearAge, curGender, "Slalom");
            IEnumerator myList = curDropdownList.GetEnumerator();
            while ( myList.MoveNext() ) {
                string curEntry = (String)myList.Current;
                string curAgeDiv = curEntry.Substring(0, curEntry.IndexOf(" "));

                if ( curAgeDiv.Equals(inDiv) ) {
                    //Divisions already checked
                } else {
                    Decimal curPassNumMinSpeed = 0;
                    if ( myAgeGroupList.isDivisionIntl(curAgeDiv) ) {
                        curSqlStmt = new StringBuilder("");
                        curSqlStmt.Append("SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue");
                        curSqlStmt.Append(" FROM CodeValueList");
                        curSqlStmt.Append(" WHERE ListName = 'IWWFSlalomMin' AND ListCode = '" + curAgeDiv + "'");
                        curSqlStmt.Append(" ORDER BY SortSeq");
                        DataTable curMinSpeedDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                        if ( curMinSpeedDataTable.Rows.Count > 0 ) {
                            curPassNumMinSpeed = Convert.ToInt16((Decimal) curMinSpeedDataTable.Rows[0]["MinValue"]);
                            curPassNumMinSpeed--;
                        }
                    }
                    curSqlStmt = new StringBuilder("");
                    curSqlStmt.Append("SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc ");
                    curSqlStmt.Append("FROM CodeValueList ");
                    curSqlStmt.Append("WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')");
                    curSqlStmt.Append("  AND ListCode = '" + curAgeDiv + "-S' ");
                    curRecordDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                    if ( curRecordDataTable.Rows.Count > 0 ) {
                        if ( curRecordDataTable.Rows[0]["MaxValue"] != System.DBNull.Value ) {
                            Decimal curRecordScore = (Decimal) curRecordDataTable.Rows[0]["MaxValue"];
                            Decimal curScore = 0;
                            try {
                                curScore = Convert.ToDecimal(inScore);
                            } catch {
                                curScore = 0;
                            }
                            /*
                             * Adjust score for comparision purposes if the comparable division has a minimum speed 
                             */
                            if ( curPassNumMinSpeed < 0 ) {
                                if ( curScore > ( curPassNumMinSpeed * 6 ) ) {
                                    curScore += curPassNumMinSpeed * 6;
                                }
                            }
                            if ( curOrigPassNumMinSpeed < 0 ) {
                                curScore += curOrigPassNumMinSpeed * -6;
                            }
                            if ( curScore >= curRecordScore ) {
                                if ( curReturnMsg.Length > 5 ) {
                                    curReturnMsg.Append("\n\n");
                                }
                                curReturnMsg.Append("\n\nCurrent skier score of " + curScore.ToString("##0.00"));
                                curReturnMsg.Append("\nMatches or exceeds the record for division " + (String) curRecordDataTable.Rows[0]["CodeValue"]
                                    + ", which the skier is eligible for, current record ");
                                String curRecordInfo = "";
                                try {
                                    curRecordInfo = (String) curRecordDataTable.Rows[0]["CodeDesc"];
                                } catch {
                                    curRecordInfo = "No previous record information available";
                                }
                                int curDelim = curRecordInfo.IndexOf('/');
                                if ( curDelim > 5 ) {
                                    if ( curRecordInfo.Substring(curDelim - 1).Equals(" ") ) {
                                        curDelim--;
                                    } else {
                                        curDelim--;
                                        curDelim--;
                                    }
                                    int curDelim2 = curRecordInfo.IndexOf(' ', curDelim + 1);
                                    curReturnMsg.Append("\n" + curRecordScore.ToString("##0.00") + " buoys, ");
                                    curReturnMsg.Append(" " + curRecordInfo.Substring(0, curDelim));
                                    curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(curDelim + 1, curDelim2 - curDelim - 1));
                                    curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim2 + 1));
                                } else {
                                    curReturnMsg.Append("\n" + curRecordScore.ToString("##0.00") + " buoys");
                                    curReturnMsg.Append("\nRecord information " + curRecordInfo);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return curReturnMsg.ToString();
        }

        public String checkRecordTrick( String inDiv, String inScore, short inSkiYearAge, String inGender ) {
            StringBuilder curReturnMsg = new StringBuilder( "" );
            StringBuilder curSqlStmt = new StringBuilder( "" );

            #region Check to see if score is equal to or great than divisions current record score
            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')" );
            curSqlStmt.Append( "  AND ListCode = '" + inDiv + "-T' " );
            DataTable curRecordDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curRecordDataTable.Rows.Count > 0 ) {
                if ( curRecordDataTable.Rows[0]["MaxValue"] != System.DBNull.Value ) {
                    Decimal curRecordScore = (Decimal)curRecordDataTable.Rows[0]["MaxValue"];
                    Decimal curScore = 0;
                    try {
                        curScore = Convert.ToDecimal( inScore );
                    } catch {
                        curScore = 0;
                    }
                    if ( curScore >= curRecordScore ) {
                        curReturnMsg.Append( "Current skier score of " + curScore.ToString( "##,##0" ) );
                        curReturnMsg.Append("\nMatches or exceeds the " + (String) curRecordDataTable.Rows[0]["CodeValue"] + " division's current record ");
                        String curRecordInfo = "";
                        try {
                            curRecordInfo = (String)curRecordDataTable.Rows[0]["CodeDesc"];
                        } catch {
                            curRecordInfo = "No previous record information available";
                        }
                        int curDelim = curRecordInfo.IndexOf( '/' );
                        if ( curDelim > 0 ) {
                            int curDelim2 = curRecordInfo.IndexOf( ' ', curDelim );
                            curReturnMsg.Append( "\n" + curRecordScore.ToString( "##,##0" ) + " points" );
                            curReturnMsg.Append( "\nRecord set on " + curRecordInfo.Substring( 0, curDelim2 ) );
                            curReturnMsg.Append( "\nRecord holder(s) " + curRecordInfo.Substring( curDelim2 + 1 ) );
                        } else {
                            curReturnMsg.Append( "\n" + curRecordScore.ToString( "##,##0" ) + " points" );
                            curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo );
                        }
                    }
                }
            }
            #endregion

            #region Check scores for comparable age divisions 
            String curGender = inGender;
            if ( inGender == null || inGender.Equals("") ) {
                curGender = myAgeGroupList.getGenderOfAgeDiv(inDiv);
            }
            ArrayList curDropdownList = myAgeGroupList.getComparableDivListForAge(inDiv, inSkiYearAge, curGender, "Trick");
            IEnumerator myList = curDropdownList.GetEnumerator();
            while ( myList.MoveNext() ) {
                string curEntry = (String) myList.Current;
                string curAgeDiv = curEntry.Substring(0, curEntry.IndexOf(" "));

                if ( curAgeDiv.Equals(inDiv) ) {
                    //Divisions already checked
                } else {
                    curSqlStmt = new StringBuilder("");
                    curSqlStmt.Append("SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc ");
                    curSqlStmt.Append("FROM CodeValueList ");
                    curSqlStmt.Append("WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')");
                    curSqlStmt.Append("  AND ListCode = '" + curAgeDiv + "-T' ");
                    curRecordDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                    if ( curRecordDataTable.Rows.Count > 0 ) {
                        if ( curRecordDataTable.Rows[0]["MaxValue"] != System.DBNull.Value ) {
                            Decimal curRecordScore = (Decimal) curRecordDataTable.Rows[0]["MaxValue"];
                            Decimal curScore = 0;
                            try {
                                curScore = Convert.ToDecimal(inScore);
                            } catch {
                                curScore = 0;
                            }
                            if ( curScore >= curRecordScore ) {
                                if ( curReturnMsg.Length > 5 ) {
                                    curReturnMsg.Append("\n\n");
                                }
                                curReturnMsg.Append("\n\nCurrent skier score of " + curScore.ToString("##,##0"));
                                curReturnMsg.Append("\nMatches or exceeds the record for division " + (String) curRecordDataTable.Rows[0]["CodeValue"] 
                                    + ", which the skier is eligible for, current record ");
                                String curRecordInfo = "";
                                try {
                                    curRecordInfo = (String) curRecordDataTable.Rows[0]["CodeDesc"];
                                } catch {
                                    curRecordInfo = "No previous record information available";
                                }
                                int curDelim = curRecordInfo.IndexOf('/');
                                if ( curDelim > 0 ) {
                                    int curDelim2 = curRecordInfo.IndexOf(' ', curDelim);
                                    curReturnMsg.Append("\n" + curRecordScore.ToString("##,##0") + " points");
                                    curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(0, curDelim2));
                                    curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim2 + 1));
                                } else {
                                    curReturnMsg.Append("\n" + curRecordScore.ToString("##,##0") + " points");
                                    curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo);
                                }
                            }
                        }
                    }
                }
            }
            #endregion


            return curReturnMsg.ToString();
        }

        public String checkRecordJump( String inDiv, String inScoreFeet, String inScoreMeters, short inSkiYearAge, String inGender ) {
            StringBuilder curReturnMsg = new StringBuilder( "" );
            StringBuilder curSqlStmt = new StringBuilder( "" );

            #region Check to see if score is equal to or great than divisions current record score
            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')" );
            curSqlStmt.Append( "  AND ListCode = '" + inDiv + "-J' " );
            DataTable curRecordDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curRecordDataTable.Rows.Count > 0 ) {
                if ( curRecordDataTable.Rows[0]["MaxValue"] != System.DBNull.Value ) {
                    Decimal curRecordScore = (Decimal)curRecordDataTable.Rows[0]["MaxValue"];
                    Decimal curScore = 0;
                    try {
                        if ( ( (String) curRecordDataTable.Rows[0]["ListName"] ).Equals("IwwfRecords") ) {
                            curScore = Convert.ToDecimal(inScoreMeters);
                        } else {
                            curScore = Convert.ToDecimal(inScoreFeet);
                        }
                    } catch {
                        curScore = 0;
                    }
                    if ( curScore >= curRecordScore ) {
                        curReturnMsg.Append("Current skier score of " + curScore.ToString("##0.00"));
                        curReturnMsg.Append("\nMatches or exceeds the " + (String) curRecordDataTable.Rows[0]["CodeValue"] + " division's current record ");
                        String curRecordInfo = (String) curRecordDataTable.Rows[0]["CodeDesc"];
                        int curDelim = curRecordInfo.IndexOf(' ');
                        if ( curDelim > 0 ) {
                            int curDelim2 = curRecordInfo.IndexOf('/');
                            if ( curDelim > curDelim2 ) {
                                curReturnMsg.Append("\n" + curRecordScore.ToString("##0.0") + " " + curRecordInfo.Substring(0, curDelim + 1));
                                curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(0, curDelim - 1));
                                curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim + 1));
                            } else {
                                int curDelim3 = curRecordInfo.IndexOf(' ', curDelim2);
                                if ( curDelim > 5 ) {
                                    curReturnMsg.Append("\n" + curRecordScore.ToString("##0.0") + " " + curRecordInfo.Substring(0, curDelim + 1));
                                    curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(curDelim + 1, curDelim3 - curDelim - 1));
                                    curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim3 + 1));
                                } else {
                                    curReturnMsg.Append("\n" + curRecordScore.ToString("##0.0") + " " + curRecordInfo.Substring(0, curDelim + 1));
                                    curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(curDelim + 1, curDelim3 - curDelim - 1));
                                    curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim3 + 1));
                                }
                            }
                        } else {
                            curReturnMsg.Append("\n" + curRecordInfo);
                        }
                    }
                }
            }
            #endregion

            #region Check scores for comparable age divisions 
            String curGender = inGender;
            if ( inGender == null || inGender.Equals("") ) {
                curGender = myAgeGroupList.getGenderOfAgeDiv(inDiv);
            }
            ArrayList curDropdownList = myAgeGroupList.getComparableDivListForAge(inDiv, inSkiYearAge, curGender, "Jump");
            IEnumerator myList = curDropdownList.GetEnumerator();
            while ( myList.MoveNext() ) {
                string curEntry = (String) myList.Current;
                string curAgeDiv = curEntry.Substring(0, curEntry.IndexOf(" "));

                if ( curAgeDiv.Equals(inDiv) ) {
                    //Divisions already checked
                } else {
                    curSqlStmt = new StringBuilder("");
                    curSqlStmt.Append("SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc ");
                    curSqlStmt.Append("FROM CodeValueList ");
                    curSqlStmt.Append("WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')");
                    curSqlStmt.Append("  AND ListCode = '" + curAgeDiv + "-J' ");
                    curRecordDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                    if ( curRecordDataTable.Rows.Count > 0 ) {
                        if ( curRecordDataTable.Rows[0]["MaxValue"] != System.DBNull.Value ) {
                            Decimal curRecordScore = (Decimal) curRecordDataTable.Rows[0]["MaxValue"];
                            Decimal curScore = 0;
                            try {
                                if ( ( (String) curRecordDataTable.Rows[0]["ListName"] ).Equals("IwwfRecords") ) {
                                    curScore = Convert.ToDecimal(inScoreMeters);
                                } else {
                                    curScore = Convert.ToDecimal(inScoreFeet);
                                }
                            } catch {
                                curScore = 0;
                            }
                            if ( curScore >= curRecordScore ) {
                                curReturnMsg.Append("\n\nCurrent skier score of " + curScore.ToString("##0.00"));
                                curReturnMsg.Append("\nMatches or exceeds the record for division " + (String) curRecordDataTable.Rows[0]["CodeValue"]
                                    + ", which the skier is eligible for, current record ");
                                String curRecordInfo = (String) curRecordDataTable.Rows[0]["CodeDesc"];
                                int curDelim = curRecordInfo.IndexOf(' ');
                                if ( curDelim > 0 ) {
                                    int curDelim2 = curRecordInfo.IndexOf('/');
                                    if ( curDelim > curDelim2 ) {
                                        curReturnMsg.Append("\n" + curRecordScore.ToString("##0.0") + " " + curRecordInfo.Substring(0, curDelim + 1));
                                        curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(0, curDelim - 1));
                                        curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim + 1));
                                    } else {
                                        int curDelim3 = curRecordInfo.IndexOf(' ', curDelim2);
                                        if ( curDelim > 5 ) {
                                            curReturnMsg.Append("\n" + curRecordScore.ToString("##0.0") + " " + curRecordInfo.Substring(0, curDelim + 1));
                                            curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(curDelim + 1, curDelim3 - curDelim - 1));
                                            curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim3 + 1));
                                        } else {
                                            curReturnMsg.Append("\n" + curRecordScore.ToString("##0.0") + " " + curRecordInfo.Substring(0, curDelim + 1));
                                            curReturnMsg.Append("\nRecord set on " + curRecordInfo.Substring(curDelim + 1, curDelim3 - curDelim - 1));
                                            curReturnMsg.Append("\nRecord holder(s) " + curRecordInfo.Substring(curDelim3 + 1));
                                        }
                                    }
                                } else {
                                    curReturnMsg.Append("\n" + curRecordInfo);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return curReturnMsg.ToString();
        }
    }
}
