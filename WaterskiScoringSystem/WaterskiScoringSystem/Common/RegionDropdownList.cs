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
    class RegionDropdownList {
        private ArrayList myDropdownList = new ArrayList();
        private DataTable myDataTable;

        //Class instantiation method
        public RegionDropdownList() {
            // Loads data 
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT CodeValue FROM CodeValueList ");
            curSqlStmt.Append("WHERE ListName = 'StateRegion' ");
            curSqlStmt.Append("Group BY CodeValue");
            myDataTable = getData(curSqlStmt.ToString());

            String curListCode, curCodeValue;
            curListCode = "All";
            curCodeValue = "All";
            myDropdownList.Add(new ListItem(curCodeValue, curListCode));

            foreach ( DataRow curRow in myDataTable.Rows ) {
                curListCode = (String) curRow["CodeValue"];
                curCodeValue = (String) curRow["CodeValue"];
                myDropdownList.Add(new ListItem(curCodeValue, curListCode));
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
