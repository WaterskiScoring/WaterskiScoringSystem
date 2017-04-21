using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace ValidationLibrary.Trick {
    public class Validation {
        private String myTourRules = "";
        private String myValidationMessage = "";
        private String myUpdatedTrickCode = "";
        private Int16 myNumSkis = -1;

        private DataRow myClassCRow = null;
        private DataRow myClassERow = null;

        private String[] myAllowedRepeatReverseList = { "RS", "RTS", "RB", "RF", "RTB", "RTF" };
        private ArrayList mySpecialFlipList = new ArrayList();

        //NumSkis, NumTurns, PK, Points, RuleCode, RuleNum, StartPos, TrickCode, TypeCode
        private DataTable myTrickListDataTable = null;
        //SELECT ListCodeNum, ListCode, SortSeq, CodeValue FROM CodeValueList WHERE ListName = 'Class'
        private DataTable mySkierClassDataTable = null;

        public Validation() {
            myValidationMessage = "";
        }

        public DataTable TrickListDataTable {
            get {
                return myTrickListDataTable;
            }
            set {
                myTrickListDataTable = value;

                mySpecialFlipList.Add("BFLB");
                mySpecialFlipList.Add("RBFLB");
                mySpecialFlipList.Add("BFLF");
                mySpecialFlipList.Add("RBFLF");
                mySpecialFlipList.Add("BFLLB");
                mySpecialFlipList.Add("RBFLLB");
                mySpecialFlipList.Add("FFLF");
                mySpecialFlipList.Add("FFLB");
            }
        }

        public DataTable SkierClassDataTable {
            get {
                return mySkierClassDataTable;
            }
            set {
                mySkierClassDataTable = value;

                myClassCRow = mySkierClassDataTable.Select("ListCode = 'C'")[0];
                myClassERow = mySkierClassDataTable.Select("ListCode = 'E'")[0];
            }
        }

        public String TourRules {
            get {
                return myTourRules;
            }
            set {
                myTourRules = value;
            }
        }

        public String ValidationMessage {
            get {
                return myValidationMessage;
            }
        }

        public Int16 NumSkis {
            get {
                return myNumSkis;
            }
        }

        public String UpdatedTrickCode {
            get {
                return myUpdatedTrickCode;
            }
        }
        
        /*
        Validate the number of skis field
        */
        public Int16 validateNumSkis( String inNumSkis ) {
            return validateNumSkis(inNumSkis, myTourRules);
        }
        public Int16 validateNumSkis(String inNumSkis, String inTourRules) {
            Int16 curNumSkis = -1;

            if ( inNumSkis.Length > 0 ) {
                if ( inNumSkis.Equals("1")
                    || inNumSkis.Equals("2")
                    ) {
                    Int16.TryParse(inNumSkis, out curNumSkis);
                } else if ( inTourRules.ToLower().Equals("ncwsa")
                    && ( inNumSkis.ToLower().Equals("wb")
                    || inNumSkis.ToLower().Equals("w")
                    || inNumSkis.ToLower().Equals("kb")
                    || inNumSkis.ToLower().Equals("k")
                    ) ) {
                    if ( inNumSkis.ToLower().Equals("wb") || inNumSkis.ToLower().Equals("w") ) {
                        curNumSkis = 0;
                    } else if ( inNumSkis.ToLower().Equals("kb") || inNumSkis.ToLower().Equals("k") ) {
                        curNumSkis = 9;
                    } else {
                        myValidationMessage = "Number of skis must equal 1 or 2 or (WB or KB for NCWSA)";
                    }
                } else {
                    myValidationMessage = "Number of skis must equal 1 or 2 or (WB or KB for NCWSA)";
                }
            }

            myNumSkis = curNumSkis;
            return curNumSkis;
        }

        /*
        Validate the results status field
        */
        public bool validateResultStatus( String inResults ) {
            return validateResultStatus(inResults, myTourRules);
        }
        public bool validateResultStatus( String inResults, String inTourRules ) {
            bool returnValue = true;
            if ( inResults.ToLower().Equals("credit")
                || inResults.ToLower().Equals("fall")
                || inResults.ToLower().Equals("no credit")
                || inResults.ToLower().Equals("before")
                || inResults.ToLower().Equals("ooc")
                || inResults.ToLower().Equals("repeat")
                || inResults.ToLower().Equals("end")
                || inResults.ToLower().Equals("unresolved")
                ) {
                returnValue = true;
            } else {
                myValidationMessage = String.Format("Trick result {0} is not valid", inResults);
                returnValue = false;
            }
            return returnValue;
        }

        /*
        Validate the trick code
        */
        public bool validateTrickCode( String inCode, Int16 inNumSkis, String inSkierClass, DataRow[] prevTrickRows ) {
            bool returnValue = false;
            Int16 curNumTurns, curStartPos, curTypeCodeValue, curRuleNum;
            Int16 prevStartPos, prevNumTurns, prevTypeCodeValue, prevRuleNum;
            String curCode, prevCode;
            myUpdatedTrickCode = "";
            int prevTrickIdx = 0;
            DataRow curTrickRow;

            DataRow curSkierClassRow = mySkierClassDataTable.Select("ListCode = '" + inSkierClass + "'")[0];

            if ( inCode != null && inCode.Length > 0 ) {
                if ( inCode.ToUpper().Equals("R") || inCode.Substring(0, 1).ToUpper().Equals("R") ) {
                    #region Determine actual trick code for reverse code

                    if ( prevTrickRows == null || prevTrickRows.Length == 0) {
                        myValidationMessage = String.Format("Reverse code not logical as first trick in a pass"
                            + "\n Trick Code {0} on {1} ski(s) is not valid"
                            , inCode, inNumSkis.ToString());
                        returnValue = false;
                    } else {
                        prevTrickIdx = 0;
                        prevCode = (String) prevTrickRows[prevTrickIdx]["TrickCode"];
                        prevStartPos = (byte) prevTrickRows[prevTrickIdx]["StartPos"];
                        prevNumTurns = (byte) prevTrickRows[prevTrickIdx]["NumTurns"];
                        prevTypeCodeValue = (byte) prevTrickRows[prevTrickIdx]["TypeCode"];
                        prevRuleNum = (Int16) prevTrickRows[prevTrickIdx]["RuleNum"];

                        //Check to determine if the previous number of turns in the trick are an odd or even number indicating starting and stoping position
                        if ( ( prevNumTurns % 2 ) == 0 ) {
                            #region When previous trick has an even number of turns, will need to check previous trick
                            if ( inCode.Length > 1 ) {
                                curCode = inCode;
                            } else {
                                if ( prevCode.Substring(0, 1).Equals("R") ) {
                                    if ( myAllowedRepeatReverseList.Contains(prevCode) ) {
                                        curCode = prevCode;
                                    } else {
                                        curCode = inCode + prevCode;
                                    }
                                } else {
                                    curCode = inCode + prevCode;
                                }
                            }
                            curTrickRow = getTrickRow(curCode, inNumSkis, inSkierClass);
                            if ( curTrickRow == null ) {
                                myValidationMessage = String.Format("Invalid trick code for number of skis"
                                    + "\n Trick Code {0} on {1} ski(s) is not valid"
                                    , inCode, inNumSkis.ToString());
                                returnValue = false;
                            } else {
                                //Check to ensure 360 multiples tricks has an appropriate starting position on the previous 2 tricks
                                curStartPos = (Byte)curTrickRow["StartPos"];
                                curNumTurns = (Byte)curTrickRow["NumTurns"];
                                curTypeCodeValue = (Byte)curTrickRow["TypeCode"];
                                if ( curStartPos == prevStartPos ) {
                                    myUpdatedTrickCode = curCode;
                                    returnValue = true;
                                } else {
                                    myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode);
                                    returnValue = false;
                                }
                            }
                            #endregion
                        } else {
                            #region When previous trick has an odd number of turns, will need to check 2nd previous tricks
                            if ( prevTrickRows.Length > 1 ) {
                                //Set to look at 2nd previous trick
                                prevTrickIdx++;
                                if ( inCode.Length > 1 ) {
                                    curCode = inCode;
                                } else {
                                    if ( (Decimal) curSkierClassRow["ListCodeNum"] > (Decimal) myClassERow["ListCodeNum"] ) {
                                        curCode = inCode + (String) prevTrickRows[prevTrickIdx]["TrickCode"];
                                    } else {
                                        prevCode = (String) prevTrickRows[prevTrickIdx]["TrickCode"];
                                        if ( prevCode.Substring(0, 1).Equals("R") ) {
                                            if ( myAllowedRepeatReverseList.Contains(prevCode) ) {
                                                curCode = prevCode;
                                            } else {
                                                curCode = inCode + prevCode;
                                            }
                                        } else {
                                            curCode = inCode + prevCode;
                                        }
                                    }
                                }
                                curTrickRow = getTrickRow(curCode, inNumSkis, inSkierClass);
                                if ( curTrickRow == null ) {
                                    myValidationMessage = String.Format("Invalid trick code for number of skis"
                                        + "\n Trick Code {0} on {1} ski(s) is not valid"
                                        , inCode, inNumSkis.ToString());
                                    returnValue = false;
                                } else {
                                    //Check to see if starting position of previous trick is appropriate
                                    curStartPos = (Byte) curTrickRow["StartPos"];
                                    curNumTurns = (Byte) curTrickRow["NumTurns"];
                                    curTypeCodeValue = (Byte) curTrickRow["TypeCode"];
                                    curRuleNum = (Int16) ( (Int16) curTrickRow["RuleNum"] + ( inNumSkis * 100 ) );

                                    if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
                                        if ( curStartPos == 0 ) {
                                            //Check to see if starting position of 2nd previous trick is also appropriate
                                            prevCode = (String) prevTrickRows[prevTrickIdx]["TrickCode"];
                                            prevStartPos = (byte) prevTrickRows[prevTrickIdx]["StartPos"];
                                            prevNumTurns = (byte) prevTrickRows[prevTrickIdx]["NumTurns"];
                                            if ( curStartPos == prevStartPos ) {
                                                myUpdatedTrickCode = curCode;
                                                returnValue = true;
                                            } else {
                                                myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode);
                                                returnValue = false;
                                            }
                                        } else {
                                            myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode);
                                            returnValue = false;
                                        }
                                    } else {
                                        if ( curStartPos == 0 ) {
                                            myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode);
                                            returnValue = false;
                                        } else {
                                            if ( curRuleNum == prevRuleNum ) {
                                                if ( curNumTurns > 2 ) {
                                                    myValidationMessage = "This type of reverse trick can not have more than 360 degrees";
                                                    returnValue = false;
                                                } else {
                                                    myUpdatedTrickCode = curCode;
                                                    returnValue = true;
                                                }
                                            } else {
                                                //Check to see if starting position of 2nd previous trick is also appropriate
                                                prevCode = (String) prevTrickRows[prevTrickIdx]["TrickCode"];
                                                prevStartPos = (byte) prevTrickRows[prevTrickIdx]["StartPos"];
                                                prevNumTurns = (byte) prevTrickRows[prevTrickIdx]["NumTurns"];
                                                if ( curStartPos == prevStartPos ) {
                                                    myUpdatedTrickCode = curCode;
                                                    returnValue = true;
                                                } else {
                                                    myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode);
                                                    returnValue = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            } else {
                                if ( inCode.Length > 1 ) {
                                    curCode = inCode;
                                    curTrickRow = getTrickRow(curCode, inNumSkis, inSkierClass);
                                    if ( curTrickRow == null ) {
                                        myValidationMessage = String.Format("Trick sequence is not possible"
                                            + "\n Use of reverse code {0} but second trick can't be a reverse of trick {1}"
                                            , curCode, prevCode);
                                        returnValue = false;
                                    } else {
                                        curStartPos = (Byte) curTrickRow["StartPos"];
                                        curNumTurns = (Byte) curTrickRow["NumTurns"];
                                        curTypeCodeValue = (Byte) curTrickRow["TypeCode"];
                                        curRuleNum = (Int16) ( (Int16) curTrickRow["RuleNum"] + ( inNumSkis * 100 ) );

                                        //Check to see if starting position of previous trick is appropriate
                                        if ( ( ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) && ( curStartPos == 0 ) )
                                            || ( ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 1 ) && ( curStartPos == 1 ) )
                                            ) {
                                            if ( curRuleNum == prevRuleNum ) {
                                                if ( curNumTurns > 2 ) {
                                                    myValidationMessage = "This type of reverse trick can not have more than 360 degrees";
                                                    returnValue = false;
                                                } else {
                                                    myUpdatedTrickCode = curCode;
                                                    returnValue = true;
                                                }
                                            } else {
                                                myValidationMessage = String.Format("Trick sequence is not possible"
                                                    + "\n Use of reverse code {0} but second trick can't be a reverse of trick {1}"
                                                    , curCode, prevCode);
                                                returnValue = false;
                                            }
                                        } else {
                                            myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", inCode, prevCode);
                                            returnValue = false;
                                        }
                                    }
                                } else {
                                    myValidationMessage = String.Format("Trick sequence is not possible"
                                        + "\n Use of reverse code {0} but second trick can't be a reverse of trick {1}"
                                        , inCode, prevCode);
                                    returnValue = false;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                } else {
                    #region Analyze trick code
                    curTrickRow = getTrickRow(inCode, inNumSkis, inSkierClass);
                    if ( curTrickRow == null ) {
                        myValidationMessage = String.Format("Invalid trick code for number of skis"
                            + "\n Trick Code {0} on {1} ski(s) is not valid"
                            , inCode, inNumSkis.ToString());
                        returnValue = false;
                    } else {
                        curStartPos = (Byte) curTrickRow["StartPos"];
                        curNumTurns = (Byte) curTrickRow["NumTurns"];
                        curTypeCodeValue = (Byte) curTrickRow["TypeCode"];

                        if ( prevTrickRows.Length > 0 ) {
                            if ( prevTrickRows.Length == 1
                                && inCode.ToUpper().Equals("FALL")
                                && ( (String) prevTrickRows[prevTrickIdx]["TrickCode"] ).ToUpper().Equals("FALL") ) {
                                myUpdatedTrickCode = inCode;
                                returnValue = true;
                            } else {
                                prevTrickIdx = 0;
                                prevCode = (String) prevTrickRows[prevTrickIdx]["TrickCode"];
                                prevStartPos = (Int16)((Byte) prevTrickRows[prevTrickIdx]["StartPos"]);
                                prevNumTurns = (Int16)((Byte) prevTrickRows[prevTrickIdx]["NumTurns"]);
                                prevTypeCodeValue = Convert.ToInt16((Byte) prevTrickRows[prevTrickIdx]["TypeCode"]);

                                if ( prevCode.ToUpper().Equals("FALL") ) {
                                    if ( curStartPos == 0 ) {
                                        myUpdatedTrickCode = inCode;
                                        returnValue = true;
                                    } else {
                                        myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", inCode, prevCode);
                                        returnValue = false;
                                    }
                                } else if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
                                    if ( curStartPos == 0 ) {
                                        myUpdatedTrickCode = inCode;
                                        returnValue = true;
                                    } else {
                                        myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", inCode, prevCode);
                                        returnValue = false;
                                    }
                                } else {
                                    if ( curStartPos == 1 ) {
                                        myUpdatedTrickCode = inCode;
                                        returnValue = true;
                                    } else {
                                        if ( prevCode.ToUpper().Equals("FALL") ) {
                                            myUpdatedTrickCode = inCode;
                                            returnValue = true;
                                        } else {
                                            myValidationMessage = String.Format("Trick sequence is not possible, {0} following {1} is not possible", inCode, prevCode);
                                            returnValue = false;
                                        }
                                    }
                                }
                            }
                        } else {
                            if ( curStartPos == 0 ) {
                                myUpdatedTrickCode = inCode;
                                returnValue = true;
                            } else {
                                myValidationMessage = "Trick sequence is not possible "
                                    + "\n First trick can not be one that starts from the back position";
                                returnValue = false;
                            }
                        }
                    }
                    #endregion
                }
            } else {
                myValidationMessage = "Trick code is required";
                returnValue = false;
            }
            return returnValue;
        }

        public bool calcScore() {
            bool returnValue = true;
            return returnValue;
        }

        //DataTable
        //DataGridView inPassDataTable, DataGridViewRow inPassRow, String inColPrefix
        public Int16 calcPoints( DataTable inPass1DataTable, DataTable inPass2DataTable, DataRow inViewRow, int inRowIdx, String inColPrefix, String inSkierClass ) {
            DataRow curTrickRow;
            Int16 retPoints = -1, curRuleNum = 0, curNumTurns = 0, curStartPos = 0, curTypeCodeValue = 0;
            Int16 prevStartPos, prevNumTurns, prev0NumTurns, prevTypeCodeValue, prevRuleNum, tempRuleNum;
            String prevCode;

            DataTable activePassDataTable, idlePassDataTable;
            if ( inColPrefix.Equals("Pass1") ) {
                activePassDataTable = inPass1DataTable;
                idlePassDataTable = inPass2DataTable;
            } else {
                activePassDataTable = inPass2DataTable;
                idlePassDataTable = inPass1DataTable;
            }

            myValidationMessage = "";
            int curIdx = inRowIdx;
            String curCode = (String) inViewRow["Code"];

            #region Determine points for current trick code
            if ( curCode.Length > 0 ) {
                if ( curCode.ToUpper().Equals("R") || curCode.Substring(0, 1).ToUpper().Equals("R") ) {
                    #region Determine trick associated with reverse reference
                    if ( curIdx > 0 ) {
                        if ( curCode.Length > 1 ) {
                            curTrickRow = getTrickRow(curCode, (Byte)inViewRow["Skis"], inSkierClass);
                            if ( curTrickRow == null ) {
                                curRuleNum = 0;
                            } else {
                                curStartPos = (Byte) curTrickRow["StartPos"];
                                curNumTurns = (Byte) curTrickRow["NumTurns"];
                                curTypeCodeValue = (Byte) curTrickRow["TypeCode"];
                                curRuleNum = (Int16) ( (Int16) curTrickRow["RuleNum"] + ( ( (Byte)inViewRow["Skis"] * 100 ) + 200 ) );

                                inViewRow["StartPos"] = curStartPos.ToString();
                                inViewRow["NumTurns"] = curNumTurns.ToString();
                                inViewRow["RuleNum"] = curRuleNum.ToString();
                                inViewRow["TypeCode"] = curTypeCodeValue.ToString();
                            }
                        } else {
                            curTrickRow = null;
                        }

                        curIdx--;
                        prevCode = (String)activePassDataTable.Rows[curIdx]["Code"];
                        prevRuleNum = (Int16)activePassDataTable.Rows[curIdx]["RuleNum"];
                        prevStartPos = (Byte)activePassDataTable.Rows[curIdx]["StartPos"];
                        prevNumTurns = (Byte) activePassDataTable.Rows[curIdx]["NumTurns"];
                        prevTypeCodeValue = (Byte)activePassDataTable.Rows[curIdx]["TypeCode"];

                        if ( ( prevNumTurns % 2 ) == 0 ) {
                            //Determine reverse trick when previous trick had an even number of 180 degree turns
                            if ( curCode.Length == 1 ) {
                                if ( prevCode.Substring(0, 1).Equals("R") ) {
                                    curCode = prevCode;
                                } else {
                                    curCode = curCode + prevCode;
                                }
                                curTrickRow = getTrickRow(curCode, (Byte)inViewRow["Skis"], inSkierClass);
                                if ( curTrickRow == null ) {
                                } else {
                                    curStartPos = (Byte) curTrickRow["StartPos"];
                                    curNumTurns = (Byte) curTrickRow["NumTurns"];
                                    curTypeCodeValue = (Byte) curTrickRow["TypeCode"];
                                    curRuleNum = (Int16) ( (Int16) curTrickRow["RuleNum"] + ( ( (Byte)inViewRow["Skis"] * 100 ) + 200 ) );

                                    inViewRow["Code"] = curCode;
                                    inViewRow["StartPos"] = curStartPos;
                                    inViewRow["NumTurns"] = curNumTurns;
                                    inViewRow["RuleNum"] = curRuleNum;
                                    inViewRow["TypeCode"] = curTypeCodeValue;
                                }
                            }

                            if ( curStartPos == prevStartPos ) {
                                if ( curCode.ToUpper().Equals(prevCode.ToUpper()) ) {
                                    retPoints = (Int16) curTrickRow["Points"];
                                } else if ( prevRuleNum == ( curRuleNum - 200 ) ) {
                                    retPoints = (Int16) curTrickRow["Points"];
                                } else {
                                    myValidationMessage = "Points not allowed "
                                        + "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
                                    retPoints = -1;
                                }
                            } else {
                                myValidationMessage = "Points not allowed "
                                    + "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
                                retPoints = -1;
                            }
                        } else {
                            //Determine reverse trick when previous trick had an odd number of 180 degree turns
                            if ( ( (Int16) ( curRuleNum - 200 ) ) == prevRuleNum ) {
                                if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
                                    if ( curStartPos == 0 ) {
                                        retPoints = (Int16) curTrickRow["Points"];
                                    } else {
                                        myValidationMessage = "Points not allowed "
                                            + "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
                                        retPoints = -1;
                                    }
                                } else {
                                    if ( curStartPos == 1 ) {
                                        retPoints = (Int16) curTrickRow["Points"];
                                    } else {
                                        myValidationMessage = "Points not allowed "
                                            + "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
                                        retPoints = -1;
                                    }
                                }
                            } else {
                                if ( curIdx > 0 ) {
                                    prev0NumTurns = prevNumTurns;
                                    curIdx--;
                                    prevCode = (String) activePassDataTable.Rows[curIdx]["Code"];
                                    prevRuleNum = (Int16) activePassDataTable.Rows[curIdx]["RuleNum"];
                                    prevStartPos = (Byte) activePassDataTable.Rows[curIdx]["StartPos"];
                                    prevNumTurns = (Byte) activePassDataTable.Rows[curIdx]["NumTurns"];
                                    prevTypeCodeValue = (Byte) activePassDataTable.Rows[curIdx]["TypeCode"];

                                    if ( prevCode.Substring(0, 1).Equals("R") ) {
                                        curCode = prevCode;
                                    } else {
                                        if ( curCode.Length > 1 ) {
                                        } else {
                                            curCode = curCode + prevCode;
                                        }
                                    }
                                    curTrickRow = getTrickRow(curCode, (Byte)inViewRow["Skis"], inSkierClass);
                                    if ( curTrickRow == null ) {
                                    } else {
                                        curStartPos = (Byte) curTrickRow["StartPos"];
                                        curNumTurns = (Byte) curTrickRow["NumTurns"];
                                        curRuleNum = (Int16) ( (Int16) curTrickRow["RuleNum"] + ( ( (Byte)inViewRow["Skis"] * 100 ) + 200 ) );
                                        curTypeCodeValue = (Byte) curTrickRow["TypeCode"];

                                        inViewRow["Code"] = curCode;
                                        inViewRow["StartPos"] = curStartPos;
                                        inViewRow["NumTurns"] = curNumTurns;
                                        inViewRow["RuleNum"] = curRuleNum;
                                        inViewRow["TypeCode"] = curTypeCodeValue;

                                        if ( curStartPos == prevStartPos && prev0NumTurns == 1 ) {
                                            if ( curCode.ToUpper().Equals(prevCode.ToUpper()) ) {
                                                retPoints = (Int16) curTrickRow["Points"];
                                            } else if ( prevRuleNum == ( curRuleNum - 200 ) ) {
                                                retPoints = (Int16) curTrickRow["Points"];
                                            } else {
                                                myValidationMessage = "Points not allowed "
                                                    + "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
                                                retPoints = -1;
                                            }
                                        } else {
                                            myValidationMessage = "Points not allowed for " + curCode
                                                + "\n More than 180 degrees has occurred between a trick and its reverse";
                                            retPoints = -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                } else {
                    #region Validate trick code entered
                    curTrickRow = getTrickRow(curCode, (Byte)inViewRow["Skis"], inSkierClass);
                    if ( curTrickRow == null ) {
                    } else {
                        curStartPos = (Byte) curTrickRow["StartPos"];
                        curNumTurns = (Byte) curTrickRow["NumTurns"];
                        curTypeCodeValue = (Byte) curTrickRow["TypeCode"];
                        curRuleNum = (Int16) ( (Int16) curTrickRow["RuleNum"] + ( (Byte)inViewRow["Skis"] * 100 ) );

                        inViewRow["StartPos"] = curStartPos;
                        inViewRow["NumTurns"] = curNumTurns;
                        inViewRow["RuleNum"] = curRuleNum;
                        inViewRow["TypeCode"] = curTypeCodeValue;

                        if ( curIdx > 0 ) {
                            curIdx--;
                            prevCode = (String) activePassDataTable.Rows[curIdx]["Code"];
                            prevRuleNum = (Int16) activePassDataTable.Rows[curIdx]["RuleNum"];
                            prevStartPos = (Byte) activePassDataTable.Rows[curIdx]["StartPos"];
                            prevNumTurns = (Byte) activePassDataTable.Rows[curIdx]["NumTurns"];
                            prevTypeCodeValue = (Byte) activePassDataTable.Rows[curIdx]["TypeCode"];

                            if ( prevCode.ToUpper().Equals("FALL") ) {
                                if ( curStartPos == 0 ) {
                                    retPoints = (Int16) curTrickRow["Points"];
                                }
                            } else if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
                                if ( curStartPos == 0 ) {
                                    retPoints = (Int16) curTrickRow["Points"];
                                }
                            } else {
                                if ( curStartPos == 1 ) {
                                    retPoints = (Int16) curTrickRow["Points"];
                                }
                            }
                        } else {
                            retPoints = (Int16) curTrickRow["Points"];
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region Determine if trick has previously been successfully performed
            if ( retPoints > 0 ) {
                //Search previous tricks on active pass to determine if trick successfully performed previously
                curIdx = inRowIdx -1;
                if ( curCode.ToLower().Equals("t5b") ) {
                    if ( inRowIdx > 0 ) {
                        prevCode = (String)activePassDataTable.Rows[curIdx]["Code"];
                        if ( prevCode.ToLower().Equals("t7f") || prevCode.ToLower().Equals("t7") ) {
                            curIdx = -1;
                        }
                    }
                }
                while ( curIdx >= 0 ) {
                    try {
                        tempRuleNum = (Int16) activePassDataTable.Rows[curIdx]["RuleNum"];
                        retPoints = checkForRepeat(activePassDataTable, idlePassDataTable, inViewRow, activePassDataTable.Rows[curIdx]
                            , inRowIdx, curIdx, inColPrefix, true, curRuleNum, tempRuleNum, retPoints);
                        if ( retPoints == 0 ) break;
                    } catch {
                    }
                    curIdx--;
                }

                //If second pass is active then check first pass to determine if trick successfully performed previously
                if ( inColPrefix.Equals("Pass2") ) {
                    curIdx = 0;
                    while ( curIdx < inPass1DataTable.Rows.Count ) {
                        try {
                            tempRuleNum = (Int16) inPass1DataTable.Rows[curIdx]["RuleNum"];
                            retPoints = checkForRepeat(activePassDataTable, idlePassDataTable, inViewRow, inPass1DataTable.Rows[curIdx]
                                , inRowIdx, curIdx, inColPrefix, false, curRuleNum, tempRuleNum, retPoints);
                            if ( retPoints == 0 ) break;
                        } catch ( Exception ex ) {
                            myValidationMessage = "Exception encountered calculating points: \n" + ex.Message;
                            retPoints = -1;
                            break;
                        }
                        curIdx++;
                    }
                }
            }
            #endregion

            #region Determine if subsequent tricks are a repeat of current trick
            if ( retPoints > 0 ) {
                //Check for repeats on active pass
                curIdx = inRowIdx + 1;
                while ( curIdx < activePassDataTable.Rows.Count ) {
                    try {
                        tempRuleNum = (Int16) activePassDataTable.Rows[curIdx]["RuleNum"];
                        retPoints = checkForRepeat(activePassDataTable, idlePassDataTable, inViewRow, activePassDataTable.Rows[curIdx]
                            , inRowIdx, curIdx, inColPrefix, true, curRuleNum, tempRuleNum, retPoints);
                        if ( retPoints == 0 ) break;
                    } catch ( Exception ex ) {
                        myValidationMessage = "Exception encountered calculating points: \n" + ex.Message;
                        retPoints = -1;
                        break;
                    }
                    curIdx++;
                }
                //if first pass is active check for repeats on second pass already done on first pass
                if ( inColPrefix.Equals("Pass1") ) {
                    curIdx = 0;
                    while ( curIdx < inPass2DataTable.Rows.Count ) {
                        try {
                            tempRuleNum = (Int16) inPass2DataTable.Rows[curIdx]["RuleNum"];
                            retPoints = checkForRepeat(activePassDataTable, idlePassDataTable, inViewRow, inPass2DataTable.Rows[curIdx]
                                , inRowIdx, curIdx, inColPrefix, false, curRuleNum, tempRuleNum, retPoints);
                            if ( retPoints == 0 ) break;
                        } catch ( Exception ex ) {
                            myValidationMessage = "Exception encountered calculating points: \n" + ex.Message;
                            retPoints = -1;
                            break;
                        }
                        curIdx++;
                    }
                }
            }
            #endregion

            #region Determine if maxinum number of flips have previously been accomplished
            if ( curTypeCodeValue == 1 ) {
                String curResults = "";
                Int16 tempTypeCode, curFlipCount = 0;

                if ( inColPrefix.Equals("Pass2") ) {
                    curIdx = inRowIdx - 1;
                    while ( curIdx >= 0 ) {
                        try {
                            tempTypeCode = (Int16) inPass2DataTable.Rows[curIdx]["TypeCode"];
                            if ( tempTypeCode == 1 ) {
                                curResults = (String)inPass2DataTable.Rows[curIdx]["Results"];
                                if ( curResults.Equals("Credit") ) curFlipCount++;
                            }
                        } catch {
                        }
                        curIdx--;
                    }
                }

                if ( inColPrefix.Equals("Pass1") ) {
                    curIdx = inRowIdx - 1;
                } else {
                    curIdx = inPass1DataTable.Rows.Count - 1;
                }
                while ( curIdx >= 0 ) {
                    try {
                        tempTypeCode = (Int16)( (byte) inPass1DataTable.Rows[curIdx]["TypeCode"]);
                        if ( tempTypeCode == 1 ) {
                            curResults = (String)inPass1DataTable.Rows[curIdx]["Results"];
                            if ( curResults.Equals("Credit") ) curFlipCount++;
                        }
                    } catch (Exception ex) {
                        String curMsg = "Exception: " + ex.Message;
                    }
                    curIdx--;
                }
                if ( curFlipCount >= 6 ) {
                    DataRow curSkierClassRow = mySkierClassDataTable.Select("ListCode = '" + inSkierClass + "'")[0];
                    if ( (Decimal) curSkierClassRow["ListCodeNum"] > (Decimal) myClassERow["ListCodeNum"] ) {
                        inViewRow["Score"] = 0;
                        inViewRow["Results"] = "No Credit";
                        myValidationMessage = "The maximum of 6 flips for IWWF have previously been completed.  No credit is given for more than 6 flips.";
                        retPoints = 0;
                    }
                }
            }
            #endregion

            return retPoints;
        }

        //Int16 curRuleNum, Int16 tempRuleNum, DataGridViewRow curViewRow, DataGridViewRow curActivePassRow, String inColPrefix, Int16 curActivePoints
        //activePassDataTable, idlePassDataTable
        private Int16 checkForRepeat( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, DataRow inCheckedRow
            , int inRowViewIdx, int inRowCheckedIdx, String inColPrefix, bool inActiveView, Int16 curRuleNum, Int16 tempRuleNum, Int16 inActivePoints ) {

            Int16 retPoints = inActivePoints;
            if ( curRuleNum == tempRuleNum ) {
                retPoints = checkCreditAndPoints(activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inRowViewIdx, inRowCheckedIdx, inColPrefix, inActiveView, retPoints);
            } else if ( curRuleNum > 100 && curRuleNum < 200 ) {
                if ( ( curRuleNum + 200 ) == tempRuleNum ) {
                    if ( inActiveView ) {
                        if ( inRowViewIdx > inRowCheckedIdx ) {
                            retPoints = checkCreditAndPoints(activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inRowViewIdx, inRowCheckedIdx, inColPrefix, inActiveView, retPoints);
                        }
                    } else {
                        if ( inColPrefix.Equals("Pass2") ) {
                            retPoints = checkCreditAndPoints(activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inRowViewIdx, inRowCheckedIdx, inColPrefix, inActiveView, retPoints);
                        }
                    }
                }
            } else if ( curRuleNum > 200 && curRuleNum < 300 ) {
                if ( ( curRuleNum + 200 ) == tempRuleNum ) {
                    retPoints = checkCreditAndPoints(activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inRowViewIdx, inRowCheckedIdx, inColPrefix, inActiveView, retPoints);
                }
            }
            return retPoints;
        }

        //DataGridViewRow curViewRow, DataGridViewRow curActivePassRow, String inColPrefix, Int16 curActivePoints
        private Int16 checkCreditAndPoints( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, DataRow inCheckedRow
            , int inRowViewIdx, int inRowCheckedIdx, String inColPrefix, bool inActiveView, Int16 inActivePoints ) {
            //If trick repeated determine if previous trick was same number but worth more or less points

            Int16 retPoints = inActivePoints;
            try {
                if ( ( (String) inViewRow["Results"] ).Equals("Credit") 
                    && ( (String) inCheckedRow["Results"] ).Equals("Credit")
                    ) {
                    if ( (Int16)inCheckedRow["Score"] < retPoints ) {
                        if ( inActiveView ) {
                            if ( inRowViewIdx > 0 ) {
                                if ( inRowViewIdx > inRowCheckedIdx ) {
                                    inCheckedRow["Score"] = 0;
                                    inCheckedRow["Results"] = "Repeat";
                                } else {
                                    inViewRow["Score"] = 0;
                                    inViewRow["Results"] = "Repeat";
                                    retPoints = 0;
                                }
                            } else {
                                inCheckedRow["Score"] = 0;
                                inCheckedRow["Results"] = "Repeat";
                                retPoints = 0;
                            }
                        } else {
                            if ( inColPrefix.Equals("Pass1") ) {
                                inCheckedRow["Score"] = 0;
                                inCheckedRow["Results"] = "Repeat";
                            } else {
                                inViewRow["Score"] = 0;
                                inViewRow["Results"] = "Repeat";
                                retPoints = 0;
                            }
                        }
                    } else {
                        if ( inActiveView ) {
                            if ( inRowViewIdx > 0 ) {
                                if ( inRowViewIdx > inRowCheckedIdx ) {
                                    //If the CheckRow is 1 row behind (2 for a 180 trick) and is a reverse trick
                                    //Check the previous trick to see if same trick number but is not for credit
                                    //If that is the case then this trick should get credit
                                    // prevRuleNum = (Int16)activePassDataTable.Rows[curIdx]["RuleNum"];
                                    // prevStartPos = (Byte) activePassDataTable.Rows[curIdx]["StartPos"];
                                    // prevNumTurns = (Byte) activePassDataTable.Rows[curIdx]["NumTurns"];
                                    Int16 curNumTurns = (Byte) inViewRow["NumTurns"];
                                    if ( ( ( curNumTurns % 2 ) == 0 ) && ( (inRowViewIdx - 1) == inRowCheckedIdx ) ) {
                                        Int16 curRuleNum = (Int16)inViewRow["RuleNum"];
                                        Int16 prevRuleNum = (Int16) activePassDataTable.Rows[inRowCheckedIdx - 1]["RuleNum"];
                                        if ( curRuleNum == prevRuleNum && (Int16) activePassDataTable.Rows[inRowCheckedIdx - 1]["Score"] == 0 ) {
                                        } else {
                                            inViewRow["Score"] = 0;
                                            inViewRow["Results"] = "Repeat";
                                            retPoints = 0;
                                        }

                                    } else {
                                        if ( ( ( curNumTurns % 2 ) > 0 ) && ( ( inRowViewIdx - 2 ) == inRowCheckedIdx ) ) {
                                            Int16 curRuleNum = (Int16) inViewRow["RuleNum"];
                                            Int16 prevRuleNum = (Int16) activePassDataTable.Rows[inRowCheckedIdx - 2]["RuleNum"];
                                            if ( curRuleNum == prevRuleNum && (Int16) activePassDataTable.Rows[inRowCheckedIdx - 2]["Score"] == 0 ) {
                                            } else {
                                                inViewRow["Score"] = 0;
                                                inViewRow["Results"] = "Repeat";
                                                retPoints = 0;
                                            }

                                        } else {
                                            inViewRow["Score"] = 0;
                                            inViewRow["Results"] = "Repeat";
                                            retPoints = 0;
                                        }
                                    }

                                } else {
                                    inCheckedRow["Score"] = 0;
                                    inCheckedRow["Results"] = "Repeat";
                                }
                            } else {
                                inCheckedRow["Score"] = 0;
                                inCheckedRow["Results"] = "Repeat";
                                retPoints = 0;
                            }
                        } else {
                            if ( inColPrefix.Equals("Pass1") ) {
                                inCheckedRow["Score"] = 0;
                                inCheckedRow["Results"] = "Repeat";
                            } else {
                                inViewRow["Score"] = 0;
                                inViewRow["Results"] = "Repeat";
                                retPoints = 0;
                            }
                        }
                    }
                }
            } catch ( Exception ex ) {
                String curMsg = ex.Message;
            }
            return retPoints;
        }


        public DataRow getTrickRow( DataRow inDataRow, String inSkierClass ) {
            return getTrickRow((String) inDataRow["Code"], (Int16)((Byte) inDataRow["Skis"]), inSkierClass);
        }
        public DataRow getTrickRow( String inTrickCode, Int16 inNumSkies, String inSkierClass ) {
            DataRow curReturnRow = null;
            DataRow[] curFoundList;

            curFoundList = myTrickListDataTable.Select("TrickCode = '" + inTrickCode + "'" + " AND NumSkis = " + inNumSkies);
            if ( curFoundList.Length > 0 ) {
                curReturnRow = curFoundList[0];

                DataRow curSkierClassRow = mySkierClassDataTable.Select("ListCode = '" + inSkierClass + "'")[0];
                if ( (Decimal) curSkierClassRow["ListCodeNum"] <= (Decimal) myClassERow["ListCodeNum"] ) {
                    if ( mySpecialFlipList.Contains(inTrickCode) ) {
                        curReturnRow["NumTurns"] = Convert.ToByte("3");
                    }
                }
            }

            return curReturnRow;
        }

    }
}
