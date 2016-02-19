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
                        curClassType.InvokeMember( "set" + (String)curRow["PropKey"]
                            , BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod
                            , null
                            , curTourProperties
                            , new Object[] { (String)curRow["PropValue"] } );
                    }
                }
                return true;
            } else {
                return false;
            }
        }

        //-----------------------------------------------------
        #region Master Summary methods
        private String myMasterSummaryDataType = "best";
        public String MasterSummaryDataType {
            get { return myMasterSummaryDataType; }
            set { setMasterSummaryDataType( value ); }
        }
        public void setMasterSummaryDataType(String value) {
            myMasterSummaryDataType = value;
            updateProperty( "MasterSummaryDataType", myMasterSummaryDataType, 100 );
        }

        private String myMasterSummaryPointsMethod = "nops";
        public String MasterSummaryPointsMethod {
            get { return myMasterSummaryPointsMethod; }
            set {setMasterSummaryPointsMethod( value );}
        }
        public void setMasterSummaryPointsMethod(String value) {
            myMasterSummaryPointsMethod = value;
            updateProperty( "MasterSummaryPointsMethod", myMasterSummaryPointsMethod, 100 );
        }

        private String myMasterSummaryPlcmtMethod = "score";
        public String MasterSummaryPlcmtMethod {
            get { return myMasterSummaryPlcmtMethod; }
            set { setMasterSummaryPlcmtMethod( value ); }
        }
        public void setMasterSummaryPlcmtMethod(String value) {
            myMasterSummaryPlcmtMethod = value;
            updateProperty( "MasterSummaryPlcmtMethod", myMasterSummaryPlcmtMethod, 100 );
        }

        private String myMasterSummaryPlcmtOrg = "div";
        public String MasterSummaryPlcmtOrg {
            get { return myMasterSummaryPlcmtOrg; }
            set { setMasterSummaryPlcmtOrg( value ); }
        }
        public void setMasterSummaryPlcmtOrg(String value) {
            myMasterSummaryPlcmtOrg = value;
            updateProperty( "MasterSummaryPlcmtOrg", myMasterSummaryPlcmtOrg, 100 );
        }
        #endregion

        //-----------------------------------------------------
        #region Master Overall Summary methods
        private String myMasterSummaryOverallDataType = "round";
        public String MasterSummaryOverallDataType {
            get { return myMasterSummaryOverallDataType; }
            set { setMasterSummaryOverallDataType( value ); }
        }
        public void setMasterSummaryOverallDataType(String value) {
            myMasterSummaryOverallDataType = value;
            updateProperty( "MasterSummaryOverallDataType", myMasterSummaryOverallDataType, 110 );
        }

        private String myMasterSummaryOverallPointsMethod = "nops";
        public String MasterSummaryOverallPointsMethod {
            get { return myMasterSummaryOverallPointsMethod; }
            set { setMasterSummaryOverallPointsMethod( value ); }
        }
        public void setMasterSummaryOverallPointsMethod(String value) {
            myMasterSummaryOverallPointsMethod = value;
            updateProperty( "MasterSummaryOverallPointsMethod", myMasterSummaryOverallPointsMethod, 110 );
        }

        private String myMasterSummaryOverallPlcmtMethod = "points";
        public String MasterSummaryOverallPlcmtMethod {
            get { return myMasterSummaryOverallPlcmtMethod; }
            set { setMasterSummaryOverallPlcmtMethod( value ); }
        }
        public void setMasterSummaryOverallPlcmtMethod(String value) {
            myMasterSummaryOverallPlcmtMethod = value;
            updateProperty( "MasterSummaryOverallPlcmtMethod", myMasterSummaryOverallPlcmtMethod, 110 );
        }

        private String myMasterSummaryOverallPlcmtOrg = "div";
        public String MasterSummaryOverallPlcmtOrg {
            get { return myMasterSummaryOverallPlcmtOrg; }
            set { setMasterSummaryOverallPlcmtOrg( value ); }
        }
        public void setMasterSummaryOverallPlcmtOrg(String value) {
            myMasterSummaryOverallPlcmtOrg = value;
            updateProperty( "MasterSummaryOverallPlcmtOrg", myMasterSummaryOverallPlcmtOrg, 110 );
        }

        private String myMasterSummaryOverallFilter = "all";
        public String MasterSummaryOverallFilter {
            get { return myMasterSummaryOverallFilter; }
            set { setMasterSummaryOverallFilter( value ); }
        }
        public void setMasterSummaryOverallFilter(String value) {
            myMasterSummaryOverallFilter = value;
            updateProperty( "MasterSummaryOverallFilter", myMasterSummaryOverallFilter, 110 );
        }
        #endregion

        //-----------------------------------------------------
        #region Master Team Summary methods
        private String myTeamSummaryDataType = "best";
        public String TeamSummaryDataType {
            get { return myTeamSummaryDataType; }
            set { setTeamSummaryDataType( value ); }
        }
        public void setTeamSummaryDataType(String value) {
            myTeamSummaryDataType = value;
            updateProperty( "TeamSummaryDataType", myTeamSummaryDataType, 900 );
        }

        private String myTeamSummaryPointsMethod = "nops";
        public String TeamSummaryPointsMethod {
            get { return myTeamSummaryPointsMethod; }
            set { setTeamSummaryPointsMethod( value ); }
        }
        public void setTeamSummaryPointsMethod(String value) {
            myTeamSummaryPointsMethod = value;
            updateProperty( "TeamSummaryPointsMethod", myTeamSummaryPointsMethod, 900 );
        }

        private String myTeamSummaryPlcmtMethod = "points";
        public String TeamSummaryPlcmtMethod {
            get { return myTeamSummaryPlcmtMethod; }
            set { setTeamSummaryPlcmtMethod( value ); }
        }
        public void setTeamSummaryPlcmtMethod(String value) {
            myTeamSummaryPlcmtMethod = value;
            updateProperty( "TeamSummaryPlcmtMethod", myTeamSummaryPlcmtMethod, 900 );
        }

        private String myTeamSummaryPlcmtOrg = "div";
        public String TeamSummaryPlcmtOrg {
            get { return myTeamSummaryPlcmtOrg; }
            set { setTeamSummaryPlcmtOrg( value ); }
        }
        public void setTeamSummaryPlcmtOrg(String value) {
            myTeamSummaryPlcmtOrg = value;
            updateProperty( "TeamSummaryPlcmtOrg", myTeamSummaryPlcmtOrg, 900 );
        }

        private String myTeamSummary_NumPerTeam = "0";
        public String TeamSummary_NumPerTeam {
            get {
                if (myTeamSummary_NumPerTeam == null) myTeamSummary_NumPerTeam = "";
                if (myTeamSummary_NumPerTeam.Length == 0) {
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    setTeamSummary_NumPerTeam( "4" );
                } else {
                    setTeamSummary_NumPerTeam( "0" );
                }
            }
            return myTeamSummary_NumPerTeam;
            }
            set { setTeamSummary_NumPerTeam( value ); }
        }
        public void setTeamSummary_NumPerTeam(String value) {
            myTeamSummary_NumPerTeam = value;
            updateProperty( "TeamSummary_NumPerTeam", myTeamSummary_NumPerTeam, 900 );
        }
        #endregion

        #region Slalom Team Summary methods
        private String mySlalomTeamSummaryDataType = ""; //myInstance.TeamSummaryDataType;
        public String SlalomTeamSummaryDataType {
            get { return mySlalomTeamSummaryDataType; }
            set { setSlalomTeamSummaryDataType( value ); }
        }
        public void setSlalomTeamSummaryDataType(String value) {
            mySlalomTeamSummaryDataType = value;
            updateProperty( "SlalomTeamSummaryDataType", mySlalomTeamSummaryDataType, 910 );
        }

        private String mySlalomTeamSummaryPointsMethod = ""; //myInstance.TeamSummaryPointsMethod;
        public String SlalomTeamSummaryPointsMethod {
            get { return mySlalomTeamSummaryPointsMethod; }
            set { setSlalomTeamSummaryPointsMethod( value ); }
        }
        public void setSlalomTeamSummaryPointsMethod(String value) {
            mySlalomTeamSummaryPointsMethod = value;
            updateProperty( "SlalomTeamSummaryPointsMethod", mySlalomTeamSummaryPointsMethod, 910 );
        }

        private String mySlalomTeamSummaryPlcmtMethod = ""; //myInstance.TeamSummaryPlcmtMethod;
        public String SlalomTeamSummaryPlcmtMethod {
            get { return mySlalomTeamSummaryPlcmtMethod; }
            set { setSlalomTeamSummaryPlcmtMethod( value ); }
        }
        public void setSlalomTeamSummaryPlcmtMethod(String value) {
            mySlalomTeamSummaryPlcmtMethod = value;
            updateProperty( "SlalomTeamSummaryPlcmtMethod", mySlalomTeamSummaryPlcmtMethod, 910 );
        }

        private String mySlalomTeamSummaryPlcmtOrg = ""; //myInstance.TeamSummaryPlcmtOrg;
        public String SlalomTeamSummaryPlcmtOrg {
            get { return mySlalomTeamSummaryPlcmtOrg; }
            set { setSlalomTeamSummaryPlcmtOrg( value ); }
        }
        public void setSlalomTeamSummaryPlcmtOrg(String value) {
            mySlalomTeamSummaryPlcmtOrg = value;
            updateProperty( "SlalomTeamSummaryPlcmtOrg", mySlalomTeamSummaryPlcmtOrg, 910 );
        }

        private String mySlalomTeamSummary_NumPerTeam = ""; //myInstance.TeamSummary_NumPerTeam;
        public String SlalomTeamSummary_NumPerTeam {
            get {
                if (mySlalomTeamSummary_NumPerTeam == null) setSlalomTeamSummary_NumPerTeam( myTeamSummary_NumPerTeam);
                if (mySlalomTeamSummary_NumPerTeam.Length == 0) setSlalomTeamSummary_NumPerTeam( myTeamSummary_NumPerTeam);
                return mySlalomTeamSummary_NumPerTeam;
            }
            set { setSlalomTeamSummary_NumPerTeam( value ); }
        }
        public void setSlalomTeamSummary_NumPerTeam(String value) {
            mySlalomTeamSummary_NumPerTeam = value;
            updateProperty( "SlalomTeamSummary_NumPerTeam", mySlalomTeamSummary_NumPerTeam, 910 );
        }
        #endregion

        #region Trick Team Summary methods
        private String myTrickTeamSummaryDataType = ""; //myInstance.TeamSummaryDataType;
        public String TrickTeamSummaryDataType {
            get { return myTrickTeamSummaryDataType; }
            set { setTrickTeamSummaryDataType( value ); }
        }
        public void setTrickTeamSummaryDataType(String value) {
            myTrickTeamSummaryDataType = value;
            updateProperty( "TrickTeamSummaryDataType", myTrickTeamSummaryDataType, 920 );
        }

        private String myTrickTeamSummaryPointsMethod = ""; //myInstance.TeamSummaryPointsMethod;
        public String TrickTeamSummaryPointsMethod {
            get { return myTrickTeamSummaryPointsMethod; }
            set { setTrickTeamSummaryPointsMethod( value ); }
        }
        public void setTrickTeamSummaryPointsMethod(String value) {
            myTrickTeamSummaryPointsMethod = value;
            updateProperty( "TrickTeamSummaryPointsMethod", myTrickTeamSummaryPointsMethod, 920 );
        }

        private String myTrickTeamSummaryPlcmtMethod = ""; //myInstance.TeamSummaryPlcmtMethod;
        public String TrickTeamSummaryPlcmtMethod {
            get { return myTrickTeamSummaryPlcmtMethod; }
            set { setTrickTeamSummaryPlcmtMethod( value ); }
        }
        public void setTrickTeamSummaryPlcmtMethod(String value) {
            myTrickTeamSummaryPlcmtMethod = value;
            updateProperty( "TrickTeamSummaryPlcmtMethod", myTrickTeamSummaryPlcmtMethod, 920 );
        }

        private String myTrickTeamSummaryPlcmtOrg = ""; //myInstance.TeamSummaryPlcmtOrg;
        public String TrickTeamSummaryPlcmtOrg {
            get { return myTrickTeamSummaryPlcmtOrg; }
            set { setTrickTeamSummaryPlcmtOrg( value ); }
        }
        public void setTrickTeamSummaryPlcmtOrg(String value) {
            myTrickTeamSummaryPlcmtOrg = value;
            updateProperty( "TrickTeamSummaryPlcmtOrg", myTrickTeamSummaryPlcmtOrg, 920 );
        }

        private String myTrickTeamSummary_NumPerTeam = ""; //myInstance.TeamSummary_NumPerTeam;
        public String TrickTeamSummary_NumPerTeam {
            get {
                if (myTrickTeamSummary_NumPerTeam == null) setTrickTeamSummary_NumPerTeam( myTeamSummary_NumPerTeam );
                if (myTrickTeamSummary_NumPerTeam.Length == 0) setTrickTeamSummary_NumPerTeam( myTeamSummary_NumPerTeam );
                return myTrickTeamSummary_NumPerTeam;
            }
            set { setTrickTeamSummary_NumPerTeam( value ); }
        }
        public void setTrickTeamSummary_NumPerTeam(String value) {
            myTrickTeamSummary_NumPerTeam = value;
            updateProperty( "TrickTeamSummary_NumPerTeam", myTrickTeamSummary_NumPerTeam, 920 );
        }
        #endregion

        #region Jump Team Summary methods
        private String myJumpTeamSummaryDataType = ""; //myInstance.TeamSummaryDataType;
        public String JumpTeamSummaryDataType {
            get { return myJumpTeamSummaryDataType; }
            set { setJumpTeamSummaryDataType( value ); }
        }
        public void setJumpTeamSummaryDataType(String value) {
            myJumpTeamSummaryDataType = value;
            updateProperty( "JumpTeamSummaryDataType", myJumpTeamSummaryDataType, 930 );
        }

        private String myJumpTeamSummaryPointsMethod = ""; //myInstance.TeamSummaryPointsMethod;
        public String JumpTeamSummaryPointsMethod {
            get { return myJumpTeamSummaryPointsMethod; }
            set { setJumpTeamSummaryPointsMethod( value ); }
        }
        public void setJumpTeamSummaryPointsMethod(String value) {
            myJumpTeamSummaryPointsMethod = value;
            updateProperty( "JumpTeamSummaryPointsMethod", myJumpTeamSummaryPointsMethod, 930 );
        }

        private String myJumpTeamSummaryPlcmtMethod = ""; //myInstance.TeamSummaryPlcmtMethod;
        public String JumpTeamSummaryPlcmtMethod {
            get { return myJumpTeamSummaryPlcmtMethod; }
            set { setJumpTeamSummaryPlcmtMethod( value ); }
        }
        public void setJumpTeamSummaryPlcmtMethod(String value) {
            myJumpTeamSummaryPlcmtMethod = value;
            updateProperty( "JumpTeamSummaryPlcmtMethod", myJumpTeamSummaryPlcmtMethod, 930 );
        }

        private String myJumpTeamSummaryPlcmtOrg = ""; //myInstance.TeamSummaryPlcmtOrg;
        public String JumpTeamSummaryPlcmtOrg {
            get { return myJumpTeamSummaryPlcmtOrg; }
            set { setJumpTeamSummaryPlcmtOrg( value ); }
        }
        public void setJumpTeamSummaryPlcmtOrg(String value) {
            myJumpTeamSummaryPlcmtOrg = value;
            updateProperty( "JumpTeamSummaryPlcmtOrg", myJumpTeamSummaryPlcmtOrg, 930 );
        }

        private String myJumpTeamSummary_NumPerTeam = ""; //myInstance.TeamSummary_NumPerTeam;
        public String JumpTeamSummary_NumPerTeam {
            get {
                if (myJumpTeamSummary_NumPerTeam == null) setJumpTeamSummary_NumPerTeam( myTeamSummary_NumPerTeam );
                if (myJumpTeamSummary_NumPerTeam.Length == 0) setJumpTeamSummary_NumPerTeam( myTeamSummary_NumPerTeam );
                return myJumpTeamSummary_NumPerTeam;
            }
            set { setJumpTeamSummary_NumPerTeam( value ); }
        }
        public void setJumpTeamSummary_NumPerTeam(String value) {
            myJumpTeamSummary_NumPerTeam = value;
            updateProperty( "JumpTeamSummary_NumPerTeam", myJumpTeamSummary_NumPerTeam, 930 );
        }
        #endregion

        //-----------------------------------------------------
        #region Slalom Summary methods
        private String mySlalomSummaryDataType = ""; 
        public String SlalomSummaryDataType {
            get {
                if (mySlalomSummaryDataType == null) setSlalomSummaryDataType( myMasterSummaryDataType);
                if (mySlalomSummaryDataType.Length == 0) setSlalomSummaryDataType( myMasterSummaryDataType);
                return mySlalomSummaryDataType; }
            set { setSlalomSummaryDataType( value ); }
        }
        public void setSlalomSummaryDataType(String value) {
            mySlalomSummaryDataType = value;
            updateProperty( "SlalomSummaryDataType", mySlalomSummaryDataType, 300 );
        }

        private String mySlalomSummaryPointsMethod = "";
        public String SlalomSummaryPointsMethod {
            get {
                if (mySlalomSummaryPointsMethod == null) setSlalomSummaryPointsMethod( myMasterSummaryPointsMethod );
                if (mySlalomSummaryPointsMethod.Length == 0) setSlalomSummaryPointsMethod( myMasterSummaryPointsMethod );
                return mySlalomSummaryPointsMethod;
            }
            set { setSlalomSummaryPointsMethod( value ); }
        }
        public void setSlalomSummaryPointsMethod(String value) {
            mySlalomSummaryPointsMethod = value;
            updateProperty( "SlalomSummaryPointsMethod", mySlalomSummaryPointsMethod, 300 );
        }

        private String mySlalomSummaryPlcmtMethod = ""; 
        public String SlalomSummaryPlcmtMethod {
            get {
                if (mySlalomSummaryPlcmtMethod == null) setSlalomSummaryPlcmtMethod( myMasterSummaryPlcmtMethod );
                if (mySlalomSummaryPlcmtMethod.Length == 0) setSlalomSummaryPlcmtMethod( myMasterSummaryPlcmtMethod );
                return mySlalomSummaryPlcmtMethod;
            }
            set { setSlalomSummaryPlcmtMethod( value ); }
        }
        public void setSlalomSummaryPlcmtMethod(String value) {
            mySlalomSummaryPlcmtMethod = value;
            updateProperty( "SlalomSummaryPlcmtMethod", mySlalomSummaryPlcmtMethod, 300 );
        }

        private String mySlalomSummaryPlcmtOrg = ""; 
        public String SlalomSummaryPlcmtOrg {
            get {
                if (mySlalomSummaryPlcmtOrg == null) setSlalomSummaryPlcmtOrg( myMasterSummaryPlcmtOrg );
                if (mySlalomSummaryPlcmtOrg.Length == 0) setSlalomSummaryPlcmtOrg( myMasterSummaryPlcmtOrg );
                return mySlalomSummaryPlcmtOrg;
            }
            set { setSlalomSummaryPlcmtOrg( value ); }
        }
        public void setSlalomSummaryPlcmtOrg(String value) {
            mySlalomSummaryPlcmtOrg = value;
            updateProperty( "SlalomSummaryPlcmtOrg", mySlalomSummaryPlcmtOrg, 300 );
        }
        
        private String mySlalomSummaryNumPrelim = "";
        public String SlalomSummaryNumPrelim {
            get { return mySlalomSummaryNumPrelim; }
            set { setSlalomSummaryNumPrelim( value ); }
        }
        public void setSlalomSummaryNumPrelim(String value) {
            mySlalomSummaryNumPrelim = value;
            updateProperty( "SlalomSummaryNumPrelim", mySlalomSummaryNumPrelim, 300 );
        }

        private String mySlalomSummaryAwardsNum = "5";
        public String SlalomSummaryAwardsNum {
            get { return mySlalomSummaryAwardsNum; }
            set { setSlalomSummaryAwardsNum( value ); }
        }
        public void setSlalomSummaryAwardsNum(String value) {
            mySlalomSummaryAwardsNum = value;
            updateProperty( "SlalomSummaryAwardsNum", mySlalomSummaryAwardsNum, 300 );
        }

        private String mySlalomScoreSummary_Sort = "";
        public String SlalomScoreSummary_Sort {
            get { return mySlalomScoreSummary_Sort; }
            set { setSlalomScoreSummary_Sort( value ); }
        }
        public void setSlalomScoreSummary_Sort(String value) {
            mySlalomScoreSummary_Sort = value;
            updateProperty( "SlalomScoreSummary_Sort", mySlalomScoreSummary_Sort, 300 );
        }
        #endregion

        //-----------------------------------------------------
        #region Slalom methods
        private String myRunningOrderSortSlalom = "";
        public String RunningOrderSortSlalom {
            get {
                if (myRunningOrderSortSlalom == null) myRunningOrderSortSlalom = "";
                if (myRunningOrderSortSlalom.Length == 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        setRunningOrderSortSlalom("DivOrder ASC, Div ASC, RunOrder ASC, RankingScore ASC, SkierName ASC");
                    } else {
                        setRunningOrderSortSlalom( "EventGroup ASC, RunOrder ASC, RankingScore ASC, SkierName ASC" );
                    }
                }
                return myRunningOrderSortSlalom;
            }
            set { setRunningOrderSortSlalom( value ); }
        }
        public void setRunningOrderSortSlalom(String value) {
            myRunningOrderSortSlalom = value;
            updateProperty( "RunningOrderSortSlalom", myRunningOrderSortSlalom, 310 );
        }
        #endregion

        //-----------------------------------------------------
        #region Slalom Scorebook methods
        private String mySlalomScorebookDataType = "";
        public String SlalomScorebookDataType {
            get {
                if (mySlalomScorebookDataType == null) setSlalomScorebookDataType( myMasterSummaryDataType );
                if (mySlalomScorebookDataType.Length == 0) setSlalomScorebookDataType( myMasterSummaryDataType );
                return mySlalomScorebookDataType;
            }
            set { setSlalomScorebookDataType( value ); }
        }
        public void setSlalomScorebookDataType(String value) {
            mySlalomScorebookDataType = value;
            updateProperty( "SlalomScorebookDataType", mySlalomScorebookDataType, 320 );
        }

        private String mySlalomScorebookPointsMethod = "";
        public String SlalomScorebookPointsMethod {
            get {
                if (mySlalomScorebookPointsMethod == null) setSlalomScorebookPointsMethod( myMasterSummaryPointsMethod );
                if (mySlalomScorebookPointsMethod.Length == 0) setSlalomScorebookPointsMethod( myMasterSummaryPointsMethod );
                return mySlalomScorebookPointsMethod;
            }
            set { setSlalomScorebookPointsMethod( value ); }
        }
        public void setSlalomScorebookPointsMethod(String value) {
            mySlalomScorebookPointsMethod = value;
            updateProperty( "SlalomScorebookPointsMethod", mySlalomScorebookPointsMethod, 320 );
        }

        private String mySlalomScorebookPlcmtMethod = "";
        public String SlalomScorebookPlcmtMethod {
            get {
                if (mySlalomScorebookPlcmtMethod == null) setSlalomScorebookPlcmtMethod( myMasterSummaryPlcmtMethod );
                if (mySlalomScorebookPlcmtMethod.Length == 0) setSlalomScorebookPlcmtMethod( myMasterSummaryPlcmtMethod );
                return mySlalomScorebookPlcmtMethod;
            }
            set { setSlalomScorebookPlcmtMethod( value ); }
        }
        public void setSlalomScorebookPlcmtMethod(String value) {
            mySlalomScorebookPlcmtMethod = value;
            updateProperty( "SlalomScorebookPlcmtMethod", mySlalomScorebookPlcmtMethod, 320 );
        }

        private String mySlalomScorebookPlcmtOrg = "";
        public String SlalomScorebookPlcmtOrg {
            get {
                if (mySlalomScorebookPlcmtOrg == null) setSlalomScorebookPlcmtOrg( myMasterSummaryPlcmtOrg );
                if (mySlalomScorebookPlcmtOrg.Length == 0) setSlalomScorebookPlcmtOrg( myMasterSummaryPlcmtOrg );
                return mySlalomScorebookPlcmtOrg;
            }
            set { setSlalomScorebookPlcmtOrg( value ); }
        }
        public void setSlalomScorebookPlcmtOrg(String value) {
            mySlalomScorebookPlcmtOrg = value;
            updateProperty( "SlalomScorebookPlcmtOrg", mySlalomScorebookPlcmtOrg, 320 );
        }
        #endregion  

        //-----------------------------------------------------
        #region Trick Summary methods
        private String myTrickSummaryDataType = ""; //myInstance.MasterSummaryDataType;
        public String TrickSummaryDataType {
            get {
                if (myTrickSummaryDataType == null) setTrickSummaryDataType( myMasterSummaryDataType );
                if (myTrickSummaryDataType.Length == 0) setTrickSummaryDataType( myMasterSummaryDataType );
                return myTrickSummaryDataType;
            }
            set { setTrickSummaryDataType( value ); }
        }
        public void setTrickSummaryDataType(String value) {
            myTrickSummaryDataType = value;
            updateProperty( "TrickSummaryDataType", myTrickSummaryDataType, 400 );
        }

        private String myTrickSummaryPointsMethod = ""; //myInstance.MasterSummaryPointsMethod;
        public String TrickSummaryPointsMethod {
            get {
                if (myTrickSummaryPointsMethod == null) setTrickSummaryPointsMethod( myMasterSummaryPointsMethod );
                if (myTrickSummaryPointsMethod.Length == 0) setTrickSummaryPointsMethod( myMasterSummaryPointsMethod );
                return myTrickSummaryPointsMethod;
            }
            set { setTrickSummaryPointsMethod( value ); }
        }
        public void setTrickSummaryPointsMethod(String value) {
            myTrickSummaryPointsMethod = value;
            updateProperty( "TrickSummaryPointsMethod", myTrickSummaryPointsMethod, 400 );
        }

        private String myTrickSummaryPlcmtMethod = ""; //myInstance.MasterSummaryPlcmtMethod;
        public String TrickSummaryPlcmtMethod {
            get {
                if (myTrickSummaryPlcmtMethod == null) setTrickSummaryPlcmtMethod( myMasterSummaryPlcmtMethod );
                if (myTrickSummaryPlcmtMethod.Length == 0) setTrickSummaryPlcmtMethod( myMasterSummaryPlcmtMethod );
                return myTrickSummaryPlcmtMethod;
            }
            set { setTrickSummaryPlcmtMethod( value ); }
        }
        public void setTrickSummaryPlcmtMethod(String value) {
            myTrickSummaryPlcmtMethod = value;
            updateProperty( "TrickSummaryPlcmtMethod", myTrickSummaryPlcmtMethod, 400 );
        }

        private String myTrickSummaryPlcmtOrg = ""; //myInstance.MasterSummaryPlcmtOrg;
        public String TrickSummaryPlcmtOrg {
            get {
                if (myTrickSummaryPlcmtOrg == null) setTrickSummaryPlcmtOrg( myMasterSummaryPlcmtOrg );
                if (myTrickSummaryPlcmtOrg.Length == 0) setTrickSummaryPlcmtOrg( myMasterSummaryPlcmtOrg );
                return myTrickSummaryPlcmtOrg;
            }
            set { setTrickSummaryPlcmtOrg( value ); }
        }
        public void setTrickSummaryPlcmtOrg(String value) {
            myTrickSummaryPlcmtOrg = value;
            updateProperty( "TrickSummaryPlcmtOrg", myTrickSummaryPlcmtOrg, 400 );
        }

        private String myTrickSummaryNumPrelim = "";
        public String TrickSummaryNumPrelim {
            get { return myTrickSummaryNumPrelim; }
            set { setTrickSummaryNumPrelim( value ); }
        }
        public void setTrickSummaryNumPrelim(String value) {
            myTrickSummaryNumPrelim = value;
            updateProperty( "TrickSummaryNumPrelim", myTrickSummaryNumPrelim, 400 );
        }

        private String myTrickSummaryAwardsNum = "5";
        public String TrickSummaryAwardsNum {
            get { return myTrickSummaryAwardsNum; }
            set { setTrickSummaryAwardsNum( value ); }
        }
        public void setTrickSummaryAwardsNum(String value) {
            myTrickSummaryAwardsNum = value;
            updateProperty( "TrickSummaryAwardsNum", myTrickSummaryAwardsNum, 400 );
        }

        private String myTrickScoreSummary_Sort = "";
        public String TrickScoreSummary_Sort {
            get { return myTrickScoreSummary_Sort; }
            set { setTrickScoreSummary_Sort( value ); }
        }
        public void setTrickScoreSummary_Sort(String value) {
            myTrickScoreSummary_Sort = value;
            updateProperty( "TrickScoreSummary_Sort", myTrickScoreSummary_Sort, 400 );
        }

        #endregion

        //-----------------------------------------------------
        #region Trick methods
        private String myRunningOrderSortTrick = "";
        public String RunningOrderSortTrick {
            get {
                if (myRunningOrderSortTrick == null) myRunningOrderSortTrick = "";
                if (myRunningOrderSortTrick.Length == 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        setRunningOrderSortTrick( "DivOrder ASC, Div ASC, RunOrder ASC, RankingScore ASC, SkierName ASC" );
                    } else {
                        setRunningOrderSortTrick( "EventGroup ASC, RunOrder ASC, RankingScore ASC, SkierName ASC" );
                    }
                }
                return myRunningOrderSortTrick;
            }
            set { setRunningOrderSortTrick( value ); }
        }
        public void setRunningOrderSortTrick(String value) {
            myRunningOrderSortTrick = value;
            updateProperty( "RunningOrderSortTrick", myRunningOrderSortTrick, 410 );
        }
        #endregion

        #region Trick Scorebook methods
        private String myTrickScorebookDataType = "";
        public String TrickScorebookDataType {
            get {
                if (myTrickScorebookDataType == null) setTrickScorebookDataType( myMasterSummaryDataType );
                if (myTrickScorebookDataType.Length == 0) setTrickScorebookDataType( myMasterSummaryDataType );
                return myTrickScorebookDataType;
            }
            set { setTrickScorebookDataType( value ); }
        }
        public void setTrickScorebookDataType(String value) {
            myTrickScorebookDataType = value;
            updateProperty( "TrickScorebookDataType", myTrickScorebookDataType, 420 );
        }

        private String myTrickScorebookPointsMethod = "";
        public String TrickScorebookPointsMethod {
            get {
                if (myTrickScorebookPointsMethod == null) setTrickScorebookPointsMethod( myMasterSummaryPointsMethod );
                if (myTrickScorebookPointsMethod.Length == 0) setTrickScorebookPointsMethod( myMasterSummaryPointsMethod );
                return myTrickScorebookPointsMethod;
            }
            set { setTrickScorebookPointsMethod( value ); }
        }
        public void setTrickScorebookPointsMethod(String value) {
            myTrickScorebookPointsMethod = value;
            updateProperty( "TrickScorebookPointsMethod", myTrickScorebookPointsMethod, 420 );
        }

        private String myTrickScorebookPlcmtMethod = "";
        public String TrickScorebookPlcmtMethod {
            get {
                if (myTrickScorebookPlcmtMethod == null) setTrickScorebookPlcmtMethod( myMasterSummaryPlcmtMethod );
                if (myTrickScorebookPlcmtMethod.Length == 0) setTrickScorebookPlcmtMethod( myMasterSummaryPlcmtMethod );
                return myTrickScorebookPlcmtMethod;
            }
            set { setTrickScorebookPlcmtMethod( value ); }
        }
        public void setTrickScorebookPlcmtMethod(String value) {
            myTrickScorebookPlcmtMethod = value;
            updateProperty( "TrickScorebookPlcmtMethod", myTrickScorebookPlcmtMethod, 420 );
        }

        private String myTrickScorebookPlcmtOrg = "";
        public String TrickScorebookPlcmtOrg {
            get {
                if (myTrickScorebookPlcmtOrg == null) setTrickScorebookPlcmtOrg( myMasterSummaryPlcmtOrg );
                if (myTrickScorebookPlcmtOrg.Length == 0) setTrickScorebookPlcmtOrg( myMasterSummaryPlcmtOrg );
                return myTrickScorebookPlcmtOrg;
            }
            set { setTrickScorebookPlcmtOrg( value ); }
        }
        public void setTrickScorebookPlcmtOrg(String value) {
            myTrickScorebookPlcmtOrg = value;
            updateProperty( "TrickScorebookPlcmtOrg", myTrickScorebookPlcmtOrg, 420 );
        }
#endregion

        //-----------------------------------------------------
        #region Jump Summary methods
        private String myJumpSummaryDataType = ""; //myInstance.MasterSummaryDataType;
        public String JumpSummaryDataType {
            get {
                if (myJumpSummaryDataType == null) setJumpSummaryDataType( myMasterSummaryDataType );
                if (myJumpSummaryDataType.Length == 0) setJumpSummaryDataType( myMasterSummaryDataType );
                return myJumpSummaryDataType;
            }
            set { setJumpSummaryDataType( value ); }
        }
        public void setJumpSummaryDataType(String value) {
            myJumpSummaryDataType = value;
            updateProperty( "JumpSummaryDataType", myJumpSummaryDataType, 500 );
        }

        private String myJumpSummaryPointsMethod = ""; //myInstance.MasterSummaryPointsMethod;
        public String JumpSummaryPointsMethod {
            get {
                if (myJumpSummaryPointsMethod == null) setJumpSummaryPointsMethod( myMasterSummaryPointsMethod );
                if (myJumpSummaryPointsMethod.Length == 0) setJumpSummaryPointsMethod( myMasterSummaryPointsMethod );
                return myJumpSummaryPointsMethod;
            }
            set { setJumpSummaryPointsMethod( value ); }
        }
        public void setJumpSummaryPointsMethod(String value) {
            myJumpSummaryPointsMethod = value;
            updateProperty( "JumpSummaryPointsMethod", myJumpSummaryPointsMethod, 500 );
        }

        private String myJumpSummaryPlcmtMethod = ""; //myInstance.MasterSummaryPlcmtMethod;
        public String JumpSummaryPlcmtMethod {
            get {
                if (myJumpSummaryPlcmtMethod == null) setJumpSummaryPlcmtMethod( myMasterSummaryPlcmtMethod );
                if (myJumpSummaryPlcmtMethod.Length == 0) setJumpSummaryPlcmtMethod( myMasterSummaryPlcmtMethod );
                return myJumpSummaryPlcmtMethod;
            }
            set { setJumpSummaryPlcmtMethod( value ); }
        }
        public void setJumpSummaryPlcmtMethod(String value) {
            myJumpSummaryPlcmtMethod = value;
            updateProperty( "JumpSummaryPlcmtMethod", myJumpSummaryPlcmtMethod, 500 );
        }

        private String myJumpSummaryPlcmtOrg = ""; //myInstance.MasterSummaryOverallPlcmtOrg;
        public String JumpSummaryPlcmtOrg {
            get {
                if (myJumpSummaryPlcmtOrg == null) setJumpSummaryPlcmtOrg( myMasterSummaryPlcmtOrg );
                if (myJumpSummaryPlcmtOrg.Length == 0) setJumpSummaryPlcmtOrg( myMasterSummaryPlcmtOrg );
                return myJumpSummaryPlcmtOrg;
            }
            set { setJumpSummaryPlcmtOrg( value ); }
        }
        public void setJumpSummaryPlcmtOrg(String value) {
            myJumpSummaryPlcmtOrg = value;
            updateProperty( "JumpSummaryPlcmtOrg", myJumpSummaryPlcmtOrg, 500 );
        }

        private String myJumpSummaryNumPrelim = "";
        public String JumpSummaryNumPrelim {
            get { return myJumpSummaryNumPrelim; }
            set { setJumpSummaryNumPrelim( value ); }
        }
        public void setJumpSummaryNumPrelim(String value) {
            myJumpSummaryNumPrelim = value;
            updateProperty( "JumpSummaryNumPrelim", myJumpSummaryNumPrelim, 500 );
        }

        private String myJumpSummaryAwardsNum = "5";
        public String JumpSummaryAwardsNum {
            get { return myJumpSummaryAwardsNum; }
            set { setJumpSummaryAwardsNum( value ); }
        }
        public void setJumpSummaryAwardsNum(String value) {
            myJumpSummaryAwardsNum = value;
            updateProperty( "JumpSummaryAwardsNum", myJumpSummaryAwardsNum, 500 );
        }

        private String myJumpScoreSummary_Sort = "";
        public String JumpScoreSummary_Sort {
            get { return myJumpScoreSummary_Sort; }
            set { setJumpScoreSummary_Sort( value ); }
        }
        public void setJumpScoreSummary_Sort(String value) {
            myJumpScoreSummary_Sort = value;
            updateProperty( "JumpScoreSummary_Sort", myJumpScoreSummary_Sort, 500 );
        }

        #endregion

        //-----------------------------------------------------
        #region Jump methods
        private String myRunningOrderSortJump = "";
        public String RunningOrderSortJump {
            get {
                if (myRunningOrderSortJump == null) myRunningOrderSortJump = "";
                if (myRunningOrderSortJump.Length == 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        setRunningOrderSortJump( "DivOrder ASC, Div ASC, RunOrder ASC, RankingScore ASC, SkierName ASC" );
                    } else {
                        setRunningOrderSortJump( "EventGroup ASC, RunOrder ASC, RankingScore ASC, SkierName ASC" );
                    }
                }
                return myRunningOrderSortJump;
            }
            set { setRunningOrderSortJump( value ); }
        }
        public void setRunningOrderSortJump(String value) {
            myRunningOrderSortJump = value;
            updateProperty( "RunningOrderSortJump", myRunningOrderSortJump, 510 );
        }

        //JumpEntryNumJudges
        private String myJumpEntryNumJudges = "0";
        public String JumpEntryNumJudges {
            get {
                if (myJumpEntryNumJudges == null) myJumpEntryNumJudges = "0";
                return myJumpEntryNumJudges;
            }
            set { setJumpEntryNumJudges( value ); }
        }
        public void setJumpEntryNumJudges(String value) {
            myJumpEntryNumJudges = value;
            updateProperty( "JumpEntryNumJudges", myJumpEntryNumJudges, 510 );
        }
        #endregion

        #region Jump Scorebook methods
        private String myJumpScorebookDataType = "";
        public String JumpScorebookDataType {
            get {
                if (myJumpScorebookDataType == null) setJumpScorebookDataType( myMasterSummaryDataType );
                if (myJumpScorebookDataType.Length == 0) setJumpScorebookDataType( myMasterSummaryDataType );
                return myJumpScorebookDataType;
            }
            set { setJumpScorebookDataType( value ); }
        }
        public void setJumpScorebookDataType(String value) {
            myJumpScorebookDataType = value;
            updateProperty( "JumpScorebookDataType", myJumpScorebookDataType, 520 );
        }

        private String myJumpScorebookPointsMethod = "";
        public String JumpScorebookPointsMethod {
            get {
                if (myJumpScorebookPointsMethod == null) setJumpScorebookPointsMethod( myMasterSummaryPointsMethod );
                if (myJumpScorebookPointsMethod.Length == 0) setJumpScorebookPointsMethod( myMasterSummaryPointsMethod );
                return myJumpScorebookPointsMethod;
            }
            set { setJumpScorebookPointsMethod( value ); }
        }
        public void setJumpScorebookPointsMethod(String value) {
            myJumpScorebookPointsMethod = value;
            updateProperty( "JumpScorebookPointsMethod", myJumpScorebookPointsMethod, 520 );
        }

        private String myJumpScorebookPlcmtMethod = "";
        public String JumpScorebookPlcmtMethod {
            get {
                if (myJumpScorebookPlcmtMethod == null) setJumpScorebookPlcmtMethod( myMasterSummaryPlcmtMethod );
                if (myJumpScorebookPlcmtMethod.Length == 0) setJumpScorebookPlcmtMethod( myMasterSummaryPlcmtMethod );
                return myJumpScorebookPlcmtMethod;
            }
            set { setJumpScorebookPlcmtMethod( value ); }
        }
        public void setJumpScorebookPlcmtMethod(String value) {
            myJumpScorebookPlcmtMethod = value;
            updateProperty( "JumpScorebookPlcmtMethod", myJumpScorebookPlcmtMethod, 520 );
        }

        private String myJumpScorebookPlcmtOrg = "";
        public String JumpScorebookPlcmtOrg {
            get {
                if (myJumpScorebookPlcmtOrg == null) setJumpScorebookPlcmtOrg( myMasterSummaryPlcmtOrg );
                if (myJumpScorebookPlcmtOrg.Length == 0) setJumpScorebookPlcmtOrg( myMasterSummaryPlcmtOrg );
                return myJumpScorebookPlcmtOrg;
            }
            set { setJumpScorebookPlcmtOrg( value ); }
        }
        public void setJumpScorebookPlcmtOrg(String value) {
            myJumpScorebookPlcmtOrg = value;
            updateProperty( "JumpScorebookPlcmtOrg", myJumpScorebookPlcmtOrg, 520 );
        }
        #endregion

        #region OfficialWorkRecord methods
        private String myOfficialWorkRecordSort = "";
        public String OfficialWorkRecordSort {
            get {
                if (myOfficialWorkRecordSort == null) myOfficialWorkRecordSort = "";
                return myJumpSummaryDataType;
            }
            set { setOfficialWorkRecordSort( value ); }
        }
        public void setOfficialWorkRecordSort(String value) {
            myOfficialWorkRecordSort = value;
            updateProperty( "OfficialWorkRecordSort", myOfficialWorkRecordSort, 610 );
        }
        #endregion

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
