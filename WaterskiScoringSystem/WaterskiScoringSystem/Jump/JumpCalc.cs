using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Jump {
    class JumpCalc {
        //Values 1 thru 12 equivalent to A1 thru A12
        //Values 21 thru 28 equivalent to T1 thru T8
        //Values 31 thru 34 equivalent to S1 thru S4
        private Double[] myWorkValue = new Double[35];
        
        private Double myAngleAtoB;
        private Double myAngleBtoA;
        private Double myAngleAtoC;
        private Double myAngleCtoA;
        private Double myAngleBtoC;
        private Double myAngleCtoB;

        private Double myDistAtoB;
        private Double myDistAtoC;
        private Double myDistBtoC;

        private Decimal myTriangleZero;
        private Decimal myTriangleJump;
        private Decimal myTriangle15M;
        private Decimal myXCoordZero;
        private Decimal myYCoordZero;
        private Decimal myXCoord15M;
        private Decimal myYCoord15M;
        private Decimal myXCoordJump;
        private Decimal myYCoordJump;

        private Int16 myJumpDir;
        private DataTable mySetupDataTable;

        public JumpCalc() {
            //Constructor
        }

        public JumpCalc(String inSanctionId) {
            // Constructor
            // Retrieve jump meter setup data and populate instance variables.
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM JumpMeterSetup WHERE SanctionId = '" + inSanctionId + "' " );
            mySetupDataTable = getData( curSqlStmt.ToString() );
            if ( mySetupDataTable.Rows.Count > 0 ) {
                DataRow curRow = mySetupDataTable.Rows[0];
                try {
                    myAngleAtoB = Convert.ToDouble( (Decimal)curRow["AngleAtoB"] );
                    myAngleBtoA = Convert.ToDouble( (Decimal)curRow["AngleBtoA"] );
                    myAngleAtoC = Convert.ToDouble( (Decimal)curRow["AngleAtoC"] );
                    myAngleCtoA = Convert.ToDouble( (Decimal)curRow["AngleCtoA"] );
                    myAngleBtoC = Convert.ToDouble( (Decimal)curRow["AngleBtoC"] );
                    myAngleCtoB = Convert.ToDouble( (Decimal)curRow["AngleCtoB"] );

                    myDistAtoB = Convert.ToDouble( (Decimal)curRow["DistAtoB"] );
                    myDistAtoC = Convert.ToDouble( (Decimal)curRow["DistAtoC"] );
                    myDistBtoC = Convert.ToDouble( (Decimal)curRow["DistBtoC"] );

                    myTriangleZero = (Decimal)curRow["TriangleZero"];
                    myTriangle15M = (Decimal)curRow["Triangle15ET"];
                    myXCoordZero = (Decimal)curRow["XCoordZero"];
                    myYCoordZero = (Decimal)curRow["YCoordZero"];
                    myXCoord15M = (Decimal)curRow["XCoord15ET"];
                    myYCoord15M = (Decimal)curRow["YCoord15ET"];
                    myJumpDir = (Byte)curRow["JumpDirection"];
                    if ( myJumpDir == 0 ) myJumpDir = -1;
                } catch {
                    MessageBox.Show( "Exception encountered loaded jump setup data" );
                }
            } else {
                myDistAtoB = 0;
                myDistAtoC = 0;
                myDistBtoC = 0;
            }
        }

        public bool calcTriangle( Double inAngleA, Double inAngleB, Double inAngleC ) {
            /*
            * Routine to solve for the Center of the triangle based on
            * the input angles.  First derive intersection coordinates.
            */
            // A1=AB*SIN((B-QA)*RC)/SIN((180-(B-QA)-(PB-A))*RC)
            // A1=ABS(A1)
            // A2=A1*SIN((PC-A)*RC)
            // A3=A1*COS((PC-A)*RC):' A-B Intersection

            // A-B Intersection
            myWorkValue[1] = myDistAtoB * Math.Sin( ( inAngleB - myAngleBtoA ) * RC )
                / Math.Sin( ( 180 - ( inAngleB - myAngleBtoA ) - ( myAngleAtoB - inAngleA ) ) * RC );
            myWorkValue[1] = Math.Abs( myWorkValue[1] );
            myWorkValue[2] = myWorkValue[1] * Math.Sin( ( myAngleAtoC - inAngleA ) * RC );
            myWorkValue[3] = myWorkValue[1] * Math.Cos( ( myAngleAtoC - inAngleA ) * RC );

            // A-C Intersection
            // A4=AC*SIN((C-RA)*RC)/SIN((180-(C-RA)-(PC-A))*RC):A4=ABS(A4)
            // A5=A4*SIN((PC-A)*RC):A6=A4*COS((PC-A)*RC):' A-C Intersection
            myWorkValue[4] = myDistAtoC * Math.Sin( ( inAngleC - myAngleCtoA ) * RC )
                / Math.Sin( ( 180 - ( inAngleC - myAngleCtoA ) - ( myAngleAtoC - inAngleA ) ) * RC );
            myWorkValue[4] = Math.Abs( myWorkValue[4] );
            myWorkValue[5] = myWorkValue[4] * Math.Sin( ( myAngleAtoC - inAngleA ) * RC );
            myWorkValue[6] = myWorkValue[4] * Math.Cos( ( myAngleAtoC - inAngleA ) * RC );

            // B-C Intersection
            // A7=BC*SIN((QC-B)*RC)/SIN((180-(C-RB)-(QC-B))*RC):A7=ABS(A7)
            // A8=A7*SIN((C-RA)*RC):A9=AC-A7*COS((C-RA)*RC):' B-C Intersection
            myWorkValue[7] = myDistBtoC * Math.Sin( ( myAngleBtoC - inAngleB ) * RC )
                / Math.Sin( ( 180 - ( inAngleC - myAngleCtoB ) - ( myAngleBtoC - inAngleB ) ) * RC );
            myWorkValue[7] = Math.Abs( myWorkValue[7] );
            myWorkValue[8] = myWorkValue[7] * Math.Sin( ( inAngleC - myAngleCtoA ) * RC );
            myWorkValue[9] = myDistAtoC - myWorkValue[7] * Math.Cos( ( inAngleC - myAngleCtoA ) * RC );

            // A-B & B-C Vertex Bisector angles and slopes
            // T2=((A+180-PC)+(B-QA+PB-PC))/2:T3=-TAN(T2*RC):' A-B Vertex
            // T8=((B-QA+PB-PC)+(C-RA))/2:T9=-TAN(T8*RC):' B-C Vertex
            // A11=((A2/T3)-(A8/T9)-A3+A9)/((1/T3)-(1/T9))
            // A12=(T3*A3-T9*A9-A2+A8)/(T3-T9)
            // A-B Vertex
            myWorkValue[22] = ( ( inAngleA + 180 - myAngleAtoC )
                + ( inAngleB - myAngleBtoA + myAngleAtoB - myAngleAtoC ) ) / 2;
            myWorkValue[23] = -Math.Tan( myWorkValue[22] * RC );
            // B-C Vertex
            myWorkValue[28] = ( ( inAngleB - myAngleBtoA + myAngleAtoB - myAngleAtoC )
                + ( inAngleC - myAngleCtoA ) ) / 2;

            myWorkValue[29] = -Math.Tan( myWorkValue[28] * RC );
            myWorkValue[11] = ( ( myWorkValue[2] / myWorkValue[23] )
                - ( myWorkValue[8] / myWorkValue[29] ) - myWorkValue[3] + myWorkValue[9] )
                / ( ( 1 / myWorkValue[23] ) - ( 1 / myWorkValue[29] ) );
            myWorkValue[12] = ( myWorkValue[23] * myWorkValue[3]
                - myWorkValue[29] * myWorkValue[9] - myWorkValue[2] + myWorkValue[8] )
                / ( myWorkValue[23] - myWorkValue[29] );

            // Calculate Side Lengths and Error Circle Diameter (TR)
            // S1=SQR((A2-A5)^2+(A3-A6)^2):S2=SQR((A2-A8)^2+(A3-A9)^2)
            // S3=SQR((A5-A8)^2+(A6-A9)^2):S4=(S1+S2+S3)/2
            // TR=2*SQR((S4-S1)*(S4-S2)*(S4-S3)/S4)
            myWorkValue[31] = Math.Sqrt(
                Math.Pow( myWorkValue[2] - myWorkValue[5], 2 )
                + Math.Pow( myWorkValue[3] - myWorkValue[6], 2 ) );
            myWorkValue[32] = Math.Sqrt(
                Math.Pow( myWorkValue[2] - myWorkValue[8], 2 )
                + Math.Pow( myWorkValue[3] - myWorkValue[9], 2 ) );
            myWorkValue[33] = Math.Sqrt(
                Math.Pow( myWorkValue[5] - myWorkValue[8], 2 )
                + Math.Pow( myWorkValue[6] - myWorkValue[9], 2 ) );
            myWorkValue[34] = ( myWorkValue[31] + myWorkValue[32] + myWorkValue[33] ) / 2;
            myWorkValue[13] = 2 *
                Math.Sqrt( ( myWorkValue[34] - myWorkValue[31] )
                    * ( myWorkValue[34] - myWorkValue[32] )
                    * ( myWorkValue[34] - myWorkValue[33] )
                    / myWorkValue[34] );

            // Round X, Y and Error Circle Diameter
            // X=INT(100*A12+.5)/100:Y=INT(100*A11+.5)/100:TR=INT(100*TR+.5)/100
            myXCoordJump = Math.Round( Convert.ToDecimal( myWorkValue[12] ), 2);
            myYCoordJump = Math.Round( Convert.ToDecimal( myWorkValue[11] ), 2);
            myTriangleJump = Math.Round( Convert.ToDecimal( myWorkValue[13] ), 2);

            return true;
        }

        public Int32[] calcDistance( Double inAngleA, Double inAngleB, Double inAngleC ) {
            if ( calcTriangle( inAngleA, inAngleB, inAngleC ) ) {
                return calcDistance();
            } else {
                Int32[] returnValues = new Int32[3];
                returnValues[0] = -1;
                return returnValues;
            }
        }
        public Int32[] calcDistance( ) {
            Int32[] returnValues = new Int32[5];
            Double numDistHigh = 0, numDistLow = 0;

            // Calculate distance
            // AL=SQR((X-XI)^2+(Y-YI)^2)
            Double numDist = Math.Sqrt( 
                Math.Pow( ( myWorkValue[12] - Convert.ToDouble(myXCoordZero) ), 2 ) 
                + Math.Pow( ( myWorkValue[11] - Convert.ToDouble(myYCoordZero) ), 2 ) 
                );
            // D1=SQR((A3-XI)^2+(A2-YI)^2)
            // D2=SQR((A6-XI)^2+(A5-YI)^2)
            // D3=SQR((A9-XI)^2+(A8-YI)^2)
            Double numDist1 = Math.Sqrt(
                Math.Pow( ( myWorkValue[3] - Convert.ToDouble( myXCoordZero ) ), 2 )
                + Math.Pow( ( myWorkValue[2] - Convert.ToDouble( myYCoordZero ) ), 2 )
                );
            Double numDist2 = Math.Sqrt(
                Math.Pow( ( myWorkValue[6] - Convert.ToDouble( myXCoordZero ) ), 2 )
                + Math.Pow( ( myWorkValue[5] - Convert.ToDouble( myYCoordZero ) ), 2 )
                );
            Double numDist3 = Math.Sqrt(
                Math.Pow( ( myWorkValue[9] - Convert.ToDouble( myXCoordZero ) ), 2 )
                + Math.Pow( ( myWorkValue[8] - Convert.ToDouble( myYCoordZero ) ), 2 )
                );

            if ( numDist1 > numDist2 ) {
                if ( numDist1 > numDist3 ) {
                    numDistHigh = numDist1;
                } else {
                    numDistHigh = numDist3;
                }
            } else {
                if ( numDist2 > numDist3 ) {
                    numDistHigh = numDist2;
                } else {
                    numDistHigh = numDist3;
                }
            }

            if ( numDist1 < numDist2 ) {
                if ( numDist1 < numDist3 ) {
                    numDistLow = numDist1;
                } else {
                    numDistLow = numDist3;
                }
            } else {
                if ( numDist2 < numDist3 ) {
                    numDistLow = numDist2;
                } else {
                    numDistLow = numDist3;
                }
            }

            Int32 returnDistHigh = (Int32)Math.Round( Convert.ToDecimal( numDistHigh ), 0 );
            Int32 returnDistLow = (Int32)Math.Round( Convert.ToDecimal( numDistLow ), 0 );
            Int32 returnDist = (Int32)Math.Round( Convert.ToDecimal( numDist ), 0 );
            Int32 returnDistExtd = (Int32)Math.Round( Convert.ToDecimal( numDist * 100 ), 0 );

            returnValues[0] = returnDist;
            returnValues[1] = returnDistHigh;
            returnValues[2] = returnDistLow;
            returnValues[3] = returnDistExtd;

            returnDist = (Int32)Math.Round( Convert.ToDecimal( numDist ), 0 );
            returnDistExtd = (Int32)Math.Round( Convert.ToDecimal( numDistLow * 100 ), 0 );
            returnValues[4] = returnDistExtd;

            return returnValues;
        }

        public Double calcCourseAngle( Double inDist ) {
            //RJ = ATN( ( Y - YI ) / ( X - XI ) ) - RL * ATN( 49.2 / SQR( ABS( AL * AL - 2420.6 ) ) );
            Double xCoordJump = Convert.ToDouble( myXCoordJump );
            Double yCoordJump = Convert.ToDouble( myYCoordJump );
            Double xCoordZero = Convert.ToDouble( myXCoordZero );
            Double yCoordZero = Convert.ToDouble( myYCoordZero );
            Double numJumpBaseline = Math.Atan( ( yCoordJump - yCoordZero ) / ( xCoordJump - xCoordZero ) )
                 - myJumpDir * Math.Atan( 49.2 / Math.Sqrt( Math.Abs( inDist * inDist - 2420.6 ) ) );
            numJumpBaseline = numJumpBaseline / RC;
            return numJumpBaseline;
        }

        public bool calcDistAtoC() {
            Double tempFloatValue = ( myDistAtoB * myDistAtoB ) + ( myDistBtoC * myDistBtoC )
                - ( 2 * ( myDistAtoB * myDistBtoC ) * Math.Cos( ( myAngleBtoC - myAngleBtoA ) * RC ) );
            Double numCalcDistAtoC = Math.Sqrt( tempFloatValue );
            numCalcDistAtoC = Convert.ToDouble( Math.Round( Convert.ToDecimal( numCalcDistAtoC ), 2 ) );
            if ( numCalcDistAtoC > ( myDistAtoC + .1 )
                || numCalcDistAtoC < ( myDistAtoC - .1 )
                ) {
                MessageBox.Show( "Calculated distance of meter A to meter C does not successfully match input distance"
                    + "\n Must be plus or minus .1"
                    + "\n A to C distance has been set to calculated value"
                    + "\n\n Calculated distance from meter A to meter C = " + numCalcDistAtoC.ToString( "##0.00" )
                    + "\n Input distance from meter A to meter C = " + myDistAtoC.ToString( "##0.00" )
                    + "\n\n Formula: SQRT(( curDistAtoB * curDistAtoB ) + ( curDistBtoC * curDistBtoC ) "
                    + " - ( 2 * ( curDistAtoB * curDistBtoC * Math.Cos( ( curAngleBtoC - curAngleBtoA ) * curRC ) ) ) )"
                    );
                distAtoC = numCalcDistAtoC;
                return false;
            } else {
                return true;
            }
        }

        public bool checkMeterSetup() {
            Double numAngleMeterCheck = myAngleAtoB - myAngleAtoC + myAngleBtoC - myAngleBtoA + myAngleCtoA - myAngleCtoB;
            Decimal curAngleMeterCheck = Convert.ToDecimal( numAngleMeterCheck.ToString("##0.000") );
            if ( curAngleMeterCheck > 180.001M
                || curAngleMeterCheck < 179.999M
                ) {
                MessageBox.Show( "Meter to meter validation failed."
                    + "\n Validation calculation equals " + curAngleMeterCheck.ToString( "##0.00" )
                    + "\n Must be plus or minus .001 from 180 degrees"
                    + "\n\n Formula: Angle AtoB - Angle AtoC + Angle BtoC - Angle BtoA + Angle CtoA - Angle CtoB"
                    );
                return false;
            } else {
                return true;
            }
        }

        public bool ValidateMeterSetup() {
            if ( checkMeterSetup() ) {
                if ( this.xCoordZero != 0 && this.yCoordZero != 0 ) {
                    if ( this.xCoord15M > 0 && this.yCoord15M > 0 ) {
                        return true;
                    } else {
                        MessageBox.Show("Coordinates for 15 meter timing buoy have not been calculated and are required");
                        return false;
                    }
                } else {
                    MessageBox.Show("Coordinates for jump center point have not been calculated and are required");
                    return false;
                }
            } else {
                return false;
            }
        }

        public bool checkMeterConfig() {
            /*
             * VA=ABS(PB-PC)*RC
             * VC=ABS(RA-RB)*RC
             */
            Double numVertexA = Math.Abs( myAngleAtoB - myAngleAtoC ) * RC;
            Double numVertexC = Math.Abs( myAngleCtoA - myAngleCtoB ) * RC;
            if ( Math.Abs( numVertexA ) < .002 ) {
                MessageBox.Show( "Meter Configuration Proportionality is acceptable"
                    + "\n"
                    + "\n Vertex  AB to AC = " + numVertexA.ToString( "##0.000" )
                    + "\n Vertex  CA to CB = " + numVertexC.ToString( "##0.000" )
                    );
                return true;
            } else {
                /*
                 * IF BC/SIN(VA-.001)<AB/SIN(VC+.001) THEN ST$="Large"
                 * IF BC/SIN(VA+.001)>AB/SIN(VC-.001) THEN ST$="Small"
                 */
                Double numVertexAa = myDistBtoC / ( Math.Sin( numVertexA ) );
                Double numVertexCc = myDistAtoB / ( Math.Sin( numVertexC ) );
                if ( Math.Abs( numVertexAa - numVertexCc ) < .002 ) {
                    /*
                    MessageBox.Show( "Meter Configuration Proportionality is acceptable"
                        + "\n Results must be within plus or minus .002"
                        + "\n Vertex  AB to AC = " + numVertexA.ToString( "##0.000" ) + " : " + numVertexAa.ToString( "##0.000" )
                        + "\n Vertex  CA to CB = " + numVertexC.ToString( "##0.000" ) + " : " + numVertexCc.ToString( "##0.000" )
                        + "\n\n Formula: "
                        + "\n DistBtoC / Math.Sin(  Math.Abs( AngleAtoB - AngleAtoC ) * (PI / 1800) ) "
                        + "\n DistAtoB / Math.Sin(  Math.Abs( AngleCtoA - AngleCtoB ) * (PI / 1800) ) "
                        );
                     */
                    return true;
                } else {
                    MessageBox.Show( "Meter Configuration Proportionality is not acceptable"
                        + "\n Results must be within plus or minus .002"
                        + "\n Vertex  AB to AC = " + numVertexA.ToString( "##0.000" ) + " : " + numVertexAa.ToString( "##0.000" )
                        + "\n Vertex  CA to CB = " + numVertexC.ToString( "##0.000" ) + " : " + numVertexCc.ToString( "##0.000" )
                        + "\n\n Formula: "
                        + "\n DistBtoC / Math.Sin(  Math.Abs( AngleAtoB - AngleAtoC ) * (PI / 1800) ) "
                        + "\n DistAtoB / Math.Sin(  Math.Abs( AngleCtoA - AngleCtoB ) * (PI / 1800) ) "
                        );
                    return false;
                }
            }
        }

        public Double angleAtoB {
            get {
                return myAngleAtoB;
            }
            set {
                myAngleAtoB = value;
            }
        }

        public Double angleBtoA {
            get {
                return myAngleBtoA;
            }
            set {
                myAngleBtoA = value;
            }
        }

        public Double angleAtoC {
            get {
                return myAngleAtoC;
            }
            set {
                myAngleAtoC = value;
            }
        }

        public Double angleCtoA {
            get {
                return myAngleCtoA;
            }
            set {
                myAngleCtoA = value;
            }
        }

        public Double angleBtoC {
            get {
                return myAngleBtoC;
            }
            set {
                myAngleBtoC = value;
            }
        }

        public Double angleCtoB {
            get {
                return myAngleCtoB;
            }
            set {
                myAngleCtoB = value;
            }
        }

        public Double distAtoB {
            get {
                return myDistAtoB;
            }
            set {
                myDistAtoB = value;
            }
        }

        public Double distAtoC {
            get {
                return myDistAtoC;
            }
            set {
                myDistAtoC = value;
            }
        }

        public Double distBtoC {
            get {
                return myDistBtoC;
            }
            set {
                myDistBtoC = value;
            }
        }

        public Int16 jumpDirection {
            get {
                return myJumpDir;
            }
            set {
                myJumpDir = value;
            }
        }

        public Double RC {
            get {
                Double curValue = Math.PI / 180;
                return curValue;
            }
        }

        public Decimal TriangleJump {
            get {
                return myTriangleJump;
            }
            set {
                myTriangleJump = value;
            }
        }

        public Decimal TriangleZero {
            get {
                return myTriangleZero;
            }
            set {
                myTriangleZero = value;
            }
        }

        public Decimal Triangle15M {
            get {
                return myTriangle15M;
            }
            set {
                myTriangle15M = value;
            }
        }

        public Decimal xCoordJump {
            get {
                return myXCoordJump;
            }
            set {
                myXCoordJump = value;
            }
        }

        public Decimal yCoordJump {
            get {
                return myYCoordJump;
            }
            set {
                myYCoordJump = value;
            }
        }

        public Decimal xCoordZero {
            get {
                return myXCoordZero;
            }
            set {
                myXCoordZero = value;
            }
        }

        public Decimal yCoordZero {
            get {
                return myYCoordZero;
            }
            set {
                myYCoordZero = value;
            }
        }

        public Decimal xCoord15M {
            get {
                return myXCoord15M;
            }
            set {
                myXCoord15M = value;
            }
        }

        public Decimal yCoord15M {
            get {
                return myYCoord15M;
            }
            set {
                myYCoord15M = value;
            }
        }

        public Double[] workValues {
            get {
                return myWorkValue;
            }
            set {
                myWorkValue = value;
            }
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }
}
