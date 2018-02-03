using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data.SqlServerCe;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace WaterskiScoringSystem.Common {
    class StateDropdownList {
        private ArrayList myDropdownList = new ArrayList();
        private DataTable myDataTable;
        private String myRegion = "All";

        //Class instantiation method
        public StateDropdownList() {
            // Loads data 
            updateDropdownList();
        }

        private void updateDropdownList() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCode, CodeValue FROM CodeValueList ");
            curSqlStmt.Append("WHERE ListName = 'StateRegion' ");
            if ( myRegion.ToLower().Equals("all") || myRegion.Equals("") ) {
            } else {
                curSqlStmt.Append("  AND CodeValue = '" + myRegion + "' ");
            }
            curSqlStmt.Append("ORDER BY ListCode");
            myDataTable = getData(curSqlStmt.ToString());

            myDropdownList = new ArrayList();
            String curListCode, curCodeValue;
            foreach ( DataRow curRow in myDataTable.Rows ) {
                curListCode = (String) curRow["ListCode"];
                curCodeValue = (String) curRow["ListCode"];
                myDropdownList.Add(new ListItem(curCodeValue, curListCode));
            }

        }

        public String Region {
            get {
                return myRegion;
            }
            set {
                myRegion = value;
                updateDropdownList();
            }
        }

        public ArrayList DropdownList {
            get {
                return myDropdownList;
            }
            set {
                myDropdownList = value;
            }
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
