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
            this.ResumeLayout(false);

        }

    }
}
