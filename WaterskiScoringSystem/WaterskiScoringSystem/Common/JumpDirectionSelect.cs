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
    [ComplexBindingProperties("DataSource", "DataMember")]
    [DefaultBindingProperty("Tag")]

    public partial class JumpDirectionSelect : UserControl {

        RadioButtonWithValue myRadioButtonDefault = new RadioButtonWithValue();

        public JumpDirectionSelect() {
            InitializeComponent();
        }

        private void JumpDirectionSelect_Load(object sender, EventArgs e) {
        }

        public void SelectList_Load(EventHandler parentEvent) {
            String curListCode = "", curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;
            int curIdx = 1;

            //Left to rightjump direction
            curListCode = "-1";
            curListValue = "Left to Right";
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
            if ( parentEvent != null ) {
                curRadioButton.Click += new System.EventHandler( parentEvent );
            }
            curItemLoc += 17;
            this.Controls.Add( curRadioButton );

            //Right to left jump direction
            curListCode = "1";
            curListValue = "Right to Left";
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
            if ( parentEvent != null ) {
                curRadioButton.Click += new System.EventHandler( parentEvent );
            }
            curItemLoc += 17;
            this.Controls.Add( curRadioButton );
        }

        private void radioButton_Click(object sender, EventArgs e) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            this.Tag = (String)myRadio.Value;
        }

        public String CurrentValue {
            get {
                String myValue = "0";
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
                String rbValue = "0";
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        rbValue = (String)myRadio.Value;
                        if (rbValue == value) {
                            myRadio.Checked = true;
                            this.Tag = rbValue.ToString();
                            return;
                        }
                    }
                }
                return;
            }
        }

    }
}
