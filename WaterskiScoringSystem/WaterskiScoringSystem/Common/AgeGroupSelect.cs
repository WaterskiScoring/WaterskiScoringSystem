using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    [System.ComponentModel.Bindable( true )]
    [ComplexBindingProperties( "DataSource", "DataMember" )]
    [DefaultBindingProperty( "Tag" )]

    public partial class AgeGroupSelect : UserControl {
        private AgeGroupDropdownList myAgeGroupDropdownList = null;

        public DataTable myDataTable;

        public AgeGroupSelect() {
            InitializeComponent();
        }

        private void AgeGroupSelect_Load( object sender, EventArgs e ) {
        }

        public void SelectList_Load(EventHandler parentEvent) {
            IEnumerator myList = this.Controls.GetEnumerator();
            while ( myList.MoveNext() ) {
                if ( myList.Current.GetType() == typeof(RadioButtonWithValue) ) {
                    this.Controls.Remove( (RadioButtonWithValue)myList.Current );
                }
            }

        }
        public void SelectList_Load(String inGender, DataRow inTourRow, EventHandler parentEvent) {
            String curListCode = "", curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;
            int curIdx = 1;
            this.Controls.Clear();
            myAgeGroupDropdownList = new AgeGroupDropdownList( inTourRow );
            ArrayList myDropdownList = myAgeGroupDropdownList.getDivListForGender( inGender );
            IEnumerator myList = myDropdownList.GetEnumerator();
            while (myList.MoveNext()) {
                curListCode = (String)myList.Current;
                curItemSize = curListValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point( 1, curItemLoc );
                curRadioButton.Name = curListValue;
                curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
                curRadioButton.TabIndex = curIdx;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curListCode;
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
        public void SelectList_Load(Int16 inSkiYearAge, String inGender, DataRow inTourRow, EventHandler parentEvent) {
            String curListCode = "", curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 1;
            int curItemSize = 0;
            int curIdx = 1;
            this.Controls.Clear();
            myAgeGroupDropdownList = new AgeGroupDropdownList( inTourRow );
            ArrayList myDropdownList = myAgeGroupDropdownList.getDivListForAge( inSkiYearAge, inGender );
            IEnumerator myList = myDropdownList.GetEnumerator();
            while ( myList.MoveNext() ) {
                curListCode = (String)myList.Current;
                curItemSize = curListValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point( 1, curItemLoc );
                curRadioButton.Name = curListValue;
                curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
                curRadioButton.TabIndex = curIdx;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curListCode;
                curRadioButton.UseVisualStyleBackColor = true;
                curRadioButton.Value = curListCode;
                curRadioButton.Click += new System.EventHandler( radioButton_Click );
                if ( parentEvent != null ) {
                    curRadioButton.Click += new System.EventHandler( parentEvent );
                }
                curItemLoc += 17;
                this.Controls.Add( curRadioButton );
            }
        }

        private void radioButton_Click( object sender, EventArgs e ) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            this.Tag = (String)myRadio.Value;
        }

        public String CurrentValue {
            get {
                String myValue = "";
                RadioButtonWithValue myRadio = null;
                foreach ( Control myControl in this.Controls ) {
                    if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                        myRadio = (RadioButtonWithValue)myControl;
                        if ( myRadio.Checked == true ) {
                            if (( (String)myRadio.Value ).Length > 1) {
                                myValue = ((String)myRadio.Value).Substring(0, 2);
                            } else {
                                myValue = (String)myRadio.Value;
                            }
                            myRadio.ForeColor = Color.DarkBlue;
                            myRadio.BackColor = Color.White;
                        } else {
                            myRadio.ForeColor = Color.Black;
                            myRadio.BackColor = Color.Silver;
                        }
                    }
                }
                return myValue;
            }

            set {
                String rbValue = null;
                RadioButtonWithValue myRadio = null;
                foreach ( Control myControl in this.Controls ) {
                    if ( myControl.GetType() == typeof( RadioButtonWithValue ) ) {
                        myRadio = (RadioButtonWithValue)myControl;
                        rbValue = (String)myRadio.Value;
                        if ( rbValue.Substring(0,2).Equals( value ) ) {
                            myRadio.Checked = true;
                            myRadio.ForeColor = Color.DarkBlue;
                            myRadio.BackColor = Color.White;
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

        public Boolean isDivisionIntl(String inAgeGroup) {
            return myAgeGroupDropdownList.isDivisionIntl( inAgeGroup );
        }
    }
}
