using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Slalom {
    class SlalomBoatPathAnalysis {

        /*
		 * --------------------------------
		 * 2024 Reride narrative
		 * If the Buoy Deviation or cumulative deviation is NEGATIVE (path away from the skier) and is greater than 25cm
		 * The skier is entitled to an optional re-ride. The skier can improve. 
		 * The maximum score not out of tolerance to the positive is protected. 
		 * 
		 * If the Buoy Deviation or cumulative deviation is POSITIVE (path towards the skier) and is greater than 25cm: 
		 * The skier has the following options: 
		 * • Accept the score that was achieved within tolerance. 
		 * • Take a re-ride. The skier can improve.  
		 * However, for a score of less than 6, if the deviation occurred at the last buoy the skier scored, 
		 * the skier cannot improve over that score.  
		 * The original score is not protected. 
		 * • For a completed pass, “Continue at Risk” as outlined below. 
		 * 
         * 
         * Boat Path Measurement System rule per IWWF World Rules 2025
         * Rule 8.15: Boat Path/End Course Video
         * A Boat Path Measurement System, (BPMS) is required for:
         *	Record Capability(R), Rankings Lists(L), and Pro events
         *		All passes shall be monitored and applicable buoy and cumulative deviation tolerances with re-ride situations applied at 11.25 and shorter.
         *	Titled Events (World or Confederation)
         *		All passes shall be monitored and applicable buoy and cumulative deviation tolerances with re-ride situations applied at 14.25 and shorter.
         *		Note: currently this system is not used for titled events so deviations are only mandatory for 11.215
		 * 
		 * Reride scenarios for buoy deviation
		 *	Pass not completed
		 *		If the Buoy Deviation is NEGATIVE (path away from the skier) and is greater than 25cm:
		 *			• The skier is entitled to an optional re-ride
		 *			• The skier can improve
		 *			• The maximum score not out of tolerance to the positive is protected.
		 *			
		 *		If the Buoy Deviation is POSITIVE (path towards the skier) and is greater than 25cm
		 *			The skier has the following options:
		 *				• Accept the score that was achieved within tolerance.
		 *				• Take a re-ride. The skier cannot improve over the original score. The score that was achieved within tolerance is protected.
		 *
		 *	Pass completed
		 *		If the Buoy Deviation is NEGATIVE (path away from the skier) and is greater than 25cm:
		 *			• The skier is entitled to an optional re-ride.
		 *			• The skier can improve
		 *			• The maximum score not out of tolerance to the positive is protected.
		 *
		 *		If the Buoy Deviation is POSITIVE (path towards the skier) and is greater than 25cm:
		 *			The skier has the following options:
		 *				• Accept the score that was achieved within tolerance.
		 *				• Take a re-ride. The skier can improve over the original score. The score that was achieved within tolerance is protected.
		 *				• Continuing at risk (see below)
		 * Continuing at Risk
		 *  If a skier decides to continue at risk to the next pass and:
		 *      a) Does not complete the pass, the score awarded will be the higher of:
		 *          • The score from the original pass that was in tolerance; or
		 *          • The score from the ‘continue at risk’ pass as though it were at the original speed and rope length.
		 *      b) Completes the pass, and that pass is out of tolerance for boat path deviation, 
		 *          the skier shall be entitled to no more than two additional mandatory re-rides for that specific pass. 
		 *          The skier does not have an option to continue at risk to the next pass
		 *          
         * Cumulative Deviation
         *  a) Pass not completed
         *      If the Cumulative Deviation is NEGATIVE out of tolerance (away from the skier)
         *          • The skier is entitled to an optional re-ride
         *          • The skier can improve
         *          • The maximum score not out of tolerance to the positive is protected
         *      If the Cumulative Deviation is POSITIVE out of tolerance (toward the skier), the skier has the following options:
         *          • Accept the score that was achieved within tolerance
         *          • Take a re-ride. The skier cannot improve over the original score. The score that was achieved within tolerance is protected.
         *  b) Pass completed
         *      If the Cumulative Deviation is NEGATIVE out of tolerance (away from the skier)
         *          • The skier is entitled to an optional re-ride
         *          • The skier can improve
         *          • The maximum score not out of tolerance to the positive is protected.
         *      If the Cumulative Deviation is POSITIVE out of tolerance (toward the skier), the skier has the following options:
         *          • The skier can accept the score that was achieved within tolerance
         *          • The skier can take a re-ride. The skier can improve over the original score. The score that was achieved within tolerance is protected
         *  Continuing at Risk
         *      If a skier decides to continue at risk to the next pass and:
         *          a) Does not complete the pass, the score awarded will be the higher of:
         *              • The score from the original pass that was in tolerance; or
         *              • The score from the ‘continue at risk’ pass as though it were at the original speed and rope length.
         *          b) Completes the pass, and that pass is out of tolerance for boat path deviation, 
         *              the skier shall be entitled to no more than two additional re-rides for that specific pass. 
         *              The skier does not have an option to continue at risk to the next pass.
         *              
		 * NOTE:
		 *      There shall be no more than two re-rides for positive boat path deviation in a single pass
		 *      If during the second re-ride the boat deviation would require another re-ride for positive boat path deviation, 
		 *      then the score awarded will be the highest score achieved in tolerance from the original pass or from either re-ride. 
		 *      The driver shall be warned that the driving is not acceptable, and that he may be replaced.
		 */
        private DataGridViewRow myRecapRow;
        private DataRow myBoatPathDataRow;
        private DataGridView myBoatPathDataGridView;
        private SlalomSetAnalysis mySlalomSetAnalysis;

        public String SkierPassMsg { get; set; } = "";
        public bool CalcScoreReqd { get; set; } = false;

        public SlalomBoatPathAnalysis( SlalomSetAnalysis inSlalomSetAnalysis, DataGridView inBoatPathDataGridView, DataGridViewRow inRecapRow, DataRow inBoatPathDataRow ) {
            mySlalomSetAnalysis = inSlalomSetAnalysis;
            myBoatPathDataGridView = inBoatPathDataGridView;
            myRecapRow = inRecapRow;
            myBoatPathDataRow = inBoatPathDataRow;
        }

        public decimal checkBoatPathReride( String curSkierClass, Decimal curPassScore ) {
            Int16 curViewIdx = 0, curRerideFlag = 0, curRerideOptional = 0, curRerideMandatory = 0;
            Decimal curPassScoreOrig = curPassScore;

            decimal curMinRopeLength = 14.25M;
            DataRow curMinRopeLengthRow = SlalomEventData.getSlalomBoatPathRerideRopeMin( curSkierClass );
            if ( curMinRopeLengthRow == null ) return curPassScore;
            curMinRopeLength = HelperFunctions.getDataRowColValueDecimal( curMinRopeLengthRow, "MinRopeLength", 14.25M );

            foreach ( DataGridViewRow curViewRow in myBoatPathDataGridView.Rows ) {
                curRerideFlag = Convert.ToInt16( HelperFunctions.getViewRowColValue( curViewRow, "boatPathRerideFlag", "0" ) );
                if ( curRerideFlag < 0 && curRerideOptional == 0 ) curRerideOptional = curViewIdx;
                if ( curRerideFlag > 0 && curRerideMandatory == 0 ) curRerideMandatory = curViewIdx;
                curViewIdx++;
            }

            if ( curRerideOptional == 0 && curRerideMandatory == 0 ) return curPassScore;

            if ( curRerideMandatory > 0 ) {
                // Mandatory reride indicated based on boat path data
                decimal curPassLineLength = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
                if ( curPassLineLength <= curMinRopeLength && (Decimal)mySlalomSetAnalysis.ClassRowSkier["ListCodeNum"] > (Decimal)SlalomEventData.myClassCRow["ListCodeNum"] ) {
                    myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                    myRecapRow.Cells["BoatPathGoodRecap"].Value = "N";

                    String curProtectMsg = "Cannot improve score";
                    Decimal curScoreProtected = curRerideMandatory - 1;
                    if ( curScoreProtected < curPassScore ) {
                        curPassScore = curScoreProtected;
                        curProtectMsg = String.Format( "score {0} protected ", curScoreProtected );
                        myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                        myRecapRow.Cells["ProtectedScoreRecap"].Value = curScoreProtected.ToString( "#.00" );
                    }
                    if ( curPassScoreOrig < 6 ) curProtectMsg += String.Format( ", can't improve score {0}", curPassScoreOrig );
                    SkierPassMsg = String.Format( "Mandatory reride based on boat path deviation at buoy {0}, {1}"
                        , curRerideMandatory, curProtectMsg );
                    myRecapRow.Cells["RerideReasonRecap"].Value = SkierPassMsg;

                    CalcScoreReqd = true;
                    return curPassScore;
                }
            }

            if ( curPassScore == 6 ) return curPassScore;
            if ( curRerideOptional > 0 ) {
                // Mandatory reride indicated based on boat path data
                SkierPassMsg = String.Format( "Optional reride based on boat path deviation at buoy {0}" +
                    ", score {1} protected, can improve "
                    , curRerideOptional, curPassScore );
                myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                myRecapRow.Cells["ProtectedScoreRecap"].Value = curPassScore;
                myRecapRow.Cells["RerideRecap"].Value = "Y";
                myRecapRow.Cells["RerideReasonRecap"].Value = SkierPassMsg;
            }
            return curPassScore;
        }

        /*
		 * Retrieve data for current tournament
		 * Used for initial load and to refresh data after updates
		 */
        public bool loadBoatPathDataGridRow( DataGridViewRow curViewRow, int curViewIdx, String curEvent, String curSkierClass
            , String curMemberId, String curRound, String curPassNum, Decimal curPassScore ) {
            Font curFontBold = new Font( "Arial Narrow", 9, FontStyle.Bold );
            Font curFont = new Font( "Arial Narrow", 9, FontStyle.Regular );

            bool curReturnValue = true;
            int[] curTolCheckResults = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            Decimal curBuoyDevTol = 0;
            Decimal curCumDevTol = 0;

            curViewRow.Cells["boatPathScoreRange"].Style.Font = curFont;
            curViewRow.Cells["boatTimeBuoy"].Style.Font = curFont;
            curViewRow.Cells["boatTimeBuoy"].Style.ForeColor = Color.DarkGreen;
            curViewRow.Cells["boatPathBuoyDev"].Style.Font = curFont;
            curViewRow.Cells["boatPathBuoyDev"].Style.ForeColor = Color.DarkGreen;
            curViewRow.Cells["boatPathZoneDev"].Style.Font = curFont;
            curViewRow.Cells["boatPathZoneDev"].Style.ForeColor = Color.DarkGreen;
            curViewRow.Cells["boatPathCumDev"].Style.Font = curFont;
            curViewRow.Cells["boatPathCumDev"].Style.ForeColor = Color.DarkGreen;
            curViewRow.Cells["boatPathBuoyTol"].Style.Font = curFont;
            curViewRow.Cells["boatPathBuoyTol"].Style.ForeColor = Color.DarkGray;
            curViewRow.Cells["boatPathCumTol"].Style.Font = curFont;
            curViewRow.Cells["boatPathCumTol"].Style.ForeColor = Color.DarkGray;

            DataRow curBoatPathDevMaxRow = SlalomEventData.getBoatPathDevMaxRow( curViewIdx, curSkierClass );
            if ( curBoatPathDevMaxRow != null ) {
                curBuoyDevTol = (Decimal)curBoatPathDevMaxRow["BuoyDev"];
                curCumDevTol = (Decimal)curBoatPathDevMaxRow["CumDev"];
            }

            Decimal curPathBuoyDev = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "PathDevBuoy" + curViewIdx, 0 );
            Decimal curPathZoneDev = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "PathDevZone" + curViewIdx, 0 );
            Decimal curPathCumDev = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "PathDevCum" + curViewIdx, 0 );

            if ( curViewIdx == 0 ) {
                curViewRow.Cells["boatPathPos"].Value = (String)curBoatPathDevMaxRow["Buoy"];
                curViewRow.Cells["boatPathScoreRange"].Value = "";
                curViewRow.Cells["boatPathBuoyTol"].Value = curBuoyDevTol.ToString( "##0" );
                curViewRow.Cells["boatPathCumTol"].Value = curCumDevTol.ToString( "##0" );
                curViewRow.Cells["boatPathZoneDev"].Value = curPathZoneDev.ToString( "##0" );
                curViewRow.Cells["boatPathBuoyDev"].Value = curPathBuoyDev.ToString( "##0" );

                if ( (Decimal)SlalomEventData.myClassRowTour["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"]
                    && curBuoyDevTol > 0
                    && Math.Abs( curPathBuoyDev ) > curBuoyDevTol
                    ) {
                    curReturnValue = false;
                    curViewRow.Cells["boatPathBuoyDev"].Style.Font = curFontBold;
                    if ( curPathBuoyDev > 0 ) {
                        curTolCheckResults[curViewIdx] = 1;
                        curViewRow.Cells["boatPathBuoyDev"].Style.ForeColor = Color.Red;
                    } else {
                        curTolCheckResults[curViewIdx] = -1;
                        curViewRow.Cells["boatPathBuoyDev"].Style.ForeColor = Color.BlueViolet;
                    }
                }
                curViewRow.Cells["boatPathRerideFlag"].Value = curTolCheckResults[curViewIdx].ToString();
                return curReturnValue;

            } else if ( curViewIdx == 7 ) {
                curViewRow.Cells["boatPathPos"].Value = "EXIT";
                curViewRow.Cells["boatTimeBuoy"].Value = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "boatTimeBuoy" + curViewIdx, "0", 2 );
                curViewRow.Cells["boatPathRerideFlag"].Value = "0";
                return true;
            }

            curViewRow.Cells["boatPathPos"].Value = (String)curBoatPathDevMaxRow["Buoy"];

            if ( curPassScore == ( curViewIdx - 1 ) ) {
                curViewRow.Cells["boatTimeBuoy"].Value = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "boatTimeBuoy" + curViewIdx, "0", 2 );
                curViewRow.Cells["boatPathRerideFlag"].Value = "0";
                return true;
            }

            curViewRow.Cells["boatPathScoreRange"].Value = ( (String)curBoatPathDevMaxRow["CodeDesc"] ).Substring( 6, 9 );
            curViewRow.Cells["boatPathBuoyTol"].Value = curBuoyDevTol.ToString( "##0" );
            curViewRow.Cells["boatPathCumTol"].Value = curCumDevTol.ToString( "##0" );

            curViewRow.Cells["boatPathZoneDev"].Value = curPathZoneDev.ToString( "##0" );
            curViewRow.Cells["boatPathBuoyDev"].Value = curPathBuoyDev.ToString( "##0" );
            curViewRow.Cells["boatPathCumDev"].Value = curPathCumDev.ToString( "##0" );

            curViewRow.Cells["boatTimeBuoy"].Value = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "boatTimeBuoy" + curViewIdx, "0", 2 );

            if ( curBuoyDevTol > 0 && Math.Abs( curPathBuoyDev ) > curBuoyDevTol ) {
                curReturnValue = false;
                curViewRow.Cells["boatPathBuoyDev"].Style.Font = curFontBold;
                if ( curPathBuoyDev > 0 ) {
                    curTolCheckResults[curViewIdx] = 1;
                    curViewRow.Cells["boatPathBuoyDev"].Style.ForeColor = Color.Red;
                } else {
                    curTolCheckResults[curViewIdx] = -1;
                    curViewRow.Cells["boatPathBuoyDev"].Style.ForeColor = Color.BlueViolet;
                }
            }

            if ( curCumDevTol > 0 && Math.Abs( curPathCumDev ) > curCumDevTol ) {
                curReturnValue = false;
                curViewRow.Cells["boatPathCumDev"].Style.Font = curFontBold;
                if ( curPathCumDev > 0 ) {
                    curTolCheckResults[curViewIdx] = 1;
                    curViewRow.Cells["boatPathCumDev"].Style.ForeColor = Color.Red;
                } else {
                    curTolCheckResults[curViewIdx] = -1;
                    curViewRow.Cells["boatPathCumDev"].Style.ForeColor = Color.BlueViolet;
                }
            }

            curViewRow.Cells["boatPathRerideFlag"].Value = curTolCheckResults[curViewIdx].ToString();

            return curReturnValue;
        }

    }
}
