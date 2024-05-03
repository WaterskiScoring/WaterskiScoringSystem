using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Trick {
    class PrintTrickJudgeForm {
        #region  "Class instance variables"
        private Graphics myGraphicControl;

        private bool myCenterHeaderOnPage;
        private bool myCenterReportOnPage;
        private bool myPrintLandscape;
        private bool myPrintPreview;
        private bool myUsePageNum;
        private bool myShowHeader;
        private bool myShowFooter;

        private int myRowIdx;
        private int myPageNumber;
        private int myTourRounds;
        private int myTourRoundsPrint;
        private int myNumJudges;
        private int myNumJudgesPrint;
        private Single myPageWidth;
        private Single myPageHeight;
        private Single myLeftMargin;
        private Single myTopMargin;
        private Single myRightMargin;
        private Single myBottomMargin;

        private String myReportName;
        private String myReportHeader;
        private String myReportFooter;
        private String myTourRules;
        private String myTourName;

        private Font myReportHeaderFont;
        private Font myReportFooterFont;
        private Font myReportBodyFont;
        private Font myReportSubTitleFont;
        private Color myReportHeaderTextColor;
        private Color myReportFooterTextColor;
        private Color myReportBodyTextColor;

		private Brush myBrush;
        private PrintDocument myPrintDoc;

        private DataTable myShowDataTable;
        private DataTable myDivInfoDataTable;

        #endregion

        public PrintTrickJudgeForm() {
            myReportName = "PrintTrickJudgeForm";
            myReportHeader = "PrintTrickJudgeForm";
            myReportFooter = "";
            myTourRules = "";
            myTourName = "";
            myReportHeaderFont = new Font( "Tahoma", 10, FontStyle.Bold, GraphicsUnit.Point );
            myReportHeaderTextColor = Color.Black;
            myReportFooterFont = new Font( "Tahoma", 8, FontStyle.Bold, GraphicsUnit.Point );
            myReportFooterTextColor = Color.Black;
            myReportBodyFont = new Font( "Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point );
            myReportSubTitleFont = new Font( "Tahoma", 9, FontStyle.Bold, GraphicsUnit.Point );

            myReportBodyTextColor = Color.Black;
            myCenterHeaderOnPage = false;
            myCenterReportOnPage = false;
            myPrintLandscape = false;
            myPrintPreview = true;
            myUsePageNum = true;
            myShowHeader = true;
            myShowFooter = true;

            myPageWidth = 0;
            myPageHeight = 0;
            myLeftMargin = 25;
            myTopMargin = 25;
            myRightMargin = 25;
            myBottomMargin = 35;
            myPageNumber = 0;
            myTourRounds = 1;
            myTourRoundsPrint = 1;
            myNumJudges = 3;
            myNumJudgesPrint = 1;

            myBrush = Brushes.Black;
        }

        public bool Print() {
            return Print( true );
        }
        public bool Print(bool inShowPreview) {
			String curMethodName = "PrintTrickJudgeForm:Print: ";
			try {
                PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
                PrintDialog curPrintDialog = new PrintDialog();

                curPrintDialog.AllowCurrentPage = true;
                curPrintDialog.AllowPrintToFile = true;
                curPrintDialog.AllowSelection = false;
                curPrintDialog.AllowSomePages = true;
                curPrintDialog.PrintToFile = false;
                curPrintDialog.ShowHelp = false;
                curPrintDialog.ShowNetwork = false;
                curPrintDialog.UseEXDialog = true;

                if (curPrintDialog.ShowDialog() == DialogResult.OK) {
                    myPrintDoc = new PrintDocument();
                    myPrintDoc.DocumentName = myReportName;
                    myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                    myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                    InitPrinting( ref myPrintDoc );
                    myPrintDoc.PrintPage += new PrintPageEventHandler( this.PrintReport );

                    if (myTourRounds == 2) myTourRoundsPrint = 1;

                    if (inShowPreview) {
                        curPreviewDialog.Document = myPrintDoc;
                        curPreviewDialog.WindowState = FormWindowState.Normal;
						curPreviewDialog.Size = new System.Drawing.Size( 750, 750 );
                        curPreviewDialog.Focus();
						curPreviewDialog.ShowDialog();
                    } else {
                        myPrintDoc.Print();
                    }
                }

            } catch (Exception ex) {
				String curMsg = String.Format( "{0}Exception encountered printing {1},  Exception: {2}"
					, curMethodName, myReportName, ex.Message );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
				return false;
			}
			return true;
        }

		private void PrintReport( object inSender, System.Drawing.Printing.PrintPageEventArgs inPrintEventArgs ) {
			int curPageSkierCount = 0, curPageSkierMax = 2;
			bool hasMorePages = false;
			myGraphicControl = inPrintEventArgs.Graphics;

			StringFormat curTextFormat = new StringFormat();
			curTextFormat.Alignment = StringAlignment.Near;
			curTextFormat.LineAlignment = StringAlignment.Center;

			Single curPosY = DrawHeader();

			while ( curPageSkierCount <= curPageSkierMax && myRowIdx < myShowDataTable.Rows.Count && !hasMorePages ) {
				PrintReportEntry( myShowDataTable.Rows[myRowIdx], curPageSkierCount, curPosY );
				curPageSkierCount++;

				if ( myTourRounds == 2 ) {
					PrintReportEntry( myShowDataTable.Rows[myRowIdx], curPageSkierCount, curPosY );
					curPageSkierCount++;
				}

				if ( curPageSkierCount >= curPageSkierMax ) hasMorePages = true;
				myRowIdx++;
				if ( hasMorePages ) break;
			}

			if ( myRowIdx >= myShowDataTable.Rows.Count ) {
				myNumJudgesPrint++;
				if ( myNumJudgesPrint > myNumJudges ) {
					myTourRoundsPrint++;
					if ( myTourRoundsPrint > myTourRounds || ( myTourRounds == 2 ) ) {
						hasMorePages = false;
						myRowIdx = 0;
						myPageNumber = 0;
						myTourRoundsPrint = 1;
						myNumJudgesPrint = 1;

					} else {
						hasMorePages = true;
						curPageSkierCount = 0;
						myRowIdx = 0;
						myNumJudgesPrint = 1;
					}

				} else {
					hasMorePages = true;
					curPageSkierCount = 0;
					myRowIdx = 0;
				}

			} else {
				hasMorePages = true;
			}

			inPrintEventArgs.HasMorePages = hasMorePages;
		}

		private void PrintReportEntry( DataRow curRow, int curPageSkierCount, Single curPosY ) {
            Single[] curPagePos;
            
			Single curTextBoxHeight = Convert.ToSingle(30);
            Single curTextBoxWidthName = Convert.ToSingle( 200.0 );
            Single curTextBoxWidthDiv = Convert.ToSingle( 135 );
            DataRow[] curFindRows;

            String curText = "";
            StringFormat curTextFormat = new StringFormat();
            curTextFormat.Alignment = StringAlignment.Near;
            curTextFormat.LineAlignment = StringAlignment.Center;

            Single curFormBoxWidth = myPageWidth / 2;
            Single curPosXName = myLeftMargin;
            Single curPosXDiv = myLeftMargin;

			if ( curPageSkierCount == 0 ) {
				curPosXName += Convert.ToSingle( 83 );
				curPosXDiv += Convert.ToSingle( 340 );
			} else {
				curPosXName += curFormBoxWidth + Convert.ToSingle( 125 );
				curPosXDiv += curFormBoxWidth + Convert.ToSingle( 382 );
			}

			if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
				String curTeam = "";
				if ( curRow["TeamCode"] != System.DBNull.Value ) {
					curTeam = (String)curRow["TeamCode"];
				}
				curText = (String)curRow["SkierName"] + "\nTeam: " + curTeam;
			} else {
				curText = (String)curRow["SkierName"] + "\nClass: " + (String)curRow["EventClass"];
			}
			if ( curRow["ReadyForPlcmt"] == System.DBNull.Value ) {
				curText += " Plcmt: N";
			} else {
				curText += " Plcmt: " + (String)curRow["ReadyForPlcmt"];
			}
			curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, curText, curPosXName, curPosY, curTextBoxWidthName, curTextBoxHeight, 0, 0, myReportBodyFont, new SolidBrush( myReportBodyTextColor ), curTextFormat );

			curFindRows = myDivInfoDataTable.Select( "Div = '" + (String)curRow["AgeGroup"] + "'" );
			if ( curFindRows.Length > 0 ) {
				curText = (String)curFindRows[0]["DivName"];
			} else {
				curText = (String)curRow["AgeGroup"];
			}
			curText += "\nRank: " + ( (Decimal)curRow["RankingScore"] ).ToString( "####0.0" );
			curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, curText, curPosXDiv, curPosY, curTextBoxWidthDiv, curTextBoxHeight, 0, 0, myReportBodyFont, new SolidBrush( myReportBodyTextColor ), curTextFormat );
		}

		private void InitPrinting(ref PrintDocument inPrintDoc) {
            myRowIdx = 0;
            // Calculate Form position for printing
            // Set page area
            if (myTopMargin == 0) {
                myTopMargin = inPrintDoc.DefaultPageSettings.Margins.Top;
            }
            if (myBottomMargin == 0) {
                myBottomMargin = inPrintDoc.DefaultPageSettings.Margins.Bottom;
            }
            if (myLeftMargin == 0) {
                myLeftMargin = inPrintDoc.DefaultPageSettings.Margins.Left;
            }
            if (myRightMargin == 0) {
                myRightMargin = inPrintDoc.DefaultPageSettings.Margins.Right;
            }

            if (myPrintLandscape) {
                inPrintDoc.DefaultPageSettings.Landscape = true;
            } else {
                //Int32 curPrintAreaWidth = inPrintDoc.DefaultPageSettings.Bounds.Width - myRightMargin - myLeftMargin;
                inPrintDoc.DefaultPageSettings.Landscape = false;
            }

            myPageHeight = inPrintDoc.DefaultPageSettings.Bounds.Height - myTopMargin - myBottomMargin;
            myPageWidth = inPrintDoc.DefaultPageSettings.Bounds.Width - myLeftMargin - myRightMargin;
            if (myCenterReportOnPage) {
                Int32 curMarginAdj = Convert.ToInt32( Math.Round( ( Convert.ToDouble( myPageWidth ) / 2 ) ) );
                myLeftMargin -= curMarginAdj;
                myRightMargin -= curMarginAdj;
            }
        }

        // The funtion that print the title, page number, and the header row
        private Single DrawHeader() {
            Single[] curPagePos = null;
            Single curPosY = myTopMargin;
            Single curPosX = myLeftMargin;
            Image curImageFile = global::WaterskiScoringSystem.Properties.Resources.TrickForm;

            myGraphicControl.DrawImage( curImageFile, myLeftMargin, myTopMargin, myPageWidth, myPageHeight );

            // Print the title if available and indicated
            if (myShowHeader) {
                Single curTextBoxHeight = Convert.ToSingle( 9.50 );
                Single curTextBoxWidth = myPageWidth / 2;
                String curText = "";

                StringFormat TitleFormat = new StringFormat();
                TitleFormat.Alignment = StringAlignment.Center;
                TitleFormat.LineAlignment = StringAlignment.Center;

                curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, myTourName, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportHeaderFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                curPosX = curPagePos[0];
                curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, myTourName, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportHeaderFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                curPosY = curPagePos[1];
                curPosX = myLeftMargin;

                if (myReportHeader.Length > 1) {
                    curText = myReportHeader;
                    TitleFormat.Alignment = StringAlignment.Center;
                    TitleFormat.LineAlignment = StringAlignment.Center;
                    curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    curPosX = curPagePos[0];
                    
					curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    curPosY = curPagePos[1];
                }
                if (myTourRounds == 2) {
                    curText = "Round 1 Judge " + myNumJudgesPrint;
                    TitleFormat.Alignment = StringAlignment.Center;
                    TitleFormat.LineAlignment = StringAlignment.Center;
                    curPosX = myLeftMargin;
                    curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    
					curText = "Round 2 Judge " + myNumJudgesPrint;
                    curPosX = curPagePos[0];
                    curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    curPosY = curPagePos[1];
                
				} else {
                    curText = "Round " + myTourRoundsPrint + " Judge " + myNumJudgesPrint;
                    TitleFormat.Alignment = StringAlignment.Center;
                    TitleFormat.LineAlignment = StringAlignment.Center;
                    curPosX = myLeftMargin;
                    curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    
					curPosX = curPagePos[0];
					curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    curPosY = curPagePos[1];
                }
            }

            curPosX = myLeftMargin;
            curPosY = myTopMargin + Convert.ToSingle( 69.0 );
            
            // Printing the page number if required
            if (myShowFooter) {
                if (myUsePageNum) {
                    myPageNumber++;
                    string PageString = "Page " + myPageNumber.ToString();
                    string DateString = DateTime.Now.ToString();

                    StringFormat PageStringFormat = new StringFormat();
                    PageStringFormat.Trimming = StringTrimming.Word;
                    PageStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    PageStringFormat.Alignment = StringAlignment.Far;

                    StringFormat DateStringFormat = new StringFormat();
                    DateStringFormat.Trimming = StringTrimming.Word;
                    DateStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    DateStringFormat.Alignment = StringAlignment.Near;

                    Font PageStringFont = new Font( "Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point );
                    Single curFontHeight = HelperPrintFunctions.FontHeight( myGraphicControl, new Font( PageStringFont.Name, PageStringFont.Size ) );
                    Single curTextBoxWidth = myPageWidth / 2;

                    curPosX = myLeftMargin + Convert.ToSingle( 10 );
                    Single curPosYFooter = myPageHeight + myTopMargin;
					HelperPrintFunctions.PrintText( myGraphicControl, DateString, curPosX, curPosYFooter, 0, 0, PageStringFont, myBrush, false, HorizontalAlignment.Left );
                    curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, PageString, curPosX, curPosYFooter, curTextBoxWidth, 0, 0, 0, PageStringFont, new SolidBrush( myReportFooterTextColor ), HelperPrintFunctions.BuildFormat( HorizontalAlignment.Center ) );

                    curPosX = curPagePos[0];
                    HelperPrintFunctions.PrintText( myGraphicControl, DateString, curPosX, curPosYFooter, 0, 0, PageStringFont, myBrush, false, HorizontalAlignment.Left );
                    curPagePos = HelperPrintFunctions.PrintText( myGraphicControl, PageString, curPosX, curPosYFooter, curTextBoxWidth, 0, 0, 0, PageStringFont, new SolidBrush( myReportFooterTextColor ), HelperPrintFunctions.BuildFormat( HorizontalAlignment.Center ) );
                }
            }

            return curPosY;
        }

        #region  "Getter and Setter for all instance objects"
        
        public bool ShowHeader {
            get { return myShowHeader; }
            set { myShowHeader = value; }
        }
        public bool ShowFooter {
            get { return myShowFooter; }
            set { myShowFooter = value; }
        }
        public bool UsePageNum {
            get { return myUsePageNum; }
            set { myUsePageNum = value; }
        }
        public String ReportName {
            get { return myReportName; }
            set { myReportName = value; }
        }
        public String ReportHeader {
            get { return myReportHeader; }
            set { myReportHeader = value; }
        }
        public String ReportFooter {
            get { return myReportFooter; }
            set { myReportFooter = value; }
        }
        public String TourRules {
            get { return myTourRules; }
            set { myTourRules = value; }
        }
        public String TourName {
            get { return myTourName; }
            set { myTourName = value; }
        }
        public int TourRounds {
            get { return myTourRounds; }
            set { myTourRounds = value; }
        }
        public int NumJudges {
            get { return myNumJudges; }
            set { myNumJudges = value; }
        }
        public Font ReportHeaderFont {
            get { return myReportHeaderFont; }
            set { myReportHeaderFont = value; }
        }
        public Color ReportHeaderTextColor {
            get { return myReportHeaderTextColor; }
            set { myReportHeaderTextColor = value; }
        }
        public bool CenterHeaderOnPage {
            get { return myCenterHeaderOnPage; }
            set { myCenterHeaderOnPage = value; }
        }
        public bool CenterReportOnPage {
            get { return myCenterReportOnPage; }
            set { myCenterReportOnPage = value; }
        }
        public bool PrintLandscape {
            get { return myPrintLandscape; }
            set { myPrintLandscape = value; }
        }
        public bool PrintPreview {
            get { return myPrintPreview; }
            set { myPrintPreview = value; }
        }
        public Single PageWidth {
            get { return myPageWidth; }
            set { myPageWidth = value; }
        }
        public Single PageHeight {
            get { return myPageHeight; }
            set { myPageHeight = value; }
        }
        public Single LeftMargin {
            get { return myLeftMargin; }
            set { myLeftMargin = value; }
        }
        public Single TopMargin {
            get { return myTopMargin; }
            set { myTopMargin = value; }
        }
        public Single RightMargin {
            get { return myRightMargin; }
            set { myRightMargin = value; }
        }
        public Single BottomMargin {
            get { return myBottomMargin; }
            set { myBottomMargin = value; }
        }

        public DataTable ShowDataTable {
            get { return myShowDataTable; }
            set { myShowDataTable = value; }
        }

        public DataTable DivInfoDataTable {
            get { return myDivInfoDataTable; }
            set { myDivInfoDataTable = value; }
        }

        #endregion
    }
}
