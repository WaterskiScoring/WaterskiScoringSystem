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
		private String myAgeGroup;
		private Int16 myMaxSpeedKph;
        private Int16 myMinSpeedKph;
        private Int16 myShowSpeedKph = 0;
		private EventHandler myParentEvent;

		public SlalomSpeedSelect() {
            InitializeComponent();
        }

        private void SlalomSpeedSelect_Load(object sender, EventArgs e) {
        }

        public void SelectList_Load(EventHandler parentEvent) {
			myDataTable = getSlalomSpeedList();
			myParentEvent = parentEvent;
            loadSelectionList();
		}

		public void refreshSelectionList( String inAgeGroup, Int16 inMaxSpeedKph, Int16 inMinSpeedKph ) {
			this.myMaxSpeedKph = inMaxSpeedKph;
			this.myMinSpeedKph = inMinSpeedKph;
			this.myAgeGroup = inAgeGroup;
            this.Controls.Clear();
			loadSelectionList();

			Int16 rbValue;
			RadioButtonWithValue curRadio = null;
			foreach ( Control curControl in this.Controls ) {
				if ( curControl.GetType() == typeof( RadioButtonWithValue ) ) {
					curRadio = (RadioButtonWithValue) curControl;
					rbValue = Convert.ToInt16( (String) curRadio.Value );
					if ( rbValue > myMaxSpeedKph ) {
						if ( myAgeGroup.StartsWith( "M" ) ) {
							curRadio.Visible = true;
							curRadio.ForeColor = Color.Gray;
							curRadio.BackColor = Color.Silver;
						} else if ( ( myAgeGroup.StartsWith( "W" ) ) && ( rbValue < 58 ) ) {
							curRadio.Visible = true;
							curRadio.ForeColor = Color.Gray;
							curRadio.BackColor = Color.Silver;
						} else {
							curRadio.Visible = false;
						}

					} else if ( rbValue < myMinSpeedKph ) {
						curRadio.Visible = false;

					} else {
						curRadio.Visible = true;
						curRadio.ForeColor = Color.Black;
						curRadio.BackColor = Color.Silver;
					}
				}
			}

			return;
		}

		private void loadSelectionList() {
			Decimal curListCode = 0;
			String curListValue = "";
			RadioButtonWithValue curRadioButton;
			int curItemLoc = 1;
			int curItemSize = 0;

			foreach ( DataRow curRow in myDataTable.Rows ) {
				curListCode = (Decimal) curRow["ListCodeNum"];
				curListValue = (String) curRow["SpeedMphDesc"] + "/" + (String) curRow["SpeedKphDesc"];
				curItemSize = curListValue.Length * 8;

				curRadioButton = new RadioButtonWithValue();
				curRadioButton.AutoSize = true;
				curRadioButton.Location = new System.Drawing.Point( 1, curItemLoc );
				curRadioButton.Name = curListValue;
				curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
				curRadioButton.TabIndex = 0;
				curRadioButton.TabStop = true;
				curRadioButton.Text = curListValue;
				curRadioButton.UseVisualStyleBackColor = true;
				curRadioButton.Value = curListCode.ToString( "00" );
				curItemLoc += 17;
				curRadioButton.Click += new System.EventHandler( radioButton_Click );
				if ( myParentEvent != null ) {
					curRadioButton.Click += new System.EventHandler( myParentEvent );
				}
				this.Controls.Add( curRadioButton );
			}
		}

		private void radioButton_Click(object sender, EventArgs e) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            Int16 rbValue = Convert.ToByte( (String)myRadio.Value );
            this.Tag = rbValue.ToString("00");
        }

        public Int16 SelectSpeekKph {
            get {
				Int16 curValue = 0;
				RadioButtonWithValue curRadio = null;
                foreach (Control curControl in this.Controls) {
                    if (curControl.GetType() == typeof(RadioButtonWithValue)) {
                        curRadio = (RadioButtonWithValue)curControl;
                        if (curRadio.Checked == true) {
							curValue = Convert.ToInt16((String)curRadio.Value);
                            break;
                        }
                    }
                }
                return curValue;
            }

            set {
				if ( value == 0 ) return;
                Int16 rbValue = 0;
                foreach (Control curControl in this.Controls) {
                    if (curControl.GetType() == typeof(RadioButtonWithValue)) {
						RadioButtonWithValue curRadio = (RadioButtonWithValue)curControl;
                        rbValue = Convert.ToInt16((String)curRadio.Value);
						if ( curRadio.Visible == false ) {
						} else if ( rbValue == value ) {
                            curRadio.Checked = true;
							curRadio.ForeColor = Color.DarkBlue;
							curRadio.BackColor = Color.White;
							this.Tag = rbValue.ToString();
                            CurrentShowSpeedKph = rbValue;

						} else if ( rbValue > myMaxSpeedKph ) {
							curRadio.Visible = true;
							curRadio.ForeColor = Color.Gray;
							curRadio.BackColor = Color.Silver;

						} else {
							curRadio.Visible = true;
							curRadio.ForeColor = Color.Black;
							curRadio.BackColor = Color.Silver;
                        }
                    }
                }
                return;
            }
        }

        public String CurrentValueDesc {
            get {
                String curValue = "";
                RadioButtonWithValue curRadio = null;
                foreach (Control curControl in this.Controls) {
                    if (curControl.GetType() == typeof(RadioButtonWithValue)) {
                        curRadio = (RadioButtonWithValue)curControl;
                        if (curRadio.Checked == true) {
                            curValue = curRadio.Text;
                            break;
                        }
                    }
                }
                return curValue;
            }

            set {
                return;
            }

        }

        public Int16 CurrentShowSpeedKph {
            get { return myShowSpeedKph; }
            set {
				if ( value == 0 ) return;
				myShowSpeedKph = value;
			}
        }

        public void showActiveValue( Int16 inValue ) {
            Int16 rbValue;
            String curEntryValue = "";
            //RadioButtonWithValue curRadio = null;
            foreach ( Control curControl in this.Controls ) {
                if ( curControl.GetType() == typeof( RadioButtonWithValue ) ) {
					RadioButtonWithValue curRadio = (RadioButtonWithValue)curControl;
                    if ( curRadio.Checked ) {
                    } else if ( curRadio.Visible == false ) {
					} else {
						curEntryValue = (String)curRadio.Value;
                        rbValue = Convert.ToInt16( curEntryValue );
                        if ( rbValue == inValue ) {
                            CurrentShowSpeedKph = rbValue;
							curRadio.ForeColor = Color.DarkBlue;
							curRadio.BackColor = Color.Lime;

						} else if ( rbValue > myMaxSpeedKph ) {
							curRadio.Visible = true;
							curRadio.ForeColor = Color.Gray;
							curRadio.BackColor = Color.Silver;

						} else {
							curRadio.Visible = true;
							curRadio.ForeColor = Color.Black;
							curRadio.BackColor = Color.Silver;
                        }
                    }
                }
            }
			return;
        }

		public Int16 MaxSpeedKph {
			get {
				return myMaxSpeedKph;
			}
			set {
				return;
			}
		}

		public Int16 MinSpeedKph {
			get {
				return myMinSpeedKph;
			}
			set {
				return;
			}
		}

		public String NextValue {
            get {
                String curValue = "";
                Boolean setNextValue = false;
                RadioButtonWithValue curRadio = null;
                foreach (Control curControl in this.Controls) {
                    if (curControl.GetType() == typeof(RadioButtonWithValue)) {
                        curRadio = (RadioButtonWithValue)curControl;
                        if (setNextValue) {
                            curRadio.Checked = true;
                            curRadio.ForeColor = Color.DarkBlue;
                            curRadio.BackColor = Color.White;
                            curValue = (String)curRadio.Value;
                            break;

						} else if (curRadio.Checked == true) {
							curRadio.Checked = false;
							setNextValue = true;
						}
					}
                }
                return curValue;
            }
        }

        private DataTable getSlalomSpeedList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCodeNum, CodeValue as SpeedMphDesc, ListCode as SpeedKphDesc" );
			curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'SlalomSpeeds'" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
            return getData( curSqlStmt.ToString() );
		}

		private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
