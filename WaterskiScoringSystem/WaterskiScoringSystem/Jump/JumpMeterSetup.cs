using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Tournament;

namespace WaterskiScoringSystem.Jump {
    public partial class JumpMeterSetup : Form {
        #region instance variables
        private Int16 myNumJudges;
        private Boolean isDataModified = false;

        private String mySanctionNum;
        private String myTourClass;

        private Decimal myMeterDistTol;
        private Decimal myMeterZeroTol;
        private Decimal myMeter6Tol;

        private DataRow myTourRow;
        private DataRow mySetupDataRow;
        private DataRow myClassCRow;
        private DataRow myClassERow;
        private DataRow myClassRRow;
        private DataTable mySetupDataTable;

        private ListSkierClass mySkierClassList;
        private JumpCalc myJumpCalc;
        private PrintDocument myPrintDoc;
        #endregion

        public JumpMeterSetup() {
            InitializeComponent();

            myJumpCalc = new JumpCalc();
        }

        private void JumpMeterSetup_Load( object sender, EventArgs e ) {
            if (Properties.Settings.Default.JumpMeterSetup_Width > 0) {
                this.Width = Properties.Settings.Default.JumpMeterSetup_Width;
            }
            if (Properties.Settings.Default.JumpMeterSetup_Height > 0) {
                this.Height = Properties.Settings.Default.JumpMeterSetup_Height;
            }
            if (Properties.Settings.Default.JumpMeterSetup_Location.X > 0
                && Properties.Settings.Default.JumpMeterSetup_Location.Y > 0) {
                this.Location = Properties.Settings.Default.JumpMeterSetup_Location;
            }
            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            Cursor.Current = Cursors.WaitCursor;

            TriangleZeroMsg.Visible = false;
            Triangle15ETMsg.Visible = false;
            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData( );

                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        if ( (byte)myTourRow["JumpRounds"] > 0 ) {
                            myNumJudges = 0;

                            mySkierClassList = new ListSkierClass();
                            mySkierClassList.ListSkierClassLoad();

                            myTourClass = myTourRow["Class"].ToString().ToUpper();
                            myClassCRow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'C'")[0];
                            myClassERow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'E'")[0];

                            jumpDirectionSelectList.SelectList_Load( JumpDirection_Click );
                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue as TriangleFeet FROM CodeValueList " );
                            curSqlStmt.Append( "WHERE ListName in ('JumpMeter6Tol', 'JumpTriangle', 'JumpTriangleZero') " );
                            curSqlStmt.Append( "And ListCode = '" + myTourClass + "' " );
                            curSqlStmt.Append( "ORDER BY SortSeq" );
                            DataTable curTolDataTable = getData( curSqlStmt.ToString() );

                            DataRow myTolRow = curTolDataTable.Select( "ListName = 'JumpTriangle'" )[0];
                            myMeterDistTol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"]);
                            myTolRow = curTolDataTable.Select( "ListName = 'JumpTriangleZero'" )[0];
                            myMeterZeroTol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"]);
                            myTolRow = curTolDataTable.Select( "ListName = 'JumpMeter6Tol'" )[0];
                            myMeter6Tol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"] );

                            navRefresh_Click( null, null );
                        } else {
                            MessageBox.Show( "The jump event is not defined for the active tournament" );
                        }
                    } else {
                        MessageBox.Show( "The active tournament is not properly defined.  You must select from the Administration menu Tournament List option" );
                    }
                }
            }
            Cursor.Current = Cursors.Default;

        }

        private void JumpMeterSetup_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.JumpMeterSetup_Width = this.Size.Width;
                Properties.Settings.Default.JumpMeterSetup_Height = this.Size.Height;
                Properties.Settings.Default.JumpMeterSetup_Location = this.Location;
            }
        }

        private void JumpMeterSetup_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                navSaveItem_Click( null, null );
            }
        }

        private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "Error happened " + e.Context.ToString() + "\n Exception Message: " + e.Exception.Message );
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
            String curMethodName = "JumpMeterSetup:Save";
            int rowsProc = 0;
            try {
                if (isDataModified) {
                    //sanctionIdTextBox.Text = mySanctionNum;
                    StringBuilder curSqlStmt = new StringBuilder( "" );
                    if ( mySetupDataRow == null ) {
                        curSqlStmt.Append( "Insert JumpMeterSetup ( " );
                        curSqlStmt.Append( "SanctionId, JumpDirection, AngleAtoB, AngleBtoA, AngleAtoC, AngleCtoA, AngleBtoC, AngleCtoB, " );
                        curSqlStmt.Append( "DistAtoB, DistBtoC, DistAtoC, " );
                        curSqlStmt.Append( "AngleAtoZ, AngleBtoZ, AngleCtoZ, AngleAto15ET, AngleBto15ET, AngleCto15ET, ");
                        curSqlStmt.Append( "TriangleMaxZero, TriangleZero, XCoordZero, YCoordZero, " );
                        curSqlStmt.Append( "TriangleMax, Triangle15ET, XCoord15ET, YCoord15ET, DistTo15ET " );
                        curSqlStmt.Append( ") Values ( " );
                        curSqlStmt.Append( "'" + sanctionIdTextBox.Text + "'");
                        try {
                            if ( isObjectEmpty( JumpDirectionTextBox.Text ) ) {
                                curSqlStmt.Append( ", " + JumpDirectionTextBox.Text );
                            } else {
                                curSqlStmt.Append( ", 0" );
                            }
                        } catch {
                            curSqlStmt.Append( ", 0" );
                        }
                        try {
                            if ( isObjectEmpty( angleAtoBTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleAtoBTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleBtoATextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleBtoATextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleAtoCTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleAtoCTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleCtoATextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleCtoATextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleBtoCTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleBtoCTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleCtoBTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleCtoBTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( distAtoBTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + distAtoBTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( distBtoCTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + distBtoCTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( distAtoCTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + distAtoCTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleAtoZTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleAtoZTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleBtoZTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleBtoZTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleCtoZTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleCtoZTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleAto15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleAto15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleBto15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleBto15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( angleCto15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + angleCto15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( triangleMaxZeroTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + triangleMaxZeroTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( triangleZeroTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + triangleZeroTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( xCoordZeroTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + xCoordZeroTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( yCoordZeroTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + yCoordZeroTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( triangleMaxTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + triangleMaxTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( triangle15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + triangle15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( xCoord15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + xCoord15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( yCoord15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + yCoord15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        try {
                            if ( isObjectEmpty( distTo15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", null" );
                            } else {
                                curSqlStmt.Append( ", " + distTo15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", null" );
                        }
                        curSqlStmt.Append( ")" );

                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        if (rowsProc > 0) {
                            navRefresh_Click( null, null );
                        }
                    } else {
                        curSqlStmt.Append( "Update JumpMeterSetup Set " );
                        curSqlStmt.Append( " SanctionId = '" + sanctionIdTextBox.Text + "'" );
                        try {
                            if ( isObjectEmpty( JumpDirectionTextBox.Text ) ) {
                                curSqlStmt.Append( ", JumpDirection = 0" );
                            } else {
                                curSqlStmt.Append( ", JumpDirection = " + JumpDirectionTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", JumpDirection = 0" );
                        }
                        try {
                            if ( isObjectEmpty( angleAtoBTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleAtoB = null" );
                            } else {
                                curSqlStmt.Append( ", AngleAtoB = " + angleAtoBTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleAtoB = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleBtoATextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleBtoA = null" );
                            } else {
                                curSqlStmt.Append( ", AngleBtoA = " + angleBtoATextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleBtoA = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleAtoCTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleAtoC = null" );
                            } else {
                                curSqlStmt.Append( ", AngleAtoC = " + angleAtoCTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleAtoC = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleCtoATextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleCtoA = null" );
                            } else {
                                curSqlStmt.Append( ", AngleCtoA = " + angleCtoATextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleCtoA = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleBtoCTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleBtoC = null" );
                            } else {
                                curSqlStmt.Append( ", AngleBtoC = " + angleBtoCTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleBtoC = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleCtoBTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleCtoB = null" );
                            } else {
                                curSqlStmt.Append( ", AngleCtoB = " + angleCtoBTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleCtoB = null" );
                        }
                        try {
                            if ( isObjectEmpty( distAtoBTextBox.Text ) ) {
                                curSqlStmt.Append( ", DistAtoB = null" );
                            } else {
                                curSqlStmt.Append( ", DistAtoB = " + distAtoBTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", DistAtoB = null" );
                        }
                        try {
                            if ( isObjectEmpty( distBtoCTextBox.Text ) ) {
                                curSqlStmt.Append( ", DistBtoC = null" );
                            } else {
                                curSqlStmt.Append( ", DistBtoC = " + distBtoCTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", DistBtoC = null" );
                        }
                        try {
                            if ( isObjectEmpty( distAtoCTextBox.Text ) ) {
                                curSqlStmt.Append( ", DistAtoC = null" );
                            } else {
                                curSqlStmt.Append( ", DistAtoC = " + distAtoCTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", DistAtoC = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleAtoZTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleAtoZ = null" );
                            } else {
                                curSqlStmt.Append( ", AngleAtoZ = " + angleAtoZTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleAtoZ = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleBtoZTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleBtoZ = null" );
                            } else {
                                curSqlStmt.Append( ", AngleBtoZ = " + angleBtoZTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleBtoZ = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleCtoZTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleCtoZ = null" );
                            } else {
                                curSqlStmt.Append( ", AngleCtoZ = " + angleCtoZTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleCtoZ = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleAto15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleAto15ET = null" );
                            } else {
                                curSqlStmt.Append( ", AngleAto15ET = " + angleAto15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleAto15ET = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleBto15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleBto15ET = null" );
                            } else {
                                curSqlStmt.Append( ", AngleBto15ET = " + angleBto15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleBto15ET = null" );
                        }
                        try {
                            if ( isObjectEmpty( angleCto15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", AngleCto15ET = null" );
                            } else {
                                curSqlStmt.Append( ", AngleCto15ET = " + angleCto15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", AngleCto15ET = null" );
                        }
                        try {
                            if ( isObjectEmpty( triangleMaxZeroTextBox.Text ) ) {
                                curSqlStmt.Append( ", TriangleMaxZero = null" );
                            } else {
                                curSqlStmt.Append( ", TriangleMaxZero = " + triangleMaxZeroTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", TriangleMaxZero = null" );
                        }
                        try {
                            if ( isObjectEmpty( triangleZeroTextBox.Text ) ) {
                                curSqlStmt.Append( ", TriangleZero = null" );
                            } else {
                                curSqlStmt.Append( ", TriangleZero = " + triangleZeroTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", TriangleZero = null" );
                        }
                        try {
                            if ( isObjectEmpty( xCoordZeroTextBox.Text ) ) {
                                curSqlStmt.Append( ", XCoordZero = null" );
                            } else {
                                curSqlStmt.Append( ", XCoordZero = " + xCoordZeroTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", XCoordZero = null" );
                        }
                        try {
                            if ( isObjectEmpty( yCoordZeroTextBox.Text ) ) {
                                curSqlStmt.Append( ", YCoordZero = null" );
                            } else {
                                curSqlStmt.Append( ", YCoordZero = " + yCoordZeroTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", YCoordZero = null" );
                        }
                        try {
                            if ( isObjectEmpty( triangleMaxTextBox.Text ) ) {
                                curSqlStmt.Append( ", TriangleMax = null" );
                            } else {
                                curSqlStmt.Append( ", TriangleMax = " + triangleMaxTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", TriangleMax = null" );
                        }
                        try {
                            if ( isObjectEmpty( triangle15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", Triangle15ET = null" );
                            } else {
                                curSqlStmt.Append( ", Triangle15ET = " + triangle15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", Triangle15ET = null" );
                        }
                        try {
                            if ( isObjectEmpty( xCoord15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", XCoord15ET = null" );
                            } else {
                                curSqlStmt.Append( ", XCoord15ET = " + xCoord15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", XCoord15ET = null" );
                        }
                        try {
                            if ( isObjectEmpty( yCoord15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", YCoord15ET = null" );
                            } else {
                                curSqlStmt.Append( ", YCoord15ET = " + yCoord15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", YCoord15ET = null" );
                        }
                        try {
                            if ( isObjectEmpty( distTo15ETTextBox.Text ) ) {
                                curSqlStmt.Append( ", DistTo15ET = null" );
                            } else {
                                curSqlStmt.Append( ", DistTo15ET = " + distTo15ETTextBox.Text );
                            }
                        } catch {
                            curSqlStmt.Append( ", DistTo15ET = null" );
                        }
                        curSqlStmt.Append( " Where PK = " + pKTextBox.Text );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    }
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    
                    isDataModified = false;
                }
            } catch ( Exception excp ) {
                String curMsg = ":Error attempting to save changes \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            winStatusMsg.Text = "Retrieving tournament jump meter setup data";

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM JumpMeterSetup WHERE SanctionId = '" + mySanctionNum + "' " );
            mySetupDataTable = getData( curSqlStmt.ToString() );
            if ( mySetupDataTable.Rows.Count > 0 ) {
                mySetupDataRow = mySetupDataTable.Rows[0];
                pKTextBox.Text = ( (Int64)mySetupDataRow["PK"] ).ToString();
                sanctionIdTextBox.Text = (String)mySetupDataRow["SanctionId"];
                try {
                    jumpDirectionSelectList.CurrentValue = ( (Byte)mySetupDataRow["JumpDirection"] ).ToString();
                } catch {
                    jumpDirectionSelectList.CurrentValue = "0";
                }
                try {
                    angleAtoBTextBox.Text = ( (Decimal)mySetupDataRow["AngleAtoB"] ).ToString( "###.00" );
                } catch {
                    angleAtoBTextBox.Text = "";
                }
                try {
                    angleBtoATextBox.Text = ( (Decimal)mySetupDataRow["AngleBtoA"] ).ToString( "###.00" );
                } catch {
                    angleBtoATextBox.Text = "";
                }
                try {
                    angleAtoCTextBox.Text = ( (Decimal)mySetupDataRow["AngleAtoC"] ).ToString( "###.00" );
                } catch {
                    angleAtoCTextBox.Text = "";
                }
                try {
                    angleCtoATextBox.Text = ( (Decimal)mySetupDataRow["AngleCtoA"] ).ToString( "###.00" );
                } catch {
                    angleCtoATextBox.Text = "";
                }
                try {
                    angleBtoCTextBox.Text = ( (Decimal)mySetupDataRow["AngleBtoC"] ).ToString( "###.00" );
                } catch {
                    angleBtoCTextBox.Text = "";
                }
                try {
                    angleCtoBTextBox.Text = ( (Decimal)mySetupDataRow["AngleCtoB"] ).ToString( "###.00" );
                } catch {
                    angleCtoBTextBox.Text = "";
                }

                try {
                    distAtoBTextBox.Text = ( (Decimal)mySetupDataRow["DistAtoB"] ).ToString( "###.00" );
                } catch {
                    distAtoBTextBox.Text = "";
                }
                try {
                    distBtoCTextBox.Text = ( (Decimal)mySetupDataRow["DistBtoC"] ).ToString( "###.00" );
                } catch {
                    distBtoCTextBox.Text = "";
                }
                try {
                    distAtoCTextBox.Text = ( (Decimal)mySetupDataRow["DistAtoC"] ).ToString( "###.00" );
                } catch {
                    distAtoCTextBox.Text = "";
                }

                try {
                    angleAtoZTextBox.Text = ( (Decimal)mySetupDataRow["AngleAtoZ"] ).ToString( "###.00" );
                } catch {
                    angleAtoZTextBox.Text = "";
                }
                try {
                    angleBtoZTextBox.Text = ( (Decimal)mySetupDataRow["AngleBtoZ"] ).ToString( "###.00" );
                } catch {
                    angleBtoZTextBox.Text = "";
                }
                try {
                    angleCtoZTextBox.Text = ( (Decimal)mySetupDataRow["AngleCtoZ"] ).ToString( "###.00" );
                } catch {
                    angleCtoZTextBox.Text = "";
                }
                try {
                    triangleMaxZeroTextBox.Text = ( (Decimal)mySetupDataRow["TriangleMaxZero"] ).ToString( "0.00##" );
                    triangleMaxZeroTextBox.Text = myMeterZeroTol.ToString( "0.0###" );
                } catch {
                    triangleMaxZeroTextBox.Text = "";
                }
                try {
                    triangleZeroTextBox.Text = ( (Decimal)mySetupDataRow["TriangleZero"] ).ToString( "##.00" );
                } catch {
                    triangleZeroTextBox.Text = "";
                }
                try {
                    xCoordZeroTextBox.Text = ( (Decimal)mySetupDataRow["XCoordZero"] ).ToString( "##.00" );
                } catch {
                    xCoordZeroTextBox.Text = "";
                }
                try {
                    yCoordZeroTextBox.Text = ( (Decimal)mySetupDataRow["YCoordZero"] ).ToString( "###.00" );
                } catch {
                    yCoordZeroTextBox.Text = "";
                }

                try {
                    angleAto15ETTextBox.Text = ( (Decimal)mySetupDataRow["AngleAto15ET"] ).ToString( "###.00" );
                } catch {
                    angleAto15ETTextBox.Text = "";
                }
                try {
                    angleBto15ETTextBox.Text = ( (Decimal)mySetupDataRow["AngleBto15ET"] ).ToString( "###.00" );
                } catch {
                    angleBto15ETTextBox.Text = "";
                }
                try {
                    angleCto15ETTextBox.Text = ( (Decimal)mySetupDataRow["AngleCto15ET"] ).ToString( "###.00" );
                } catch {
                    angleCto15ETTextBox.Text = "";
                }

                try {
                    triangleMaxTextBox.Text = ( (Decimal)mySetupDataRow["TriangleMax"] ).ToString( "0.00##" );
                    triangleMaxTextBox.Text = myMeterDistTol.ToString( "0.0###" );
                } catch {
                    triangleMaxTextBox.Text = "";
                }
                try {
                    triangle15ETTextBox.Text = ( (Decimal)mySetupDataRow["Triangle15ET"] ).ToString( "#0.00##" );
                } catch {
                    triangle15ETTextBox.Text = "";
                }
                try {
                    xCoord15ETTextBox.Text = ( (Decimal)mySetupDataRow["XCoord15ET"] ).ToString( "###.00" );
                } catch {
                    xCoord15ETTextBox.Text = "";
                }
                try {
                    yCoord15ETTextBox.Text = ( (Decimal)mySetupDataRow["YCoord15ET"] ).ToString( "###.00" );
                } catch {
                    yCoord15ETTextBox.Text = "";
                }
                try {
                    distTo15ETTextBox.Text = ( (Decimal)mySetupDataRow["DistTo15ET"] ).ToString( "###.0" );
                } catch {
                    distTo15ETTextBox.Text = "";
                }
                CourseAngleTextBox.Text = "";

                isDataModified = true;
            } else {
                mySetupDataRow = null;

                sanctionIdTextBox.Text = mySanctionNum;
                pKTextBox.Text = "";
                jumpDirectionSelectList.CurrentValue = "1";
                JumpDirectionTextBox.Text = jumpDirectionSelectList.CurrentValue;

                angleAtoBTextBox.Text = "";
                angleBtoATextBox.Text = "";
                angleAtoCTextBox.Text = "";
                angleCtoATextBox.Text = "";
                angleBtoCTextBox.Text = "";
                angleCtoBTextBox.Text = "";
                
                distAtoBTextBox.Text = "";
                distBtoCTextBox.Text = "";
                distAtoCTextBox.Text = "";
                
                angleAtoZTextBox.Text = "";
                angleBtoZTextBox.Text = "";
                angleCtoZTextBox.Text = "";
                angleAto15ETTextBox.Text = "";
                angleBto15ETTextBox.Text = "";
                angleCto15ETTextBox.Text = "";

                triangleMaxZeroTextBox.Text = myMeterZeroTol.ToString( "0.0###" );
                triangleZeroTextBox.Text = "";
                xCoordZeroTextBox.Text = "";
                yCoordZeroTextBox.Text = "";

                triangleMaxTextBox.Text = myMeterDistTol.ToString( "0.0###" );
                triangle15ETTextBox.Text = "";
                xCoord15ETTextBox.Text = "";
                yCoord15ETTextBox.Text = "";
                distTo15ETTextBox.Text = "";
                CourseAngleTextBox.Text = "";
                                
                isDataModified = true;
            }
        }

        private void navExport_Click( object sender, EventArgs e ) {
            if ( isDataModified ) { navSaveItem_Click( null, null ); }

            ExportData myExportData = new ExportData();
            String mySelectCommand = "Select * from JumpMeterSetup ";
            mySelectCommand = mySelectCommand
                + " Where SanctionId = '" + mySanctionNum + "'";
            myExportData.exportData( "JumpMeterSetup", mySelectCommand );
        }

        private void JumpDirection_Click( object sender, EventArgs e ) {
            JumpDirectionTextBox.Text = jumpDirectionSelectList.CurrentValue;
        }

        private void ItemValidating( object sender, CancelEventArgs e ) {
            isDataModified = true;
        }

        private void ValidateButton_Click( object sender, EventArgs e ) {
            bool isAllDataValid = true;
            TriangleZeroMsg.Visible = false;
            Triangle15ETMsg.Visible = false;

            isDataModified = true;
            triangleZeroTextBox.Text = "";
            xCoordZeroTextBox.Text = "";
            yCoordZeroTextBox.Text = "";

            triangle15ETTextBox.Text = "";
            xCoord15ETTextBox.Text = "";
            yCoord15ETTextBox.Text = "";
            distTo15ETTextBox.Text = "";
            CourseAngleTextBox.Text = "";
            
            try {
                myJumpCalc.angleAtoB = Convert.ToDouble( angleAtoBTextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            try {
                myJumpCalc.angleBtoA = Convert.ToDouble( angleBtoATextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            try {
                myJumpCalc.angleAtoC = Convert.ToDouble( angleAtoCTextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            try {
                myJumpCalc.angleCtoA = Convert.ToDouble( angleCtoATextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            try {
                myJumpCalc.angleBtoC = Convert.ToDouble( angleBtoCTextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            try {
                myJumpCalc.angleCtoB = Convert.ToDouble( angleCtoBTextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            try {
                myJumpCalc.distAtoB = Convert.ToDouble( distAtoBTextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            try {
                myJumpCalc.distAtoC = Convert.ToDouble( distAtoCTextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            try {
                myJumpCalc.distBtoC = Convert.ToDouble( distBtoCTextBox.Text );
            } catch {
                isAllDataValid = false;
            }
            if ( isAllDataValid ) {
                navSaveItem_Click( null, null );
                isAllDataValid = myJumpCalc.checkMeterSetup();
                if ( isAllDataValid ) {
                    isAllDataValid = myJumpCalc.calcDistAtoC();
                }
                if ( isAllDataValid ) {
                    //Need to check this calculation
                    //Only seems to work if A to B and B to C distances are equal
                    //isAllDataValid = myJumpCalc.checkMeterConfig();
                }
            }
            if ( isAllDataValid ) {
                Double numAngleAtoJump = 0, numAngleBtoJump = 0, numAngleCtoJump = 0;
                try {
                    numAngleAtoJump = Convert.ToDouble( angleAtoZTextBox.Text );
                } catch {
                    isAllDataValid = false;
                }
                try {
                    numAngleBtoJump = Convert.ToDouble( angleBtoZTextBox.Text );
                } catch {
                    isAllDataValid = false;
                }
                try {
                    numAngleCtoJump = Convert.ToDouble( angleCtoZTextBox.Text );
                } catch {
                    isAllDataValid = false;
                }
                if ( isAllDataValid ) {
                    isAllDataValid = myJumpCalc.calcTriangle( numAngleAtoJump, numAngleBtoJump, numAngleCtoJump );
                    if ( isAllDataValid ) {
                        isDataModified = true;
                        myJumpCalc.TriangleZero = myJumpCalc.TriangleJump;
                        myJumpCalc.xCoordZero = myJumpCalc.xCoordJump;
                        myJumpCalc.yCoordZero = myJumpCalc.yCoordJump;

                        triangleZeroTextBox.Text = myJumpCalc.TriangleZero.ToString("#0.00");
                        xCoordZeroTextBox.Text = myJumpCalc.xCoordZero.ToString( "###.00;-###.00" );
                        yCoordZeroTextBox.Text = myJumpCalc.yCoordZero.ToString( "###.00;-###.00" );

                        if ( myMeterZeroTol < myJumpCalc.TriangleZero ) {
                            TriangleZeroMsg.Visible = true;
                        }
                    } else {
                        TriangleZeroMsg.Visible = true;
                    }
                }
                if ( isAllDataValid ) {
                    Double numAngleA = 0, numAngleB = 0, numAngleC = 0;

                    try {
                        numAngleA = Convert.ToDouble( angleAto15ETTextBox.Text );
                    } catch {
                        isAllDataValid = false;
                    }
                    try {
                        numAngleB = Convert.ToDouble( angleBto15ETTextBox.Text );
                    } catch {
                        isAllDataValid = false;
                    }
                    try {
                        numAngleC = Convert.ToDouble( angleCto15ETTextBox.Text );
                    } catch {
                        isAllDataValid = false;
                    }

                    if ( isAllDataValid ) {
                        //Calculate triangle, coordinates, and distance of 15M timing buoy
                        myJumpCalc.jumpDirection = Convert.ToInt16( jumpDirectionSelectList.CurrentValue );
                        Int32[] calcDistResults = myJumpCalc.calcDistance( numAngleA, numAngleB, numAngleC );
                        Decimal curDistExtd = calcDistResults[3];
                        Decimal curDist = Math.Round( ( curDistExtd / 100 ), 2 );
                        if ( curDist < 0 ) isAllDataValid = false;
                        if ( isAllDataValid ) {
                            myJumpCalc.Triangle15M = myJumpCalc.TriangleJump;
                            myJumpCalc.xCoord15M = myJumpCalc.xCoordJump;
                            myJumpCalc.yCoord15M = myJumpCalc.yCoordJump;

                            triangle15ETTextBox.Text = myJumpCalc.Triangle15M.ToString("##.00");
                            xCoord15ETTextBox.Text = myJumpCalc.xCoord15M.ToString( "###.00;-###.00" );
                            yCoord15ETTextBox.Text = myJumpCalc.yCoord15M.ToString( "###.00;-###.00" );
                            distTo15ETTextBox.Text = curDist.ToString( "####.0" );

                            if ( myMeterZeroTol < myJumpCalc.Triangle15M ) {
                                Triangle15ETMsg.Visible = true;
                            }

                            Double numDist = Convert.ToDouble( curDist );
                            Double numCourseAngle = myJumpCalc.calcCourseAngle( numDist );
                            CourseAngleTextBox.Text = Math.Round( Convert.ToDecimal(numCourseAngle), 1).ToString("##0.0");

                        } else {
                            MessageBox.Show("15 Meter timing bouy coordinates are not valid and are required to validate meter setup"
                                + "\n Coordinates produced a negative distance");
                        }

                        navSaveItem_Click( null, null );
                    } else {
                        MessageBox.Show("15 Meter timing bouy coordinates are required to validate meter setup");
                    }
                }
            }
        }

        private void JumpIncline_Validated( object sender, EventArgs e ) {
            String curValue1 = outOfWaterTextBox.Text.Trim();
            String curValue2 = JumpHeightTextBox.Text.Trim();
            if ( curValue1.Length > 0 && curValue2.Length > 0 ) {
                Double curResults = CalcJumpIncline( curValue1, curValue2 );
                Convert.ToDecimal( curResults.ToString( "0.000" ) );
                JumpInclineTextBox.Text = Math.Round( Convert.ToDecimal( curResults ), 3 ).ToString( "0.000" );
            }
        }

        private Double CalcJumpIncline( String inOutOfWater, String inJumpHeight ) {
            Double curOutOfWater = 0, curJumpHeight = 0, curFeet = 0, curInches = 0;

            try {
                if ( inOutOfWater.Length > 2 ) {
                    int posDelim = inOutOfWater.IndexOf( " " );
                    if ( posDelim > 0 ) {
                        curFeet = Convert.ToDouble( inOutOfWater.Substring( 0, posDelim ) );
                        curInches = Convert.ToDouble( inOutOfWater.Substring( posDelim ) );
                        curOutOfWater = curFeet + ( curInches / 12 );
                    } else {
                        curOutOfWater = Convert.ToDouble( inOutOfWater );
                    }
                } else {
                    curOutOfWater = Convert.ToDouble( inOutOfWater );
                }
            } catch {
                MessageBox.Show( "Out of water value not acceptable for calculations" );
                return 0;
            }

            try {
                if ( inJumpHeight.Length > 2 ) {
                    int posDelim = inJumpHeight.IndexOf( " " );
                    if ( posDelim > 0 ) {
                        curFeet = Convert.ToDouble( inJumpHeight.Substring( 0, posDelim ) );
                        curInches = Convert.ToDouble( inJumpHeight.Substring( posDelim ) );
                        curJumpHeight = curFeet + ( curInches / 12 );
                    } else {
                        curJumpHeight = Convert.ToDouble( inJumpHeight );
                    }
                } else {
                    curJumpHeight = Convert.ToDouble( inJumpHeight );
                }
            } catch {
                MessageBox.Show( "Jump height value not acceptable for calculations" );
                return 0;
            }

            Double curResults = curJumpHeight / curOutOfWater;
            return curResults;
        }

        private void printButton_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            myPrintDoc = new PrintDocument();
            curPrintDialog.Document = myPrintDoc;
            CaptureScreen();

            curPrintDialog.AllowCurrentPage = false;
            curPrintDialog.AllowPrintToFile = false;
            curPrintDialog.AllowSelection = false;
            curPrintDialog.AllowSomePages = false;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;

            if (curPrintDialog.ShowDialog() == DialogResult.OK) {
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintScreen);
                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        private void printDoc_PrintScreen(object sender, PrintPageEventArgs e) {
            int curXPos = 0;
            int curYPos = 25;
            Font fontPrintTitle = new Font("Arial Narrow", 14, FontStyle.Bold);
            Font fontPrintFooter = new Font("Times New Roman", 10);
            String printTitle = Properties.Settings.Default.Mdi_Title + " - " + this.Text;

            //display a title
            curXPos = 100;
            e.Graphics.DrawString(printTitle, fontPrintTitle, Brushes.Black, curXPos, curYPos);

            //display form
            curXPos = 50;
            curYPos += 50;
            e.Graphics.DrawImage(memoryImage, curXPos, curYPos);

            curXPos = 50;
            curYPos = curYPos + memoryImage.Height + 25;
            string DateString = DateTime.Now.ToString();
            e.Graphics.DrawString(DateString, fontPrintFooter, Brushes.Black, curXPos, curYPos);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern long BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
        private Bitmap memoryImage;
        private void CaptureScreen() {
            Graphics mygraphics = this.CreateGraphics();
            Size s = this.Size;
            memoryImage = new Bitmap(s.Width, s.Height, mygraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            IntPtr dc1 = mygraphics.GetHdc();
            IntPtr dc2 = memoryGraphics.GetHdc();
            BitBlt(dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376);
            mygraphics.ReleaseHdc(dc1);
            memoryGraphics.ReleaseHdc(dc2);
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private bool isObjectEmpty( object inObject ) {
            bool curReturnValue = false;
            if ( inObject == null ) {
                curReturnValue = true;
            } else if ( inObject == System.DBNull.Value ) {
                curReturnValue = true;
            } else if ( inObject.ToString().Length > 0 ) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

    }
}
