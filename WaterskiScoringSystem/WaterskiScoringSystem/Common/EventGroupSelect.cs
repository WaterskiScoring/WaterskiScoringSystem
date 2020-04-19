using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    public partial class EventGroupSelect : Form {
        private String myEventGroup = "";

        public EventGroupSelect() {
            InitializeComponent();
        }

        private void EventGroupSelect_Load( object sender, EventArgs e ) {
        }

        public void EventGroupListLoad( String inSanctionId, String inEvent ) {
            String curValue = "All";
            int curItemPosY = okButton.Location.Y + okButton.Size.Height + 5;
            int curItemPosX = cancelButton.Location.X - 15;
            int curItemSize = 0;

            RadioButtonWithValue curRadioButton = new RadioButtonWithValue();
            curRadioButton.AutoSize = true;
            curRadioButton.Location = new System.Drawing.Point( curItemPosX, curItemPosY );
            curRadioButton.Name = curValue;
            curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
            curRadioButton.TabIndex = 0;
            curRadioButton.TabStop = true;
            curRadioButton.Text = curValue;
            curRadioButton.UseVisualStyleBackColor = true;
            curRadioButton.Value = "All";
            curItemPosY += 17;
            curRadioButton.Click += new System.EventHandler( radioButton_Click );
            this.Controls.Add( curRadioButton );
            
            String curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
                + "WHERE SanctionId = '" + inSanctionId + "' "
                + "And Event = '" + inEvent + "' "
                + "Order by EventGroup";
            DataTable curDataTable = getData( curSqlStmt );
            foreach ( DataRow curRow in curDataTable.Rows ) {
                curValue = (String)curRow["EventGroup"];
                curItemSize = curValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point( curItemPosX, curItemPosY );
                curRadioButton.Name = curValue;
                curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
                curRadioButton.TabIndex = 0;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curValue;
                curRadioButton.UseVisualStyleBackColor = true;
                curRadioButton.Value = curValue;
                curItemPosY += 17;
                curRadioButton.Click += new System.EventHandler( radioButton_Click );
                this.Controls.Add( curRadioButton );
            }
        }

        public String EventGroup {
            get {
                return myEventGroup;
            }
            set {
                myEventGroup = value;
            }
        }

        private void radioButton_Click( object sender, EventArgs e ) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            myEventGroup = (String)myRadio.Value;
        }


        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private bool isObjectEmpty( object inObject ) {
            bool curReturnValue = false;
            if ( inObject == null ) {
                curReturnValue = true;
            } else if ( inObject == System.DBNull.Value ) {
                curReturnValue = true;
            } else if ( inObject.ToString().Length > 0 ) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

    }
}
