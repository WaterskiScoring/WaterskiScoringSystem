using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tournament {
    public partial class BoatUseReport : Form {
        private String mySanctionNum = null;
        private waterskiDataSet.TourBoatUseDataTable myBoatUseDataTable;

        public BoatUseReport() {
            InitializeComponent();
        }

        private void BoatUseReport_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.BoatUseReport_Width > 0 ) {
                this.Width = Properties.Settings.Default.BoatUseReport_Width;
            }
            if ( Properties.Settings.Default.BoatUseReport_Height > 0 ) {
                this.Height = Properties.Settings.Default.BoatUseReport_Height;
            }
            if ( Properties.Settings.Default.BoatUseReport_Location.X > 0
                && Properties.Settings.Default.BoatUseReport_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.BoatUseReport_Location;
            }

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            RefreshButton_Click( null, null );

        }

        private void BoatUseReport_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.BoatUseReport_Width = this.Size.Width;
                Properties.Settings.Default.BoatUseReport_Height = this.Size.Height;
                Properties.Settings.Default.BoatUseReport_Location = this.Location;
            }
        }

        private void RefreshButton_Click( object sender, EventArgs e ) {
            // Retrieve data from database
            if ( mySanctionNum != null ) {
                Cursor.Current = Cursors.WaitCursor;
                tournamentTableAdapter.FillBy( waterskiDataSet.Tournament, mySanctionNum );
                sanctionIdTextBox.BeginInvoke( (MethodInvoker)delegate() {
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                } );
                if ( tournamentBindingSource.Count > 0 ) {
                    DataRowView curTourRow = (DataRowView)tournamentBindingSource.Current;
                    tourBoatUseTableAdapter.FillByTour( waterskiDataSet.TourBoatUse, mySanctionNum );

                    RegionTextBox.Text = mySanctionNum.Substring( 2, 1 );

                    String curValue = "";
                    if ( curTourRow["ChiefJudgeName"] != System.DBNull.Value ) {
                        curValue = (String)curTourRow["ChiefJudgeName"];
                        int curDelim = curValue.IndexOf( ',' );
                        ChiefJudgeSigTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                        chiefJudgeNameTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                    }
                    if ( curTourRow["ChiefDriverName"] != System.DBNull.Value ) {
                        curValue = (String)curTourRow["ChiefDriverName"];
                        int curDelim = curValue.IndexOf( ',' );
                        ChiefDriverSigTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                        chiefDriverNameTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                    }
                }
            }
        }

        private void PrintButton_Click( object sender, EventArgs e ) {
            foreach ( TabPage curPage in ReportTabControl.TabPages ) {
                curPage.Select();
                curPage.Focus();
                curPage.Show();
            }
            ReportTabControl.TabPages[0].Select();
            ReportTabControl.TabPages[0].Focus();
            ReportTabControl.TabPages[0].Show();

            FormReportPrinter curFormPrint = new FormReportPrinter( ReportTabControl );
            curFormPrint.ReportHeader = "Tow Boat Use and Performance Document" + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            curFormPrint.CenterHeaderOnPage = true;
            curFormPrint.ReportHeaderFont = new Font( "Arial", 12, FontStyle.Bold, GraphicsUnit.Point );
            curFormPrint.ReportHeaderTextColor = Color.Black;

            curFormPrint.BottomMargin = 75;
            curFormPrint.TopMargin = 50;
            curFormPrint.LeftMargin = 50;
            curFormPrint.RightMargin = 50;

            curFormPrint.Print();
        }

    }
}
