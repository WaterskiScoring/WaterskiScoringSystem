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

    public partial class JumpSpeedSelect : UserControl {
        public DataTable myDataTable;

        public JumpSpeedSelect() {
            InitializeComponent();
        }

        private void JumpSpeedSelect_Load(object sender, EventArgs e) {
        }

        public void SelectList_Load(EventHandler parentEvent) {
            Decimal curListCode = 0;
            String curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;
            getJumpSpeedList();

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
                curRadioButton.Value = curListCode.ToString();
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
            Int16 rbValue = Convert.ToInt16(Convert.ToDecimal((String)myRadio.Value));
            this.Tag = rbValue.ToString();
        }

        public Int16 CurrentValue {
            get {
                Int16 myValue = 0;
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Checked == true) {
                            myValue = Convert.ToInt16(Convert.ToDecimal((String)myRadio.Value));
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
                        rbValue = Convert.ToInt16(Convert.ToDecimal((String)myRadio.Value));
                        if (rbValue == value) {
                            myRadio.Checked = true;
                            this.Tag = rbValue.ToString();
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
                            rbValue = (String)myRadio.Value;
                            this.Tag = rbValue;
                        } else {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
                return;
            }

        }

        public void SetMaxValue(Decimal inMaxValue) {
            Decimal rbValue;
            RadioButtonWithValue myRadio = null;
            foreach (Control myControl in this.Controls) {
                if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                    myRadio = (RadioButtonWithValue)myControl;
                    rbValue = Convert.ToDecimal((String)myRadio.Value);
                    if (rbValue > inMaxValue) {
                        myRadio.Visible = false;
                    } else {
                        myRadio.Visible = true;
                    }
                }
            }
            return;
        }

        public void SetMinValue(Decimal inMinValue) {
            Decimal rbValue;
            RadioButtonWithValue myRadio = null;
            foreach (Control myControl in this.Controls) {
                if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                    myRadio = (RadioButtonWithValue)myControl;
                    rbValue = Convert.ToDecimal((String)myRadio.Value);
                    if (rbValue < inMinValue) {
                        myRadio.Visible = false;
                    } else {
                        myRadio.Visible = true;
                    }
                }
            }
            return;
        }

        public void SetMinMaxValue(Decimal inMinValue, Decimal inMaxValue) {
            Decimal rbValue;
            RadioButtonWithValue myRadio = null;
            foreach (Control myControl in this.Controls) {
                if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                    myRadio = (RadioButtonWithValue)myControl;
                    rbValue = Convert.ToDecimal((String)myRadio.Value);
                    if (rbValue > inMaxValue) {
                        myRadio.Visible = false;
                    } else if (rbValue < inMinValue) {
                        myRadio.Visible = false;
                    } else {
                        myRadio.Visible = true;
                    }
                }
            }
            return;
        }

        private void getJumpSpeedList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'JumpSpeeds'" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
            myDataTable = getData( curSqlStmt.ToString() );
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
