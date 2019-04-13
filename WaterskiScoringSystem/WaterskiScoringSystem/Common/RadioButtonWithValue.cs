using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem {
    class RadioButtonWithValue : RadioButton {
        private object myValue;
        
        public object Value {
            get { return myValue; }
            set { myValue = value; }
        }

        private void InitializeComponent() {
			this.SuspendLayout();
			// 
			// RadioButtonWithValue
			// 
			this.Font = new System.Drawing.Font("Arial Narrow", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			//this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.ForeColor = Color.DarkOrange;
			this.ResumeLayout(false);

        }

    }
}
