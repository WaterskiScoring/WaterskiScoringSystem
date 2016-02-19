using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterskiScoringSystem.Common {
    /* 
     * Class definition for a dropdown list of values with display descriptions
     */
    public class ListItem {
        private string myName;
        private string myValue;
        private int myValueNum;
        private decimal myValueDec;

        public ListItem() {
        }
        public ListItem(string inName, string inValue) {
            this.myName = inName;
            this.myValue = inValue;
        }
        public ListItem(string inName, int inValue) {
            this.myName = inName;
            this.myValueNum = inValue;
            this.myValue = inValue.ToString();
        }
        public ListItem(string inName, decimal inValue) {
            this.myName = inName;
            this.myValueDec = inValue;
            this.myValue = inValue.ToString();
        }

        public string ItemName {
            get {
                return myName;
            }
        }

        public string ItemValue {
            get {
                return myValue;
            }
        }
        public int ItemValueNum {
            get {
                return myValueNum;
            }
        }
        public decimal ItemValueDec {
            get {
                return myValueDec;
            }
        }

        public override string ToString() {
            return this.ItemValue + " - " + this.ItemName;
        }
    }
}
