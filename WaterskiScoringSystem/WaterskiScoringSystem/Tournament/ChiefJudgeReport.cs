using System;
using System.Data;
using System.Drawing;
using System.IO;
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
                        ChiefJudgeAddressTextBox.Text = HelperFunctions.getDataRowColValue( myTourRow, "ChiefJudgeAddress", "" );
                        ChiefJudgeDayPhoneTextBox.Text = HelperFunctions.getDataRowColValue( myTourRow, "ChiefJudgePhone", "" );
                        curValue = (String)myTourRow["ChiefJudgeMemberId"];
                        DataRow curMemberRow = getTourMemberRating( curValue );
                        ChiefJudgeRatingTextBox.Text = HelperFunctions.getDataRowColValue( myTourRow, "JudgeSlalomRating", "Unrated" );
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
                    ExportReportTxt();
                } else {
                    MessageBox.Show( "Report can not be produced because some data is missing or invalid" );
                }
            }
        }

        private void ExportReportTxt() {
            String curMethodName = "ChiefJudgeReport:ExportReportTxt";
            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            if ( !(isRequiredDataAvailable()) ) return;

            String curFilename = (String)myTourRow["SanctionId"] + "CJ" + ".txt";
            String curReportTitle = "Chief Judge Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
			String curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			outBuffer = HelperFunctions.getExportFile( curFileFilter, curFilename );
			if ( outBuffer == null ) return;

			try {
				Log.WriteFile( "Export chief judge data file begin: " + curFilename );

				//Tournament data
				outLine = new StringBuilder( "" );
				outLine.Append( "SanctionNumber" );
				outLine.Append( HelperFunctions.TabChar + "Name" );
				outLine.Append( HelperFunctions.TabChar + "Class" );
				outLine.Append( HelperFunctions.TabChar + "Federation" );
				outLine.Append( HelperFunctions.TabChar + "Rules" );
				outLine.Append( HelperFunctions.TabChar + "EventDates" );
				outLine.Append( HelperFunctions.TabChar + "EventLocation" );

				outLine.Append( HelperFunctions.TabChar + "SlalomRounds" );
				outLine.Append( HelperFunctions.TabChar + "TrickRounds" );
				outLine.Append( HelperFunctions.TabChar + "JumpRounds" );

				outLine.Append( HelperFunctions.TabChar + "ContactMemberId" );
				outLine.Append( HelperFunctions.TabChar + "ContactMember" );
				outLine.Append( HelperFunctions.TabChar + "ContactPhone" );
				outLine.Append( HelperFunctions.TabChar + "ContactEmail" );
				outLine.Append( HelperFunctions.TabChar + "ContactAddress" );

				outLine.Append( HelperFunctions.TabChar + "ChiefJudgeMemberId" );
				outLine.Append( HelperFunctions.TabChar + "ChiefJudgeMember" );
				outLine.Append( HelperFunctions.TabChar + "ChiefJudgeRating" );
				outLine.Append( HelperFunctions.TabChar + "ChiefJudgeAddress" );
				outLine.Append( HelperFunctions.TabChar + "ChiefJudgePhone" );
				outLine.Append( HelperFunctions.TabChar + "ChiefJudgeEmail" );

				outLine.Append( HelperFunctions.TabChar + "ChiefDriverMemberId" );
				outLine.Append( HelperFunctions.TabChar + "ChiefDriverMember" );
				outLine.Append( HelperFunctions.TabChar + "ChiefDriverRating" );
				outLine.Append( HelperFunctions.TabChar + "ChiefDriverAddress" );
				outLine.Append( HelperFunctions.TabChar + "ChiefDriverPhone" );
				outLine.Append( HelperFunctions.TabChar + "ChiefDriverEmail" );

				outLine.Append( HelperFunctions.TabChar + "ChiefScorerMemberId" );
				outLine.Append( HelperFunctions.TabChar + "ChiefScorerMember" );
				outLine.Append( HelperFunctions.TabChar + "ChiefScorerRating" );
				outLine.Append( HelperFunctions.TabChar + "ChiefScorerAddress" );
				outLine.Append( HelperFunctions.TabChar + "ChiefScorerPhone" );
				outLine.Append( HelperFunctions.TabChar + "ChiefScorerEmail" );

				outLine.Append( HelperFunctions.TabChar + "SafetyDirMemberId" );
				outLine.Append( HelperFunctions.TabChar + "SafetyDirMember" );
				outLine.Append( HelperFunctions.TabChar + "SafetyDirRating" );
				outLine.Append( HelperFunctions.TabChar + "SafetyDirAddress" );
				outLine.Append( HelperFunctions.TabChar + "SafetyDirPhone" );
				outLine.Append( HelperFunctions.TabChar + "SafetyDirEmail" );
				//Chief Safety Director's Performance Report
				outLine.Append( HelperFunctions.TabChar + "safetyDirPerfReport" );

				//Rule Exceptions
				outLine.Append( HelperFunctions.TabChar + "RuleExceptQ1" );
				outLine.Append( HelperFunctions.TabChar + "RuleExceptQ2" );
				outLine.Append( HelperFunctions.TabChar + "RuleExceptQ3" );
				outLine.Append( HelperFunctions.TabChar + "RuleExceptQ4" );
				outLine.Append( HelperFunctions.TabChar + "ruleExceptions" );

				//Interpretations of the Rules
				outLine.Append( HelperFunctions.TabChar + "RuleInterQ1" );
				outLine.Append( HelperFunctions.TabChar + "RuleInterQ2" );
				outLine.Append( HelperFunctions.TabChar + "RuleInterQ3" );
				outLine.Append( HelperFunctions.TabChar + "RuleInterQ4" );
				outLine.Append( HelperFunctions.TabChar + "ruleInterpretations" );

				//Technical Report Slalom
				outLine.Append( HelperFunctions.TabChar + "RopeHandle1" );
				outLine.Append( HelperFunctions.TabChar + "RopeHandle2" );
				outLine.Append( HelperFunctions.TabChar + "RopeHandle3" );
				outLine.Append( HelperFunctions.TabChar + "RopeHandle4" );

				outLine.Append( HelperFunctions.TabChar + "Rope1Line2300" );
				outLine.Append( HelperFunctions.TabChar + "Rope1Line1825" );
				outLine.Append( HelperFunctions.TabChar + "Rope1Line1600" );
				outLine.Append( HelperFunctions.TabChar + "Rope1Line1425" );
				outLine.Append( HelperFunctions.TabChar + "Rope1Line1300" );
				outLine.Append( HelperFunctions.TabChar + "Rope1Line1200" );
				outLine.Append( HelperFunctions.TabChar + "Rope1Line1125" );
				outLine.Append( HelperFunctions.TabChar + "Rope1Line1075" );
				outLine.Append( HelperFunctions.TabChar + "Rope1Line1025" );

				outLine.Append( HelperFunctions.TabChar + "Rope2Line2300" );
				outLine.Append( HelperFunctions.TabChar + "Rope2Line1825" );
				outLine.Append( HelperFunctions.TabChar + "Rope2Line1600" );
				outLine.Append( HelperFunctions.TabChar + "Rope2Line1425" );
				outLine.Append( HelperFunctions.TabChar + "Rope2Line1300" );
				outLine.Append( HelperFunctions.TabChar + "Rope2Line1200" );
				outLine.Append( HelperFunctions.TabChar + "Rope2Line1125" );
				outLine.Append( HelperFunctions.TabChar + "Rope2Line1075" );
				outLine.Append( HelperFunctions.TabChar + "Rope2Line1025" );

				outLine.Append( HelperFunctions.TabChar + "Rope3Line2300" );
				outLine.Append( HelperFunctions.TabChar + "Rope3Line1825" );
				outLine.Append( HelperFunctions.TabChar + "Rope3Line1600" );
				outLine.Append( HelperFunctions.TabChar + "Rope3Line1425" );
				outLine.Append( HelperFunctions.TabChar + "Rope3Line1300" );
				outLine.Append( HelperFunctions.TabChar + "Rope3Line1200" );
				outLine.Append( HelperFunctions.TabChar + "Rope3Line1125" );
				outLine.Append( HelperFunctions.TabChar + "Rope3Line1075" );
				outLine.Append( HelperFunctions.TabChar + "Rope3Line1025" );

				//Technical Report Jump
				outLine.Append( HelperFunctions.TabChar + "JumpHandle1" );
				outLine.Append( HelperFunctions.TabChar + "JumpHandle2" );
				outLine.Append( HelperFunctions.TabChar + "JumpHandle3" );
				outLine.Append( HelperFunctions.TabChar + "JumpHandle4" );

				outLine.Append( HelperFunctions.TabChar + "JumpLine1" );
				outLine.Append( HelperFunctions.TabChar + "JumpLine2" );
				outLine.Append( HelperFunctions.TabChar + "JumpLine3" );
				outLine.Append( HelperFunctions.TabChar + "JumpLine4" );

				//Technical Report Jump Meters
				outLine.Append( HelperFunctions.TabChar + "angleAtoB" );
				outLine.Append( HelperFunctions.TabChar + "angleAtoC" );
				outLine.Append( HelperFunctions.TabChar + "angleBtoA" );
				outLine.Append( HelperFunctions.TabChar + "angleBtoC" );
				outLine.Append( HelperFunctions.TabChar + "angleCtoA" );
				outLine.Append( HelperFunctions.TabChar + "angleCtoB" );
				outLine.Append( HelperFunctions.TabChar + "angleAtoZ" );
				outLine.Append( HelperFunctions.TabChar + "angleBtoZ" );
				outLine.Append( HelperFunctions.TabChar + "angleCtoZ" );
				outLine.Append( HelperFunctions.TabChar + "angleAto15ET" );
				outLine.Append( HelperFunctions.TabChar + "angleBto15ET" );
				outLine.Append( HelperFunctions.TabChar + "angleCto15ET" );
				outLine.Append( HelperFunctions.TabChar + "distAtoB" );
				outLine.Append( HelperFunctions.TabChar + "distAtoC" );
				outLine.Append( HelperFunctions.TabChar + "distBtoC" );
				outLine.Append( HelperFunctions.TabChar + "triangle15ET" );
				outLine.Append( HelperFunctions.TabChar + "triangleZero" );

				outLine.Append( HelperFunctions.TabChar + "slalomCourseSpecs" );
				outLine.Append( HelperFunctions.TabChar + "trickCourseSpecs" );
				outLine.Append( HelperFunctions.TabChar + "jumpCourseSpecs" );
				outLine.Append( HelperFunctions.TabChar + "buoySpecs" );

				outBuffer.WriteLine( outLine.ToString() );

				outLine = new StringBuilder( "" );
				outLine.Append( sanctionIdTextBox.Text.Substring( 0, 6 ) );
				outLine.Append( HelperFunctions.TabChar + nameTextBox.Text );
				outLine.Append( HelperFunctions.TabChar + myTourClass );
				outLine.Append( HelperFunctions.TabChar + (String)myTourRow["Federation"] );
				outLine.Append( HelperFunctions.TabChar + (String)myTourRow["Rules"] );
				outLine.Append( HelperFunctions.TabChar + eventDatesTextBox.Text );
				outLine.Append( HelperFunctions.TabChar + eventLocationTextBox.Text );

				outLine.Append( HelperFunctions.TabChar + mySlalomRounds.ToString() );
				outLine.Append( HelperFunctions.TabChar + myTrickRounds.ToString() );
				outLine.Append( HelperFunctions.TabChar + myJumpRounds.ToString() );

				//Tournament Contact
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ContactMemberId", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ContactName", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ContactPhone", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ContactEmail", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ContactAddress", "" ) );

                //Chief Judge
                outLine.Append( HelperFunctions.TabChar + (String)myTourRow["ChiefJudgeMemberId"] );
				outLine.Append( HelperFunctions.TabChar + ChiefJudgeNameTextBox.Text );
				outLine.Append( HelperFunctions.TabChar + ChiefJudgeRatingTextBox.Text );
				outLine.Append( HelperFunctions.TabChar + ChiefJudgeAddressTextBox.Text );
				outLine.Append( HelperFunctions.TabChar + ChiefJudgeDayPhoneTextBox.Text );
				outLine.Append( HelperFunctions.TabChar + (String)myTourRow["ChiefJudgeEmail"] );

				//Chief Driver
				outLine.Append( HelperFunctions.TabChar + (String)myTourRow["ChiefDriverMemberId"] );
				outLine.Append( HelperFunctions.TabChar + (String)myTourRow["ChiefDriverName"] );
				String curValue = (String)myTourRow["ChiefDriverMemberId"];
				DataRow curMemberRow = getTourMemberRating( curValue );
				if ( curMemberRow != null ) {
					if ( curMemberRow["DriverSlalomRating"] == System.DBNull.Value ) {
						curValue = "";
					} else if ( ( (String)curMemberRow["DriverSlalomRating"] ).Equals( "Unrated" ) ) {
					} else {
						curValue = (String)curMemberRow["DriverSlalomRating"];
					}
				}
                outLine.Append( HelperFunctions.TabChar + curValue );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ChiefDriverAddress", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ChiefDriverPhone", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ChiefDriverEmail", "" ) );

                //Chief Scorer
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerMemberId", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerName", "" ) );
				curValue = (String)myTourRow["ChiefScorerMemberId"];
				curMemberRow = getTourMemberRating( curValue );
                curValue = "";
                if ( curMemberRow != null ) {
					if ( curMemberRow["ScorerSlalomRating"] == System.DBNull.Value ) {
					} else if ( ( (String)curMemberRow["ScorerSlalomRating"] ).Equals( "Unrated" ) ) {
					} else {
						curValue = (String)curMemberRow["ScorerSlalomRating"];
					}
                }
                outLine.Append( HelperFunctions.TabChar + curValue );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerAddress", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerPhone", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerEmail", "" ) );

				//Chief Safety Director
				outLine.Append( HelperFunctions.TabChar + (String)myTourRow["SafetyDirMemberId"] );
				outLine.Append( HelperFunctions.TabChar + SafetyDirNameTextBox.Text );
				curValue = (String)myTourRow["SafetyDirMemberId"];
				curMemberRow = getTourMemberRating( curValue );
                curValue = "";
                if ( curMemberRow != null ) {
					if ( curMemberRow["SafetyOfficialRating"] == System.DBNull.Value ) {
					} else if ( ( (String)curMemberRow["SafetyOfficialRating"] ).Equals( "Unrated" ) ) {
					} else {
						curValue = (String)curMemberRow["SafetyOfficialRating"];
					}
				}
				outLine.Append( HelperFunctions.TabChar + curValue );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirAddress", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirPhone", "" ) );
                outLine.Append( HelperFunctions.TabChar + HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirEmail", "" ) );

				//Chief Safety Director's Performance Report
				outLine.Append( HelperFunctions.TabChar + replaceLinefeed( safetyDirPerfReportTextBox.Text ) );

				//Rule Exceptions
				outLine.Append( HelperFunctions.TabChar + RuleExceptQ1TextBox.Text );
				outLine.Append( HelperFunctions.TabChar + RuleExceptQ2TextBox.Text );
				outLine.Append( HelperFunctions.TabChar + RuleExceptQ3GroupBox.Text );
				if ( RuleExceptQ4Yes.Checked ) {
					outLine.Append( HelperFunctions.TabChar + RuleExceptQ4Yes.Text );
				} else if ( RuleExceptQ4No.Checked ) {
					outLine.Append( HelperFunctions.TabChar + RuleExceptQ4No.Text );
				} else if ( RuleExceptQ4NA.Checked ) {
					outLine.Append( HelperFunctions.TabChar + RuleExceptQ4NA.Text );
				} else {
					outLine.Append( HelperFunctions.TabChar + RuleExceptQ4NA.Text );
				}
				outLine.Append( HelperFunctions.TabChar + replaceLinefeed( ruleExceptionsTextBox.Text ) );

				//Interpretations of the Rules
				outLine.Append( HelperFunctions.TabChar + RuleInterQ1TextBox.Text );
				outLine.Append( HelperFunctions.TabChar + RuleInterQ2TextBox.Text );
				outLine.Append( HelperFunctions.TabChar + RuleInterQ3GroupBox.Text );
				if ( RuleInterQ4Yes.Checked ) {
					outLine.Append( HelperFunctions.TabChar + RuleInterQ4Yes.Text );
				} else if ( RuleInterQ4No.Checked ) {
					outLine.Append( HelperFunctions.TabChar + RuleInterQ4No.Text );
				} else if ( RuleInterQ4NA.Checked ) {
					outLine.Append( HelperFunctions.TabChar + RuleInterQ4NA.Text );
				} else {
					outLine.Append( HelperFunctions.TabChar + RuleInterQ4NA.Text );
				}
				outLine.Append( HelperFunctions.TabChar + replaceLinefeed( ruleInterpretationsTextBox.Text ) );

				//Technical Slalom Ropes
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( RopeHandle1TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( RopeHandle2TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( RopeHandle3TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( RopeHandle4TextBox.Text ) );

				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line2300TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line1825TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line1600TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line1425TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line1300TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line1200TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line1125TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line1075TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope1Line1025TextBox.Text ) );

				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line2300TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line1825TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line1600TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line1425TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line1300TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line1200TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line1125TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line1075TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope2Line1025TextBox.Text ) );

				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line2300TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line1825TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line1600TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line1425TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line1300TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line1200TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line1125TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line1075TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( Rope3Line1025TextBox.Text ) );

				//Technical Jump Ropes
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( JumpHandle1TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( JumpHandle2TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( JumpHandle3TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( JumpHandle4TextBox.Text ) );

				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( JumpLine1TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( JumpLine2TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( JumpLine3TextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( JumpLine4TextBox.Text ) );

				//Technical Jump Meters
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleAtoBTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleAtoCTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleBtoATextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleBtoCTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleCtoATextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleCtoBTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleAtoZTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleBtoZTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleCtoZTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleAto15ETTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleBto15ETTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( angleCto15ETTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( distAtoBTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( distAtoCTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( distBtoCTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( triangle15ETTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + encodeNumericAsText( triangleZeroTextBox.Text ) );

				//Course specs
				outLine.Append( HelperFunctions.TabChar + replaceLinefeed( slalomCourseSpecsTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + replaceLinefeed( trickCourseSpecsTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + replaceLinefeed( jumpCourseSpecsTextBox.Text ) );
				outLine.Append( HelperFunctions.TabChar + replaceLinefeed( buoySpecsTextBox.Text ) );

				outBuffer.WriteLine( outLine.ToString() );

				Log.WriteFile( "Export chief judge data file complete: " + curFilename );
			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Failure detected writing Chief Judges Extract file (txt)\n\nError: " + ex.Message );
				String curMsg = curMethodName + ":Exception=" + ex.Message;
				Log.WriteFile( curMsg );

			} finally {
				if ( outBuffer != null ) outBuffer.Close();
			}
		}

        private bool isRequiredDataAvailable() {
            String curValue = HelperFunctions.getDataRowColValue( myTourRow, "ContactMemberId", "" );
            if ( curValue.Length == 0 ) {
                MessageBox.Show( "Primary contact is not available and is required" );
                return false;
            }
            curValue = HelperFunctions.getDataRowColValue( myTourRow, "ChiefJudgeMemberId", "" );
            if ( curValue.Length == 0 ) {
                MessageBox.Show( "Chief Judge is not available and is required" );
                return false;
            }
            curValue = HelperFunctions.getDataRowColValue( myTourRow, "ChiefDriverMemberId", "" );
            if ( curValue.Length == 0 ) {
                MessageBox.Show( "Chief Driver is not available and is required" );
                return false;
            }
            curValue = HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerMemberId", "" );
            if ( curValue.Length == 0 ) {
                MessageBox.Show( "Chief Scorer is not available and is required" );
                return false;
            }
            curValue = HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirMemberId", "" );
            if ( curValue.Length == 0 ) {
                MessageBox.Show( "Safety Director is not available and is required" );
                return false;
            }

            return true;
        }

        public void ExportReportPrintFile() {
            String curMethodName = "ChiefJudgeReport:ExportReportPrintFile";
            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = (String)myTourRow["SanctionId"] + "CJ" + ".prn";
            String curReportTitle = "Chief Judge Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
			String curFileFilter = "prn files (*.prn)|*.prn|All files (*.*)|*.*";
			outBuffer = HelperFunctions.getExportFile( curFileFilter, curFilename );
			if ( outBuffer == null ) return;

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
				outLine.Append( "  Federation: " + (String)myTourRow["Federation"] );
				outLine.Append( "  Rules: " + (String)myTourRow["Rules"] );
				outLine.Append( Environment.NewLine + nameLabel.Text + " " + nameTextBox.Text );
				outLine.Append( Environment.NewLine + eventLocationLabel.Text + " " + eventLocationTextBox.Text );
				outBuffer.WriteLine( outLine.ToString() );

				outLine = new StringBuilder( "" );
				//Chief Judge
				outLine.Append( Environment.NewLine + ChiefJudgeNameLabel.Text + " " + ChiefJudgeNameTextBox.Text );
				outLine.Append( "  " + ChiefJudgeRatingLabel.Text + " " + ChiefJudgeRatingTextBox.Text );
				outLine.Append( Environment.NewLine + ChiefJudgeAddressLabel.Text + " " + ChiefJudgeAddressTextBox.Text );
				outLine.Append( Environment.NewLine + ChiefJudgeDayPhoneLabel.Text + " " + ChiefJudgeDayPhoneTextBox.Text );
				outLine.Append( "  Email: " + (String)myTourRow["ChiefJudgeEmail"] );
				outBuffer.WriteLine( outLine.ToString() );

				outLine = new StringBuilder( "" );
				//Tournament Contact
				outLine.Append( Environment.NewLine + "Tournament contact: " + HelperFunctions.getDataRowColValue(myTourRow, "ContactName", "" ) );
				outLine.Append( Environment.NewLine + "Phone: " + HelperFunctions.getDataRowColValue( myTourRow, "ContactPhone", "" ) );
				outLine.Append( "  Email: " + HelperFunctions.getDataRowColValue( myTourRow, "ContactEmail", "" ) );
				outLine.Append( Environment.NewLine + "Address: " + HelperFunctions.getDataRowColValue( myTourRow, "ContactAddress", "" ) );
				outBuffer.WriteLine( outLine.ToString() );

				outLine = new StringBuilder( "" );
				//Chief Driver
				outLine.Append( Environment.NewLine + "Chief Driver: " + HelperFunctions.getDataRowColValue( myTourRow, "ChiefDriverName", "") );
				String curValue = (String)myTourRow["ChiefDriverMemberId"];
				DataRow curMemberRow = getTourMemberRating( curValue );
                curValue = "";
                if ( curMemberRow != null ) {
					if ( curMemberRow["DriverSlalomRating"] == System.DBNull.Value ) {
						curValue = "";
					} else if ( ( (String)curMemberRow["DriverSlalomRating"] ).Equals( "Unrated" ) ) {
					} else {
						curValue = (String)curMemberRow["DriverSlalomRating"];
					}
				}
				outLine.Append( "  Rating: " + curValue );
				outLine.Append( Environment.NewLine + "Phone: " + HelperFunctions.getDataRowColValue( myTourRow, "ChiefDriverPhone", "") );
				outLine.Append( "  Email: " + HelperFunctions.getDataRowColValue( myTourRow, "ChiefDriverEmail", "" ) );
                outLine.Append( Environment.NewLine + "Address: " + HelperFunctions.getDataRowColValue(myTourRow, "ChiefDriverAddress", ""));

                outBuffer.WriteLine( outLine.ToString() );

				outLine = new StringBuilder( "" );
				//Chief Scorer
				outLine.Append( Environment.NewLine + "Chief Scorer: " + (String)myTourRow["ChiefScorerName"] );
				curValue = (String)myTourRow["ChiefScorerMemberId"];
				curMemberRow = getTourMemberRating( curValue );
				if ( curMemberRow != null ) {
					if ( curMemberRow["ScorerSlalomRating"] == System.DBNull.Value ) {
						curValue = "";
					} else if ( ( (String)curMemberRow["ScorerSlalomRating"] ).Equals( "Unrated" ) ) {
					} else {
						curValue = (String)curMemberRow["ScorerSlalomRating"];
					}
				}
				outLine.Append( "  Rating: " + curValue );
                outLine.Append( Environment.NewLine + "Phone: " + HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerPhone", "" ) );
                outLine.Append( "  Email: " + HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerEmail", "" ) );
                outLine.Append( Environment.NewLine + "Address: " + HelperFunctions.getDataRowColValue( myTourRow, "ChiefScorerAddress", "" ) );
				outBuffer.WriteLine( outLine.ToString() );

				outLine = new StringBuilder( "" );
				//Chief Safety Director
				outLine.Append( Environment.NewLine + "Chief Safety: " + SafetyDirNameTextBox.Text );
				curValue = (String)myTourRow["SafetyDirMemberId"];
				curMemberRow = getTourMemberRating( curValue );
				if ( curMemberRow != null ) {
					if ( curMemberRow["SafetyOfficialRating"] == System.DBNull.Value ) {
						curValue = "";
					} else if ( ( (String)curMemberRow["SafetyOfficialRating"] ).Equals( "Unrated" ) ) {
					} else {
						curValue = (String)curMemberRow["SafetyOfficialRating"];
					}
				}
				outLine.Append( "  Rating: " + curValue );
				outLine.Append( Environment.NewLine + "Phone: " + HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirPhone", "" ) );
                outLine.Append("  Email: " + HelperFunctions.getDataRowColValue(myTourRow, "SafetyDirEmail", ""));
                outLine.Append( Environment.NewLine + "Address: " + HelperFunctions.getDataRowColValue( myTourRow, "SafetyDirAddress", "" ));

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
				if ( RuleExceptQ4Yes.Checked ) {
					outLine.Append( Environment.NewLine + RuleExceptQ4Label.Text + " " + RuleExceptQ4Yes.Text );
				} else if ( RuleExceptQ4No.Checked ) {
					outLine.Append( Environment.NewLine + RuleExceptQ4Label.Text + " " + RuleExceptQ4No.Text );
				} else if ( RuleExceptQ4NA.Checked ) {
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
				if ( RuleInterQ4Yes.Checked ) {
					outLine.Append( Environment.NewLine + RuleInterQ4Label.Text + " " + RuleInterQ4Yes.Text );
				} else if ( RuleInterQ4No.Checked ) {
					outLine.Append( Environment.NewLine + RuleInterQ4Label.Text + " " + RuleInterQ4No.Text );
				} else if ( RuleInterQ4NA.Checked ) {
					outLine.Append( Environment.NewLine + RuleInterQ4Label.Text + " " + RuleInterQ4NA.Text );
				} else {
					outLine.Append( Environment.NewLine + RuleInterQ4Label.Text + " " + RuleInterQ4NA.Text );
				}
				outLine.Append( Environment.NewLine + RuleInterQ5Label.Text + " " + replaceLinefeed( ruleInterpretationsTextBox.Text ) );
				outBuffer.WriteLine( outLine.ToString() );

				outLine = new StringBuilder( "" );
				//Technical Slalom Ropes
				outLine.Append( Environment.NewLine + TechReportSlalomLabel.Text );
				outLine.Append( Environment.NewLine + RopeHandle1Label.Text + " " + RopeHandle1TextBox.Text );
				outLine.Append( "  " + RopeHandle2Label.Text + " " + RopeHandle2TextBox.Text );
				outLine.Append( "  " + RopeHandle3Label.Text + " " + RopeHandle3TextBox.Text );
				outLine.Append( "  " + RopeHandle4Label.Text + " " + RopeHandle4TextBox.Text );

				outLine.Append( Environment.NewLine + Rope1Label.Text );
				outLine.Append( "  23M (long): " + Rope1Line2300TextBox.Text );
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
				outLine.Append( "  14.25M (-28): " + Rope2Line1425TextBox.Text );
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
				outLine.Append( " on " + DateTime.Now.ToString( "MMMM d, yyyy HH:mm:ss" ) );
				outBuffer.WriteLine( outLine.ToString() );

				Log.WriteFile( "Export chief judge data print file complete: " + curFilename );
				MessageBox.Show( "Export chief judge data print file complete:" );

			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Failure detected writing Chief Judges print file (txt)\n\nError: " + ex.Message );
				String curMsg = curMethodName + ":Exception=" + ex.Message;
				Log.WriteFile( curMsg );

			} finally {
				if ( outBuffer != null ) outBuffer.Close();
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
