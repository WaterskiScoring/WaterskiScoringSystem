﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Slalom {
	class SlalomSetAnalysis {
        private DataGridViewRow myTourRegViewRow;
        private DataGridViewRow myRecapViewRow;
        private DataGridViewRowCollection myRecapViewRows;
        
		private DataRow myClassRowSkier;
		private DataRow myPassRow;
		private DataRow myPassTimeRow;

		private Int16 myNumJudges;
		private Int16 myDivMaxSpeedKph;
		private Int16 myPassNumMinSpeed;

		private String myAgeGroup;
		private String myTimeTolMsg;
		private String mySkierPassMsg;

		public SlalomSetAnalysis( DataGridViewRow inTourRegViewRow, DataGridViewRowCollection inRecapViewRows, DataGridViewRow inRecapViewRow, Int16 inNumJudges ) {
            myTourRegViewRow = inTourRegViewRow;
            myRecapViewRow = inRecapViewRow;
            myRecapViewRows = inRecapViewRows;
			myNumJudges = inNumJudges;
			myTimeTolMsg = "";
			mySkierPassMsg = "";

			myClassRowSkier = SlalomEventData.getSkierClass( HelperFunctions.getViewRowColValue( myTourRegViewRow, "EventClass", SlalomEventData.myTourClass ) );
			myAgeGroup = HelperFunctions.getViewRowColValue( myTourRegViewRow, "AgeGroup", "" );

			// Check for adjustment for division or class with a minimum speed
			myPassNumMinSpeed = 0;
			DataTable curMinSpeedDataTable = SlalomEventData.getMinSpeedData( myAgeGroup );
			if ( curMinSpeedDataTable.Rows.Count > 0 ) myPassNumMinSpeed = Convert.ToInt16( (Decimal)curMinSpeedDataTable.Rows[0]["NumPassMinSpeed"] );
		}

		public String TimeTolMsg { get => myTimeTolMsg; set => myTimeTolMsg = value; }
		public String SkierPassMsg { get => mySkierPassMsg; set => mySkierPassMsg = value; }
		public Int16 NumJudges { get => myNumJudges; set => myNumJudges = value; }
		public Int16 DivMaxSpeedKph { get => myDivMaxSpeedKph; set => myDivMaxSpeedKph = value; }
		public DataGridViewRow TourRegViewRow { get => myTourRegViewRow; set => myTourRegViewRow = value; }
        public DataGridViewRow RecapViewRow { get => myRecapViewRow; set => myRecapViewRow = value; }
		public DataRow PassTimeRow { get => myPassTimeRow; set => myPassTimeRow = value; }
		public DataGridViewRowCollection RecapViewRows { get => myRecapViewRows; set => myRecapViewRows = value; }
		public DataRow ClassRowSkier { get => myClassRowSkier; set => myClassRowSkier = value; }

		// Analyze the boat time and fill when determined that short hand entry has been used
		public bool SlalomTimeValidate() {
			decimal tempTime, tempMinDiff, tempMaxDiff;
			mySkierPassMsg = "";

			// Determine if sufficient information is available to calculate a score
			if ( HelperFunctions.isObjectEmpty( myRecapViewRow.Cells["BoatTimeRecap"].Value ) ) return false;
			decimal curScore = calcScoreForPass();
			if ( curScore < 0 ) return false;

            Int16 curPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "PassSpeedKphRecap", "0" ) );
			decimal curPassLineLengthMeters = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "PassLineLengthRecap", "0" );
			myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			
			myPassTimeRow = getBoatTimeRow( curPassSpeedKph, Convert.ToInt16( curScore.ToString().Substring( 0, 1 ) ) );
			decimal curMinTime = HelperFunctions.getDataRowColValueDecimal( myPassTimeRow, "MinValue", 0 );
			decimal curMaxTime = HelperFunctions.getDataRowColValueDecimal( myPassTimeRow, "MaxValue", 0 );
			decimal curActualTime = HelperFunctions.getDataRowColValueDecimal( myPassTimeRow, "CodeValue", 0 );

            bool curBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "BoatPathGoodRecap", "Y" ) );

            String curBoatTimeOrigValue = myRecapViewRow.Cells["BoatTimeRecap"].Value.ToString();
			if ( curBoatTimeOrigValue.ToUpper().Equals( "OK" ) ) {
				myRecapViewRow.Cells["BoatTimeRecap"].Value = curActualTime.ToString( "#0.00" );
				curBoatTimeOrigValue = curActualTime.ToString( "#0.00" );

			} else if ( curBoatTimeOrigValue.ToUpper().Equals( "NONE" ) ) {
				myRecapViewRow.Cells["BoatTimeRecap"].Value = ( curActualTime * -1 ).ToString( "##0.00" );
				curBoatTimeOrigValue = ( curActualTime * -1 ).ToString( "##0.00" );

			} else if ( curBoatTimeOrigValue.Length == 1 ) {
				curBoatTimeOrigValue = "0" + curBoatTimeOrigValue;
			}

			// If 2 digits entered then analyze time range and determine the proper full time
			if ( curBoatTimeOrigValue.Length == 2 ) {
				if ( !( curBoatTimeOrigValue.Contains( "." ) ) ) {
					Int32 delimPos = 0;
					String newValue = "";

					delimPos = curActualTime.ToString().IndexOf( '.' );
					newValue = curActualTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
					tempTime = Convert.ToDecimal( newValue );
					if ( ( tempTime < curMinTime ) || ( tempTime > curMaxTime ) ) {
						delimPos = curMinTime.ToString().IndexOf( '.' );
						newValue = curMinTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
						tempTime = Convert.ToDecimal( newValue );
						if ( tempTime < curMinTime ) {
							tempMinDiff = curMinTime - tempTime;
							newValue = curMaxTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
							tempTime = Convert.ToDecimal( newValue );
							if ( tempTime > curMaxTime ) {
								tempMaxDiff = tempTime - curMaxTime;
								if ( tempMaxDiff > tempMinDiff ) {
									newValue = curMinTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
								} else {
									newValue = curMaxTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
								}
							}
						}
					}

					myRecapViewRow.Cells["BoatTimeRecap"].Value = newValue;

					if ( curBoatPathGood ) {
						myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
						myRecapViewRow.Cells["RerideRecap"].Value = "N";
						myRecapViewRow.Cells["RerideReasonRecap"].Value = "";
					}
				}
			}

			// Determine if entered time is within tolerances
			Decimal curBoatTime = Convert.ToDecimal( myRecapViewRow.Cells["BoatTimeRecap"].Value.ToString() );
			if ( curBoatTime < 0 ) {
				myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
				if ( curBoatPathGood ) {
					myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
					myRecapViewRow.Cells["RerideRecap"].Value = "N";
					myRecapViewRow.Cells["RerideReasonRecap"].Value = "";
				}

				myRecapViewRow.Cells["RerideRecap"].Value = "Y";
				myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
				mySkierPassMsg = "No time was available";
				myRecapViewRow.Cells["RerideReasonRecap"].Value = mySkierPassMsg;
				return false;
			}

			if ( curBoatTime > curMaxTime ) {
				myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
				myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
				myRecapViewRow.Cells["RerideRecap"].Value = "Y";
				myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
				mySkierPassMsg = String.Format( "SLOW time for score {0} Actual={1} F={2} S={3}", curScore.ToString().Substring( 0, 1 ), curActualTime, curMinTime, curMaxTime );
				myRecapViewRow.Cells["RerideReasonRecap"].Value = mySkierPassMsg;
				return false;
			}

			if ( curBoatTime < curMinTime ) {
				myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
				myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
				if ( curScore < 6.0M ) myRecapViewRow.Cells["RerideRecap"].Value = "Y";
				myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
				mySkierPassMsg = String.Format( "FAST time for score {0} Actual={1} F={2} S={3}", curScore.ToString().Substring( 0, 1 ), curActualTime, curMinTime, curMaxTime );
				myRecapViewRow.Cells["RerideReasonRecap"].Value = mySkierPassMsg;
				return false;
			}

			myRecapViewRow.Cells["TimeInTolRecap"].Value = "Y";
			myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;
			if ( curBoatPathGood ) {
				myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
				myRecapViewRow.Cells["RerideRecap"].Value = "N";
				myRecapViewRow.Cells["RerideReasonRecap"].Value = "";
				mySkierPassMsg = String.Format( "Times for score {0} Actual={1} F={2} S={3}", curScore.ToString().Substring( 0, 1 ), curActualTime, curMinTime, curMaxTime );

			} else {
				mySkierPassMsg = HelperFunctions.getViewRowColValue( myRecapViewRow, "RerideReasonRecap", "" );
			}

			return true;
        }

        public Decimal calcScoreForPass() {
			return calcScoreForPass( myRecapViewRow );
        }

        public Decimal calcScoreForPass(DataGridViewRow inRecapViewRow ) {
			Decimal[] curJudgeScore = new Decimal[myNumJudges];
			int[] curGateEntry = new int[myNumJudges];
			int[] curGateExit = new int[myNumJudges];

			if ( myNumJudges == 1 ) {
				if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge1ScoreRecap", "" ) ) ) return -1;
				curJudgeScore[0] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge1ScoreRecap", "0" );
				if ( curJudgeScore[0] < 6 ) inRecapViewRow.Cells["GateExit1Recap"].Value = false;
				if ( isEntryGatesGood() ) return SlalomRecapScoreCalc( curJudgeScore );

				/*
				 * If class C skier and first pass 
				 * If skier missed entrance gates they are allowe to continue to the 2nd pass
				 */
				if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"]
					&& inRecapViewRow.Index == 0 ) {
					Decimal curScore = SlalomRecapScoreCalc( curJudgeScore );
					if ( curScore < 6 ) return 0;
					return curScore;
				}

				return 0;
			}

			if ( myNumJudges == 3 ) {
				if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge1ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge2ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge3ScoreRecap", "" ) )
				   ) return -1;

				curJudgeScore[0] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge1ScoreRecap", "0" );
				curJudgeScore[1] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge2ScoreRecap", "0" );
				curJudgeScore[2] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge3ScoreRecap", "0" );

				if ( isEntryGatesGood() ) return SlalomRecapScoreCalc( curJudgeScore );

				/*
				 * If class C skier and first pass 
				 * If skier missed entrance gates they are allowe to continue to the 2nd pass
				 */
				if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"]
					&& inRecapViewRow.Index == 0 ) {
					Decimal curScore = SlalomRecapScoreCalc( curJudgeScore );
					if ( curScore < 6 ) return 0;
					return curScore;
				}

				return 0;
			}

			if ( myNumJudges == 5 ) {
				if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge1ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge2ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge3ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge4ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inRecapViewRow, "Judge5ScoreRecap", "" ) )
				   ) return -1;

				curJudgeScore[0] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge1ScoreRecap", "0" );
				curJudgeScore[1] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge2ScoreRecap", "0" );
				curJudgeScore[2] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge3ScoreRecap", "0" );
				curJudgeScore[3] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge4ScoreRecap", "0" );
				curJudgeScore[4] = HelperFunctions.getViewRowColValueDecimal( inRecapViewRow, "Judge5ScoreRecap", "0" );

				if ( isEntryGatesGood() ) return SlalomRecapScoreCalc( curJudgeScore );

				/*
				 * If class C skier and first pass 
				 * If skier missed entrance gates they are allowe to continue to the 2nd pass
				 */
				if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"]
					&& inRecapViewRow.Index == 0 ) {
					Decimal curScore = SlalomRecapScoreCalc( curJudgeScore );
					if ( curScore < 6 ) return 0;
					return curScore;
				}

				return 0;
			}

			return -1;
		}

		private Decimal SlalomRecapScoreCalc( Decimal[] inJudgeScores ) {
			if ( myNumJudges == 1 ) return inJudgeScores[0];

			if ( myNumJudges == 3 ) {
				Int16[] usedIndex = { 9, 9, 9 };
				Decimal[] myJudgeScore = new Decimal[myNumJudges];
				for ( Int16 idxSort = 0; idxSort < inJudgeScores.Length; idxSort++ ) {
					for ( Int16 idx = 0; idx < inJudgeScores.Length; idx++ ) {
						if ( !( usedIndex.Contains( idx ) ) ) {
							if ( inJudgeScores[idx] > myJudgeScore[idxSort] ) {
								myJudgeScore[idxSort] = inJudgeScores[idx];
								usedIndex[idxSort] = idx;
							}
						}
					}
				}
				return myJudgeScore[1];
			}

			if ( myNumJudges == 5 ) {
				Int16[] usedIndex = { 9, 9, 9, 9, 9 };
				Decimal[] myJudgeScore = new Decimal[myNumJudges];
				for ( Int16 idxSort = 0; idxSort < inJudgeScores.Length; idxSort++ ) {
					for ( Int16 idx = 0; idx < inJudgeScores.Length; idx++ ) {
						if ( !( usedIndex.Contains( idx ) ) ) {
							if ( inJudgeScores[idx] > myJudgeScore[idxSort] ) {
								myJudgeScore[idxSort] = inJudgeScores[idx];
								usedIndex[idxSort] = idx;
							}
						}
					}
				}

				return myJudgeScore[2];
			}

			return -1;
		}

		public Decimal SlalomScoreCalc( Decimal inScore, Int16 inDivMaxSpeedKph ) {
			String curMethodName = "SlalomScoreCalc";
			Int16 curPassNumMinSpeed = myPassNumMinSpeed;
			myDivMaxSpeedKph = inDivMaxSpeedKph;

			try {
				myRecapViewRow.Cells["ScoreRecap"].Value = inScore.ToString( "#.00" );

				Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( myAgeGroup, myDivMaxSpeedKph );
				Int16 curSkierPassNum = Convert.ToInt16( (String)myRecapViewRow.Cells["skierPassRecap"].Value );

                /*
				 * Check for adjustment for division or class that allows scoring for line shortening alternate score method
				 * 
				 * Skiers in qualified divisions can increase their score either by increasing speed or shortening the rope or both
				 * It is no longer required that the skier be scored at long line when at less than max speed
				 * 
				 * See SlalomBoatPathAnalysis for documentation on IWWF Rule 8.15 Boat Path Deviation rerides and scores
                 */
                Int16 curPassSpeedKph = Convert.ToInt16( (String)myRecapViewRow.Cells["PassSpeedKphRecap"].Value );
				bool isSpeedBelowMax = ( curPassSpeedKph < curMaxSpeedKphDiv );
				Decimal curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapViewRow.Cells["PassLineLengthRecap"].Value );
				myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );

				bool isZbsAllowed = SlalomEventData.isQualifiedAltScoreMethod( myAgeGroup, (String)myClassRowSkier["ListCode"] );
				if ( isZbsAllowed && curPassSpeedKph > curMaxSpeedKphDiv && (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)SlalomEventData.myClassCRow["ListCodeNum"] ) {
					// For E/L/R skiers can't score above division maximum speed
					myPassRow = SlalomEventData.getPassRow( curMaxSpeedKphDiv, curPassLineLengthMeters );
				}

				if ( curSkierPassNum == 1 ) return calcSkierScorePassFirst( inScore, curPassNumMinSpeed, isZbsAllowed, SlalomEventData.isDivisionIntl( myAgeGroup ), isSpeedBelowMax );
				
				return calcSkierScorePass2Plus( inScore, curPassNumMinSpeed, isZbsAllowed, SlalomEventData.isDivisionIntl( myAgeGroup ), isSpeedBelowMax );

			} catch ( Exception ex ) {
				MessageBox.Show( curMethodName + ": \n" + ex.Message );
				return -1;
			}
		}

		/*
		 * Calculate skier score for first pass
		 */
		private Decimal calcSkierScorePassFirst( Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl, bool isSpeedBelowMax ) {
			int curPassSpeedNum = (int)myPassRow["SlalomSpeedNum"];
			int curPassLineNum = (int)myPassRow["SlalomLineNum"];
			if ( !( isZbsAllowed ) && isSpeedBelowMax ) curPassLineNum = 0;
			if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;

			bool curScoreProtCkd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "ScoreProtRecap", "Y" ) );
			bool curTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "TimeInTolRecap", "N" ) );
			bool curBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "BoatPathGoodRecap", "Y" ) );

			if ( curTimeGood && curBoatPathGood ) {
				if ( inScore == 6 && isExitGatesGood() ) {
					if ( isEntryGatesGood() ) {
						setEventRegRowStatus( "2-InProg" );
						return inScore * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed );
					} 
					
					if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
						setEventRegRowStatus( "2-InProg" );
						myRecapViewRow.Cells["RerideRecap"].Value = "Y";
						mySkierPassMsg = "Allowed to continue per rule 10.03 (C)";
						myRecapViewRow.Cells["RerideReasonRecap"].Value = mySkierPassMsg;
						return 0;
					}

					setEventRegRowStatus( "4-Done" );
					return 0;
				}

				setEventRegRowStatus( "4-Done" );
				return inScore;
			} 
			
			if ( curBoatPathGood ) {
				if ( inScore == 6 && isExitGatesGood() ) {
					setEventRegRowStatus( "2-InProg" );
					return inScore * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed );
				}

				setEventRegRowStatus( "3-Error" );
				return inScore;
			} 
			
			if ( curScoreProtCkd ) {
				setEventRegRowStatus( "3-Error" );
				return HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "ProtectedScoreRecap", "0" );
			}

			setEventRegRowStatus( "3-Error" );
			return 0;
		}

		/*
		 * Calculate skier score for all passes after the first
		 */
		private Decimal calcSkierScorePass2Plus( Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl, bool isSpeedBelowMax ) {
			bool curRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "RerideRecap", "Y" ) );
			bool curTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "TimeInTolRecap", "N" ) );
			bool curBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "BoatPathGoodRecap", "Y" ) );

			if ( curTimeGood && curBoatPathGood ) {

				if ( curRerideInd ) {
					return calcSkierScoreGeneralReride( inScore, inPassNumMinSpeed, isZbsAllowed, isDivisionIntl );

				} else if ( inScore == 6 && isExitGatesGood() ) {
					return calcSkierScoreFullPassTimeGood( inScore, inPassNumMinSpeed, isZbsAllowed, isDivisionIntl, isSpeedBelowMax );
				}
				return calcSkierScoreNotFullPassTimeGood( inScore, inPassNumMinSpeed, isZbsAllowed, isDivisionIntl );

			} else if ( curTimeGood ) {
				return calcSkierScorePathNotGood( inScore, inPassNumMinSpeed, isZbsAllowed, isDivisionIntl );

			} else {
				return calcSkierScoreTimeNotGood( inScore, inPassNumMinSpeed, isZbsAllowed, isDivisionIntl );
			}
		}

		/*
		 * Calculate skier score for a full pass after the first when time is good and boat path is good
		 */
		private Decimal calcSkierScoreFullPassTimeGood( Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl, bool isSpeedBelowMax ) {
			int curPassSpeedNum = (int)myPassRow["SlalomSpeedNum"];
			int curPassLineNum = (int)myPassRow["SlalomLineNum"];
			if ( !( isZbsAllowed ) && isSpeedBelowMax ) curPassLineNum = 0;
			if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;

			setEventRegRowStatus( "2-InProg" );
			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( myAgeGroup, myDivMaxSpeedKph );
			int prevIdx = myRecapViewRow.Index - 1;
			DataGridViewRow prevRecapRow = myRecapViewRows[prevIdx];

			// If previous pass was NOT a reride then score is based on completed pass for current pass
			bool prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "N" ) );
			if ( !( prevRerideInd ) ) return 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed );

			// Check attributes for previous pass that required a reirde
			bool prevScoreProtInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreProtRecap", "N" ) );
			bool prevTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "TimeInTolRecap", "N" ) );
			bool prevBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "BoatPathGoodRecap", "N" ) );
            
			decimal curPassScore = inScore;
            decimal prevPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreRecap", "0" ) );

            // If 1st previous pass has a good time and path, then simply use full current pass score
            if ( prevTimeGood && prevBoatPathGood ) return 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed );

            /*
             * If 1st previous pass has a protected score and if previous pass was an optional reride improvement is allowed
             * However if previous pass was mandatory reride then improvement is not allowed
             */
            if ( prevScoreProtInd ) {
				if ( ( (String)prevRecapRow.Cells["RerideReasonRecap"].Value ).ToLower().Contains( "mandatory" ) ) {
                    curPassScore = getProtectedScore( prevRecapRow, inScore );
                    myRecapViewRow.Cells["ScoreRecap"].Value = curPassScore.ToString( "#.00" );
                    return curPassScore + (6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) - 6 );

                } else {
                    return 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed );
                }
            }

            /*
			 * Scoring when time for current pass is good and full pass for class C and below skiers
			 * Note: Class C skiers are allowed to improve score if previous pass required a reride for slow time
			 */
            if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
				return 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed );
			}

			/*
			 * Current pass is a full pass with no reride required for class E and above skiers
			 * Previous pass required a reride due to a slow time
			 */
			if ( ( (String)prevRecapRow.Cells["RerideReasonRecap"].Value ).ToLower().Contains( "slow" )
				&& ( (Decimal)myClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) ) {
				if ( prevPassScore < inScore ) curPassScore = prevPassScore;
			}

			if ( curPassScore == 6 ) return ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );

			/*
			 * Previous pass required reride due to slow boat time but not a full pass
			 * Find first previous available completed pass without a reride requirement
			 */
			myRecapViewRow.Cells["ScoreRecap"].Value = prevPassScore.ToString( "#.00" );
			setEventRegRowStatus( "4-Done" );
			prevRecapRow = null;

			for ( prevIdx--; prevIdx >= 0; prevIdx-- ) {
				prevRecapRow = myRecapViewRows[prevIdx];
				prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "N" ) );
				if ( prevRerideInd ) continue;
				break;
			}

			if ( prevRecapRow == null ) return curPassScore;

			/*
			 * Complete pass with good time but pass is a reride pass so must review previous pass
			 */
			Int16 curPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "PassSpeedKphRecap", "0" ) );
			decimal curPassLineLengthMeters = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "PassLineLengthRecap", "0" );
			DataRow tempPassRow = null;
			if ( curPassSpeedKph > curMaxSpeedKphDiv
				&& (Decimal)myClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"]
				) {
				tempPassRow = SlalomEventData.getPassRow( curMaxSpeedKphDiv, curPassLineLengthMeters );
			} else {
				tempPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			}
			curPassSpeedNum = (int)tempPassRow["SlalomSpeedNum"];
			curPassLineNum = (int)tempPassRow["SlalomLineNum"];
			if ( !( isZbsAllowed ) && isSpeedBelowMax ) curPassLineNum = 0;
			if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;
			return curPassScore + ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );
		}

		/*
		 * Calculate skier score when not a full pass after the first when time is good and boat path is good
		 */
		private Decimal calcSkierScoreNotFullPassTimeGood( Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl ) {
			Decimal curPassScore = inScore;
			int prevRowIdx = myRecapViewRow.Index - 1;
			DataGridViewRow prevRecapRow = myRecapViewRows[prevRowIdx];

			if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
				/*
				 * If Class C skier check to see if this is the first score-able pass 
				 * after being allowed to continued missing entry gates on the first pass due to Rule 10.03C
				 */
				DataGridViewRow tempRecapRow;
				bool tempRerideInd, tempScoreProtInd;
				for ( prevRowIdx = myRecapViewRow.Index - 1; prevRowIdx >= 0; prevRowIdx-- ) {
					tempRecapRow = myRecapViewRows[prevRowIdx];
					tempRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "RerideRecap", "N" ) );
					tempScoreProtInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreProtRecap", "N" ) );
					if ( tempRerideInd && tempScoreProtInd ) {
						curPassScore = getProtectedScore( prevRecapRow, inScore );
						myRecapViewRow.Cells["ScoreRecap"].Value = curPassScore.ToString( "#.00" );
					}
					if ( tempRerideInd ) continue;
					break;
				}
				if ( prevRowIdx < 0 ) prevRowIdx = 0;
				prevRecapRow = myRecapViewRows[prevRowIdx];
			}

			bool prevTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "TimeInTolRecap", "N" ) );
			bool prevBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "BoatPathGoodRecap", "N" ) );
			bool prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "N" ) );

			if ( !(prevRerideInd) ) {
				 // Calculate score when current pass is not a full pass and previous pass was not a reride
				return calcSkierScoreNotFullPassTimeGoodNoRerides( prevRecapRow, curPassScore, inPassNumMinSpeed, isZbsAllowed, isDivisionIntl );
			}

			/*
			 * Calculate score when current pass is not a full pass but previous pass(s) required a reride
			 */
			if ( prevTimeGood && prevBoatPathGood ) {
				return calcSkierScoreNotFullPassTimeGoodPrevRerides( prevRecapRow, curPassScore, inPassNumMinSpeed, isZbsAllowed, isDivisionIntl );
			}

			/*
			 * Calculate score when current pass is not a full pass and previous pass time or boat path required a reride
			 */
			return calcSkierScoreNotFullPassTimeGoodPrevTimeReride( prevRecapRow, curPassScore, inPassNumMinSpeed, isZbsAllowed, isDivisionIntl );

		}

		/*
		 * Calculate score when current pass is not a full pass and previous pass was not a reride
		 */
		private Decimal calcSkierScoreNotFullPassTimeGoodNoRerides( DataGridViewRow prevRecapRow, Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl ) {
			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( myAgeGroup, myDivMaxSpeedKph );
			Int16 curPassSpeedKphOrig = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "PassSpeedKphRecap", "0" ) );

			Int16 curPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "PassSpeedKphRecap", "0" ) );
			decimal curPassLineLengthMeters = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "PassLineLengthRecap", "0" );

			DataRow tempPassRow = null;
			bool isSpeedBelowMax = ( curPassSpeedKphOrig < myDivMaxSpeedKph );

			if ( curPassSpeedKph > curMaxSpeedKphDiv
				&& (Decimal)myClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"]
				) {
				tempPassRow = SlalomEventData.getPassRow( curMaxSpeedKphDiv, curPassLineLengthMeters );
			} else {
				tempPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			}
			int curPassSpeedNum = (int)tempPassRow["SlalomSpeedNum"];
			int curPassLineNum = (int)tempPassRow["SlalomLineNum"];
			if ( !( isZbsAllowed ) && isSpeedBelowMax ) curPassLineNum = 0;
			if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;
			if ( !( isZbsAllowed ) && curPassSpeedKphOrig >= myDivMaxSpeedKph && curPassSpeedKph < myDivMaxSpeedKph ) curPassLineNum = 1;

			setEventRegRowStatus( "4-Done" );
			return inScore + ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );
		}

		/*
		 * Calculate score when current pass is not a full pass but previous pass(s) required a reride
		 */
		private Decimal calcSkierScoreNotFullPassTimeGoodPrevRerides( DataGridViewRow prevRecapRow, Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl ) {
			Int16 curPassSpeedKph;
			int curPassSpeedNum, curPassLineNum;
			decimal skierScore, curPassScore, prevPassScore;
			decimal curPassLineLengthMeters;
			DataRow tempPassRow;

			int prevRowIdx = prevRecapRow.Index;
			bool prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "N" ) );
			bool prevScoreProtInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreProtRecap", "N" ) );

			if ( prevScoreProtInd ) {
				// Determine current pass score when previous reride pass indicated a protected score
				curPassScore = getProtectedScore( prevRecapRow, inScore );
				//myRecapViewRow.Cells["ScoreRecap"].Value = curPassScore.ToString( "#.00" );

			} else {
				if ( prevRowIdx == 0 && (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
					if ( ( (String)prevRecapRow.Cells["RerideReasonRecap"].Value ).Contains( "10.03 (C)" ) ) {
						// Class C Skier that missed entry gates on first pass but completed 2nd pass
						curPassScore = 0;
					
					} else {
						// Previous pass is a reride but without a protected score
						curPassScore = inScore;
					}

				} else if ( ( (String)prevRecapRow.Cells["RerideReasonRecap"].Value ).ToLower().Contains( "slow" )
							&& ( (Decimal)myClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] )
						) {
					// Previous pass had a slow time and skier was not a class C skier
					prevPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreRecap", "0" ) );
					if ( prevPassScore < inScore ) {
						curPassScore = prevPassScore;
					} else {
						curPassScore = inScore;
					}

				} else {
					curPassScore = inScore;
				}
			}

			/*
			 * Find first previous pass that is not a reride
			 */
			skierScore = curPassScore;
			for ( prevRowIdx--; prevRowIdx >= 0; prevRowIdx-- ) {
				prevRecapRow = myRecapViewRows[prevRowIdx];
				prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "N" ) );
				if ( prevRerideInd ) continue;
				break;
			}

			/*
			 * Calculate skier score Using last pass score and full score of last completed clean pass
			 */
			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( myAgeGroup, myDivMaxSpeedKph );
			Int16 curPassSpeedKphOrig = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "PassSpeedKphRecap", "0" ) );

			if ( prevRowIdx >= 0 ) {
				prevPassScore = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "ScoreRecap", "0" );
				if ( prevPassScore == 6 ) {
					curPassLineLengthMeters = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "PassLineLengthRecap", "0" );
					curPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "PassSpeedKphRecap", "0" ) );
					if ( curPassSpeedKph > curMaxSpeedKphDiv && (Decimal)myClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
						tempPassRow = SlalomEventData.getPassRow( curMaxSpeedKphDiv, curPassLineLengthMeters );
					} else {
						tempPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
					}
					curPassSpeedNum = (int)tempPassRow["SlalomSpeedNum"];
					curPassLineNum = (int)tempPassRow["SlalomLineNum"];

					bool isSpeedBelowMax = ( (curPassSpeedKph < myDivMaxSpeedKph ) || ( curPassSpeedKphOrig < myDivMaxSpeedKph ) );
					if ( !( isZbsAllowed ) && isSpeedBelowMax ) curPassLineNum = 0;
					if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;

					skierScore = curPassScore + ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );
				}
			}

			setEventRegRowStatus( "4-Done" );
			return skierScore;
		}

		/*
		 * Calculate score when current pass is not a full pass
		 * Previous pass time or boat path were required a reride
		 * 
		 * First step is to determine the current pass score depending on the reride reason for the previous pass 
		 */
		private Decimal calcSkierScoreNotFullPassTimeGoodPrevTimeReride( DataGridViewRow prevRecapRow, Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl ) {
			decimal skierScore, curPassScore, prevPassScore;
			decimal curPassLineLengthMeters;
			Int16 curPassSpeedKph;
			int curPassSpeedNum, curPassLineNum;

			int prevRowIdx = prevRecapRow.Index;
			bool prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "N" ) );
			bool prevScoreProtInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreProtRecap", "N" ) );
			bool prevTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "TimeInTolRecap", "N" ) );

			if ( ( prevRowIdx == 1 ) && ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"] )
				&& ( ( (String)myRecapViewRows[0].Cells["RerideReasonRecap"].Value ).Contains( "10.03 (C)" ) ) ) {
				// Class C skier missed entry gate on 1st pass and does not complete 2nd pass scores zero
				curPassScore = 0;

			} else {
				curPassScore = inScore;
				if ( prevScoreProtInd ) {
					// Determine current pass score when previous reride pass indicated a protected score
					curPassScore = getProtectedScore( prevRecapRow, inScore );

				} else {
					if ( !prevTimeGood ) {
						// Reride from previous pass because path not good but score not protected
						prevPassScore = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "ScoreRecap", "0" );
						if ( ( (String)prevRecapRow.Cells["RerideReasonRecap"].Value ).ToLower().Contains( "slow" ) ) {
							if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
								curPassScore = inScore;

							} else if ( prevPassScore < inScore ) curPassScore = prevPassScore;
						}
					}
				}
			}
			myRecapViewRow.Cells["ScoreRecap"].Value = curPassScore.ToString( "#.00" );

			/*
			 * Find the last pass not requiring a reride to determine the pass score
			 */
			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( myAgeGroup, myDivMaxSpeedKph );
			Int16 curPassSpeedKphOrig = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "PassSpeedKphRecap", "0" ) );

			prevPassScore = -1;
			skierScore = curPassScore;
			for ( prevRowIdx--; prevRowIdx >= 0; prevRowIdx-- ) {
				prevRecapRow = myRecapViewRows[prevRowIdx];
				prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "Y" ) );
				prevScoreProtInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreProtRecap", "N" ) );
				if ( prevRerideInd && prevScoreProtInd ) {
					prevPassScore = getProtectedScore( prevRecapRow, inScore );
					skierScore = prevPassScore;
				}
				if ( prevRerideInd ) continue;
				
				prevPassScore = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "ScoreRecap", "0" );
				skierScore = prevPassScore;
				if ( prevPassScore == 6 ) break;
			}

			setEventRegRowStatus( "4-Done" );
			if ( prevPassScore < 6 ) return skierScore;

			curPassLineLengthMeters = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "PassLineLengthRecap", "0" );
			curPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "PassSpeedKphRecap", "0" ) );
			DataRow tempPassRow = null;
			if ( curPassSpeedKph > curMaxSpeedKphDiv && (Decimal)myClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
				tempPassRow = SlalomEventData.getPassRow( curMaxSpeedKphDiv, curPassLineLengthMeters );
			} else {
				tempPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			}
			curPassSpeedNum = (int)tempPassRow["SlalomSpeedNum"];
			curPassLineNum = (int)tempPassRow["SlalomLineNum"];
			bool isSpeedBelowMax = ( ( curPassSpeedKphOrig < myDivMaxSpeedKph) || ( curPassSpeedKphOrig >= myDivMaxSpeedKph && curPassSpeedKph < myDivMaxSpeedKph ) );
			if ( !( isZbsAllowed ) && isSpeedBelowMax ) curPassLineNum = 0;
			if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;

			skierScore = curPassScore + ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );
			return skierScore;
		}

		/*
		 * Calculate skier score when not a full pass after the first when time is good and boat path is good
		 */
		private Decimal calcSkierScoreTimeNotGood( Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl ) {
			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( myAgeGroup, myDivMaxSpeedKph );

			int curPassSpeedNum = (int)myPassRow["SlalomSpeedNum"];
			int curPassLineNum = (int)myPassRow["SlalomLineNum"];
			if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;
			decimal curPassProtScore = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "ProtectedScoreRecap", "0" );

			bool curScoreProtCkd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "ScoreProtRecap", "Y" ) );

			if ( curScoreProtCkd ) {
				if ( curPassProtScore == 0 && inScore == 6 && isExitGatesGood() ) {
					setEventRegRowStatus( "2-InProg" );
					return ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );
				}

				decimal curPassScore = inScore;
				if ( curPassProtScore > 0 ) curPassScore = curPassProtScore;

				int prevRowIdx = myRecapViewRow.Index - 1;
				DataGridViewRow prevRecapRow = myRecapViewRows[prevRowIdx];
				if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
					//For Class C and below tournaments check to see if this is the first score-able pass 
					//after being allowed to continued after the first pass due to Rule 10.03C
					int curTempIdx = prevRowIdx;
					for ( curTempIdx = prevRowIdx; curTempIdx >= 0; curTempIdx-- ) {
						DataGridViewRow tempRecapRow = myRecapViewRows[curTempIdx];
						bool curRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "RerideRecap", "N" ) );
						bool curTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "TimeInTolRecap", "N" ) );

						if ( !curRerideInd || !curTimeGood ) break;
					}
					if ( curTempIdx < 0 ) {
						setEventRegRowStatus( "4-Done" );
						return 0;
					}
				}

				setEventRegRowStatus( "2-InProg" );
				return curPassScore + findScoreLastGoodPass( myRecapViewRow, myClassRowSkier, inPassNumMinSpeed, curMaxSpeedKphDiv, isZbsAllowed, isDivisionIntl );
			}

			setEventRegRowStatus( "2-InProg" );
			return findScoreLastNonReride( myRecapViewRow, myClassRowSkier, inPassNumMinSpeed, curMaxSpeedKphDiv, isZbsAllowed, isDivisionIntl );
		}

		/*
		 * Calculate skier score when not a full pass after the first when time is good and boat path is not good
		 */
		private Decimal calcSkierScorePathNotGood( Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl ) {
			int prevRowIdx = myRecapViewRow.Index - 1;

			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( myAgeGroup, myDivMaxSpeedKph );

			int curPassSpeedNum = (int)myPassRow["SlalomSpeedNum"];
			int curPassLineNum = (int)myPassRow["SlalomLineNum"];
			if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;
			decimal curPassProtScore = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "ProtectedScoreRecap", "0" );

			bool curScoreProtCkd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "ScoreProtRecap", "Y" ) );

			if ( curScoreProtCkd ) {
				if ( curPassProtScore == 0 && inScore == 6 && isExitGatesGood() ) {
					setEventRegRowStatus( "2-InProg" );
					return inScore + ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );
				}

				decimal curPassScore = inScore;
				if ( curPassProtScore > 0 ) curPassScore = curPassProtScore;

				int countReride = countPathDevRerides();
				if ( countReride >= 2 ) {
					myRecapViewRow.Cells["RerideRecap"].Value = "N";
					setEventRegRowStatus( "4-Done" );
					return curPassScore + findScoreLastGoodPass( myRecapViewRow, myClassRowSkier, inPassNumMinSpeed, curMaxSpeedKphDiv, isZbsAllowed, isDivisionIntl );
				}

				DataGridViewRow prevRecapRow = myRecapViewRows[prevRowIdx];
				if ( (Decimal)myClassRowSkier["ListCodeNum"] < (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
					//For Class C and below tournaments check to see if this is the first score-able pass 
					//after being allowed to continued after the first pass due to Rule 10.03C
					int curTempIdx = prevRowIdx;
					for ( curTempIdx = prevRowIdx; curTempIdx >= 0; curTempIdx-- ) {
						DataGridViewRow tempRecapRow = myRecapViewRows[curTempIdx];
						bool curRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "RerideRecap", "N" ) );
						bool curTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "TimeInTolRecap", "N" ) );
						if ( !curRerideInd || !curTimeGood ) break;
					}
					if ( curTempIdx < 0 ) {
						setEventRegRowStatus( "4-Done" );
						return curPassScore;
					}
				}

				setEventRegRowStatus( "2-InProg" );
				return curPassScore + findScoreLastGoodPass( myRecapViewRow, myClassRowSkier, inPassNumMinSpeed, curMaxSpeedKphDiv, isZbsAllowed, isDivisionIntl );
			}

			setEventRegRowStatus( "2-InProg" );
			return findScoreLastGoodPass( myRecapViewRow, myClassRowSkier, inPassNumMinSpeed, curMaxSpeedKphDiv, isZbsAllowed, isDivisionIntl );
		}

		/*
		 * Calculate skier score when reride is indicated with no issue with time or path
		 */
		private Decimal calcSkierScoreGeneralReride( Decimal inScore, Int16 inPassNumMinSpeed, bool isZbsAllowed, bool isDivisionIntl ) {
			int prevRowIdx = myRecapViewRow.Index - 1;

			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( myAgeGroup, myDivMaxSpeedKph );
			bool curScoreProtCkd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "ScoreProtRecap", "Y" ) );
			decimal curPassProtScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapViewRow, "ProtectedScoreRecap", "0" ) );

			int curPassSpeedNum = (int)myPassRow["SlalomSpeedNum"];
			int curPassLineNum = (int)myPassRow["SlalomLineNum"];
			if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;

			if ( curScoreProtCkd ) {
				if ( curPassProtScore == 0 && inScore == 6 && isExitGatesGood() ) {
					setEventRegRowStatus( "2-InProg" );
					return inScore + ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );
				}

				decimal curPassScore = inScore;
				if ( curPassProtScore > 0 ) curPassScore = curPassProtScore;

				setEventRegRowStatus( "2-InProg" );
				return curPassScore + findScoreLastGoodPass( myRecapViewRow, myClassRowSkier, inPassNumMinSpeed, curMaxSpeedKphDiv, isZbsAllowed, isDivisionIntl );

			} else {
				setEventRegRowStatus( "2-InProg" );
				return findScoreLastGoodPass( myRecapViewRow, myClassRowSkier, inPassNumMinSpeed, curMaxSpeedKphDiv, isZbsAllowed, isDivisionIntl );
			}
		}

		private decimal findScoreLastGoodPass( DataGridViewRow inRecapRow, DataRow inClassRow, int inPassNumMinSpeed, Int16 inMaxSpeedKphDiv, bool isZbsAllowed, bool isDivisionIntl ) {
			DataGridViewRow prevRecapRow;
			decimal prevPassProtScore = 0;

			for ( int prevRowIdx = inRecapRow.Index - 1; prevRowIdx >= 0; prevRowIdx-- ) {
				prevRecapRow = myRecapViewRows[prevRowIdx];
				bool prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "N" ) );
				bool prevScoreProtCkd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreProtRecap", "Y" ) );
				if ( prevRerideInd && prevScoreProtCkd ) {
					prevPassProtScore = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "ProtectedScoreRecap", "0" );
					if ( prevPassProtScore == 0 ) prevPassProtScore = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "ScoreRecap", "0" );
				}
				if ( prevRerideInd ) continue;

				decimal prevPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreRecap", "0" ) );
				Int16 curPassSpeedKph = Convert.ToInt16( (String)prevRecapRow.Cells["PassSpeedKphRecap"].Value );
				decimal curPassLineLengthMeters = Convert.ToDecimal( (String)prevRecapRow.Cells["PassLineLengthRecap"].Value );
				DataRow curPassRow = null;
				if ( curPassSpeedKph > inMaxSpeedKphDiv
					&& (Decimal)inClassRow["ListCodeNum"] > (Decimal)SlalomEventData.myClassCRow["ListCodeNum"]
					) {
					curPassRow = SlalomEventData.getPassRow( inMaxSpeedKphDiv, curPassLineLengthMeters );
				} else {
					curPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
				}
				int curPassSpeedNum = (int)curPassRow["SlalomSpeedNum"];
				int curPassLineNum = (int)curPassRow["SlalomLineNum"];
				bool isSpeedBelowMax = ( curPassSpeedKph < inMaxSpeedKphDiv );
				if ( !( isZbsAllowed ) && isSpeedBelowMax ) curPassLineNum = 0;
				if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;
				if ( prevPassScore == 6 ) return ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );
			}

			return prevPassProtScore;
		}

		private decimal findScoreLastNonReride( DataGridViewRow inRecapRow, DataRow inClassRow, int inPassNumMinSpeed, Int16 inMaxSpeedKphDiv, bool isZbsAllowed, bool isDivisionIntl ) {
			DataGridViewRow prevRecapRow;

			for ( int prevRowIdx = inRecapRow.Index - 1; prevRowIdx >= 0; prevRowIdx-- ) {
				prevRecapRow = myRecapViewRows[prevRowIdx];
				bool prevRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "RerideRecap", "N" ) );
				if ( prevRerideInd ) continue;

				setEventRegRowStatus( "2-InProg" );
				decimal prevPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( prevRecapRow, "ScoreRecap", "0" ) );
				if ( prevPassScore == 6 ) {
					Int16 curPassSpeedKph = Convert.ToInt16( (String)prevRecapRow.Cells["PassSpeedKphRecap"].Value );
					decimal curPassLineLengthMeters = Convert.ToDecimal( (String)prevRecapRow.Cells["PassLineLengthRecap"].Value );
					DataRow curPassRow = null;
					if ( curPassSpeedKph > inMaxSpeedKphDiv
						&& (Decimal)inClassRow["ListCodeNum"] > (Decimal)SlalomEventData.myClassCRow["ListCodeNum"]
						) {
						curPassRow = SlalomEventData.getPassRow( inMaxSpeedKphDiv, curPassLineLengthMeters );
					} else {
						curPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
					}
					int curPassSpeedNum = (int)curPassRow["SlalomSpeedNum"];
					int curPassLineNum = (int)curPassRow["SlalomLineNum"];
					bool isSpeedBelowMax = ( curPassSpeedKph < inMaxSpeedKphDiv );
					if ( !( isZbsAllowed ) && isSpeedBelowMax ) curPassLineNum = 0;
					if ( !( isZbsAllowed ) && isDivisionIntl && curPassLineNum > 0 ) curPassLineNum--;
					return ( 6 * ( curPassSpeedNum + curPassLineNum + inPassNumMinSpeed ) );

				} else {
					return findScoreLastGoodPass( myRecapViewRow, inClassRow, inPassNumMinSpeed, inMaxSpeedKphDiv, isZbsAllowed, isDivisionIntl );
				}
			}

			return 0;
		}

		/*
		 * Reride from previous pass because time or path not good and score protected
		 */
		private decimal getProtectedScore( DataGridViewRow prevRecapRow, Decimal inScore ) {
			//decimal curPassScore = inScore;
			//decimal prevPassScoreFull = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "ScoreRecap", "0" );
            decimal prevScoreForPass = calcScoreForPass( prevRecapRow );
            decimal prevPassProtScore = HelperFunctions.getViewRowColValueDecimal( prevRecapRow, "ProtectedScoreRecap", "0" );
            bool prevBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( prevRecapRow, "BoatPathGoodRecap", "Y" ) );

            if ( prevScoreForPass > inScore ) {
				if ( prevPassProtScore > 0 && prevPassProtScore > inScore ) return prevPassProtScore;
				return inScore;
			}
			
			if ( prevBoatPathGood ) return inScore; // Previous boat path was good then can improve score

			return prevScoreForPass; // If previous boat path not good then can't improve score so previous pass score is used.
		}

		private int countPathDevRerides() {
			int returnRerideCount = 0;
			Int16 curPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "PassSpeedKphRecap", "0" ) );
			decimal curPassLineLengthMeters = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "PassLineLengthRecap", "0" );
			bool curRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "RerideRecap", "Y" ) );
			bool curTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "TimeInTolRecap", "Y" ) );
			bool curBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "BoatPathGoodRecap", "Y" ) );

			DataGridViewRow tempRecapRow;
			bool tempRerideInd, tempBoatPathGood, tempTimeGood;
			Int16 tempPassSpeedKph;
			decimal tempPassLineLengthMeters;
			for ( int prevRowIdx = myRecapViewRow.Index - 1; prevRowIdx >= 0; prevRowIdx-- ) {
				tempRecapRow = myRecapViewRows[prevRowIdx];
				
				tempPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( tempRecapRow, "PassSpeedKphRecap", "0" ) );
				tempPassLineLengthMeters = HelperFunctions.getViewRowColValueDecimal( tempRecapRow, "PassLineLengthRecap", "0" );
				if ( curPassSpeedKph != tempPassSpeedKph || curPassLineLengthMeters != tempPassLineLengthMeters ) return returnRerideCount;

				tempRerideInd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "RerideRecap", "Y" ) );
				tempTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "TimeInTolRecap", "Y" ) );
				tempBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "BoatPathGoodRecap", "Y" ) );

				if ( curRerideInd && tempRerideInd
					&& curTimeGood && tempTimeGood
					&& !curBoatPathGood && !tempBoatPathGood
					) {
					returnRerideCount++;
				
				} else {
					break;
				}
			}

			return returnRerideCount;
		}

		public bool isEntryGatesGood() {
			int curGateEntryValue = 0;

			if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "GateEntry1Recap", "false" ) ) ) curGateEntryValue++;
			if ( myNumJudges > 1 ) {
				if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "GateEntry2Recap", "false" ) ) ) curGateEntryValue++;
				if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "GateEntry3Recap", "false" ) ) ) curGateEntryValue++;
			}

			if ( myNumJudges == 1 ) {
				if ( curGateEntryValue == 1 ) return true;
				return false;

			} else {
				if ( curGateEntryValue > 1 ) return true;
				return false;
			}
		}

		public bool isExitGatesGood( DataRow inRow ) {
			int curGateExitValue = 0;
			if ( HelperFunctions.isValueTrue( HelperFunctions.getDataRowColValue( inRow, "GateExit1Recap", "false" ) ) ) {
				decimal curJudgeScore = HelperFunctions.getDataRowColValueDecimal( inRow, "Judge1ScoreRecap", -1 );
				if ( curJudgeScore == 6 ) curGateExitValue++;
			}
			if ( myNumJudges > 1 ) {
				if ( HelperFunctions.isValueTrue( HelperFunctions.getDataRowColValue( inRow, "GateExit2Recap", "false" ) ) ) {
					decimal curJudgeScore = HelperFunctions.getDataRowColValueDecimal( inRow, "Judge2ScoreRecap", -1 );
					if ( curJudgeScore == 6 ) curGateExitValue++;
				}
				if ( HelperFunctions.isValueTrue( HelperFunctions.getDataRowColValue( inRow, "GateExit3Recap", "false" ) ) ) {
					decimal curJudgeScore = HelperFunctions.getDataRowColValueDecimal( inRow, "Judge3ScoreRecap", -1 );
					if ( curJudgeScore == 6 ) curGateExitValue++;
				}
			}

			if ( myNumJudges == 1 ) {
				if ( curGateExitValue == 1 ) return true;
				return false;

			} else {
				if ( curGateExitValue > 1 ) return true;
				return false;
			}
		}
		
		public bool isExitGatesGood() {
			int curGateExitValue = 0;
			if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "GateExit1Recap", "false" ) ) ) {
				decimal curJudgeScore = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "Judge1ScoreRecap", "-1" );
				if ( curJudgeScore == 6 ) curGateExitValue++;
			}
            if (myNumJudges > 3) {
                if (HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "GateExit2Recap", "false" ) )) {
                    decimal curJudgeScore = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "Judge4ScoreRecap", "-1" );
                    if (curJudgeScore == 6) curGateExitValue++;
                }
                if (HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "GateExit3Recap", "false" ) )) {
                    decimal curJudgeScore = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "Judge5ScoreRecap", "-1" );
                    if (curJudgeScore == 6) curGateExitValue++;
                }
            } else if ( myNumJudges > 1 ) {
				if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "GateExit2Recap", "false" ) ) ) {
					decimal curJudgeScore = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "Judge2ScoreRecap", "-1" );
					if ( curJudgeScore == 6 ) curGateExitValue++;
				}
				if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapViewRow, "GateExit3Recap", "false" ) ) ) {
					decimal curJudgeScore = HelperFunctions.getViewRowColValueDecimal( myRecapViewRow, "Judge3ScoreRecap", "-1" );
					if ( curJudgeScore == 6 ) curGateExitValue++;
				}
			}

			if ( myNumJudges == 1 ) {
				if ( curGateExitValue == 1 ) return true;
				return false;

			} else {
				if ( curGateExitValue > 1 ) return true;
				return false;
			}
		}

		private void setEventRegRowStatus( String inStatus) {
			if ( inStatus.Equals( "4-Done" ) || inStatus.Equals( "Done" ) || inStatus.Equals( "Complete" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
				myTourRegViewRow.Cells["SkierName"].Style.Font = curFont;
				myTourRegViewRow.Cells["SkierName"].Style.ForeColor = Color.DarkBlue;
				myTourRegViewRow.Cells["SkierName"].Style.BackColor = SystemColors.Window;
				myTourRegViewRow.Cells["Status"].Value = "4-Done";
				return;
			}
			if ( inStatus.Equals( "2-InProg" ) || inStatus.Equals( "InProg" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
				myTourRegViewRow.Cells["SkierName"].Style.Font = curFont;
				myTourRegViewRow.Cells["SkierName"].Style.ForeColor = Color.White;
				myTourRegViewRow.Cells["SkierName"].Style.BackColor = Color.LimeGreen;
				myTourRegViewRow.Cells["Status"].Value = "2-InProg";
				return;
			}
			if ( inStatus.Equals( "3-Error" ) || inStatus.Equals( "Error" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
				myTourRegViewRow.Cells["SkierName"].Style.Font = curFont;
				myTourRegViewRow.Cells["SkierName"].Style.ForeColor = Color.White;
				myTourRegViewRow.Cells["SkierName"].Style.BackColor = Color.Red;
				myTourRegViewRow.Cells["Status"].Value = "3-Error";
				return;
			}
			if ( inStatus.Equals( "1-TBD" ) || inStatus.Equals( "TBD" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Regular );
				myTourRegViewRow.Cells["SkierName"].Style.Font = curFont;
				myTourRegViewRow.Cells["SkierName"].Style.ForeColor = SystemColors.ControlText;
				myTourRegViewRow.Cells["SkierName"].Style.BackColor = SystemColors.Window;
				myTourRegViewRow.Cells["Status"].Value = "1-TBD";
				return;
			}
		}

		public DataRow getBoatTimeRow( Int16 inSpeed, Int16 inScore ) {
			String curTimeKey = String.Format( "{0}-{1}-{2}", inSpeed, (String)myClassRowSkier["ListCode"], inScore );
			DataRow[] curTimeRowsFound = SlalomEventData.myTimesDataTable.Select( "ListCode = '" + curTimeKey + "'" );
			if ( curTimeRowsFound.Length > 0 )  return curTimeRowsFound[0];
            curTimeKey = String.Format( "{0}-{1}-{2}", inSpeed, "C", inScore );
            curTimeRowsFound = SlalomEventData.myTimesDataTable.Select( "ListCode = '" + curTimeKey + "'" );
            if (curTimeRowsFound.Length > 0) return curTimeRowsFound[0];
            return null;
		}

		public String getBoatTimeMsg() {
			return String.Format( "Times for {0} Actual={1} F={2} S={3}"
				, HelperFunctions.getDataRowColValue( myPassTimeRow, "CodeDesc", "" )
				, HelperFunctions.getDataRowColValueDecimal( myPassTimeRow, "CodeValue", "0", 2 )
				, HelperFunctions.getDataRowColValueDecimal( myPassTimeRow, "MinValue", "0", 2 )
				, HelperFunctions.getDataRowColValueDecimal( myPassTimeRow, "MaxValue", "0", 2 )
				);
		}

	}
}
