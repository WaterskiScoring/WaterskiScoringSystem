using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Jump {
    class JumpSetAnalysis {
        private DataGridViewRow myTourRegViewRow;
        private DataGridViewRow myRecapViewRow;
        private DataGridViewRowCollection myRecapViewRows;
        private DataRow myClassRowSkier;

        private Int16 myMaxSpeedDiv;
        private Int16 myBoatSpeedSelected;
        private Int16 mySkiYearAge;

        private String mySkierBoatPathSelected;
        private String myRampHeightSelected;
        
        private String myTimeTolMsg;
        private String myTriangleTolMsg;

        private DataRow[] myJump3TimesDiv;

        public JumpSetAnalysis( DataGridViewRow inTourRegViewRow, DataGridViewRowCollection inRecapViewRows, DataGridViewRow inRecapViewRow ) {
            myTourRegViewRow = inTourRegViewRow;
            myRecapViewRow = inRecapViewRow;
            myRecapViewRows = inRecapViewRows;
            myTimeTolMsg = "";
            myTriangleTolMsg = "";

            String curAgeGroup = HelperFunctions.getViewRowColValue( myTourRegViewRow, "AgeGroup", "" );

            myMaxSpeedDiv = JumpEventData.getMaxDivSpeed( curAgeGroup );
            mySkiYearAge = 0;
            Int16.TryParse( HelperFunctions.getViewRowColValue( myTourRegViewRow, "SkiYearAge", "0" ), out mySkiYearAge );

            // For class L,R tournaments get IWWF equivalent divisions
            myJump3TimesDiv = new DataRow[0];

            myClassRowSkier = JumpEventData.getSkierClass( HelperFunctions.getViewRowColValue( myTourRegViewRow, "EventClass", JumpEventData.myTourClass ) );
            if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                myJump3TimesDiv = JumpEventData.getJump3TimesDiv( curAgeGroup, mySkiYearAge );
            }
        }

        public String TimeTolMsg { get => myTimeTolMsg; set => myTimeTolMsg = value; }
        public String TriangleTolMsg { get => myTriangleTolMsg; set => myTriangleTolMsg = value; }
        public DataGridViewRow TourRegViewRow { get => myTourRegViewRow; set => myTourRegViewRow = value; }
        public DataGridViewRow RecapViewRow { get => myRecapViewRow; set => myRecapViewRow = value; }
        public DataGridViewRowCollection RecapViewRows { get => myRecapViewRows; set => myRecapViewRows = value; }
        public short MaxSpeedDiv { get => myMaxSpeedDiv; set => myMaxSpeedDiv = value; }
        public short BoatSpeedSelected { get => myBoatSpeedSelected; set => myBoatSpeedSelected = value; }
        public string SkierBoatPathSelected { get => mySkierBoatPathSelected; set => mySkierBoatPathSelected = value; }
        public string RampHeightSelected { get => myRampHeightSelected; set => myRampHeightSelected = value; }

        public JumpPassResultStatus AnalyzeRecapPasses() {
            JumpPassResultStatus curReturnResult = new JumpPassResultStatus();

            if ( myRecapViewRows.Count == 0 ) return curReturnResult;

            String curAgeGroup = HelperFunctions.getViewRowColValue( myRecapViewRow, "AgeGroupRecap", "" );
            if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) curReturnResult.isIwwfSkier = true;

            foreach ( DataGridViewRow curRecapRow in myRecapViewRows ) {
                // Get score and reride attributes
                curReturnResult.setPassDefultResult();
                curReturnResult.passResult = HelperFunctions.getViewRowColValue( curRecapRow, "ResultsRecap", "" );
                curReturnResult.passScoreFeet = HelperFunctions.getViewRowColValueDecimal( curRecapRow, "ScoreFeetRecap", "0" );
                curReturnResult.passScoreMeters = HelperFunctions.getViewRowColValueDecimal( curRecapRow, "ScoreMetersRecap", "0" );

                curReturnResult.passTimeInTol = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( curRecapRow, "TimeInTolRecap", "N" ) );
                curReturnResult.passReride = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( curRecapRow, "RerideRecap", "N" ) );
                curReturnResult.passScoreProt = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( curRecapRow, "ScoreProtRecap", "N" ) );
                curReturnResult.passRerideIfBest = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( curRecapRow, "RerideIfBestRecap", "N" ) );
                curReturnResult.passRerideCanImprove = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( curRecapRow, "RerideCanImproveRecap", "N" ) );

                if ( curReturnResult.passTimeInTol ) {
                    if ( curReturnResult.passReride ) {
                        curReturnResult.passOptionalReride();

                    } else {
                        if ( curReturnResult.passResult.Equals( "Jump" ) && ( curReturnResult.passScoreFeet > 0 ) && ( curReturnResult.passScoreMeters > 0 ) ) {
                            curReturnResult.passGoodJump();
                        } else {
                            curReturnResult.passGoodNoJump();
                        }
                    }

                } else {
                    curReturnResult.passOutTolReride();
                }
            }

            curReturnResult.checkSetStatus();

            return curReturnResult;
        }

        /*
		 * Method used when a time entry attribute for a pass has been modified
		 * Check all 3 time attributes and validate the times if all have been entered
		*/
        public bool checkNeedTimeValidate() {
            myRecapViewRow.Cells["Updated"].Value = "Y";

            // BoatSplitTime (52M segment)
            if ( HelperFunctions.isObjectEmpty( myRecapViewRow.Cells["BoatSplitTimeRecap"].Value ) ) return false;

            // BoatSplitTime2 (82M segment)
            if ( HelperFunctions.isObjectEmpty( myRecapViewRow.Cells["BoatSplitTime2Recap"].Value ) ) return false;

            if ( myRecapViewRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" ) ) {
                // Validate times
                TimeValidate();

            } else if ( myRecapViewRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" ) ) {
                // BoatEndTime (41M segment)
                if ( HelperFunctions.isObjectEmpty( myRecapViewRow.Cells["BoatEndTimeRecap"].Value ) ) return false;

                // Validate times
                TimeValidate();

            } else {
                if ( HelperFunctions.isObjectEmpty( myRecapViewRow.Cells["BoatEndTimeRecap"].Value ) ) return false;

                // Validate times
                TimeValidate();

                // If feet and meters have been entered then check to determine if score for round should be update
                if ( HelperFunctions.isObjectEmpty( myRecapViewRow.Cells["ScoreFeetRecap"].Value ) ) return false;
                if ( HelperFunctions.isObjectEmpty( myRecapViewRow.Cells["ScoreMetersRecap"].Value ) ) return false;
            }

            return true;
        }

        public void TimeValidate() {
            myTimeTolMsg = "";
            Int16 curBoatSpeed = Convert.ToInt16( (String)myRecapViewRow.Cells["BoatSpeedRecap"].Value );
            String curResults = (String)myRecapViewRow.Cells["ResultsRecap"].Value;
            String curRtb = (String)myRecapViewRow.Cells["ReturnToBaseRecap"].Value;
            String curDiv = (String)myRecapViewRow.Cells["AgeGroupRecap"].Value;

            // Get score and tolerance data needed for making time validation determinations.
            Decimal curScoreMeters = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapViewRow, "ScoreMetersRecap", "0" ) );
            Decimal curBoatEndTime = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapViewRow, "BoatEndTimeRecap", "0" ) );
            Decimal curBoatSplit52Time = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapViewRow, "BoatSplitTimeRecap", "0" ) );
            Decimal curBoatSplit82Time = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapViewRow, "BoatSplitTime2Recap", "0" ) );
            myClassRowSkier = JumpEventData.getSkierClass( HelperFunctions.getViewRowColValue( myTourRegViewRow, "EventClass", JumpEventData.myTourClass ) );
            String curEventClass = HelperFunctions.getDataRowColValue( myClassRowSkier, "ListCode", JumpEventData.myTourClass );

            //-----------------------------------
            // Validate 82M segment time
            //-----------------------------------
            Int16[] curTolTimes = validate82MSegmentTime( curBoatSplit82Time, curEventClass, curBoatSpeed, curResults );
            Int16 curTolSplit82Time = curTolTimes[0];
            Int16 curTolSplit82TimeMax = curTolTimes[1];

            //-----------------------------------
            // Validate first segment time
            //-----------------------------------
            curTolTimes = validate52MSegmentTime( curBoatSplit52Time, curEventClass, curBoatSpeed, curScoreMeters );
            Int16 curTolSplit52Time = curTolTimes[0];
            Int16 curTolSplit52TimeMax = curTolTimes[1];

            //-----------------------------------
            // Validate 41M segment (end) time
            //-----------------------------------
            curTolTimes = validate41MSegmentTime( curBoatEndTime, curEventClass, curBoatSpeed, curResults, curRtb );
            Int16 curTolEndTime = curTolTimes[0];
            Int16 curTolEndTimeMax = curTolTimes[1];

            //-----------------------------------
            // Determine results of pass based on boat times compared to tolerances
            //-----------------------------------
            if ( curTolSplit82Time == 0 ) {
                if ( curTolEndTime == 0 ) {
                    // 82M good, 41M good
                    validateGoodMtGoodEt( curResults, curBoatSpeed, curScoreMeters, curTolSplit52Time, curTolSplit52TimeMax );

                } else if ( curTolEndTime > 0 ) {
                    validateGoodMtFastEt( curResults, curBoatSpeed, curScoreMeters, curTolEndTime, curTolEndTimeMax, curTolSplit52Time, curTolSplit52TimeMax );

                } else if ( curTolEndTime < 0 ) {
                    validateGoodMtSlowEt( curResults, curBoatSpeed, curScoreMeters, curTolEndTime, curTolEndTimeMax, curTolSplit52Time, curTolSplit52TimeMax );
                }

            } else if ( curTolSplit82Time > 0 ) {
                if ( curTolEndTime == 0 ) {
                    // 82M fast, 41M good
                    validateFastMtGoodEt( curResults, curBoatSpeed, curScoreMeters, curTolSplit82Time, curTolSplit82TimeMax, curTolSplit52Time, curTolSplit52TimeMax );

                } else if ( curTolEndTime > 0 ) {
                    // 82M fast, 41M fast
                    validateFastMtFastEt( curResults, curBoatSpeed, curScoreMeters, curTolEndTime, curTolEndTimeMax, curTolSplit82Time, curTolSplit82TimeMax, curTolSplit52Time, curTolSplit52TimeMax );

                } else if ( curTolEndTime < 0 ) {
                    // 82M fast, 41M slow
                    validateFastMtSlowEt( curResults, curBoatSpeed, curScoreMeters, curTolEndTime, curTolEndTimeMax, curTolSplit82Time, curTolSplit82TimeMax, curTolSplit52Time, curTolSplit52TimeMax );
                }

            } else if ( curTolSplit82Time < 0 ) {
                if ( curTolEndTime == 0 ) {
                    // 82M slow, 41M good (processing confirmed 7/2/14)
                    validateSlowMtGoodEt( curResults, curBoatSpeed, curScoreMeters, curTolEndTime, curTolEndTimeMax, curTolSplit82Time, curTolSplit82TimeMax, curTolSplit52Time, curTolSplit52TimeMax );
                
                } else if ( curTolEndTime > 0 ) {
                    // 82M slow, 41M fast (processing confirmed 7/2/14)
                    validateSlowMtFastEt( curResults, curBoatSpeed, curScoreMeters, curTolEndTime, curTolEndTimeMax, curTolSplit82Time, curTolSplit82TimeMax, curTolSplit52Time, curTolSplit52TimeMax );

                } else if ( curTolEndTime < 0 ) {
                    // 82M slow, 41M slow (processing confirmed 7/2/14)
                    validateSlowMtSlowEt( curResults, curBoatSpeed, curScoreMeters, curTolEndTime, curTolEndTimeMax, curTolSplit82Time, curTolSplit82TimeMax, curTolSplit52Time, curTolSplit52TimeMax );
                }
            }

            String curskierPassMsg = myTimeTolMsg;
            if ( HelperFunctions.isObjectPopulated( myTriangleTolMsg) ) curskierPassMsg += " : " + myTriangleTolMsg;
            myRecapViewRow.Cells["RerideReasonRecap"].Value = curskierPassMsg;
            if ( myTriangleTolMsg.Length > 1 ) {
                if ( myRecapViewRow.Cells["RerideRecap"].Value.Equals( "N" ) ) {
                    myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                }
            }
        }

        /*
         * Segments 82M good, 41M good (processing confirmed October 2022)
         */
        private void validateGoodMtGoodEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax ) {
            myRecapViewRow.Cells["TimeInTolRecap"].Value = "Y";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapViewRow.Cells["RerideRecap"].Value = "N";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "N";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;

            if ( curResults.ToLower().Equals( "fall" ) || curResults.ToLower().Equals( "pass" ) ) return;

            if ( curBoatSpeed >= myMaxSpeedDiv ) {
                // Skier speed matches division max speed

                // 52M NT segment only required for specific divisions and jump exceeds specified distance for the division
                if ( myJump3TimesDiv.Length == 0 || curScoreMeters < (Decimal)myJump3TimesDiv[0]["MaxValue"] ) return;

                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) return; // Segment time good

                if ( curTolSplit52Time > 0 ) {
                    myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                    myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                    myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                    myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                    myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                    myTimeTolMsg = "Fast 52M segment, optional reride with protected score, improvement allowed (See IWWF 10.06)";
                }

                if ( curTolSplit52Time < 0 ) {
                    myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                    myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                    myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                    myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "N";
                    myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                    myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                    myTimeTolMsg = "Slow 52M segment, mandatory reride if best, score not protected, no improvement allowed (See IWWF 10.06)";
                }

                return;
            }

            /*
             * Skier speed less than allowed division max speed
             */

            /*
             * 52M NT segment only required for L/R skiers in specific divisions when jump exceeds specified distance for the division
             */
            if ( myJump3TimesDiv.Length == 0 || curScoreMeters < (Decimal)myJump3TimesDiv[0]["MaxValue"] ) return;

            /*
             * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
             */
            if ( curTolSplit52Time == 0 ) return; // 52M Segment time good

            myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapViewRow.Cells["RerideRecap"].Value = "Y";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

            if ( curTolSplit52Time > 0 ) {
                if ( curTolSplit52TimeMax > 0 ) {
                    myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                    myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                    myTimeTolMsg = "Fast 52M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                } else {
                    myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                    myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                    myTimeTolMsg = "Fast 52M segment, optional reride, protected score, improvement allowed (See IWWF 10.06)";
                }
            }

            if ( curTolSplit52Time < 0 ) {
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                myTimeTolMsg = "Slow 52M segment, optional reride, score protected (See IWWF 10.06)";
            }

            return;
        }

        /*
         * 82M good, 41M fast (processing confirmed October 2022)
         */
        private void validateGoodMtFastEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters
            , Int16 curTolEndTime, Int16 curTolEndTimeMax, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax 
            ) {
            if ( curResults.ToLower().Equals( "fall" ) ) {
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Fast 41M end time, immediate optional reride for fall (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Fast 41M end time, immediate optional reride for fall (See 9.10.B.2)";
                }

                return;
            }

            /*
             * Successful jump
             */
            if ( curBoatSpeed >= myMaxSpeedDiv ) {
                // Skier speed matches division max speed allowed
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Fast 41M end time, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                        myTimeTolMsg = "Fast 52M segment, Fast 41M end time, optional reride, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                        myTimeTolMsg = "Slow 52M segment, Fast 41M end time, mandatory reride if best, score not protected, no improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        //For class L,R tournaments use IWWF rules
                        myTimeTolMsg = "Fast 41M end time, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else {
                        //Otherwise use AWSA rules
                        myTimeTolMsg = "Fast 41M end time, mandatory reride if best, score not protected, improvement allowed (See 9.10.B.5)";
                    }
                }

                return;
            }

            /*
             * Skier speed less than allowed division max speed
             */
            if ( curTolEndTimeMax > 0 ) {
                //82M good, 41M fast for max speed
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                /*
                 * 52M NT segment only required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( myJump3TimesDiv.Length == 0 || curScoreMeters < (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    //82M good, 41M fast for max speed
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Fast 41M end time for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 41M end time for max speed, mandatory reride if best, score not protected, improvement allowed (See 9.10.B.4)";
                    }
                    return;
                }

                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) {
                    myTimeTolMsg = "Fast 41M end time for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    return;
                }

                if ( curTolSplit52Time > 0 ) {
                    if ( curTolSplit52TimeMax > 0 ) {
                        myTimeTolMsg = "Fast 52M segment for max speed, Fast 41M end time for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else {
                        myTimeTolMsg = "Fast 52M segment but good for max speed, Fast 41M end time for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    }

                    return;
                }

                if ( curTolSplit52Time < 0 ) {
                    myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                    myTimeTolMsg = "Slow 52M segment, Fast 41M end time for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                    return;
                }

                return;
            }

            /*
             * 82M good, 41M fast but good for max speed
             */
            myRecapViewRow.Cells["TimeInTolRecap"].Value = "Y";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapViewRow.Cells["RerideRecap"].Value = "N";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "N";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;

            /*
             * 52M NT segment only required for L/R skiers in specific divisions when jump exceeds specified distance for the division
             */
            if ( myJump3TimesDiv.Length == 0 || curScoreMeters < (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                myTimeTolMsg = "Fast 41M end time but good for max speed, jump good";
                return;
            }

            /*
             * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
             */
            if ( curTolSplit52Time == 0 ) {
                myTimeTolMsg = "Fast 41M end time but good for max speed, jump good";
                return;
            }

            myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

            if ( curTolSplit52Time > 0 ) {
                if ( curTolSplit52TimeMax > 0 ) {
                    myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                    myTimeTolMsg = "Fast 52M segment for max speed, Fast 41M end time but good for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                } else {
                    myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                    myTimeTolMsg = "Fast 52M segment for max speed, Fast 41M end time but good for max speed, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                }

                return;
            }

            if ( curTolSplit52Time < 0 ) {
                myTimeTolMsg = "Slow 52M segment, Fast 41M end time but good for max speed, mandatory reride if best, otherwise optional reride, score protected, improvement allowed (See IWWF 10.06)";
                return;
            }

        }

        /*
         * 82M good, 41M slow (processing confirmed October 2022)
         */
        private void validateGoodMtSlowEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters
            , Int16 curTolEndTime, Int16 curTolEndTimeMax, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax 
            ) {
            if ( curResults.ToLower().Equals( "fall" ) ) {
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Slow 41M end time, immediate optional reride for fall (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Slow 41M end time, immediate optional reride for fall (See 9.10.B.2)";
                }

                return;
            }

            /*
             * Successful jump
             */
            if ( curBoatSpeed >= myMaxSpeedDiv ) {
                // Skier speed matches division max speed allowed
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        myTimeTolMsg = "Fast 52M segment, Slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myTimeTolMsg = "Slow 52M segment, Slow 41M end time, optional reride, score not protected, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Slow 41M end time, optional reride, score protected, use RTB slow column if applicable, improvement allowed (See IWWF 10.06)";

                    } else {
                        myTimeTolMsg = "Slow 41M end time, optional reride, score protected (See 9.10.B.5), use RTB slow column if applicable, improvement allowed (See 9.17)";
                    }
                }

                return;
            }

            /*
             * Skier speed less than allowed division max speed
             */
            myRecapViewRow.Cells["TimeInTolRecap"].Value = "Y";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapViewRow.Cells["RerideRecap"].Value = "N";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;

            if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) {
                    myTimeTolMsg = "Slow 41M end time but no reride because speed less than max and 82M segment is good (See IWWF 10.06)";

                } else if ( curTolSplit52Time > 0 ) {
                    myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                    myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                    myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                    if ( curTolSplit52TimeMax > 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                        myTimeTolMsg = "Fast 52M segment for max speed, Slow 41M end time, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";

                    } else {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                        myRecapViewRow.Cells["RerideRecap"].Value = "N";
                        myTimeTolMsg = "Fast 52M segment but not for max speed, Slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                    }

                } else if ( curTolSplit52Time < 0 ) {
                    myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                    myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                    myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                    myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                    myTimeTolMsg = "Slow 52M segment, Slow 41M end time, optional reride, score protected. (See IWWF 10.06)";
                }

            } else {
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Slow 41M end time but no reride because speed less than max and 82M segment is good (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Slow 41M end time but no reride because speed less than max and 82M segment is good (See  9.10.B.4)";
                }
            }
        }

        /*
         * 82M fast, 41M good (processing confirmed October 2022)
         */
        private void validateFastMtGoodEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters
            , Int16 curTolSplit82Time, Int16 curTolSplit82TimeMax, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax 
            ) {
            if ( curResults.ToLower().Equals( "fall" ) || curResults.ToLower().Equals( "pass" ) ) {
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Fast 82M segment, immediate optional reride for fall or pass (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Fast 82M segment, immediate optional reride for fall or pass (See 9.10.B.2)";
                }

                return;
            }

            /*
             * Successful jump
             */
            if ( curBoatSpeed >= myMaxSpeedDiv ) {
                /*
                 * Skier speed matches division max speed allowed
                 */
                #region Skier speed matches division max speed allowed
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Fast 82M segment, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        myTimeTolMsg = "Fast 52M segment, Fast 82M segment, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "N";
                        myTimeTolMsg = "Slow 52M segment, fast 82M segment, mandatory reride if best, score not protected, no improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Fast 82M segment, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 82M segment, mandatory reride if best, score not protected, improvement allowed (See 9.10.B.5)";
                    }
                }

                return;
                #endregion
            }

            /*
             * Skier speed less than allowed division max speed
             */
            if ( curTolSplit82TimeMax > 0 ) {
                // 82M fast for max speed, 41M good
                #region Skier speed less than allowed division max speed, 82M fast for max speed, 41M good
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Fast 82M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        if ( curTolSplit52TimeMax > 0 ) {
                            myTimeTolMsg = "Fast 52M segment for max speed, Fast 82M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                        } else {
                            myTimeTolMsg = "Fast 52M segment but good for max speed, Fast 82M segment for max speed, mandatory reride if best jump, score not protected, improvement allowed (See IWWF 10.06)";
                        }

                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                        myTimeTolMsg = "Slow 52M segment, fast 82M segment for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Fast 82M segment for max speed, mandatory reride if best jump, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 82M segment for max speed, mandatory reride if best jump, score not protected, improvement allowed (See 9.10.B.4)";
                    }
                }

                return;
                #endregion
            }

            // 82M fast but good for max speed, 41M good
            #region Skier speed less than allowed division max speed, 82M fast but good for max speed, 41M good
            myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

            if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) {
                    myTimeTolMsg = "Fast 82M segment but good for max speed, optional reride, score protected (See IWWF 10.06)";

                } else if ( curTolSplit52Time > 0 ) {
                    if ( curTolSplit52TimeMax > 0 ) {
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myTimeTolMsg = "Fast 52M segment for max speed, Fast 82M segment but good for max speed, mandatory reride if best jump, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 52M segment but good for max speed, Fast 82M segment but good for max speed, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                    }

                } else if ( curTolSplit52Time < 0 ) {
                    myTimeTolMsg = "Slow 52M segment, Fast 82M segment but good for max speed, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                }

            } else {
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Fast 82M segment but good for max speed, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Fast 82M segment but good for max speed, optional reride, score protected, improvement allowed (See 9.10.B.4";
                }
            }
            #endregion
        }

        /*
         * 82M fast, 41M fast (processing confirmed October 2022)
         */
        private void validateFastMtFastEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters
            , Int16 curTolEndTime, Int16 curTolEndTimeMax, Int16 curTolSplit82Time, Int16 curTolSplit82TimeMax, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax 
            ) {
            if ( curResults.ToLower().Equals( "fall" ) || curResults.ToLower().Equals( "pass" ) ) {
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Fast 82M segment, fast 41M segment, immediate optional reride for fall or pass (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Fast 82M segment, fast 41M segment, immediate optional reride for fall or pass (See 9.10.B.2)";
                }

                return;
            }

            /*
             * Successful jump
             */
            if ( curBoatSpeed >= myMaxSpeedDiv ) {
				/*
                 * Skier speed matches division max speed allowed
                 */
				#region Skier speed matches division max speed allowed
				myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                
                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Fast 82M segment, fast 41M end time, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    
                    } else if ( curTolSplit52Time > 0 ) {
                        myTimeTolMsg = "Fast 52M segment, fast 82M segment, Fast 41M end time, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    
                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "N";
                        myTimeTolMsg = "Slow 52M segment, fast 82M segment, Fast 41M end time, mandatory reride if best, score not protected, no improvement allowed (See IWWF 10.06)";
                    }
                
                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Fast 82M segment, fast 41M end time, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 82M segment, fast 41M end time, mandatory reride if best, score not protected, improvement allowed (See 9.10.B.5)";
                    }
                }

                return;
                #endregion
            }

            /*
             * Skier speed less than allowed division max speed
             */
            myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapViewRow.Cells["RerideRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

            if ( curTolSplit82TimeMax > 0 ) {
				#region 82M segment fast for max speed
				if ( curTolEndTimeMax > 0 ) {
                    /*
                     * 82M fast for max speed, 41M fast for max speed
                     */
                    if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                        /*
                         * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                         */
                        if ( curTolSplit52Time == 0 ) {
                            myTimeTolMsg = "Fast 82M segment for max speed, fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                        } else if ( curTolSplit52Time > 0 ) {
                            if ( curTolSplit52TimeMax > 0 ) {
                                myTimeTolMsg = "Fast 52M segment for max speed, fast 82M segment for max speed, Fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                            } else {
                                myTimeTolMsg = "Fast 52M segment but good for max speed, fast 82M segment for max speed, Fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                            }

                        } else if ( curTolSplit52Time < 0 ) {
                            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                            myTimeTolMsg = "Slow 52M segment, fast 82M segment for max speed, Fast 41M segment for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                        }
                    
                    } else {
                        if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                            myTimeTolMsg = "Fast 82M segment for max speed, fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                        } else {
                            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                            myTimeTolMsg = "Fast 82M segment for max speed, fast 41M segment for max speed, mandatory reride if best, otherwise optional reride with protected score, improvement allowed (See 9.10.B.5)";
                        }

                    }

                    return;
                }

                /*
                 * 82M fast for max speed, 41M fast but good for max speed
                 */
                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Fast 82M segment for max speed, fast 41M segment but good for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        if ( curTolSplit52TimeMax > 0 ) {
                            myTimeTolMsg = "Fast 52M segment for max speed, fast 82M segment for max speed, Fast 41M segment but good for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                        } else {
                            myTimeTolMsg = "Fast 52M segment but good for max speed, fast 82M segment for max speed, Fast 41M segment but good for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                        }

                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                        myTimeTolMsg = "Slow 52M segment, fast 82M segment for max speed, Fast 41M segment but good for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Fast 82M segment for max speed, fast 41M segment but good for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                        myTimeTolMsg = "Fast 82M segment for max speed, fast 41M segment but good for max speed, mandatory reride if best, otherwise optional reride with protected score, improvement allowed (See 9.10.B.5)";
                    }

                }

                return;
                #endregion
            }

            if ( curTolEndTimeMax > 0 ) {
				/*
                 * 82M fast but good for max speed, 41M fast for max speed
                 */
				#region 82M fast but good for max speed, 41M fast for max speed
				if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Fast 82M segment but good for max speed, fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        if ( curTolSplit52TimeMax > 0 ) {
                            myTimeTolMsg = "Fast 52M segment for max speed, Fast 82M segment for max speed but good for max speed, fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                        } else {
                            myTimeTolMsg = "Fast 52M segment but good for max speed, fast 82M segment but good for max speed, fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                        }



                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                        myTimeTolMsg = "Slow 52M segment, fast 82M segment but good for max speed, fast 41M segment for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Fast 82M segment but good for max speed, fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 82M segment but good for max speed, fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See 9.10.B.5)";
                    }

                }

                return;
                #endregion
            }

            /*
             * 82M fast but good for max speed, 41M fast but good for max speed
             */
            #region 82M fast but good for max speed, 41M fast but good for max speed
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
            if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) {
                    myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                    myTimeTolMsg = "Fast 82M segment but good for max speed, fast 41M segment but good for max speed, score protected, optional reride, improvement allowed (See IWWF 10.06)";

                } else if ( curTolSplit52Time > 0 ) {
                    if ( curTolSplit52TimeMax > 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myTimeTolMsg = "Fast 52M segment for max speed, fast 82M segment but good for max speed, Fast 41M segment but good for max speed, score not protected, mandatory reride if best score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                        myTimeTolMsg = "Fast 52M segment but good for max speed, Fast 82M segment but good for max speed, fast 41M segment but good for max speed, score protected, optional reride, improvement allowed (See IWWF 10.06)";
                    }

                } else if ( curTolSplit52Time < 0 ) {
                    myTimeTolMsg = "Slow 52M segment, fast 82M segment but good for max speed, Fast 41M segment but good for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                }

            } else {
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Fast 82M segment but good for max speed, fast 41M segment but good for max speed, score protected, optional reride, improvement allowed (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Fast 82M segment but good for max speed, fast 41M segment but good for max speed, score protected, optional reride, improvement allowed (See 9.10.B.4)";
                }

            }
            #endregion

            return;
        }

        /*
         * 82M fast, 41M slow (processing confirmed October 2022)
         */
        private void validateFastMtSlowEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters
            , Int16 curTolEndTime, Int16 curTolEndTimeMax, Int16 curTolSplit82Time, Int16 curTolSplit82TimeMax, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax
            ) {
            if ( curResults.ToLower().Equals( "fall" ) || curResults.ToLower().Equals( "pass" ) ) {
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Fast 82M segment, slow 41M segment, immediate optional reride for fall or pass (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Fast 82M segment, slow 41M segment, immediate optional reride for fall or pass (See 9.10.B.2)";
                }

                return;
            }

            /*
             * Successful jump
             */
            if ( curBoatSpeed >= myMaxSpeedDiv ) {
                /*
                 * Skier speed matches division max speed allowed
                 */
                #region Skier speed matches division max speed allowed
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Fast 82M segment, slow 41M end time, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        myTimeTolMsg = "Fast 52M segment, fast 82M segment, slow 41M end time, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "N";
                        myTimeTolMsg = "Slow 52M segment, fast 82M segment, slow 41M end time, mandatory reride if best, score not protected, no improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Fast 82M segment, slow 41M end time, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 82M segment, slow 41M end time, mandatory reride if best, score not protected, improvement allowed (See 9.10.B.5)";
                    }
                }

                return;
                #endregion
            }

            /*
             * Skier speed less than allowed division max speed
             */
            myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapViewRow.Cells["RerideRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

            if ( curTolSplit82TimeMax > 0 ) {
                #region 82M segment fast for max speed
                /*
                 * 82M fast for max speed, 41M slow
                 */
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Fast 82M segment for max speed, slow 41M segment, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        if ( curTolSplit52TimeMax > 0 ) {
                            myTimeTolMsg = "Fast 52M segment for max speed, Fast 82M segment for max speed, slow 41M segment, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                        } else {
                            myTimeTolMsg = "Fast 52M segment but good for max speed, Fast 82M segment for max speed, slow 41M segment, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                        }

                    } else if ( curTolSplit52Time < 0 ) {
                        myTimeTolMsg = "Slow 52M segment, Fast 82M segment for max speed, slow 41M segment, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Fast 82M segment for max speed, slow 41M segment, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 82M segment for max speed, slow 41M segment, mandatory reride if best, score not protected , improvement allowed (See 9.10.B.4)";
                    }

                }

                return;
                #endregion
            }

            /*
             * 82M fast but good for max speed, 41M slow
             */
            #region 82M fast but good for max speed, 41M slow
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
            if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) {
                    myTimeTolMsg = "Fast 82M segment but good for max speed, slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";

                } else if ( curTolSplit52Time > 0 ) {
                    if ( curTolSplit52TimeMax > 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                        myTimeTolMsg = "Fast 52M segment for max speed, fast 82M segment but good for max speed, slow 41M end time, mandatory reride if best, otherwise optional reride with protected score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 52M segment but good for max speed, fast 82M segment but good for max speed, slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                    }

                } else if ( curTolSplit52Time < 0 ) {
                    myTimeTolMsg = "Slow 52M segment, Fast 82M segment but good for max speed, slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                }

            } else {
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Fast 82M segment but good for max speed, slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Fast 82M segment but good for max speed, slow 41M end time, optional reride, score protected, improvement allowed (See 9.10.B.4)";
                }

            }
            
            #endregion
            return;
        }


        /*
         * 82M slow, 41M good (processing confirmed October 2022)
         */
        private void validateSlowMtGoodEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters
            , Int16 curTolEndTime, Int16 curTolEndTimeMax, Int16 curTolSplit82Time, Int16 curTolSplit82TimeMax, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax
            ) {
            if ( curResults.ToLower().Equals( "fall" ) || curResults.ToLower().Equals( "pass" ) ) {
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Slow 82M segment, immediate optional reride for fall or pass (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Slow 82M segment, immediate optional reride for fall or pass (See 9.10.B.2)";
                }

                return;
            }

            /*
             * Successful jump
             */
            if ( curBoatSpeed >= myMaxSpeedDiv ) {
                /*
                 * Skier speed matches division max speed allowed
                 */
                #region Skier speed matches division max speed allowed
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Slow 82M segment, optional reride, protected score, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        myTimeTolMsg = "Fast 52M segment, slow 82M segment, optional reride, protected score, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myTimeTolMsg = "Slow 52M segment, slow 82M segment, optional reride, score not protected, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Slow 82M segment, optional reride, protected score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Slow 82M segment, optional reride, protected score, improvement allowed (See 9.10.B.5)";
                    }
                }

                return;
                #endregion
            }

            /*
             * Skier speed less than allowed division max speed
             */
            myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

            /*
             * 82M slow, 41M slow
             */
            #region 82M slow, 41M slow
            if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) {
                    myTimeTolMsg = "Slow 82M segment, optional reride, score protected, improvement allowed (See IWWF 10.06)";

                } else if ( curTolSplit52Time > 0 ) {
                    if ( curTolSplit52TimeMax > 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                        myTimeTolMsg = "Fast 52M segment for max speed, slow 82M segment, mandatory reride if best, otherwise optional reride with protected score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 52M segment but good for max speed, slow 82M segment, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                    }

                } else if ( curTolSplit52Time < 0 ) {
                    myTimeTolMsg = "Slow 52M segment, slow 82M segment, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                }

            } else {
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Slow 82M segment, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Slow 82M segment, optional reride, score protected, improvement allowed (See 9.10.B.4)";
                }
            }

            #endregion
            return;
        }

        /*
         * 82M slow, 41M fast (processing confirmed October 2022)
         */
        private void validateSlowMtFastEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters
            , Int16 curTolEndTime, Int16 curTolEndTimeMax, Int16 curTolSplit82Time, Int16 curTolSplit82TimeMax, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax
            ) {
            if ( curResults.ToLower().Equals( "fall" ) || curResults.ToLower().Equals( "pass" ) ) {
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Slow 82M segment, fast 41M segment, immediate optional reride for fall or pass (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Slow 82M segment, fast 41M segment, immediate optional reride for fall or pass (See 9.10.B.2)";
                }

                return;
            }

            /*
             * Successful jump
             */
            if ( curBoatSpeed >= myMaxSpeedDiv ) {
                /*
                 * Skier speed matches division max speed allowed
                 */
                #region Skier speed matches division max speed allowed
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Slow 82M segment, fast 41M end time, optional reride, protected score, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        myTimeTolMsg = "Fast 52M segment, slow 82M segment, fast 41M end time, optional reride, protected score, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time < 0 ) {
                        myTimeTolMsg = "Slow 52M segment, slow 82M segment, fast 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Slow 82M segment, fast 41M end time, optional reride, protected score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Slow 82M segment, fast 41M end time, optional reride, protected score, improvement allowed (See 9.10.B.5), use RTB slow column if applicable (See 9.17).";
                    }
                }

                return;
                #endregion
            }

            /*
             * Skier speed less than allowed division max speed
             */

            myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

            if ( curTolEndTimeMax > 0 ) {
                /*
                 * 82M slow, 41M fast for max speed
                 */
                #region 82M slow, 41M fast but good for max speed
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Slow 82M segment, fast 41M segment for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        if ( curTolSplit52TimeMax > 0 ) {
                            myTimeTolMsg = "Fast 52M segment for max speed, slow 82M segment, fast 41M segment for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                        } else {
                            myTimeTolMsg = "Fast 52M segment but good for max speed, slow 82M segment, fast 41M segment for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                        }

                    } else if ( curTolSplit52Time < 0 ) {
                        myTimeTolMsg = "Slow 52M segment, slow 82M segment, fast 41M segment for max speed, mandatory reride if best, otherwise optional with protected score, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Slow 82M segment, fast 41M segment for max speed, mandatory reride if best, score not protected, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Slow 82M segment, fast 41M segment for max speed, mandatory reride if best, otherwise optional reride with protected score, improvement allowed (See 9.10.B.5)";
                    }
                }

                return;
                #endregion
            }

            /*
             * 82M slow, 41M fast but good for max speed
             */
            #region 82M slow, 41M fast but good for max speed
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
            if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) {
                    myTimeTolMsg = "Slow 82M segment, fast 41M end time but good for max speed, optional reride, score protected, improvement allowed (See IWWF 10.06)";

                } else if ( curTolSplit52Time > 0 ) {
                    if ( curTolSplit52TimeMax > 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                        myTimeTolMsg = "Fast 52M segment for max speed, slow 82M segment, fast 41M end time but good for max speed, mandatory reride if best, otherwise optional reride with protected score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 52M segment but good for max speed, slow 82M segment, fast 41M end time but good for max speed, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                    }

                } else if ( curTolSplit52Time < 0 ) {
                    myTimeTolMsg = "Slow 52M segment, slow 82M segment, fast 41M end time but good for max speed, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                }

            } else {
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Slow 82M segment, fast 41M end time but good for max speed, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Slow 82M segment, fast 41M end time but good for max speed, optional reride, score protected, improvement allowed (See 9.10.B.4)";
                }
            }

            #endregion
            return;
        }

        /*
         * 82M slow, 41M slow (processing confirmed October 2022)
         */
        private void validateSlowMtSlowEt( String curResults, Int16 curBoatSpeed, Decimal curScoreMeters
            , Int16 curTolEndTime, Int16 curTolEndTimeMax, Int16 curTolSplit82Time, Int16 curTolSplit82TimeMax, Int16 curTolSplit52Time, Int16 curTolSplit52TimeMax
            ) {
            if ( curResults.ToLower().Equals( "fall" ) || curResults.ToLower().Equals( "pass" ) ) {
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Slow 82M segment, slow 41M segment, immediate optional reride for fall or pass (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Slow 82M segment, slow 41M segment, immediate optional reride for fall or pass (See 9.10.B.2)";
                }

                return;
            }

            /*
             * Successful jump
             */
            if ( curBoatSpeed >= myMaxSpeedDiv ) {
                /*
                 * Skier speed matches division max speed allowed
                 */
                #region Skier speed matches division max speed allowed
                myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
                myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideRecap"].Value = "Y";
                myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
                myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
                myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

                if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    /*
                     * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                     */
                    if ( curTolSplit52Time == 0 ) {
                        myTimeTolMsg = "Slow 82M segment, slow 41M end time, optional reride, protected score, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time > 0 ) {
                        myTimeTolMsg = "Fast 52M segment, slow 82M segment, slow 41M end time, optional reride, protected score, improvement allowed (See IWWF 10.06)";

                    } else if ( curTolSplit52Time < 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myTimeTolMsg = "Slow 52M segment, slow 82M segment, slow 41M end time, optional reride, score not protected, improvement allowed (See IWWF 10.06)";
                    }

                } else {
                    if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                        myTimeTolMsg = "Slow 82M segment, slow 41M end time, optional reride, protected score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Slow 82M segment, slow 41M end time, optional reride, protected score, improvement allowed (See 9.10.B.5), use RTB slow column if applicable (See 9.17).";
                    }
                }

                return;
                #endregion
            }

            /*
             * Skier speed less than allowed division max speed
             */
            myRecapViewRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapViewRow.Cells["ScoreProtRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideRecap"].Value = "Y";
            myRecapViewRow.Cells["RerideIfBestRecap"].Value = "N";
            myRecapViewRow.Cells["RerideCanImproveRecap"].Value = "Y";
            myRecapViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;

            /*
             * 82M slow, 41M slow
             */
            #region 82M slow, 41M slow
            if ( myJump3TimesDiv.Length > 0 && curScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                /*
                 * 52M NT segment required for L/R skiers in specific divisions when jump exceeds specified distance for the division
                 */
                if ( curTolSplit52Time == 0 ) {
                    myTimeTolMsg = "Slow 82M segment, slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";

                } else if ( curTolSplit52Time > 0 ) {
                    if ( curTolSplit52TimeMax > 0 ) {
                        myRecapViewRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapViewRow.Cells["RerideIfBestRecap"].Value = "Y";
                        myTimeTolMsg = "Fast 52M segment for max speed, slow 82M segment, slow 41M end time, mandatory reride if best, otherwise optional reride with protected score, improvement allowed (See IWWF 10.06)";
                    } else {
                        myTimeTolMsg = "Fast 52M segment but good for max speed, slow 82M segment, slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                    }

                } else if ( curTolSplit52Time < 0 ) {
                    myTimeTolMsg = "Slow 52M segment, slow 82M segment, slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                }

            } else {
                if ( (Decimal)myClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
                    myTimeTolMsg = "Slow 82M segment, slow 41M end time, optional reride, score protected, improvement allowed (See IWWF 10.06)";
                } else {
                    myTimeTolMsg = "Slow 82M segment, slow 41M end time, optional reride, score protected, improvement allowed (See 9.10.B.4)";
                }
            }

            #endregion
            return;
        }

        private Int16[] validate82MSegmentTime( Decimal inBoatSplit82Time, String inEventClass, Int16 inBoatSpeed, String inResults ) {
            Int16 curTolSplit82TimeMax = 0;
            Int16 curTolSplit82Time = 0;
            Decimal curMinTime, curMaxTime, curActualTime;
            //DataRow curTimeMaxSpeedRow = null;

            String curTimeKey82Max = "";
            String curTimeKey82 = inBoatSpeed.ToString() + "-" + inEventClass;
            if ( inResults.ToLower().Equals( "pass" ) ) {
                curTimeKey82 = curTimeKey82 + "-Balk";
                if ( inBoatSpeed < myMaxSpeedDiv ) curTimeKey82Max = myMaxSpeedDiv.ToString() + "-" + inEventClass + "-82M";
            
            } else {
                curTimeKey82 = curTimeKey82 + "-82M";
                if ( inBoatSpeed < myMaxSpeedDiv ) curTimeKey82Max = myMaxSpeedDiv.ToString() + "-" + inEventClass + "-82M";
            }

            DataRow curTimeRow = getTimeEntry( curTimeKey82 );
            if ( curTimeRow != null ) {
                curMinTime = (Decimal)curTimeRow["MinValue"];
                curMaxTime = (Decimal)curTimeRow["MaxValue"];
                curActualTime = Convert.ToDecimal( (String)curTimeRow["CodeValue"] );
                if ( inBoatSplit82Time > curMaxTime ) {
                    curTolSplit82Time = -1; //Slow split time

                } else if ( inBoatSplit82Time < curMinTime ) {
                    curTolSplit82Time = 1; //Fast split time

                    if ( inBoatSpeed < myMaxSpeedDiv ) {
                        curTimeRow = getTimeEntry( curTimeKey82Max );
                        curMinTime = (Decimal)curTimeRow["MinValue"];
                        if ( inBoatSplit82Time < curMinTime ) {
                            curTolSplit82TimeMax = 1; //Fast split for max speed
                        }
                    }

                } else {
                    curTolSplit82Time = 0;
                }
            }

            myRecapViewRow.Cells["Split82TimeTolRecap"].Value = ( curTolSplit82Time + curTolSplit82TimeMax ).ToString();
            return new Int16[] { curTolSplit82Time, curTolSplit82TimeMax };
        }

        /* -----------------------------------
		 * Validate first 52M segment time
		 * ----------------------------------- */
        private Int16[] validate52MSegmentTime( Decimal inBoatSplit52Time, String inEventClass, Int16 inBoatSpeed, Decimal inScoreMeters ) {
            Int16 curTolSplit52TimeMax = 0;
            Int16 curTolSplit52Time = 0;
            Decimal curMinTime, curMaxTime, curActualTime;

            String curTimeKey52Max = "";
            String curTimeKey52 = inBoatSpeed.ToString() + "-" + inEventClass + "-52M";
            if ( inBoatSpeed < myMaxSpeedDiv ) curTimeKey52Max = myMaxSpeedDiv.ToString() + "-" + inEventClass + "-52M";

            DataRow curTimeRow = getTimeEntry( curTimeKey52 );
            if ( curTimeRow != null ) {
                curMinTime = (Decimal)curTimeRow["MinValue"];
                curMaxTime = (Decimal)curTimeRow["MaxValue"];
                curActualTime = Convert.ToDecimal( (String)curTimeRow["CodeValue"] );
                if ( inBoatSplit52Time > curMaxTime ) {
                    curTolSplit52Time = -1; //Slow split time

                } else if ( inBoatSplit52Time < curMinTime ) {
                    curTolSplit52Time = 1; //Fast split time

                    if ( inBoatSpeed < myMaxSpeedDiv ) {
                        curTimeRow = getTimeEntry( curTimeKey52Max );
                        curMinTime = (Decimal)curTimeRow["MinValue"];
                        if ( inBoatSplit52Time < curMinTime ) {
                            curTolSplit52TimeMax = 1; //Fast split for max speed
                        }
                    }

                } else {
                    curTolSplit52Time = 0;
                }

            } else {
                curTolSplit52Time = 1; //Default to fast split time
            }

            if ( myJump3TimesDiv.Length > 0 ) {
                if ( inScoreMeters >= (Decimal)myJump3TimesDiv[0]["MaxValue"] ) {
                    myRecapViewRow.Cells["Split52TimeTolRecap"].Value = ( curTolSplit52Time + curTolSplit52TimeMax ).ToString();
                } else {
                    myRecapViewRow.Cells["Split52TimeTolRecap"].Value = "0";
                }
            } else {
                myRecapViewRow.Cells["Split52TimeTolRecap"].Value = "0";
            }

            return new Int16[] { curTolSplit52Time, curTolSplit52TimeMax };
        }

        /* -----------------------------------
		 * Validate 41M segment (end) time
		 * ----------------------------------- */
        private Int16[] validate41MSegmentTime( Decimal inBoatEndTime, String inEventClass, Int16 inBoatSpeed, String inResults, String inRtb ) {
            Int16 curTolEndTimeMax = 0;
            Int16 curTolEndTime = 0;
            Decimal curMinTime, curMaxTime, curActualTime;

            String curTimeKey41Max = "";
            String curTimeKey41 = "";

            if ( inResults.ToLower().Equals( "pass" ) ) {
                curTolEndTime = 0;

            } else {
                curTimeKey41 = inBoatSpeed.ToString() + "-" + inEventClass + "-41M";
                if ( inRtb.Equals( "Y" ) ) {
                    curTimeKey41 = curTimeKey41 + "-RTB";
                    if ( inBoatSpeed < myMaxSpeedDiv ) {
                        curTimeKey41Max = myMaxSpeedDiv.ToString() + "-" + inEventClass + "-41M-RTB";
                    } else {
                        curTimeKey41Max = myMaxSpeedDiv.ToString() + "-" + inEventClass + "-41M";
                    }

                } else {
                    if ( inBoatSpeed < myMaxSpeedDiv ) {
                        curTimeKey41Max = myMaxSpeedDiv.ToString() + "-" + inEventClass + "-41M";
                    }
                }

                DataRow curTimeRow = getTimeEntry( curTimeKey41 );
                if ( curTimeRow != null ) {
                    curMinTime = (Decimal)curTimeRow["MinValue"];
                    curMaxTime = (Decimal)curTimeRow["MaxValue"];
                    curActualTime = Convert.ToDecimal( (String)curTimeRow["CodeValue"] );
                    if ( inBoatEndTime > curMaxTime ) {
                        curTolEndTime = -1; //Slow end course time

                    } else if ( inBoatEndTime < curMinTime ) {
                        curTolEndTime = 1; //Fast end course time
                        if ( inBoatSpeed < myMaxSpeedDiv ) {
                            curTimeRow = getTimeEntry( curTimeKey41Max );
                            curMinTime = (Decimal)curTimeRow["MinValue"];
                            if ( inBoatEndTime < curMinTime ) {
                                curTolEndTimeMax = 1; //Fast end course time for max speed
                            }

                        } else {
                            if ( inRtb.Equals( "Y" ) ) {
                                curTimeRow = getTimeEntry( curTimeKey41Max );
                                curMinTime = (Decimal)curTimeRow["MinValue"];
                                if ( inBoatEndTime < curMinTime ) {
                                    curTolEndTimeMax = 1; //Fast end course time for fast second segment speed
                                }
                            }
                        }

                    } else {
                        curTolEndTime = 0;
                    }
                }
            }

            myRecapViewRow.Cells["Split41TimeTolRecap"].Value = ( curTolEndTime + curTolEndTimeMax ).ToString();
            return new Int16[] { curTolEndTime, curTolEndTimeMax };
        }

        public void SplitTimeFormat() {
            Decimal curMinTime, curMaxTime, curActualTime;
            Decimal tempTime, tempDiff1, tempDiff2, tempDiff3;
            String curTimeOrigValue;

            Int16 curBoatSpeed = Convert.ToInt16( (String)myRecapViewRow.Cells["BoatSpeedRecap"].Value );
            String curResults = (String)myRecapViewRow.Cells["ResultsRecap"].Value;
            String curDiv = (String)myRecapViewRow.Cells["AgeGroupRecap"].Value;
            String curEventClass = HelperFunctions.getDataRowColValue( myClassRowSkier, "ListCode", JumpEventData.myTourClass );

            //-----------------------------------
            //Validate first segment time
            //-----------------------------------
            String curTimeKey = curBoatSpeed.ToString() + "-" + curEventClass + "-52M";
            DataRow curTimeRow = getTimeEntry( curTimeKey );
            if ( curTimeRow == null ) return;

            curMinTime = (Decimal)curTimeRow["MinValue"];
            curMaxTime = (Decimal)curTimeRow["MaxValue"];
            curActualTime = Convert.ToDecimal( (String)curTimeRow["CodeValue"] );

            curTimeOrigValue = (String)myRecapViewRow.Cells["BoatSplitTimeRecap"].Value;
            if ( curTimeOrigValue.ToUpper().Equals( "OK" ) ) {
                curTimeOrigValue = curActualTime.ToString( "#0.00" );
                myRecapViewRow.Cells["BoatSplitTimeRecap"].Value = curTimeOrigValue;
            
            } else if ( curTimeOrigValue.ToUpper().Equals( "NONE" ) ) {
                curTimeOrigValue = .01m.ToString( "#0.00" );
                myRecapViewRow.Cells["BoatSplitTimeRecap"].Value = curTimeOrigValue;
            }

            if ( curTimeOrigValue.Length == 1 ) curTimeOrigValue = "0" + curTimeOrigValue;
            if ( curTimeOrigValue.Length != 2 ) return;
            if ( curTimeOrigValue.Contains( "." ) ) return;

            Int32 delimPos = curActualTime.ToString().IndexOf( '.' );
            Int32 curDigits = Convert.ToInt32( curActualTime.ToString().Substring( 0, delimPos ) );
            tempTime = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue );
            if ( tempTime < curActualTime ) {
                tempDiff1 = curActualTime - tempTime;
            } else {
                tempDiff1 = tempTime - curActualTime;
            }
            tempTime = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue );
            tempDiff2 = tempTime - curActualTime;
            tempTime = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue );
            tempDiff3 = curActualTime - tempTime;

            if ( tempDiff1 < tempDiff2 ) {
                if ( tempDiff1 < tempDiff3 ) {
                    myRecapViewRow.Cells["BoatSplitTimeRecap"].Value = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                } else {
                    myRecapViewRow.Cells["BoatSplitTimeRecap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                }
            
            } else {
                if ( tempDiff2 < tempDiff3 ) {
                    myRecapViewRow.Cells["BoatSplitTimeRecap"].Value = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                } else {
                    myRecapViewRow.Cells["BoatSplitTimeRecap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                }
            }
        }

        public void Split2TimeFormat() {
            Decimal curMinTime, curMaxTime, curActualTime;
            Decimal tempTime, tempDiff1, tempDiff2, tempDiff3;
            String curTimeOrigValue;

            Int16 curBoatSpeed = Convert.ToInt16( (String)myRecapViewRow.Cells["BoatSpeedRecap"].Value );
            String curResults = (String)myRecapViewRow.Cells["ResultsRecap"].Value;
            String curDiv = (String)myRecapViewRow.Cells["AgeGroupRecap"].Value;
            String curEventClass = HelperFunctions.getDataRowColValue( myClassRowSkier, "ListCode", JumpEventData.myTourClass );

            //-----------------------------------
            //Validate second segment time
            //-----------------------------------
            String curTimeKey = curBoatSpeed.ToString() + "-" + curEventClass + "-82M";
            DataRow curTimeRow = getTimeEntry( curTimeKey );
            if ( curTimeRow == null ) return;

            curMinTime = (Decimal)curTimeRow["MinValue"];
            curMaxTime = (Decimal)curTimeRow["MaxValue"];
            curActualTime = ( curMinTime + curMaxTime ) / 2;

            curTimeOrigValue = myRecapViewRow.Cells["BoatSplitTime2Recap"].Value.ToString();
            if ( curTimeOrigValue.ToUpper().Equals( "OK" ) ) {
                curTimeOrigValue = curActualTime.ToString( "#0.00" );
                myRecapViewRow.Cells["BoatSplitTime2Recap"].Value = curTimeOrigValue;
            } else if ( curTimeOrigValue.ToUpper().Equals( "NONE" ) ) {
                curTimeOrigValue = .01m.ToString( "#0.00" );
                myRecapViewRow.Cells["BoatSplitTime2Recap"].Value = curTimeOrigValue;
            }

            if ( curTimeOrigValue.Length == 1 ) curTimeOrigValue = "0" + curTimeOrigValue;
            if ( curTimeOrigValue.Length != 2 ) return;
            if ( curTimeOrigValue.Contains( "." ) ) return;

            Int32 delimPos = curActualTime.ToString().IndexOf( '.' );
            Int32 curDigits = Convert.ToInt32( curActualTime.ToString().Substring( 0, delimPos ) );
            tempTime = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue );
            if ( tempTime < curActualTime ) {
                tempDiff1 = curActualTime - tempTime;
            } else {
                tempDiff1 = tempTime - curActualTime;
            }
            tempTime = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue );
            tempDiff2 = tempTime - curActualTime;
            tempTime = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue );
            tempDiff3 = curActualTime - tempTime;

            if ( tempDiff1 < tempDiff2 ) {
                if ( tempDiff1 < tempDiff3 ) {
                    myRecapViewRow.Cells["BoatSplitTime2Recap"].Value = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                } else {
                    myRecapViewRow.Cells["BoatSplitTime2Recap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                }
            
            } else {
                if ( tempDiff2 < tempDiff3 ) {
                    myRecapViewRow.Cells["BoatSplitTime2Recap"].Value = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                } else {
                    myRecapViewRow.Cells["BoatSplitTime2Recap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                }
            }
        }

        public void EndTimeFormat() {
            Decimal curMinTime, curMaxTime, curActualTime;
            Decimal tempTime, tempDiff1, tempDiff2, tempDiff3;
            String curTimeOrigValue;

            Int16 curBoatSpeed = Convert.ToInt16( (String)myRecapViewRow.Cells["BoatSpeedRecap"].Value );
            String curResults = (String)myRecapViewRow.Cells["ResultsRecap"].Value;
            String curRtb = (String)myRecapViewRow.Cells["ReturnToBaseRecap"].Value;
            String curDiv = (String)myRecapViewRow.Cells["AgeGroupRecap"].Value;
            String curEventClass = HelperFunctions.getDataRowColValue( myClassRowSkier, "ListCode", JumpEventData.myTourClass );

            //-----------------------------------
            //Validate second segment time
            //-----------------------------------
            String curTimeKey = curBoatSpeed.ToString() + "-" + curEventClass + "-41M";
            if ( curRtb.Equals( "Y" ) ) curTimeKey = curTimeKey + "-RTB";
            DataRow curTimeRow = getTimeEntry( curTimeKey );
            if ( curTimeRow == null ) return;

            curMinTime = (Decimal)curTimeRow["MinValue"];
            curMaxTime = (Decimal)curTimeRow["MaxValue"];
            curActualTime = Convert.ToDecimal( (String)curTimeRow["CodeValue"] );

            curTimeOrigValue = myRecapViewRow.Cells["BoatEndTimeRecap"].Value.ToString();
            if ( curTimeOrigValue.ToUpper().Equals( "OK" ) ) {
                curTimeOrigValue = curActualTime.ToString( "#0.00" );
                myRecapViewRow.Cells["BoatEndTimeRecap"].Value = curTimeOrigValue;
            } else if ( curTimeOrigValue.ToUpper().Equals( "NONE" ) ) {
                curTimeOrigValue = .01m.ToString( "#0.00" );
                myRecapViewRow.Cells["BoatEndTimeRecap"].Value = curTimeOrigValue;
            }

            if ( curTimeOrigValue.Length == 1 ) curTimeOrigValue = "0" + curTimeOrigValue;
            if ( curTimeOrigValue.Length != 2 ) return;
            if ( curTimeOrigValue.Contains( "." ) ) return;

            Int32 delimPos = curActualTime.ToString().IndexOf( '.' );
            Int32 curDigits = Convert.ToInt32( curActualTime.ToString().Substring( 0, delimPos ) );
            tempTime = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue );
            if ( tempTime < curActualTime ) {
                tempDiff1 = curActualTime - tempTime;
            } else {
                tempDiff1 = tempTime - curActualTime;
            }
            tempTime = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue );
            tempDiff2 = tempTime - curActualTime;
            tempTime = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue );
            tempDiff3 = curActualTime - tempTime;

            if ( tempDiff1 < tempDiff2 ) {
                if ( tempDiff1 < tempDiff3 ) {
                    myRecapViewRow.Cells["BoatEndTimeRecap"].Value = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                } else {
                    myRecapViewRow.Cells["BoatEndTimeRecap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                }

            } else {
                if ( tempDiff2 < tempDiff3 ) {
                    myRecapViewRow.Cells["BoatEndTimeRecap"].Value = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                } else {
                    myRecapViewRow.Cells["BoatEndTimeRecap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                }
            }
        }

        public DataRow getTimeEntry(String inTimeKey ) {
            DataRow[] curRowsFound = JumpEventData.myTimesDataTable.Select( String.Format("ListCode = '{0}'", inTimeKey ) );
            if ( curRowsFound.Length > 0 ) return curRowsFound[0];

            int curDelimStart = inTimeKey.IndexOf( "-" );
            int curDelimEnd = inTimeKey.LastIndexOf( "-" );
            String curTimeKey = inTimeKey.Substring( 0, curDelimStart + 1 ) + "C" + inTimeKey.Substring( curDelimEnd );
            curRowsFound = JumpEventData.myTimesDataTable.Select( String.Format( "ListCode = '{0}'", curTimeKey ) );
            if ( curRowsFound.Length > 0 ) return curRowsFound[0];

            String curMsg = String.Format( "Jump boat time entry not found for key {0}", inTimeKey );
            Log.WriteFile( curMsg );
            MessageBox.Show( curMsg );
            return null;
        }
    }
}
