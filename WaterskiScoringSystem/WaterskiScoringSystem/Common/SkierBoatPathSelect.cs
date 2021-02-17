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
    public partial class SkierBoatPathSelect : UserControl {
		private String myDefaultValue = "";

        public SkierBoatPathSelect() {
            InitializeComponent();
        }

        private void SkierBoatPathSelect_Load(object sender, EventArgs e) {
        }

        public void SelectList_Load(EventHandler parentEvent) {
            String curListCode = "", curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;
            int curIdx = 1;
			DataTable curDataTable = getSkierBoatPath();
			myDefaultValue = (String)curDataTable.Rows[0]["ListCode"];

			foreach (DataRow curRow in curDataTable.Rows) {
                curListCode = (String)curRow["ListCode"];
                curListValue = (String)curRow["ListCode"] + "/" + (String)curRow["CodeDesc"];
                curItemSize = curListValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point( 1, curItemLoc );
                curRadioButton.Name = curListValue;
                curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
                curRadioButton.TabIndex = curIdx;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curListValue;
                curRadioButton.UseVisualStyleBackColor = true;
                curRadioButton.Value = curListCode;
                curRadioButton.Click += new System.EventHandler( radioButton_Click );
                if (parentEvent != null) {
                    curRadioButton.Click += new System.EventHandler( parentEvent );
                }
                curItemLoc += 17;
                this.Controls.Add( curRadioButton );
            }
        }

        private void radioButton_Click(object sender, EventArgs e) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            this.Tag = (String)myRadio.Value;
        }

		public String DefaultValue {
			get {
				return myDefaultValue;
			}
		}

        public String CurrentValue {
            get {
				String myValue = myDefaultValue;
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof( RadioButtonWithValue )) {
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
				String myValue = myDefaultValue;
				RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof( RadioButtonWithValue )) {
                        myRadio = (RadioButtonWithValue)myControl;
						myValue = (String)myRadio.Value;
                        if ( myValue == value) {
                            myRadio.Checked = true;
                            this.Tag = myValue.ToString();
                            myRadio.ForeColor = Color.DarkBlue;
                            myRadio.BackColor = Color.White;
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
                    if (myControl.GetType() == typeof( RadioButtonWithValue )) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Checked == true) {
                            myValue = myRadio.Text;
                            break;
                        }
                    }
                }
                return myValue;
            }
        }

		private DataTable getSkierBoatPath() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode,  CodeDesc " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = 'JumpSkierBoatPath' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

	}
}
