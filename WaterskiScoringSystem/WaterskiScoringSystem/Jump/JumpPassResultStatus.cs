using System;

namespace WaterskiScoringSystem.Jump {
	class JumpPassResultStatus {
		public bool isIwwfSkier = false;

		public bool rerideImproveAllowed = false; // Indicates if there is a pass marked as a mandatory reride if best score
		public Decimal rerideScoreFeet = 0M; // Indicates score in feet for a pass marked as a mandatory reride if best score
		public Decimal rerideScoreMeters = 0M; // Indicates score in meters for a pass marked as a mandatory reride if best score

		public Int16 passCount = 0; // Number of passes whether good or not
		public Int16 passCountGood = 0; // Number of passes with acceptable time and condiitions 
		public Int16 passCountOptReride = 0; // Number of passes with good time but marked as reride (requested!)
		public Int16 passCountOptOutReride = 0; // Number of passes out of tolerance time but with optional reride
		public Int16 passCountManReride = 0; // Number of passes requiring reride if score best of set

		/*
		 * Accepted score attributes for the jump set
		 */
		public String skierSetStatus = "";
		
		public Decimal bestScoreFeet = 0M;
		public Decimal bestScoreMeters = 0M;

		/*
		 * Attributes of current pass being analyzed
		 */
		public String passResult = "";
		public Decimal passScoreFeet = 0M;
		public Decimal passScoreMeters = 0M;

		public bool passReride = false;
		public bool passTimeInTol = false;
		public bool passScoreProt = false;
		public bool passRerideIfBest = false;
		public bool passRerideCanImprove = false;

		public JumpPassResultStatus() {
			isIwwfSkier = false;
			bestScoreFeet = 0M;
			bestScoreMeters = 0M;

			rerideImproveAllowed = false;
			rerideScoreFeet = 0M;
			rerideScoreMeters = 0M;

			passCount = 0;
			passCountGood = 0;
			passCountOptReride = 0;
			passCountOptOutReride = 0;
			passCountManReride = 0;

			skierSetStatus = "1-TBD";

			setPassDefultResult();
		}

		public void setPassDefultResult() {
			passResult = ""; 
			
			passScoreFeet = 0M;
			passScoreMeters = 0M;

			passTimeInTol = true;
			passReride = false;
			passScoreProt = false;
			passRerideIfBest = false;
			passRerideCanImprove = false;

		}

		public void passGoodJump() {
			passCount++;
			passCountGood++;
			updateBestScore();
		}

		public void checkSetStatus() {
			if ( passCount > 0 ) {
				if ( passCountGood >= 3 ) {
					skierSetStatus = "4-Done";
				} else {
					skierSetStatus = "2-InProg";
				}
			} else {
				skierSetStatus = "1-TBD";
			}
		}

		public bool isSkierSetComplete() {
			if ( skierSetStatus.Equals( "4-Done" ) ) return true;
			return false;
		}

		public void passGoodNoJump() {
			passCount++;
			passCountGood++;
			if ( passCountOptOutReride > 0 ) passCountOptOutReride--;
			else if ( passCountOptReride > 0 ) passCountOptReride--;
			else if ( passCountManReride > 0 && passCount > 3 ) passCountManReride--;
		}

		public void passOptionalReride() {
			passCount++;
			passCountOptReride++;
			if ( passRerideCanImprove ) rerideImproveAllowed = passRerideCanImprove;
			if ( passScoreProt ) updateBestScore();

		}

		public void passOutTolReride() {
			passCount++;
			if ( passScoreProt ) {
				passCountOptOutReride++;
				updateBestScore();
			}
			if ( passRerideCanImprove ) rerideImproveAllowed = passRerideCanImprove;
			if ( passRerideIfBest ) updateIfBestRerideScore();
		}

		public void updateBestScore() {
			if ( ( isIwwfSkier && passScoreMeters > bestScoreMeters )
				|| ( !( isIwwfSkier ) && passScoreFeet > bestScoreFeet )
				|| ( !( isIwwfSkier ) && passScoreFeet == bestScoreFeet && passScoreMeters > bestScoreMeters )
				) {
				if ( passCount <= 3 ) {
					bestScoreMeters = passScoreMeters;
					bestScoreFeet = passScoreFeet;

				} else {
					// Reride pass, check for improvement
					if ( rerideImproveAllowed ) {
						bestScoreMeters = passScoreMeters;
						bestScoreFeet = passScoreFeet;

					} else if ( ( isIwwfSkier && passScoreMeters > rerideScoreMeters )
					|| ( !( isIwwfSkier ) && passScoreFeet > rerideScoreFeet )
					|| ( !( isIwwfSkier ) && passScoreFeet == rerideScoreFeet && passScoreMeters > rerideScoreMeters )
					) {
						bestScoreMeters = rerideScoreMeters;
						bestScoreFeet = rerideScoreFeet;

					} else {
						bestScoreMeters = passScoreMeters;
						bestScoreFeet = passScoreFeet;
					}
				}

				if ( passCountManReride > 0 ) removeIfBestRerideScore();
			}
		}

		public void updateIfBestRerideScore() {
			if ( ( isIwwfSkier && passScoreMeters > bestScoreMeters )
				|| ( !( isIwwfSkier ) && passScoreFeet > bestScoreFeet )
				|| ( !( isIwwfSkier ) && passScoreFeet == bestScoreFeet && passScoreMeters > bestScoreMeters )
				) {
				passCountManReride++;
				if ( ( isIwwfSkier && passScoreMeters > rerideScoreMeters )
					|| ( !( isIwwfSkier ) && passScoreFeet == rerideScoreFeet && passScoreMeters > rerideScoreMeters )
					) {
					rerideScoreMeters = passScoreMeters;
					rerideScoreFeet = passScoreFeet;
				}

			} else {
				passCountGood++;
			}
		}

		public void removeIfBestRerideScore() {
			if ( ( isIwwfSkier && bestScoreMeters > rerideScoreMeters )
				|| ( !( isIwwfSkier ) && bestScoreFeet > rerideScoreFeet )
				|| ( !( isIwwfSkier ) && bestScoreFeet == rerideScoreFeet && bestScoreMeters > rerideScoreMeters )
				) {
				passCountManReride--;
				passCountGood++;
				if ( passCountManReride <= 0 ) {
					rerideScoreMeters = 0;
					rerideScoreFeet = 0;
				}

				if ( passCountManReride <= 0 ) passRerideIfBest = false;
			}
		}

	}
}
