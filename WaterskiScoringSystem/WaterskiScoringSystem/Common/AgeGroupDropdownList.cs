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
    class AgeGroupDropdownList {
        private ArrayList myDropdownList = new ArrayList();
        private DataTable myDataTable;
        private DataTable myIwwfDivDataTable;
        private String myRuleType = "all";
        private String myTourClass = "";
        private String myFed = "";
        private DataRow myClassERow = null;
        private DataRow myTourClassRow = null;

        //Class instantiation method
        public AgeGroupDropdownList() {
            // Loads data 
            //AgeGroupDropdownList_Load( "all" );
        }

        //Class instantiation method
        public AgeGroupDropdownList( DataRow inTourRow ) {
            AgeGroupDropdownList_Load( inTourRow );
        }

        private void AgeGroupDropdownList_Load( DataRow inTourRow ) {
            // Loads data 
            bool isIntlTour = false;
            myRuleType = "all";
            myTourClass = "";
            myFed = "";
            DataRow[] curFindRow;
            StringBuilder curSelectStmt = new StringBuilder( "" );
            StringBuilder curWhereStmt = new StringBuilder( "" );

            if ( inTourRow != null ) {
                myRuleType = (String)inTourRow["Rules"];
                myTourClass = (String)inTourRow["Class"];
                myFed = (String)inTourRow["Federation"];

                curSelectStmt.Append( "SELECT ListCode, CodeValue, ListCodeNum, SortSeq FROM CodeValueList WHERE ListName = 'ClassTour' ORDER BY SortSeq" );
                DataTable curTourClassDataTable = getData( curSelectStmt.ToString() );
                curFindRow = curTourClassDataTable.Select( "ListCode = 'E'" );
                if ( curFindRow.Length > 0 ) {
                    myClassERow = curFindRow[0];
                    curFindRow = curTourClassDataTable.Select( "ListCode = '" + myTourClass + "'" );
                    if ( curFindRow.Length > 0 ) {
                        myTourClassRow = curFindRow[0];
                        if ( (Decimal)myClassERow["ListCodeNum"] < (Decimal)myTourClassRow["ListCodeNum"] ) {
                            isIntlTour = true;
                        }
                    }
                }
            }

            curSelectStmt = new StringBuilder( "" );
            curSelectStmt.Append( "SELECT Distinct ListCode as Division, CodeValue as DivisionName");
            curSelectStmt.Append( ", MinValue as AgeBegin, MaxValue as AgeEnd, SortSeq ");
            curSelectStmt.Append( "FROM CodeValueList " );
            if ( myRuleType.ToLower().Equals( "awsa" ) ) {
                if ( isIntlTour ) {
                    curWhereStmt.Append( "WHERE ListName LIKE '%AgeGroup' ");
                    curWhereStmt.Append( "  AND ListName != 'NcwsaAgeGroup' " );
                    //curWhereStmt.Append( "WHERE ListName in ('AWSAAgeGroup', 'IwwfAgeGroup') ");
                    curWhereStmt.Append( "Order by SortSeq, CodeValue " );
                } else {
                    curWhereStmt.Append( "WHERE ListName = 'AWSAAgeGroup' " );
                    curWhereStmt.Append( "Order by SortSeq, CodeValue " );
                }
            } else if ( myRuleType.ToLower().Equals( "iwwf" ) ) {
                curWhereStmt.Append( "WHERE ListName = 'IwwfAgeGroup' " );
                curWhereStmt.Append( "Order by SortSeq, CodeValue " );
            } else if ( myRuleType.ToLower().Equals( "can" ) ) {
                curWhereStmt.Append( "WHERE ListName = 'CWSAAgeGroup' ");
                curWhereStmt.Append( "Order by SortSeq, CodeValue " );
            } else if (myRuleType.ToLower().Equals( "awwf" )) {
                curWhereStmt.Append( "WHERE ListName = 'AWWFAgeGroup' " );
                curWhereStmt.Append( "Order by SortSeq, CodeValue " );
            } else if (myRuleType.ToLower().Equals( "ncwsa" )) {
                if (isIntlTour) {
                    curWhereStmt.Append( "WHERE ListName LIKE '%AgeGroup' " );
                    //curWhereStmt.Append( "WHERE ListName in ('AWSAAgeGroup', 'IwwfAgeGroup', 'NcwsaAgeGroup') " );
                    curWhereStmt.Append( "Order by SortSeq, CodeValue " );
                } else {
                    curWhereStmt.Append( "WHERE ListName in ('AWSAAgeGroup', 'NcwsaAgeGroup') ");
                    curWhereStmt.Append( "Order by SortSeq, CodeValue " );
                }
            } else {
                curWhereStmt.Append( "WHERE ListName LIKE '%AgeGroup' " );
                curWhereStmt.Append( "Order by SortSeq, CodeValue " );
            }
            myDataTable = getData( curSelectStmt.ToString() + curWhereStmt.ToString() );
            if ( myDataTable == null ) {
                curWhereStmt = new StringBuilder( "WHERE ListName LIKE '%AgeGroup' " );
                //curWhereStmt = new StringBuilder( "WHERE ListName in ('AWSAAgeGroup', 'IwwfAgeGroup', 'NcwsaAgeGroup') " );
                curWhereStmt.Append( "Order by SortSeq, CodeValue " );
                myDataTable = getData( curSelectStmt.ToString() + curWhereStmt.ToString() );
            } else {
                if ( myDataTable.Rows.Count == 0 ) {
                    curWhereStmt = new StringBuilder( "WHERE ListName LIKE '%AgeGroup' " );
                    //curWhereStmt = new StringBuilder( " WHERE ListName in ('AWSAAgeGroup', 'IwwfAgeGroup', 'NcwsaAgeGroup') " );
                    curWhereStmt.Append( "Order by SortSeq, CodeValue " );
                    myDataTable = getData( curSelectStmt.ToString() + curWhereStmt.ToString() );
                }
            }

            String curListCode, curCodeValue;
            foreach ( DataRow curRow in myDataTable.Rows ) {
                curListCode = (String)curRow["Division"];
                curCodeValue = (String)curRow["DivisionName"];
                //myDropdownList.Add( new ListItem( curCodeValue, curListCode ) );
                if (myDropdownList.Contains( curListCode )) {
                } else {
                    myDropdownList.Add( curListCode );
                }
            }

            myIwwfDivDataTable = null;
            if ( (myRuleType.ToLower().Equals( "awsa" ) && isIntlTour)
                || myRuleType.ToLower().Equals( "iwwf" ) ) {
                curSelectStmt = new StringBuilder( "" );
                curSelectStmt.Append( "SELECT Distinct ListCode as Division, CodeValue as DivisionName" );
                curSelectStmt.Append( ", MinValue as AgeBegin, MaxValue as AgeEnd, SortSeq " );
                curSelectStmt.Append( "FROM CodeValueList " );
                curSelectStmt.Append( "WHERE ListName = 'IwwfAgeGroup' " );
                curSelectStmt.Append( "Order by SortSeq, CodeValue " );
                myIwwfDivDataTable = getData( curSelectStmt.ToString() );
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

        public DataTable AgeDivDataTable {
            get {
                return myDataTable;
            }
        }

        public bool validAgeDiv( String inAgeDiv ) {
            bool isValid = false;
            DataRow[] findRows = myDataTable.Select( "Division = '" + inAgeDiv.ToUpper() + "'" );
            if ( findRows.Length > 0 ) { isValid = true; }
            return isValid;
        }

        public Boolean isDivisionIntl(String inAgeGroup) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct ListCode as Division, CodeValue as DivisionName" );
            curSqlStmt.Append( ", MinValue as AgeBegin, MaxValue as AgeEnd, SortSeq ");
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append("WHERE ListName = 'IwwfAgeGroup' AND ListCode = '" + inAgeGroup + "'");
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable == null) {
                return false;
            } else {
                if (curDataTable.Rows.Count > 0) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        public int getAgeDivIndex( String inAgeDiv ) {
            int returnIndex = 0, curIdx = 0;
            foreach ( DataRow curRow in myDataTable.Rows ) {
                if ( curRow["Division"].ToString().ToUpper() == inAgeDiv.ToUpper() ) {
                    returnIndex = curIdx;
                    break;
                } else {
                    curIdx++;
                }
            }
            return returnIndex;
        }

        public int getMinAgeForDiv( String inAgeDiv ) {
            int returnValue = 0;
            DataRow[] findRows = myDataTable.Select("Division = '" + inAgeDiv.ToUpper() + "'");
            if ( findRows.Length > 0 ) {
                returnValue = (int)(Decimal) findRows[0]["AgeBegin"];
            }
            return returnValue;
        }

        public String getGenderOfAgeDiv( String inAgeDiv ) {
            String curGender = "";
            DataRow[] findRows = myDataTable.Select( "Division = '" + inAgeDiv + "'" );
            if (findRows.Length == 0) {
                StringBuilder curSelectStmt = new StringBuilder( "" );
                curSelectStmt.Append( "SELECT Distinct ListCode as Division, CodeValue as DivisionName" );
                curSelectStmt.Append( " , MinValue as AgeBegin, MaxValue as AgeEnd, SortSeq " );
                curSelectStmt.Append( "FROM CodeValueList " );
                curSelectStmt.Append( "WHERE ListName in ('AWSAAgeGroup', 'IwwfAgeGroup', 'CWSAAgeGroup', 'NcwsaAgeGroup') " );
                curSelectStmt.Append( "  And ListCode = '" + inAgeDiv + "' " );
                curSelectStmt.Append( "Order by SortSeq, CodeValue " );
                DataTable curDataTable = getData( curSelectStmt.ToString() );
                findRows = myDataTable.Select( "Division = '" + inAgeDiv + "'" );
            }
            if (findRows.Length > 0) {
                String curDivName = (String)findRows[0]["DivisionName"];
                if ( curDivName.ToLower().IndexOf( "women" ) > -1 ) {
                    curGender = "F";
                } else if ( curDivName.ToLower().IndexOf( "girl" ) > -1 ) {
                    curGender = "F";
                } else if ( curDivName.ToLower().IndexOf( "men" ) > -1 ) {
                    curGender = "M";
                } else if (curDivName.ToLower().IndexOf( "boy" ) > -1) {
                    curGender = "M";
                } else {
                    curGender = "";
                }
            }

            return curGender;
        }

        public ArrayList getDivListForGender(String inGender) {
            ArrayList curDropdownList = new ArrayList();
            String curListCode, curCodeValue;
            foreach (DataRow curRow in myDataTable.Rows) {
                if (inGender == getGenderOfAgeDiv( (String)curRow["Division"] )) {
                    curListCode = (String)curRow["Division"];
                    curCodeValue = (String)curRow["DivisionName"];
                    curDropdownList.Add( curListCode + " (" + curCodeValue + ")" );
                } else {
                    if (( (String)curRow["Division"] ).ToString().Equals( "OF" )) {
                        curListCode = (String)curRow["Division"];
                        curCodeValue = (String)curRow["DivisionName"];
                        curDropdownList.Add( curListCode + " (" + curCodeValue + ")" );
                    }
                }
            }
            return curDropdownList;
        }

        public ArrayList getDivListForAge(Int16 inAge, String inGender) {
            ArrayList curDropdownList = new ArrayList();
            String curListCode, curCodeValue;
            foreach ( DataRow curRow in myDataTable.Rows ) {
                if ( inAge >= (Decimal)curRow[ "AgeBegin" ] && inAge <= (Decimal)curRow[ "AgeEnd" ] ) {
                    if ( inGender == getGenderOfAgeDiv( (String)curRow["Division"] ) ) {
                        curListCode = (String)curRow["Division"];
                        curCodeValue = (String)curRow["DivisionName"];
                        curDropdownList.Add( curListCode + " (" + curCodeValue + ")" );
                    } else {
                        if ( ( (String)curRow["Division"] ).ToString().Equals( "OF" ) ) {
                            curListCode = (String)curRow["Division"];
                            curCodeValue = (String)curRow["DivisionName"];
                            curDropdownList.Add( curListCode + " (" + curCodeValue + ")" );
                        }
                    }
                }
            }
            return curDropdownList;
        }

        public ArrayList getComparableDivListForAge( String inDiv, Int16 inAge, String inGender, String inEvent ) {
            ArrayList curDropdownList = new ArrayList();

            Int16 curMaxSpeed = 0;
            Decimal curRampMax = 0;
            if ( inEvent.Equals("Slalom") ) {
                DataTable curDataTable = getMaxSlalomSpeedData(inDiv);
                if ( curDataTable.Rows.Count > 0 ) {
                    curMaxSpeed = Convert.ToInt16((Decimal) curDataTable.Rows[0]["MaxValue"]);
                }
            }
            if ( inEvent.Equals("Jump") ) {
                DataTable curDataTable = getMaxJumpSpeedData(inDiv);
                if ( curDataTable.Rows.Count > 0 ) {
                    curMaxSpeed = Convert.ToInt16((Decimal) curDataTable.Rows[0]["MaxValue"]);
                }
                curDataTable = getRampMaxData(inDiv);
                if ( curDataTable.Rows.Count > 0 ) {
                    curRampMax = (Decimal) curDataTable.Rows[0]["MaxValue"];
                }
            }

            // Select for age groups that are comparable to the currently specified division 
            bool isIntlTour = false;
            StringBuilder curSqlStmt = new StringBuilder("");

            curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT AG.ListName, AG.ListCode as Division, AG.CodeValue as DivisionName");
            curSqlStmt.Append(", AG.MinValue as AgeBegin, AG.MaxValue as AgeEnd, AG.SortSeq ");
            if ( inEvent.Equals("Slalom") ) {
                curSqlStmt.Append(", SP.MaxValue as KPH, SP.MinValue as MPH ");
            }
            if ( inEvent.Equals("Jump") ) {
                curSqlStmt.Append(", SP.MaxValue as KPH, SP.MinValue as MPH ");
                curSqlStmt.Append(", RM.MaxValue as RampHeight, RM.MinValue as RampAngle ");
            }
            curSqlStmt.Append("FROM CodeValueList AG ");
            if ( inEvent.Equals("Slalom") ) {
                curSqlStmt.Append("INNER JOIN CodeValueList SP ON SP.ListName like '%SlalomMax' AND SP.ListCode = AG.ListCode ");
            }
            if ( inEvent.Equals("Jump") ) {
                curSqlStmt.Append("INNER JOIN CodeValueList SP ON SP.ListName like '%JumpMax' AND SP.ListCode = AG.ListCode ");
                curSqlStmt.Append("INNER JOIN CodeValueList RM ON RM.ListName like '%RampMax' AND RM.ListCode = AG.ListCode ");
            }
            curSqlStmt.Append("Where " + inAge + " >= AG.MinValue ");
            curSqlStmt.Append("And " + inAge + " <= AG.MaxValue ");
            if ( inEvent.Equals("Slalom") ) {
                curSqlStmt.Append("And " + curMaxSpeed + " = SP.MaxValue ");
            }
            if ( inEvent.Equals("Jump") ) {
                curSqlStmt.Append("And " + curMaxSpeed + " = SP.MaxValue ");
                curSqlStmt.Append("And " + curRampMax + " = RM.MaxValue ");
            }
            if ( myRuleType.ToLower().Equals("awsa") ) {
                if ( isIntlTour ) {
                    curSqlStmt.Append("And AG.ListName LIKE '%AgeGroup' ");
                    curSqlStmt.Append("And AG.ListName != 'NcwsaAgeGroup' ");
                    curSqlStmt.Append("Order by AG.SortSeq, AG.CodeValue ");
                } else {
                    curSqlStmt.Append("And AG.ListName = 'AWSAAgeGroup' ");
                    curSqlStmt.Append("Order by AG.SortSeq, AG.CodeValue ");
                }
            } else if ( myRuleType.ToLower().Equals("iwwf") ) {
                curSqlStmt.Append("And AG.ListName = 'IwwfAgeGroup' ");
                curSqlStmt.Append("Order by AG.SortSeq, AG.CodeValue ");
            } else if ( myRuleType.ToLower().Equals("can") ) {
                curSqlStmt.Append("And AG.ListName = 'CWSAAgeGroup' ");
                curSqlStmt.Append("Order by AG.SortSeq, AG.CodeValue ");
            } else if ( myRuleType.ToLower().Equals("awwf") ) {
                curSqlStmt.Append("And AG.ListName = 'AWWFAgeGroup' ");
                curSqlStmt.Append("Order by AG.SortSeq, AG.CodeValue ");
            } else if ( myRuleType.ToLower().Equals("ncwsa") ) {
                if ( isIntlTour ) {
                    curSqlStmt.Append("And AG.ListName LIKE '%AgeGroup' ");
                    curSqlStmt.Append("Order by AG.SortSeq, AG.CodeValue ");
                } else {
                    curSqlStmt.Append("And AG.ListName in ('AWSAAgeGroup', 'NcwsaAgeGroup') ");
                    curSqlStmt.Append("Order by AG.SortSeq, AG.CodeValue ");
                }
            } else {
                curSqlStmt.Append("And AG.ListName LIKE '%AgeGroup' ");
                curSqlStmt.Append("Order by AG.SortSeq, AG.CodeValue ");
            }

            DataTable curDivDataTable = getData(curSqlStmt.ToString());

            String curListCode, curCodeValue;
            foreach ( DataRow curRow in curDivDataTable.Rows ) {
                if ( ( (String) curRow["Division"] ).Equals(inDiv) ) {
                    //Bypass if primary division
                } else if ( inGender == getGenderOfAgeDiv((String) curRow["Division"]) ) {
                    curListCode = (String) curRow["Division"];
                    curCodeValue = (String) curRow["DivisionName"];
                    curDropdownList.Add(curListCode + " (" + curCodeValue + ")");
                }
            }
            return curDropdownList;
        }

        public ArrayList getDivListForAgeIwwf( Int16 inAge, String inDiv ) {
            ArrayList curDropdownList = new ArrayList();
            if (myIwwfDivDataTable != null) {
                String curGender = getGenderOfAgeDiv( inDiv );

                String curListCode, curCodeValue;
                foreach (DataRow curRow in myIwwfDivDataTable.Rows) {
                    if (inAge >= (Decimal)curRow["AgeBegin"] && inAge <= (Decimal)curRow["AgeEnd"]) {
                        if (curGender == getGenderOfAgeDiv( (String)curRow["Division"] )) {
                            curListCode = (String)curRow["Division"];
                            curCodeValue = (String)curRow["DivisionName"];
                            curDropdownList.Add( curListCode + " (" + curCodeValue + ")" );
                        } else {
                            if (( (String)curRow["Division"] ).ToString().Equals( "OF" )) {
                                curListCode = (String)curRow["Division"];
                                curCodeValue = (String)curRow["DivisionName"];
                                curDropdownList.Add( curListCode + " (" + curCodeValue + ")" );
                            }
                        }
                    }
                }
            }
            return curDropdownList;
        }

        private DataTable getMaxSlalomSpeedData( String inAgeGroup ) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue");
            curSqlStmt.Append(" FROM CodeValueList");
            curSqlStmt.Append(" WHERE (ListName like '%SlalomMax' AND ListCode = '" + inAgeGroup + "')");
            curSqlStmt.Append(" ORDER BY SortSeq");
            return getData(curSqlStmt.ToString());
        }

        private DataTable getMaxJumpSpeedData( String inAgeGroup ) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue");
            curSqlStmt.Append(" FROM CodeValueList");
            curSqlStmt.Append(" WHERE (ListName like '%JumpMax' AND ListCode = '" + inAgeGroup + "')");
            curSqlStmt.Append(" ORDER BY SortSeq");
            return getData(curSqlStmt.ToString());
        }

        private DataTable getRampMaxData( String inAgeGroup ) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue");
            curSqlStmt.Append(" FROM CodeValueList");
            curSqlStmt.Append(" WHERE (ListName like '%RampMax' AND ListCode = '" + inAgeGroup + "')");
            curSqlStmt.Append(" ORDER BY SortSeq");
            return getData(curSqlStmt.ToString());
        }


        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
