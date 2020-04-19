using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterskiScoringSystem.Common {
    class MemberIdValidate {

        public bool checkMemberId( String inMemberId ) {
            bool curReturnStatus = false;
            int curCheckSum = 0, curRem = 0, curSumOdd, curSumEven, curPosFirstNonZero = 0;
            char[] curMemberChars = inMemberId.ToCharArray();
            String curCheckValue = inMemberId.Substring( 0, 7 );
            curCheckValue = inMemberId.Substring( 7, 1 );
            if ( inMemberId.Substring( 0, 7 ).Equals( "0000000" ) ) {
                //Valid temporary membership number
                curReturnStatus = true;
            } else {
                curSumEven = Convert.ToInt32( curMemberChars[2].ToString() )
                    + Convert.ToInt32( curMemberChars[4].ToString() )
                    + Convert.ToInt32( curMemberChars[6].ToString() )
                    + Convert.ToInt32( curMemberChars[8].ToString() );
                curSumOdd = Convert.ToInt32( curMemberChars[1].ToString() )
                    + Convert.ToInt32( curMemberChars[3].ToString() )
                    + Convert.ToInt32( curMemberChars[5].ToString() )
                    + Convert.ToInt32( curMemberChars[7].ToString() );
                for ( int curIdx = 1; curIdx < 9; curIdx++ ) {
                    if ( !( curMemberChars[curIdx].ToString().Equals( "0" ) ) ) {
                        curPosFirstNonZero = curIdx;
                        break;
                    }
                }
                if ( curPosFirstNonZero > 0 ) {
                    long curValue = Math.DivRem( curPosFirstNonZero, 2, out curRem );
                    if ( curRem == 0 ) {
                        curValue = Math.DivRem( ( ( curSumEven * 3 ) + curSumOdd ), 10, out curRem );
                    } else {
                        curValue = Math.DivRem( ( ( curSumOdd * 3 ) + curSumEven ), 10, out curRem );
                    }
                    curCheckSum = 10 - curRem;
                    if ( curCheckSum == 10 ) curCheckSum = 0;
                }
                if ( curCheckSum == Convert.ToInt32( curMemberChars[0].ToString() ) ) {
                    curReturnStatus = true;
                }
            }

            return curReturnStatus;
        }

    }
}
