/*
 * Write controls from a windows form as form based report
 * For a multi page form report use a tab control and each tab page will become a report page
 * 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WaterskiScoringSystem.Common {
    class FormReportToFile {
        #region  "Class instance variables"
        private int myPageIdx; // Instance variable to keep track of current tab page being printed
        private String myReportName;
        private Control myControlToPrint;
        private String myReportHeader;

        private ArrayList myTextBoxLikeControls;
        private ArrayList myPrintDelegates;

        private class PrintDelegateControl {
            public string TypeName;
            public ControlPrinting PrintFunction;
        }
	    #endregion

        public FormReportToFile( Control inControl ) {
            myControlToPrint = inControl;
            myReportName = inControl.Text;
            myReportHeader = "";
            myPageIdx = 0;

            myTextBoxLikeControls = new ArrayList();
            myPrintDelegates = new ArrayList();

            // add build-in types and functions
            AddTextBoxLikeControl( "ComboBox" );
            AddTextBoxLikeControl( "DateTimePicker" );
            AddTextBoxLikeControl( "DateTimeSlicker" );
            AddTextBoxLikeControl( "NumericUpDown" );

            AddPrintDelegateControl( "TextBox", new ControlPrinting( WriteTextBox ) );
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

        public void Write() {
            Write( "print" );
        }
        public void Write(String inFileName) {
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;
            try {
				String curFileFilter = "prn files (*.prn)|*.prn|All files (*.*)|*.*";
				outBuffer = HelperFunctions.getExportFile( curFileFilter, inFileName );
                if (outBuffer != null) {
                    outLine = new StringBuilder( "Report Name: " + inFileName );
                    outBuffer.WriteLine( outLine.ToString() );
                    WriteReport( outBuffer );
                }
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in Write method of FormReportToFile"
                + "\n Exception: " + ex.ToString() );
            } finally {
                outBuffer.Close();
                MessageBox.Show( "Form successfully written to selected file" );
            }

        }

        private void WriteReport( StreamWriter outBuffer ) {
            bool curReturnValue = false;

            string topControlType = myControlToPrint.GetType().ToString();
            if ( topControlType.IndexOf( "TabControl" ) > 0 ) {
                TabControl curTabControl = (TabControl)myControlToPrint;
                curReturnValue = WriteControls( outBuffer, curTabControl.TabPages[myPageIdx], 0, 0 );
            } else {
                curReturnValue = WriteControls( outBuffer, myControlToPrint, 0, 0 );
            }

        }

		/// <summary>
		/// Print child controls of a "parent" control.
		/// Controls are printed from top to bottom.
		/// This is the function that calculate position of each control depending of the extension of child controls
		/// </summary>
        public bool WriteControls(StreamWriter outBuffer, Control inControlToPrint, int inPageAdjX, int inPageAdjY) {
            int curCount = inControlToPrint.Controls.Count;
            Single[] yPosList = new Single[curCount];
            Control[] controlList = new Control[curCount];

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
            var curLineControlList = new List<KeyValuePair<int, Control>>();
            int curX = 0, curY = 0, prevY = 0;
            for ( int curIdx = 0; curIdx < curCount; curIdx++ ) {
                Control curControl = controlList[curIdx];
                curX = controlList[curIdx].Location.X + inPageAdjX;
                curY = controlList[curIdx].Location.Y + inPageAdjY;
                if (prevY > 0) {
                    if ( curY > prevY ) {
                        if (curLineControlList.Count > 0) {
                            WriteControlsForLine( outBuffer, curLineControlList, inPageAdjX, prevY );
                        }

                        curLineControlList = new List<KeyValuePair<Int32, Control>>();
                        curLineControlList.Add( new KeyValuePair<int, Control>( curX, curControl ) );
                    } else {
                        curLineControlList.Add( new KeyValuePair<int, Control>( curX, curControl ) );
                    }
                } else {
                    curLineControlList = new List<KeyValuePair<Int32, Control>>();
                    curLineControlList.Add( new KeyValuePair<int, Control>( curX, curControl ) );
                }
                prevY = curY;
            }
            if (curLineControlList.Count > 0) {
                WriteControlsForLine( outBuffer, curLineControlList, inPageAdjX, prevY );
            }
            return false;
        }

        public bool WriteControlsForLine(StreamWriter outBuffer, List<KeyValuePair<int, Control>> inLineControlList, int inPageAdjX, int inPageAdjY) {
            //KeyValuePair<int, Control> curKeyValuePair;
            int curX = 0, curY = 0;
            inLineControlList.Sort( CompareLineListControl );
            //System.Array.Sort( inLineControlList.g, controlList );
            for (int curIdx = 0; curIdx < inLineControlList.Count; curIdx++) {
                Control curControl = inLineControlList[curIdx].Value;
                curX = curControl.Location.X + inPageAdjX;
                curY = curControl.Location.Y + inPageAdjY;
                WriteControl( outBuffer, curControl, curX, curY );
            }

            return true;
        }

        public bool WriteControl(StreamWriter outBuffer, Control inControlToPrint, int inPageAdjX, int inPageAdjY) {
            string topControlType = myControlToPrint.GetType().ToString();

            if ( inControlToPrint.Visible == true || topControlType.IndexOf( "TabControl" ) > 0 ) {
                string curControlType = inControlToPrint.GetType().ToString();
                bool controlFound = false;
                //First check if it's a text box like control
                foreach ( string tmpControlType in myTextBoxLikeControls ) {
                    if ( curControlType.IndexOf( tmpControlType ) >= 0 ) {
                        controlFound = true;
                        WriteText( outBuffer, inControlToPrint, inPageAdjX, inPageAdjY );
                        break;
                    }
                }

                if ( !(controlFound) ) {
                    for ( int curIdx = myPrintDelegates.Count - 1; curIdx >= 0; curIdx-- ) {
                        PrintDelegateControl curDelegate = (PrintDelegateControl)myPrintDelegates[curIdx];
                        if ( curControlType.EndsWith( curDelegate.TypeName ) ) {
                            curDelegate.PrintFunction( outBuffer, inControlToPrint, inPageAdjX, inPageAdjY );
                            if ( inControlToPrint.Controls.Count > 0 ) {
                                int curControlX = inPageAdjX + inControlToPrint.Location.X;
                                int curControlY = inPageAdjY + inControlToPrint.Location.Y;
                                WriteControls( outBuffer, inControlToPrint, curControlX, curControlY );
                            }
                            break;
                        }
                    }
                }
            }
            return false;
        }

        // delegate for providing of print function by control type
        public delegate void ControlPrinting(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY);

        #region  "Getter and Setter for all instance objects"
        public String ReportName {
            get { return myReportName; }
            set { myReportName = value; }
        }
        public String ReportHeader {
            get { return myReportHeader; }
            set { myReportHeader = value; }
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
        /// <param name="printFunction">function (must match with FormReportToFile.ControlPrinting delegate)</param>
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
        public void WriteTextBox(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
            TextBox curControl = (System.Windows.Forms.TextBox)inControl;
            WriteText( outBuffer, inControl, inPageAdjX, inPageAdjY );
        }

        public void PrintLabel(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
            Label curControl = (System.Windows.Forms.Label)inControl;
            string curText = curControl.Text;
            WriteText( outBuffer, inControl, inPageAdjX, inPageAdjY );
        }

        public void PrintCheckBox(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
            CheckBox curControl = (System.Windows.Forms.CheckBox)inControl;
            string curText = curControl.Text;

            int curX = inPageAdjX + curControl.Location.X;
            int curY = inPageAdjY + curControl.Location.Y;
            if ( curControl.Checked ) {
            }
            WriteText( outBuffer, inControl, inPageAdjX, inPageAdjY );
        }

        public void PrintRadioButton(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
            RadioButton curControl = (System.Windows.Forms.RadioButton)inControl;
            string curText = curControl.Text;

            int curX = inPageAdjX + curControl.Location.X;
            int curY = inPageAdjY + curControl.Location.Y;
            if ( curControl.Checked ) {
            }

            WriteText( outBuffer, inControl, inPageAdjX, inPageAdjY );
        }

        public void PrintPanel(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
        }

        public void PrintGroupBox(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
            bool isDrawBox = false;
            bool isVAlignCenter = false;

            GroupBox curControl = (System.Windows.Forms.GroupBox)inControl;
            string curText = curControl.Text;

            int curX = inPageAdjX + curControl.Location.X;
            int curY = inPageAdjY + curControl.Location.Y;
            WriteText( outBuffer, curControl, curX, curY );
        }

        public void PrintTabControl(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
        }

        public void PrintTabPage(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
        }

        public void PrintPictureBox(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
        }

        public void PrintListBox(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
        }

        public void PrintCheckedListBox(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
        }

        public void PrintDataGrid(StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY) {
        }

        // The funtion that print the title, page number, and the header row
        private int DrawHeader( ) {
            return 0;
        }

        #endregion

        public void WriteText( StreamWriter outBuffer, Control inControl, int inPageAdjX, int inPageAdjY ) {
            // Print a single line text for many controls. Do some formatting
            int curX = inPageAdjX + inControl.Location.X;
            int curY = inPageAdjY + inControl.Location.Y;
            outBuffer.WriteLine( "X:" + inControl.Location.X + " Y:" + inControl.Location.Y + " TEXT:" + inControl.Text );
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

        static int CompareLineListControl(KeyValuePair<int, Control> a, KeyValuePair<int, Control> b) {
            return a.Key.CompareTo( b.Key );
        }
    }
}
