/*
 * Prints controls from a windows form as form based report
 * For a multi page form report use a tab control and each tab page will become a report page
 * 
*/
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    class FormReportPrinter {
        #region  "Class instance variables"
        private int myPageIdx; // Instance variable to keep track of current tab page being printed
        private String myReportName;
        private Graphics myGraphicControl;
        private Control myControlToPrint;
        private String myReportHeader;
        private Font myReportHeaderFont;
        private Color myReportHeaderTextColor;
        private bool myCenterHeaderOnPage;
        private bool myCenterReportOnPage;
        private bool myPrintLandscape;
        private bool myPrintPreview;
        private bool myUsePageNum;
        private int myPageNumber;
        private int myPageWidth;
        private int myPageHeight;
        private int myLeftMargin;
        private int myTopMargin;
        private int myRightMargin;
        private int myBottomMargin;

        private Pen myPen;
        private Brush myBrush;
        private ArrayList myTextBoxLikeControls;
        private ArrayList myPrintDelegates;
        private StringBuilder mytraceLog;
        private PrintDocument myPrintDoc;

        private class PrintDelegateControl {
            public string TypeName;
            public ControlPrinting PrintFunction;
        }
	    #endregion

        public FormReportPrinter( Control inControl ) {
            myControlToPrint = inControl;
            myReportName = inControl.Text;
            myReportHeader = "";
            myReportHeaderFont = myControlToPrint.Font;
            myReportHeaderTextColor = myControlToPrint.ForeColor;
            myCenterHeaderOnPage = false;
            myCenterReportOnPage = false;
            myPrintLandscape = false;
            myPrintPreview = true;
            myUsePageNum = true;
            myPageWidth = 0;
            myPageHeight = 0;
            myLeftMargin = 0;
            myTopMargin = 0;
            myRightMargin = 0;
            myBottomMargin = 0;
            myPageIdx = 0;
            myPageNumber = 0;

            myTextBoxLikeControls = new ArrayList();
            myPrintDelegates = new ArrayList();
            myPen = new Pen( Color.Black );
            myBrush = Brushes.Black;

            // add build-in types and functions
            AddTextBoxLikeControl( "ComboBox" );
            AddTextBoxLikeControl( "DateTimePicker" );
            AddTextBoxLikeControl( "DateTimeSlicker" );
            AddTextBoxLikeControl( "NumericUpDown" );

            AddPrintDelegateControl( "TextBox", new ControlPrinting( PrintTextBox ) );
            AddPrintDelegateControl( "System.Windows.Forms.Label", new ControlPrinting( PrintLabel ) );
            AddPrintDelegateControl( "System.Windows.Forms.CheckBox", new ControlPrinting( PrintCheckBox ) );
            AddPrintDelegateControl( "System.Windows.Forms.RadioButton", new ControlPrinting( PrintRadioButton ) );
            AddPrintDelegateControl( "System.Windows.Forms.GroupBox", new ControlPrinting( PrintGroupBox ) );
            AddPrintDelegateControl( "System.Windows.Forms.Panel", new ControlPrinting( PrintPanel ) );
            AddPrintDelegateControl( "System.Windows.Forms.TabControl", new ControlPrinting( PrintTabControl ) );
            AddPrintDelegateControl( "System.Windows.Forms.TabPage", new ControlPrinting( PrintTabPage ) );
            AddPrintDelegateControl( "System.Windows.Forms.PictureBox", new ControlPrinting( PrintPictureBox ) );
            AddPrintDelegateControl( "System.Windows.Forms.ListBox", new ControlPrinting( PrintListBox ) );
            AddPrintDelegateControl( "System.Windows.Forms.CheckedListBox", new ControlPrinting( PrintCheckedListBox ) );
            AddPrintDelegateControl( "System.Windows.Forms.DataGridView", new ControlPrinting( PrintDataGrid ) );
        }

        public void Print() {
            Print( true );
        }
        public void Print(bool inShowPreview) {
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

                if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                    myPrintDoc = new PrintDocument();
                    myPrintDoc.DocumentName = myReportName;
                    InitPrinting( ref myPrintDoc );
                    myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                    myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                    myPrintDoc.PrintPage += new PrintPageEventHandler( this.PrintReport );

                    if ( inShowPreview ) {
                        curPreviewDialog.Document = myPrintDoc;
                        curPreviewDialog.WindowState = FormWindowState.Normal;
                        curPreviewDialog.ShowDialog();
                    } else {
                        myPrintDoc.Print();
                        String curPrintString = myPrintDoc.ToString();
                        if (curPrintString.Length > 0) {
                            MessageBox.Show( curPrintString );
                        }
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in print method of FormReportPrinter"
                + "\n Exception: " + ex.ToString() );
            }
        }

        private void InitPrinting( ref PrintDocument inPrintDoc ) {
            mytraceLog = new System.Text.StringBuilder();

            // Calculate Form position for printing
            // Set page area
            if ( myTopMargin == 0 ) {
                myTopMargin = inPrintDoc.DefaultPageSettings.Margins.Top;
            }
            if ( myBottomMargin == 0 ) {
                myBottomMargin = inPrintDoc.DefaultPageSettings.Margins.Bottom;
            }
            if ( myLeftMargin == 0 ) {
                myLeftMargin = inPrintDoc.DefaultPageSettings.Margins.Left;
            }
            if ( myRightMargin == 0 ) {
                myRightMargin = inPrintDoc.DefaultPageSettings.Margins.Right;
            }

            if ( myPrintLandscape ) {
                inPrintDoc.DefaultPageSettings.Landscape = true;
            } else {
                Int32 curPrintAreaWidth = inPrintDoc.DefaultPageSettings.Bounds.Width - myRightMargin - myLeftMargin;
                if ( myControlToPrint.Width > curPrintAreaWidth ) {
                    myPrintLandscape = true;
                    inPrintDoc.DefaultPageSettings.Landscape = true;
                } else {
                    inPrintDoc.DefaultPageSettings.Landscape = false;
                }
            }

            myPageHeight = inPrintDoc.DefaultPageSettings.Bounds.Height - myTopMargin - myBottomMargin;
            myPageWidth = inPrintDoc.DefaultPageSettings.Bounds.Width - myLeftMargin - myRightMargin;
            if ( myPrintLandscape ) {
                int tempValue = myPageHeight;
                myPageHeight = myPageWidth;
                myPageWidth = tempValue;
            }

            if ( myCenterReportOnPage ) {
                if ( myPageWidth > myControlToPrint.Width ) {
                    Int32 curMarginAdj = Convert.ToInt32(Math.Round( (Convert.ToDouble ( myPageWidth - myControlToPrint.Width ) / 2) ) ) ;
                    myLeftMargin -= curMarginAdj;
                    myRightMargin -= curMarginAdj;
                }
            }

        }

        private void PrintReport( object inSender, System.Drawing.Printing.PrintPageEventArgs inPrintEventArgs ) {
            bool hasMorePages = false;
            myGraphicControl = inPrintEventArgs.Graphics;

            int curX = 0, curY = 0;
            curX = myLeftMargin;
            curY += DrawHeader();

            string topControlType = myControlToPrint.GetType().ToString();
            if ( topControlType.IndexOf( "TabControl" ) > 0 ) {
                TabControl curTabControl = (TabControl)myControlToPrint;
                hasMorePages = PrintControls( curTabControl.TabPages[myPageIdx], curX, curY );
                if ( !(hasMorePages) ) {
                    myPageIdx++;
                    if ( myPageIdx < curTabControl.TabPages.Count ) {
                        hasMorePages = true;
                    } else {
                        myPageIdx = 0;
                        myPageNumber = 0;
                    }
                }
            } else {
                hasMorePages = PrintControls( myControlToPrint, myLeftMargin, curY );
            }

            inPrintEventArgs.HasMorePages = hasMorePages;
        }

		/// <summary>
		/// Print child controls of a "parent" control.
		/// Controls are printed from top to bottom.
		/// This is the function that calculate position of each control depending of the extension of child controls
		/// </summary>
        public bool PrintControls( Control inControlToPrint, int inPageAdjX, int inPageAdjY ) {
			Single curX, curY;

            int curCount = inControlToPrint.Controls.Count;
            Single[] yPosList = new Single[curCount];
            System.Windows.Forms.Control[] controlList = new System.Windows.Forms.Control[curCount];

            //Do a list of child controls
            for ( int curIdx = 0; curIdx < curCount; curIdx++ ) {
                controlList[curIdx] = inControlToPrint.Controls[curIdx];
                yPosList[curIdx] = inControlToPrint.Controls[curIdx].Location.Y;
            }

            //Sort them by vertical position
            System.Array.Sort( yPosList, controlList );

            // *****************************************************************
            // This loop over child controls calculate position of them.
            // Algorithm take care of controls that expand besides and above.
            // It keep an arraylist of original and printed (after expansion) bottom
            // position of expanded control.
            // So control is push down if it was originally below expanded controls
            // *****************************************************************
            for ( int curIdx = 0; curIdx < curCount; curIdx++ ) {
                PrintControl( controlList[curIdx], inPageAdjX, inPageAdjY );
            }
            return false;
        }

        public bool PrintControl( Control inControlToPrint, int inPageAdjX, int inPageAdjY ) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            string topControlType = myControlToPrint.GetType().ToString();

            if ( inControlToPrint.Visible == true || topControlType.IndexOf( "TabControl" ) > 0 ) {
                string curControlType = inControlToPrint.GetType().ToString();
                bool controlFound = false;
                //First check if it's a text box like control
                foreach ( string tmpControlType in myTextBoxLikeControls ) {
                    if ( curControlType.IndexOf( tmpControlType ) >= 0 ) {
                        controlFound = true;
                        Single curFontHeight = FontHeight( new Font( inControlToPrint.Font.Name, inControlToPrint.Font.Size ) );
                        PrintText( inControlToPrint, inPageAdjX, inPageAdjY, isDrawBox, isVAlignCenter, HorizontalAlignment.Left );
                        break;
                    }
                }

                if ( !(controlFound) ) {
                    for ( int curIdx = myPrintDelegates.Count - 1; curIdx >= 0; curIdx-- ) {
                        PrintDelegateControl curDelegate = (PrintDelegateControl)myPrintDelegates[curIdx];
                        if ( curControlType.EndsWith( curDelegate.TypeName ) ) {
                            curDelegate.PrintFunction( inControlToPrint, inPageAdjX, inPageAdjY );
                            if ( inControlToPrint.Controls.Count > 0 ) {
                                int curControlX = inPageAdjX + inControlToPrint.Location.X;
                                int curControlY = inPageAdjY + inControlToPrint.Location.Y;
                                PrintControls( inControlToPrint, curControlX, curControlY );
                            }
                            break;
                        }
                    }
                }
                mytraceLog.Append( curControlType + "  " + inControlToPrint.Name + ":" + inControlToPrint.Text + " X=" + (inControlToPrint.Location.X + inPageAdjX).ToString() + " Y=" + (inControlToPrint.Location.Y + inPageAdjY).ToString() + " H=" + inControlToPrint.Height.ToString() + " W=" + inControlToPrint.Width + System.Environment.NewLine );
            }
            return false;
        }

        // delegate for providing of print function by control type
        public delegate void ControlPrinting( Control inControl, int inPageAdjX, int inPageAdjY);

        #region  "Getter and Setter for all instance objects"
        public String ReportName {
            get { return myReportName; }
            set { myReportName = value; }
        }
        public String ReportHeader {
            get { return myReportHeader; }
            set { myReportHeader = value; }
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
        public int PageWidth {
            get { return myPageWidth; }
            set { myPageWidth = value; }
        }
        public int PageHeight {
            get { return myPageHeight; }
            set { myPageHeight = value; }
        }
        public int LeftMargin {
            get { return myLeftMargin; }
            set { myLeftMargin = value; }
        }
        public int TopMargin {
            get { return myTopMargin; }
            set { myTopMargin = value; }
        }
        public int RightMargin {
            get { return myRightMargin; }
            set { myRightMargin = value; }
        }
        public int BottomMargin {
            get { return myBottomMargin; }
            set { myBottomMargin = value; }
        }
        #endregion

        /// <summary>
        /// Let user add TextBox like control type name
        /// </summary>
        /// <param name="stringType">TextBox like control type name</param>
        public void AddTextBoxLikeControl( string inControlTypeName ) {
            myTextBoxLikeControls.Add( inControlTypeName );
        }

        /// <summary>
        /// Let users provide their own print function for specific a control type
        /// </summary>
        /// <param name="stringType">Control type name</param>
        /// <param name="printFunction">function (must match with FormReportPrinter.ControlPrinting delegate)</param>
        public void AddPrintDelegateControl( string inControlTypeName, ControlPrinting inPrintFunction ) {
            PrintDelegateControl curDelegate = new PrintDelegateControl();
            curDelegate.TypeName = inControlTypeName;
            curDelegate.PrintFunction = inPrintFunction;
            myPrintDelegates.Add( curDelegate );
        }

        #region "Functions to print each type of control"
        /// <summary>
        /// Print single line or multi lines TextBox.
        /// </summary>
        public void PrintTextBox( Control inControl, int inPageAdjX, int inPageAdjY ) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            TextBox curControl = (System.Windows.Forms.TextBox)inControl;
            if ( curControl.BorderStyle.Equals( BorderStyle.None ) ) {
                isDrawBox = false;
            } else {
                isDrawBox = true;
            }

            PrintText( inControl, inPageAdjX, inPageAdjY, isDrawBox, isVAlignCenter, curControl.TextAlign );
        }

        public void PrintLabel( Control inControl, int inPageAdjX, int inPageAdjY ) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            Label curControl = (System.Windows.Forms.Label)inControl;
            string curText = curControl.Text;
            if ( curControl.BorderStyle.Equals( BorderStyle.None ) ) {
                isDrawBox = false;
            } else {
                isDrawBox = true;
            }

            Single curFontHeight = FontHeight( new Font( curControl.Font.Name, curControl.Font.Size ) );
            if ( curControl.Height > curFontHeight )
                curFontHeight = curControl.Height;

            // Convert ContentAlignment (property of labels) to HorizontalAlignment (Left, center, Right)
            HorizontalAlignment curHAlign;
            ContentAlignment curHAlign2 = curControl.TextAlign;
            switch ( curHAlign2 ) {
                case ContentAlignment.BottomLeft:
                    curHAlign = HorizontalAlignment.Left;
                    break;
                case ContentAlignment.TopLeft:
                    curHAlign = HorizontalAlignment.Left;
                    break;
                case ContentAlignment.MiddleLeft:
                    curHAlign = HorizontalAlignment.Left;
                    break;

                case ContentAlignment.BottomCenter:
                    curHAlign = HorizontalAlignment.Center;
                    break;
                case ContentAlignment.TopCenter:
                    curHAlign = HorizontalAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    curHAlign = HorizontalAlignment.Center;
                    break;

                case ContentAlignment.BottomRight:
                    curHAlign = HorizontalAlignment.Right;
                    break;
                case ContentAlignment.TopRight:
                    curHAlign = HorizontalAlignment.Right;
                    break;
                case ContentAlignment.MiddleRight:
                    curHAlign = HorizontalAlignment.Right;
                    break;
                default:
                    curHAlign = HorizontalAlignment.Left;
                    break;
            }
            PrintText( inControl, inPageAdjX, inPageAdjY, isDrawBox, isVAlignCenter, curHAlign );
        }

        public void PrintCheckBox( Control inControl, int inPageAdjX, int inPageAdjY ) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            CheckBox curControl = (System.Windows.Forms.CheckBox)inControl;
            string curText = curControl.Text;

            Single curFontHeight = FontHeight( new Font( curControl.Font.Name, curControl.Font.Size ) );
            if ( curControl.Height > curFontHeight )
                curFontHeight = curControl.Height;

            int curX = inPageAdjX + curControl.Location.X;
            int curY = inPageAdjY + curControl.Location.Y;
            DrawRectangle( myPen, curX, curY, curFontHeight, curFontHeight );
            if ( curControl.Checked ) {
                Single d = 3;
                DrawLines( myPen, curX + d, curY + d, curX + curFontHeight - d, curY + curFontHeight - d );
                PointF[] points2 = new PointF[] { new PointF( curX + curFontHeight - d, curY + d ), new PointF( curX + d, curY + curFontHeight - d ) };
                DrawLines( myPen, curX + curFontHeight - d, curY + d, curX + d, curY + curFontHeight - d );
            }
            int curPageAdjX = inPageAdjX + Convert.ToInt32( (Single)curFontHeight + 2 );
            PrintText( inControl, curPageAdjX, inPageAdjY, isDrawBox, isVAlignCenter, HorizontalAlignment.Left );
        }

        public void PrintRadioButton( Control inControl, int inPageAdjX, int inPageAdjY ) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            RadioButton curControl = (System.Windows.Forms.RadioButton)inControl;
            string curText = curControl.Text;

            Single curFontHeight = FontHeight( new Font( curControl.Font.Name, curControl.Font.Size ) );
            if ( curControl.Height > curFontHeight )
                curFontHeight = curControl.Height;

            int curX = inPageAdjX + curControl.Location.X;
            int curY = inPageAdjY + curControl.Location.Y;
            DrawEllipse( myPen, curX, curY, curFontHeight, curFontHeight );
            if ( curControl.Checked ) {
                Single d = 3;
                FillEllipse( myBrush, curX + d, curY + d, curFontHeight - d - d, curFontHeight - d - d );
            }

            int curPageAdjX = inPageAdjX + Convert.ToInt32( (Single)curFontHeight + 2 );
            PrintText( inControl, curPageAdjX, inPageAdjY, isDrawBox, isVAlignCenter, HorizontalAlignment.Left );
        }

        public void PrintPanel( Control inControl, int inPageAdjX, int inPageAdjY ) {
        }

        public void PrintGroupBox( Control inControl, int inPageAdjX, int inPageAdjY ) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            GroupBox curControl = (System.Windows.Forms.GroupBox)inControl;
            string curText = curControl.Text;

            Single curFontHeight = FontHeight( new Font( curControl.Font.Name, curControl.Font.Size ) );
            if ( curControl.Height > curFontHeight )
                curFontHeight = curControl.Height;

            int curX = inPageAdjX + curControl.Location.X;
            int curY = inPageAdjY + curControl.Location.Y;
            DrawRectangle( myPen, curX, curY, inControl.Width, inControl.Height );
        }

        public void PrintTabControl( Control inControl, int inPageAdjX, int inPageAdjY ) {
        }

        public void PrintTabPage( Control inControl, int inPageAdjX, int inPageAdjY ) {
        }

        public void PrintPictureBox( Control inControl, int inPageAdjX, int inPageAdjY ) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            PictureBox curControl = (System.Windows.Forms.PictureBox)inControl;
            string curText = curControl.Text;
            if ( curControl.BorderStyle.Equals( BorderStyle.None ) ) { isDrawBox = true; }

            Single curFontHeight = FontHeight( new Font( curControl.Font.Name, curControl.Font.Size ) );
            if ( curControl.Height > curFontHeight )
                curFontHeight = curControl.Height;

            int curX = inPageAdjX + curControl.Location.X;
            int curY = inPageAdjY + curControl.Location.Y;

            DrawImage( curControl.Image, curX, curY, inControl.Width, inControl.Height );
        }

        public void PrintListBox( Control inControl, int inPageAdjX, int inPageAdjY ) {
        }

        public void PrintCheckedListBox( Control inControl, int inPageAdjX, int inPageAdjY ) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            CheckedListBox curControl = (System.Windows.Forms.CheckedListBox)inControl;
            int curX = inPageAdjX + curControl.Location.X;
            int curY = inPageAdjY + curControl.Location.Y;
            int curColWidth = curControl.ColumnWidth;
            int curWidth = curColWidth;
            string curText = curControl.Text;
            foreach ( String itemText in curControl.Items ) {
                Single curFontHeight = FontHeight( new Font( curControl.Font.Name, curControl.Font.Size ) );
                //if ( curControl.Height > curFontHeight )
                    //curFontHeight = curControl.Height;

                DrawRectangle( myPen, curX, curY, curFontHeight, curFontHeight );
                if ( curText.Equals(itemText) ) {
                    Single d = 3;
                    DrawLines( myPen, curX + d, curY + d, curX + curFontHeight - d, curY + curFontHeight - d );
                    PointF[] points2 = new PointF[] { new PointF( curX + curFontHeight - d, curY + d ), new PointF( curX + d, curY + curFontHeight - d ) };
                    DrawLines( myPen, curX + curFontHeight - d, curY + d, curX + d, curY + curFontHeight - d );
                }
                curWidth = curColWidth - Convert.ToInt32( curFontHeight );
                curX = curX + Convert.ToInt32( curFontHeight );
                DrawString( itemText, curControl.Font, myBrush, curX, curY, curWidth, curControl.Height, BuildFormat( HorizontalAlignment.Left ) );

                curX += curWidth;
            }

        }

        public void PrintDataGrid( Control inControl, int inPageAdjX, int inPageAdjY ) {
            DataGridView curControl = (System.Windows.Forms.DataGridView)inControl;
            bool curCenterOnPage = false;
            int curPageHeight = myPageHeight;
            int curPageWidth = myPageWidth;
            int curTopMargin = curControl.Location.Y + inPageAdjY;
            int curBottomMargin = myBottomMargin;
            int curLeftMargin = myLeftMargin;
            int curRightMargin = myRightMargin;

            /*
            DataGridViewPrinter myPrintDataGrid = new DataGridViewPrinter( curControl, 
                curCenterOnPage, curPageHeight, curPageWidth, 
                curTopMargin, curBottomMargin, curLeftMargin, curRightMargin
            );
             */
            DataGridViewPrinter myPrintDataGrid = new DataGridViewPrinter( curControl, myPrintDoc, 
                curCenterOnPage, curPageHeight, curPageWidth, 
                curTopMargin, curBottomMargin, curLeftMargin, curRightMargin
            );

            myPrintDataGrid.DrawDataGridView( myGraphicControl );

        }

        // The funtion that print the title, page number, and the header row
        private int DrawHeader( ) {
            Single curFontHeight;
            int curY = myTopMargin;

            // Printing the page number if required
            if ( myUsePageNum ) {
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
                curFontHeight = FontHeight( new Font( PageStringFont.Name, PageStringFont.Size ) );
                RectangleF PageStringRectangle = new RectangleF( (float)myLeftMargin, curY, (float)myPageWidth, curFontHeight );
                RectangleF DateStringRectangle = new RectangleF( (float)myLeftMargin, curY, (float)myPageWidth, curFontHeight );
                DrawString( PageString, PageStringFont, new SolidBrush( Color.Black ), PageStringRectangle, PageStringFormat );
                DrawString( DateString, PageStringFont, new SolidBrush( Color.Black ), DateStringRectangle, DateStringFormat );

                curY += (int)curFontHeight;
            }

            
            // Printing the title if available
            if ( myReportHeader.Length > 1 ) {
                curFontHeight = FontHeight( new Font( myReportHeaderFont.Name, myReportHeaderFont.Size ) );

                StringFormat TitleFormat = new StringFormat();
                TitleFormat.Trimming = StringTrimming.Word;
                TitleFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                if ( myCenterHeaderOnPage )
                    TitleFormat.Alignment = StringAlignment.Center;
                else
                    TitleFormat.Alignment = StringAlignment.Near;

                RectangleF TitleRectangle = new RectangleF( (float)myLeftMargin, curY, (float)myPageWidth, curFontHeight );
                DrawString( myReportHeader, myReportHeaderFont, new SolidBrush( myReportHeaderTextColor ), TitleRectangle, TitleFormat );

                curY += (int)curFontHeight;
            }

            DrawRectangle( myPen, myLeftMargin, curY, myPageWidth, myPageHeight - curY + TopMargin);

            return curY;
        }

        #endregion

        #region "Text printing"
        /// <summary>
        /// Print a single line text for many controls. Do some formatting
        /// </summary>
        public void PrintText( Control inControl, int inPageAdjX, int inPageAdjY, bool isDrawBox, bool isVAlignCenter, HorizontalAlignment inHAlign ) {
            int curX = inPageAdjX + inControl.Location.X;
            int curY = inPageAdjY + inControl.Location.Y;

            // Draw border if required
            if ( isDrawBox ) {
                DrawRectangle( myPen, curX, curY, inControl.Width, inControl.Height );
            }
            
            // Print Text
            if ( isVAlignCenter ) {
                Single fontHeight = inControl.Font.GetHeight( myGraphicControl );
                Single deltaHeight = ( inControl.Height - fontHeight ) / 2;
                curY += Convert.ToInt32( deltaHeight ); 
            }

            DrawString( inControl.Text, inControl.Font, myBrush, curX, curY, inControl.Width, inControl.Height, BuildFormat( inHAlign ) );
        }

        public StringFormat BuildFormat( HorizontalAlignment hAlignment ) {
            StringFormat drawFormat = new StringFormat();
            switch ( hAlignment ) {
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

        public string TrimBlankLines( string s ) {
            if ( s == null )
                return s;
            else {
                for ( int i = s.Length; i == 1; i-- )
                    if ( ( s[i].ToString() != Keys.Enter.ToString() ) && ( s[i].ToString() != Keys.LineFeed.ToString() ) && ( s[i].ToString() != " " ) )
                        return s.Substring( 0, i );
                return s;
            }
        }
        #endregion

        #region "General object print functions"
        public Single FontHeight( Font font ) {
            return font.GetHeight( myGraphicControl );
        }

        public void DrawLines( Pen pen, Single x1, Single y1, Single x2, Single y2 ) {
            Single y1page = y1;
            Single y2page = y2;
            PointF[] points = new PointF[2];
            points[0].X = x1;
            points[0].Y = y1page;
            points[1].X = x2;
            points[1].Y = y2page;
            myGraphicControl.DrawLines( pen, points );
        }

        public void DrawString( string s, Font printFont, Brush brush, Single x, Single y, Single w, Single h ) {
            DrawString( s, printFont, brush, x, y, w, h, new StringFormat() );
            //RectangleF
        }

        public void DrawString( string s, Font printFont, Brush brush, Single x, Single y, Single w, Single h, StringFormat sf ) {
            Single yPage = y;
            RectangleF r = new RectangleF();
            r.X = x;
            r.Y = yPage;
            r.Width = w;
            r.Height = h;
            myGraphicControl.DrawString( s, printFont, brush, r, sf );
        }

        public void DrawString( string s, Font printFont, Brush brush, RectangleF r, StringFormat sf ) {
            myGraphicControl.DrawString( s, printFont, brush, r, sf );
        }

        public void DrawRectangle( Pen pen, Single x, Single y, Single w, Single h ) {
            Single yPage = y;
            myGraphicControl.DrawRectangle( pen, x, yPage, w, h );
        }

        public void DrawEllipse( Pen pen, Single x, Single y, Single w, Single h ) {
            Single yPage = y;
            myGraphicControl.DrawEllipse( pen, x, yPage, w, h );
        }

        public void FillEllipse( Brush brush, Single x, Single y, Single w, Single h ) {
            Single yPage = y;
            myGraphicControl.FillEllipse( brush, x, yPage, w, h );
        }

        public void DrawImage( Image image, Single x, Single y, Single w, Single h ) {
            Single yPage = y;
            myGraphicControl.DrawImage( image, x, yPage, w, h );
        }

        public void DrawFrame( Pen pen, Single x, Single y, Single w, Single h ) {
        }
        #endregion

    }
}
