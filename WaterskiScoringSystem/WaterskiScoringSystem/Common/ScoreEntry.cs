using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterskiScoringSystem.Common {
    public class ScoreEntry {
        private String myEvent;
        private String myRating;
        private Decimal myScore;
        private Decimal myNops;

        public ScoreEntry() {
        }
        public ScoreEntry(String inEvent, Decimal inScore, String inRating, Decimal inNops) {
            myEvent = inEvent;
            myRating = inRating;
            myScore = inScore;
            myNops = inNops;
        }

        public String Event {
            get {
                return myEvent;
            }
            set {
                myEvent = value;
            }
        }

        public Decimal Score {
            get {
                return myScore;
            }
            set {
                myScore = value;
            }
        }

        public String Rating {
            get {
                return myRating;
            }
            set {
                myRating = value;
            }
        }

        public Decimal Nops {
            get {
                return myNops;
            }
            set {
                myNops = value;
            }
        }

        public override String ToString() {
            String myScoreEntry = myEvent + ", " + myScore + ", " + myRating + ", " + myNops;
            return myScoreEntry;
        }

    }
}
