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
    class FedDropdownList {
        private ArrayList myDropdownList = new ArrayList();
        private DataTable myDataTable;

        //Class instantiation method
        public FedDropdownList() {
            // Loads data 
            String curSqlStmt = "SELECT ListCode, CodeValue FROM CodeValueList WHERE ListName = 'Federation ' ORDER BY SortSeq";
            myDataTable = getData( curSqlStmt );

            String curListCode, curCodeValue;
            foreach ( DataRow curRow in myDataTable.Rows ) {
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

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
