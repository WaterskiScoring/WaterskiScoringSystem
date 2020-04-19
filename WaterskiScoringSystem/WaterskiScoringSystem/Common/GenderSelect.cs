using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Admin;

namespace WaterskiScoringSystem.Common {
    [System.ComponentModel.Bindable(true)]
    [ComplexBindingProperties("DataSource", "DataMember")]
    [DefaultBindingProperty("Tag")]

    public partial class GenderSelect : UserControl {
        public GenderSelect() {
            InitializeComponent();
        }

        private void GenderSelect_Load(object sender, EventArgs e) {
            String curListCode = "", curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 5;
            int curItemSize = 0;

            curListCode = "M";
            curListValue = "Male";
            curItemSize = curListValue.Length * 13;

            curRadioButton = new RadioButtonWithValue();
            curRadioButton.AutoSize = true;
            curRadioButton.Location = new System.Drawing.Point(curItemLoc, 5);
            curRadioButton.Name = curListValue;
            curRadioButton.Size = new System.Drawing.Size(curItemSize, 17);
            curRadioButton.TabIndex = 0;
            curRadioButton.TabStop = true;
            curRadioButton.Text = curListValue;
            curRadioButton.UseVisualStyleBackColor = true;
            curRadioButton.Value = curListCode;
            curRadioButton.Click += new System.EventHandler(radioButton_Click);
            curItemLoc += curItemSize + 8;
            this.Controls.Add(curRadioButton);

            curListCode = "F";
            curListValue = "Female";
            curItemSize = curListValue.Length * 8;

            curRadioButton = new RadioButtonWithValue();
            curRadioButton.AutoSize = true;
            curRadioButton.Location = new System.Drawing.Point(curItemLoc, 5);
            curRadioButton.Name = curListValue;
            curRadioButton.Size = new System.Drawing.Size(curItemSize, 17);
            curRadioButton.TabIndex = 0;
            curRadioButton.TabStop = true;
            curRadioButton.Text = curListValue;
            curRadioButton.UseVisualStyleBackColor = true;
            curRadioButton.Value = curListCode;
            curRadioButton.Click += new System.EventHandler(radioButton_Click);
            curItemLoc += curItemSize + 8;
            this.Controls.Add(curRadioButton);
        }

        private void radioButton_Click(object sender, EventArgs e) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            this.Tag = (String)myRadio.Value;
        }

        public void addClickEvent(System.EventHandler inEventHandler ) {
            RadioButtonWithValue myRadio = null;
            foreach ( Control myControl in this.Controls ) {
                if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                    myRadio = (RadioButtonWithValue)myControl;
                    myRadio.Click += new System.EventHandler( inEventHandler );
                }
            }
        }

        public String RatingValue {
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
                String curCellValue = (String)value;
                this.Tag = curCellValue;
                RadioButtonWithValue myDefaultRadio = null;
                RadioButtonWithValue myRadio = null;
                foreach (Control myControl in this.Controls) {
                    if (myControl.GetType() == typeof(RadioButtonWithValue)) {
                        myRadio = (RadioButtonWithValue)myControl;
                        rbValue = (String)myRadio.Value;
                        if (rbValue.Equals(curCellValue)) {
                            myRadio.Checked = true;
                            return;
                        } else if (rbValue.Equals("")) {
                            myDefaultRadio = myRadio;
                        }
                    }
                }
                if (myDefaultRadio != null) myDefaultRadio.Checked = true;
                return;
            }
        }

    }
}
