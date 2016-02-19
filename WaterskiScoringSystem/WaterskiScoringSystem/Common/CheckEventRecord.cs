using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    class CheckEventRecord {

        public CheckEventRecord() {
        }

        public String checkRecordSlalom( String inDiv, String inScore ) {
            StringBuilder curReturnMsg = new StringBuilder( "" );
            StringBuilder curSqlStmt = new StringBuilder( "" );

            #region Check to see if score is equal to or great than divisions current record score
            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')" );
            curSqlStmt.Append( "  AND ListCode = '" + inDiv + "-S' " );
            DataTable curRecordDataTable = getData( curSqlStmt.ToString() );
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
                        curReturnMsg.Append( "Current skier score of " + curScore.ToString( "##0.00" ) );
                        curReturnMsg.Append( "\nMatches or exceeds the division's " + inDiv + " current record" );
                        String curRecordInfo = (String)curRecordDataTable.Rows[0]["CodeDesc"];
                        int curDelim = curRecordInfo.IndexOf( '/' );
                        int curDelim2 = curRecordInfo.IndexOf( ' ', curDelim );
                        if ( curDelim > 5 ) {
                            if ( curRecordInfo.Substring(curDelim - 1).Equals(" ") ) {
                                curDelim--;
                            } else {
                                curDelim--;
                                curDelim--;
                            }
                            curReturnMsg.Append( "\n" + curRecordScore.ToString( "##0.00" ) + " buoys, " );
                            curReturnMsg.Append( " " + curRecordInfo.Substring( 0, curDelim ) );
                            curReturnMsg.Append( "\nRecord set on " + curRecordInfo.Substring( curDelim + 1, curDelim2 - curDelim - 1 ) );
                            curReturnMsg.Append( "\nRecord holder(s) " + curRecordInfo.Substring( curDelim2 + 1 ) );
                        } else {
                            curReturnMsg.Append( "\n" + curRecordScore.ToString( "##0.00" ) + " buoys" );
                            curReturnMsg.Append( "\nRecord information " + curRecordInfo );
                        }
                    }
                }
            }
            #endregion

            #region Check scores for OM/OW against IM/IW equal to or great than divisions current record score
            if ( inDiv.Equals( "OM" ) || inDiv.Equals( "OW" ) || inDiv.Equals( "IM" ) || inDiv.Equals( "IW" ) ) {
                String curDiv = "";
                if ( inDiv.Equals( "OM" ) ) {
                    curDiv = "IM";
                } else if ( inDiv.Equals( "OW" ) ) {
                    curDiv = "IW";
                } else if ( inDiv.Equals( "IM" ) ) {
                    curDiv = "OM";
                } else if ( inDiv.Equals( "IW" ) ) {
                    curDiv = "OW";
                }
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc " );
                curSqlStmt.Append( "FROM CodeValueList " );
                curSqlStmt.Append( "WHERE ListName LIKE '%AgeGroup'" );
                curSqlStmt.Append( "  AND ListCode = '" + curDiv + "-S' " );
                curRecordDataTable = getData( curSqlStmt.ToString() );
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
                            if ( curReturnMsg.Length > 5 ) {
                                curReturnMsg.Append( "\n\n" );
                            }
                            curReturnMsg.Append( "Current skier score of " + curScore.ToString( "##0.00" ) );
                            curReturnMsg.Append( "\nMatches or exceeds the division's " + curDiv + " current record" );
                            String curRecordInfo = "";
                            try {
                                curRecordInfo = (String)curRecordDataTable.Rows[0]["CodeDesc"];
                            } catch {
                                curRecordInfo = "No previous record information available";
                            }
                            int curDelim = curRecordInfo.IndexOf( '/' );
                            if ( curDelim > 5 ) {
                                if ( curRecordInfo.Substring( curDelim - 1 ).Equals( " " ) ) {
                                    curDelim--;
                                } else {
                                    curDelim--;
                                    curDelim--;
                                }
                                int curDelim2 = curRecordInfo.IndexOf( ' ', curDelim );
                                curReturnMsg.Append( "\n" + curRecordScore.ToString( "##0.00" ) + " buoys, " );
                                curReturnMsg.Append( " " + curRecordInfo.Substring( 0, curDelim ) );
                                curReturnMsg.Append( "\nRecord set on " + curRecordInfo.Substring( curDelim + 1, curDelim2 - curDelim - 1 ) );
                                curReturnMsg.Append( "\nRecord holder(s) " + curRecordInfo.Substring( curDelim2 + 1 ) );
                            } else {
                                curReturnMsg.Append( "\n" + curRecordScore.ToString( "##0.00" ) + " buoys" );
                                curReturnMsg.Append( "\nRecord information " + curRecordInfo );
                            }
                        }
                    }
                }
            }
            #endregion

            return curReturnMsg.ToString();
        }

        public String checkRecordTrick( String inDiv, String inScore ) {
            StringBuilder curReturnMsg = new StringBuilder( "" );
            StringBuilder curSqlStmt = new StringBuilder( "" );

            #region Check to see if score is equal to or great than divisions current record score
            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')" );
            curSqlStmt.Append( "  AND ListCode = '" + inDiv + "-T' " );
            DataTable curRecordDataTable = getData( curSqlStmt.ToString() );
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
                        curReturnMsg.Append( "\nMatches or exceeds the division's " + inDiv + " current record" );
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
                            curReturnMsg.Append( "\nRecord information " + curRecordInfo );
                        }
                    }
                }
            }
            #endregion

            #region Check scores for OM/OW against IM/IW equal to or great than divisions current record score
            if ( inDiv.Equals( "OM" ) || inDiv.Equals( "OW" ) || inDiv.Equals( "IM" ) || inDiv.Equals( "IW" ) ) {
                String curDiv = "";
                if ( inDiv.Equals( "OM" ) ) {
                    curDiv = "IM";
                } else if ( inDiv.Equals( "OW" ) ) {
                    curDiv = "IW";
                } else if ( inDiv.Equals( "IM" ) ) {
                    curDiv = "OM";
                } else if ( inDiv.Equals( "IW" ) ) {
                    curDiv = "OW";
                }
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc " );
                curSqlStmt.Append( "FROM CodeValueList " );
                curSqlStmt.Append( "WHERE ListName LIKE '%AgeGroup'" );
                curSqlStmt.Append( "  AND ListCode = '" + curDiv + "-T' " );
                curRecordDataTable = getData( curSqlStmt.ToString() );
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
                            if ( curReturnMsg.Length > 5 ) {
                                curReturnMsg.Append( "\n\n" );
                            }
                            curReturnMsg.Append( "Current skier score of " + curScore.ToString( "##0.00" ) );
                            curReturnMsg.Append( "\nMatches or exceeds the division's " + curDiv + " current record" );
                            String curRecordInfo = (String)curRecordDataTable.Rows[0]["CodeDesc"];
                            int curDelim = curRecordInfo.IndexOf( '/' );
                            int curDelim2 = curRecordInfo.IndexOf( ' ', curDelim );
                            if ( curDelim > 0 ) {
                                curReturnMsg.Append( "\n" + curRecordScore.ToString( "##,##0" ) + " points" );
                                curReturnMsg.Append( "\nRecord set on " + curRecordInfo.Substring( 0, curDelim2 ) );
                                curReturnMsg.Append( "\nRecord holder(s) " + curRecordInfo.Substring( curDelim2 + 1 ) );
                            } else {
                                curReturnMsg.Append( "\n" + curRecordScore.ToString( "##,##0" ) + " points" );
                                curReturnMsg.Append( "\nRecord information " + curRecordInfo );
                            }
                        }
                    }
                }
            }
            #endregion


            return curReturnMsg.ToString();
        }

        public String checkRecordJump( String inDiv, String inScoreFeet, String inScoreMeters ) {
            StringBuilder curReturnMsg = new StringBuilder( "" );
            StringBuilder curSqlStmt = new StringBuilder( "" );

            #region Check to see if score is equal to or great than divisions current record score
            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName in ('AWSARecords', 'IwwfRecords', 'NCWSARecords')" );
            curSqlStmt.Append( "  AND ListCode = '" + inDiv + "-J' " );
            DataTable curRecordDataTable = getData( curSqlStmt.ToString() );
            if ( curRecordDataTable.Rows.Count > 0 ) {
                if ( curRecordDataTable.Rows[0]["MaxValue"] != System.DBNull.Value ) {
                    Decimal curRecordScore = (Decimal)curRecordDataTable.Rows[0]["MaxValue"];
                    Decimal curScore = 0;
                    try {
                        if ( ((String)curRecordDataTable.Rows[0]["ListName"]).Equals("IwwfRecords")) {
                            curScore = Convert.ToDecimal( inScoreMeters );
                        } else {
                            curScore = Convert.ToDecimal( inScoreFeet );
                        }
                    } catch {
                        curScore = 0;
                    }
                    if ( curScore >= curRecordScore ) {
                        curReturnMsg.Append("Current skier score of " + curScore.ToString( "##0.00" ));
                        curReturnMsg.Append( "\nMatches or exceeds the division's " + inDiv + " current record" );
                        String curRecordInfo = (String)curRecordDataTable.Rows[0]["CodeDesc"];
                        int curDelim = curRecordInfo.IndexOf( ' ' );
                        int curDelim2 = curRecordInfo.IndexOf( '/', curDelim );
                        int curDelim3 = curRecordInfo.IndexOf( ' ', curDelim2 );
                        if ( curDelim > 5 ) {
                            curReturnMsg.Append( "\n" + curRecordScore.ToString( "##0.0" ) + " " + curRecordInfo.Substring( 0, curDelim + 1 ) );
                            curReturnMsg.Append( "\nRecord set on " + curRecordInfo.Substring( curDelim + 1, curDelim3 - curDelim - 1 ) );
                            curReturnMsg.Append( "\nRecord holder(s) " + curRecordInfo.Substring( curDelim3 + 1 ) );
                        } else {
                            curReturnMsg.Append( "\nRecord set on " + curRecordInfo.Substring( curDelim + 1, curDelim3 - curDelim - 1 ) );
                            curReturnMsg.Append( "\nRecord holder(s) " + curRecordInfo.Substring( curDelim3 + 1 ) );
                        }
                    }
                }
            }
            #endregion

            #region Check scores for OM/OW against IM/IW equal to or great than divisions current record score
            if ( inDiv.Equals( "OM" ) || inDiv.Equals( "OW" ) || inDiv.Equals( "IM" ) || inDiv.Equals( "IW" ) ) {
                String curDiv = "";
                if ( inDiv.Equals( "OM" ) ) {
                    curDiv = "IM";
                } else if ( inDiv.Equals( "OW" ) ) {
                    curDiv = "IW";
                } else if ( inDiv.Equals( "IM" ) ) {
                    curDiv = "OM";
                } else if ( inDiv.Equals( "IW" ) ) {
                    curDiv = "OW";
                }
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, MaxValue, CodeDesc " );
                curSqlStmt.Append( "FROM CodeValueList " );
                curSqlStmt.Append( "WHERE ListName LIKE '%AgeGroup'" );
                curSqlStmt.Append( "  AND ListCode = '" + curDiv + "-J' " );
                curRecordDataTable = getData( curSqlStmt.ToString() );
                if ( curRecordDataTable.Rows.Count > 0 ) {
                    if ( curRecordDataTable.Rows[0]["MaxValue"] != System.DBNull.Value ) {
                        Decimal curRecordScore = (Decimal)curRecordDataTable.Rows[0]["MaxValue"];
                        Decimal curScore = 0;
                        try {
                            if ( ( (String)curRecordDataTable.Rows[0]["ListName"] ).Equals( "IwwfRecords" ) ) {
                                curScore = Convert.ToDecimal( inScoreMeters );
                            } else {
                                curScore = Convert.ToDecimal( inScoreFeet );
                            }
                        } catch {
                            curScore = 0;
                        }
                        if ( curScore >= curRecordScore ) {
                            if ( curReturnMsg.Length > 5 ) {
                                curReturnMsg.Append( "\n\n" );
                            }

                            curReturnMsg.Append( "Current skier score of " + curScore.ToString( "##0.0" ) );
                            curReturnMsg.Append( "\nMatches or exceeds the division's " + curDiv + " current record" );
                            String curRecordInfo = "";
                            try {
                                curRecordInfo = (String)curRecordDataTable.Rows[0]["CodeDesc"];
                            } catch {
                                curRecordInfo = "No previous record information available";
                            }
                            int curDelim = curRecordInfo.IndexOf( ' ' );
                            if ( curDelim > 0 ) {
                                int curDelim2 = curRecordInfo.IndexOf( '/', curDelim );
                                int curDelim3 = curRecordInfo.IndexOf( ' ', curDelim2 );
                                if (curDelim > 5) {
                                    curReturnMsg.Append( "\n" + curRecordScore.ToString( "##0.0" ) + " " + curRecordInfo.Substring( 0, curDelim + 1 ) );
                                    curReturnMsg.Append( "\nRecord set on " + curRecordInfo.Substring( curDelim + 1, curDelim3 - curDelim - 1 ) );
                                    curReturnMsg.Append( "\nRecord holder(s) " + curRecordInfo.Substring( curDelim3 + 1 ) );
                                } else {
                                    curReturnMsg.Append( "\nRecord set on " + curRecordInfo.Substring( curDelim + 1, curDelim3 - curDelim - 1 ) );
                                    curReturnMsg.Append( "\nRecord holder(s) " + curRecordInfo.Substring( curDelim3 + 1 ) );
                                }
                            } else {
                                curReturnMsg.Append( "\n" + curRecordInfo );
                            }
                        }
                    }
                }
            }
            #endregion
            return curReturnMsg.ToString();
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    
    }
}
