using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    class ListSkierClass {
        private ArrayList myDropdownList = new ArrayList();
        private DataTable myDataTable = null;
        private DataTable myTourClassDataTable = null;
        private DataTable myClassToEventDataTable = null;

        public ListSkierClass() {
        }

        public void ListSkierClassLoad () {
			myDataTable = getClassList();

			String curListCode, curCodeValue;
            foreach (DataRow curRow in myDataTable.Rows) {
                curListCode = curRow["ListCode"].ToString();
                curCodeValue = curRow["CodeValue"].ToString();
                myDropdownList.Add( new ListItem( curCodeValue, curListCode ) );
            }
        }

        public DataTable SkierClassDataTable {
            get {
                return myDataTable;
            }
        }

        public ArrayList DropdownList {
            get {
                return myDropdownList;
            }
        }

        public Int16 compareClassChange( String inSkierClass, String inTourClass ) {
            Int16 curReturnValue = 0;
            Decimal curTourLevel = 0, curClassLevel = 0;
            DataRow curTourRow, curClassRow;
            DataRow[] findRows;

            if (myTourClassDataTable == null) {
                myTourClassDataTable = getTourClassList();
            }

            if (inSkierClass.ToLower().Equals(inTourClass.ToLower())) {
                return curReturnValue;
            } else {
                findRows = myDataTable.Select( "ListCode = '" + inSkierClass + "'" );
                if ( findRows.Length > 0 ) {
                    curClassRow = findRows[0];
                    curClassLevel = (Decimal)curClassRow["ListCodeNum"];
                }

                findRows = myTourClassDataTable.Select("ListCode = '" + inTourClass + "'");
                if ( findRows.Length > 0 ) {
                    curTourRow = findRows[0];
                    curTourLevel = (Decimal)curTourRow["ListCodeNum"];
                }

                if ( curTourLevel == curClassLevel ) {
                    curReturnValue = 0;
                } else if ( curTourLevel < curClassLevel ) {
                    curReturnValue -= 1;
                } else if ( curTourLevel > curClassLevel ) {
                    curReturnValue += 1;
                }
            }
            return curReturnValue;
        }

        public String getSkierTourEventClass(String inValue) {
            String curReturnValue = "";
            DataRow[] findRows;

            if (myClassToEventDataTable == null) {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT ListCode, SortSeq, CodeValue, CodeDesc" );
                curSqlStmt.Append( " FROM CodeValueList" );
                curSqlStmt.Append( " WHERE ListName = 'ClassToEvent'" );
                curSqlStmt.Append( " ORDER BY SortSeq" );
                myClassToEventDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            }
            if (myClassToEventDataTable.Rows.Count == 0) {
                curReturnValue = "C";
            } else {
                findRows = myClassToEventDataTable.Select( "ListCode = '" + inValue + "'" );
                if (findRows.Length > 0) {
                    curReturnValue = (String)findRows[0]["CodeValue"];
                } else {
                    curReturnValue = "C";
                }
            }
            return curReturnValue;
        }

        private DataTable getClassList() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCodeNum, ListCode, SortSeq, CodeValue ");
            curSqlStmt.Append("FROM CodeValueList ");
            curSqlStmt.Append("WHERE ListName = 'Class' ");
            curSqlStmt.Append("Order by SortSeq ");
            return DataAccess.getDataTable( curSqlStmt.ToString());
        }

        public DataTable getTourClassList() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCodeNum, ListCode, SortSeq, CodeValue ");
            curSqlStmt.Append("FROM CodeValueList ");
            curSqlStmt.Append("WHERE ListName = 'ClassTour' ");
            curSqlStmt.Append("Order by SortSeq ");
            return DataAccess.getDataTable( curSqlStmt.ToString());
        }

    }
}
