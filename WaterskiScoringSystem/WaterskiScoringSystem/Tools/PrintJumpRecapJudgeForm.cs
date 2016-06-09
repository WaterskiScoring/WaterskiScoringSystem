using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    class PrintJumpRecapJudgeForm {
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

        public PrintJumpRecapJudgeForm() {
            myReportName = "PrintJumpRecapJudgeForm";
            myReportHeader = "PrintJumpRecapJudgeForm";
            myReportFooter = "";
            myTourRules = "";
            myTourName = "";
            myReportHeaderFont = new Font( "Tahoma", 12, FontStyle.Bold, GraphicsUnit.Point );
            myReportHeaderTextColor = Color.Black;
            myReportFooterFont = new Font( "Tahoma", 8, FontStyle.Bold, GraphicsUnit.Point );
            myReportFooterTextColor = Color.Black;
            myReportBodyFont = new Font( "Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point );
            myReportSubTitleFont = new Font( "Tahoma", 10, FontStyle.Bold, GraphicsUnit.Point );

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
                MessageBox.Show( "Exception encountered in print method of PrintJumpRecapJudgeForm \n Exception: " + ex.ToString() );
                curReturnValue = false;
            }
            return curReturnValue;
        }

        private void PrintReport(object inSender, System.Drawing.Printing.PrintPageEventArgs inPrintEventArgs) {
            int curPageRowCount = 0, curPageRowMax = 8;
            DataRow[] curFindRows;
            Single[] curPagePos;
            Single curTextBoxHeight = Convert.ToSingle(81.15);
            Single curTextBoxWidth = Convert.ToSingle( 349.0 );
            Single curTextBoxDivWidth = Convert.ToSingle( 22.0 );
            bool hasMorePages = false;
            DataRow curRow = null;
            myGraphicControl = inPrintEventArgs.Graphics;

            String curText = "", curDiv = "", prevDiv = "", curGroup = "", prevGroup = "";
            StringFormat curTextFormat = new StringFormat();
            curTextFormat.Alignment = StringAlignment.Near;
            curTextFormat.LineAlignment = StringAlignment.Center;

            Single curPosX = myLeftMargin + Convert.ToSingle( 6 );
            Single curPosY = DrawHeader();

            while (curPageRowCount <= curPageRowMax && myRowIdx < myShowDataTable.Rows.Count && !hasMorePages) {
                curRow = myShowDataTable.Rows[myRowIdx];
                curDiv = (String)curRow["AgeGroup"];
                if (myTourRules.ToLower().Equals( "ncwsa" ) || myTourRules.ToLower().Equals( "htoh-div" )) {
                    curGroup = (String)curRow["AgeGroup"];
                } else {
                    curGroup = (String)curRow["EventGroup"];
                }
                //if ( (curGroup == prevGroup && curDiv == prevDiv) || curPageRowCount == 0) {
                if (curGroup == prevGroup || curPageRowCount == 0) {
                    if (curDiv != prevDiv) {
                        curFindRows = myDivInfoDataTable.Select( "Div = '" + curDiv + "'" );
                        if (curFindRows.Length > 0) {
                            curPageRowCount++;
                            if (curPageRowCount >= curPageRowMax) {
                                hasMorePages = true;
                                break;
                            }
                            curPosX = myLeftMargin + Convert.ToSingle( 6 );
                            curText = (String)curFindRows[0]["DivName"] + "\nMax Speed: " + (String)curFindRows[0]["MaxSpeed"] + "\nMax Ramp: " + (String)curFindRows[0]["RampHeight"];
                            curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxDivWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), curTextFormat );
                            curPosY = curPagePos[1];
                            if (curPageRowCount >= curPageRowMax) hasMorePages = true;

                        }
                    }
                    curPosX = myLeftMargin + Convert.ToSingle( 6 );
                    if (myTourRules.ToLower().Equals( "htoh-group" )) {
                        curText = (String)curRow["SkierName"] + " (" + (String)curRow["EventGroup"] + ")";
                    } else {
                        curText = (String)curRow["SkierName"];
                    }
                    try {
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            String curTeam = "";
                            if ( curRow["TeamCode"] != System.DBNull.Value ) {
                                curTeam = (String) curRow["TeamCode"];
                            }
                            curText += "\n  Team: " + curTeam
                                + " Rank: " + ( (Decimal) curRow["RankingScore"] ).ToString("##0.00")
                                + " Ramp: " + ( (Decimal) curRow["JumpHeight"] ).ToString("#0.0");
                        } else {
                            curText += "\n  Class: " + (String)curRow["EventClass"]
                                + " Rank: " + ( (Decimal)curRow["RankingScore"] ).ToString( "##0.00" )
                                + " Ramp: " + ( (Decimal)curRow["JumpHeight"] ).ToString( "#0.0" );
                        }
                    } catch {
                    }
                    curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportBodyFont, new SolidBrush( myReportBodyTextColor ), curTextFormat );
                    curPosX = curPagePos[0] + Convert.ToSingle( 5 );
                    curText = (String)curRow["AgeGroup"];
                    curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxDivWidth, curTextBoxHeight, 0, 0, myReportBodyFont, new SolidBrush( myReportBodyTextColor ), curTextFormat );
                    curPosY = curPagePos[1];
                    curPageRowCount++;
                    myRowIdx++;
                    if (curPageRowCount >= curPageRowMax) hasMorePages = true;
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
            Single[] curPagePos = null;
            Single curPosY = myTopMargin;
            Single curPosX = myLeftMargin + Convert.ToSingle( 6 );
            Single curTextBoxHeight = Convert.ToSingle( 16.00 );
            Single curTextBoxWidth = Convert.ToSingle( 600.0 );
            String curText = "";

            StringFormat TitleFormat = new StringFormat();
            TitleFormat.Alignment = StringAlignment.Near;
            TitleFormat.LineAlignment = StringAlignment.Far;

            Image curImageFile = global::WaterskiScoringSystem.Properties.Resources.JumpRecapJudgeForm;
            myGraphicControl.DrawImage( curImageFile, myLeftMargin, myTopMargin, myPageWidth, myPageHeight );

            // Print the title if available and indicated
            //myShowHeader = false;
            if (myShowHeader) {
                curPosY += Convert.ToSingle( 23.0 );
                curPosX = myLeftMargin + Convert.ToSingle( 90 );
                curPagePos = PrintText( myTourName, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                curPosY = curPagePos[1];

                curTextBoxHeight = Convert.ToSingle( 36.00 );
                curTextBoxWidth = Convert.ToSingle( 450.0 );
                if (myReportHeader.Length > 1) {
                    if (myReportHeader.IndexOf( "\\n" ) > 0) {
                        int curDelim = myReportHeader.IndexOf( "\\n" );
                        curText = myReportHeader.Substring( 0, curDelim ) + "\n" + myReportHeader.Substring( curDelim + 2 );
                    } else if (myReportHeader.Length > 300) {
                        int curDelim = myReportHeader.Substring( 0, 300 ).LastIndexOf( " " );
                        if (curDelim > 0) {
                            curText = myReportHeader.Substring( 0, curDelim ) + "\n" + myReportHeader.Substring( curDelim );
                        } else {
                            curText = myReportHeader.Substring( 0, 300 ) + "\n" + myReportHeader.Substring( 300 );
                        }
                    } else {
                        curText = myReportHeader;
                    }

                    TitleFormat.Alignment = StringAlignment.Center;
                    TitleFormat.LineAlignment = StringAlignment.Center;
                    curPosY -= Convert.ToSingle( 5.0 );
                    curPosX = myLeftMargin + Convert.ToSingle( 360.0 );
                    curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportHeaderFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    curPosY = curPagePos[1];
                }

                TitleFormat.Alignment = StringAlignment.Center;
                TitleFormat.LineAlignment = StringAlignment.Center;
                curTextBoxHeight = Convert.ToSingle( 32.00 );
                curTextBoxWidth = Convert.ToSingle( 375.0 );
                curPosX = myLeftMargin + Convert.ToSingle( 360.0 );

                /*
                DataRow[] curFindRows;
                String curDiv = (String)myShowDataTable.Rows[myRowIdx]["AgeGroup"];
                curFindRows = myDivInfoDataTable.Select( "Div = '" + curDiv + "'" );
                if (curFindRows.Length > 0) {
                    curText = (String)curFindRows[0]["DivName"] + "\nMax Speed: " + (String)curFindRows[0]["MaxSpeed"] + "   Max Ramp: " + (String)curFindRows[0]["RampHeight"];
                    curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat );
                    curPosY = curPagePos[1];
                }
                 */
            }

            curPosY = myTopMargin + Convert.ToSingle( 142.5 );
            
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

                    curPosX = myLeftMargin + Convert.ToSingle( 10 );
                    Single curPosYFooter = myPageHeight + myTopMargin;
                    PrintText( DateString, curPosX, curPosYFooter, 0, 0, PageStringFont, false, HorizontalAlignment.Left );
                    PrintText( PageString, curPosX, curPosYFooter, myPageWidth, 0, 0, 0, PageStringFont, new SolidBrush( myReportFooterTextColor ), BuildFormat( HorizontalAlignment.Center ) );
                    curPosYFooter += curFontHeight;
                    //PrintText( myReportFooter, myLeftMargin, curPosYFooter, myPageWidth, 0, 0, 0, PageStringFont, new SolidBrush( myReportFooterTextColor ), BuildFormat( HorizontalAlignment.Center ) );
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
