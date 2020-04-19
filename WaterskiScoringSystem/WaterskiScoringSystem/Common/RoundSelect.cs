using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    [System.ComponentModel.Bindable(true)]
    [DefaultBindingProperty("Tag")]

    public partial class RoundSelect : UserControl {
        RadioButtonWithValue myRadioButtonDefault = new RadioButtonWithValue();
        public RoundSelect() {
            InitializeComponent();
        }

        private void RoundSelect_Load(object sender, EventArgs e) {
        }

        public void SelectList_Load(String inRound, EventHandler parentEvent) {
            String curListCode = "", curListValue = "";
            int myRounds = Convert.ToInt16(inRound) + 1;
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;

            try {
                for (int idx = 1; idx < myRounds; idx++) {
                    curListCode = idx.ToString();
                    curListValue = idx.ToString();
                    curItemSize = curListValue.Length * 8;

                    curRadioButton = new RadioButtonWithValue();
                    curRadioButton.AutoSize = true;
                    curRadioButton.Location = new System.Drawing.Point(1, curItemLoc);
                    curRadioButton.Name = curListValue;
                    curRadioButton.Size = new System.Drawing.Size(curItemSize, 15);
                    curRadioButton.TabIndex = 0;
                    curRadioButton.TabStop = true;
                    curRadioButton.Text = curListValue;
                    curRadioButton.ForeColor = Color.Black;
                    curRadioButton.BackColor = Color.DarkGray;
                    curRadioButton.Value = curListCode;
                    curRadioButton.Click += new System.EventHandler(parentEvent);

                    curItemLoc += 17;
                    this.Controls.Add(curRadioButton);
                }

                curListCode = "25";
                curListValue = "RO";
                curItemSize = curListValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point( 1, curItemLoc );
                curRadioButton.Name = curListValue;
                curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
                curRadioButton.TabIndex = 0;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curListValue;
                curRadioButton.ForeColor = Color.Black;
                curRadioButton.BackColor = Color.DarkGray;
                curRadioButton.Value = curListCode;
                curRadioButton.Click += new System.EventHandler( parentEvent );

                curItemLoc += 17;
                this.Controls.Add( curRadioButton );

            } catch (Exception ex) {
                MessageBox.Show("Exception encountered"
                    + "\n ListCode: " + curListCode
                    + "\n CodeValue: " + curListValue
                    + "\n Exception: " + ex.Message
                    );
            }
        }

        public void SelectList_LoadHorztl(String inRound, EventHandler parentEvent) {
            SelectList_LoadHorztl(inRound, parentEvent, true);
        }
        public void SelectList_LoadHorztl (String inRound, EventHandler parentEvent, bool inAddRO) {
            String curListCode = "", curListValue = "";
            int myRounds = Convert.ToInt16(inRound) + 1;
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 2;
            int curItemSize = 0;

            try {
                for (int idx = 1; idx < myRounds; idx++) {
                    curListCode = idx.ToString();
                    curListValue = idx.ToString();
                    curItemSize = curListValue.Length * 20;

                    curRadioButton = new RadioButtonWithValue();
                    curRadioButton.AutoSize = true;
                    curRadioButton.Location = new System.Drawing.Point(curItemLoc, 2);
                    curRadioButton.Name = curListValue;
                    curRadioButton.Size = new System.Drawing.Size(curItemSize, 20);
                    curRadioButton.TabIndex = 0;
                    curRadioButton.TabStop = true;
                    curRadioButton.Text = curListValue;
                    curRadioButton.ForeColor = Color.Black;
                    curRadioButton.BackColor = Color.Silver;
                    curRadioButton.Value = curListCode;
                    curRadioButton.Click += new System.EventHandler(parentEvent);

                    curItemLoc += curItemSize + 20;
                    this.Controls.Add(curRadioButton);
                }
                if (inAddRO) {
                    curListCode = "25";
                    curListValue = "RO";
                    curItemSize = curListValue.Length * 20;

                    curRadioButton = new RadioButtonWithValue();
                    curRadioButton.AutoSize = true;
                    curRadioButton.Location = new System.Drawing.Point(curItemLoc, 2);
                    curRadioButton.Name = curListValue;
                    curRadioButton.Size = new System.Drawing.Size(curItemSize, 20);
                    curRadioButton.TabIndex = 0;
                    curRadioButton.TabStop = true;
                    curRadioButton.Text = curListValue;
                    curRadioButton.ForeColor = Color.Black;
                    curRadioButton.BackColor = Color.Silver;
                    curRadioButton.Value = curListCode;
                    curRadioButton.Click += new System.EventHandler(parentEvent);

                    curItemLoc += curItemSize + 20;
                    this.Controls.Add(curRadioButton);
                }
            } catch ( Exception ex ) {
                MessageBox.Show("Exception encountered"
                    + "\n ListCode: " + curListCode
                    + "\n CodeValue: " + curListValue
                    + "\n Exception: " + ex.Message
                    );
            }
        }

        public String RoundValue {
            get {
                String myValue = "";
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if (myRadio.Checked == true) {
                            myValue = (String)myRadio.Value;
                            myRadio.ForeColor = Color.DarkBlue;
                            myRadio.BackColor = Color.White;
                            break;
                        }
                    }
                }
                return myValue;
            }

            set {
                String rbValue = null;
                String curCellValue = (String)value;
                this.Tag = curCellValue;
                RadioButtonWithValue myDefaultRadio = null;
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        rbValue = (String)myRadio.Value;
                        if (rbValue.Equals(curCellValue)) {
                            myRadio.ForeColor = Color.DarkBlue;
                            myRadio.BackColor = Color.White;
                            myRadio.Checked = true;
                        } else if (rbValue.Equals("")) {
                            myDefaultRadio = myRadio;
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        } else {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
                if (myDefaultRadio != null) myDefaultRadio.Checked = true;
                return;
            }
        }

    }
}
