using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlServerCe;

namespace WaterskiScoringSystem.Common {
    class CalcEventPlcmt {
        private String mySanctionNum;

        public CalcEventPlcmt() {
        }

        public DataTable setSlalomPlcmt( DataRow inTourRow, DataTable inEventResults ) {
            return setSlalomPlcmt( inTourRow, inEventResults, "score", "div", "" );
        }
        public DataTable setSlalomPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtOrg ) {
            return setSlalomPlcmt( inTourRow, inEventResults, "score", inPlcmtOrg, "" );
        }
        public DataTable setSlalomPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg ) {
            return setSlalomPlcmt( inTourRow, inEventResults, inPlcmtMethod, inPlcmtOrg, "" );
        }
        public DataTable setSlalomPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType ) {
            /* **********************************************************
             * Use results from slalom event to determine skier placment 
             * within age division
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
            String curFilterCmd = "";
            int curEventRounds = 0;
            try {
                curEventRounds = Convert.ToInt16( inTourRow["SlalomRounds"].ToString() );
            } catch {
                curEventRounds = 0;
            }

            String curPlcmt, curGroup, curGroupName, curSelectCmd;
            DataRow[] curSkierList;
            DataTable curSkierScoreList, curTiePlcmtList;

            String curPlcmtName = "PlcmtSlalom";
            DataTable curPlcmtResults = setInitEventPlcmt( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, curPlcmtName, "Slalom" );

            //Analzye and assign placements by age group and / or event groups as specifeid by request
            if (curRules.ToLower().Equals( "awsa" )) {
                if (inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
                    #region AWSA placement calculations - placements for ties for the total tournament
                    //Calculate placements for ties for the total tournament
                    curTiePlcmtList = findEventPlcmtTiesTour( curPlcmtName, curPlcmtResults );
                    foreach (DataRow curRow in curTiePlcmtList.Rows) {
                        curPlcmt = (String)curRow[curPlcmtName];
                        curFilterCmd = curPlcmtName + " = '" + curPlcmt + "'";
                        curSkierList = curPlcmtResults.Select( curFilterCmd );

                        //Analyze tied placments and gather all data needed for tie breakers
                        curSkierScoreList = getSlalomTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName, inDataType );

                        //Analyze data for tied placments utilizing tie breakers to determine placments
                        setSlalomTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                        //Analyze results of tie breakers and update event skier placments
                        setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                    }
                    #endregion
                } else {
                    #region AWSA placement calculations - Analzye and assign placements by age group and / or event groups as specifeid by request
                    curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
                    foreach (DataRow curRow in curTiePlcmtList.Rows) {
                        curPlcmt = (String)curRow[curPlcmtName];
                        curGroup = "";
                        if (inPlcmtOrg.ToLower().Equals( "div" )) {
                            curGroupName = "AgeGroup";
                            curGroup = (String)curRow[curGroupName];
                            curSelectCmd = curGroupName + " = '" + curGroup + "'";
                        } else if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                            curSelectCmd = "AgeGroup ASC, EventGroup ASC";
                            curGroupName = "AgeGroup,EventGroup";
                            curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
                            curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
                                + "AND EventGroup = '" + (String)curRow["EventGroup"] + "'";
                        } else if (inPlcmtOrg.ToLower().Equals( "group" )) {
                            curGroupName = "EventGroup";
                            curGroup = (String)curRow[curGroupName];
                            curSelectCmd = curGroupName + " = '" + curGroup + "'";
                        } else {
                            curGroupName = "";
                            curGroup = "";
                            curSelectCmd = "";
                        }

                        //Get results for the skiers that are tied for the placement by age group and position
                        curFilterCmd = curSelectCmd + " AND " + curPlcmtName + " = '" + curPlcmt + "'";
                        curSkierList = curPlcmtResults.Select( curFilterCmd );

                        //Analyze tied placments and gather all data needed for tie breakers
                        curSkierScoreList = getSlalomTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName, inDataType );

                        //Analyze data for tied placments utilizing tie breakers to determine placments
                        setSlalomTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                        //Analyze results of tie breakers and update event skier placments
                        setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                    }
                    #endregion
                }
            }

            return curPlcmtResults;
        }
        
        public DataTable setSlalomPlcmtIwwf(DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType) {
            /* **********************************************************
             * Use results from slalom event to determine skier placment 
             * within age division
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
            String curFilterCmd = "";
            int curEventRounds = 0;
            try {
                curEventRounds = Convert.ToInt16( inTourRow["SlalomRounds"].ToString() );
            } catch {
                curEventRounds = 0;
            }

            String curPlcmt, curGroup, curGroupName, curSelectCmd, curPlcmtName;
            DataRow[] curSkierList;
            DataTable curSkierScoreList, curTiePlcmtList;

            DataTable curPlcmtResults = setInitEventPlcmtIwwf( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, "Slalom" );

            //Analzye and assign placements by age group and / or event groups as specifeid by request
            curPlcmtName = "PlcmtSlalom";
            curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
            foreach (DataRow curRow in curTiePlcmtList.Rows) {
                curPlcmt = (String)curRow[curPlcmtName];
                curGroup = "";
                if (inPlcmtOrg.ToLower().Equals( "div" )) {
                    curGroupName = "AgeGroup";
                    curGroup = (String)curRow[curGroupName];
                    curSelectCmd = curGroupName + " = '" + curGroup + "' AND " + curPlcmtName + " = '" + curPlcmt + "' AND ";
                } else if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                    curSelectCmd = "AgeGroup ASC, EventGroup ASC";
                    curGroupName = "AgeGroup,EventGroup";
                    curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
                    curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
                        + "AND EventGroup = '" + (String)curRow["EventGroup"] + "' AND " + curPlcmtName + " = '" + curPlcmt + "' AND ";
                } else if (inPlcmtOrg.ToLower().Equals( "group" )) {
                    curGroupName = "EventGroup";
                    curGroup = (String)curRow[curGroupName];
                    curSelectCmd = curGroupName + " = '" + curGroup + "' AND " + curPlcmtName + " = '" + curPlcmt + "' AND ";
                } else {
                    curGroupName = "";
                    curGroup = "";
                    curSelectCmd = "";
                }

                //Get results for the skiers that are tied for the placement by age group and position
                curFilterCmd = curSelectCmd + curPlcmtName + " = '" + curPlcmt + "'";
                curSkierList = curPlcmtResults.Select( curFilterCmd );

                //Analyze tied placments and gather all data needed for tie breakers
                //curSkierScoreList = getSlalomTieBreakerData( curSkierList, inEventResults, curEventRounds, curRules, curPlcmtName );
                curSkierScoreList = getSlalomTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName, inDataType );

                //Analyze data for tied placments utilizing tie breakers to determine placments
                setSlalomTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                //Analyze results of tie breakers and update event skier placments
                setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
            }

            return curPlcmtResults;
        }

        public DataTable setTrickPlcmt(DataRow inTourRow, DataTable inEventResults) {
            return setTrickPlcmt( inTourRow, inEventResults, "score", "div", "" );
        }
        public DataTable setTrickPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg ) {
            return setTrickPlcmt( inTourRow, inEventResults, inPlcmtMethod, inPlcmtOrg, "" );
        }
        public DataTable setTrickPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType ) {
            /* **********************************************************
             * Use results from trick event to determine skier placment 
             * within age division
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
            int curEventRounds = 0;
            try {
                curEventRounds = Convert.ToInt16( inTourRow["TrickRounds"].ToString() );
            } catch {
                curEventRounds = 0;
            }

            String curPlcmt, curGroup, curGroupName, curSelectCmd, curPlcmtName;
            DataRow[] curSkierList;
            DataTable curSkierScoreList, curTiePlcmtList;

            curPlcmtName = "PlcmtTrick";
            DataTable curPlcmtResults = setInitEventPlcmt( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, curPlcmtName, "Trick" );

            if (curRules.ToLower().Equals( "awsa" )) {
                if (inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
                    #region AWSA placement calculations - placements for ties for the total tournament
                    curPlcmtName = "PlcmtTrick";
                    curTiePlcmtList = findEventPlcmtTiesTour( curPlcmtName, curPlcmtResults );
                    foreach (DataRow curRow in curTiePlcmtList.Rows) {
                        curPlcmt = (String)curRow[curPlcmtName];
                        curSkierList = curPlcmtResults.Select( curPlcmtName + " = '" + curPlcmt + "'" );

                        //Analyze tied placments and gather all data needed for tie breakers
                        curSkierScoreList = getTrickTieBreakerData( curSkierList, curEventRounds, curRules );

                        //Analyze data for tied placments utilizing tie breakers to determine placments
                        setTrickTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                        //Analyze results of tie breakers and update event skier placments
                        setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                    }
                    #endregion
                } else {
                    #region AWSA placement calculations - Analzye and assign placements by age group and / or event groups as specifeid by request
                    curPlcmtName = "PlcmtTrick";
                    curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
                    foreach (DataRow curRow in curTiePlcmtList.Rows) {
                        curPlcmt = (String)curRow[curPlcmtName];
                        curGroup = "";
                        if (inPlcmtOrg.ToLower().Equals( "div" )) {
                            curGroupName = "AgeGroup";
                            curGroup = (String)curRow[curGroupName];
                            curSelectCmd = curGroupName + " = '" + curGroup + "'";
                        } else if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                            curSelectCmd = "AgeGroup ASC, EventGroup ASC";
                            curGroupName = "AgeGroup,EventGroup";
                            curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
                            curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
                                + "AND EventGroup = '" + (String)curRow["EventGroup"] + "'";
                        } else if (inPlcmtOrg.ToLower().Equals( "group" )) {
                            curGroupName = "EventGroup";
                            curGroup = (String)curRow[curGroupName];
                            curSelectCmd = curGroupName + " = '" + curGroup + "'";
                        } else {
                            curGroupName = "";
                            curGroup = "";
                            curSelectCmd = "";
                        }

                        //Get results for the skiers that are tied for the placement by age group and position
                        curSkierList = curPlcmtResults.Select( curSelectCmd + " AND " + curPlcmtName + " = '" + curPlcmt + "'" );

                        //Analyze tied placments and gather all data needed for tie breakers
                        curSkierScoreList = getTrickTieBreakerData( curSkierList, curEventRounds, curRules );

                        //Analyze data for tied placments utilizing tie breakers to determine placments
                        setTrickTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                        //Analyze results of tie breakers and update event skier placments
                        setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                    }
                    #endregion
                }
            }

            return curPlcmtResults;
        }

        public DataTable setTrickPlcmtIwwf(DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType) {
            /* **********************************************************
             * Use results from Trick event to determine skier placment 
             * within age division
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
            String curFilterCmd = "";
            int curEventRounds = 0;
            try {
                curEventRounds = Convert.ToInt16( inTourRow["TrickRounds"].ToString() );
            } catch {
                curEventRounds = 0;
            }

            String curPlcmt, curGroup, curGroupName, curSelectCmd, curPlcmtName;
            DataRow[] curSkierList;
            DataTable curSkierScoreList, curTiePlcmtList;

            DataTable curPlcmtResults = setInitEventPlcmtIwwf( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, "Trick" );

            //Analzye and assign placements by age group and / or event groups as specifeid by request
            curPlcmtName = "PlcmtTrick";
            curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
            foreach (DataRow curRow in curTiePlcmtList.Rows) {
                curPlcmt = (String)curRow[curPlcmtName];
                curGroup = "";
                if (inPlcmtOrg.ToLower().Equals( "div" )) {
                    curGroupName = "AgeGroup";
                    curGroup = (String)curRow[curGroupName];
                    curSelectCmd = curGroupName + " = '" + curGroup + "' AND ";
                } else if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                    curSelectCmd = "AgeGroup ASC, EventGroup ASC";
                    curGroupName = "AgeGroup,EventGroup";
                    curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
                    curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
                        + "AND EventGroup = '" + (String)curRow["EventGroup"] + "' AND ";
                } else if (inPlcmtOrg.ToLower().Equals( "group" )) {
                    curGroupName = "EventGroup";
                    curGroup = (String)curRow[curGroupName];
                    curSelectCmd = curGroupName + " = '" + curGroup + "' AND ";
                } else {
                    curGroupName = "";
                    curGroup = "";
                    curSelectCmd = "";
                }

                //Get results for the skiers that are tied for the placement by age group and position
                curSkierList = curPlcmtResults.Select( curSelectCmd + curPlcmtName + " = '" + curPlcmt + "'" );

                //Analyze tied placments and gather all data needed for tie breakers
                curSkierScoreList = getTrickTieBreakerData( curSkierList, curEventRounds, curRules );

                //Analyze data for tied placments utilizing tie breakers to determine placments
                setTrickTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                //Analyze results of tie breakers and update event skier placments
                setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
            }

            return curPlcmtResults;
        }

        public DataTable setJumpPlcmt(DataRow inTourRow, DataTable inEventResults) {
            return setJumpPlcmt( inTourRow, inEventResults, "score", "div", "" );
        }
        public DataTable setJumpPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg ) {
            return setJumpPlcmt( inTourRow, inEventResults, inPlcmtMethod, inPlcmtOrg, "" );
        }
        public DataTable setJumpPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType ) {
            /* **********************************************************
             * Use results from jump event to determine skier placment 
             * within age division
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
            int curEventRounds = 0;
            try {
                curEventRounds = Convert.ToInt16( inTourRow["JumpRounds"].ToString() );
            } catch {
                curEventRounds = 0;
            }

            String curPlcmt, curGroup, curGroupName, curSelectCmd, curPlcmtName;
            DataRow[] curSkierList;
            DataTable curSkierScoreList, curTiePlcmtList;

            curPlcmtName = "PlcmtJump";
            DataTable curPlcmtResults = setInitEventPlcmt( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, curPlcmtName, "Jump" );

            if ( curRules.ToLower().Equals( "awsa" ) ) {
                if (inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
                    #region AWSA placement calculations - placements for ties for the total tournament
                    curPlcmtName = "PlcmtJump";
                    curTiePlcmtList = findEventPlcmtTiesTour( curPlcmtName, curPlcmtResults );
                    foreach (DataRow curRow in curTiePlcmtList.Rows) {
                        curPlcmt = (String)curRow[curPlcmtName];
                        curSkierList = curPlcmtResults.Select( curPlcmtName + " = '" + curPlcmt + "'" );

                        //Analyze tied placments and gather all data needed for tie breakers
                        curSkierScoreList = getJumpTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName );

                        //Analyze data for tied placments utilizing tie breakers to determine placments
                        setJumpTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                        //Analyze results of tie breakers and update event skier placments
                        setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                    }
                    #endregion
                } else {
                    #region AWSA placement calculations - Analzye and assign placements by age group and / or event groups as specifeid by request
                    curPlcmtName = "PlcmtJump";
                    curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
                    foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
                        curPlcmt = (String)curRow[curPlcmtName];
                        curGroup = "";
                        if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
                            curGroupName = "AgeGroup";
                            curGroup = (String)curRow[curGroupName];
                            curSelectCmd = curGroupName + " = '" + curGroup + "'";
                        } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                            curSelectCmd = "AgeGroup ASC, EventGroup ASC";
                            curGroupName = "AgeGroup,EventGroup";
                            curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
                            curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
                                + "AND EventGroup = '" + (String)curRow["EventGroup"] + "'";
                        } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                            curGroupName = "EventGroup";
                            curGroup = (String)curRow[curGroupName];
                            curSelectCmd = curGroupName + " = '" + curGroup + "'";
                        } else {
                            curGroupName = "";
                            curGroup = "";
                            curSelectCmd = "";
                        }

                        //Get results for the skiers that are tied for the placement by age group and position
                        curSkierList = curPlcmtResults.Select( curSelectCmd + " AND " + curPlcmtName + " = '" + curPlcmt + "'" );

                        //Analyze tied placments and gather all data needed for tie breakers
                        curSkierScoreList = getJumpTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName );

                        //Analyze data for tied placments utilizing tie breakers to determine placments
                        setJumpTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                        //Analyze results of tie breakers and update event skier placments
                        setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                    }
                    #endregion
                }
            } else if (curRules.ToLower().Equals( "ncwsa" )) {
                if (inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
                    #region Collegiate placement calculations - placements for ties for the total tournament
                    if (curPlcmtResults.Columns.Contains( "PlcmtTour" )) {
                        curPlcmtName = "PlcmtJump";
                        curTiePlcmtList = findEventPlcmtTiesTour( curPlcmtName, curPlcmtResults );
                        foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
                            curPlcmt = (String)curRow[curPlcmtName];
                            curSkierList = curPlcmtResults.Select( curPlcmtName + " = '" + curPlcmt + "'" );

                            //Analyze tied placments and gather all data needed for tie breakers
                            curSkierScoreList = getJumpTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName );

                            //Analyze data for tied placments utilizing tie breakers to determine placments
                            setJumpTieBreakerNcwsaPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                            //Analyze results of tie breakers and update event skier placments
                            setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                        }
                    }
                    #endregion
                } else {
                    #region Collegiate placement calculations - Analzye and assign placements by age group and / or event groups as specifeid by request
                    curPlcmtName = "PlcmtJump";
                    curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
                    foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
                        curPlcmt = (String)curRow[curPlcmtName];
                        curGroup = "";
                        if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
                            curGroupName = "AgeGroup";
                            curGroup = (String)curRow[curGroupName];
                            curSelectCmd = curGroupName + " = '" + curGroup + "'";
                        } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                            curSelectCmd = "AgeGroup ASC, EventGroup ASC";
                            curGroupName = "AgeGroup,EventGroup";
                            curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
                            curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
                                + "AND EventGroup = '" + (String)curRow["EventGroup"] + "'";
                        } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                            curGroupName = "EventGroup";
                            curGroup = (String)curRow[curGroupName];
                            curSelectCmd = curGroupName + " = '" + curGroup + "'";
                        } else {
                            curGroupName = "";
                            curGroup = "";
                            curSelectCmd = "";
                        }

                        //Get results for the skiers that are tied for the placement by age group and position
                        curSkierList = curPlcmtResults.Select( curSelectCmd + " AND " + curPlcmtName + " = '" + curPlcmt + "'" );

                        //Analyze tied placments and gather all data needed for tie breakers
                        curSkierScoreList = getJumpTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName );

                        //Analyze data for tied placments utilizing tie breakers to determine placments
                        setJumpTieBreakerNcwsaPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                        //Analyze results of tie breakers and update event skier placments
                        setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                    }
                    #endregion
                }
            }

            return curPlcmtResults;
        }

        public DataTable setJumpPlcmtIwwf(DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType) {
            /* **********************************************************
             * Use results from Jump event to determine skier placment 
             * within age division
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
            String curFilterCmd = "";
            int curEventRounds = 0;
            try {
                curEventRounds = Convert.ToInt16( inTourRow["JumpRounds"].ToString() );
            } catch {
                curEventRounds = 0;
            }

            String curPlcmt, curGroup, curGroupName, curSelectCmd, curPlcmtName;
            DataRow[] curSkierList;
            DataTable curSkierScoreList, curTiePlcmtList;

            DataTable curPlcmtResults = setInitEventPlcmtIwwf( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, "Jump" );

            //Analzye and assign placements by age group and / or event groups as specifeid by request
            curPlcmtName = "PlcmtJump";
            curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
            foreach (DataRow curRow in curTiePlcmtList.Rows) {
                curPlcmt = (String)curRow[curPlcmtName];
                curGroup = "";
                if (inPlcmtOrg.ToLower().Equals( "div" )) {
                    curGroupName = "AgeGroup";
                    curGroup = (String)curRow[curGroupName];
                    curSelectCmd = curGroupName + " = '" + curGroup + "' AND ";
                } else if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                    curSelectCmd = "AgeGroup ASC, EventGroup ASC";
                    curGroupName = "AgeGroup,EventGroup";
                    curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
                    curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' AND "
                        + "AND EventGroup = '" + (String)curRow["EventGroup"] + "'";
                } else if (inPlcmtOrg.ToLower().Equals( "group" )) {
                    curGroupName = "EventGroup";
                    curGroup = (String)curRow[curGroupName];
                    curSelectCmd = curGroupName + " = '" + curGroup + "' AND ";
                } else {
                    curGroupName = "";
                    curGroup = "";
                    curSelectCmd = "";
                }

                //Get results for the skiers that are tied for the placement by age group and position
                curSkierList = curPlcmtResults.Select( curSelectCmd + curPlcmtName + " = '" + curPlcmt + "'" );

                //Analyze tied placments and gather all data needed for tie breakers
                curSkierScoreList = getJumpTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName );

                //Analyze data for tied placments utilizing tie breakers to determine placments
                setJumpTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                //Analyze results of tie breakers and update event skier placments
                setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
            }

            return curPlcmtResults;
        }

        public DataTable setOverallPlcmt(DataRow inTourRow, DataTable inEventResults, DataTable inEventResultsAll) {
            return setOverallPlcmt( inTourRow, inEventResults, inEventResultsAll, "score", "div", "" );
        }
        public DataTable setOverallPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg ) {
            return setOverallPlcmt( inTourRow, inEventResults, null, inPlcmtMethod, inPlcmtOrg, "" );
        }
        public DataTable setOverallPlcmt( DataRow inTourRow, DataTable inEventResults, DataTable inEventResultsAll, String inPlcmtMethod, String inPlcmtOrg ) {
            return setOverallPlcmt( inTourRow, inEventResults, inEventResultsAll, inPlcmtMethod, inPlcmtOrg, "" );
        }
        public DataTable setOverallPlcmt( DataRow inTourRow, DataTable inEventResults, DataTable inEventResultsAll, String inPlcmtMethod, String inPlcmtOrg, String inDataType ) {
            /* **********************************************************
             * Use results from Overall event to determine skier placment 
             * within age division
             * ******************************************************* */
            String curRules = (String)inTourRow["Rules"];
            int curEventRounds = 0, curSlalomRounds = 0, curTrickRounds = 0, curJumpRounds = 0;
            try {
                curSlalomRounds = Convert.ToInt16( inTourRow["SlalomRounds"].ToString() );
            } catch {
                curSlalomRounds = 0;
            }
            try {
                curTrickRounds = Convert.ToInt16( inTourRow["TrickRounds"].ToString() );
            } catch {
                curTrickRounds = 0;
            }
            try {
                curJumpRounds = Convert.ToInt16( inTourRow["JumpRounds"].ToString() );
            } catch {
                curJumpRounds = 0;
            }
            if ( curSlalomRounds >= curTrickRounds && curSlalomRounds >= curJumpRounds ) {
                curEventRounds = curSlalomRounds;
            } else if ( curTrickRounds >= curSlalomRounds && curTrickRounds >= curSlalomRounds ) {
                curEventRounds = curSlalomRounds;
            } else if ( curJumpRounds >= curSlalomRounds && curJumpRounds >= curTrickRounds ) {
                curEventRounds = curJumpRounds;
            }

            //DataTable curPlcmtResults = setInitEventPlcmt( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType );
            String curPlcmtName = "PlcmtOverall";
            DataTable curPlcmtResults = setInitEventPlcmt( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, curPlcmtName, "Overall" );
            //String curPlcmtName = "PlcmtGroup";
            DataTable curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );

            //Analzye and assign placements by age group and / or event groups as specifeid by request
            if ( curRules.ToLower().Equals( "awsa" ) ) {
                String curPlcmt, curGroup;
                DataRow[] curSkierList;

                foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
                    curPlcmt = (String)curRow["PlcmtGroup"];
                    curGroup = (String)curRow["AgeGroup"];

                    //Get results for the skiers that are tied for the placement by age group and position
                    curSkierList = curPlcmtResults.Select( "AgeGroup = '" + curGroup + "' AND PlcmtGroup = '" + curPlcmt + "'" );

                    //Analyze tied placments and gather all data needed for tie breakers
                    DataTable curSkierScoreList = getOverallTieBreakerData( curSkierList, inEventResultsAll, curEventRounds );

                    //Analyze data for tied placments utilizing tie breakers to determine placments
                    setOverallTieBreakerPlcmt( curSkierScoreList, curPlcmt, curGroup );

                    //Analyze results of tie breakers and update event skier placments
                    setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, "PlcmtOverall" );
                }
            }

            return curPlcmtResults;
        }

        public DataTable setOverallPlcmtIwwf(DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType) {
            /* **********************************************************
             * Use results from Overall event to determine skier placment 
             * within age division
             * ******************************************************* */
            DataTable curPlcmtResults = setInitEventPlcmtIwwf( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, "Overall" );

            return curPlcmtResults;
        }

        private DataTable getSlalomTieBreakerData(DataRow[] inSkierList, int inRounds, String inRules, String inPlcmtName, String inDataType) {
            /* **********************************************************
             * Retrieve all data required for a slalom tie breaker
             * ******************************************************* */
            StringBuilder curSqlStmt = new StringBuilder( "" );
            DataRowView newSkierScoreRow;
            DataRow[] curSkierDetailRow;
            DataTable curSkierScoreList = buildSkierSlalomScoreList();
            String curScoreName = "ScoreSlalom";
            String curRoundName = "RoundSlalom";
            String curPointsName = "PointsSlalom";

            foreach ( DataRow curSkierRow in inSkierList ) {
                newSkierScoreRow = curSkierScoreList.DefaultView.AddNew();
                newSkierScoreRow["MemberId"] = (String)curSkierRow["MemberId"];
                newSkierScoreRow["AgeGroup"] = (String)curSkierRow["AgeGroup"];
                try {
                    if ( curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Byte" ) ) {
                        newSkierScoreRow["Round"] = (Byte)curSkierRow[curRoundName];
                    } else if ( curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Int16" ) ) {
                        newSkierScoreRow["Round"] = (Int16)curSkierRow[curRoundName];
                    } else if ( curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Int32" ) ) {
                        newSkierScoreRow["Round"] = (int)curSkierRow[curRoundName];
                    } else {
                        newSkierScoreRow["Round"] = 1;
                    }
                } catch {
                    newSkierScoreRow["Round"] = 1;
                }

                newSkierScoreRow["Score"] = 0;
                newSkierScoreRow["Speed"] = 0;
                newSkierScoreRow["PassScore"] = 0;
                newSkierScoreRow["RunoffScore"] = 0;
                newSkierScoreRow["FirstScore"] = 0;
                newSkierScoreRow["BackupScore"] = 0;
                newSkierScoreRow["RankingScore"] = 0;

                if ( inRules.ToLower().Equals( "iwwf" )) {
                    newSkierScoreRow["LineLength"] = 18.25M;
                } else {
                    newSkierScoreRow["LineLength"] = 23M;
                }

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode" );
                curSqlStmt.Append(", COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.RankingScore");
                curSqlStmt.Append( ", SS.Round as " + curRoundName + ", SS.Score as " + curScoreName + ", SS.NopsScore as " + curPointsName );
                curSqlStmt.Append( ", MaxSpeed, StartSpeed, StartLen, Status, FinalPassNum, FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff, FinalPassScore " );
                curSqlStmt.Append( "FROM SlalomScore SS " );
                curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
                curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
                curSqlStmt.Append( "WHERE SS.SanctionId = '" + (String)curSkierRow["SanctionId"] + "' AND ER.Event = 'Slalom'" );
                curSqlStmt.Append( "  And SS.MemberId = '" + (String)curSkierRow["Memberid"] + "' " );
                curSqlStmt.Append( "  And ER.AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' " );
                curSqlStmt.Append( "  And SS.Round < 25 " );
                curSqlStmt.Append( "ORDER BY SS.SanctionId, ER.AgeGroup, SS.MemberId, SS.Round" );
                DataTable curSkierResultsAll = getData( curSqlStmt.ToString() );

                if (curSkierResultsAll.Rows.Count > 0) {
                    try {
                        //Retrieve runoff socre
                        newSkierScoreRow["RunoffScore"] = getSlalomRunoffScore( (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"] );
                        
                        //Retrieve tie score details, related final pass score, line length, final pass speed
                        curSkierDetailRow = curSkierResultsAll.Select( "MemberId = '" + (String)curSkierRow["MemberId"] + "' "
                            + "AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' AND " + curRoundName + " = " + newSkierScoreRow["Round"] );
                        try {
                            newSkierScoreRow["RankingScore"] = (Decimal) curSkierDetailRow[0]["RankingScore"];
                        } catch {
                            newSkierScoreRow["RankingScore"] = 0;
                        }
                        try {
                            newSkierScoreRow["Score"] = (Decimal)curSkierDetailRow[0][curScoreName];
                        } catch {
                            newSkierScoreRow["Score"] = 0;
                        }
                        try {
                            newSkierScoreRow["PassScore"] = (Decimal)curSkierDetailRow[0]["FinalPassScore"];
                        } catch {
                            newSkierScoreRow["PassScore"] = 0;
                        }
                        try {
                            Decimal[] curLastPassData = getSlalomLastPass( curSkierDetailRow[0], curSkierRow, curRoundName );
                            newSkierScoreRow["Speed"] = Convert.ToByte( curLastPassData[0] );
                            newSkierScoreRow["LineLength"] = 23M - curLastPassData[1];
                        } catch {
                            newSkierScoreRow["Speed"] = 0;
                            if (inRules.ToLower().Equals( "iwwf" )) {
                                newSkierScoreRow["LineLength"] = 18.25M;
                            } else {
                                newSkierScoreRow["LineLength"] = 23M;
                            }
                        }
                    } catch {
                    }
                } else {
                    //No skier detail so set values to defaults
                }

                if ( inRounds > 1 ) {
                    curSkierDetailRow = curSkierResultsAll.Select("MemberId = '" + (String)curSkierRow["MemberId"] + "'"
                        + " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' AND " + curRoundName + " = 1 " );
                    if (curSkierDetailRow.Length > 0) {
                        newSkierScoreRow["FirstScore"] = (Decimal)curSkierDetailRow[0][curScoreName];
                        if ( inDataType.ToLower().Equals("first") || inDataType.ToLower().Equals("final") ) {
                        } else {
                            if ( (Byte) newSkierScoreRow["Round"] > 1 ) {
                                newSkierScoreRow["BackupScore"] = (Decimal) curSkierDetailRow[0][curScoreName];
                                for ( Byte curSkierRound = 1; curSkierRound <= inRounds; curSkierRound++ ) {
                                    curSkierDetailRow = curSkierResultsAll.Select("MemberId = '" + (String) curSkierRow["MemberId"] + "'"
                                        + " AND AgeGroup = '" + (String) curSkierRow["AgeGroup"] + "' AND " + curRoundName + " = " + curSkierRound.ToString());
                                    if ( curSkierDetailRow.Length > 0 ) {
                                        if ( (Byte) newSkierScoreRow["Round"] != curSkierRound ) {
                                            if ( (Decimal) curSkierDetailRow[0][curScoreName] > (Decimal) newSkierScoreRow["BackupScore"] ) {
                                                newSkierScoreRow["BackupScore"] = (Decimal) curSkierDetailRow[0][curScoreName];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } else {
                        newSkierScoreRow["FirstScore"] = 0;
                        newSkierScoreRow["BackupScore"] = 0;
                    }
                } else {
                    newSkierScoreRow["FirstScore"] = (Decimal)curSkierRow[curScoreName];
                    newSkierScoreRow["BackupScore"] = 0;
                }
                newSkierScoreRow.EndEdit();
            }

            return curSkierScoreList;
        }

        private void setSlalomTieBreakerPlcmt( DataTable inSkierScoreList, String inPlcmtName, String inPlcmt ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * curGroup, curGroupName
             * ******************************************************* */
            String curPlcmt = "";
            int nextPlcmtPos = 0, nextPlctAdj = 0, curPlcmtNum;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] != (String)curCheckScoreRow["MemberId"] ) {
                        if ( (Decimal)curSkierScoreRow["Speed"] == (Decimal)curCheckScoreRow["Speed"] ) {
                            if ( (Decimal)curSkierScoreRow["LineLength"] == (Decimal)curCheckScoreRow["LineLength"] ) {
                                if ( (Decimal)curSkierScoreRow["BackupScore"] == (Decimal)curCheckScoreRow["BackupScore"] ) {
                                    if ( (Decimal)curSkierScoreRow["FirstScore"] == (Decimal)curCheckScoreRow["FirstScore"] ) {
                                        if ( (Decimal)curSkierScoreRow["RunoffScore"] == (Decimal)curCheckScoreRow["RunoffScore"] ) {
                                            nextPlctAdj--;
                                        } else if ( (Decimal)curSkierScoreRow["RunoffScore"] > (Decimal)curCheckScoreRow["RunoffScore"] ) {
                                            nextPlctAdj--;
                                        }
                                    } else if ( (Decimal)curSkierScoreRow["FirstScore"] > (Decimal)curCheckScoreRow["FirstScore"] ) {
                                        nextPlctAdj--;
                                    }
                                } else if ( (Decimal)curSkierScoreRow["BackupScore"] > (Decimal)curCheckScoreRow["BackupScore"] ) {
                                    nextPlctAdj--;
                                }
                            } else if ( (Decimal)curSkierScoreRow["LineLength"] > (Decimal)curCheckScoreRow["LineLength"] ) {
                                nextPlctAdj--;
                            }
                        } else if ( (Decimal)curSkierScoreRow["Speed"] > (Decimal)curCheckScoreRow["Speed"] ) {
                            nextPlctAdj--;
                        }
                    }
                }
                curPlcmtNum = curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1;
                if ( curPlcmt.Contains( "T" ) ) {
                    curSkierScoreRow[inPlcmtName] = curPlcmt;
                } else {
                    curPlcmt = curPlcmtNum.ToString( "##0" );
                    if ( curPlcmt.Length > 3 ) {
                        curSkierScoreRow[inPlcmtName] = curPlcmt.Substring( 0, 3 ) + " ";
                    } else {
                        curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft(3, ' ') + " ";
                    }
                }
            }
        }

        private DataTable getTrickTieBreakerData(DataRow[] inSkierList, int inRounds, String inRules) {
            /* **********************************************************
             * Analyze results and determine skier placment within age division
             * ******************************************************* */
            StringBuilder curSqlStmt = new StringBuilder( "" );
            DataTable curSkierScoreList = buildSkierTrickScoreList();
            DataRowView newSkierScoreRow;
            DataRow[] curSkierDetailRow;
            String curScoreName = "ScoreTrick";
            String curRoundName = "RoundTrick";
            String curPointsName = "PointsTrick";

            foreach ( DataRow curSkierRow in inSkierList ) {
                newSkierScoreRow = curSkierScoreList.DefaultView.AddNew();
                newSkierScoreRow["MemberId"] = (String)curSkierRow["MemberId"];
                newSkierScoreRow["AgeGroup"] = (String)curSkierRow["AgeGroup"];
                try {
                    if ( curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Byte" ) ) {
                        newSkierScoreRow["Round"] = (Byte)curSkierRow[curRoundName];
                    } else if ( curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Int16" ) ) {
                        newSkierScoreRow["Round"] = (Int16)curSkierRow[curRoundName];
                    } else if ( curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Int32" ) ) {
                        newSkierScoreRow["Round"] = (int)curSkierRow[curRoundName];
                    } else {
                        newSkierScoreRow["Round"] = 0;
                    }
                } catch {
                    newSkierScoreRow["Round"] = 0;
                }

                newSkierScoreRow["Score"] = 0;
                newSkierScoreRow["PassScore1"] = 0;
                newSkierScoreRow["PassScore2"] = 0;
                newSkierScoreRow["PassScore3"] = 0;
                newSkierScoreRow["PassScore4"] = 0;
                newSkierScoreRow["RunoffScore"] = 0;
                newSkierScoreRow["RankingScore"] = 0;
                newSkierScoreRow["RunoffScore"] = getTrickRunoffScore( (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"] );

                if (inRules.ToLower().Equals( "iwwf" )) {
                } else {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode" );
                    curSqlStmt.Append(", COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.RankingScore");
                    curSqlStmt.Append( ", SS.Round as " + curRoundName + ", SS.Score as " + curScoreName + ", SS.NopsScore as " + curPointsName );
                    curSqlStmt.Append( ", SS.ScorePass1, SS.ScorePass2 " );
                    curSqlStmt.Append( "FROM TrickScore SS " );
                    curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
                    curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
                    curSqlStmt.Append( "WHERE SS.SanctionId = '" + (String)curSkierRow["SanctionId"] + "' AND ER.Event = 'Trick'" );
                    curSqlStmt.Append( "  And SS.MemberId = '" + (String)curSkierRow["Memberid"] + "' " );
                    curSqlStmt.Append( "  And ER.AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' " );
                    curSqlStmt.Append( "  And SS.Round < 25 " );
                    curSqlStmt.Append( "ORDER BY SS.SanctionId, ER.AgeGroup, SS.MemberId, SS.Round" );
                    DataTable curSkierResultsAll = getData( curSqlStmt.ToString() );

                    if (curSkierResultsAll.Rows.Count > 0) {
                        curSkierDetailRow = curSkierResultsAll.Select( "MemberId = '" + (String)curSkierRow["MemberId"] + "'"
                            + " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' AND " + curRoundName + " = " + newSkierScoreRow["Round"].ToString() );
                        try {
                            newSkierScoreRow["RankingScore"] = (Decimal) curSkierDetailRow[0]["RankingScore"];
                        } catch {
                            newSkierScoreRow["RankingScore"] = 0;
                        }
                        try {
                            newSkierScoreRow["Score"] = (Int16)curSkierDetailRow[0][curScoreName];
                        } catch {
                            newSkierScoreRow["Score"] = 0;
                        }
                        try {
                            newSkierScoreRow["PassScore1"] = (Int16)curSkierDetailRow[0]["ScorePass1"];
                        } catch {
                            newSkierScoreRow["PassScore1"] = 0;
                        }
                        try {
                            if ((Int16)curSkierDetailRow[0]["ScorePass2"] > (Int16)newSkierScoreRow["PassScore1"]) {
                                newSkierScoreRow["PassScore2"] = newSkierScoreRow["PassScore1"];
                                newSkierScoreRow["PassScore1"] = (Int16)curSkierDetailRow[0]["ScorePass2"];
                            } else {
                                newSkierScoreRow["PassScore2"] = (Int16)curSkierDetailRow[0]["ScorePass2"];
                            }
                        } catch {
                            newSkierScoreRow["PassScore2"] = 0;
                        }
                        newSkierScoreRow["PassScore3"] = 0;
                        newSkierScoreRow["PassScore4"] = 0;

                        if (inRounds > 1) {
                            curSkierDetailRow = curSkierResultsAll.Select( "MemberId = '" + (String)curSkierRow["MemberId"] + "'"
                                + " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' AND " + curRoundName + " = 1" );
                            if (curSkierDetailRow.Length > 0) {
                                newSkierScoreRow["FirstScore"] = (Int16)curSkierDetailRow[0][curScoreName];
                                if ((Byte)newSkierScoreRow["Round"] > 1) {
                                    newSkierScoreRow["BackupScore"] = (Int16)curSkierDetailRow[0][curScoreName];
                                }
                            }
                            for (Byte curSkierRound = 1; curSkierRound <= inRounds; curSkierRound++) {
                                curSkierDetailRow = curSkierResultsAll.Select( "MemberId = '" + (String)curSkierRow["MemberId"] + "'"
                                    + " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' AND " + curRoundName + " = " + curSkierRound.ToString() );
                                if (curSkierDetailRow.Length > 0) {
                                    if ((Byte)newSkierScoreRow["Round"] != curSkierRound) {
                                        if ((Int16)curSkierDetailRow[0][curScoreName] > (Int16)newSkierScoreRow["BackupScore"]) {
                                            newSkierScoreRow["BackupScore"] = (Int16)curSkierDetailRow[0][curScoreName];
                                        }
                                        //Save individual pass scores in score order
                                        if ((Int16)curSkierDetailRow[0]["ScorePass1"] > (Int16)newSkierScoreRow["PassScore1"]) {
                                            newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
                                            newSkierScoreRow["PassScore3"] = newSkierScoreRow["PassScore2"];
                                            newSkierScoreRow["PassScore2"] = newSkierScoreRow["PassScore1"];
                                            newSkierScoreRow["PassScore1"] = (Int16)curSkierDetailRow[0]["ScorePass1"];
                                        } else if ((Int16)curSkierDetailRow[0]["ScorePass1"] > (Int16)newSkierScoreRow["PassScore2"]) {
                                            newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
                                            newSkierScoreRow["PassScore3"] = newSkierScoreRow["PassScore2"];
                                            newSkierScoreRow["PassScore2"] = (Int16)curSkierDetailRow[0]["ScorePass1"];
                                        } else if ((Int16)curSkierDetailRow[0]["ScorePass1"] > (Int16)newSkierScoreRow["PassScore3"]) {
                                            newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
                                            newSkierScoreRow["PassScore3"] = (Int16)curSkierDetailRow[0]["ScorePass1"];
                                        } else if ((Int16)curSkierDetailRow[0]["ScorePass1"] > (Int16)newSkierScoreRow["PassScore4"]) {
                                            newSkierScoreRow["PassScore4"] = (Int16)curSkierDetailRow[0]["ScorePass1"];
                                        }

                                        if ((Int16)curSkierDetailRow[0]["ScorePass2"] > (Int16)newSkierScoreRow["PassScore1"]) {
                                            newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
                                            newSkierScoreRow["PassScore3"] = newSkierScoreRow["PassScore2"];
                                            newSkierScoreRow["PassScore2"] = newSkierScoreRow["PassScore1"];
                                            newSkierScoreRow["PassScore1"] = (Int16)curSkierDetailRow[0]["ScorePass2"];
                                        } else if ((Int16)curSkierDetailRow[0]["ScorePass2"] > (Int16)newSkierScoreRow["PassScore2"]) {
                                            newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
                                            newSkierScoreRow["PassScore3"] = newSkierScoreRow["PassScore2"];
                                            newSkierScoreRow["PassScore2"] = (Int16)curSkierDetailRow[0]["ScorePass2"];
                                        } else if ((Int16)curSkierDetailRow[0]["ScorePass2"] > (Int16)newSkierScoreRow["PassScore3"]) {
                                            newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
                                            newSkierScoreRow["PassScore3"] = (Int16)curSkierDetailRow[0]["ScorePass2"];
                                        } else if ((Int16)curSkierDetailRow[0]["ScorePass2"] > (Int16)newSkierScoreRow["PassScore4"]) {
                                            newSkierScoreRow["PassScore4"] = (Int16)curSkierDetailRow[0]["ScorePass2"];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                newSkierScoreRow.EndEdit();
            }

            return curSkierScoreList;
        }

        private void setTrickTieBreakerPlcmt( DataTable inSkierScoreList, String inPlcmtName, String inPlcmt ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * ******************************************************* */
            String curPlcmt = "";
            int nextPlcmtPos = 0, nextPlctAdj = 0;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] != (String)curCheckScoreRow["MemberId"] ) {
                        if ( (Int16)curSkierScoreRow["BackupScore"] == (Int16)curCheckScoreRow["BackupScore"] ) {
                            if ( (Int16)curSkierScoreRow["FirstScore"] == (Int16)curCheckScoreRow["FirstScore"] ) {
                                if ( (Int16)curSkierScoreRow["PassScore1"] == (Int16)curCheckScoreRow["PassScore1"] ) {
                                    if ( (Int16)curSkierScoreRow["PassScore2"] == (Int16)curCheckScoreRow["PassScore2"] ) {
                                        if ( (Int16)curSkierScoreRow["PassScore3"] == (Int16)curCheckScoreRow["PassScore3"] ) {
                                            if ( (Int16)curSkierScoreRow["PassScore4"] == (Int16)curCheckScoreRow["PassScore4"] ) {

                                                if ( (Int16)curSkierScoreRow["RunoffScore"] == (Int16)curCheckScoreRow["RunoffScore"] ) {
                                                    nextPlctAdj--;
                                                } else if ( (Int16)curSkierScoreRow["RunoffScore"] > (Int16)curCheckScoreRow["RunoffScore"] ) {
                                                    nextPlctAdj--;
                                                }
                                            } else if ( (Int16)curSkierScoreRow["PassScore4"] > (Int16)curCheckScoreRow["PassScore4"] ) {
                                                nextPlctAdj--;
                                            }
                                        } else if ( (Int16)curSkierScoreRow["PassScore3"] > (Int16)curCheckScoreRow["PassScore3"] ) {
                                            nextPlctAdj--;
                                        }
                                    } else if ( (Int16)curSkierScoreRow["PassScore2"] > (Int16)curCheckScoreRow["PassScore2"] ) {
                                        nextPlctAdj--;
                                    }
                                } else if ( (Int16)curSkierScoreRow["PassScore1"] > (Int16)curCheckScoreRow["PassScore1"] ) {
                                    nextPlctAdj--;
                                }
                            } else if ( (Int16)curSkierScoreRow["FirstScore"] > (Int16)curCheckScoreRow["FirstScore"] ) {
                                nextPlctAdj--;
                            }
                        } else if ( (Int16)curSkierScoreRow["BackupScore"] > (Int16)curCheckScoreRow["BackupScore"] ) {
                            nextPlctAdj--;
                        }
                    }
                }
                curPlcmt = Convert.ToString( curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1 );
                if ( curPlcmt.Contains( "T" ) ) {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 4, ' ' );
                } else {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                }
            }
        }

        private DataTable getJumpTieBreakerData( DataRow[] inSkierList, int inRounds, String inRules, String inPlcmtName ) {
            /* **********************************************************
             * Analyze results and determine skier placment within age division
             * ******************************************************* */
            StringBuilder curSqlStmt = new StringBuilder( "" );
            DataTable curSkierScoreList = buildSkierJumpScoreList();
            DataRowView newSkierScoreRow;
            DataRow[] curSkierDetailRow;
            Decimal[] curRunoffScores;
            String curScoreName = "ScoreJump";
            String curRoundName = "RoundJump";
            String curPointsName = "PointsJump";

            foreach ( DataRow curSkierRow in inSkierList ) {
                newSkierScoreRow = curSkierScoreList.DefaultView.AddNew();
                newSkierScoreRow["MemberId"] = (String)curSkierRow["MemberId"];
                newSkierScoreRow["AgeGroup"] = (String)curSkierRow["AgeGroup"];
                try {
                    if ( curSkierRow["RoundJump"].GetType() == System.Type.GetType( "System.Byte" ) ) {
                        newSkierScoreRow["Round"] = (Byte)curSkierRow["RoundJump"];
                    } else if (curSkierRow["RoundJump"].GetType() == System.Type.GetType( "System.Int16" )) {
                        newSkierScoreRow["Round"] = (Int16)curSkierRow["RoundJump"];
                    } else if (curSkierRow["RoundJump"].GetType() == System.Type.GetType( "System.Int32" )) {
                        newSkierScoreRow["Round"] = (int)curSkierRow["RoundJump"];
                    } else {
                        newSkierScoreRow["Round"] = 0;
                    }
                } catch {
                    newSkierScoreRow["Round"] = 0;
                }

                newSkierScoreRow["ScoreFeet"] = 0;
                newSkierScoreRow["ScoreMeters"] = 0;
                newSkierScoreRow["RunoffScoreFeet"] = 0;
                newSkierScoreRow["RunoffScoreMeters"] = 0;
                newSkierScoreRow["BackupScoreFeet"] = 0;
                newSkierScoreRow["BackupScoreMeters"] = 0;
                newSkierScoreRow["RankingScore"] = 0;

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, SS.BoatSpeed, SS.RampHeight, ER.TeamCode, '' as BoatCode " );
                curSqlStmt.Append(", ER.AgeGroup,  ER.EventGroup, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.RankingScore");
                curSqlStmt.Append( ", SS.ScoreFeet, SS.ScoreMeters, SS.Round as " + curRoundName + ", SS.NopsScore as " + curPointsName + " " );
                curSqlStmt.Append( "FROM JumpScore AS SS " );
                curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
                curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
                curSqlStmt.Append( "WHERE SS.SanctionId = '" + (String)curSkierRow["SanctionId"] + "' AND ER.Event = 'Jump'" );
                curSqlStmt.Append( "  And SS.MemberId = '" + (String)curSkierRow["Memberid"] + "' " );
                curSqlStmt.Append( "  And ER.AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' " );
                curSqlStmt.Append( "ORDER BY SS.SanctionId, ER.AgeGroup, SS.MemberId, SS.Round" );
                DataTable curSkierResultsAll = getData( curSqlStmt.ToString() );

                if (curSkierResultsAll.Rows.Count > 0) {
                    curSkierDetailRow = curSkierResultsAll.Select( "MemberId = '" + (String)curSkierRow["MemberId"] + "'"
                        + " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' AND " + curRoundName + " = " + newSkierScoreRow["Round"].ToString() );
                    try {
                        newSkierScoreRow["RankingScore"] = (Decimal) curSkierDetailRow[0]["RankingScore"];
                    } catch {
                        newSkierScoreRow["RankingScore"] = 0;
                    }
                    try {
                        if (inRules.ToLower().Equals( "iwwf" )) {
                            newSkierScoreRow["ScoreFeet"] = 0;
                        } else {
                            newSkierScoreRow["ScoreFeet"] = (Decimal)curSkierDetailRow[0]["ScoreFeet"];
                        }
                    } catch {
                        newSkierScoreRow["ScoreFeet"] = 0;
                    }
                    try {
                        newSkierScoreRow["ScoreMeters"] = (Decimal)curSkierDetailRow[0]["ScoreMeters"];
                    } catch {
                        newSkierScoreRow["ScoreMeters"] = 0;
                    }
                    curRunoffScores = getJumpRunoffScore( (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"] );
                    if (inRules.ToLower().Equals( "iwwf" )) {
                        newSkierScoreRow["RunoffScoreFeet"] = 0;
                    } else {
                        newSkierScoreRow["RunoffScoreFeet"] = curRunoffScores[0];
                    }
                    newSkierScoreRow["RunoffScoreMeters"] = curRunoffScores[1];

                    if (inRounds > 1) {
                        if (inRules.ToLower().Equals( "iwwf" )) {
                        } else {
                            foreach (DataRow curRow in curSkierResultsAll.Rows) {
                                if ((Decimal)curRow["ScoreFeet"] > (Decimal)newSkierScoreRow["BackupScoreFeet"]
                                    || ( (Decimal)curRow["ScoreFeet"] == (Decimal)newSkierScoreRow["BackupScoreFeet"]
                                        && (Decimal)curRow["ScoreMeters"] > (Decimal)newSkierScoreRow["BackupScoreMeters"] )) {
                                    newSkierScoreRow["BackupScoreFeet"] = (Decimal)curRow["ScoreFeet"];
                                    newSkierScoreRow["BackupScoreMeters"] = (Decimal)curRow["ScoreMeters"];
                                }
                            }
                        }
                    }
                }

                newSkierScoreRow.EndEdit();
            }

            return curSkierScoreList;
        }

        private void setJumpTieBreakerPlcmt( DataTable inSkierScoreList, String inPlcmtName, String inPlcmt ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * ******************************************************* */
            String curPlcmt = "";
            int nextPlcmtPos = 0, nextPlctAdj = 0;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] != (String)curCheckScoreRow["MemberId"] ) {
                        if ( (Decimal)curSkierScoreRow["ScoreMeters"] == (Decimal)curCheckScoreRow["ScoreMeters"] ) {
                            if ( (Decimal)curSkierScoreRow["BackupScoreFeet"] == (Decimal)curCheckScoreRow["BackupScoreFeet"] ) {
                                if ( (Decimal)curSkierScoreRow["BackupScoreMeters"] == (Decimal)curCheckScoreRow["BackupScoreMeters"] ) {
                                    if ( (Decimal)curSkierScoreRow["RunoffScoreFeet"] == (Decimal)curCheckScoreRow["RunoffScoreFeet"] ) {
                                        if ( (Decimal)curSkierScoreRow["RunoffScoreMeters"] == (Decimal)curCheckScoreRow["RunoffScoreMeters"] ) {
                                            nextPlctAdj--;
                                        } else if ( (Decimal)curSkierScoreRow["RunoffScoreMeters"] > (Decimal)curCheckScoreRow["RunoffScoreMeters"] ) {
                                            nextPlctAdj--;
                                        }
                                    } else if ( (Decimal)curSkierScoreRow["RunoffScoreFeet"] > (Decimal)curCheckScoreRow["RunoffScoreFeet"] ) {
                                        nextPlctAdj--;
                                    }
                                } else if ( (Decimal)curSkierScoreRow["BackupScoreMeters"] > (Decimal)curCheckScoreRow["BackupScoreMeters"] ) {
                                    nextPlctAdj--;
                                }
                            } else if ( (Decimal)curSkierScoreRow["BackupScoreFeet"] > (Decimal)curCheckScoreRow["BackupScoreFeet"] ) {
                                nextPlctAdj--;
                            }
                        } else if ( (Decimal)curSkierScoreRow["ScoreMeters"] > (Decimal)curCheckScoreRow["ScoreMeters"] ) {
                            nextPlctAdj--;
                        }
                    }
                }
                curPlcmt = Convert.ToString( curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1 );
                if ( curPlcmt.Contains( "T" ) ) {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 4, ' ' );
                } else {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                }
            }
        }

        private void setJumpTieBreakerNcwsaPlcmt( DataTable inSkierScoreList, String inPlcmtName, String inPlcmt ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * ******************************************************* */
            String curPlcmt = "";
            int nextPlcmtPos = 0, nextPlctAdj = 0;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] != (String)curCheckScoreRow["MemberId"] ) {
                        if ( (Decimal)curSkierScoreRow["ScoreMeters"] == (Decimal)curCheckScoreRow["ScoreMeters"] ) {
                            nextPlctAdj--;
                        } else if ( (Decimal)curSkierScoreRow["ScoreMeters"] > (Decimal)curCheckScoreRow["ScoreMeters"] ) {
                            nextPlctAdj--;
                        }
                    }
                }
                curPlcmt = Convert.ToString( curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1 );
                if ( curPlcmt.Contains( "T" ) ) {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 4, ' ' );
                } else {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                }
            }
        }

        private DataTable getOverallTieBreakerData( DataRow[] inSkierList, DataTable inResultsAll, int inRounds ) {
            /* **********************************************************
             * Analyze results and determine skier placment within age division
             * ******************************************************* */
            DataTable curSkierScoreList = buildSkierOverallScoreList();
            DataRowView newSkierScoreRow;
            DataRow[] curSkierDataRow;

            foreach ( DataRow curSkierRow in inSkierList ) {
                newSkierScoreRow = curSkierScoreList.DefaultView.AddNew();
                newSkierScoreRow["MemberId"] = (String)curSkierRow["MemberId"];
                newSkierScoreRow["AgeGroup"] = (String)curSkierRow["AgeGroup"];
                newSkierScoreRow["Round"] = (Byte)curSkierRow["Round"];
                curSkierDataRow = inResultsAll.Select(
                    "MemberId = '" + (String)curSkierRow["MemberId"] + "'"
                    + " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "'"
                    + " AND Round = " + ( (Byte)curSkierRow["Round"] ).ToString() );
                if ( curSkierDataRow.Length > 0 ) {
                    newSkierScoreRow["Score"] = (Decimal)curSkierRow["Score"];
                    newSkierScoreRow["RunoffScore"] = getOverallRunoffScore( (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"], inResultsAll );

                    if ( inRounds > 1 ) {
                        curSkierDataRow = inResultsAll.Select(
                            "MemberId = '" + (String)curSkierRow["MemberId"] + "'"
                            + " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "'"
                            + " AND Round = 1" );
                        if ( curSkierDataRow.Length > 0 ) {
                            newSkierScoreRow["FirstScore"] = (Decimal)curSkierDataRow[0]["Score"];
                            if ( (Byte)newSkierScoreRow["Round"] > 1 ) {
                                newSkierScoreRow["BackupScore"] = (Decimal)curSkierDataRow[0]["Score"];
                            }
                            for ( Byte curSkierRound = 1; curSkierRound <= inRounds; curSkierRound++ ) {
                                curSkierDataRow = inResultsAll.Select(
                                    "MemberId = '" + (String)curSkierRow["MemberId"] + "'"
                                    + " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "'"
                                    + " AND Round = " + curSkierRound.ToString() );
                                if ( curSkierDataRow.Length > 0 ) {
                                    if ( (Byte)newSkierScoreRow["Round"] != curSkierRound ) {
                                        if ( (Decimal)curSkierDataRow[0]["Score"] > (Decimal)newSkierScoreRow["BackupScore"] ) {
                                            newSkierScoreRow["BackupScore"] = (Decimal)curSkierDataRow[0]["Score"];
                                        }
                                    }
                                }
                            }
                        }
                    } else {
                        newSkierScoreRow["FirstScore"] = (Decimal)curSkierRow["Score"];
                        newSkierScoreRow["BackupScore"] = 0;
                    }
                } else {
                    MessageBox.Show( "Overall detail not found for skier" );
                }
                newSkierScoreRow.EndEdit();
                /*
                MessageBox.Show( "   MemberId: " + newSkierScoreRow["MemberId"]
                    + "\n    AgeGroup: " + newSkierScoreRow["AgeGroup"]
                    + "\n       Round: " + newSkierScoreRow["Round"]
                    + "\n       Score: " + newSkierScoreRow["Score"]
                    + "\n   PassScore: " + newSkierScoreRow["PassScore"]
                    + "\n  FirstScore: " + newSkierScoreRow["FirstScore"]
                    + "\n BackupScore: " + newSkierScoreRow["BackupScore"]
                    + "\n RunoffScore: " + newSkierScoreRow["RunoffScore"]
                    );
                 */
            }

            return curSkierScoreList;
        }

        private void setOverallTieBreakerPlcmt( DataTable inSkierScoreList, String inPlcmt, String inDiv ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * ******************************************************* */
            int nextPlcmtPos = 0, nextPlctAdj = 0;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] != (String)curCheckScoreRow["MemberId"] ) {
                        if ( (Decimal)curSkierScoreRow["BackupScore"] == (Decimal)curCheckScoreRow["BackupScore"] ) {
                            if ( (Decimal)curSkierScoreRow["FirstScore"] == (Decimal)curCheckScoreRow["FirstScore"] ) {
                                if ( (Decimal)curSkierScoreRow["RunoffScore"] == (Decimal)curCheckScoreRow["RunoffScore"] ) {
                                    nextPlctAdj--;
                                } else if ( (Decimal)curSkierScoreRow["RunoffScore"] > (Decimal)curCheckScoreRow["RunoffScore"] ) {
                                    nextPlctAdj--;
                                }
                            } else if ( (Decimal)curSkierScoreRow["FirstScore"] > (Decimal)curCheckScoreRow["FirstScore"] ) {
                                nextPlctAdj--;
                            }
                        } else if ( (Decimal)curSkierScoreRow["BackupScore"] > (Decimal)curCheckScoreRow["BackupScore"] ) {
                            nextPlctAdj--;
                        }
                    }
                }
                curSkierScoreRow["PlcmtGroup"] = Convert.ToString( curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1 );
            }
        }

        private void setEventFinalPlcmt(DataTable inSkierScoreList, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inPlcmtName, String inPlcmtNameSmry) {
            /* **********************************************************
             * Analyze results of tie breakers and update event skier placments
             * ******************************************************* */
            inSkierScoreList.DefaultView.Sort = inPlcmtNameSmry + " ASC, MemberId ASC";
            DataTable curSkierScoreList = inSkierScoreList.DefaultView.ToTable();
            DataRow prevSkierScoreRow;
            DataRow[] curSkierList;
            int prevIdx = 0, curIdx = 0;
            String curPlcmt = "";
            foreach ( DataRow curSkierScoreRow in curSkierScoreList.Rows ) {
                if ( curIdx > 0 ) {
                    prevSkierScoreRow = curSkierScoreList.Rows[prevIdx];
                    if (curSkierScoreRow[inPlcmtNameSmry].Equals( prevSkierScoreRow[inPlcmtNameSmry] )) {
                        curSkierList = inEventResults.Select( "MemberId = '" + prevSkierScoreRow["MemberId"] + "' AND AgeGroup = '" + prevSkierScoreRow["AgeGroup"] + "'" );
                        if ( curSkierList.Length > 0 ) {
                            curPlcmt = ( (String)prevSkierScoreRow[inPlcmtNameSmry] );
                            if ( curPlcmt.Length > 3 ) {
                                curSkierList[0][inPlcmtName] = curPlcmt.Substring(0, 3) + "T";
                            } else {
                                curSkierList[0][inPlcmtName] = curPlcmt.PadLeft(3, ' ') + "T";
                            }
                        }
                        curSkierList = inEventResults.Select( "MemberId = '" + curSkierScoreRow["MemberId"] + "' AND AgeGroup = '" + curSkierScoreRow["AgeGroup"] + "'" );
                        if ( curSkierList.Length > 0 ) {
                            curPlcmt = ( (String)curSkierScoreRow[inPlcmtNameSmry] );
                            if ( curPlcmt.Length > 3 ) {
                                curSkierList[0][inPlcmtName] = curPlcmt.Substring( 0, 3 ) + "T";
                            } else {
                                curSkierList[0][inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + "T";
                            }
                        }
                    } else {
                        curSkierList = inEventResults.Select( "MemberId = '" + curSkierScoreRow["MemberId"] + "' AND AgeGroup = '" + curSkierScoreRow["AgeGroup"] + "'" );
                        if ( curSkierList.Length > 0 ) {
                            curPlcmt = ( (String)curSkierScoreRow[inPlcmtNameSmry] );
                            if ( curPlcmt.Length > 3 ) {
                                curSkierList[0][inPlcmtName] = curPlcmt.Substring( 0, 3 ) + " ";
                            } else {
                                curSkierList[0][inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                            }
                        }
                        if ( curIdx == 1 ) {
                            curSkierList = inEventResults.Select( "MemberId = '" + prevSkierScoreRow["MemberId"] + "' AND AgeGroup = '" + prevSkierScoreRow["AgeGroup"] + "'" );
                            if ( curSkierList.Length > 0 ) {
                                curPlcmt = ( (String)prevSkierScoreRow[inPlcmtNameSmry] );
                                if ( curPlcmt.Length > 3 ) {
                                    curSkierList[0][inPlcmtName] = curPlcmt.Substring( 0, 3 ) + " ";
                                } else {
                                    curSkierList[0][inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                                }
                            }
                        }
                    }
                }
                prevIdx = curIdx;
                curIdx++;
            }
        }

        private DataTable setInitEventPlcmt(DataTable inResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, String inPlcmtName, String inEvent) {
            /* **********************************************************
             * Analyze results and determine skier placment within age division
             * ******************************************************* */
            DataTable curPlcmtResults = inResults;
            String curPlcmt = "", prevGroup = "", curGroup = "", curSortCmd = "", curRound = "";
            int curIdx = 0, curPlcmtPos = 1;
            Decimal curScore = 0, prevScore = -1;

            if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
                #region Calculate placement for all tournament participants
                if (inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
                    curSortCmd = "Round" + inEvent + " ASC, ";
                } else if (inDataType.ToLower().Equals( "final" )) {
                    curSortCmd = "Round" + inEvent + " DESC, ";
                } else {
                    curSortCmd = "";
                }
                if (inEvent.ToLower().Equals( "jump" )) {
                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        curSortCmd += "ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
                    } else {
                        curSortCmd += "Points" + inEvent + " DESC, SkierName ASC ";
                    }
                } else {
                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        curSortCmd += "Score" + inEvent + " DESC, SkierName ASC ";
                    } else {
                        curSortCmd += "Points" + inEvent + " DESC, SkierName ASC ";
                    }
                }

                curPlcmtResults.DefaultView.Sort = curSortCmd;
                curPlcmtResults = curPlcmtResults.DefaultView.ToTable();
                foreach (DataRow curRow in curPlcmtResults.Rows) {
                    curPlcmt = "";
                    prevGroup = curGroup;
                    prevScore = curScore;

                    try {
                        if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Byte" )) {
                            curRound = ( (Byte)curRow["Round" + inEvent] ).ToString();
                        } else if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int16" )) {
                            curRound = ( (Int16)curRow["Round" + inEvent] ).ToString();
                        } else if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int32" )) {
                            curRound = ( (int)curRow["Round" + inEvent] ).ToString();
                        } else {
                            curRound = "0";
                        }
                    } catch {
                        curRound = "0";
                    }
                    if (inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" )) {
                        curGroup = curRound;
                    } else {
                        curGroup = "";
                    }
                    if (!( curGroup.Equals( prevGroup ) )) {
                        curPlcmtPos = 1;
                        prevScore = -1;
                    }

                    if (Convert.ToInt32( curRound ) > 0) {
                        if (inPlcmtMethod.ToLower().Equals( "score" )) {
                            try {
                                if (inEvent.ToLower().Equals( "jump" )) {
                                    curScore = (Decimal)( curRow["ScoreFeet"] );
                                } else {
                                    if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Decimal" )) {
                                        curScore = (Decimal)( curRow["Score" + inEvent] );
                                    } else if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int16" )) {
                                        curScore = (Int16)( curRow["Score" + inEvent] );
                                    } else if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int32" )) {
                                        curScore = (int)( curRow["Score" + inEvent] );
                                    } else {
                                        curScore = 0;
                                    }
                                }
                            } catch {
                                curScore = 0;
                            }
                        } else {
                            try {
                                curScore = (Decimal)( curRow["Points" + inEvent] );
                            } catch {
                                curScore = 0;
                            }
                        }

                        if ( curScore == prevScore && curIdx > 0 ) {
                            curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1][inPlcmtName];
                            if (curPlcmt.Contains( "T" )) {
                            } else {
                                curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
                                curPlcmtResults.Rows[curIdx - 1][inPlcmtName] = curPlcmt;
                            }
                        } else {
                            curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
                            curPlcmt += " ";
                        }
                        curRow[inPlcmtName] = curPlcmt;
                        curPlcmtPos++;
                    } else {
                        curPlcmt = "";
                        curScore = -1;
                    }
                    curRow[inPlcmtName] = curPlcmt;
                    curIdx++;
                }
                #endregion

            } else {
                #region Calculate placement based on requested organization
                curPlcmtPos = 1;
                curIdx = 0;
                curScore = 0;
                prevScore = -1;
                prevGroup = "";
                curGroup = "";
                if ( inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
                    curSortCmd = "AgeGroup ASC, ";
                } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                    curSortCmd = "AgeGroup ASC, EventGroup ASC, ";
                } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                    curSortCmd = "EventGroup ASC, ";
                } else {
                    curSortCmd = "AgeGroup ASC, ";
                }
                if (inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" )) {
                    curSortCmd += "Round" + inEvent + " ASC, ";
                } else if (inDataType.ToLower().Equals( "final" )) {
                    curSortCmd += "Round" + inEvent + " DESC, ";
                } else if (inDataType.ToLower().Equals( "first" )) {
                    curSortCmd += "Round" + inEvent + " DESC, ";
                } else {
                }
                if (inPlcmtMethod.ToLower().Equals( "score" )) {
                    if ( inEvent.ToLower().Equals("jump") ) {
                        curSortCmd += "ScoreFeet DESC, ScoreMeters, SkierName ASC ";
                    } else {
                        curSortCmd += "Score" + inEvent + " DESC, SkierName ASC ";
                    }
                } else {
                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        curSortCmd += "Score" + inEvent + " DESC, SkierName ASC ";
                    } else {
                        curSortCmd += "Points" + inEvent + " DESC, SkierName ASC ";
                    }
                }
                curPlcmtResults.DefaultView.Sort = curSortCmd;
                curPlcmtResults = curPlcmtResults.DefaultView.ToTable();

                foreach ( DataRow curRow in curPlcmtResults.Rows ) {
                    curPlcmt = "";
                    prevGroup = curGroup;
                    prevScore = curScore;

                    try {
                        if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Byte" )) {
                            curRound = ( (Byte)curRow["Round" + inEvent] ).ToString();
                        } else if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int16" )) {
                            curRound = ( (Int16)curRow["Round" + inEvent] ).ToString();
                        } else if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int32" )) {
                            curRound = ( (int)curRow["Round" + inEvent] ).ToString();
                        } else {
                            curRound = "0";
                        }
                    } catch {
                        curRound = "0";
                    }

                    if (inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "agegroup" )) {
                        curGroup = (String)curRow["AgeGroup"];
                    } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                        curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
                    } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                        curGroup = (String)curRow["EventGroup"];
                    } else {
                        curGroup = (String)curRow["AgeGroup"];
                    }
                    if (inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" )) {
                        curGroup += "-" + curRound; 
                    }
                    if ( !( curGroup.Equals( prevGroup ) ) ) {
                        curPlcmtPos = 1;
                        prevScore = -1;
                    }

                    if (Convert.ToInt32( curRound ) > 0) {
                        if (inPlcmtMethod.ToLower().Equals( "score" )) {
                            try {
                                if (inEvent.ToLower().Equals( "jump" )) {
                                    curScore = (Decimal)( curRow["ScoreFeet"] );
                                } else {
                                    if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Decimal" )) {
                                        curScore = (Decimal)( curRow["Score" + inEvent] );
                                    } else if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int16" )) {
                                        curScore = (Int16)( curRow["Score" + inEvent] );
                                    } else if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int32" )) {
                                        curScore = (int)( curRow["Score" + inEvent] );
                                    } else {
                                        curScore = 0;
                                    }
                                }
                            } catch {
                                curScore = 0;
                            }
                        } else {
                            try {
                                curScore = (Decimal)( curRow["Points" + inEvent] );
                            } catch {
                                curScore = 0;
                            }
                        }

                        if (curScore == prevScore && curIdx > 0) {
                            curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1][inPlcmtName];
                            if (curPlcmt.Contains( "T" )) {
                            } else {
                                curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
                                curPlcmtResults.Rows[curIdx - 1][inPlcmtName] = curPlcmt;
                            }
                        } else {
                            curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
                            curPlcmt += " ";
                        }
                        curPlcmtPos++;
                    } else {
                        curPlcmt = "";
                        curScore = -1;
                    }
                    curRow[inPlcmtName] = curPlcmt;
                    curIdx++;

                }
                #endregion
            }

            return curPlcmtResults;
        }

        private DataTable setInitEventPlcmtIwwf(DataTable inResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, String inEvent) {
            /* **********************************************************
             * Analyze results and determine skier placment within age division
             * ******************************************************* */
            DataTable curPlcmtResults = inResults;
            String curPlcmt = "", prevGroup = "", curGroup = "", curSortCmd = "", curRound = "";
            int curIdx = 0, curPlcmtPos = 1, curRoundInt = 0, prevRoundInt = 0;
            Decimal curScore = 0, prevScore = 0;

            if (inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
                #region Calculate placement for all tournament participants
                if (inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" )) {
                    curSortCmd += "Round" + inEvent + " ASC, ";
                } else if (inDataType.ToLower().Equals( "final" )) {
                    curSortCmd += "Round" + inEvent + " DESC, ";
                } else {
                    curSortCmd = "";
                }
                if (inEvent.Equals( "Jump" )) {
                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        curSortCmd += "ScoreMeters DESC, SkierName ASC ";
                    } else {
                        curSortCmd += "Points" + inEvent + " DESC, SkierName ASC ";
                    }
                } else {
                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        curSortCmd += "Score" + inEvent + " DESC, SkierName ASC ";
                    } else {
                        if (( inEvent.Equals( "Overall" ) || inEvent.Equals( "Scoreboard" ) )) {
                            curSortCmd += "Score" + inEvent + " DESC, SkierName ASC ";
                        } else {
                            curSortCmd += "Points" + inEvent + " DESC, SkierName ASC ";
                        }
                    }
                }
                curPlcmtResults.DefaultView.Sort = curSortCmd;
                curPlcmtResults = curPlcmtResults.DefaultView.ToTable();
                foreach (DataRow curRow in curPlcmtResults.Rows) {
                    curPlcmt = "";
                    prevGroup = curGroup;
                    prevScore = curScore;

                    if (inDataType.ToLower().Equals( "round" )) {
                        try {
                            if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Byte" )) {
                                curRound = ( (Byte)curRow["Round" + inEvent] ).ToString();
                            } else if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int16" )) {
                                curRound = ( (Int16)curRow["Round" + inEvent] ).ToString();
                            } else if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int32" )) {
                                curRound = ( (int)curRow["Round" + inEvent] ).ToString();
                            } else {
                                curRound = "1";
                            }
                        } catch {
                            curRound = "1";
                        }
                        curGroup = curRound;
                    } else {
                        curGroup = "";
                    }
                    if (!( curGroup.Equals( prevGroup ) )) {
                        curPlcmtPos = 1;
                        prevScore = -1;
                    }

                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        try {
                            if (inEvent.Equals( "Jump" )) {
                                curScore = (Decimal)( curRow["ScoreMeters"] );
                            } else {
                                if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Decimal" )) {
                                    curScore = (Decimal)( curRow["Score" + inEvent] );
                                } else if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int16" )) {
                                    curScore = (Int16)( curRow["Score" + inEvent] );
                                } else if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int32" )) {
                                    curScore = (int)( curRow["Score" + inEvent] );
                                } else {
                                    curScore = 0;
                                }
                            }
                        } catch {
                            curScore = 0;
                        }
                    } else {
                        try {
                            curScore = (Decimal)( curRow["Points" + inEvent] );
                        } catch {
                            curScore = 0;
                        }
                    }

                    if (( inEvent.Equals( "Overall" ) || inEvent.Equals( "Scoreboard" ) ) ) {
                        if ( ((String)( curRow["QualifyOverall"] ) ).ToLower().Equals( "yes" ) ) {
                            if (curScore == prevScore && curIdx > 0) {
                                curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent];
                                if (curPlcmt.Contains( "T" )) {
                                } else {
                                    curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
                                    curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent] = curPlcmt;
                                }
                            } else {
                                curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
                                curPlcmt += " ";
                            }
                            curRow["Plcmt" + inEvent] = curPlcmt;
                            curPlcmtPos++;
                        } else {
                            curRow["Plcmt" + inEvent] = "";
                        }
                    } else {
                        if (curScore == prevScore && curIdx > 0) {
                            curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent];
                            if (curPlcmt.Contains( "T" )) {
                            } else {
                                curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
                                curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent] = curPlcmt;
                            }
                        } else {
                            curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
                            curPlcmt += " ";
                        }
                        curRow["Plcmt" + inEvent] = curPlcmt;
                        curPlcmtPos++;
                    }
                    curIdx++;
                }
                #endregion

            } else {
                #region Calculate placement based on requested organization
                curPlcmtPos = 1;
                curIdx = 0;
                curScore = 0;
                prevScore = 0;
                prevGroup = "";
                curGroup = "";
                if (inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "agegroup" )) {
                    curSortCmd = "AgeGroup ASC, ";
                } else if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                    curSortCmd = "AgeGroup ASC, EventGroup ASC, ";
                } else if (inPlcmtOrg.ToLower().Equals( "group" )) {
                    curSortCmd = "EventGroup ASC, ";
                } else {
                    curSortCmd = "AgeGroup ASC, ";
                }
                if (inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" )) {
                    curSortCmd += "Round" + inEvent + " ASC, ";
                } else if (inDataType.ToLower().Equals( "final" )) {
                    curSortCmd += "Round" + inEvent + " DESC, ";
                }
                if (inEvent.Equals( "Jump" )) {
                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        curSortCmd += "ScoreMeters DESC, SkierName ASC ";
                    } else {
                        curSortCmd += "Points" + inEvent + " DESC, SkierName ASC ";
                    }
                } else {
                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        curSortCmd += "Score" + inEvent + " DESC, SkierName ASC ";
                    } else {
                        if (( inEvent.Equals( "Overall" ) || inEvent.Equals( "Scoreboard" ) )) {
                            curSortCmd += "Score" + inEvent + " DESC, SkierName ASC ";
                        } else {
                            curSortCmd += "Points" + inEvent + " DESC, SkierName ASC ";
                        }
                    }
                }
                curPlcmtResults.DefaultView.Sort = curSortCmd;
                curPlcmtResults = curPlcmtResults.DefaultView.ToTable();

                foreach (DataRow curRow in curPlcmtResults.Rows) {
                    curPlcmt = "";
                    prevGroup = curGroup;
                    prevScore = curScore;
                    prevRoundInt = curRoundInt;

                    if (inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "agegroup" )) {
                        curGroup = (String)curRow["AgeGroup"];
                    } else if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                        curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
                    } else if (inPlcmtOrg.ToLower().Equals( "group" )) {
                        curGroup = (String)curRow["EventGroup"];
                    } else {
                        curGroup = (String)curRow["AgeGroup"];
                    }
                    try {
                        if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Byte" )) {
                            curRound = ( (Byte)curRow["Round" + inEvent] ).ToString();
                        } else if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int16" )) {
                            curRound = ( (Int16)curRow["Round" + inEvent] ).ToString();
                        } else if (curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int32" )) {
                            curRound = ( (int)curRow["Round" + inEvent] ).ToString();
                        } else {
                            curRound = "1";
                        }
                    } catch {
                        curRound = "1";
                    }
                    curRoundInt = Convert.ToInt32( curRound );
                    if (inDataType.ToLower().Equals( "round" )) {
                        curGroup += "-" + curRound;
                    }
                    if (!( curGroup.Equals( prevGroup ) )) {
                        curPlcmtPos = 1;
                        prevScore = -1;
                    }

                    if (inPlcmtMethod.ToLower().Equals( "score" )) {
                        try {
                            if (inEvent.Equals( "Jump" )) {
                                curScore = (Decimal)( curRow["ScoreMeters"] );
                            } else {
                                if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Decimal" )) {
                                    curScore = (Decimal)( curRow["Score" + inEvent] );
                                } else if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int16" )) {
                                    curScore = (Int16)( curRow["Score" + inEvent] );
                                } else if (curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int32" )) {
                                    curScore = (int)( curRow["Score" + inEvent] );
                                } else {
                                    curScore = 0;
                                }
                            }
                        } catch {
                            curScore = 0;
                        }
                    } else {
                        try {
                            curScore = (Decimal)( curRow["Points" + inEvent] );
                        } catch {
                            curScore = 0;
                        }
                    }

                    if (( inEvent.Equals( "Overall" ) || inEvent.Equals( "Scoreboard" ) ) ) {
                        if ( ((String)( curRow["QualifyOverall"] ) ).ToLower().Equals( "yes" ) ) {
                            if (curScore == prevScore && curRoundInt == prevRoundInt && curIdx > 0) {
                                curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent];
                                if (curPlcmt.Length > 0) {
                                    if (curPlcmt.Contains( "T" )) {
                                    } else {
                                        curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
                                        curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent] = curPlcmt;
                                    }
                                } else {
                                    curPlcmt = "   ";
                                }
                            } else {
                                curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
                                curPlcmt += " ";
                            }
                            curRow["Plcmt" + inEvent] = curPlcmt;
                            curPlcmtPos++;
                        } else {
                            curRow["Plcmt" + inEvent] = "";
                        }
                    } else {
                        if (curScore == prevScore && curRoundInt == prevRoundInt && curIdx > 0) {
                            curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent];
                            if (curPlcmt.Length > 0) {
                                if (curPlcmt.Contains( "T" )) {
                                } else {
                                    curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
                                    curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent] = curPlcmt;
                                }
                            } else {
                                curPlcmt = "   ";
                            }
                        } else {
                            curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
                            curPlcmt += " ";
                        }
                        curRow["Plcmt" + inEvent] = curPlcmt;
                        curPlcmtPos++;
                    }
                    curIdx++;
                }
                #endregion
            }

            return curPlcmtResults;
        }

        private DataTable findEventPlcmtTiesGroup(String inPlcmtName, String inPlcmtOrg, DataTable inResults) {
            /* **********************************************************
             * Analyze event results to identify ties
             * ******************************************************* */
            String curPlcmt = "", curDiv = "", curGroup = "", curPlcmtValue = "", prevPlcmtValue = "";
            DataRowView newPlcmtRow;
            DataTable curPlcmtList = new DataTable();

            #region Build placement table to hold and calculate placements
            DataColumn curCol = new DataColumn();
            curCol.ColumnName = inPlcmtName;
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curPlcmtList.Columns.Add( curCol );

            if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
                curCol = new DataColumn();
                curCol.ColumnName = "AgeGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );
            } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                curCol = new DataColumn();
                curCol.ColumnName = "AgeGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );

                curCol = new DataColumn();
                curCol.ColumnName = "EventGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );
            } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                curCol = new DataColumn();
                curCol.ColumnName = "EventGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );
            } else {
                curCol = new DataColumn();
                curCol.ColumnName = "AgeGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );
            }
            #endregion

            #region Find ties in initial placement determinations
            foreach ( DataRow curRow in inResults.Rows ) {
                curPlcmt = curRow[inPlcmtName].ToString();
                curPlcmtValue = "";
                if ( curPlcmt.Contains( "T" ) ) {
                    if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
                        curDiv = (String)curRow["AgeGroup"];
                        curPlcmtValue = curDiv + "-" + curPlcmt;
                    } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                        curDiv = (String)curRow["AgeGroup"];
                        curGroup = (String)curRow["EventGroup"];
                        curPlcmtValue = curDiv + "-" + curGroup + "-" + curPlcmt;
                    } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                        curGroup = (String)curRow["EventGroup"];
                        curPlcmtValue = curGroup + "-" + curPlcmt;
                    } else {
                        curDiv = (String)curRow["AgeGroup"];
                        curPlcmtValue = curDiv + "-" + curPlcmt;
                    }

                    if ( !( curPlcmtValue.Equals( prevPlcmtValue ) ) ) {
                        newPlcmtRow = curPlcmtList.DefaultView.AddNew();
                        if ( curDiv.Length > 0 ) {
                            newPlcmtRow["AgeGroup"] = curDiv;
                        }
                        if ( curGroup.Length > 0 ) {
                            newPlcmtRow["EventGroup"] = curGroup;
                        }
                        newPlcmtRow[inPlcmtName] = curPlcmt;
                        newPlcmtRow.EndEdit();
                    }
                    prevPlcmtValue = curPlcmtValue;
                } else {
                    prevPlcmtValue = "";
                }
            }
            #endregion

            return curPlcmtList;
        }

        private DataTable findEventPlcmtTiesTour( String inPlcmtName, DataTable inResults ) {
            /* **********************************************************
             * Analyze event results to identify ties
             * ******************************************************* */
            String curPlcmt = "", curPlcmtValue = "", prevPlcmtValue = "";
            DataRowView newPlcmtRow;
            DataTable curPlcmtList = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = inPlcmtName;
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curPlcmtList.Columns.Add( curCol );

            foreach ( DataRow curRow in inResults.Rows ) {
                curPlcmt = curRow[inPlcmtName].ToString();
                curPlcmtValue = "";
                if ( curPlcmt.Contains( "T" ) ) {
                    curPlcmtValue = curPlcmt;
                    if ( !( curPlcmtValue.Equals( prevPlcmtValue ) ) ) {
                        newPlcmtRow = curPlcmtList.DefaultView.AddNew();
                        newPlcmtRow[inPlcmtName] = curPlcmt;
                        newPlcmtRow.EndEdit();
                    }
                    prevPlcmtValue = curPlcmtValue;
                } else {
                    prevPlcmtValue = "";
                }
            }

            return curPlcmtList;
        }

        private DataTable buildSkierSlalomScoreList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtSlalom";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Speed";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "LineLength";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "FirstScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RankingScore";
            curCol.DataType = System.Type.GetType("System.Decimal");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            return curDataTable;
        }

        private DataTable buildSkierTrickScoreList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtTrick";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScore";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "FirstScore";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScore";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore1";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore2";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore3";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore4";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RankingScore";
            curCol.DataType = System.Type.GetType("System.Decimal");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            return curDataTable;
        }

        private DataTable buildSkierJumpScoreList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtJump";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "ScoreFeet";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "ScoreMeters";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScoreFeet";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScoreMeters";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScoreFeet";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScoreMeters";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RankingScore";
            curCol.DataType = System.Type.GetType("System.Decimal");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            return curDataTable;
        }

        private DataTable buildSkierOverallScoreList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtTour";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "FirstScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            return curDataTable;
        }

        private Decimal getSlalomRunoffScore( String inMemberId, String inAgeGroup ) {
            Decimal curReturnScore = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SS.Score FROM SlalomScore SS " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
            curSqlStmt.Append( "WHERE SS.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Slalom' AND SS.Round >= 25 " );
            curSqlStmt.Append( "AND SS.MemberId = '" + inMemberId + "' AND SS.AgeGroup = '" + inAgeGroup + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            try {
                if (curDataTable.Rows.Count > 0) {
                    curReturnScore = (Decimal)curDataTable.Rows[0]["Score"];
                } else {
                    curReturnScore = 0;
                }
            } catch {
                curReturnScore = 0;
            }

            return curReturnScore;
        }

        private Decimal[] getSlalomLastPass(DataRow curSkierDetailRow, DataRow curSkierRow, String curRoundName) {
            Decimal[] curReturnData = { 0M, 0M };
            Byte curMaxSpeed = 0, curStartSpeed = 0;
            Decimal curStartLen = 0M;

            String curMemberId = (String)curSkierRow["MemberId"];
            String curAgeGroup = (String)curSkierRow["AgeGroup"];
            String curRound = "1";
            try {
                if (curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Byte" )) {
                    curRound = ( (Byte)curSkierRow[curRoundName] ).ToString();
                } else if (curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Int16" )) {
                    curRound = ( (Int16)curSkierRow[curRoundName] ).ToString();
                } else if (curSkierRow[curRoundName].GetType() == System.Type.GetType( "System.Int32" )) {
                    curRound = ( (int)curSkierRow[curRoundName] ).ToString();
                } else {
                    curRound = "1";
                }
            } catch {
                curRound = "1";
            }
            curMaxSpeed = (Byte)curSkierDetailRow["MaxSpeed"];
            curStartSpeed = (Byte)curSkierDetailRow["StartSpeed"];
            curStartLen = Convert.ToDecimal( ( (String)curSkierDetailRow["StartLen"] ).Substring( 0, ( (String)curSkierDetailRow["StartLen"] ).Length - 1 ) );

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select MemberId, AgeGroup, Round, PassNum, SkierRunNum, Score, CodeValue, MinValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM SlalomRecap" );
            curSqlStmt.Append( "  INNER JOIN CodeValueList ON ListName = 'SlalomPass" + curMaxSpeed.ToString() + "' AND ListCodeNum = PassNum " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' " );
            curSqlStmt.Append( "  AND AgeGroup = '" + curAgeGroup + "' AND Round = " + curRound + " " );
            curSqlStmt.Append( "ORDER BY MemberId, Round, PassNum" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            try {
                if (curDataTable.Rows.Count > 0) {
                    int curIdx = curDataTable.Rows.Count - 1;
                    Decimal curReturnScore = (Decimal)curDataTable.Rows[curIdx]["Score"];
                    Decimal curReturnSpeed = (Decimal)curDataTable.Rows[curIdx]["MaxValue"];
                    Decimal curReturnRopeLen = (Decimal)curDataTable.Rows[curIdx]["MinValue"];
                    if (curReturnRopeLen > curStartLen) {
                        curReturnRopeLen = curStartLen;
                    }
                    curReturnData[0] = curReturnSpeed;
                    curReturnData[1] = curReturnRopeLen;
                }
            } catch {
            }

            return curReturnData;
        }

        private Int16 getTrickRunoffScore( String inMemberId, String inAgeGroup ) {
            Int16 curReturnScore = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SS.Score FROM TrickScore SS " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
            curSqlStmt.Append( "WHERE SS.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Trick' AND SS.Round >= 25 " );
            curSqlStmt.Append( "AND SS.MemberId = '" + inMemberId + "' AND SS.AgeGroup = '" + inAgeGroup + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            try {
                if ( curDataTable.Rows.Count > 0 ) {
                    curReturnScore = (Int16)curDataTable.Rows[0]["Score"];
                }
            } catch {
                curReturnScore = 0;
            }

            return curReturnScore;
        }

        private Decimal[] getJumpRunoffScore( String inMemberId, String inAgeGroup ) {
            Decimal[] curReturnScore = new Decimal[] {0, 0};
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SS.ScoreFeet, SS.ScoreMeters FROM JumpScore SS " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
            curSqlStmt.Append( "WHERE SS.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Jump' AND SS.Round >= 25 " );
            curSqlStmt.Append( "AND SS.MemberId = '" + inMemberId + "' AND SS.AgeGroup = '" + inAgeGroup + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            try {
                if ( curDataTable.Rows.Count > 0 ) {
                    curReturnScore[0] = (Decimal)curDataTable.Rows[0]["ScoreFeet"];
                    curReturnScore[1] = (Decimal)curDataTable.Rows[0]["ScoreMeters"];
                }
            } catch {
                curReturnScore = new Decimal[] { 0, 0 };
            }
            return curReturnScore;
        }

        private Decimal getOverallRunoffScore( String inMemberId, String inAgeGroup, DataTable inResultsAll ) {
            Decimal curReturnScore = 0;
            DataRow[] curFindRows = inResultsAll.Select(
                "MemberId = '" + inMemberId + "'"
                    + " AND AgeGroup = '" + inAgeGroup + "'"
                    + " AND Round = 25 "
                );
            if ( curFindRows.Length > 0 ) {
                curReturnScore = (Decimal)curFindRows[0]["Score"];
            }
            return curReturnScore;
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }
}
