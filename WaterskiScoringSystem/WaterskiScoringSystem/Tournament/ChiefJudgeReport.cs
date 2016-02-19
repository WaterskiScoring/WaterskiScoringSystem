using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Deployment.Application;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class ChiefJudgeReport : Form {
        private String mySanctionNum = null;
        private bool myDataValid = false;
        private bool isTourClassELR = false;
        private Int16 mySlalomRounds;
        private Int16 myTrickRounds;
        private Int16 myJumpRounds;

        private String myTourClass = "";
        private DataRow myTourRow = null;
        private DataRow myJumpMeterSetupRow = null;
        private DataRow myClassRow;
        private DataRow myClassCRow;
        private DataRow myClassERow;
        private ListTourClass myTourClassList;
        
        public ChiefJudgeReport() {
            InitializeComponent();
        }

        private void ChiefJudgeReport_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.ChiefJudgeReport_Width > 0 ) {
                this.Width = Properties.Settings.Default.ChiefJudgeReport_Width;
            }
            if ( Properties.Settings.Default.ChiefJudgeReport_Height > 0 ) {
                this.Height = Properties.Settings.Default.ChiefJudgeReport_Height;
            }
            if ( Properties.Settings.Default.ChiefJudgeReport_Location.X > 0
                && Properties.Settings.Default.ChiefJudgeReport_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.ChiefJudgeReport_Location;
            }

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            myTourClassList = new ListTourClass();
            myTourClassList.ListTourClassLoad();

            RefreshButton_Click( null, null );

        }

        public void RefreshButton_Click( object sender, EventArgs e ) {
            // Retrieve data from database
            if ( mySanctionNum != null ) {
                Cursor.Current = Cursors.WaitCursor;
                sanctionIdTextBox.BeginInvoke( (MethodInvoker)delegate() {
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                } );
                myTourRow = getTourData();
                if (myTourRow != null) {
                    String[] curValueList;
                    myJumpMeterSetupRow = getJumpMeterSetup();
                        
                    myTourClass = ( (String)myTourRow["Class"] ).ToString().ToUpper().Trim();
                    myClassCRow = myTourClassList.TourClassDataTable.Select( "ListCode = 'C'" )[0];
                    myClassERow = myTourClassList.TourClassDataTable.Select( "ListCode = 'E'" )[0];
                    myClassRow = myTourClassList.TourClassDataTable.Select( "ListCode = '" + myTourClass + "'" )[0];
                    if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassCRow["ListCodeNum"]) {
                        isTourClassELR = true;
                    }

                    sanctionIdTextBox.Text =  (String)myTourRow["SanctionId"] + (String)myTourRow["Class"];
                    nameTextBox.Text = (String)myTourRow["Name"];
                    eventDatesTextBox.Text = (String)myTourRow["EventDates"];
                    eventLocationTextBox.Text = (String)myTourRow["EventLocation"];



                    if (myTourRow["slalomCourseSpecs"] == System.DBNull.Value) {
                    } else {
                        slalomCourseSpecsTextBox.Text = (String)myTourRow["SlalomCourseSpecs"];
                    }
                    if (myTourRow["JumpCourseSpecs"] == System.DBNull.Value) {
                    } else {
                        jumpCourseSpecsTextBox.Text = (String)myTourRow["JumpCourseSpecs"];
                    }
                    if (myTourRow["TrickCourseSpecs"] == System.DBNull.Value) {
                    } else {
                        trickCourseSpecsTextBox.Text = (String)myTourRow["TrickCourseSpecs"];
                    }
                    if (myTourRow["BuoySpecs"] == System.DBNull.Value) {
                    } else {
                        buoySpecsTextBox.Text = (String)myTourRow["BuoySpecs"];
                    }

                    if ( myTourRow["SlalomRopesSpecs"] == System.DBNull.Value ) {
                    } else {
                        curValueList = ( (String)myTourRow["SlalomRopesSpecs"] ).Split( ';' );

                        Rope1Line2300TextBox.Text = curValueList[1];
                        Rope1Line1825TextBox.Text = curValueList[2];
                        Rope1Line1600TextBox.Text = curValueList[3];
                        Rope1Line1425TextBox.Text = curValueList[4];
                        Rope1Line1300TextBox.Text = curValueList[5];
                        Rope1Line1200TextBox.Text = curValueList[6];
                        Rope1Line1125TextBox.Text = curValueList[7];
                        Rope1Line1075TextBox.Text = curValueList[8];
                        Rope1Line1025TextBox.Text = curValueList[9];
                        Rope2Line2300TextBox.Text = curValueList[11];
                        Rope2Line1825TextBox.Text = curValueList[12];
                        Rope2Line1600TextBox.Text = curValueList[13];
                        Rope2Line1425TextBox.Text = curValueList[14];
                        Rope2Line1300TextBox.Text = curValueList[15];
                        Rope2Line1200TextBox.Text = curValueList[16];
                        Rope2Line1125TextBox.Text = curValueList[17];
                        Rope2Line1075TextBox.Text = curValueList[18];
                        Rope2Line1025TextBox.Text = curValueList[19];
                        Rope3Line2300TextBox.Text = curValueList[21];
                        Rope3Line1825TextBox.Text = curValueList[22];
                        Rope3Line1600TextBox.Text = curValueList[23];
                        Rope3Line1425TextBox.Text = curValueList[24];
                        Rope3Line1300TextBox.Text = curValueList[25];
                        Rope3Line1200TextBox.Text = curValueList[26];
                        Rope3Line1125TextBox.Text = curValueList[27];
                        Rope3Line1075TextBox.Text = curValueList[28];
                        Rope3Line1025TextBox.Text = curValueList[29];
                    }

                    if (myTourRow["SafetyDirPerfReport"] == System.DBNull.Value) {
                    } else {
                        safetyDirPerfReportTextBox.Text = (String)myTourRow["SafetyDirPerfReport"];
                    }
                    
                    if ( myTourRow["RopeHandlesSpecs"] == System.DBNull.Value ) {
                    } else {
                        curValueList = ( (String)myTourRow["RopeHandlesSpecs"] ).Split( ';' );
                        RopeHandle1TextBox.Text = curValueList[1];
                        RopeHandle2TextBox.Text = curValueList[2];
                        RopeHandle3TextBox.Text = curValueList[3];
                        RopeHandle4TextBox.Text = curValueList[4];
                    }

                    if ( myTourRow["JumpRopesSpecs"] == System.DBNull.Value ) {
                    } else {
                        curValueList = ( (String)myTourRow["JumpRopesSpecs"] ).Split( ';' );
                        JumpLine1TextBox.Text = curValueList[1];
                        JumpLine2TextBox.Text = curValueList[2];
                        JumpLine3TextBox.Text = curValueList[3];
                        JumpLine4TextBox.Text = curValueList[4];
                        JumpHandle1TextBox.Text = curValueList[6];
                        JumpHandle2TextBox.Text = curValueList[7];
                        JumpHandle3TextBox.Text = curValueList[8];
                        JumpHandle4TextBox.Text = curValueList[9];
                    }

                    String curValue = "";
                    if (myTourRow["RuleExceptQ1"] != System.DBNull.Value) {
                        RuleExceptQ1TextBox.Text = (String)myTourRow["RuleExceptQ1"];
                    } else {
                        RuleExceptQ1TextBox.Text = "";
                    }
                    if (myTourRow["RuleExceptQ2"] != System.DBNull.Value) {
                        RuleExceptQ2TextBox.Text = (String)myTourRow["RuleExceptQ2"];
                    } else {
                        RuleExceptQ2TextBox.Text = "";
                    }
                    if ( myTourRow["RuleExceptQ3"] != System.DBNull.Value ) {
                        curValue = (String)myTourRow["RuleExceptQ3"];
                    } else {
                        curValue = "";
                    }
                    if ( curValue.Equals( "Y" ) ) {
                        RuleExceptQ3Yes.Checked = true;
                    } else if ( curValue.Equals( "N" ) ) {
                        RuleExceptQ3No.Checked = true;
                    } else if ( curValue.Equals( "A" ) ) {
                        RuleExceptQ3NA.Checked = true;
                    } else {
                        RuleExceptQ3NA.Checked = true;
                    }

                    if ( myTourRow["RuleExceptQ4"] != System.DBNull.Value ) {
                        curValue = (String)myTourRow["RuleExceptQ4"];
                    } else {
                        curValue = "";
                    }
                    if ( curValue.Equals( "Y" ) ) {
                        RuleExceptQ4Yes.Checked = true;
                    } else if ( curValue.Equals( "N" ) ) {
                        RuleExceptQ4No.Checked = true;
                    } else if ( curValue.Equals( "A" ) ) {
                        RuleExceptQ4NA.Checked = true;
                    } else {
                        RuleExceptQ4NA.Checked = true;
                    }

                    if (myTourRow["RuleInterQ1"] != System.DBNull.Value) {
                        RuleInterQ1TextBox.Text = (String)myTourRow["RuleInterQ1"];
                    } else {
                        RuleInterQ1TextBox.Text = "";
                    }
                    if (myTourRow["RuleInterQ2"] != System.DBNull.Value) {
                        RuleInterQ2TextBox.Text = (String)myTourRow["RuleInterQ2"];
                    } else {
                        RuleInterQ2TextBox.Text = "";
                    }
                    if (myTourRow["RuleInterQ3"] != System.DBNull.Value) {
                        curValue = (String)myTourRow["RuleInterQ3"];
                    } else {
                        curValue = "";
                    }
                    if ( curValue.Equals( "Y" ) ) {
                        RuleInterQ3Yes.Checked = true;
                    } else if ( curValue.Equals( "N" ) ) {
                        RuleInterQ3No.Checked = true;
                    } else if ( curValue.Equals( "A" ) ) {
                        RuleInterQ3NA.Checked = true;
                    } else {
                        RuleInterQ3NA.Checked = true;
                    }

                    if ( myTourRow["RuleInterQ4"] != System.DBNull.Value ) {
                        curValue = (String)myTourRow["RuleInterQ4"];
                    } else {
                        curValue = "";
                    }
                    if ( curValue.Equals( "Y" ) ) {
                        RuleInterQ4Yes.Checked = true;
                    } else if ( curValue.Equals( "N" ) ) {
                        RuleInterQ4No.Checked = true;
                    } else if ( curValue.Equals( "A" ) ) {
                        RuleInterQ4NA.Checked = true;
                    } else {
                        RuleInterQ4NA.Checked = true;
                    }

                    if (myJumpMeterSetupRow != null) {
                        if (myJumpMeterSetupRow["AngleAtoB"] == System.DBNull.Value) {
                        } else {
                            angleAtoBTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleAtoB"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleAtoC"] == System.DBNull.Value) {
                        } else {
                            angleAtoCTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleAtoC"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleBtoA"] == System.DBNull.Value) {
                        } else {
                            angleBtoATextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleBtoA"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleBtoC"] == System.DBNull.Value) {
                        } else {
                            angleBtoCTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleBtoC"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleCtoA"] == System.DBNull.Value) {
                        } else {
                            angleCtoATextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleCtoA"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleCtoB"] == System.DBNull.Value) {
                        } else {
                            angleCtoBTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleCtoB"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleAtoZ"] == System.DBNull.Value) {
                        } else {
                            angleAtoZTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleAtoZ"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleBtoZ"] == System.DBNull.Value) {
                        } else {
                            angleBtoZTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleBtoZ"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleCtoZ"] == System.DBNull.Value) {
                        } else {
                            angleCtoZTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleCtoZ"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleAto15ET"] == System.DBNull.Value) {
                        } else {
                            angleAto15ETTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleAto15ET"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleBto15ET"] == System.DBNull.Value) {
                        } else {
                            angleBto15ETTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleBto15ET"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["AngleCto15ET"] == System.DBNull.Value) {
                        } else {
                            angleCto15ETTextBox.Text = ( (Decimal)myJumpMeterSetupRow["AngleCto15ET"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["DistAtoB"] == System.DBNull.Value) {
                        } else {
                            distAtoBTextBox.Text = ( (Decimal)myJumpMeterSetupRow["DistAtoB"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["DistAtoC"] == System.DBNull.Value) {
                        } else {
                            distAtoCTextBox.Text = ( (Decimal)myJumpMeterSetupRow["DistAtoC"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["DistBtoC"] == System.DBNull.Value) {
                        } else {
                            distBtoCTextBox.Text = ( (Decimal)myJumpMeterSetupRow["DistBtoC"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["Triangle15ET"] == System.DBNull.Value) {
                        } else {
                            triangle15ETTextBox.Text = ( (Decimal)myJumpMeterSetupRow["Triangle15ET"] ).ToString();
                        }
                        if (myJumpMeterSetupRow["TriangleZero"] == System.DBNull.Value) {
                        } else {
                            triangleZeroTextBox.Text = ( (Decimal)myJumpMeterSetupRow["TriangleZero"] ).ToString();
                        }
                    }

                    if ( myTourRow["ChiefJudgeName"] != System.DBNull.Value ) {
                        curValue = (String)myTourRow["ChiefJudgeName"];
                        int curDelim = curValue.IndexOf( ',' );
                        ChiefJudgeSigTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                        ChiefJudgeNameTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                        ChiefJudgeSigDateTextBox.Text = DateTime.Today.ToString( "MM/dd/yyyy" );
                        if (myTourRow["ChiefJudgeAddress"] == System.DBNull.Value) {
                            ChiefJudgeAddressTextBox.Text = "";
                        } else {
                            ChiefJudgeAddressTextBox.Text = (String)myTourRow["ChiefJudgeAddress"];
                        }
                        if (myTourRow["ChiefJudgePhone"] == System.DBNull.Value) {
                            ChiefJudgeDayPhoneTextBox.Text = "";
                        } else {
                            ChiefJudgeDayPhoneTextBox.Text = (String)myTourRow["ChiefJudgePhone"];
                        }
                        curValue = (String)myTourRow["ChiefJudgeMemberId"];
                        DataRow curMemberRow = getTourMemberRating( curValue );
                        if (curMemberRow != null) {
                            if (curMemberRow["JudgeSlalomRating"] == System.DBNull.Value) {
                                curValue = "";
                            } else if (((String)curMemberRow["JudgeSlalomRating"]).Equals("Unrated")) {
                            } else {
                                curValue = (String)curMemberRow["JudgeSlalomRating"];
                            }
                            ChiefJudgeRatingTextBox.Text = curValue;
                        }
                    }

                    if ( myTourRow["ChiefSafetyName"] != System.DBNull.Value ) {
                        curValue = (String)myTourRow["ChiefSafetyName"];
                        int curDelim = curValue.IndexOf( ',' );
                        SafetyDirNameTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                    }

                    validateData();

                } else {
                    MessageBox.Show("Tournament data not available." );
                }
            }
        }

        public void ExportButton_Click( object sender, EventArgs e ) {
            if ( myTourRow != null ) {
                if ( myDataValid ) {
                    ExportReportTxtV2();
                    //ExportReportTxt();
                } else {
                    MessageBox.Show( "Report can not be produced because some data is missing or invalid" );
                }
            }
        }

        private void  ExportReportTxt() {
            String curMethodName = "ChiefJudgeReport:ExportReportTxt";
            String tabDelim = "\t";

            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = (String)myTourRow["SanctionId"] + "CJ" + ".txt";
            String curReportTitle = "Chief Judge Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            outBuffer = getExportFile( curFilename );
            if ( outBuffer != null ) {
                try {
                    Log.WriteFile( "Export chief judge data file begin: " + curFilename );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "SanctionNumber" + tabDelim );
                    outLine.Append( "Name" + tabDelim );
                    outLine.Append( "EventDates" + tabDelim );
                    outLine.Append( "EventLocation" + tabDelim );
                    outLine.Append( "ChiefJudgeName" + tabDelim );
                    outLine.Append( "ChiefJudgeRating" + tabDelim );
                    outLine.Append( "ChiefJudgeAddress" + tabDelim );
                    outLine.Append( "ChiefJudgeDayPhone" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "Tournament" + tabDelim );
                    outLine.Append( sanctionIdTextBox.Text + myTourClass + tabDelim );
                    outLine.Append( nameTextBox.Text + tabDelim );
                    outLine.Append( eventDatesTextBox.Text + tabDelim );
                    outLine.Append( eventLocationTextBox.Text + tabDelim );
                    outLine.Append( ChiefJudgeNameTextBox.Text + tabDelim );
                    outLine.Append( ChiefJudgeRatingTextBox.Text + tabDelim );
                    outLine.Append( ChiefJudgeAddressTextBox.Text + tabDelim );
                    outLine.Append( ChiefJudgeDayPhoneTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Rule Exceptions
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "RuleExceptQ1" + tabDelim );
                    outLine.Append( "RuleExceptQ2" + tabDelim );
                    outLine.Append( "RuleExceptQ3" + tabDelim );
                    outLine.Append( "RuleExceptQ4" + tabDelim );
                    outLine.Append( "ruleExceptions" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "RuleExceptions" + tabDelim );
                    outLine.Append( RuleExceptQ1TextBox.Text + tabDelim );
                    outLine.Append( RuleExceptQ2TextBox.Text + tabDelim );
                    outLine.Append( RuleExceptQ3GroupBox.Text + tabDelim );
                    outLine.Append( RuleExceptQ4Yes.Text + tabDelim );
                    outLine.Append( replaceLinefeed( ruleExceptionsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Interpretations of the Rules
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "RuleInterQ1" + tabDelim );
                    outLine.Append( "RuleInterQ2" + tabDelim );
                    outLine.Append( "RuleInterQ3" + tabDelim );
                    outLine.Append( "RuleInterQ4" + tabDelim );
                    outLine.Append( "ruleInterpretations" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "RuleInterpretations" + tabDelim );
                    outLine.Append( RuleInterQ1TextBox.Text + tabDelim );
                    outLine.Append( RuleInterQ2TextBox.Text + tabDelim );
                    outLine.Append( RuleInterQ3GroupBox.Text + tabDelim );
                    outLine.Append( RuleInterQ4Yes.Text + tabDelim );
                    outLine.Append( replaceLinefeed( ruleInterpretationsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Chief Safety Director's Performance Report
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "SafetyDirName" + tabDelim );
                    outLine.Append( "safetyDirPerfReport" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "Safety" + tabDelim );
                    outLine.Append( SafetyDirNameTextBox.Text + tabDelim );
                    outLine.Append( replaceLinefeed( safetyDirPerfReportTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Technical Report Slalom
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "Rope1Line2300" + tabDelim );
                    outLine.Append( "Rope1Line1825" + tabDelim );
                    outLine.Append( "Rope1Line1600" + tabDelim );
                    outLine.Append( "Rope1Line1425" + tabDelim );
                    outLine.Append( "Rope1Line1300" + tabDelim );
                    outLine.Append( "Rope1Line1200" + tabDelim );
                    outLine.Append( "Rope1Line1125" + tabDelim );
                    outLine.Append( "Rope1Line1075" + tabDelim );
                    outLine.Append( "Rope1Line1025" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "TechSlalomRope1" + tabDelim );
                    outLine.Append( Rope1Line2300TextBox.Text + tabDelim );
                    outLine.Append( Rope1Line1825TextBox.Text + tabDelim );
                    outLine.Append( Rope1Line1600TextBox.Text + tabDelim );
                    outLine.Append( Rope1Line1425TextBox.Text + tabDelim );
                    outLine.Append( Rope1Line1300TextBox.Text + tabDelim );
                    outLine.Append( Rope1Line1200TextBox.Text + tabDelim );
                    outLine.Append( Rope1Line1125TextBox.Text + tabDelim );
                    outLine.Append( Rope1Line1075TextBox.Text + tabDelim );
                    outLine.Append( Rope1Line1025TextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "Rope2Line2300" + tabDelim );
                    outLine.Append( "Rope2Line1825" + tabDelim );
                    outLine.Append( "Rope2Line1600" + tabDelim );
                    outLine.Append( "Rope2Line1425" + tabDelim );
                    outLine.Append( "Rope2Line1300" + tabDelim );
                    outLine.Append( "Rope2Line1200" + tabDelim );
                    outLine.Append( "Rope2Line1125" + tabDelim );
                    outLine.Append( "Rope2Line1075" + tabDelim );
                    outLine.Append( "Rope2Line1025" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "TechSlalomRope2" + tabDelim );
                    outLine.Append( Rope2Line2300TextBox.Text + tabDelim );
                    outLine.Append( Rope2Line1825TextBox.Text + tabDelim );
                    outLine.Append( Rope2Line1600TextBox.Text + tabDelim );
                    outLine.Append( Rope2Line1425TextBox.Text + tabDelim );
                    outLine.Append( Rope2Line1300TextBox.Text + tabDelim );
                    outLine.Append( Rope2Line1200TextBox.Text + tabDelim );
                    outLine.Append( Rope2Line1125TextBox.Text + tabDelim );
                    outLine.Append( Rope2Line1075TextBox.Text + tabDelim );
                    outLine.Append( Rope2Line1025TextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "Rope3Line2300" + tabDelim );
                    outLine.Append( "Rope3Line1825" + tabDelim );
                    outLine.Append( "Rope3Line1600" + tabDelim );
                    outLine.Append( "Rope3Line1425" + tabDelim );
                    outLine.Append( "Rope3Line1300" + tabDelim );
                    outLine.Append( "Rope3Line1200" + tabDelim );
                    outLine.Append( "Rope3Line1125" + tabDelim );
                    outLine.Append( "Rope3Line1075" + tabDelim );
                    outLine.Append( "Rope3Line1025" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "TechSlalomRope3" + tabDelim );
                    outLine.Append( Rope3Line2300TextBox.Text + tabDelim );
                    outLine.Append( Rope3Line1825TextBox.Text + tabDelim );
                    outLine.Append( Rope3Line1600TextBox.Text + tabDelim );
                    outLine.Append( Rope3Line1425TextBox.Text + tabDelim );
                    outLine.Append( Rope3Line1300TextBox.Text + tabDelim );
                    outLine.Append( Rope3Line1200TextBox.Text + tabDelim );
                    outLine.Append( Rope3Line1125TextBox.Text + tabDelim );
                    outLine.Append( Rope3Line1075TextBox.Text + tabDelim );
                    outLine.Append( Rope3Line1025TextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "RopeHandle1" + tabDelim );
                    outLine.Append( "RopeHandle2" + tabDelim );
                    outLine.Append( "RopeHandle3" + tabDelim );
                    outLine.Append( "RopeHandle4" + tabDelim );
                    outLine.Append( "slalomCourseSpecs" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "TechSlalom" + tabDelim );
                    outLine.Append( RopeHandle1TextBox.Text + tabDelim );
                    outLine.Append( RopeHandle2TextBox.Text + tabDelim );
                    outLine.Append( RopeHandle3TextBox.Text + tabDelim );
                    outLine.Append( RopeHandle4TextBox.Text + tabDelim );
                    outLine.Append( replaceLinefeed( slalomCourseSpecsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Technical Report Trick
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "trickCourseSpecs" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "TechTrick" + tabDelim );
                    outLine.Append( replaceLinefeed( trickCourseSpecsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Technical Report Jump
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "JumpLine1" + tabDelim );
                    outLine.Append( "JumpLine2" + tabDelim );
                    outLine.Append( "JumpLine3" + tabDelim );
                    outLine.Append( "JumpLine4" + tabDelim );
                    outLine.Append( "JumpHandle1" + tabDelim );
                    outLine.Append( "JumpHandle2" + tabDelim );
                    outLine.Append( "JumpHandle3" + tabDelim );
                    outLine.Append( "JumpHandle4" + tabDelim );
                    outLine.Append( "jumpCourseSpecs" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "TechJump" + tabDelim );
                    outLine.Append( JumpLine1TextBox.Text + tabDelim );
                    outLine.Append( JumpLine2TextBox.Text + tabDelim );
                    outLine.Append( JumpLine3TextBox.Text + tabDelim );
                    outLine.Append( JumpLine4TextBox.Text + tabDelim );
                    outLine.Append( JumpHandle1TextBox.Text + tabDelim );
                    outLine.Append( JumpHandle2TextBox.Text + tabDelim );
                    outLine.Append( JumpHandle3TextBox.Text + tabDelim );
                    outLine.Append( JumpHandle4TextBox.Text + tabDelim );
                    outLine.Append( replaceLinefeed( jumpCourseSpecsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Technical Report Jump
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "angleAtoB" + tabDelim );
                    outLine.Append( "angleAtoC" + tabDelim );
                    outLine.Append( "angleBtoA" + tabDelim );
                    outLine.Append( "angleBtoC" + tabDelim );
                    outLine.Append( "angleCtoA" + tabDelim );
                    outLine.Append( "angleCtoB" + tabDelim );
                    outLine.Append( "angleAtoZ" + tabDelim );
                    outLine.Append( "angleBtoZ" + tabDelim );
                    outLine.Append( "angleCtoZ" + tabDelim );
                    outLine.Append( "angleAto15ET" + tabDelim );
                    outLine.Append( "angleBto15ET" + tabDelim );
                    outLine.Append( "angleCto15ET" + tabDelim );
                    outLine.Append( "distAtoB" + tabDelim );
                    outLine.Append( "distAtoC" + tabDelim );
                    outLine.Append( "distBtoC" + tabDelim );
                    outLine.Append( "triangle15ET" + tabDelim );
                    outLine.Append( "triangleZero" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "TechJumpMeters" + tabDelim );
                    outLine.Append( angleAtoBTextBox.Text + tabDelim );
                    outLine.Append( angleAtoCTextBox.Text + tabDelim );
                    outLine.Append( angleBtoATextBox.Text + tabDelim );
                    outLine.Append( angleBtoCTextBox.Text + tabDelim );
                    outLine.Append( angleCtoATextBox.Text + tabDelim );
                    outLine.Append( angleCtoBTextBox.Text + tabDelim );
                    outLine.Append( angleAtoZTextBox.Text + tabDelim );
                    outLine.Append( angleBtoZTextBox.Text + tabDelim );
                    outLine.Append( angleCtoZTextBox.Text + tabDelim );
                    outLine.Append( angleAto15ETTextBox.Text + tabDelim );
                    outLine.Append( angleBto15ETTextBox.Text + tabDelim );
                    outLine.Append( angleCto15ETTextBox.Text + tabDelim );
                    outLine.Append( distAtoBTextBox.Text + tabDelim );
                    outLine.Append( distAtoCTextBox.Text + tabDelim );
                    outLine.Append( distBtoCTextBox.Text + tabDelim );
                    outLine.Append( triangle15ETTextBox.Text + tabDelim );
                    outLine.Append( triangleZeroTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" + tabDelim );
                    outLine.Append( "buoySpecs" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "TechBuoys" + tabDelim );
                    outLine.Append( replaceLinefeed( buoySpecsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    Log.WriteFile( "Export chief judge data file complete: " + curFilename );
                } catch ( Exception ex ) {
                    MessageBox.Show( "Error: Failure detected writing Chief Judges Extract file (txt)\n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
                outBuffer.Close();
            }
        }

        private void ExportReportTxtV2() {
            String curMethodName = "ChiefJudgeReport:ExportReportTxt";
            String tabDelim = "\t";

            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = (String)myTourRow["SanctionId"] + "CJ" + ".txt";
            String curReportTitle = "Chief Judge Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            outBuffer = getExportFile( curFilename );
            if (outBuffer != null) {
                try {
                    Log.WriteFile( "Export chief judge data file begin: " + curFilename );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( "SanctionNumber" );
                    outLine.Append( tabDelim + "Name" );
                    outLine.Append( tabDelim + "Class" );
                    outLine.Append( tabDelim + "Federation" );
                    outLine.Append( tabDelim + "Rules" );
                    outLine.Append( tabDelim + "EventDates" );
                    outLine.Append( tabDelim + "EventLocation" );

                    outLine.Append( tabDelim + "SlalomRounds" );
                    outLine.Append( tabDelim + "TrickRounds" );
                    outLine.Append( tabDelim + "JumpRounds" );

                    outLine.Append( tabDelim + "ContactMemberId" );
                    outLine.Append( tabDelim + "ContactMember" );
                    outLine.Append( tabDelim + "ContactPhone" );
                    outLine.Append( tabDelim + "ContactEmail" );
                    outLine.Append( tabDelim + "ContactAddress" );

                    outLine.Append( tabDelim + "ChiefJudgeMemberId" );
                    outLine.Append( tabDelim + "ChiefJudgeMember" );
                    outLine.Append( tabDelim + "ChiefJudgeRating" );
                    outLine.Append( tabDelim + "ChiefJudgeAddress" );
                    outLine.Append( tabDelim + "ChiefJudgePhone" );
                    outLine.Append( tabDelim + "ChiefJudgeEmail" );

                    outLine.Append( tabDelim + "ChiefDriverMemberId" );
                    outLine.Append( tabDelim + "ChiefDriverMember" );
                    outLine.Append( tabDelim + "ChiefDriverRating" );
                    outLine.Append( tabDelim + "ChiefDriverAddress" );
                    outLine.Append( tabDelim + "ChiefDriverPhone" );
                    outLine.Append( tabDelim + "ChiefDriverEmail" );

                    outLine.Append( tabDelim + "ChiefScorerMemberId" );
                    outLine.Append( tabDelim + "ChiefScorerMember" );
                    outLine.Append( tabDelim + "ChiefScorerRating" );
                    outLine.Append( tabDelim + "ChiefScorerAddress" );
                    outLine.Append( tabDelim + "ChiefScorerPhone" );
                    outLine.Append( tabDelim + "ChiefScorerEmail" );

                    outLine.Append( tabDelim + "SafetyDirMemberId" );
                    outLine.Append( tabDelim + "SafetyDirMember" );
                    outLine.Append( tabDelim + "SafetyDirRating" );
                    outLine.Append( tabDelim + "SafetyDirAddress" );
                    outLine.Append( tabDelim + "SafetyDirPhone" );
                    outLine.Append( tabDelim + "SafetyDirEmail" );
                    //Chief Safety Director's Performance Report
                    outLine.Append( tabDelim + "safetyDirPerfReport" );

                    //Rule Exceptions
                    outLine.Append( tabDelim + "RuleExceptQ1" );
                    outLine.Append( tabDelim + "RuleExceptQ2" );
                    outLine.Append( tabDelim + "RuleExceptQ3" );
                    outLine.Append( tabDelim + "RuleExceptQ4" );
                    outLine.Append( tabDelim + "ruleExceptions" );

                    //Interpretations of the Rules
                    outLine.Append( tabDelim + "RuleInterQ1" );
                    outLine.Append( tabDelim + "RuleInterQ2" );
                    outLine.Append( tabDelim + "RuleInterQ3" );
                    outLine.Append( tabDelim + "RuleInterQ4" );
                    outLine.Append( tabDelim + "ruleInterpretations" );

                    //Technical Report Slalom
                    outLine.Append( tabDelim + "RopeHandle1" );
                    outLine.Append( tabDelim + "RopeHandle2" );
                    outLine.Append( tabDelim + "RopeHandle3" );
                    outLine.Append( tabDelim + "RopeHandle4" );

                    outLine.Append( tabDelim + "Rope1Line2300" );
                    outLine.Append( tabDelim + "Rope1Line1825" );
                    outLine.Append( tabDelim + "Rope1Line1600" );
                    outLine.Append( tabDelim + "Rope1Line1425" );
                    outLine.Append( tabDelim + "Rope1Line1300" );
                    outLine.Append( tabDelim + "Rope1Line1200" );
                    outLine.Append( tabDelim + "Rope1Line1125" );
                    outLine.Append( tabDelim + "Rope1Line1075" );
                    outLine.Append( tabDelim + "Rope1Line1025" );

                    outLine.Append( tabDelim + "Rope2Line2300" );
                    outLine.Append( tabDelim + "Rope2Line1825" );
                    outLine.Append( tabDelim + "Rope2Line1600" );
                    outLine.Append( tabDelim + "Rope2Line1425" );
                    outLine.Append( tabDelim + "Rope2Line1300" );
                    outLine.Append( tabDelim + "Rope2Line1200" );
                    outLine.Append( tabDelim + "Rope2Line1125" );
                    outLine.Append( tabDelim + "Rope2Line1075" );
                    outLine.Append( tabDelim + "Rope2Line1025" );

                    outLine.Append( tabDelim + "Rope3Line2300" );
                    outLine.Append( tabDelim + "Rope3Line1825" );
                    outLine.Append( tabDelim + "Rope3Line1600" );
                    outLine.Append( tabDelim + "Rope3Line1425" );
                    outLine.Append( tabDelim + "Rope3Line1300" );
                    outLine.Append( tabDelim + "Rope3Line1200" );
                    outLine.Append( tabDelim + "Rope3Line1125" );
                    outLine.Append( tabDelim + "Rope3Line1075" );
                    outLine.Append( tabDelim + "Rope3Line1025" );

                    //Technical Report Jump
                    outLine.Append( tabDelim + "JumpHandle1" );
                    outLine.Append( tabDelim + "JumpHandle2" );
                    outLine.Append( tabDelim + "JumpHandle3" );
                    outLine.Append( tabDelim + "JumpHandle4" );

                    outLine.Append( tabDelim + "JumpLine1" );
                    outLine.Append( tabDelim + "JumpLine2" );
                    outLine.Append( tabDelim + "JumpLine3" );
                    outLine.Append( tabDelim + "JumpLine4" );

                    //Technical Report Jump Meters
                    outLine.Append( tabDelim + "angleAtoB" );
                    outLine.Append( tabDelim + "angleAtoC" );
                    outLine.Append( tabDelim + "angleBtoA" );
                    outLine.Append( tabDelim + "angleBtoC" );
                    outLine.Append( tabDelim + "angleCtoA" );
                    outLine.Append( tabDelim + "angleCtoB" );
                    outLine.Append( tabDelim + "angleAtoZ" );
                    outLine.Append( tabDelim + "angleBtoZ" );
                    outLine.Append( tabDelim + "angleCtoZ" );
                    outLine.Append( tabDelim + "angleAto15ET" );
                    outLine.Append( tabDelim + "angleBto15ET" );
                    outLine.Append( tabDelim + "angleCto15ET" );
                    outLine.Append( tabDelim + "distAtoB" );
                    outLine.Append( tabDelim + "distAtoC" );
                    outLine.Append( tabDelim + "distBtoC" );
                    outLine.Append( tabDelim + "triangle15ET" );
                    outLine.Append( tabDelim + "triangleZero" );

                    outLine.Append( tabDelim + "slalomCourseSpecs" );
                    outLine.Append( tabDelim + "trickCourseSpecs" );
                    outLine.Append( tabDelim + "jumpCourseSpecs" );
                    outLine.Append( tabDelim + "buoySpecs" );

                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( sanctionIdTextBox.Text.Substring(0, 6) );
                    outLine.Append( tabDelim + nameTextBox.Text );
                    outLine.Append( tabDelim + myTourClass );
                    outLine.Append( tabDelim + myTourRow["Federation"] );
                    outLine.Append( tabDelim + myTourRow["Rules"] );
                    outLine.Append( tabDelim + eventDatesTextBox.Text );
                    outLine.Append( tabDelim + eventLocationTextBox.Text );

                    outLine.Append( tabDelim + mySlalomRounds.ToString() );
                    outLine.Append( tabDelim + myTrickRounds.ToString() );
                    outLine.Append( tabDelim + myJumpRounds.ToString() );

                    //Tournament Contact
                    outLine.Append( tabDelim + myTourRow["ContactMemberId"] );
                    outLine.Append( tabDelim + myTourRow["ContactName"] );
                    outLine.Append( tabDelim + myTourRow["ContactPhone"] );
                    outLine.Append( tabDelim + myTourRow["ContactEmail"] );
                    outLine.Append( tabDelim + myTourRow["ContactAddress"] );

                    //Chief Judge
                    outLine.Append( tabDelim + myTourRow["ChiefJudgeMemberId"] );
                    outLine.Append( tabDelim + ChiefJudgeNameTextBox.Text );
                    outLine.Append( tabDelim + ChiefJudgeRatingTextBox.Text );
                    outLine.Append( tabDelim + ChiefJudgeAddressTextBox.Text );
                    outLine.Append( tabDelim + ChiefJudgeDayPhoneTextBox.Text );
                    outLine.Append( tabDelim + myTourRow["ChiefJudgeEmail"] );

                    //Chief Driver
                    outLine.Append( tabDelim + myTourRow["ChiefDriverMemberId"] );
                    outLine.Append( tabDelim + myTourRow["ChiefDriverName"] );
                    String curValue = (String)myTourRow["ChiefDriverMemberId"];
                    DataRow curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["DriverSlalomRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["DriverSlalomRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["DriverSlalomRating"];
                        }
                    }
                    outLine.Append( tabDelim + curValue );
                    outLine.Append( tabDelim + myTourRow["ChiefDriverAddress"] );
                    outLine.Append( tabDelim + myTourRow["ChiefDriverPhone"] );
                    outLine.Append( tabDelim + myTourRow["ChiefDriverEmail"] );

                    //Chief Scorer
                    //curSqlStmt.Append( ", T.ChiefScorerMemberId, TourRegCC.SkierName AS ChiefScorerName, T.ChiefScorerAddress, T.ChiefScorerPhone, T.ChiefScorerEmail " );
                    outLine.Append( tabDelim + myTourRow["ChiefScorerMemberId"] );
                    outLine.Append( tabDelim + myTourRow["ChiefScorerName"] );
                    curValue = (String)myTourRow["ChiefScorerMemberId"];
                    curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["ScorerSlalomRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["ScorerSlalomRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["ScorerSlalomRating"];
                        }
                    }
                    outLine.Append( tabDelim + curValue );
                    outLine.Append( tabDelim + myTourRow["ChiefScorerAddress"] );
                    outLine.Append( tabDelim + myTourRow["ChiefScorerPhone"] );
                    outLine.Append( tabDelim + myTourRow["ChiefScorerEmail"] );

                    //Chief Safety Director
                    //curSqlStmt.Append( ", T.SafetyDirMemberId, TourRegCS.SkierName AS ChiefSafetyName, T.SafetyDirAddress, T.SafetyDirPhone, T.SafetyDirEmail" );
                    outLine.Append( tabDelim + myTourRow["SafetyDirMemberId"] );
                    outLine.Append( tabDelim + SafetyDirNameTextBox.Text );
                    curValue = (String)myTourRow["SafetyDirMemberId"];
                    curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["SafetyOfficialRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["SafetyOfficialRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["SafetyOfficialRating"];
                        }
                    }
                    outLine.Append( tabDelim + curValue );
                    outLine.Append( tabDelim + myTourRow["SafetyDirAddress"] );
                    outLine.Append( tabDelim + myTourRow["SafetyDirPhone"] );
                    outLine.Append( tabDelim + myTourRow["SafetyDirEmail"] );

                    //Chief Safety Director's Performance Report
                    outLine.Append( tabDelim + replaceLinefeed( safetyDirPerfReportTextBox.Text ) );

                    //Rule Exceptions
                    outLine.Append( tabDelim + RuleExceptQ1TextBox.Text );
                    outLine.Append( tabDelim + RuleExceptQ2TextBox.Text );
                    outLine.Append( tabDelim + RuleExceptQ3GroupBox.Text );
                    if (RuleExceptQ4Yes.Checked) {
                        outLine.Append( tabDelim + RuleExceptQ4Yes.Text );
                    } else if (RuleExceptQ4No.Checked) {
                        outLine.Append( tabDelim + RuleExceptQ4No.Text );
                    } else if (RuleExceptQ4NA.Checked) {
                        outLine.Append( tabDelim + RuleExceptQ4NA.Text );
                    } else {
                        outLine.Append( tabDelim + RuleExceptQ4NA.Text );
                    }
                    outLine.Append( tabDelim + replaceLinefeed( ruleExceptionsTextBox.Text ) );

                    //Interpretations of the Rules
                    outLine.Append( tabDelim + RuleInterQ1TextBox.Text );
                    outLine.Append( tabDelim + RuleInterQ2TextBox.Text );
                    outLine.Append( tabDelim + RuleInterQ3GroupBox.Text );
                    if (RuleInterQ4Yes.Checked) {
                        outLine.Append( tabDelim + RuleInterQ4Yes.Text );
                    } else if (RuleInterQ4No.Checked) {
                        outLine.Append( tabDelim + RuleInterQ4No.Text );
                    } else if (RuleInterQ4NA.Checked) {
                        outLine.Append( tabDelim + RuleInterQ4NA.Text );
                    } else {
                        outLine.Append( tabDelim + RuleInterQ4NA.Text );
                    }
                    outLine.Append( tabDelim + replaceLinefeed( ruleInterpretationsTextBox.Text ) );

                    //Technical Slalom Ropes
                    outLine.Append( tabDelim + encodeNumericAsText(RopeHandle1TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(RopeHandle2TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(RopeHandle3TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(RopeHandle4TextBox.Text) );

                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line2300TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line1825TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line1600TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line1425TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line1300TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line1200TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line1125TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line1075TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope1Line1025TextBox.Text) );

                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line2300TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line1825TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line1600TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line1425TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line1300TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line1200TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line1125TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line1075TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope2Line1025TextBox.Text) );

                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line2300TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line1825TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line1600TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line1425TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line1300TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line1200TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line1125TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line1075TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(Rope3Line1025TextBox.Text) );

                    //Technical Jump Ropes
                    outLine.Append( tabDelim + encodeNumericAsText(JumpHandle1TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(JumpHandle2TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(JumpHandle3TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(JumpHandle4TextBox.Text) );

                    outLine.Append( tabDelim + encodeNumericAsText(JumpLine1TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(JumpLine2TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(JumpLine3TextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(JumpLine4TextBox.Text) );

                    //Technical Jump Meters
                    outLine.Append( tabDelim + encodeNumericAsText(angleAtoBTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleAtoCTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleBtoATextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleBtoCTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleCtoATextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleCtoBTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleAtoZTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleBtoZTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleCtoZTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleAto15ETTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleBto15ETTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(angleCto15ETTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(distAtoBTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(distAtoCTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(distBtoCTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText(triangle15ETTextBox.Text) );
                    outLine.Append( tabDelim + encodeNumericAsText( triangleZeroTextBox.Text ) );

                    //Course specs
                    outLine.Append( tabDelim + replaceLinefeed( slalomCourseSpecsTextBox.Text ) );
                    outLine.Append( tabDelim + replaceLinefeed( trickCourseSpecsTextBox.Text ) );
                    outLine.Append( tabDelim + replaceLinefeed( jumpCourseSpecsTextBox.Text ) );
                    outLine.Append( tabDelim + replaceLinefeed( buoySpecsTextBox.Text ) );

                    outBuffer.WriteLine( outLine.ToString() );

                    Log.WriteFile( "Export chief judge data file complete: " + curFilename );
                } catch (Exception ex) {
                    MessageBox.Show( "Error: Failure detected writing Chief Judges Extract file (txt)\n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
                outBuffer.Close();
            }
        }

        public void ExportReportPrintFile() {
            String curMethodName = "ChiefJudgeReport:ExportReportPrintFile";

            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = (String)myTourRow["SanctionId"] + "CJ" + ".prn";
            String curReportTitle = "Chief Judge Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            outBuffer = getExportFile( curFilename );
            if (outBuffer != null) {
                try {
                    Log.WriteFile( "Export chief judge data file begin: " + curFilename );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( ChiefJudgeIntroTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + eventDatesLabel.Text + " " + eventDatesTextBox.Text );
                    outLine.Append( "  " + sanctionIdLabel.Text + " " + sanctionIdTextBox.Text.Substring( 0, 6 ) );
                    outLine.Append( "  Class: " + myTourClass );
                    outLine.Append( "  Federation: " + myTourRow["Federation"] );
                    outLine.Append( "  Rules: " + myTourRow["Rules"] );
                    outLine.Append( Environment.NewLine + nameLabel.Text + " " + nameTextBox.Text );
                    outLine.Append( Environment.NewLine + eventLocationLabel.Text + " " + eventLocationTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Chief Judge
                    outLine.Append( Environment.NewLine + ChiefJudgeNameLabel.Text + " " + ChiefJudgeNameTextBox.Text );
                    outLine.Append( "  " + ChiefJudgeRatingLabel.Text + " " + ChiefJudgeRatingTextBox.Text );
                    outLine.Append( Environment.NewLine + ChiefJudgeAddressLabel.Text + " " + ChiefJudgeAddressTextBox.Text );
                    outLine.Append( Environment.NewLine + ChiefJudgeDayPhoneLabel.Text + " " + ChiefJudgeDayPhoneTextBox.Text );
                    outLine.Append( "  Email: " + myTourRow["ChiefJudgeEmail"] );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Tournament Contact
                    outLine.Append( Environment.NewLine + "Tournament contact: " + myTourRow["ContactName"] );
                    outLine.Append( Environment.NewLine + "Phone: " + myTourRow["ContactPhone"] );
                    outLine.Append( "  Email: " + myTourRow["ContactEmail"] );
                    outLine.Append( Environment.NewLine + "Address: " + myTourRow["ContactAddress"] );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Chief Driver
                    outLine.Append( Environment.NewLine + "Chief Driver: " + myTourRow["ChiefDriverName"] );
                    String curValue = (String)myTourRow["ChiefDriverMemberId"];
                    DataRow curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["DriverSlalomRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["DriverSlalomRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["DriverSlalomRating"];
                        }
                    }
                    outLine.Append( "  Rating: " + curValue );
                    outLine.Append( Environment.NewLine + "Phone: " + myTourRow["ChiefDriverPhone"] );
                    outLine.Append( "  Email: " + myTourRow["ChiefDriverEmail"] );
                    outLine.Append( Environment.NewLine + "Address: " + myTourRow["ChiefDriverAddress"] );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Chief Scorer
                    outLine.Append( Environment.NewLine + "Chief Scorer: " + myTourRow["ChiefScorerName"] );
                    curValue = (String)myTourRow["ChiefScorerMemberId"];
                    curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["ScorerSlalomRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["ScorerSlalomRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["ScorerSlalomRating"];
                        }
                    }
                    outLine.Append( "  Rating: " + curValue );
                    outLine.Append( Environment.NewLine + "Phone: " + myTourRow["ChiefScorerPhone"] );
                    outLine.Append( "  Email: " + myTourRow["ChiefScorerEmail"] );
                    outLine.Append( Environment.NewLine + "Address: " + myTourRow["ChiefScorerAddress"] );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Chief Safety Director
                    outLine.Append( Environment.NewLine + "Chief Safety: " + SafetyDirNameTextBox.Text );
                    curValue = (String)myTourRow["SafetyDirMemberId"];
                    curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["SafetyOfficialRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["SafetyOfficialRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["SafetyOfficialRating"];
                        }
                    }
                    outLine.Append( "  Rating: " + curValue );
                    outLine.Append( Environment.NewLine + "Phone: " + myTourRow["SafetyDirPhone"] );
                    outLine.Append( "  Email: " + myTourRow["SafetyDirEmail"] );
                    outLine.Append( Environment.NewLine + "Address: " + myTourRow["SafetyDirAddress"] );
                    outLine.Append( Environment.NewLine + SafetyDirPerfLabel.Text + ": " + replaceLinefeed( safetyDirPerfReportTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + "Slalom Rounds: " + mySlalomRounds.ToString() );
                    outLine.Append( "  Trick Rounds: " + myTrickRounds.ToString() );
                    outLine.Append( "  Jump Rounds: " + myJumpRounds.ToString() );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Rule Exceptions
                    outLine.Append( Environment.NewLine + ruleExceptionsLabel.Text );
                    outLine.Append( Environment.NewLine + RuleExceptQ1Label.Text + " " + RuleExceptQ1TextBox.Text );
                    outLine.Append( Environment.NewLine + RuleExceptQ2Label.Text + " " + RuleExceptQ2TextBox.Text );
                    outLine.Append( Environment.NewLine + RuleExceptQ3Label.Text + " " + RuleExceptQ3GroupBox.Text );
                    if (RuleExceptQ4Yes.Checked) {
                        outLine.Append( Environment.NewLine + RuleExceptQ4Label.Text + " " + RuleExceptQ4Yes.Text );
                    } else if (RuleExceptQ4No.Checked) {
                        outLine.Append( Environment.NewLine + RuleExceptQ4Label.Text + " " + RuleExceptQ4No.Text );
                    } else if (RuleExceptQ4NA.Checked) {
                        outLine.Append( Environment.NewLine + RuleExceptQ4Label.Text + " " + RuleExceptQ4NA.Text );
                    } else {
                        outLine.Append( Environment.NewLine + RuleExceptQ4Label.Text + " " + RuleExceptQ4NA.Text );
                    }
                    outLine.Append( Environment.NewLine + RuleExceptQ5Label.Text + " " + replaceLinefeed( ruleExceptionsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Interpretations of the Rules
                    outLine.Append( Environment.NewLine + RuleInterLabel.Text );
                    outLine.Append( Environment.NewLine + RuleInterQ1Label.Text + " " + RuleInterQ1TextBox.Text );
                    outLine.Append( Environment.NewLine + RuleInterQ2Label.Text + " " + RuleInterQ2TextBox.Text );
                    outLine.Append( Environment.NewLine + RuleInterQ3Label.Text + " " + RuleInterQ3GroupBox.Text );
                    if (RuleInterQ4Yes.Checked) {
                        outLine.Append( Environment.NewLine + RuleInterQ4Label.Text + " " + RuleInterQ4Yes.Text );
                    } else if (RuleInterQ4No.Checked) {
                        outLine.Append( Environment.NewLine + RuleInterQ4Label.Text + " " + RuleInterQ4No.Text );
                    } else if (RuleInterQ4NA.Checked) {
                        outLine.Append( Environment.NewLine + RuleInterQ4Label.Text + " " + RuleInterQ4NA.Text );
                    } else {
                        outLine.Append( Environment.NewLine + RuleInterQ4Label.Text + " " + RuleInterQ4NA.Text );
                    }
                    outLine.Append( Environment.NewLine + RuleInterQ5Label.Text + " " + replaceLinefeed( ruleInterpretationsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Technical Slalom Ropes
                    outLine.Append( Environment.NewLine + TechReportSlalomLabel.Text );
                    outLine.Append( Environment.NewLine + RopeHandle1Label.Text + " " +  RopeHandle1TextBox.Text  );
                    outLine.Append( "  " + RopeHandle2Label.Text + " " +  RopeHandle2TextBox.Text  );
                    outLine.Append( "  " + RopeHandle3Label.Text + " " +  RopeHandle3TextBox.Text  );
                    outLine.Append( "  " + RopeHandle4Label.Text + " " +  RopeHandle4TextBox.Text  );

                    outLine.Append( Environment.NewLine + Rope1Label.Text );
                    outLine.Append( "  23M (long): " +  Rope1Line2300TextBox.Text );
                    outLine.Append( "  18.25M (-15): " + Rope1Line1825TextBox.Text );
                    outLine.Append( "  16M (-22): " + Rope1Line1600TextBox.Text );
                    outLine.Append( "  14.25M (-28): " + Rope1Line1425TextBox.Text );
                    outLine.Append( "  13M (-32): " + Rope1Line1300TextBox.Text );
                    outLine.Append( "  12M (-35): " + Rope1Line1200TextBox.Text );
                    outLine.Append( "  11.25M (-38): " + Rope1Line1125TextBox.Text );
                    outLine.Append( "  10.75M (-39): " + Rope1Line1075TextBox.Text );
                    outLine.Append( "  10.25M (-41): " + Rope1Line1025TextBox.Text );

                    outLine.Append( Environment.NewLine + Rope2Label.Text );
                    outLine.Append( "  23M (long): " + Rope2Line2300TextBox.Text );
                    outLine.Append( "  18.25M (-15): " + Rope2Line1825TextBox.Text );
                    outLine.Append( "  16M (-22): " + Rope2Line1600TextBox.Text );
                    outLine.Append( "  14.25M (-28): " +  Rope2Line1425TextBox.Text );
                    outLine.Append( "  13M (-32): " + Rope2Line1300TextBox.Text );
                    outLine.Append( "  12M (-35): " + Rope2Line1200TextBox.Text );
                    outLine.Append( "  11.25M (-38): " + Rope2Line1125TextBox.Text );
                    outLine.Append( "  10.75M (-39): " + Rope2Line1075TextBox.Text );
                    outLine.Append( "  10.25M (-41): " + Rope2Line1025TextBox.Text );

                    outLine.Append( Environment.NewLine + Rope3Label.Text );
                    outLine.Append( "  23M (long): " + Rope3Line2300TextBox.Text );
                    outLine.Append( "  18.25M (-15): " + Rope3Line1825TextBox.Text );
                    outLine.Append( "  16M (-22): " + Rope3Line1600TextBox.Text );
                    outLine.Append( "  14.25M (-28): " + Rope3Line1425TextBox.Text );
                    outLine.Append( "  13M (-32): " + Rope3Line1300TextBox.Text );
                    outLine.Append( "  12M (-35): " + Rope3Line1200TextBox.Text );
                    outLine.Append( "  11.25M (-38): " + Rope3Line1125TextBox.Text );
                    outLine.Append( "  10.75M (-39): " + Rope3Line1075TextBox.Text );
                    outLine.Append( "  10.25M (-41): " + Rope3Line1025TextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Technical Jump Ropes
                    outLine.Append( Environment.NewLine + TechReportJumpLabel.Text );
                    outLine.Append( Environment.NewLine + JumpHandle1Label.Text + " " + JumpHandle1TextBox.Text );
                    outLine.Append( "  " + JumpHandle2Label.Text + " " + JumpHandle2TextBox.Text );
                    outLine.Append( "  " + JumpHandle3Label.Text + " " + JumpHandle3TextBox.Text );
                    outLine.Append( "  " + JumpHandle4Label.Text + " " + JumpHandle4TextBox.Text );

                    outLine.Append( Environment.NewLine + JumpLine1Label.Text + " " + JumpLine1TextBox.Text );
                    outLine.Append( "  " + JumpLine1Label.Text + " " + JumpLine2TextBox.Text );
                    outLine.Append( "  " + JumpLine1Label.Text + " " + JumpLine3TextBox.Text );
                    outLine.Append( "  " + JumpLine1Label.Text + " " + JumpLine4TextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Technical Jump Meters
                    outLine.Append( Environment.NewLine + TechReportMeterLabel.Text );
                    outLine.Append( Environment.NewLine + DistToMetersLabel.Text );
                    outLine.Append( Environment.NewLine + angleAtoBLabel.Text + ": " + angleAtoBTextBox.Text );
                    outLine.Append( "  " + angleAtoCLabel.Text + ": " + angleAtoCTextBox.Text );
                    outLine.Append( "  " + angleBtoALabel.Text + ": " + angleBtoATextBox.Text );
                    outLine.Append( "  " + angleBtoCLabel.Text + ": " + angleBtoCTextBox.Text );
                    outLine.Append( "  " + angleCtoALabel.Text + ": " + angleCtoATextBox.Text );
                    outLine.Append( "  " + angleCtoBLabel.Text + ": " + angleCtoBTextBox.Text );

                    outLine.Append( Environment.NewLine + distAtoBLabel.Text + " " + distAtoBTextBox.Text );
                    outLine.Append( "  " + distAtoCLabel.Text + " " + distAtoCTextBox.Text );
                    outLine.Append( "  " + distBtoCLabel.Text + " " + distBtoCTextBox.Text );

                    outLine.Append( Environment.NewLine + Environment.NewLine + TechReportJumpSightLabel.Text );
                    outLine.Append( Environment.NewLine + label3.Text );
                    outLine.Append( Environment.NewLine + angleAtoZLabel.Text + ": " + angleAtoZTextBox.Text );
                    outLine.Append( "  " + angleBtoZLabel.Text + ": " + angleBtoZTextBox.Text );
                    outLine.Append( "  " + angleCtoZLabel.Text + ": " + angleCtoZTextBox.Text );
                    outLine.Append( "  " + triangleZeroLabel.Text + ": " + triangleZeroTextBox.Text );
                    outLine.Append( Environment.NewLine + label4.Text );
                    outLine.Append( Environment.NewLine + angleAto15ETLabel.Text + ": " + angleAto15ETTextBox.Text );
                    outLine.Append( "  " + angleBto15ETLabel.Text + ": " + angleBto15ETTextBox.Text );
                    outLine.Append( "  " + angleCto15ETLabel.Text + ": " + angleCto15ETTextBox.Text );
                    outLine.Append( "  " + triangle15ETLabel.Text + ": " + triangle15ETTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Course specs
                    outLine.Append( Environment.NewLine + slalomCourseSpecsLabel.Text + ": " + replaceLinefeed( slalomCourseSpecsTextBox.Text ) );
                    outLine.Append( Environment.NewLine + trickCourseSpecsLabel.Text + ": " + replaceLinefeed( trickCourseSpecsTextBox.Text ) );
                    outLine.Append( Environment.NewLine + jumpCourseSpecsLabel.Text + ": " + replaceLinefeed( jumpCourseSpecsTextBox.Text ) );
                    outLine.Append( Environment.NewLine + buoySpecsLabel.Text + ": " + replaceLinefeed( buoySpecsTextBox.Text ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + "Report created by " );
                    outLine.Append( Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion );
                    outLine.Append(" on " + DateTime.Now.ToString( "MMMM d, yyyy HH:mm:ss" ));
                    outBuffer.WriteLine( outLine.ToString() );

                    Log.WriteFile( "Export chief judge data print file complete: " + curFilename );
                    MessageBox.Show( "Export chief judge data print file complete:" );
                } catch (Exception ex) {
                    MessageBox.Show( "Error: Failure detected writing Chief Judges print file (txt)\n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
                outBuffer.Close();
            }
        }

        private bool validateData() {
            myDataValid = true;
            StringBuilder curDataValidMsg = new StringBuilder( "" );

            if ( myTourRow["ChiefJudgeName"] == System.DBNull.Value ) {
                myDataValid = false;
                curDataValidMsg.Append( "\n" + "Missing chief judge name" );
            } else {
                if ( ( (String)myTourRow["ChiefJudgeName"] ).Length < 5 ) {
                    myDataValid = false;
                    curDataValidMsg.Append( "\n" + "Missing chief judge name" );
                }
            }

            if ( myTourRow["ChiefSafetyName"] == System.DBNull.Value ) {
                myDataValid = false;
                curDataValidMsg.Append( "\n" + "Missing chief safety name" );
            } else {
                if ( ( (String)myTourRow["ChiefSafetyName"] ).Length < 5 ) {
                    myDataValid = false;
                    curDataValidMsg.Append( "\n" + "Missing chief safety name" );
                }
            }

            if ( !(isTourClassELR) ) {
                try {
                    mySlalomRounds = Convert.ToInt16( myTourRow["SlalomRounds"].ToString() );
                } catch {
                    mySlalomRounds = 0;
                }
                try {
                    myTrickRounds = Convert.ToInt16( myTourRow["TrickRounds"].ToString() );
                } catch {
                    myTrickRounds = 0;
                }
                try {
                    myJumpRounds = Convert.ToInt16( myTourRow["JumpRounds"].ToString() );
                } catch {
                    myJumpRounds = 0;
                }

                if ( mySlalomRounds > 0 ) {
                    if ( myTourRow["SlalomRopesSpecs"] == System.DBNull.Value ) {
                        myDataValid = false;
                        curDataValidMsg.Append( "\n" + "Missing data for slalom rope specs" );
                    } else {
                        if ( ( (String)myTourRow["SlalomRopesSpecs"] ).Length < 5 ) {
                            myDataValid = false;
                            curDataValidMsg.Append( "\n" + "Missing data for slalom rope specs" );
                        }
                    }
                }

                if ( myJumpRounds > 0 ) {
                    if ( myTourRow["JumpRopesSpecs"] == System.DBNull.Value ) {
                        myDataValid = false;
                        curDataValidMsg.Append( "\n" + "Missing data for jump rope specs" );
                    } else {
                        if ( ( (String)myTourRow["JumpRopesSpecs"] ).Length < 5 ) {
                            myDataValid = false;
                            curDataValidMsg.Append( "\n" + "Missing data for jump rope specs" );
                        }
                    }
                }

                if ( mySlalomRounds > 0 ) {
                    if ( myTourRow["SlalomCourseSpecs"] == System.DBNull.Value ) {
                        myDataValid = false;
                        curDataValidMsg.Append( "\n" + "Slalom course specifications must be provided" );
                    } else {
                        if ( ( (String)myTourRow["SlalomCourseSpecs"] ).Length < 2 ) {
                            myDataValid = false;
                            curDataValidMsg.Append( "\n" + "Slalom course specifications must be provided" );
                        }
                    }
                }

                if ( myTrickRounds > 0 ) {
                    if ( myTourRow["TrickCourseSpecs"] == System.DBNull.Value ) {
                        myDataValid = false;
                        curDataValidMsg.Append( "\n" + "Trick course specifications must be provided" );
                    } else {
                        if ( ( (String)myTourRow["TrickCourseSpecs"] ).Length < 2 ) {
                            myDataValid = false;
                            curDataValidMsg.Append( "\n" + "Trick course specifications must be provided" );
                        }
                    }
                }

                if ( myJumpRounds > 0 ) {
                    if ( myTourRow["JumpCourseSpecs"] == System.DBNull.Value ) {
                        myDataValid = false;
                        curDataValidMsg.Append( "\n" + "Jump course specifications must be provided" );
                    } else {
                        if ( ( (String)myTourRow["JumpCourseSpecs"] ).Length < 2 ) {
                            myDataValid = false;
                            curDataValidMsg.Append( "\n" + "Jump course specifications must be provided" );
                        }
                    }
                }

                if ( myTourRow["BuoySpecs"] == System.DBNull.Value ) {
                    myDataValid = false;
                    curDataValidMsg.Append( "\n" + "Buoy specifications must be provided" );
                } else {
                    if ( ( (String)myTourRow["BuoySpecs"] ).Length < 2 ) {
                        myDataValid = false;
                        curDataValidMsg.Append( "\n" + "Buoy specifications must be provided" );
                    }
                }
            }

            if ( !( myDataValid ) ) {
                MessageBox.Show( "Missing or invalid data detected.  You must correct the data before the report can be produced."
                    + curDataValidMsg.ToString() );
            }

            return myDataValid;
        }

        private String replaceLinefeed( String inValue ) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            return curValue;
        }

        private String encodeSpecialChar( String inValue ) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            curValue = curValue.Replace( ",", "%2C" );
            return curValue;
        }

        private String encodeNumericAsText(String inValue) {
            String curReturnValue = inValue;
            if (inValue.Contains( "'" ) || inValue.Contains( "\"" )) {
            } else {
                curReturnValue += "\"";
            }

            return curReturnValue;
        }

        public void PrintButton_Click( object sender, EventArgs e ) {
            String curPath = Properties.Settings.Default.ExportDirectory;
            Directory.SetCurrentDirectory( curPath );
            if ( myDataValid ) {
                foreach ( TabPage curPage in ReportTabControl.TabPages ) {
                    curPage.Select();
                    curPage.Focus();
                    curPage.Show();
                }
                ReportTabControl.TabPages[0].Select();
                ReportTabControl.TabPages[0].Focus();
                ReportTabControl.TabPages[0].Show();

                FormReportPrinter curFormPrint = new FormReportPrinter( ReportTabControl );
                curFormPrint.ReportName = (String)myTourRow["SanctionId"] + "CJ";
                curFormPrint.ReportHeader = "Chief Judge Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
                curFormPrint.CenterHeaderOnPage = true;
                curFormPrint.ReportHeaderFont = new Font( "Arial", 12, FontStyle.Bold, GraphicsUnit.Point );
                curFormPrint.ReportHeaderTextColor = Color.Black;

                curFormPrint.BottomMargin = 75;
                curFormPrint.TopMargin = 50;
                curFormPrint.LeftMargin = 50;
                curFormPrint.RightMargin = 50;

                curFormPrint.Print(false);
            } else {
                MessageBox.Show( "Report can not be produced because some data is missing or invalid" );
            }
        }

        private void PrintToFileButton_Click(object sender, EventArgs e) {
            //Not currently used
            String curPath = Properties.Settings.Default.ExportDirectory;
            Directory.SetCurrentDirectory( curPath );
            if (myDataValid) {
                foreach (TabPage curPage in ReportTabControl.TabPages) {
                    curPage.Select();
                    curPage.Focus();
                    curPage.Show();
                }
                ReportTabControl.TabPages[0].Select();
                ReportTabControl.TabPages[0].Focus();
                ReportTabControl.TabPages[0].Show();

                FormReportToFile curFormPrint = new FormReportToFile( ReportTabControl );
                curFormPrint.ReportName = (String)myTourRow["SanctionId"] + "CJ.prn";
                curFormPrint.ReportHeader = "Chief Judge Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
                curFormPrint.Write( curFormPrint.ReportName );
            } else {
                MessageBox.Show( "Report can not be produced because some data is missing or invalid" );
            }
        }

        private void ChiefJudgeReport_FormClosed(object sender, FormClosedEventArgs e) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.ChiefJudgeReport_Width = this.Size.Width;
                Properties.Settings.Default.ChiefJudgeReport_Height = this.Size.Height;
                Properties.Settings.Default.ChiefJudgeReport_Location = this.Location;
            }
        }

        private StreamWriter getExportFile( String inFileName ) {
            StreamWriter outBuffer = null;

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.FileName = inFileName;

            try {
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    String myFileName = myFileDialog.FileName;
                    if ( myFileName != null ) {
                        int delimPos = myFileName.LastIndexOf( '\\' );
                        String curFileName = myFileName.Substring( delimPos + 1 );
                        if ( curFileName.IndexOf( '.' ) < 0 ) {
                            myFileName += ".txt";
                        }
                        outBuffer = File.CreateText( myFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

        private DataRow getTourData() {
            DataRow curRow = null;
            try {
                //Retrieve selected tournament attributes
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT Distinct T.SanctionId, T.Name, T.Federation, T.TourDataLoc, T.LastUpdateDate, T.Class, COALESCE(L.CodeValue, 'C') as EventScoreClass" );
                curSqlStmt.Append( ", T.SlalomRounds, T.TrickRounds, T.JumpRounds, T.Rules, T.EventDates, T.EventLocation" );
                curSqlStmt.Append( ", T.HcapSlalomBase, T.HcapTrickBase, T.HcapJumpBase, T.HcapSlalomPct, T.HcapTrickPct, T.HcapJumpPct " );
                curSqlStmt.Append( ", T.RopeHandlesSpecs, T.SlalomRopesSpecs, T.JumpRopesSpecs, T.SlalomCourseSpecs, T.JumpCourseSpecs, T.TrickCourseSpecs, T.BuoySpecs" );
                curSqlStmt.Append( ", T.SafetyDirPerfReport, T.RuleExceptions, T.RuleExceptQ1, T.RuleExceptQ2, T.RuleExceptQ3, T.RuleExceptQ4" );
                curSqlStmt.Append( ", T.RuleInterpretations, T.RuleInterQ1, T.RuleInterQ2, T.RuleInterQ3, T.RuleInterQ4" );
                curSqlStmt.Append( ", T.ContactMemberId, TourRegCO.SkierName AS ContactName, T.ContactPhone, T.ContactEmail, T.ContactAddress" );
                curSqlStmt.Append( ", T.ChiefJudgeMemberId, TourRegCJ.SkierName AS ChiefJudgeName, T.ChiefJudgeAddress, T.ChiefJudgePhone, T.ChiefJudgeEmail" );
                curSqlStmt.Append( ", T.ChiefDriverMemberId, TourRegCD.SkierName AS ChiefDriverName, T.ChiefDriverAddress, T.ChiefDriverPhone, T.ChiefDriverEmail" );
                curSqlStmt.Append( ", T.SafetyDirMemberId, TourRegCS.SkierName AS ChiefSafetyName, T.SafetyDirAddress, T.SafetyDirPhone, T.SafetyDirEmail" );
                curSqlStmt.Append( ", T.ChiefScorerMemberId, TourRegCC.SkierName AS ChiefScorerName, T.ChiefScorerAddress, T.ChiefScorerPhone, T.ChiefScorerEmail " );
                curSqlStmt.Append( "FROM Tournament T " );
                curSqlStmt.Append( "    LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
                curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCC ON T.SanctionId = TourRegCC.SanctionId AND T.ChiefScorerMemberId = TourRegCC.MemberId " );
                curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCJ ON T.SanctionId = TourRegCJ.SanctionId AND T.ChiefJudgeMemberId = TourRegCJ.MemberId " );
                curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCD ON T.SanctionId = TourRegCD.SanctionId AND T.ChiefDriverMemberId = TourRegCD.MemberId " );
                curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCS ON T.SanctionId = TourRegCS.SanctionId AND T.SafetyDirMemberId = TourRegCS.MemberId " );
                curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCO ON T.SanctionId = TourRegCO.SanctionId AND T.ContactMemberId = TourRegCO.MemberId " );
                curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
                curSqlStmt.Append( "ORDER BY T.SanctionId " );
                
                DataTable curDataTable = getData( curSqlStmt.ToString() );
                if (curDataTable.Rows.Count > 0) {
                    curRow = curDataTable.Rows[0];
                    return curRow;
                } else {
                    return null;
                }
            } catch (Exception ex) {
                MessageBox.Show( "Exception retrieving tournament data " + "\n\nError: " + ex.Message );
                return null;
            }
        }

        private DataRow getJumpMeterSetup() {
            DataRow curRow = null;
            try {
                //Retrieve selected tournament attributes
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select PK, SanctionId" );
                curSqlStmt.Append( ", AngleAto15ET, AngleAtoB, AngleAtoC, AngleAtoZ" );
                curSqlStmt.Append( ", AngleBto15ET, AngleBtoA, AngleBtoC, AngleBtoZ" );
                curSqlStmt.Append( ", AngleCto15ET, AngleCtoA, AngleCtoB, AngleCtoZ" );
                curSqlStmt.Append( ", DistAtoB, DistAtoC, DistBtoC, DistTo15ET" );
                curSqlStmt.Append( ", JumpDirection, Triangle15ET, TriangleMax, TriangleMaxZero, TriangleZero" );
                curSqlStmt.Append( ", XCoord15ET, XCoordZero, YCoord15ET, YCoordZero " );
                curSqlStmt.Append( "FROM JumpMeterSetup " );
                curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                curSqlStmt.Append( "ORDER BY SanctionId " );
                DataTable curDataTable = getData( curSqlStmt.ToString() );
                if (curDataTable.Rows.Count > 0) {
                    curRow = curDataTable.Rows[0];
                    return curRow;
                } else {
                    return null;
                }
            } catch (Exception ex) {
                MessageBox.Show( "Exception retrieving jump meter setup data " + "\n\nError: " + ex.Message );
                return null;
            }
        }

        private DataRow getTourMemberRating(String inMemberId) {
            DataRow curRow = null;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select Distinct 'OW' as EntityName, TR.SanctionId, TR.MemberId, TR.SkierName" );
            curSqlStmt.Append( ", OW.JudgeSlalomRating, OW.JudgeTrickRating, OW.JudgeJumpRating" );
            curSqlStmt.Append( ", OW.DriverSlalomRating, OW.DriverTrickRating, OW.DriverJumpRating" );
            curSqlStmt.Append( ", OW.ScorerSlalomRating, OW.ScorerTrickRating, OW.ScorerJumpRating" );
            curSqlStmt.Append( ", OW.SafetyOfficialRating, OW.TechOfficialRating, OW.AnncrOfficialRating " );
            curSqlStmt.Append( "FROM TourReg TR " );
            curSqlStmt.Append( "INNER JOIN OfficialWork OW ON OW.SanctionId = TR.SanctionId AND OW.MemberId = TR.MemberId " );
            curSqlStmt.Append( "Where TR.SanctionId = '" + mySanctionNum + " ' AND TR.MemberId = '" + inMemberId + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count == 0) {
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select Distinct 'TR' as EntityName, TR.SanctionId, TR.MemberId, TR.SkierName" );
                curSqlStmt.Append( ", JSL.CodeValue AS JudgeSlalomRating, JTL.CodeValue AS JudgeTrickRating, JJL.CodeValue AS JudgeJumpRating" );
                curSqlStmt.Append( ", SSL.CodeValue AS ScorerSlalomRating, STL.CodeValue AS ScorerTrickRating, SJL.CodeValue AS ScorerJumpRating" );
                curSqlStmt.Append( ", DSL.CodeValue AS DriverSlalomRating, DTL.CodeValue AS DriverTrickRating, DJL.CodeValue AS DriverJumpRating" );
                curSqlStmt.Append( ", SL.CodeValue AS SafetyOfficialRating, TL.CodeValue AS TechOfficialRating, AL.CodeValue AS AnncrOfficialRating " );
                curSqlStmt.Append( "FROM TourReg TR " );
                curSqlStmt.Append( "LEFT OUTER JOIN MemberList AS ML ON ML.MemberId = TR.MemberId " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS JSL ON ML.JudgeSlalomRating = JSL.ListCode AND JSL.ListName = 'JudgeRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS JTL ON ML.JudgeTrickRating = JTL.ListCode AND JTL.ListName = 'JudgeRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS JJL ON ML.JudgeJumpRating = JJL.ListCode AND JJL.ListName = 'JudgeRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS SSL ON ML.ScorerSlalomRating = SSL.ListCode AND SSL.ListName = 'ScorerRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS STL ON ML.ScorerTrickRating = STL.ListCode AND STL.ListName = 'ScorerRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS SJL ON ML.ScorerJumpRating = SJL.ListCode AND SJL.ListName = 'ScorerRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS DSL ON ML.DriverSlalomRating = DSL.ListCode AND DSL.ListName = 'DriverRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS DTL ON ML.DriverTrickRating = DTL.ListCode AND DTL.ListName = 'DriverRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS DJL ON ML.DriverJumpRating = DJL.ListCode AND DJL.ListName = 'DriverRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS SL ON ML.SafetyOfficialRating = SL.ListCode AND SL.ListName = 'SafetyRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS TL ON ML.TechOfficialRating = TL.ListCode AND TL.ListName = 'TechRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS AL ON ML.AnncrOfficialRating = AL.ListCode AND AL.ListName = 'AnnouncerRating' " );
                curSqlStmt.Append( "Where TR.SanctionId = '" + mySanctionNum + " ' AND TR.MemberId = '" + inMemberId + "' " );
                curDataTable = getData( curSqlStmt.ToString() );
            }

            if (curDataTable.Rows.Count > 0) {
                curRow = curDataTable.Rows[0];
                return curRow;
            } else {
                return null;
            }
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }


    }
}
