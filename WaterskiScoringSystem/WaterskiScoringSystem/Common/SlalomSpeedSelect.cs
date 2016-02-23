using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    [System.ComponentModel.Bindable(true)]
    [ComplexBindingProperties("DataSource", "DataMember")]
    [DefaultBindingProperty("Tag")]

    public partial class SlalomSpeedSelect : UserControl {
        public DataTable myDataTable;
        private Int16 myMaxSpeed;
        private Int16 myMinSpeed;
        private Int16 myShowValueNum = 0;

        public SlalomSpeedSelect() {
            InitializeComponent();
        }

        private void SlalomSpeedSelect_Load(object sender, EventArgs e) {
        }

        public void SelectList_Load(EventHandler parentEvent) {
            Decimal curListCode = 0;
            String curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;
            getSlalomSpeedList();

            foreach (DataRow curRow in myDataTable.Rows) {
                curListCode = (Decimal)curRow["ListCodeNum"];
                curListValue = (String)curRow["CodeValue"] + "/" + (String)curRow["ListCode"];
                curItemSize = curListValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point(1, curItemLoc);
                curRadioButton.Name = curListValue;
                curRadioButton.Size = new System.Drawing.Size(curItemSize, 15);
                curRadioButton.TabIndex = 0;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curListValue;
                curRadioButton.UseVisualStyleBackColor = true;
                curRadioButton.Value = curListCode.ToString("00");
                curItemLoc += 17;
                curRadioButton.Click += new System.EventHandler(radioButton_Click);
                if (parentEvent != null) {
                    curRadioButton.Click += new System.EventHandler(parentEvent);
                }
                this.Controls.Add(curRadioButton);
            }
        }

        private void radioButton_Click(object sender, EventArgs e) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            Int16 rbValue = Convert.ToByte( (String)myRadio.Value );
            this.Tag = rbValue.ToString("00");
        }

        public Int16 CurrentValue {
            get {
                Int16 myValue = 0;
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Checked == true) {
                            myValue = Convert.ToInt16((String)myRadio.Value);
                            break;
                        }
                    }
                }
                return myValue;
            }

            set {
                Int16 rbValue = 0;
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        rbValue = Convert.ToInt16((String)myRadio.Value);
                        if ( rbValue == value ) {
                            myRadio.Checked = true;
                            myRadio.ForeColor = Color.DarkBlue;
                            myRadio.BackColor = Color.White;
                            this.Tag = rbValue.ToString();
                            CurrentShowValueNum = rbValue;
                        } else if ( rbValue > myMaxSpeed ) {
                            myRadio.ForeColor = Color.Gray;
                            myRadio.BackColor = Color.Silver;
                        } else {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
                return;
            }
        }

        public String CurrentValueDesc {
            get {
                String myValue = "";
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Checked == true) {
                            myValue = myRadio.Text;
                            break;
                        }
                    }
                }
                return myValue;
            }

            set {
                String rbValue = null;
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Text.Equals(value)) {
                            myRadio.Checked = true;
                            myRadio.ForeColor = Color.DarkBlue;
                            myRadio.BackColor = Color.White;
                            rbValue = (String)myRadio.Value;
                            this.Tag = rbValue;
                            return;
                        } else {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
                return;
            }

        }

        public Int16 CurrentShowValueNum {
            get { return myShowValueNum; }
            set { myShowValueNum = value; }
        }

        public void showCurrentValue( Int16 inValue ) {
            Int16 rbValue;
            String curEntryValue = "";
            RadioButtonWithValue myRadio = null;
            foreach ( Control myControl in this.Controls ) {
                if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                    myRadio = (RadioButtonWithValue)myControl;
                    if ( myRadio.Checked ) {
                    } else {
                        curEntryValue = (String)myRadio.Value;
                        rbValue = Convert.ToInt16( curEntryValue );
                        if ( rbValue == inValue ) {
                            CurrentShowValueNum = rbValue;
                            myRadio.ForeColor = Color.White;
                            myRadio.BackColor = Color.Lime;
                        } else if ( rbValue > myMaxSpeed ) {
                            myRadio.ForeColor = Color.Gray;
                            myRadio.BackColor = Color.Silver;
                        } else {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
            }
        }

        public void resetCurrentValue( Int16 inValue ) {
            Int16 rbValue;
            String curEntryValue = "";
            RadioButtonWithValue myRadio = null;
            foreach ( Control myControl in this.Controls ) {
                if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                    myRadio = (RadioButtonWithValue)myControl;
                    if ( myRadio.Checked ) {
                    } else {
                        curEntryValue = (String)myRadio.Value;
                        rbValue = Convert.ToInt16( curEntryValue );
                        if ( rbValue == inValue ) {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
            }
        }

        public Int16 MaxValue {
            get {
                return myMaxSpeed;
            }
            set {
                myMaxSpeed = value;
                Int16 rbValue;
                RadioButtonWithValue myRadio = null;
                foreach ( Control myControl in this.Controls ) {
                    if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                        myRadio = (RadioButtonWithValue)myControl;
                        rbValue = Convert.ToInt16( (String)myRadio.Value );
                        if ( rbValue > myMaxSpeed ) {
                            myRadio.Visible = true;
                            myRadio.ForeColor = Color.Gray;
                            myRadio.BackColor = Color.Silver;
                        } else if (rbValue < myMinSpeed) {
                            myRadio.Visible = false;
                        } else {
                            myRadio.Visible = true;
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
                return;
            }
        }

        public void resetMaxSpeed() {
            Int16 rbValue;
            RadioButtonWithValue myRadio = null;
            foreach ( Control myControl in this.Controls ) {
                if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                    myRadio = (RadioButtonWithValue)myControl;
                    rbValue = Convert.ToInt16( (String)myRadio.Value );
                    if ( myRadio.Checked ) {
                    } else if ( rbValue > myMaxSpeed ) {
                        myRadio.Visible = true;
                        myRadio.ForeColor = Color.Gray;
                        myRadio.BackColor = Color.Silver;
                    } else if (rbValue < myMinSpeed) {
                        myRadio.Visible = false;
                    } else {
                        myRadio.Visible = true;
                    }
                }
            }
        }

        public Int16 MinValue {
            get {
                return myMinSpeed;
            }
            set {
                myMinSpeed = value;
                Int16 rbValue;
                RadioButtonWithValue myRadio = null;
                foreach ( Control myControl in this.Controls ) {
                    if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                        myRadio = (RadioButtonWithValue)myControl;
                        rbValue = Convert.ToInt16( (String)myRadio.Value );
                        if ( rbValue > myMaxSpeed ) {
                            //myRadio.Visible = false;
                            myRadio.ForeColor = Color.Gray;
                            myRadio.BackColor = Color.Silver;
                        } else if ( rbValue < myMinSpeed ) {
                            myRadio.Visible = false;
                        } else {
                            myRadio.Visible = true;
                        }
                    }
                }
                return;
            }
        }

        public String NextValue {
            get {
                String myValue = "";
                Boolean setNextValue = false;
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (setNextValue) {
                            myRadio.Checked = true;
                            myRadio.ForeColor = Color.DarkBlue;
                            myRadio.BackColor = Color.White;
                            myValue = (String)myRadio.Value;
                            break;
                        } else {
                            if (myRadio.Checked == true) {
                                myRadio.Checked = false;
                                myRadio.ForeColor = Color.Black;
                                myRadio.BackColor = Color.DarkGray;
                                setNextValue = true;
                            }
                        }
                    }
                }
                return myValue;
            }
        }

        private void getSlalomSpeedList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'SlalomSpeeds'" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
            myDataTable = getData( curSqlStmt.ToString() );
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
