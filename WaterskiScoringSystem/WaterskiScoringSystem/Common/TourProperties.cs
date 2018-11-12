using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using System.Reflection;

namespace WaterskiScoringSystem.Common {
    class TourProperties {
        private static readonly TourProperties myInstance = new TourProperties();
        private String myTourRules = "";
        private String myTourClass = "";

        private TourProperties() {}
        public static TourProperties Instance {
            get { return myInstance; }
        }

        public bool loadProperties(String inTourRules, String inTourClass) {
            myTourRules = inTourRules;
            myTourClass = inTourClass;

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PropKey, PropValue " );
            curSqlStmt.Append( "FROM TourProperties " );
            curSqlStmt.Append( "WHERE SanctionId = '" + Properties.Settings.Default.AppSanctionNum + "' " );
            curSqlStmt.Append( "Order by PropOrder, PropKey, PropValue " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );

            if (curDataTable.Rows.Count > 0) {
                Type curClassType = typeof( TourProperties );
                Object[] curPropValue = new Object[] { 1 };
                TourProperties curTourProperties = TourProperties.Instance;
                foreach (DataRow curRow in curDataTable.Rows) {
                    if (( (String)curRow["PropKey"] ).ToLower().Equals( "videotags" )) {
                    } else {
                        if ( curRow["PropValue"] == System.DBNull.Value ) {
                            curClassType.InvokeMember("set" + (String) curRow["PropKey"]
                                , BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod
                                , null
                                , curTourProperties
                                , new Object[] { "" });
                        } else {
                            curClassType.InvokeMember("set" + (String) curRow["PropKey"]
                                , BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod
                                , null
                                , curTourProperties
                                , new Object[] { (String) curRow["PropValue"] });
                        }
                    }
                }
                return true;
            } else {
                /*
                * Use reflection to get list of methods and then initialize all the property values
                */

                return false;
            }
        }

        //-----------------------------------------------------
        #region Master Summary methods
        public String MasterSummaryDataType {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryDataType");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "best";
                    setMasterSummaryDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setMasterSummaryDataType( value ); }
        }
        public void setMasterSummaryDataType(String value) {
            updateProperty( "MasterSummaryDataType", value, 100 );
        }

        public String MasterSummaryPointsMethod {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "nops";
                    setMasterSummaryPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set {setMasterSummaryPointsMethod( value );}
        }
        public void setMasterSummaryPointsMethod(String value) {
            updateProperty( "MasterSummaryPointsMethod", value, 100 );
        }

        public String MasterSummaryPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "score";
                    setMasterSummaryPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setMasterSummaryPlcmtMethod( value ); }
        }
        public void setMasterSummaryPlcmtMethod(String value) {
            updateProperty( "MasterSummaryPlcmtMethod", value, 100 );
        }

        public String MasterSummaryPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "div";
                    setMasterSummaryPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setMasterSummaryPlcmtOrg( value ); }
        }
        public void setMasterSummaryPlcmtOrg(String value) {
            updateProperty( "MasterSummaryPlcmtOrg", value, 100 );
        }
        #endregion

        //-----------------------------------------------------
        #region Master Overall Summary methods
        public String MasterSummaryOverallDataType {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryOverallDataType");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "round";
                    setMasterSummaryOverallDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setMasterSummaryOverallDataType( value ); }
        }
        public void setMasterSummaryOverallDataType(String value) {
            updateProperty( "MasterSummaryOverallDataType", value, 110 );
        }

        public String MasterSummaryOverallPointsMethod {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryOverallPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "nops";
                    setMasterSummaryOverallPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setMasterSummaryOverallPointsMethod( value ); }
        }
        public void setMasterSummaryOverallPointsMethod(String value) {
            updateProperty( "MasterSummaryOverallPointsMethod", value, 110 );
        }

        public String MasterSummaryOverallPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryOverallPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "points";
                    setMasterSummaryOverallPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setMasterSummaryOverallPlcmtMethod( value ); }
        }
        public void setMasterSummaryOverallPlcmtMethod(String value) {
            updateProperty( "MasterSummaryOverallPlcmtMethod", value, 110 );
        }

        public String MasterSummaryOverallPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryOverallPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "div";
                    setMasterSummaryOverallPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setMasterSummaryOverallPlcmtOrg( value ); }
        }
        public void setMasterSummaryOverallPlcmtOrg(String value) {
            updateProperty( "MasterSummaryOverallPlcmtOrg", value, 110 );
        }

        public String MasterSummaryOverallFilter {
            get {
                String curReturnValue = getPropertyByName("MasterSummaryOverallFilter");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "all";
                    setMasterSummaryOverallFilter(curReturnValue);
                }
                return curReturnValue;
            }
            set { setMasterSummaryOverallFilter( value ); }
        }
        public void setMasterSummaryOverallFilter(String value) {
            updateProperty( "MasterSummaryOverallFilter", value, 110 );
        }
        #endregion

        //-----------------------------------------------------
        #region Master Team Summary methods
        public String TeamSummaryDataType {
            get {
                String curReturnValue = getPropertyByName("TeamSummaryDataType");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "best";
                    setTeamSummaryDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTeamSummaryDataType( value ); }
        }
        public void setTeamSummaryDataType(String value) {
            updateProperty( "TeamSummaryDataType", value, 900 );
        }

        public String TeamSummaryPointsMethod {
            get {
                String curReturnValue = getPropertyByName("TeamSummaryPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "nops";
                    setTeamSummaryPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTeamSummaryPointsMethod( value ); }
        }
        public void setTeamSummaryPointsMethod(String value) {
            updateProperty( "TeamSummaryPointsMethod", value, 900 );
        }

        public String TeamSummaryPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("TeamSummaryPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "points";
                    setTeamSummaryPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTeamSummaryPlcmtMethod( value ); }
        }
        public void setTeamSummaryPlcmtMethod(String value) {
            updateProperty( "TeamSummaryPlcmtMethod", value, 900 );
        }

        public String TeamSummaryPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("TeamSummaryPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "div";
                    setTeamSummaryPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTeamSummaryPlcmtOrg( value ); }
        }
        public void setTeamSummaryPlcmtOrg(String value) {
            updateProperty( "TeamSummaryPlcmtOrg", value, 900 );
        }

        public String TeamSummary_NumPerTeam {
            get {
                String curReturnValue = getPropertyByName("TeamSummary_NumPerTeam");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "4";
                    } else {
                        curReturnValue = "0";
                    }
                    setTeamSummary_NumPerTeam(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTeamSummary_NumPerTeam( value ); }
        }
        public void setTeamSummary_NumPerTeam(String value) {
            updateProperty( "TeamSummary_NumPerTeam", value, 900 );
        }
        #endregion

        #region Slalom Team Summary methods
        public String SlalomTeamSummaryDataType {
            get {
                String curReturnValue = getPropertyByName("SlalomTeamSummaryDataType");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "points";
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setSlalomTeamSummaryDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomTeamSummaryDataType( value ); }
        }
        public void setSlalomTeamSummaryDataType(String value) {
            updateProperty( "SlalomTeamSummaryDataType", value, 910 );
        }

        public String SlalomTeamSummaryPointsMethod {
            get {
                String curReturnValue = getPropertyByName("SlalomTeamSummaryPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setSlalomTeamSummaryPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomTeamSummaryPointsMethod( value ); }
        }
        public void setSlalomTeamSummaryPointsMethod(String value) {
            updateProperty( "SlalomTeamSummaryPointsMethod", value, 910 );
        }

        public String SlalomTeamSummaryPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("SlalomTeamSummaryPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setSlalomTeamSummaryPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomTeamSummaryPlcmtMethod( value ); }
        }
        public void setSlalomTeamSummaryPlcmtMethod(String value) {
            updateProperty( "SlalomTeamSummaryPlcmtMethod", value, 910 );
        }

        public String SlalomTeamSummaryPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("SlalomTeamSummaryPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "tour";
                    }
                    setSlalomTeamSummaryPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomTeamSummaryPlcmtOrg( value ); }
        }
        public void setSlalomTeamSummaryPlcmtOrg(String value) {
            updateProperty( "SlalomTeamSummaryPlcmtOrg", value, 910 );
        }

        public String SlalomTeamSummary_NumPerTeam {
            get {
                String curReturnValue = getPropertyByName("SlalomTeamSummary_NumPerTeam");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "4";
                    } else {
                        curReturnValue = "0";
                    }
                    setSlalomTeamSummary_NumPerTeam(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomTeamSummary_NumPerTeam( value ); }
        }
        public void setSlalomTeamSummary_NumPerTeam(String value) {
            updateProperty( "SlalomTeamSummary_NumPerTeam", value, 910 );
        }
        #endregion

        #region Trick Team Summary methods
        public String TrickTeamSummaryDataType {
            get {
                String curReturnValue = getPropertyByName("TrickTeamSummaryDataType");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "points";
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setTrickTeamSummaryDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickTeamSummaryDataType( value ); }
        }
        public void setTrickTeamSummaryDataType(String value) {
            updateProperty( "TrickTeamSummaryDataType", value, 920 );
        }

        public String TrickTeamSummaryPointsMethod {
            get {
                String curReturnValue = getPropertyByName("TrickTeamSummaryPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setTrickTeamSummaryPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickTeamSummaryPointsMethod( value ); }
        }
        public void setTrickTeamSummaryPointsMethod(String value) {
            updateProperty( "TrickTeamSummaryPointsMethod", value, 920 );
        }

        public String TrickTeamSummaryPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("TrickTeamSummaryPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setTrickTeamSummaryPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickTeamSummaryPlcmtMethod( value ); }
        }
        public void setTrickTeamSummaryPlcmtMethod(String value) {
            updateProperty( "TrickTeamSummaryPlcmtMethod", value, 920 );
        }

        public String TrickTeamSummaryPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("TrickTeamSummaryPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "tour";
                    }
                    setTrickTeamSummaryPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickTeamSummaryPlcmtOrg( value ); }
        }
        public void setTrickTeamSummaryPlcmtOrg(String value) {
            updateProperty( "TrickTeamSummaryPlcmtOrg", value, 920 );
        }

        public String TrickTeamSummary_NumPerTeam {
            get {
                String curReturnValue = getPropertyByName("TrickTeamSummary_NumPerTeam");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "4";
                    } else {
                        curReturnValue = "0";
                    }
                    setTrickTeamSummary_NumPerTeam(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickTeamSummary_NumPerTeam( value ); }
        }
        public void setTrickTeamSummary_NumPerTeam(String value) {
            updateProperty( "TrickTeamSummary_NumPerTeam", value, 920 );
        }
        #endregion

        #region Jump Team Summary methods
        public String JumpTeamSummaryDataType {
            get {
                String curReturnValue = getPropertyByName("JumpTeamSummaryDataType");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setJumpTeamSummaryDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpTeamSummaryDataType( value ); }
        }
        public void setJumpTeamSummaryDataType(String value) {
            updateProperty( "JumpTeamSummaryDataType", value, 930 );
        }

        public String JumpTeamSummaryPointsMethod {
            get {
                String curReturnValue = getPropertyByName("JumpTeamSummaryPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setJumpTeamSummaryPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpTeamSummaryPointsMethod( value ); }
        }
        public void setJumpTeamSummaryPointsMethod(String value) {
            updateProperty( "JumpTeamSummaryPointsMethod", value, 930 );
        }

        public String JumpTeamSummaryPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("JumpTeamSummaryPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setJumpTeamSummaryPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpTeamSummaryPlcmtMethod( value ); }
        }
        public void setJumpTeamSummaryPlcmtMethod(String value) {
            updateProperty( "JumpTeamSummaryPlcmtMethod", value, 930 );
        }

        public String JumpTeamSummaryPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("JumpTeamSummaryPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "tour";
                    }
                    setJumpTeamSummaryPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpTeamSummaryPlcmtOrg( value ); }
        }
        public void setJumpTeamSummaryPlcmtOrg(String value) {
            updateProperty( "JumpTeamSummaryPlcmtOrg", value, 930 );
        }

        public String JumpTeamSummary_NumPerTeam {
            get {
                String curReturnValue = getPropertyByName("JumpTeamSummary_NumPerTeam");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "4";
                    } else {
                        curReturnValue = "0";
                    }
                    setJumpTeamSummary_NumPerTeam(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpTeamSummary_NumPerTeam( value ); }
        }
        public void setJumpTeamSummary_NumPerTeam(String value) {
            updateProperty( "JumpTeamSummary_NumPerTeam", value, 930 );
        }
        #endregion

        //-----------------------------------------------------
        #region Slalom Summary methods
        public String SlalomSummaryDataType {
            get {
                String curReturnValue = getPropertyByName("SlalomSummaryDataType");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "best";
                    setSlalomSummaryDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomSummaryDataType( value ); }
        }
        public void setSlalomSummaryDataType(String value) {
            updateProperty( "SlalomSummaryDataType", value, 300 );
        }

        public String SlalomSummaryPointsMethod {
            get {
                String curReturnValue = getPropertyByName("SlalomSummaryPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setSlalomSummaryPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomSummaryPointsMethod( value ); }
        }
        public void setSlalomSummaryPointsMethod(String value) {
            updateProperty( "SlalomSummaryPointsMethod", value, 300 );
        }

        public String SlalomSummaryPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("SlalomSummaryPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setSlalomSummaryPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomSummaryPlcmtMethod( value ); }
        }
        public void setSlalomSummaryPlcmtMethod(String value) {
            updateProperty( "SlalomSummaryPlcmtMethod", value, 300 );
        }

        public String SlalomSummaryPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("SlalomSummaryPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "div";
                    }
                    setSlalomSummaryPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomSummaryPlcmtOrg( value ); }
        }
        public void setSlalomSummaryPlcmtOrg(String value) {
            updateProperty( "SlalomSummaryPlcmtOrg", value, 300 );
        }
        
        public String SlalomSummaryNumPrelim {
            get {
                String curReturnValue = getPropertyByName("SlalomSummaryNumPrelim");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "0";
                    setSlalomSummaryNumPrelim(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomSummaryNumPrelim( value ); }
        }
        public void setSlalomSummaryNumPrelim(String value) {
            updateProperty( "SlalomSummaryNumPrelim", value, 300 );
        }

        public String SlalomSummaryAwardsNum {
            get {
                String curReturnValue = getPropertyByName("SlalomSummaryAwardsNum");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "5";
                    setSlalomSummaryAwardsNum(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomSummaryAwardsNum( value ); }
        }
        public void setSlalomSummaryAwardsNum(String value) {
            updateProperty( "SlalomSummaryAwardsNum", value, 300 );
        }

        public String SlalomScoreSummary_Sort {
            get {
                String curReturnValue = getPropertyByName("SlalomScoreSummary_Sort");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "";
                    setSlalomScoreSummary_Sort(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomScoreSummary_Sort( value ); }
        }
        public void setSlalomScoreSummary_Sort(String value) {
            updateProperty( "SlalomScoreSummary_Sort", value, 300 );
        }
        #endregion

        //-----------------------------------------------------
        #region Slalom running order methods
        public String RunningOrderSortSlalom {
            get {
                String curReturnValue = getPropertyByName("RunningOrderSortSlalom");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "DivOrder ASC, Div ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC";
                    } else {
                        curReturnValue = "EventGroup ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC";
                    }
                    setRunningOrderSortSlalom(curReturnValue);
                }
                return curReturnValue;
            }
            set { setRunningOrderSortSlalom( value ); }
        }
        public void setRunningOrderSortSlalom(String value) {
            updateProperty( "RunningOrderSortSlalom", value, 310 );
        }

		public Dictionary<string, Boolean> RunningOrderColumnFilterSlalom {
			get {
				Dictionary<string, Boolean> curPropValueList = new Dictionary<string, Boolean>();
				String curPropValue = getPropertyByName( "RunningOrderColumnFilterSlalom" );
				if ( curPropValue.Length == 0 ) {
					setRunningOrderColumnFilterSlalom( curPropValueList );

				} else {
					String[] curEntryValues = curPropValue.Split( ',' );
					foreach (String curEntryValue in curEntryValues ) {
						String[] curEntryAttrs = curEntryValue.Split( ' ' );
						Boolean curAttrVisibility = false;
						if ( curEntryAttrs[1].Equals( "True" ) ) curAttrVisibility = true;
						curPropValueList.Add( curEntryAttrs[0], curAttrVisibility );
					}
				}
				return curPropValueList;
			}
			set { setRunningOrderColumnFilterSlalom( value ); }
		}
		public void setRunningOrderColumnFilterSlalom( Dictionary<string, Boolean> propList ) {
			StringBuilder saveValues = new StringBuilder( "" );
			foreach ( KeyValuePair<string, bool> curEntry in propList ) {
				if ( saveValues.Length > 1 ) saveValues.Append( "," );
                saveValues.Append( curEntry.Key + " " + curEntry.Value );
			}
			updateProperty( "RunningOrderColumnFilterSlalom", saveValues.ToString(), 311 );
		}
		public void setRunningOrderColumnFilterSlalom( String  propValues ) {
			Dictionary<string, Boolean> curPropValueList = new Dictionary<string, Boolean>();
			String[] curEntryValues = propValues.Split( ',' );
			foreach ( String curEntryValue in curEntryValues ) {
				String[] curEntryAttrs = curEntryValue.Split( ' ' );
				Boolean curAttrVisibility = false;
				if ( curEntryAttrs[1].Equals( "True" ) ) curAttrVisibility = true;
				curPropValueList.Add( curEntryAttrs[0], curAttrVisibility );
			}
			setRunningOrderColumnFilterSlalom( curPropValueList );
        }


		#endregion

		//-----------------------------------------------------
		#region Slalom Scorebook methods
		public String SlalomScorebookDataType {
            get {
                String curReturnValue = getPropertyByName("SlalomScorebookDataType");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setSlalomScorebookDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomScorebookDataType( value ); }
        }
        public void setSlalomScorebookDataType(String value) {
            updateProperty( "SlalomScorebookDataType", value, 320 );
        }

        public String SlalomScorebookPointsMethod {
            get {
                String curReturnValue = getPropertyByName("SlalomScorebookPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setSlalomScorebookPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomScorebookPointsMethod( value ); }
        }
        public void setSlalomScorebookPointsMethod(String value) {
            updateProperty( "SlalomScorebookPointsMethod", value, 320 );
        }

        public String SlalomScorebookPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("SlalomScorebookPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "div";
                    }
                    setSlalomScorebookPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomScorebookPlcmtMethod( value ); }
        }
        public void setSlalomScorebookPlcmtMethod(String value) {
            updateProperty( "SlalomScorebookPlcmtMethod", value, 320 );
        }

        public String SlalomScorebookPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("SlalomScorebookPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "div";
                    }
                    setSlalomScorebookPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setSlalomScorebookPlcmtOrg( value ); }
        }
        public void setSlalomScorebookPlcmtOrg(String value) {
            updateProperty( "SlalomScorebookPlcmtOrg", value, 320 );
        }
        #endregion  

        //-----------------------------------------------------
        #region Trick Summary methods
        //private String myTrickSummaryDataType = ""; //myInstance.MasterSummaryDataType;
        public String TrickSummaryDataType {
            get {
                String curReturnValue = getPropertyByName("TrickSummaryDataType");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "best";
                    setTrickSummaryDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickSummaryDataType( value ); }
        }
        public void setTrickSummaryDataType(String value) {
            updateProperty( "TrickSummaryDataType", value, 400 );
        }

        public String TrickSummaryPointsMethod {
            get {
                String curReturnValue = getPropertyByName("TrickSummaryPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setTrickTeamSummaryPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickSummaryPointsMethod( value ); }
        }
        public void setTrickSummaryPointsMethod(String value) {
            updateProperty( "TrickSummaryPointsMethod", value, 400 );
        }

        public String TrickSummaryPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("TrickSummaryPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setTrickSummaryPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickSummaryPlcmtMethod( value ); }
        }
        public void setTrickSummaryPlcmtMethod(String value) {
            updateProperty( "TrickSummaryPlcmtMethod", value, 400 );
        }

        public String TrickSummaryPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("TrickSummaryPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "div";
                    }
                    setTrickSummaryPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickSummaryPlcmtOrg( value ); }
        }
        public void setTrickSummaryPlcmtOrg(String value) {
            updateProperty( "TrickSummaryPlcmtOrg", value, 400 );
        }

        public String TrickSummaryNumPrelim {
            get {
                String curReturnValue = getPropertyByName("TrickSummaryNumPrelim");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "0";
                    setTrickSummaryNumPrelim(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickSummaryNumPrelim( value ); }
        }
        public void setTrickSummaryNumPrelim(String value) {
            updateProperty( "TrickSummaryNumPrelim", value, 400 );
        }

        public String TrickSummaryAwardsNum {
            get {
                String curReturnValue = getPropertyByName("TrickSummaryAwardsNum");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "5";
                    setTrickSummaryAwardsNum(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickSummaryAwardsNum( value ); }
        }
        public void setTrickSummaryAwardsNum(String value) {
            updateProperty( "TrickSummaryAwardsNum", value, 400 );
        }

        public String TrickScoreSummary_Sort {
            get {
                String curReturnValue = getPropertyByName("TrickScoreSummary_Sort");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "";
                    setTrickScoreSummary_Sort(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickScoreSummary_Sort( value ); }
        }
        public void setTrickScoreSummary_Sort(String value) {
            updateProperty( "TrickScoreSummary_Sort", value, 400 );
        }

        #endregion

        //-----------------------------------------------------
        #region Trick running order methods
        public String RunningOrderSortTrick {
            get {
                String curReturnValue = getPropertyByName("RunningOrderSortTrick");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "DivOrder ASC, Div ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC";
                    } else {
                        curReturnValue = "EventGroup ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC";
                    }
                    setRunningOrderSortTrick(curReturnValue);
                }
                return curReturnValue;
            }
            set { setRunningOrderSortTrick( value ); }
        }
        public void setRunningOrderSortTrick(String value) {
            updateProperty( "RunningOrderSortTrick", value, 410 );
        }

		public Dictionary<string, Boolean> RunningOrderColumnFilterTrick {
			get {
				Dictionary<string, Boolean> curPropValueList = new Dictionary<string, Boolean>();
				String curPropValue = getPropertyByName( "RunningOrderColumnFilterTrick" );
				if ( curPropValue.Length == 0 ) {
					setRunningOrderColumnFilterTrick( curPropValueList );
				} else {
					String[] curEntryValues = curPropValue.Split( ',' );
					foreach ( String curEntryValue in curEntryValues ) {
						String[] curEntryAttrs = curEntryValue.Split( ' ' );
						Boolean curAttrVisibility = false;
						if ( curEntryAttrs[1].Equals( "True" ) ) curAttrVisibility = true;
						curPropValueList.Add( curEntryAttrs[0], curAttrVisibility );
					}
				}
				return curPropValueList;
			}
			set { setRunningOrderColumnFilterTrick( value ); }
		}
		public void setRunningOrderColumnFilterTrick( Dictionary<string, Boolean> propList ) {
			StringBuilder saveValues = new StringBuilder( "" );
			foreach ( KeyValuePair<string, bool> curEntry in propList ) {
				if ( saveValues.Length > 1 ) saveValues.Append( "," );
				saveValues.Append( curEntry.Key + " " + curEntry.Value );
			}
			updateProperty( "RunningOrderColumnFilterTrick", saveValues.ToString(), 311 );
		}
		public void setRunningOrderColumnFilterTrick( String propValues ) {
			Dictionary<string, Boolean> curPropValueList = new Dictionary<string, Boolean>();
			String[] curEntryValues = propValues.Split( ',' );
			foreach ( String curEntryValue in curEntryValues ) {
				String[] curEntryAttrs = curEntryValue.Split( ' ' );
				Boolean curAttrVisibility = false;
				if ( curEntryAttrs[1].Equals( "True" ) ) curAttrVisibility = true;
				curPropValueList.Add( curEntryAttrs[0], curAttrVisibility );
			}
			setRunningOrderColumnFilterTrick( curPropValueList );
		}

		#endregion

		#region Trick Scorebook methods
		public String TrickScorebookDataType {
            get {
                String curReturnValue = getPropertyByName("TrickScorebookDataType");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setTrickScorebookDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickScorebookDataType( value ); }
        }
        public void setTrickScorebookDataType(String value) {
            updateProperty( "TrickScorebookDataType", value, 420 );
        }

        public String TrickScorebookPointsMethod {
            get {
                String curReturnValue = getPropertyByName("TrickScorebookPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setTrickScorebookPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickScorebookPointsMethod( value ); }
        }
        public void setTrickScorebookPointsMethod(String value) {
            updateProperty( "TrickScorebookPointsMethod", value, 420 );
        }

        public String TrickScorebookPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("TrickScorebookPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "score";
                    }
                    setTrickScorebookPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickScorebookPlcmtMethod( value ); }
        }
        public void setTrickScorebookPlcmtMethod(String value) {
            updateProperty( "TrickScorebookPlcmtMethod", value, 420 );
        }

        //private String myTrickScorebookPlcmtOrg = "";
        public String TrickScorebookPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("TrickScorebookPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "div";
                    }
                    setTrickScorebookPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setTrickScorebookPlcmtOrg( value ); }
        }
        public void setTrickScorebookPlcmtOrg(String value) {
            updateProperty( "TrickScorebookPlcmtOrg", value, 420 );
        }
#endregion

        //-----------------------------------------------------
        #region Jump Summary methods
        public String JumpSummaryDataType {
            get {
                String curReturnValue = getPropertyByName("JumpSummaryDataType");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "best";
                    setJumpSummaryDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpSummaryDataType( value ); }
        }
        public void setJumpSummaryDataType(String value) {
            updateProperty( "JumpSummaryDataType", value, 500 );
        }

        //private String myJumpSummaryPointsMethod = ""; //myInstance.MasterSummaryPointsMethod;
        public String JumpSummaryPointsMethod {
            get {
                String curReturnValue = getPropertyByName("JumpSummaryPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setJumpSummaryPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpSummaryPointsMethod( value ); }
        }
        public void setJumpSummaryPointsMethod(String value) {
            updateProperty( "JumpSummaryPointsMethod", value, 500 );
        }

        public String JumpSummaryPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("JumpSummaryPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "points";
                    }
                    setJumpSummaryPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpSummaryPlcmtMethod( value ); }
        }
        public void setJumpSummaryPlcmtMethod(String value) {
            updateProperty( "JumpSummaryPlcmtMethod", value, 500 );
        }

        public String JumpSummaryPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("JumpSummaryPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "div";
                    }
                    setJumpSummaryPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpSummaryPlcmtOrg( value ); }
        }
        public void setJumpSummaryPlcmtOrg(String value) {
            updateProperty( "JumpSummaryPlcmtOrg", value, 500 );
        }

        public String JumpSummaryNumPrelim {
            get {
                String curReturnValue = getPropertyByName("JumpSummaryNumPrelim");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "0";
                    setJumpSummaryNumPrelim(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpSummaryNumPrelim( value ); }
        }
        public void setJumpSummaryNumPrelim(String value) {
            updateProperty( "JumpSummaryNumPrelim", value, 500 );
        }

        public String JumpSummaryAwardsNum {
            get {
                String curReturnValue = getPropertyByName("JumpSummaryAwardsNum");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "5";
                    setJumpSummaryAwardsNum(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpSummaryAwardsNum( value ); }
        }
        public void setJumpSummaryAwardsNum(String value) {
            updateProperty( "JumpSummaryAwardsNum", value, 500 );
        }

        public String JumpScoreSummary_Sort {
            get {
                String curReturnValue = getPropertyByName("JumpScoreSummary_Sort");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "";
                    setJumpScoreSummary_Sort(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpScoreSummary_Sort( value ); }
        }
        public void setJumpScoreSummary_Sort(String value) {
            updateProperty( "JumpScoreSummary_Sort", value, 500 );
        }

        #endregion

        //-----------------------------------------------------
        #region Jump methods
        public String RunningOrderSortJump {
            get {
                String curReturnValue = getPropertyByName("RunningOrderSortJump");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "DivOrder ASC, Div ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC";
                    } else {
                        curReturnValue = "EventGroup ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC";
                    }
                    setRunningOrderSortJump(curReturnValue);
                }
                return curReturnValue;
            }
            set { setRunningOrderSortJump( value ); }
        }
        public void setRunningOrderSortJump(String value) {
            updateProperty( "RunningOrderSortJump", value, 510 );
        }

        //JumpEntryNumJudges
        public String JumpEntryNumJudges {
            get {
                String curReturnValue = getPropertyByName("JumpEntryNumJudges");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "0";
                    setJumpEntryNumJudges(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpEntryNumJudges( value ); }
        }
        public void setJumpEntryNumJudges(String value) {
            updateProperty( "JumpEntryNumJudges", value, 510 );
        }

		public Dictionary<string, Boolean> RunningOrderColumnFilterJump {
			get {
				Dictionary<string, Boolean> curPropValueList = new Dictionary<string, Boolean>();
				String curPropValue = getPropertyByName( "RunningOrderColumnFilterJump" );
				if ( curPropValue.Length == 0 ) {
					setRunningOrderColumnFilterJump( curPropValueList );
				} else {
					String[] curEntryValues = curPropValue.Split( ',' );
					foreach ( String curEntryValue in curEntryValues ) {
						String[] curEntryAttrs = curEntryValue.Split( ' ' );
						Boolean curAttrVisibility = false;
						if ( curEntryAttrs[1].Equals( "True" ) ) curAttrVisibility = true;
						curPropValueList.Add( curEntryAttrs[0], curAttrVisibility );
					}
				}
				return curPropValueList;
			}
			set { setRunningOrderColumnFilterJump( value ); }
		}
		public void setRunningOrderColumnFilterJump( Dictionary<string, Boolean> propList ) {
			StringBuilder saveValues = new StringBuilder( "" );
			foreach ( KeyValuePair<string, bool> curEntry in propList ) {
				if ( saveValues.Length > 1 ) saveValues.Append( "," );
				saveValues.Append( curEntry.Key + " " + curEntry.Value );
			}
			updateProperty( "RunningOrderColumnFilterJump", saveValues.ToString(), 311 );
		}
		public void setRunningOrderColumnFilterJump( String propValues ) {
			Dictionary<string, Boolean> curPropValueList = new Dictionary<string, Boolean>();
			String[] curEntryValues = propValues.Split( ',' );
			foreach ( String curEntryValue in curEntryValues ) {
				String[] curEntryAttrs = curEntryValue.Split( ' ' );
				Boolean curAttrVisibility = false;
				if ( curEntryAttrs[1].Equals( "True" ) ) curAttrVisibility = true;
				curPropValueList.Add( curEntryAttrs[0], curAttrVisibility );
			}
			setRunningOrderColumnFilterJump( curPropValueList );
		}

		#endregion

		#region Jump Scorebook methods
		public String JumpScorebookDataType {
            get {
                String curReturnValue = getPropertyByName("JumpScorebookDataType");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "score";
                    }
                    setJumpScorebookDataType(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpScorebookDataType( value ); }
        }
        public void setJumpScorebookDataType(String value) {
            updateProperty( "JumpScorebookDataType", value, 520 );
        }

        public String JumpScorebookPointsMethod {
            get {
                String curReturnValue = getPropertyByName("JumpScorebookPointsMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "plcmt";
                    } else {
                        curReturnValue = "nops";
                    }
                    setJumpScorebookPointsMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpScorebookPointsMethod( value ); }
        }
        public void setJumpScorebookPointsMethod(String value) {
            updateProperty( "JumpScorebookPointsMethod", value, 520 );
        }

        public String JumpScorebookPlcmtMethod {
            get {
                String curReturnValue = getPropertyByName("JumpScorebookPlcmtMethod");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "score";
                    } else {
                        curReturnValue = "score";
                    }
                    setJumpScorebookPlcmtMethod(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpScorebookPlcmtMethod( value ); }
        }
        public void setJumpScorebookPlcmtMethod(String value) {
            updateProperty( "JumpScorebookPlcmtMethod", value, 520 );
        }

        public String JumpScorebookPlcmtOrg {
            get {
                String curReturnValue = getPropertyByName("JumpScorebookPlcmtOrg");
                if ( curReturnValue.Length == 0 ) {
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curReturnValue = "div";
                    } else {
                        curReturnValue = "div";
                    }
                    setJumpScorebookPlcmtOrg(curReturnValue);
                }
                return curReturnValue;
            }
            set { setJumpScorebookPlcmtOrg( value ); }
        }
        public void setJumpScorebookPlcmtOrg(String value) {
            updateProperty( "JumpScorebookPlcmtOrg", value, 520 );
        }
        #endregion

        #region OfficialWorkRecord methods
        public String OfficialWorkRecordSort {
            get {
                String curReturnValue = getPropertyByName("OfficialWorkRecordSort");
                if ( curReturnValue.Length == 0 ) {
                    curReturnValue = "";
                    setOfficialWorkRecordSort(curReturnValue);
                }
                return curReturnValue;
            }
            set { setOfficialWorkRecordSort( value ); }
        }
        public void setOfficialWorkRecordSort(String value) {
            updateProperty( "OfficialWorkRecordSort", value, 610 );
        }
        #endregion

        private String getPropertyByName(String inKey) {
            String curReturnValue = "";
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT PropKey, PropValue ");
            curSqlStmt.Append("FROM TourProperties ");
            curSqlStmt.Append("WHERE SanctionId = '" + Properties.Settings.Default.AppSanctionNum + "' ");
            curSqlStmt.Append("  AND PropKey = '" + inKey + "' ");
            curSqlStmt.Append("Order by PropOrder, PropKey, PropValue ");
            DataTable curDataTable = getData(curSqlStmt.ToString());
            if ( curDataTable.Rows.Count > 0 ) {
                curReturnValue = (String)curDataTable.Rows[0]["PropValue"];
            } else {
                curReturnValue = "";
            }
            return curReturnValue;
        }

        private void updateProperty(String inKey, String inValue, Int16 inOrder) {
            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TourProperties set ");
                curSqlStmt.Append( "PropValue = '" + inValue + "' ");
                curSqlStmt.Append( ", PropOrder = " + inOrder + " " );
                curSqlStmt.Append( "Where Sanctionid = '" + Properties.Settings.Default.AppSanctionNum + "' " );
                curSqlStmt.Append( "AND PropKey = '" + inKey + "' ");
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                if (rowsProc == 0) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Insert  TourProperties (" );
                    curSqlStmt.Append( "Sanctionid, PropKey, PropValue, PropOrder" );
                    curSqlStmt.Append( ") Values ( " );
                    curSqlStmt.Append( "'" + Properties.Settings.Default.AppSanctionNum + "', '" + inKey + "', '" + inValue + "', " + inOrder.ToString() + " )");
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                }
            } catch (Exception excp) {
                MessageBox.Show( "Exception encountered updating property \n" + inKey + "=" + inValue + "\n" + excp.Message );
            }
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }


    }
}
