using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace WaterskiScoringSystem.Common {
    class RuleDropdownList {
        private ArrayList myDropdownList = new ArrayList();

        //Class instantiation method
        public RuleDropdownList() {
            // Loads data 
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, CodeValue FROM CodeValueList WHERE ListName = 'Rules' ORDER BY SortSeq" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );

            String curListCode, curCodeValue;
            foreach (DataRow curRow in curDataTable.Rows) {
                curListCode = (String)curRow["ListCode"];
                curCodeValue = (String)curRow["CodeValue"];
                myDropdownList.Add( new ListItem( curCodeValue, curListCode ) );
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

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
