﻿using System;
using System.Collections;
using System.Data;
using System.Text;

namespace WaterskiScoringSystem.Common {
    class ListTourClass {
        private ArrayList myDropdownList = new ArrayList();
        private DataTable myDataTable = null;

        public ListTourClass() {
        }

        public void ListTourClassLoad() {
            myDataTable = getClassList();

            String curListCode, curCodeValue;
            foreach (DataRow curRow in myDataTable.Rows) {
                curListCode = curRow["ListCode"].ToString();
                curCodeValue = curRow["CodeValue"].ToString();
                myDropdownList.Add(new ListItem(curCodeValue, curListCode));
            }
        }

        public DataTable TourClassDataTable {
            get {
                return myDataTable;
            }
        }

        public ArrayList DropdownList {
            get {
                return myDropdownList;
            }
        }

        public Int16 compareClassChange(String inTourClass, String inClass) {
            Int16 curReturnValue = 0;
            Decimal curTourLevel = 0, curClassLevel = 0;
            DataRow curTourRow, curClassRow;
            DataRow[] findRows;

            if (inTourClass.ToLower().Equals(inClass.ToLower())) {
                return curReturnValue;
            } else {
                findRows = myDataTable.Select("ListCode = '" + inTourClass + "'");
                if (findRows.Length > 0) {
                    curTourRow = findRows[0];
                    curTourLevel = (Decimal)curTourRow["ListCodeNum"];
                }

                findRows = myDataTable.Select("ListCode = '" + inClass + "'");
                if (findRows.Length > 0) {
                    curClassRow = findRows[0];
                    curClassLevel = (Decimal)curClassRow["ListCodeNum"];
                }

                if (curTourLevel == curClassLevel) {
                    curReturnValue = 0;
                } else if (curTourLevel < curClassLevel) {
                    curReturnValue -= 1;
                } else if (curTourLevel > curClassLevel) {
                    curReturnValue += 1;
                }
            }
            return curReturnValue;
        }

        private DataTable getClassList() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCodeNum, ListCode, SortSeq, CodeValue ");
            curSqlStmt.Append("FROM CodeValueList ");
            curSqlStmt.Append("WHERE ListName = 'ClassTour' ");
            curSqlStmt.Append("Order by SortSeq ");
            return DataAccess.getDataTable( curSqlStmt.ToString());
        }

    }
}
