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
    public partial class RampHeightSelect : UserControl {

        public RampHeightSelect() {
            InitializeComponent();
        }

        private void RampHeightSelect_Load(object sender, EventArgs e) {
        }

        public void SelectList_Load(EventHandler parentEvent) {
            String curListCode = "", curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;
            int curIdx = 1;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, CodeValue FROM CodeValueList WHERE ListName = 'RampHeight' ORDER BY SortSeq" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );

            foreach (DataRow curRow in curDataTable.Rows) {
                curListCode = (String)curRow["ListCode"];
                curListValue = (String)curRow["CodeValue"] + "/" + (String)curRow["ListCode"];
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

        public Decimal CurrentValue {
            get {
                Decimal myValue = Convert.ToDecimal( "0.00" );
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof( RadioButtonWithValue )) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Checked == true) {
                            myValue = Convert.ToDecimal( (String)myRadio.Value );
                            break;
                        }
                    }
                }
                return myValue;
            }

            set {
                Decimal rbValue = Convert.ToDecimal( "0.00" );
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof( RadioButtonWithValue )) {
                        myRadio = (RadioButtonWithValue)myControl;
                        rbValue = Convert.ToDecimal( (String)myRadio.Value );
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

        public void SetMaxValue(Decimal inMaxValue) {
            Decimal rbValue;
            RadioButtonWithValue myRadio = null;
            foreach (Control myControl in this.Controls) {
                if (myControl.GetType() == typeof( RadioButtonWithValue )) {
                    myRadio = (RadioButtonWithValue)myControl;
                    rbValue = Convert.ToDecimal( (String)myRadio.Value );
                    if (rbValue > inMaxValue) {
                        myRadio.Visible = false;
                    } else {
                        if (inMaxValue == 6M) {
                            if (rbValue < 5.5M) {
                                myRadio.Visible = false;
                            } else {
                                myRadio.Visible = true;
                            }
                        } else {
                            myRadio.Visible = true;
                        }
                    }
                }
            }
            return;
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
