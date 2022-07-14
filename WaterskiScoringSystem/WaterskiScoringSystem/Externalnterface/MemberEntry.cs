using System;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Externalnterface {
	class MemberEntry {
		private String myMemberId = "";
		private String myLastName = "";
		private String myFirstName = "";
		private String myFederation = "";
		private String myAddress1 = "";
		private String myAddress2 = "";
		private String myCity = "";
		private String myState = "";
		private String myCountry = "";
		private String myPostalcode = "";

		private Int16 mySkiYearAge = 1;
		private String myGender = "";
		private String myAgeGroup = "";
		private String myTeam = "";

		private String myRegEventClassSlalom = "";
		private String myRegEventClassTrick = "";
		private String myRegEventClassJump = "";

		private String myEventGroupSlalom = "";
		private String myEventGroupTrick = "";
		private String myEventGroupJump = "";

		private String myMemberStatus = "";
		private String myReadyToSki = "N";
		private Boolean myCanSki = false;
		private Boolean myCanSkiGR = false;
		private Boolean myWaiverSigned = false;
		private DateTime myMemberExpireDate = Convert.ToDateTime("1/1/1900");
		private String myNote = "";

		private String myJudgeSlalomRating = "";
		private String myJudgeTrickRating = "";
		private String myJudgeJumpRating = "";
		private String myDriverSlalomRating = "";
		private String myDriverTrickRating = "";
		private String myDriverJumpRating = "";
		private String myScorerSlalomRating = "";
		private String myScorerTrickRating = "";
		private String myScorerJumpRating = "";
		private String mySafetyOfficialRating = "";
		private String myTechControllerRating = "";
		private String myAnncrOfficialRating = "";

		private DateTime myInsertDate = DateTime.Today;
		private DateTime myUpdateDate = DateTime.Today;

		public string MemberId { get => myMemberId;
			set => myMemberId = value.Length == 11 ? value.Substring( 0, 3 ) + value.Substring( 4, 2 ) + value.Substring( 7, 4 ) : value; 
		}
		public string LastName { get => myLastName; set => myLastName = value; }
		public string getLastNameForDB() {
			return HelperFunctions.stringReplace( myLastName, HelperFunctions.SingleQuoteDelim, "''" );
		}
		public string FirstName { get => myFirstName; set => myFirstName = value; }
		public string getFirstNameForDB() {
			return HelperFunctions.stringReplace( myFirstName, HelperFunctions.SingleQuoteDelim, "''" );
		}
		
		public string getSkierName() {
			return String.Format( "{0}, {1}", myLastName, myFirstName );
		}
		public string getSkierNameForDB() {
			return HelperFunctions.stringReplace( getSkierName(), HelperFunctions.SingleQuoteDelim, "''" );
		}
		
		public string AgeGroup { get => myAgeGroup; set => myAgeGroup = value; }
		public string Federation { get => myFederation; set => myFederation = value; }
		public string Address1 { get => myAddress1; set => myAddress1 = value; }
		public string Address2 { get => myAddress2; set => myAddress2 = value; }
		public string City { get => myCity; set => myCity = value; }
		public string getCityForDB() {
			return HelperFunctions.stringReplace( myCity, HelperFunctions.SingleQuoteDelim, "''" );
		}
		public string State { get => myState; set => myState = value; }
		public string Country { get => myCountry; set => myCountry = value; }
		public string Postalcode { get => myPostalcode; set => myPostalcode = value; }
		public Int16 SkiYearAge { get => mySkiYearAge; set => mySkiYearAge = value; }
		public string Gender { get => myGender; set => myGender = value; }
		public string MemberStatus { get => myMemberStatus;
			set { myMemberStatus = value;
				if ( myMemberStatus.ToLower().Equals( ImportMember.MembershipStatusTextActive.ToLower() ) ) 
					myReadyToSki = "Y"; 
				else 
					myReadyToSki = "N"; 
			} 
		}
		public string ReadyToSki { get => myReadyToSki; set => myReadyToSki = value; }
		public Boolean CanSki { get => myCanSki; set => myCanSki = value; }
		public Boolean CanSkiGR { get => myCanSkiGR; set => myCanSkiGR = value; }
		public DateTime MemberExpireDate { get => myMemberExpireDate; set => myMemberExpireDate = value; }
		public string Note { get => myNote; set => myNote = value; }
		public void setRegEventClass( String curEvent, String curValue ) {
			if ( curEvent.Equals( "Slalom" ) ) RegEventClassSlalom = curValue;
			else if ( curEvent.Equals( "Trick" ) ) RegEventClassTrick = curValue;
			else if ( curEvent.Equals( "Jump" ) ) RegEventClassJump = curValue;
		}
		public String getRegEventClass( String curEvent ) {
			if ( curEvent.Equals( "Slalom" ) ) return RegEventClassSlalom;
			else if ( curEvent.Equals( "Trick" ) ) return RegEventClassTrick;
			else if ( curEvent.Equals( "Jump" ) ) return RegEventClassJump;
			return "";
		}
		public string RegEventClassSlalom { get => myRegEventClassSlalom; set => myRegEventClassSlalom = value; }
		public string RegEventClassTrick { get => myRegEventClassTrick; set => myRegEventClassTrick = value; }
		public string RegEventClassJump { get => myRegEventClassJump; set => myRegEventClassJump = value; }

		public string getEventGroup(String curEvent ) {
			if ( curEvent.Equals( "Slalom" ) ) return myEventGroupSlalom;
			else if ( curEvent.Equals( "Trick" ) ) return myEventGroupTrick;
			else if ( curEvent.Equals( "Jump" ) ) return myEventGroupJump;
			return "";
		}
		public string EventGroupSlalom { get => myEventGroupSlalom; set => myEventGroupSlalom = value; }
		public string EventGroupTrick { get => myEventGroupTrick; set => myEventGroupTrick = value; }
		public string EventGroupJump { get => myEventGroupJump; set => myEventGroupJump = value; }
		public string Team { get => myTeam; set => myTeam = value; }
		public string JudgeSlalomRating { get => myJudgeSlalomRating; set => myJudgeSlalomRating = convertUnratedRating( value ); }
		public string JudgeTrickRating { get => myJudgeTrickRating; set => myJudgeTrickRating = convertUnratedRating(value); }
		public string JudgeJumpRating { get => myJudgeJumpRating; set => myJudgeJumpRating = convertUnratedRating( value ); }
		public string DriverSlalomRating { get => myDriverSlalomRating; set => myDriverSlalomRating = convertUnratedRating( value ); }
		public string DriverTrickRating { get => myDriverTrickRating; set => myDriverTrickRating = convertUnratedRating( value ); }
		public string DriverJumpRating { get => myDriverJumpRating; set => myDriverJumpRating = convertUnratedRating( value ); }
		public string ScorerSlalomRating { get => myScorerSlalomRating; set => myScorerSlalomRating = convertUnratedRating( value ); }
		public string ScorerTrickRating { get => myScorerTrickRating; set => myScorerTrickRating = convertUnratedRating( value ); }
		public string ScorerJumpRating { get => myScorerJumpRating; set => myScorerJumpRating = convertUnratedRating( value ); }
		public string SafetyOfficialRating { get => mySafetyOfficialRating; set => mySafetyOfficialRating = convertUnratedRating( value ); }
		public string TechControllerRating { get => myTechControllerRating; set => myTechControllerRating = convertUnratedRating( value ); }
		public string AnncrOfficialRating { get => myAnncrOfficialRating; set => myAnncrOfficialRating = convertUnratedRating( value ); }
		public DateTime InsertDate { get => myInsertDate; set => myInsertDate = value; }
		public DateTime UpdateDate { get => myUpdateDate; set => myUpdateDate =  value ; }
		public Boolean WaiverSigned { get => myWaiverSigned; set => myWaiverSigned =  value ; }

		private String convertUnratedRating( String inValue ) {
			if ( inValue.ToLower().Equals( "unrated" ) ) return "";
			return inValue;
		}
	}
}
