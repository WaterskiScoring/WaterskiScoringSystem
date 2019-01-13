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

    public partial class SlalomLineSelect : UserControl {
        public DataTable myDataTable;
        private Decimal myShowValueNum = 0M;
    
        public SlalomLineSelect() {
            InitializeComponent();
        }

        private void SlalomLineSelect_Load(object sender, EventArgs e) {
        }

        public void SelectList_Load(EventHandler parentEvent) {
            String curListCode = "", curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;
            int curIdx = 1;
            getSlalomLineData();

            foreach (DataRow curRow in myDataTable.Rows) {
                curListCode = (String)curRow["ListCode"];
                curListValue = (String)curRow["CodeValue"] + "/" + (String)curRow["ListCode"];
                curItemSize = curListValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point(1, curItemLoc);
                curRadioButton.Name = curListValue;
                curRadioButton.Size = new System.Drawing.Size(curItemSize, 15);
                curRadioButton.TabIndex = curIdx;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curListValue;
                curRadioButton.UseVisualStyleBackColor = true;
                curRadioButton.Value = curListCode;
                curRadioButton.Click += new System.EventHandler(radioButton_Click);
                if (parentEvent != null) {
                    curRadioButton.Click += new System.EventHandler(parentEvent);
                }
                curItemLoc += 17;
                this.Controls.Add(curRadioButton);
            }
        }

        private void radioButton_Click(object sender, EventArgs e) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            this.Tag = (String)myRadio.Value;
        }

        public String CurrentValue {
            get {
                String myValue = "";
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Checked == true) {
                            myValue = (String)myRadio.Value;
                            break;
                        }
                    }
                }
                return myValue;
            }

            set {
                String rbValue = null;
                String inValue = "";
                if ( value.Length > 0 ) {
                    if ( value.Substring(value.Length - 1).Equals("M") ) {
                        inValue = value;
                    } else {
                        inValue = value + "M";
                    }
                    RadioButtonWithValue myRadio = null;
                    foreach ( Control myControl in this.Controls ) {
                        if ( myControl.GetType() == typeof(RadioButtonWithValue) ) {
                            myRadio = (RadioButtonWithValue) myControl;
                            rbValue = (String) myRadio.Value;
                            if ( rbValue.Equals(inValue) ) {
                                myRadio.Checked = true;
                                myRadio.ForeColor = Color.DarkBlue;
                                myRadio.BackColor = Color.White;
                                this.Tag = rbValue;
                                CurrentShowValueNum = CurrentValueNum;
                            } else {
                                myRadio.ForeColor = Color.Black;
                                myRadio.BackColor = Color.Silver;
                            }
                        }
                    }
                }
                return;
            }
        }

        public Decimal CurrentValueNum {
            get {
                Decimal myValue = 0;
                String rbValue = "";
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Checked == true) {
                            rbValue = (String)myRadio.Value;
                            //myRadio.ForeColor = Color.DarkBlue;
                            //myRadio.BackColor = Color.White;
                            DataRow[] curLineRow = myDataTable.Select("ListCode = '" + rbValue + "'");
                            myValue = (Decimal)curLineRow[0]["MaxValue"];
                            break;
                        }
                    }
                }
                return myValue;
            }

        }

        public Decimal CurrentShowValueNum {
            get { return myShowValueNum; }
            set { myShowValueNum = value; }
        }

        public void showCurrentValue( Decimal inValue ) {
            Decimal rbValue;
            String curEntryValue = "";
            RadioButtonWithValue myRadio = null;
            foreach ( Control myControl in this.Controls ) {
                if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                    myRadio = (RadioButtonWithValue)myControl;
					curEntryValue = (String) myRadio.Value;
					if ( curEntryValue.Substring( curEntryValue.Length - 1 ).Equals( "M" ) ) {
						rbValue = Convert.ToDecimal( curEntryValue.Substring( 0, curEntryValue.Length - 1 ) );
					} else {
						rbValue = Convert.ToDecimal( curEntryValue );
					}

					if ( myRadio.Checked ) {
						CurrentShowValueNum = rbValue;

					} else {
                        if ( rbValue == inValue ) {
                            CurrentShowValueNum = rbValue;
                            myRadio.ForeColor = Color.White;
                            if ( inValue == 23M ) {
                                myRadio.BackColor = Color.AliceBlue;
                            } else if ( inValue == 18.25M ) {
                                myRadio.BackColor = Color.Red;
                            } else if ( inValue == 16M ) {
                                myRadio.BackColor = Color.Orange;
                            } else if ( inValue == 14.25M ) {
                                myRadio.BackColor = Color.Yellow;
                            } else if ( inValue == 13M ) {
                                myRadio.BackColor = Color.Lime;
                            } else if ( inValue == 12M ) {
                                myRadio.BackColor = Color.LightSkyBlue;
                            } else if ( inValue == 11.25M ) {
                                myRadio.BackColor = Color.Violet;
                            } else if ( inValue == 10.75M ) {
                                myRadio.BackColor = Color.AliceBlue;
                            } else if ( inValue == 10.25M ) {
                                myRadio.BackColor = Color.Pink;
                            } else if ( inValue == 9.75M ) {
                                myRadio.BackColor = Color.DimGray;
                            } else if ( inValue == 9.5M ) {
                                myRadio.BackColor = Color.Red;
                            } else {
                                myRadio.BackColor = Color.Silver;
                            }
                        } else {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
            }
        }

        public void resetCurrentValue( Decimal inValue ) {
            Decimal rbValue;
            String curEntryValue = "";
            RadioButtonWithValue myRadio = null;
            foreach ( Control myControl in this.Controls ) {
                if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                    myRadio = (RadioButtonWithValue)myControl;
                    if ( myRadio.Checked ) {
                    } else {
                        curEntryValue = (String)myRadio.Value;
                        if ( curEntryValue.Substring( curEntryValue.Length - 1 ).Equals( "M" ) ) {
                            rbValue = Convert.ToDecimal( curEntryValue.Substring( 0, curEntryValue.Length - 1 ) );
                        } else {
                            rbValue = Convert.ToDecimal( curEntryValue );
                        }
                        if ( rbValue == inValue ) {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                            CurrentShowValueNum = rbValue;
                        }
                    }
                }
            }
        }

        public void hideLongLine() {
            Decimal rbValue;
            String curEntryValue = "";
            RadioButtonWithValue myRadio = null;
            foreach ( Control myControl in this.Controls ) {
                if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                    myRadio = (RadioButtonWithValue)myControl;
                    curEntryValue = (String)myRadio.Value;
                    if ( curEntryValue.Substring( curEntryValue.Length - 1 ).Equals( "M" ) ) {
                        rbValue = Convert.ToDecimal( curEntryValue.Substring( 0, curEntryValue.Length - 1 ) );
                    } else {
                        rbValue = Convert.ToDecimal( curEntryValue );
                    }
                    if ( rbValue == 23M ) {
                        myRadio.Visible = false;
                    }
                }
            }
        }

        public void showLongLine() {
            Decimal rbValue;
            String curEntryValue = "";
            RadioButtonWithValue myRadio = null;
            foreach ( Control myControl in this.Controls ) {
                if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                    myRadio = (RadioButtonWithValue)myControl;
                    curEntryValue = (String)myRadio.Value;
                    if ( curEntryValue.Substring( curEntryValue.Length - 1 ).Equals( "M" ) ) {
                        rbValue = Convert.ToDecimal( curEntryValue.Substring( 0, curEntryValue.Length - 1 ) );
                    } else {
                        rbValue = Convert.ToDecimal( curEntryValue );
                    }
                    if ( rbValue == 23M ) {
                        myRadio.Visible = true;
                    }
                }
            }
        }

        public Decimal getNextValue( Decimal inValue ) {
            bool curFound = false;
			Decimal curReturnValue = 23;
            foreach ( DataRow curRow in myDataTable.Rows ) {
                if ( curFound ) {
                    curReturnValue = (Decimal)curRow["ListCodeNum"];
                    break;
                } else {
                    if ( ( (Decimal)curRow["ListCodeNum"] ) == inValue ) {
                        curFound = true;
                    }
                }
            }

            return curReturnValue;
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
                                myRadio.ForeColor = Color.DarkBlue;
                                myRadio.BackColor = Color.White;
                                myRadio.Checked = false;
                                setNextValue = true;
                            }
                        }
                    }
                }
                return myValue;
            }
        }

        private void getSlalomLineData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue, SortSeq" );
			curSqlStmt.Append( ", MinValue as LineLengthOff, MaxValue as LineLengthMeters, CodeValue as LineLengthOffDesc " );
			curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'SlalomLines' " );
            curSqlStmt.Append( "ORDER BY SortSeq" );
            myDataTable = getData( curSqlStmt.ToString() );
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
