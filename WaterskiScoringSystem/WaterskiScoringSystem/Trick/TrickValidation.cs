using System;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;
namespace WaterskiScoringSystem.Trick {
	class TrickValidation {
		private String myTourRules = "";
		private String myValidationMessage = "";
		private String myUpdatedTrickCode = "";
		private Int16 myNumSkis = -1;

		private DataRow myClassCRow = null;
		private DataRow myClassERow = null;

		private String[] myAllowedRepeatReverseList = { "RS", "RTS", "RB", "RF", "RTB", "RTF" };

		private DataTable myTrickListDataTable = null;
		private DataTable mySkierClassDataTable = null;

		public TrickValidation() {
			myValidationMessage = "";
		}

		public DataTable TrickListDataTable {
			get {
				return myTrickListDataTable;
			}
			set {
				myTrickListDataTable = value;
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

		public String setNumSkisDisplay( Int16 inNunSkis ) {
			if ( inNunSkis == 0 ) return "W";
			else if ( inNunSkis == 9 ) return "K";
			else return inNunSkis.ToString();
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
			Int16 curStartPos;
			Int16 prevStartPos, prevNumTurns;
			String prevCode;
			int prevTrickIdx = 0;
			DataRow curTrickRow;
			Regex curRegexAlphaNumeric = new Regex( @"^[a-zA-Z0-9\s,]*$" );
			myUpdatedTrickCode = "";

			if ( inCode == null || inCode.Length == 0 ) {
				myValidationMessage = "Trick code is required";
				return false;
			}

			if ( !(curRegexAlphaNumeric.IsMatch( inCode )) ) {
				myValidationMessage = "Trick code contains invalid characters";
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
			Int16 curStartPos;
			Int16 prevStartPos;
			String curCode, prevCode;
			myUpdatedTrickCode = "";

			int prevTrickIdx = 0;
			prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
			prevStartPos = (byte)prevTrickRows[prevTrickIdx]["StartPos"];

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
					+ "\n Trick Code {0} ({1}) on {2} ski(s) is not valid"
					, inCode, curCode, inNumSkis.ToString() );
				return false;
			}

			//Check to ensure 360 multiples tricks has an appropriate starting position on the previous 2 tricks
			curStartPos = (Byte)curTrickRow["StartPos"];
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
			Int16 curNumTurns, curStartPos, curRuleNum;
			Int16 prevStartPos, prevNumTurns, prevRuleNum;
			String curCode, prevCode;
			myUpdatedTrickCode = "";

			if ( prevTrickRows.Length != 2 ) {
				myValidationMessage = String.Format( "Found {0} entries in previous trick list but expecting 2 for tricks with an odd number of 180 degree turns"
					, prevTrickRows.Length );
				return false;
			}

			int prevTrickIdx = 0;
			prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
			prevStartPos = (byte)prevTrickRows[prevTrickIdx]["StartPos"];
			prevNumTurns = (byte)prevTrickRows[prevTrickIdx]["NumTurns"];
			prevRuleNum = (Int16)prevTrickRows[prevTrickIdx]["RuleNum"];
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
			DataRow curTrickRow = getTrickRow( curCode, inNumSkis, inSkierClass );
			if ( curTrickRow == null ) {
				myValidationMessage = String.Format( "Invalid trick code for number of skis"
					+ "\n Trick Code {0} ({1}) on {2} ski(s) is not valid"
					, inCode, curCode, inNumSkis.ToString() );
				return false;
			}

			if ( prevNumTurns > 1 ) {
				// Trick is not a reverse because more than 180 degrees between a trick and the reverse, therefore just a repeat
				myUpdatedTrickCode = prevCode;
				return true;
			}

			//Check to see if starting position of previous trick is appropriate
			curStartPos = (Byte)curTrickRow["StartPos"];
			curNumTurns = (Byte)curTrickRow["NumTurns"];
			curRuleNum = (Int16)( (Int16)curTrickRow["RuleNum"] + ( inNumSkis * 100 ) );

			if ( ( ( prevStartPos + prevNumTurns ) % 2 ) == 0 ) {
				if ( curStartPos == 0 ) {
					//Check to see if starting position of 2nd previous trick is also appropriate
					prevCode = (String)prevTrickRows[prevTrickIdx]["TrickCode"];
					prevStartPos = (byte)prevTrickRows[prevTrickIdx]["StartPos"];
					if ( curStartPos == prevStartPos ) {
						myUpdatedTrickCode = curCode;
						return true;
					}
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
			if ( curStartPos == prevStartPos ) {
				myUpdatedTrickCode = curCode;
				return true;
			}

			myValidationMessage = String.Format( "Trick sequence is not possible, {0} following {1} is not possible", curCode, prevCode );
			return false;
		}

		public Int16 calcPoints( DataTable inPass1DataTable, DataTable inPass2DataTable, DataRow inViewRow, int inRowIdx, String inColPrefix, String inSkierClass ) {
			Int16 retPoints;
			if ( inRowIdx < 0 ) {
				myValidationMessage = "There is no active row";
				return -1;
			}

			DataTable activePassDataTable, idlePassDataTable;
			if ( inColPrefix.Equals( "Pass1" ) ) {
				activePassDataTable = inPass1DataTable;
				idlePassDataTable = inPass2DataTable;
			} else {
				activePassDataTable = inPass2DataTable;
				idlePassDataTable = inPass1DataTable;
			}

			myValidationMessage = "";
			String curCode = (String)inViewRow["Code"];
			if ( curCode.Length <= 0 ) {
				myValidationMessage = "Trick code has not been provided or there is no active row";
				return -1;
			}

			// Determine points for current trick code
			if ( curCode.ToUpper().Equals( "R" ) || curCode.Substring( 0, 1 ).ToUpper().Equals( "R" ) ) {
				// Determine points associated with reverse trick referenced 
				retPoints = calcPointsReverseTrick( activePassDataTable, idlePassDataTable, inViewRow, inRowIdx, inColPrefix, inSkierClass );

			} else {
				retPoints = calcPointsPrimaryTrick( activePassDataTable, idlePassDataTable, inViewRow, inRowIdx, inColPrefix, inSkierClass );
			}

			// Determine if trick has previously been successfully performed
			if ( retPoints > 0 ) retPoints = checkPrevForRepeat( activePassDataTable, idlePassDataTable, inViewRow, inRowIdx, curCode, inColPrefix, retPoints );
			if ( retPoints > 0 ) retPoints = checkMaxNumFlips( activePassDataTable, idlePassDataTable, inViewRow, inRowIdx, inColPrefix, retPoints, inSkierClass );
			return retPoints;
		}

		private Int16 checkPrevForRepeat( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, int inRowIdx, String curCode, String inColPrefix, Int16 inPoints ) {
			Int16 prevRuleNum;
			Int16 retPoints = inPoints;
			int prevRowIdx = inRowIdx - 1;
			Int16 curRuleNum = (Int16)inViewRow["RuleNum"];

			//Search previous tricks on active pass to determine if trick successfully performed previously
			if ( curCode.ToLower().Equals( "t5b" ) && inRowIdx > 0 ) {
				String prevCode = (String)activePassDataTable.Rows[prevRowIdx]["Code"];
				if ( prevCode.ToLower().Equals( "t7f" ) || prevCode.ToLower().Equals( "t7" ) ) prevRowIdx = -1;
			}

			while ( prevRowIdx >= 0 ) {
				prevRuleNum = (Int16)activePassDataTable.Rows[prevRowIdx]["RuleNum"];
				retPoints = checkForRepeat( activePassDataTable, idlePassDataTable, inViewRow, activePassDataTable.Rows[prevRowIdx]
					, inRowIdx, prevRowIdx, curRuleNum, prevRuleNum, inColPrefix, true, retPoints );
				if ( retPoints == 0 ) return retPoints;
				prevRowIdx--;
			}

			//If second pass is active then check first pass to determine if trick successfully performed previously
			if ( inColPrefix.Equals( "Pass2" ) ) {
				prevRowIdx = idlePassDataTable.Rows.Count - 1;
				while ( prevRowIdx >= 0 ) {
					prevRuleNum = (Int16)idlePassDataTable.Rows[prevRowIdx]["RuleNum"];
					retPoints = checkForRepeat( activePassDataTable, idlePassDataTable, inViewRow, idlePassDataTable.Rows[prevRowIdx]
						, inRowIdx, prevRowIdx, curRuleNum, prevRuleNum, inColPrefix, false, retPoints );
					if ( retPoints == 0 ) break;
					prevRowIdx--;
				}
			}

			return retPoints;
		}

		/*
		 * Determine if maxinum number of flips have previously been accomplished
		 */
		private Int16 checkMaxNumFlips( DataTable activePassDataTable, DataTable idlePassDataTable, DataRow inViewRow, int inRowIdx, String inColPrefix, Int16 inPoints, String inSkierClass ) {
			Int16 retPoints = inPoints;
			if ( retPoints > 0 ) {
				// This validation is only performed for class L and R skiers
				DataRow curSkierClassRow = mySkierClassDataTable.Select( "ListCode = '" + inSkierClass + "'" )[0];
				if ( (Decimal)curSkierClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"] ) {
					// DataRow inViewRow, int inRowIdx, String inColPrefix
					activePassDataTable.Rows[inRowIdx]["Score"] = retPoints;
					//if ( inColPrefix.Equals( "Pass1" ) ) activePassDataTable.Rows[inRowIdx]["Score"] = retPoints;
					//if ( inColPrefix.Equals( "Pass2" ) ) idlePassDataTable.Rows[inRowIdx]["Score"] = retPoints;
					if ( checkForMaxNumFlips( activePassDataTable, idlePassDataTable ) ) {
						if ( inColPrefix.Equals( "Pass1" ) ) {
							if ( (Int16)activePassDataTable.Rows[inRowIdx]["Score"] == 0 && (String)activePassDataTable.Rows[inRowIdx]["Results"] == "Repeat" ) {
								retPoints = 0;
							}
						} else {
							if ( (Int16)idlePassDataTable.Rows[inRowIdx]["Score"] == 0 && (String)idlePassDataTable.Rows[inRowIdx]["Results"] == "Repeat" ) {
								retPoints = 0;
							}
						}

						activePassDataTable.Rows[inRowIdx]["Score"] = 0;
						//if ( inColPrefix.Equals( "Pass1" ) ) activePassDataTable.Rows[inRowIdx]["Score"] = 0;
						//if ( inColPrefix.Equals( "Pass2" ) ) activePassDataTable.Rows[inRowIdx]["Score"] = 0;
					}
				}
			}
			return retPoints;
		}

		/*
		 * Determine points for trick code entered
		 */
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
				for ( byte curIdx = 6; curIdx < curTricksPerformedSorted.Rows.Count; curIdx++ ) {
					byte curRowIdx = (byte)curTricksPerformedSorted.Rows[curIdx]["RowIdx"];
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
				tempTypeCode = (Int16)( (byte)curViewRow["TypeCode"] );
				if ( tempTypeCode == 1 ) {
					curResults = (String)curViewRow["Results"];
					if ( curResults.Equals( "Credit" ) ) {
						DataRowView newFlipRow = curTricksPerformed.DefaultView.AddNew();
						newFlipRow["TrickCode"] = (String)curViewRow["Code"];
						newFlipRow["Skis"] = (byte)curViewRow["Skis"];
						newFlipRow["Points"] = (Int16)curViewRow["Score"];
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
			, int inViewRowIdx, int inCheckedRowIdx
			, Int16 inViewRowRuleNum, Int16 inRowCheckedRuleNum
			, String inColPrefix, bool inActiveView, Int16 inActivePoints ) {

			if ( inViewRowRuleNum == inRowCheckedRuleNum ) {
				return checkForRepeatCreditAndPoints( activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inViewRowIdx, inCheckedRowIdx, inViewRowRuleNum, inRowCheckedRuleNum, inActiveView, inActivePoints );
			}

			if ( inViewRowRuleNum > 100 && inViewRowRuleNum < 200 ) { // Check for 1 ski tricks

				if ( ( inViewRowRuleNum + 200 ) == inRowCheckedRuleNum ) { // Check if current trick matches a previous reverse tricks
					if ( inActiveView ) {
						return checkForRepeatCreditAndPoints( activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inViewRowIdx, inCheckedRowIdx, inViewRowRuleNum, inRowCheckedRuleNum, inActiveView, inActivePoints );

					} else if ( inColPrefix.Equals( "Pass2" ) ) {
						return checkForRepeatCreditAndPoints( activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inViewRowIdx, inCheckedRowIdx, inViewRowRuleNum, inRowCheckedRuleNum, inActiveView, inActivePoints );
					}
				}

			} else if ( inViewRowRuleNum > 200 && inViewRowRuleNum < 300 ) { // Check for 2 ski tricks
				if ( ( inViewRowRuleNum + 200 ) == inRowCheckedRuleNum ) { // Check if current trick matches a previous reverse tricks
					return checkForRepeatCreditAndPoints( activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inViewRowIdx, inCheckedRowIdx, inViewRowRuleNum, inRowCheckedRuleNum, inActiveView, inActivePoints );
				}

			} else if ( ( inRowCheckedRuleNum + 200 ) == inViewRowRuleNum ) {
				DataRow curViewRowReverseOrig = getPrevTrickRow( activePassDataTable, inViewRow, inViewRowIdx );
				if ( ( (byte)curViewRowReverseOrig["Seq"] ).Equals( (byte)inCheckedRow["Seq"] ) ) return inActivePoints;
				return checkForRepeatCreditAndPoints( activePassDataTable, idlePassDataTable, inViewRow, inCheckedRow, inViewRowIdx, inCheckedRowIdx, inViewRowRuleNum, inRowCheckedRuleNum, inActiveView, inActivePoints );
			}

			return inActivePoints;
		}

		/* 
		 * If trick repeated determine if previous trick was same number but worth more or less points
		 */
		private Int16 checkForRepeatCreditAndPoints( DataTable activePassDataTable, DataTable idlePassDataTable
			, DataRow inViewRow, DataRow inCheckedRow
			, int inViewRowIdx, int inCheckedRowIdx
			, Int16 inViewRowRuleNum, Int16 inCheckedRowRuleNum
			, bool inActiveView, Int16 inActivePoints ) {

			try {
				// If the current trick and the trick to be checked are not both for credit then the number of points is the tricks original value
				String curViewTrickResults = (String)(String)inViewRow["Results"];
				String curCheckTrickResults = (String)inCheckedRow["Results"];
				if ( !( curViewTrickResults.Equals( "Credit" ) && curCheckTrickResults.Equals( "Credit" ) ) ) return inActivePoints;

				// Current trick and the trick to be checked are both for credit then analyze tricks to determine if the original trick points should be changed
				if ( (Int16)inCheckedRow["Score"] == inActivePoints ) {
					return checkForRepeatCreditSamePoints( activePassDataTable, idlePassDataTable
						, inViewRow, inCheckedRow
						, inViewRowIdx, inCheckedRowIdx
						, inViewRowRuleNum, inCheckedRowRuleNum
						, inActiveView, inActivePoints );
				}

				if ( (Int16)inCheckedRow["Score"] < inActivePoints ) {
					// Current trick value is greater than trick to be checked have the same value
					if ( inActiveView ) {
						if ( ( (String)inCheckedRow["Code"] ).ToLower().Equals( "t5b" )
							&& ( ( (String)inViewRow["Code"] ).ToLower().Equals( "t7f" ) || ( (String)inViewRow["Code"] ).ToLower().Equals( "t7" ) )
							&& ( ( inViewRowIdx - 1 ) == inCheckedRowIdx )
							) {
							return inActivePoints;
						}

						inCheckedRow["Score"] = 0;
						inCheckedRow["Results"] = "Repeat";
						return inActivePoints;
					}

					inCheckedRow["Score"] = 0;
					inCheckedRow["Results"] = "Repeat";
					return inActivePoints;
				}

				// Current trick value is less than value of checked trick
				inViewRow["Score"] = 0;
				inViewRow["Results"] = "Repeat";
				return 0;

			} catch ( Exception ex ) {
				String curMsg = ex.Message;
				return inActivePoints;
			}
		}

		/* 
		 * Current trick and check trick are both worth the same point value
		 */
		private Int16 checkForRepeatCreditSamePoints( DataTable activePassDataTable, DataTable idlePassDataTable
				, DataRow inViewRow, DataRow inCheckedRow
				, int inViewRowIdx, int inCheckedRowIdx
				, Int16 inViewRuleNum, Int16 inCheckRuleNum
				, bool inActiveView, Int16 inActivePoints
			) {
			String curViewTrickResults = (String)(String)inViewRow["Results"];
			String curCheckTrickResults = (String)inCheckedRow["Results"];

			if ( ( inViewRuleNum + 200 ) == inCheckRuleNum ) {
				// Current trick is not a reverse but check trick is a reverse therefore considered a repeat
				if ( (Int16)inCheckedRow["Score"] > 0 ) {
					inViewRow["Score"] = 0;
					inViewRow["Results"] = "Repeat";
					return 0;
				}
				return inActivePoints;
			}

			/* 
			 * If current trick is a reverse
			 */
			if ( ( inViewRuleNum == ( inCheckRuleNum + 200 ) ) && ( inViewRowIdx == inCheckedRowIdx ) ) return inActivePoints;

			DataRow curViewRowReverseOrig = getPrevTrickRow( activePassDataTable, inViewRow, inViewRowIdx );
			DataRow curCheckRowReverseOrig = getPrevTrickRow( inActiveView ? activePassDataTable : idlePassDataTable, inCheckedRow, inCheckedRowIdx );

			if ( inViewRuleNum > 300 && inViewRuleNum < 500 ) {
				// Current trick is a reverse and check trick is the original
				String prevViewTrickResults = curViewRowReverseOrig == null ? "" : (String)curViewRowReverseOrig["Results"];
				String prevCheckTrickResults = curCheckRowReverseOrig == null ? "" : (String)curCheckRowReverseOrig["Results"];

				if ( curCheckTrickResults.Equals( "Credit" ) && prevCheckTrickResults.Equals( "Credit" ) ) {
					inViewRow["Score"] = 0;
					inViewRow["Results"] = "Repeat";
					return 0;
				}

				if ( curViewTrickResults.Equals( "Credit" ) && ( prevViewTrickResults.Equals( "Credit" ) || prevViewTrickResults.Equals( "Repeat" ) ) ) {
					curViewRowReverseOrig["Results"] = "Credit";
					curViewRowReverseOrig["Score"] = inActivePoints;

					if ( ((byte)curViewRowReverseOrig["Seq"]).Equals( (byte)inCheckedRow["Seq"] ) ) return inActivePoints;

					if ( curCheckTrickResults.Equals( "Credit" ) ) {
						inCheckedRow["Score"] = 0;
						inCheckedRow["Results"] = "Repeat";

					} else if ( prevCheckTrickResults.Equals( "Credit" ) ) {
						curViewRowReverseOrig["Score"] = 0;
						curViewRowReverseOrig["Results"] = "Repeat";
					}

					return inActivePoints;
				}
			}

			inViewRow["Score"] = 0;
			inViewRow["Results"] = "Repeat";
			return 0;
		}

		private DataRow getPrevTrickRow( DataTable inPassDataTable, DataRow inPassRow, int inRowIdx ) {
			byte curNumTurns = (byte)inPassRow["NumTurns"];
			String curTrickCode = (String)inPassRow["Code"];
			if ( !( curTrickCode.Substring( 0, 1 ).ToUpper().Equals( "R" ) ) ) return null;
			int prevRowIdx = inRowIdx - 1;
			String prevTrickCode = (String)inPassDataTable.Rows[prevRowIdx]["Code"];

			if ( curTrickCode.ToUpper().Equals( "RT5B" ) && inRowIdx == 1
				&& ( prevTrickCode.ToUpper().Equals( "T7" ) || prevTrickCode.ToUpper().Equals( "T7F" ) ) ) {
				return inPassDataTable.Rows[prevRowIdx];
			}

			if ( ( curNumTurns % 2 ) > 0 ) prevRowIdx--;
			if ( prevRowIdx < 0 ) return null;

			// myAllowedRepeatReverseList = { "RS", "RTS", "RB", "RF", "RTB", "RTF" };
			if ( myAllowedRepeatReverseList.Contains( curTrickCode ) ) {
				DataRow curDataRow = inPassDataTable.Rows[prevRowIdx];
				prevTrickCode = (String)curDataRow["Code"];
				String prevTrickResults = (String)curDataRow["Results"];
				while ( curTrickCode == prevTrickCode && prevTrickResults.Equals("No Credit") ) {
					prevRowIdx--;
					if ( ( curNumTurns % 2 ) > 0 ) prevRowIdx--;
					if ( prevRowIdx < 0 ) return null;
					
					curDataRow = inPassDataTable.Rows[prevRowIdx];
					prevTrickCode = (String)curDataRow["Code"];
					prevTrickResults = (String)curDataRow["Results"];
				}
				return curDataRow;
			}

			return inPassDataTable.Rows[prevRowIdx];
		}

		public DataRow getTrickRow( DataRow inDataRow, String inSkierClass ) {
			return getTrickRow( (String)inDataRow["Code"], (Int16)( (Byte)inDataRow["Skis"] ), inSkierClass );
		}
		public DataRow getTrickRow( String inTrickCode, Int16 inNumSkies, String inSkierClass ) {
			DataRow[] curFoundList = myTrickListDataTable.Select( "TrickCode = '" + inTrickCode + "'" + " AND NumSkis = " + inNumSkies );
			if ( curFoundList.Length > 0 ) return curFoundList[0];

			return null;
		}
	}
}
