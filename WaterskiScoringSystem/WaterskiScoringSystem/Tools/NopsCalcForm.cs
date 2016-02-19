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

namespace WaterskiScoringSystem.Tools {

    public partial class NopsCalcForm : Form {
        private Boolean score1Changed;
        private Boolean score2Changed;
        private Boolean score3Changed;
        private int idxSlalom = 0;
        private int idxTrick = 1;
        private int idxJump = 2;
        private int idxOverall = 3;

        private String myFilterCommand;

        private CalcNops appNopsCalc;
        private List<ScoreEntry> ScoreList = new List<ScoreEntry>();
        private PrintDocument myPrintDoc;
        private DataTable myNopsDataTable;

        public NopsCalcForm() {
            InitializeComponent();
            appNopsCalc = CalcNops.Instance;
            appNopsCalc.LoadDataForTour();
            myNopsDataTable = appNopsCalc.NopsDataTable;
            //dataGridView.DataSource = myNopsDataTable;
            //N.PK, SkiYear, Event, AgeGroup, V.SortSeq as SortSeqDiv, N.SortSeq, Base, Adj, RatingOpen, RatingRec, RatingMedian, OverallBase, OverallExp, EventsReqd
            //dataGridView.Columns["PK"].Visible = false;

            ScoreList.Add( new ScoreEntry( "Slalom", 0, "", 0 ) );
            ScoreList.Add(new ScoreEntry("Trick", 0, "", 0));
            ScoreList.Add(new ScoreEntry("Jump", 0, "", 0));
            ScoreList.Add(new ScoreEntry("Overall", 0, "", 0));

            Event1.Text = ScoreList[0].Event + ": ";
            Score1.Text = Convert.ToString(ScoreList[0].Score);
            Nops1.Text = Convert.ToString(ScoreList[0].Nops);

            Event2.Text = ScoreList[1].Event + ": ";
            Score2.Text = Convert.ToString(ScoreList[1].Score);
            Nops2.Text = Convert.ToString(ScoreList[1].Nops);

            Event3.Text = ScoreList[2].Event + ": ";
            Score3.Text = Convert.ToString(ScoreList[2].Score);
            Nops3.Text = Convert.ToString(ScoreList[2].Nops);

            Event4.Text = ScoreList[3].Event + ": ";
            Nops4.Text = Convert.ToString(ScoreList[3].Nops);
        }

        private void NopsCalcForm_Load(object sender, EventArgs e) {
            // Set window size based on saved values
            if (Properties.Settings.Default.NopsCalcForm_Location.X > 0
                && Properties.Settings.Default.NopsCalcForm_Location.Y > 0) {
                this.Location = Properties.Settings.Default.NopsCalcForm_Location;
            }

            // Loads data into the 'waterskiDataSet.ListAgeDivisions' table.
            AgeGroupDropdownList myAgeGroupList = null;
            String curSanctionNum = Properties.Settings.Default.AppSanctionNum;
            DataTable curTourDataTable = getTourData( curSanctionNum );
            if (curTourDataTable.Rows.Count > 0) {
                DataRow curTourRow = curTourDataTable.Rows[0];
                myAgeGroupList = new AgeGroupDropdownList( curTourRow );
            } else {
                myAgeGroupList = new AgeGroupDropdownList( null );
            }

            listAgeDivisionsComboBox.DataSource = myAgeGroupList.DropdownList;

            // Loads data into the 'waterskiDataSet.NopsData' table.
            myFilterCommand = "AgeGroup = '" + listAgeDivisionsComboBox.SelectedValue + "'";
        }

        private void NopsCalcForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.NopsCalcForm_Location = this.Location;
            }
        }

        private void listAgeDivisionsComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (listAgeDivisionsComboBox.SelectedValue != null) {
                String curAgeGroup = listAgeDivisionsComboBox.SelectedValue.ToString();
                String myFilterCommand = "AgeGroup = '" + curAgeGroup + "'";

                dataGridView.Rows.Clear();
                DataGridViewRow curViewRow;
                int curIdx = 0;
                DataRow[] curFindRows = myNopsDataTable.Select( myFilterCommand );
                foreach (DataRow curDataRow in curFindRows) {
                    curIdx = dataGridView.Rows.Add();
                    curViewRow = dataGridView.Rows[curIdx];
                    //N.PK, SkiYear, Event, AgeGroup, V.SortSeq as SortSeqDiv, N.SortSeq, Base, Adj, RatingOpen, RatingRec, RatingMedian, OverallBase, OverallExp, EventsReqd
                    curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                    curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                    curViewRow.Cells["RatingRec"].Value = ( (Decimal)curDataRow["RatingRec"] ).ToString( "####0.00" );
                    curViewRow.Cells["RatingMedian"].Value = ((Decimal)curDataRow["RatingMedian"]).ToString("####0.00");
                    curViewRow.Cells["Base"].Value = ((Decimal)curDataRow["Base"]).ToString("#####0");
                    curViewRow.Cells["Adj"].Value = ((Decimal)curDataRow["Adj"]).ToString("##0.00");
                    curViewRow.Cells["OverallBase"].Value = ((Decimal)curDataRow["OverallBase"]).ToString("##0");
                    curViewRow.Cells["OverallExp"].Value = ((Decimal)curDataRow["OverallExp"]).ToString("#.000");
                    curViewRow.Cells["EventsReqd"].Value = (byte)curDataRow["EventsReqd"];
                }

                if (ScoreList[idxSlalom].Score > 0 || ScoreList[idxTrick].Score > 0 || ScoreList[idxJump].Score > 0) {
                    appNopsCalc.calcNops(listAgeDivisionsComboBox.SelectedValue.ToString(), ScoreList);
                    Nops1.Text = Math.Round(ScoreList[idxSlalom].Nops, 1).ToString();
                    Nops2.Text = Math.Round(ScoreList[idxTrick].Nops, 1).ToString();
                    Nops3.Text = Math.Round(ScoreList[idxJump].Nops, 1).ToString();
                    Nops4.Text = Math.Round(ScoreList[idxOverall].Nops, 1).ToString();

                    if (ScoreList[idxSlalom].Score > 0) {
                        calcSlalomScoreText(ScoreList[idxSlalom].Score, curAgeGroup);
                    }
                }
            }
        }

        private void calcSlalomScoreText(Decimal inScore, String inAgeGroup) {
            Int64 tmpRem;
            Int64 tmpScore = Convert.ToInt32( inScore );
            Int64 tmpSix = 6;
            Int64 curPassNum = Math.DivRem( tmpScore, tmpSix, out tmpRem );
            if ( inScore > ( Convert.ToDecimal( curPassNum * tmpSix ) ) ) {
                curPassNum += 1;
            }
            DataRow curMaxSpeedRow = getSlalomMaxSpeedData( inAgeGroup );
            if ( curMaxSpeedRow != null ) {
                Int16 curMaxSpeed = Convert.ToInt16( (Decimal)curMaxSpeedRow["MaxValue"] );
                String curListName = "SlalomPass" + curMaxSpeed.ToString();
                DataRow curPassRow = getPassData( curListName, Convert.ToInt16(curPassNum) );
                if ( curPassRow != null ) {
                    Decimal curPassScore = inScore - ( ( curPassNum - 1 ) * 6 );
                    String curScoreText = (String)curPassRow["CodeValue"];
                    curScoreText = curScoreText.Replace( "kph,", "kph  (" );
                    curScoreText = curScoreText.Replace( ",", " @ " );
                    SlalomScoreDesc.Text = curPassScore.ToString() + " bouys at " + curScoreText + ")";
                }
            }
        }

        private void Score1_TextChanged( object sender, EventArgs e ) {
            if (listAgeDivisionsComboBox.SelectedValue != null) {
                score1Changed = true;
            }
        }

        private void Score2_TextChanged(object sender, EventArgs e) {
            if (listAgeDivisionsComboBox.SelectedValue != null) {
                score2Changed = true;
            }
        }

        private void Score3_TextChanged(object sender, EventArgs e) {
            if (listAgeDivisionsComboBox.SelectedValue != null) {
                score3Changed = true;
            }
        }

        private void Score1_Validating(object sender, CancelEventArgs e) {
            if ( !(textIsNumeric(Score1.Text)) ) {
                e.Cancel = true;
            }
        }

        private void Score2_Validating(object sender, CancelEventArgs e) {
            if (!(textIsNumeric(Score2.Text))) {
                e.Cancel = true;
            }
        }

        private void Score3_Validating(object sender, CancelEventArgs e) {
            if (!(textIsNumeric(Score3.Text))) {
                e.Cancel = true;
            }
        }

        private Boolean textIsNumeric(String inString) {
            try {
                decimal result = Convert.ToDecimal(inString);
                return true;
            } catch {
                MessageBox.Show("Input field must be numeric");
                return false;
            }
        }

        private void Score1_Leave(object sender, EventArgs e) {
            if (score1Changed) {
                if( !(textIsNumeric( Score1.Text)) ) {
                    Score1.Text = "0";
                }
                ScoreList[idxSlalom].Score = Convert.ToDecimal( Score1.Text );
                appNopsCalc.calcNops( listAgeDivisionsComboBox.SelectedValue.ToString(), ScoreList );
                Nops1.Text = Math.Round( ScoreList[idxSlalom].Nops, 1 ).ToString();
                Nops4.Text = Math.Round( ScoreList[idxOverall].Nops, 1 ).ToString();
                score1Changed = false;
                calcSlalomScoreText( ScoreList[idxSlalom].Score, listAgeDivisionsComboBox.SelectedValue.ToString() );
            }
        }

        private void Score2_Leave(object sender, EventArgs e) {
            if (score2Changed) {
                if(!( textIsNumeric( Score2.Text ) )) {
                    Score2.Text = "0";
                }
                ScoreList[idxTrick].Score = Convert.ToDecimal( Score2.Text );
                appNopsCalc.calcNops( listAgeDivisionsComboBox.SelectedValue.ToString(), ScoreList );
                Nops2.Text = Math.Round( ScoreList[idxTrick].Nops, 1 ).ToString();
                Nops4.Text = Math.Round( ScoreList[idxOverall].Nops, 1 ).ToString();
                score2Changed = false;
            }
        }

        private void Score3_Leave(object sender, EventArgs e) {
            if (score3Changed) {
                if(!( textIsNumeric( Score3.Text ) )) {
                    Score3.Text = "0";
                }
                ScoreList[idxJump].Score = Convert.ToDecimal( Score3.Text );
                appNopsCalc.calcNops( listAgeDivisionsComboBox.SelectedValue.ToString(), ScoreList );
                Nops3.Text = Math.Round( ScoreList[idxJump].Nops, 1 ).ToString();
                Nops4.Text = Math.Round( ScoreList[idxOverall].Nops, 1 ).ToString();
                score3Changed = false;
            }
        }

        private void Score_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            } else if (e.KeyCode == Keys.Escape) {
                e.Handled = true;
            }

        }

        private void printButton_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            myPrintDoc = new PrintDocument();
            curPrintDialog.Document = myPrintDoc;
            CaptureScreen();

            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;

            if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintScreen );
                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 10, 10, 10, 10 );

                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        private void printDoc_PrintScreen( object sender, PrintPageEventArgs e ) {
            int curXPos = 0;
            int curYPos = 25;
            Font fontPrintTitle = new Font( "Arial", 16, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            //display a title
            curXPos = 100;
            e.Graphics.DrawString( myPrintDoc.DocumentName, fontPrintTitle, Brushes.Black, curXPos, curYPos );

            //display form
            curXPos = 40;
            curYPos += 25;
            e.Graphics.DrawImage( memoryImage, curXPos, curYPos );

            curXPos = 25;
            curYPos = curYPos + memoryImage.Height + 25;
            e.Graphics.DrawString( "Date Time", fontPrintFooter, Brushes.Black, curXPos, curYPos );
        }

        [System.Runtime.InteropServices.DllImport( "gdi32.dll" )]
        public static extern long BitBlt( IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop );
        private Bitmap memoryImage;
        private void CaptureScreen() {
            Graphics mygraphics = this.CreateGraphics();
            Size s = this.Size;
            memoryImage = new Bitmap( s.Width, s.Height, mygraphics );
            Graphics memoryGraphics = Graphics.FromImage( memoryImage );
            IntPtr dc1 = mygraphics.GetHdc();
            IntPtr dc2 = memoryGraphics.GetHdc();
            BitBlt( dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376 );
            mygraphics.ReleaseHdc( dc1 );
            memoryGraphics.ReleaseHdc( dc2 );
        }

        private DataRow getSlalomMaxSpeedData( String inAgeGroup ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE (ListName = 'AWSASlalomMax' AND ListCode = '" + inAgeGroup + "')" );
            curSqlStmt.Append( "    OR (ListName = 'NcwsaSlalomMax' AND ListCode = '" + inAgeGroup + "')" );
            curSqlStmt.Append( "    OR (ListName = 'IwwfSlalomMax' AND ListCode = '" + inAgeGroup + "')" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private DataRow getPassData( String inListName, Int16 inPassNum ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCode, ListCodeNum, CodeValue, MinValue, MaxValue, CodeDesc" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = '" + inListName + "' AND ListCodeNum = " + inPassNum.ToString() );
            curSqlStmt.Append( " ORDER BY SortSeq, ListCode" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private DataTable getTourData(String inSanctionId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            return curDataTable;
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private void ScoreTextbox_Enter(object sender, EventArgs e) {
            TextBox curTextBox = (TextBox)sender;
            curTextBox.SelectAll();
        }
    }
}
