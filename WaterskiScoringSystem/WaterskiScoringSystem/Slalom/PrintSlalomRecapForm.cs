using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Slalom {
    class PrintSlalomRecapForm {
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

        private Pen myPen;
        private Brush myBrush;
        private ArrayList myTextBoxLikeControls;
        private ArrayList myPrintDelegates;
        private StringBuilder mytraceLog;
        private PrintDocument myPrintDoc;

        private DataTable myShowDataTable;
        private DataTable myDivInfoDataTable;

        #endregion

        public PrintSlalomRecapForm() {
            myReportName = "PrintSlalomRecapForm";
            myReportHeader = "PrintSlalomRecapForm";
            myReportFooter = "";
            myTourRules = "";
            myTourName = "";
            myReportHeaderFont = new Font( "Tahoma", 10, FontStyle.Bold, GraphicsUnit.Point );
            myReportHeaderTextColor = Color.Black;
            myReportFooterFont = new Font( "Tahoma", 8, FontStyle.Bold, GraphicsUnit.Point );
            myReportFooterTextColor = Color.Black;
            myReportBodyFont = new Font( "Tahoma", 9, FontStyle.Regular, GraphicsUnit.Point );
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

            myTextBoxLikeControls = new ArrayList();
            myPrintDelegates = new ArrayList();
            myPen = new Pen( Color.Black );
            myBrush = Brushes.Black;
        }

        public bool Print() {
            return Print( true );
        }
        public bool Print(bool inShowPreview) {
            bool curReturnValue = true;
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

                    if (inShowPreview) {
                        curPreviewDialog.Document = myPrintDoc;
                        curPreviewDialog.WindowState = FormWindowState.Normal;
                        curPreviewDialog.ShowDialog();
                    } else {
                        myPrintDoc.Print();
                    }
                }

            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in print method of PrintSlalomRecapForm \n Exception: " + ex.ToString() );
                curReturnValue = false;
            }
            return curReturnValue;
        }

        private void PrintReport(object inSender, System.Drawing.Printing.PrintPageEventArgs inPrintEventArgs) {
            int curPageSkierCount = 0, curPageSkierMax = 18;
            Single[] curPagePos;
            Single curTextBoxHeight = Convert.ToSingle(33.5);
            Single curTextBoxWidth = 310;
            bool hasMorePages = false;
            DataRow curRow = null;
            DataRow[] curFindRows;
            myGraphicControl = inPrintEventArgs.Graphics;

            String curText = "", curDiv = "", prevDiv = "", curGroup = "", prevGroup = "";
            StringFormat curTextFormat = new StringFormat();
            curTextFormat.Alignment = StringAlignment.Near;
            curTextFormat.LineAlignment = StringAlignment.Center;

            Single curPosX = myLeftMargin + Convert.ToSingle( 28 );
            Single curPosY = DrawHeader();

            while (curPageSkierCount <= curPageSkierMax && myRowIdx < myShowDataTable.Rows.Count && !hasMorePages) {
                curRow = myShowDataTable.Rows[myRowIdx];
                curDiv = (String)curRow["AgeGroup"];
                if (myTourRules.ToLower().Equals( "ncwsa" ) || myTourRules.ToLower().Equals( "htoh-div" )) {
                    curGroup = (String)curRow["AgeGroup"];
                } else {
                    curGroup = (String)curRow["EventGroup"];
                }
                if (curGroup == prevGroup || curPageSkierCount == 0) {
                    if (curDiv != prevDiv ) {
                        curFindRows = myDivInfoDataTable.Select( "Div = '" + curDiv + "'" );
                        if (curFindRows.Length > 0) {
                            curPageSkierCount++;
                            if (curPageSkierCount >= curPageSkierMax) {
                                hasMorePages = true;
                                break;
                            }
                            if (curFindRows[0]["MinSpeed"] == System.DBNull.Value) {
                                curText = (String)curFindRows[0]["DivName"] + "\nMax: " + (String)curFindRows[0]["MaxSpeed"];
                            } else {
                                curText = (String)curFindRows[0]["DivName"] + "\nMax: " + (String)curFindRows[0]["MaxSpeed"] + " Min: " + (String)curFindRows[0]["MinSpeed"];
                            }
                            curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportBodyTextColor ), curTextFormat );
                            curPosY = curPagePos[1];
                        }
                    }
                    if (myTourRules.ToLower().Equals( "htoh-group" )) {
                        curText = (String)curRow["SkierName"] + " (" + (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"] + ")";
                    } else {
                        curText = (String)curRow["SkierName"] + " (" + (String)curRow["AgeGroup"] + ")";
                    }
                    if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        String curTeam = "";
                        if ( curRow["TeamCode"] != System.DBNull.Value ) {
                            curTeam = (String) curRow["TeamCode"];
                        }
                        if ( curRow["RankingScore"] == System.DBNull.Value ) {
                            curText += "\n  Team: " + curTeam;
                        } else {
                            curText += "\n  Team: " + curTeam + "  Rank: " + ( (Decimal) curRow["RankingScore"] ).ToString("##0.00");
                        }
                    } else {
                        if ( curRow["RankingScore"] == System.DBNull.Value ) {
                            curText += "\n  Class: " + (String) curRow["EventClass"];
                        } else {
                            curText += "\n  Class: " + (String) curRow["EventClass"] + "  Rank: " + ( (Decimal) curRow["RankingScore"] ).ToString("##0.00");
                        }
                    }
                    if ( curRow["ReadyForPlcmt"] != System.DBNull.Value ) {
                        if ( ( (String) curRow["ReadyForPlcmt"] ).Equals("N") ) {
                            curText += " Plcmt: " + (String) curRow["ReadyForPlcmt"];
                        }
                    }
                    curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportBodyFont, new SolidBrush( myReportBodyTextColor ), curTextFormat );
                    curPosY = curPagePos[1];
                    curPageSkierCount++;
                    myRowIdx++;
                    if (curPageSkierCount >= curPageSkierMax) hasMorePages = true;
                } else {
                    hasMorePages = true;
                }
                if (hasMorePages) break;
                prevDiv = curDiv;
                prevGroup = curGroup;
            }

            inPrintEventArgs.HasMorePages = hasMorePages;
            if (myRowIdx >= myShowDataTable.Rows.Count) {
                myRowIdx = 0;
                myPageNumber = 0;
                inPrintEventArgs.HasMorePages = false;
            }
        }

        private void InitPrinting(ref PrintDocument inPrintDoc) {
            mytraceLog = new System.Text.StringBuilder();

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
			Single curPosY = myTopMargin + Convert.ToSingle(33.0);
            Single curPosX = myLeftMargin + Convert.ToSingle( 130 );

			Single curHeaderTitleBufferHeight = Convert.ToSingle(35.0);
			Single curHeaderBufferHeight = Convert.ToSingle( 25.0 );
            Single curTextBoxHeight = Convert.ToSingle( 35.00 );
            Single curTextBoxWidth = 225;
            String curText = "";

            StringFormat TitleFormat = new StringFormat();
            TitleFormat.Alignment = StringAlignment.Near;
            TitleFormat.LineAlignment = StringAlignment.Far;


            Image curImageFile = global::WaterskiScoringSystem.Properties.Resources.SlalomRecapForm;
            myGraphicControl.DrawImage( curImageFile, myLeftMargin, myTopMargin, myPageWidth, myPageHeight );

            // Print the title if available and indicated
            if (myShowHeader) {
                if (myTourName.Length > 30) {
                    int curDelim = myTourName.Substring( 0, 30 ).LastIndexOf( " " );
                    if (curDelim > 0) {
                        curText = myTourName.Substring( 0, curDelim ) + "\n" + myTourName.Substring( curDelim );
                    } else {
                        curText = myTourName.Substring( 0, 30 ) + "\n" + myTourName.Substring( 30 );
                    }
                } else {
                    curText = myTourName;
                }
                Single[] curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportHeaderFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                curPosY = curPagePos[1];
				curPosY += curHeaderTitleBufferHeight;

				if (myReportHeader.Length > 1) {
                    if (myReportHeader.IndexOf( "\\n" ) > 0) {
                        int curDelim = myReportHeader.IndexOf( "\\n" );
                        curText = myReportHeader.Substring( 0, curDelim ) + "\n" + myReportHeader.Substring( curDelim + 2 );
                    } else if (myReportHeader.Length > 38) {
                        int curDelim = myReportHeader.Substring( 0, 38 ).LastIndexOf( " " );
                        if (curDelim > 0) {
                            curText = myReportHeader.Substring( 0, curDelim ) + "\n" + myReportHeader.Substring( curDelim );
                        } else {
                            curText = myReportHeader.Substring( 0, 38 ) + "\n" + myReportHeader.Substring( 38 );
                        }
                    } else {
                        curText = myReportHeader;
                    }

                    TitleFormat.Alignment = StringAlignment.Near;
                    TitleFormat.LineAlignment = StringAlignment.Center;
                    curPosX = myLeftMargin + Convert.ToSingle( 25 );
                    curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportHeaderFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    curPosY = curPagePos[1] + curHeaderBufferHeight;
                } else {
                    curPosY += curTextBoxHeight + curHeaderBufferHeight;
                }
            } else {
                curPosY += curTextBoxHeight + curTextBoxHeight + curHeaderBufferHeight;
            }
            
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
                    Single curFontHeight = FontHeight( new Font( PageStringFont.Name, PageStringFont.Size ) );

                    Single curPosYFooter = myPageHeight + myTopMargin;
                    PrintText( DateString, curPosX, curPosYFooter, 0, 0, PageStringFont, false, HorizontalAlignment.Left );
                    PrintText( PageString, curPosX, curPosYFooter, myPageWidth, 0, 0, 0, PageStringFont, new SolidBrush( myReportFooterTextColor ), BuildFormat( HorizontalAlignment.Center ) );
                    curPosYFooter += curFontHeight;
                }
            }

            return curPosY;
        }

        public Single[] PrintText(String inText, RectangleF inBox, Font inFont, Brush inBrush, StringFormat inFormat) {
            myGraphicControl.DrawString( inText, inFont, inBrush, inBox, inFormat );
            return new Single[2] { inBox.X + inBox.Width, inBox.Y + inBox.Height };
        }
        public Single[] PrintText(String inText, Single inTextLocX, Single inTextLocY, Single inTextWidth, Single inTextHeight, Single inPageAdjX, Single inPageAdjY, Font inFont, Brush inBrush, StringFormat inFormat) {
            Single curPosX = inPageAdjX + inTextLocX;
            Single curPosY = inPageAdjY + inTextLocY;
            SizeF curTextSize = StringSize(inText, inFont );
            Single curBoxHeight = curTextSize.Height;
            if ( curBoxHeight < inTextHeight ) curBoxHeight = inTextHeight;
            Single curBoxWidth = curTextSize.Width;
            if ( curBoxWidth < inTextWidth ) curBoxWidth = inTextWidth;
            RectangleF curBox = new RectangleF( curPosX, curPosY, curBoxWidth, curBoxHeight );
            //myGraphicControl.DrawRectangle( Pens.Red, curPosX, curPosY, curBoxWidth, curBoxHeight ); 
            myGraphicControl.DrawString( inText, inFont, inBrush, curBox, inFormat );
            return new Single[2] { curPosX + curBoxWidth, curPosY + curBoxHeight };
        }
        public Single[] PrintText(String inText, Single inTextLocX, Single inTextLocY, Single inPageAdjX, Single inPageAdjY, Font inFont, bool isVAlignCenter, HorizontalAlignment inHAlign) {
            Single curPosX = inPageAdjX + inTextLocX;
            Single curPosY = inPageAdjY + inTextLocY;
            SizeF curTextSize = myGraphicControl.MeasureString( inText, inFont );
            int curWidth = Convert.ToInt32( curTextSize.Width );
            int curHeight = Convert.ToInt32( curTextSize.Height );

            // Print Text
            if (isVAlignCenter) {
                curPosY += 2;
                curHeight = curHeight + 4;
            }

            PointF curPosition = new PointF( curPosX, curPosY );
            myGraphicControl.DrawString( inText, inFont, myBrush, curPosition );

            return new Single[2] { curPosX + curWidth, curPosY + curHeight };
        }

        private Single FontHeight(Font font) {
            return font.GetHeight( myGraphicControl );
        }
        private SizeF StringSize(String inText, Font inFont ) {
            return myGraphicControl.MeasureString( inText, inFont );
        }

        public StringFormat BuildFormat(HorizontalAlignment hAlignment) {
            StringFormat drawFormat = new StringFormat();
            switch (hAlignment) {
                case HorizontalAlignment.Left:
                    drawFormat.Alignment = StringAlignment.Near;
                    break;
                case HorizontalAlignment.Center:
                    drawFormat.Alignment = StringAlignment.Center;
                    break;
                case HorizontalAlignment.Right:
                    drawFormat.Alignment = StringAlignment.Far;
                    break;
            }
            return drawFormat;
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
