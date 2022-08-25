using System;
using System.Data;
using System.Linq;

namespace ValidationLibrary.Trick {
	public class Validation {
		private String myTourRules = "";
		private String myValidationMessage = "";
		private String myUpdatedTrickCode = "";
		private Int16 myNumSkis = -1;

		private DataRow myClassCRow = null;
		private DataRow myClassERow = null;

		private String[] myAllowedRepeatReverseList = { "RS", "RTS", "RB", "RF", "RTB", "RTF" };
		//private ArrayList mySpecialFlipList = new ArrayList();

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

				/*
                mySpecialFlipList.Add("BFLB");
                mySpecialFlipList.Add("RBFLB");
                mySpecialFlipList.Add("BFLF");
                mySpecialFlipList.Add("RBFLF");
                mySpecialFlipList.Add("BFLLB");
                mySpecialFlipList.Add("RBFLLB");
                mySpecialFlipList.Add("FFLF");
                mySpecialFlipList.Add("FFLB");
				 */
			}
		}

		public DataTable SkierClassDataTable {
			get {
				return mySkierClassDataTable;
			}
			set {
				mySkierClassDataTable = value;

				myClassCRow = mySkierClassDataTable.Select( "ListCode = 'C'" )[0];
				myClassERow = mySkierClassDataTable.Select( "ListCode = 'E'" )[0];
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
			return validateNumSkis( inNumSkis, myTourRules );
		}
		public Int16 validateNumSkis( String inNumSkis, String inTourRules ) {
			Int16 curNumSkis = -1;

			if ( inNumSkis.Length > 0 ) {
				if ( inNumSkis.Equals( "1" ) || inNumSkis.Equals( "2" ) ) {
					Int16.TryParse( inNumSkis, out curNumSkis );

				} else if ( inTourRules.ToLower().Equals( "ncwsa" )
					&& ( inNumSkis.ToLower().Equals( "wb" )
					|| inNumSkis.ToLower().Equals( "w" )
					|| inNumSkis.ToLower().Equals( "kb" )
					|| inNumSkis.ToLower().Equals( "k" )
					) ) {
					if ( inNumSkis.ToLower().Equals( "wb" ) || inNumSkis.ToLower().Equals( "w" ) ) {
						curNumSkis = 0;

					} else if ( inNumSkis.ToLower().Equals( "kb" ) || inNumSkis.ToLower().Equals( "k" ) ) {
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
			return validateResultStatus( inResults, myTourRules );
		}
		public bool validateResultStatus( String inResults, String inTourRules ) {
			if ( inResults.ToLower().Equals( "credit" )
				|| inResults.ToLower().Equals( "fall" )
				|| inResults.ToLower().Equals( "no credit" )
				|| inResults.ToLower().Equals( "before" )
				|| inResults.ToLower().Equals( "ooc" )
				|| inResults.ToLower().Equals( "repeat" )
				|| inResults.ToLower().Equals( "end" )
				|| inResults.ToLower().Equals( "unresolved" )
				) {
				return true;
			}

			myValidationMessage = String.Format( "Trick result {0} is not valid", inResults );
			return false;
		}

		/*
        Validate the trick code
        */
		public bool validateTrickCode( String inCode, Int16 inNumSkis, String inSkierClass, DataRow[] prevTrickRows ) {
			Int16 curNumTurns, curStartPos, curTypeCodeValue;
			Int16 prevStartPos, prevNumTurns, prevTypeCodeValue;
			String prevCode;
			int prevTrickIdx = 0;
			DataRow curTrickRow;

			myUpdatedTrickCode = "";

			if ( inCode == null || inCode.Length == 0 ) {
				myValidationMessage = "Trick code is required";
				return false;
			}

			if ( inCode.ToUpper().Equals( "T5B" ) && prevTrickRows != null && prevTrickRows.Length > 0 ) {
				prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
				if ( prevCode.Equals( "T7F" ) || prevCode.Equals( "T7" ) ) inCode = "RT5B";
			}

			if ( inCode.ToUpper().Equals( "R" ) || inCode.Substring( 0, 1 ).ToUpper().Equals( "R" ) ) {
				return validateTrickCodeReverse( inCode, inNumSkis, inSkierClass, prevTrickRows );
			}

			// Analyze trick code that doesn't start with "R"
			curTrickRow = getTrickRow( inCode, inNumSkis, inSkierClass );
			if ( curTrickRow == null ) {
				myValidationMessage = String.Format( "Invalid trick code for number of skis"
					+ "\n Trick Code {0} on {1} ski(s) is not valid"
					, inCode, inNumSkis.ToString() );
				return false;
			}

			curStartPos = (Byte)curTrickRow["StartPos"];
			curNumTurns = (Byte)curTrickRow["NumTurns"];
			curTypeCodeValue = (Byte)curTrickRow["TypeCode"];

			if ( prevTrickRows.Length == 0 ) {
				// Determine if first trick starts in the front position if no set up tricks performed
				if ( curStartPos == 0 ) {
					myUpdatedTrickCode = inCode;
					return true;
				}

				myValidationMessage = "Trick sequence is not possible "
					+ "\n First trick can not be one that starts from the back position";
				return false;
			}

			// Check sequence of tricks to ensure it is a valid and possible
			if ( prevTrickRows.Length == 1
				&& inCode.ToUpper().Equals( "FALL" )
				&& ( (String)prevTrickRows[prevTrickIdx]["TrickCode"] ).ToUpper().Equals( "FALL" ) ) {
				myUpdatedTrickCode = inCode;
				return true;
			}

			prevTrickIdx = 0;
			prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
			if ( prevCode.ToUpper().Equals( "FALL" ) ) {
				if ( curStartPos == 0 ) {
					myUpdatedTrickCode = inCode;
					return true;
				}

				myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", inCode, prevCode );
				return false;
			}

			prevStartPos = (Int16)( (Byte)prevTrickRows[prevTrickIdx]["StartPos"] );
			prevNumTurns = (Int16)( (Byte)prevTrickRows[prevTrickIdx]["NumTurns"] );
			prevTypeCodeValue = Convert.ToInt16( (Byte)prevTrickRows[prevTrickIdx]["TypeCode"] );

			if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
				if ( curStartPos == 0 ) {
					myUpdatedTrickCode = inCode;
					return true;
				}

				myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", inCode, prevCode );
				return false;
			}

			if ( curStartPos == 1 ) {
				myUpdatedTrickCode = inCode;
				return true;
			}

			if ( prevCode.ToUpper().Equals( "FALL" ) ) {
				myUpdatedTrickCode = inCode;
				return true;
			}

			myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", inCode, prevCode );
			return false;
		}

		/*
		 * Determine actual trick code for any value that starts with "R" for reverse code
		 */
		public bool validateTrickCodeReverse( String inCode, Int16 inNumSkis, String inSkierClass, DataRow[] prevTrickRows ) {
			Int16 prevNumTurns;
			myUpdatedTrickCode = "";
			int prevTrickIdx = 0;

			if ( prevTrickRows == null || prevTrickRows.Length == 0 ) {
				myValidationMessage = String.Format( "Reverse code not logical as first trick in a pass"
					+ "\n Trick Code {0} on {1} ski(s) is not valid"
					, inCode, inNumSkis.ToString() );
				return false;
			}

			//Check to determine if the previous number of turns in the trick are an odd or even number indicating starting and stoping position
			prevNumTurns = (byte)prevTrickRows[prevTrickIdx]["NumTurns"];
			if ( ( prevNumTurns % 2 ) == 0 ) {
				return validateTrickCodeReverseEven( inCode, inNumSkis, inSkierClass, prevTrickRows );
			}

			return validateTrickCodeReverseOdd( inCode, inNumSkis, inSkierClass, prevTrickRows );
		}

		/*
		 * For reverse codes where the 1st previous trick has an even number of turns
		 * Having an even number of turns means that the next trick starts facing in the same direction as the current trick
		 * When previous trick has an even number of turns, will need to check previous trick
		 */
		public bool validateTrickCodeReverseEven( String inCode, Int16 inNumSkis, String inSkierClass, DataRow[] prevTrickRows ) {
			Int16 curNumTurns, curStartPos, curTypeCodeValue;
			Int16 prevStartPos, prevNumTurns, prevTypeCodeValue, prevRuleNum;
			String curCode, prevCode;
			myUpdatedTrickCode = "";
			int prevTrickIdx = 0;

			prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
			prevStartPos = (byte)prevTrickRows[prevTrickIdx]["StartPos"];
			prevNumTurns = (byte)prevTrickRows[prevTrickIdx]["NumTurns"];
			prevTypeCodeValue = (byte)prevTrickRows[prevTrickIdx]["TypeCode"];
			prevRuleNum = (Int16)prevTrickRows[prevTrickIdx]["RuleNum"];

			if ( inCode.Length > 1 ) {
				curCode = inCode;

			} else {
				if ( prevCode.Substring( 0, 1 ).Equals( "R" ) ) {
					if ( myAllowedRepeatReverseList.Contains( prevCode ) ) {
						curCode = prevCode;
					} else {
						curCode = inCode + prevCode;
					}
				} else {
					curCode = inCode + prevCode;
				}
			}

			DataRow curTrickRow = getTrickRow( curCode, inNumSkis, inSkierClass );
			if ( curTrickRow == null ) {
				myValidationMessage = String.Format( "Invalid trick code for number of skis"
					+ "\n Trick Code {0} on {1} ski(s) is not valid"
					, inCode, inNumSkis.ToString() );
				return false;
			}

			//Check to ensure 360 multiples tricks has an appropriate starting position on the previous 2 tricks
			curStartPos = (Byte)curTrickRow["StartPos"];
			curNumTurns = (Byte)curTrickRow["NumTurns"];
			curTypeCodeValue = (Byte)curTrickRow["TypeCode"];
			if ( curStartPos == prevStartPos ) {
				myUpdatedTrickCode = curCode;
				return true;
			}
			myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode );
			return false;
		}

		/*
		 * For reverse codes where the 1st previous trick has an odd number of turns
		 * Having an odd number of turns means that the next trick starts facing in the opposite direction as the current trick
		 * When previous trick has an odd number of turns, will need to check 2nd previous tricks
		 */
		public bool validateTrickCodeReverseOdd( String inCode, Int16 inNumSkis, String inSkierClass, DataRow[] prevTrickRows ) {
			Int16 curNumTurns, curStartPos, curTypeCodeValue, curRuleNum;
			Int16 prevStartPos, prevNumTurns, prevTypeCodeValue, prevRuleNum;
			String curCode, prevCode;
			DataRow curTrickRow = null;
			myUpdatedTrickCode = "";

			int prevTrickIdx = 0;
			prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
			prevStartPos = (byte)prevTrickRows[prevTrickIdx]["StartPos"];
			prevNumTurns = (byte)prevTrickRows[prevTrickIdx]["NumTurns"];
			prevTypeCodeValue = (byte)prevTrickRows[prevTrickIdx]["TypeCode"];
			prevRuleNum = (Int16)prevTrickRows[prevTrickIdx]["RuleNum"];

			if ( prevTrickRows.Length > 1 ) {
				#region When 2 previous tricks are available start by reviewing the 2nd previous trick
				prevTrickIdx++;
				if ( inCode.Length > 1 ) {
					curCode = inCode;

				} else {
					prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
					if ( prevCode.Substring( 0, 1 ).Equals( "R" ) ) {
						if ( myAllowedRepeatReverseList.Contains( prevCode ) ) {
							curCode = prevCode;
						} else {
							curCode = inCode + prevCode;
						}
					} else {
						curCode = inCode + prevCode;
					}
				}
				curTrickRow = getTrickRow( curCode, inNumSkis, inSkierClass );
				if ( curTrickRow == null ) {
					myValidationMessage = String.Format( "Invalid trick code for number of skis"
						+ "\n Trick Code {0} on {1} ski(s) is not valid"
						, inCode, inNumSkis.ToString() );
					return false;
				}

				if ( prevNumTurns > 1 ) {
					myUpdatedTrickCode = prevCode;
					return true;
				}

				//Check to see if starting position of previous trick is appropriate
				curStartPos = (Byte)curTrickRow["StartPos"];
				curNumTurns = (Byte)curTrickRow["NumTurns"];
				curTypeCodeValue = (Byte)curTrickRow["TypeCode"];
				curRuleNum = (Int16)( (Int16)curTrickRow["RuleNum"] + ( inNumSkis * 100 ) );

				if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
					if ( curStartPos == 0 ) {
						//Check to see if starting position of 2nd previous trick is also appropriate
						prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
						prevStartPos = (byte)prevTrickRows[prevTrickIdx]["StartPos"];
						prevNumTurns = (byte)prevTrickRows[prevTrickIdx]["NumTurns"];
						if ( curStartPos == prevStartPos ) {
							myUpdatedTrickCode = curCode;
							return true;
						}

						myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode );
						return false;
					}

					myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode );
					return false;
				}

				if ( curStartPos == 0 ) {
					myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode );
					return false;
				}
				if ( curRuleNum == prevRuleNum ) {
					if ( curNumTurns > 2 ) {
						myValidationMessage = "This type of reverse trick can not have more than 360 degrees";
						return false;
					}

					myUpdatedTrickCode = curCode;
					return true;
				}

				//Check to see if starting position of 2nd previous trick is also appropriate
				prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
				prevStartPos = (byte)prevTrickRows[prevTrickIdx]["StartPos"];
				prevNumTurns = (byte)prevTrickRows[prevTrickIdx]["NumTurns"];
				if ( curStartPos == prevStartPos ) {
					myUpdatedTrickCode = curCode;
					return true;
				}

				myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode );
				return false;
				#endregion
			}

			#region Only 1 previous trick is available so we start by reviewing the 1st previous trick
			if ( inCode.Length > 1 ) {
				curCode = inCode;
				curTrickRow = getTrickRow( curCode, inNumSkis, inSkierClass );
				if ( curTrickRow == null ) {
					myValidationMessage = String.Format( "Trick sequence is not possible"
						+ "\n Use of reverse code {0} but second trick can't be a reverse of trick {1}"
						, curCode, prevCode );
					return false;
				}

				curStartPos = (Byte)curTrickRow["StartPos"];
				curNumTurns = (Byte)curTrickRow["NumTurns"];
				curTypeCodeValue = (Byte)curTrickRow["TypeCode"];
				curRuleNum = (Int16)( (Int16)curTrickRow["RuleNum"] + ( inNumSkis * 100 ) );

				//Check to see if starting position of previous trick is appropriate
				if ( ( ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) && ( curStartPos == 0 ) )
					|| ( ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 1 ) && ( curStartPos == 1 ) )
					) {
					if ( curRuleNum == prevRuleNum ) {
						if ( curNumTurns > 2 ) {
							myValidationMessage = "This type of reverse trick can not have more than 360 degrees";
							return false;
						}

						myUpdatedTrickCode = curCode;
						return true;
					}

					myValidationMessage = String.Format( "Trick sequence is not possible"
						+ "\n Use of reverse code {0} but second trick can't be a reverse of trick {1}"
						, curCode, prevCode );
					return false;
				}

				myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", inCode, prevCode );
				return false;
				#endregion
			}

			myValidationMessage = String.Format( "Trick sequence is not possible"
				+ "\n Use of reverse code {0} but second trick can't be a reverse of trick {1}"
				, inCode, prevCode );
			return false;
		}

		public bool calcScore() {
			bool returnValue = true;
			return returnValue;
		}

		public Int16 calcPoints( DataTable inPass1DataTable, DataTable inPass2DataTable, DataRow inViewRow, int inRowIdx, String inColPrefix, String inSkierClass ) {
			Int16 retPoints = -1;

			DataTable activePassDataTable, idlePassDataTable;
			if ( inColPrefix.Equals( "Pass1" ) ) {
				activePassDataTable = inPass1DataTable;
				idlePassDataTable = inPass2DataTable;
			} else {
				activePassDataTable = inPass2DataTable;
				idlePassDataTable = inPass1DataTable;
			}

			myValidationMessage = "";
			int curIdx = inRowIdx;
			String curCode = (String)inViewRow["Code"];
			if ( curCode.Length <= 0 || inRowIdx < 0 ) {
				myValidationMessage = "Trick code has not been provided or there is no active row";
				return -1;
			}

			#region Determine points for current trick code

			if ( curCode.ToUpper().Equals( "R" ) || curCode.Substring( 0, 1 ).ToUpper().Equals( "R" ) ) {
				// Determine points associated with reverse trick referenced 
				retPoints = calcPointsReverseTrick( activePassDataTable, idlePassDataTable, inViewRow, inRowIdx, inColPrefix, inSkierClass );

			} else {
				retPoints = calcPointsPrimaryTrick( activePassDataTable, idlePassDataTable, inViewRow, inRowIdx, inColPrefix, inSkierClass );
			}
			#endregion

			#region Determine if trick has previously been successfully performed
			if ( retPoints > 0 ) {
				retPoints = checkPrevForRepeat( activePassDataTable, idlePassDataTable, inViewRow, inRowIdx, curCode, inColPrefix, retPoints );
			}
			#endregion

			#region Determine if subsequent tricks are a repeat of current trick
			/*
            if ( retPoints > 0 ) {
                //Check for repeats on active pass
                curIdx = inRowIdx + 1;
                while ( curIdx < activePassDataTable.Rows.Count ) {
                    try {
                        prevRuleNum = (Int16) activePassDataTable.Rows[curIdx]["RuleNum"];
                        retPoints = checkForRepeat(activePassDataTable, idlePassDataTable, inViewRow, activePassDataTable.Rows[curIdx]
                            , inRowIdx, curIdx, inColPrefix, true, curRuleNum, prevRuleNum, retPoints);
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
                            prevRuleNum = (Int16) inPass2DataTable.Rows[curIdx]["RuleNum"];
                            retPoints = checkForRepeat(activePassDataTable, idlePassDataTable, inViewRow, inPass2DataTable.Rows[curIdx]
                                , inRowIdx, curIdx, inColPrefix, false, curRuleNum, prevRuleNum, retPoints);
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
            */
			#endregion

			#region Determine if maxinum number of flips have previously been accomplished
			if ( retPoints > 0 ) {
				// This validation is only performed for class L and R skiers
				DataRow curSkierClassRow = mySkierClassDataTable.Select( "ListCode = '" + inSkierClass + "'" )[0];
				if ( (Decimal)curSkierClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"] ) {
					// DataRow inViewRow, int inRowIdx, String inColPrefix
					if ( inColPrefix.Equals( "Pass1" ) ) inPass1DataTable.Rows[inRowIdx]["Score"] = retPoints;
					if ( inColPrefix.Equals( "Pass2" ) ) inPass2DataTable.Rows[inRowIdx]["Score"] = retPoints;
					if ( checkForMaxNumFlips( inPass1DataTable, inPass2DataTable ) ) {
						if ( inColPrefix.Equals( "Pass1" ) ) {
							if ( (Int16)inPass1DataTable.Rows[inRowIdx]["Score"] == 0 && (String)inPass1DataTable.Rows[inRowIdx]["Results"] == "Repeat" ) {
								retPoints = 0;
							}
						} else {
							if ( (Int16)inPass2DataTable.Rows[inRowIdx]["Score"] == 0 && (String)inPass2DataTable.Rows[inRowIdx]["Results"] == "Repeat" ) {
								retPoints = 0;
							}
						}

						if ( inColPrefix.Equals( "Pass1" ) ) inPass1DataTable.Rows[inRowIdx]["Score"] = 0;
						if ( inColPrefix.Equals( "Pass2" ) ) inPass2DataTable.Rows[inRowIdx]["Score"] = 0;
					}
				}
			}
			#endregion

			return retPoints;
		}

		private Int16 checkPrevForRepeat( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, int inRowIdx, String curCode, String inColPrefix, Int16 inPoints ) {
			Int16 prevRuleNum;
			int prevRowIdx = inRowIdx - 1;
			Int16 retPoints = inPoints;
			String prevCode = "";
			Byte curTypeCodeValue = (Byte)inViewRow["TypeCode"];
			Int16 curRuleNum = (Int16)inViewRow["RuleNum"];

			//Search previous tricks on active pass to determine if trick successfully performed previously
			if ( curCode.ToLower().Equals( "t5b" ) ) {
				if ( inRowIdx > 0 ) {
					prevCode = (String)activePassDataTable.Rows[prevRowIdx]["Code"];
					if ( prevCode.ToLower().Equals( "t7f" ) || prevCode.ToLower().Equals( "t7" ) ) prevRowIdx = -1;
				}
			}

			while ( prevRowIdx >= 0 ) {
				try {
					prevRuleNum = (Int16)activePassDataTable.Rows[prevRowIdx]["RuleNum"];
					retPoints = checkForRepeat( activePassDataTable, idlePassDataTable, inViewRow, activePassDataTable.Rows[prevRowIdx]
						, inRowIdx, prevRowIdx, inColPrefix, true, curRuleNum, prevRuleNum, retPoints );
					if ( retPoints == 0 ) return retPoints;
				} catch {
				}
				prevRowIdx--;
			}

			//If second pass is active then check first pass to determine if trick successfully performed previously
			if ( inColPrefix.Equals( "Pass2" ) ) {
				prevRowIdx = 0;
				while ( prevRowIdx < idlePassDataTable.Rows.Count ) {
					try {
						prevRuleNum = (Int16)idlePassDataTable.Rows[prevRowIdx]["RuleNum"];
						retPoints = checkForRepeat( activePassDataTable, idlePassDataTable, inViewRow, idlePassDataTable.Rows[prevRowIdx]
							, inRowIdx, prevRowIdx, inColPrefix, false, curRuleNum, prevRuleNum, retPoints );
						if ( retPoints == 0 ) break;
					} catch ( Exception ex ) {
						myValidationMessage = "Exception encountered calculating points: \n" + ex.Message;
						retPoints = -1;
						break;
					}
					prevRowIdx++;
				}
			}

			return retPoints;
		}

		// Determine points for trick code entered
		private Int16 calcPointsPrimaryTrick( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, int inRowIdx, String inColPrefix, String inSkierClass ) {
			Int16 curRuleNum = 0, curNumTurns = 0, curStartPos = 0, curTypeCodeValue = 0;
			Int16 prevStartPos, prevNumTurns, prevTypeCodeValue, prevRuleNum;
			String prevCode;
			String curCode = (String)inViewRow["Code"];
			int curIdx = inRowIdx;

			DataRow curTrickRow = getTrickRow( curCode, (Byte)inViewRow["Skis"], inSkierClass );
			if ( curTrickRow == null ) return -1;

			curStartPos = (Byte)curTrickRow["StartPos"];
			curNumTurns = (Byte)curTrickRow["NumTurns"];
			curTypeCodeValue = (Byte)curTrickRow["TypeCode"];
			curRuleNum = (Int16)( (Int16)curTrickRow["RuleNum"] + ( (Byte)inViewRow["Skis"] * 100 ) );

			inViewRow["StartPos"] = curStartPos;
			inViewRow["NumTurns"] = curNumTurns;
			inViewRow["RuleNum"] = curRuleNum;
			inViewRow["TypeCode"] = curTypeCodeValue;

			if ( curIdx == 0 ) return (Int16)curTrickRow["Points"];
			curIdx--;
			prevCode = (String)activePassDataTable.Rows[curIdx]["Code"];
			prevRuleNum = (Int16)activePassDataTable.Rows[curIdx]["RuleNum"];
			prevStartPos = (Byte)activePassDataTable.Rows[curIdx]["StartPos"];
			prevNumTurns = (Byte)activePassDataTable.Rows[curIdx]["NumTurns"];
			prevTypeCodeValue = (Byte)activePassDataTable.Rows[curIdx]["TypeCode"];

			if ( prevCode.ToUpper().Equals( "FALL" ) ) {
				if ( curStartPos == 0 ) return (Int16)curTrickRow["Points"];
				if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
					if ( curStartPos == 0 ) return (Int16)curTrickRow["Points"];
				} else {
					if ( curStartPos == 1 ) return (Int16)curTrickRow["Points"];
				}
			}
			
			return (Int16)curTrickRow["Points"];
		}

		private Int16 calcPointsReverseTrick( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, int inRowIdx, String inColPrefix, String inSkierClass ) {
			Int16 curRuleNum = 0, curNumTurns = 0, curStartPos = 0, curTypeCodeValue = 0;
			Int16 prevStartPos, prevNumTurns, prev0NumTurns, prevTypeCodeValue, prevRuleNum;
			String prevCode;
			String curCode = (String)inViewRow["Code"];
			int curIdx = inRowIdx;

			DataRow curTrickRow = getTrickRow( curCode, (Byte)inViewRow["Skis"], inSkierClass );
			if ( curTrickRow == null ) return -1;
			
			curStartPos = (Byte)curTrickRow["StartPos"];
			curNumTurns = (Byte)curTrickRow["NumTurns"];
			curTypeCodeValue = (Byte)curTrickRow["TypeCode"];
			curRuleNum = (Int16)( (Int16)curTrickRow["RuleNum"] + ( ( (Byte)inViewRow["Skis"] * 100 ) + 200 ) );

			inViewRow["StartPos"] = curStartPos.ToString();
			inViewRow["NumTurns"] = curNumTurns.ToString();
			inViewRow["RuleNum"] = curRuleNum.ToString();
			inViewRow["TypeCode"] = curTypeCodeValue.ToString();

			curIdx--;
			prevCode = (String)activePassDataTable.Rows[curIdx]["Code"];
			prevRuleNum = (Int16)activePassDataTable.Rows[curIdx]["RuleNum"];
			prevStartPos = (Byte)activePassDataTable.Rows[curIdx]["StartPos"];
			prevNumTurns = (Byte)activePassDataTable.Rows[curIdx]["NumTurns"];
			prevTypeCodeValue = (Byte)activePassDataTable.Rows[curIdx]["TypeCode"];

			if ( ( prevNumTurns % 2 ) == 0 ) {
				//Determine points for reverse trick when previous trick had an even number of 180 degree turns
				if ( curCode.Length == 1 ) {
					if ( prevCode.Substring( 0, 1 ).Equals( "R" ) ) {
						curCode = prevCode;
					} else {
						curCode = curCode + prevCode;
					}
					curTrickRow = getTrickRow( curCode, (Byte)inViewRow["Skis"], inSkierClass );
					if ( curTrickRow == null ) return -1;
					
					curStartPos = (Byte)curTrickRow["StartPos"];
					curNumTurns = (Byte)curTrickRow["NumTurns"];
					curTypeCodeValue = (Byte)curTrickRow["TypeCode"];
					curRuleNum = (Int16)( (Int16)curTrickRow["RuleNum"] + ( ( (Byte)inViewRow["Skis"] * 100 ) + 200 ) );

					inViewRow["Code"] = curCode;
					inViewRow["StartPos"] = curStartPos;
					inViewRow["NumTurns"] = curNumTurns;
					inViewRow["RuleNum"] = curRuleNum;
					inViewRow["TypeCode"] = curTypeCodeValue;
				}

				if ( curStartPos == prevStartPos ) {
					if ( curCode.ToUpper().Equals( prevCode.ToUpper() ) ) return (Int16)curTrickRow["Points"];
					if ( prevRuleNum == ( curRuleNum - 200 ) ) return (Int16)curTrickRow["Points"];

					myValidationMessage = "Points not allowed "
						+ "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
					return -1;
				}

				myValidationMessage = "Points not allowed "
					+ "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
				return -1;
			}

			//Determine points for a reverse trick when previous trick had an odd number of 180 degree turns
			if ( ( (Int16)( curRuleNum - 200 ) ) == prevRuleNum ) {
				if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
					if ( curStartPos == 0 ) return (Int16)curTrickRow["Points"];

					myValidationMessage = "Points not allowed "
						+ "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
					return -1;
				}

				if ( curStartPos == 1 ) return (Int16)curTrickRow["Points"];
				myValidationMessage = "Points not allowed "
					+ "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
				return -1;
			}

			if ( curIdx <= 0 ) return -1;  // Not sure how this could happen since this is marked as a reverse trick code

			prev0NumTurns = prevNumTurns;
			curIdx--;
			prevCode = (String)activePassDataTable.Rows[curIdx]["Code"];
			prevRuleNum = (Int16)activePassDataTable.Rows[curIdx]["RuleNum"];
			prevStartPos = (Byte)activePassDataTable.Rows[curIdx]["StartPos"];
			prevNumTurns = (Byte)activePassDataTable.Rows[curIdx]["NumTurns"];
			prevTypeCodeValue = (Byte)activePassDataTable.Rows[curIdx]["TypeCode"];

			if ( prevCode.Substring( 0, 1 ).Equals( "R" ) ) {
				curCode = prevCode;
			} else {
				if ( curCode.Length > 1 ) {
				} else {
					curCode = curCode + prevCode;
				}
			}
			curTrickRow = getTrickRow( curCode, (Byte)inViewRow["Skis"], inSkierClass );
			if ( curTrickRow == null ) return -1;

			curStartPos = (Byte)curTrickRow["StartPos"];
			curNumTurns = (Byte)curTrickRow["NumTurns"];
			curRuleNum = (Int16)( (Int16)curTrickRow["RuleNum"] + ( ( (Byte)inViewRow["Skis"] * 100 ) + 200 ) );
			curTypeCodeValue = (Byte)curTrickRow["TypeCode"];

			inViewRow["Code"] = curCode;
			inViewRow["StartPos"] = curStartPos;
			inViewRow["NumTurns"] = curNumTurns;
			inViewRow["RuleNum"] = curRuleNum;
			inViewRow["TypeCode"] = curTypeCodeValue;

			if ( curStartPos == prevStartPos && prev0NumTurns == 1 ) {
				if ( curCode.ToUpper().Equals( prevCode.ToUpper() ) ) return (Int16)curTrickRow["Points"];
				if ( prevRuleNum == ( curRuleNum - 200 ) ) return (Int16)curTrickRow["Points"];

				myValidationMessage = "Points not allowed "
					+ "\n Current trick " + curCode + " not an appropriate reverse for " + prevCode;
				return -1;
			}

			myValidationMessage = "Points not allowed for " + curCode
				+ "\n More than 180 degrees has occurred between a trick and its reverse";
			inViewRow["Results"] = "Repeat";
			return -1;
		}

		private Boolean checkForMaxNumFlips( DataTable inPass1DataTable, DataTable inPass2DataTable ) {
			DataTable curTricksPerformed = buildFlipsPerformed();

			byte curPassNum = 1;
			for ( byte curIdx = 0; curIdx < inPass1DataTable.Rows.Count; curIdx++ ) {
				checkViewRowForFlip( curTricksPerformed, inPass1DataTable.Rows[curIdx], curIdx, curPassNum );
			}

			curPassNum = 2;
			for ( byte curIdx = 0; curIdx < inPass2DataTable.Rows.Count; curIdx++ ) {
				checkViewRowForFlip( curTricksPerformed, inPass2DataTable.Rows[curIdx], curIdx, curPassNum );
			}

			if ( curTricksPerformed.Rows.Count > 6 ) {
                // Trick with lowest point value is marked as a repeat
				curTricksPerformed.DefaultView.Sort = "Points DESC, PassNum ASC, RowIdx ASC";
				DataTable curTricksPerformedSorted = curTricksPerformed.DefaultView.ToTable();
				for (byte curIdx = 6; curIdx < curTricksPerformedSorted.Rows.Count; curIdx++ ) {
					byte curRowIdx = (byte) curTricksPerformedSorted.Rows[curIdx]["RowIdx"];
					if ( (byte)curTricksPerformedSorted.Rows[curIdx]["PassNum"] == 1 ) {
						inPass1DataTable.Rows[curRowIdx]["Score"] = 0;
						inPass1DataTable.Rows[curRowIdx]["Results"] = "Repeat";
					} else {
                        inPass2DataTable.Rows[curRowIdx]["Score"] = 0;
						inPass2DataTable.Rows[curRowIdx]["Results"] = "Repeat";
					}
				}

                myValidationMessage = "The maximum of 6 flips for IWWF has been exceeded"
                    + "\nThe flip(s) with the lowest point value have been marked as repeat";

                return true;
			}

			return false;
		}

		private void checkViewRowForFlip( DataTable curTricksPerformed, DataRow curViewRow, byte curRowIdx, byte curPassNum ) {
			String curResults = "", curMsg = "";
			Int16 tempTypeCode = 0;

			try {
				tempTypeCode = (Int16) ( (byte) curViewRow["TypeCode"] );
				if ( tempTypeCode == 1 ) {
					curResults = (String) curViewRow["Results"];
					if ( curResults.Equals( "Credit" ) ) {
						DataRowView newFlipRow = curTricksPerformed.DefaultView.AddNew();
						newFlipRow["TrickCode"] = (String) curViewRow["Code"];
						newFlipRow["Skis"] = (byte) curViewRow["Skis"];
						newFlipRow["Points"] = (Int16) curViewRow["Score"];
						newFlipRow["PassNum"] = curPassNum;
						newFlipRow["RowIdx"] = curRowIdx;
						newFlipRow.EndEdit();
					}
				}
			} catch ( Exception ex ) {
				curMsg = "Exception: " + ex.Message;
			}
		}

		private DataTable buildFlipsPerformed() {
			/* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
			DataTable curDataTable = new DataTable();

			DataColumn curCol = new DataColumn();
			curCol.ColumnName = "TrickCode";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Skis";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Points";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PassNum";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "RowIdx";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			return curDataTable;
		}

		private Int16 checkForRepeat( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, DataRow inCheckedRow
            , int inRowViewIdx, int inRowCheckedIdx, String inColPrefix, bool inActiveView, Int16 curRuleNum, Int16 prevRuleNum, Int16 inActivePoints ) {

            Int16 retPoints = inActivePoints;
            if ( curRuleNum == prevRuleNum ) {
                return checkCreditAndPoints(activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inRowViewIdx, inRowCheckedIdx, inColPrefix, inActiveView, retPoints, curRuleNum, prevRuleNum );
            } 
			
			if ( curRuleNum > 100 && curRuleNum < 200 ) {
                if ( ( curRuleNum + 200 ) == prevRuleNum ) {
                    if ( inActiveView ) {
                        if ( inRowViewIdx > inRowCheckedIdx ) {
                            return checkCreditAndPoints(activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inRowViewIdx, inRowCheckedIdx, inColPrefix, inActiveView, retPoints, curRuleNum, prevRuleNum);
                        }
                    } else {
                        if ( inColPrefix.Equals("Pass2") ) {
                            return checkCreditAndPoints(activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inRowViewIdx, inRowCheckedIdx, inColPrefix, inActiveView, retPoints, curRuleNum, prevRuleNum );
                        }
                    }
                }
            
			} else if ( curRuleNum > 200 && curRuleNum < 300 ) {
                if ( ( curRuleNum + 200 ) == prevRuleNum ) {
                    return checkCreditAndPoints(activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inRowViewIdx, inRowCheckedIdx, inColPrefix, inActiveView, retPoints, curRuleNum, prevRuleNum );
                }
            }
            
			return retPoints;
        }

        private Int16 checkCreditAndPoints( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, DataRow inCheckedRow
            , int inRowViewIdx, int inRowCheckedIdx, String inColPrefix, bool inActiveView, Int16 inActivePoints, Int16 curRuleNum, Int16 checkRuleNum ) {
            //If trick repeated determine if previous trick was same number but worth more or less points

            Int16 retPoints = inActivePoints;
            try {
				// If the current trick and the trick to be checked are not both for credit then the number of points is the tricks original value
				if ( !( ( (String)inViewRow["Results"] ).Equals( "Credit" ) ) || !( ( (String)inCheckedRow["Results"] ).Equals( "Credit" )) ) return retPoints;

				// Current trick and the trick to be checked are both for credit then analyze tricks to determine if the original trick points should be changed
				if ( (Int16)inCheckedRow["Score"] == retPoints ) {
					#region Current trick value and trick to be checked have the same value
					if ( inActiveView ) {

						#region Current trick and trick to be checked are in the same pass
						if ( inRowViewIdx < inRowCheckedIdx ) {
							inCheckedRow["Score"] = 0;
							inCheckedRow["Results"] = "Repeat";
							return retPoints;
						}

						if ( ( curRuleNum + 200 ) == checkRuleNum ) {
							byte curNumTurns = (byte)inViewRow["NumTurns"];
							int prevRowIdx = inRowViewIdx - 1;
							if ( ( curNumTurns % 2 ) > 0 ) prevRowIdx = inRowViewIdx - 2;
							if ( prevRowIdx > inRowCheckedIdx ) {
								inViewRow["Score"] = 0;
								inViewRow["Results"] = "Repeat";
								return 0;
							}
							
							prevRowIdx = inRowCheckedIdx - 1;
							if ( ( curNumTurns % 2 ) > 0 ) prevRowIdx = inRowCheckedIdx - 2;
							Int16 prevRuleNum = (Int16)activePassDataTable.Rows[prevRowIdx]["RuleNum"];
							if ( curRuleNum != prevRuleNum || (Int16)activePassDataTable.Rows[prevRowIdx]["Score"] > 0 ) {
								inViewRow["Score"] = 0;
								inViewRow["Results"] = "Repeat";
								return 0;
							}
							return retPoints;
						}

						inViewRow["Score"] = 0;
						inViewRow["Results"] = "Repeat";
						return 0;
						#endregion
					}

					#region Current trick and trick to be checked are in different passes
					if ( ( curRuleNum + 200 ) == checkRuleNum ) {
						byte curNumTurns = (byte)inViewRow["NumTurns"];
						int prevRowIdx = inRowCheckedIdx - 1;
						if ( ( curNumTurns % 2 ) > 0 ) prevRowIdx = inRowCheckedIdx - 2;
						Int16 prevRuleNum = (Int16)activePassDataTable.Rows[prevRowIdx]["RuleNum"];
						if ( curRuleNum == prevRuleNum && (Int16)activePassDataTable.Rows[prevRowIdx]["Score"] == 0 ) return retPoints;
						
						if ( inColPrefix.Equals( "Pass1" ) ) {
							inCheckedRow["Score"] = 0;
							inCheckedRow["Results"] = "Repeat";

						} else {
							inViewRow["Score"] = 0;
							inViewRow["Results"] = "Repeat";
						}
						return 0;
					}

					inViewRow["Score"] = 0;
					inViewRow["Results"] = "Repeat";
					return 0;
					#endregion
					#endregion
				} 
				
				if ( (Int16)inCheckedRow["Score"] < retPoints ) {
					#region Current trick value is greater than trick to be checked have the same value
					if ( inActiveView ) {
						if ( inRowViewIdx > 0 ) {
							if ( inRowViewIdx > inRowCheckedIdx ) {
								inCheckedRow["Score"] = 0;
								inCheckedRow["Results"] = "Repeat";
								return retPoints;
							}

							inViewRow["Score"] = 0;
							inViewRow["Results"] = "Repeat";
							return 0;
						}
						
						if ( ( (String)inCheckedRow["Code"] ).ToLower().Equals( "t5b" )
							&& ( ( (String)inViewRow["Code"] ).ToLower().Equals( "t7f" ) || ( (String)inViewRow["Code"] ).ToLower().Equals( "t7" ) )
							) {
							retPoints = inActivePoints;
							return retPoints;
						}
						
						inCheckedRow["Score"] = 0;
						inCheckedRow["Results"] = "Repeat";
						return 0;
					} 
					
					if ( inColPrefix.Equals( "Pass1" ) ) {
						inCheckedRow["Score"] = 0;
						inCheckedRow["Results"] = "Repeat";
						return retPoints;
					}

					inViewRow["Score"] = 0;
					inViewRow["Results"] = "Repeat";
					return 0;
					#endregion
				}

				#region Current trick value is less than trick to be checked
				if ( inActiveView ) {
					if ( inRowViewIdx > 0 ) {
						if ( inRowViewIdx > inRowCheckedIdx ) {
							//If the CheckRow is 1 row behind (2 for a 180 trick) and is a reverse trick
							//Check the previous trick to see if same trick number but is not for credit
							//If that is the case then this trick should get credit
							// prevRuleNum = (Int16)activePassDataTable.Rows[curIdx]["RuleNum"];
							// prevStartPos = (Byte) activePassDataTable.Rows[curIdx]["StartPos"];
							// prevNumTurns = (Byte) activePassDataTable.Rows[curIdx]["NumTurns"];
							Int16 curNumTurns = (Byte)inViewRow["NumTurns"];
							if ( ( ( curNumTurns % 2 ) == 0 ) && ( ( inRowViewIdx - 1 ) == inRowCheckedIdx ) && ( inRowCheckedIdx > 0 ) ) {
								Int16 prevRuleNum = (Int16)activePassDataTable.Rows[inRowCheckedIdx - 1]["RuleNum"];
								if ( curRuleNum == prevRuleNum && (Int16)activePassDataTable.Rows[inRowCheckedIdx - 1]["Score"] == 0 ) return retPoints;

								inViewRow["Score"] = 0;
								inViewRow["Results"] = "Repeat";
								return 0;
							}

							if ( ( ( curNumTurns % 2 ) > 0 ) && ( ( inRowViewIdx - 2 ) == inRowCheckedIdx ) && ( inRowCheckedIdx > 1 ) ) {
								Int16 prevRuleNum = (Int16)activePassDataTable.Rows[inRowCheckedIdx - 2]["RuleNum"];
								if ( curRuleNum == prevRuleNum && (Int16)activePassDataTable.Rows[inRowCheckedIdx - 2]["Score"] == 0 ) return retPoints;

								inViewRow["Score"] = 0;
								inViewRow["Results"] = "Repeat";
								return 0;
							}

							inViewRow["Score"] = 0;
							inViewRow["Results"] = "Repeat";
							return 0;

						}

						inCheckedRow["Score"] = 0;
						inCheckedRow["Results"] = "Repeat";
						return retPoints;
					}

					inCheckedRow["Score"] = 0;
					inCheckedRow["Results"] = "Repeat";
					return retPoints;
				}

				if ( inColPrefix.Equals( "Pass1" ) ) {
					inCheckedRow["Score"] = 0;
					inCheckedRow["Results"] = "Repeat";
					return retPoints;
				}

				inViewRow["Score"] = 0;
				inViewRow["Results"] = "Repeat";
				return 0;

				#endregion

			} catch ( Exception ex ) {
                String curMsg = ex.Message;
				return retPoints;
			}
		}


        public DataRow getTrickRow( DataRow inDataRow, String inSkierClass ) {
            return getTrickRow((String) inDataRow["Code"], (Int16)((Byte) inDataRow["Skis"]), inSkierClass);
        }
        public DataRow getTrickRow( String inTrickCode, Int16 inNumSkies, String inSkierClass ) {
			DataRow[] curFoundList = myTrickListDataTable.Select("TrickCode = '" + inTrickCode + "'" + " AND NumSkis = " + inNumSkies);
            if ( curFoundList.Length > 0 ) return curFoundList[0];

			return null;
        }

    }
}
