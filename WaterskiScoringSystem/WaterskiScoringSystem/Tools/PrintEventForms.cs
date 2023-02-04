using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Jump;
using WaterskiScoringSystem.Slalom;
using WaterskiScoringSystem.Trick;

namespace WaterskiScoringSystem.Tools {
	class PrintEventForms {
		private DataRow myTourRow = null;
		private String myTourRules = "";

		public PrintEventForms( DataRow inTourRow ) {
			myTourRow = inTourRow;
			myTourRules = (String)myTourRow["Rules"];
		}

		public void PrintSlalomForm( DataTable inEventRegDataTable, String inPrintHeaderNote, String inHeadToHeadDef ) {
			PrintSlalomFormDialog curDialog = new PrintSlalomFormDialog();
			if ( curDialog.ShowDialog() != DialogResult.OK ) return;

			String curPrintReport = curDialog.ReportName;
			if ( curPrintReport.Equals( "SlalomJudgeForm" ) ) {
				PrintSlalomJudgeForms curPrintForm = new PrintSlalomJudgeForms();
				curPrintForm.PrintLandscape = true;
				curPrintForm.ReportHeader = inPrintHeaderNote;
				curPrintForm.DivInfoDataTable = getSlalomDivMaxMinSpeed();
				curPrintForm.TourRules = myTourRules;
				if ( inHeadToHeadDef != null && inHeadToHeadDef.Length > 0 ) curPrintForm.TourRules = inHeadToHeadDef;
				curPrintForm.TourName = (String)myTourRow["Name"];

				curPrintForm.ShowDataTable = inEventRegDataTable;

				curPrintForm.Print();

			} else if ( curPrintReport.Equals( "SlalomRecapForm" ) ) {
				PrintSlalomRecapForm curPrintForm = new PrintSlalomRecapForm();
				curPrintForm.PrintLandscape = true;
				curPrintForm.ReportHeader = inPrintHeaderNote;
				curPrintForm.DivInfoDataTable = getSlalomDivMaxMinSpeed();
				curPrintForm.TourRules = myTourRules;
				if ( inHeadToHeadDef != null && inHeadToHeadDef.Length > 0 ) curPrintForm.TourRules = inHeadToHeadDef;
				curPrintForm.TourName = (String)myTourRow["Name"];

				curPrintForm.ShowDataTable = inEventRegDataTable;

				curPrintForm.Print();
			}
		}

		public void PrintJumpForm( DataTable inEventRegDataTable, String inPrintHeaderNote, String inHeadToHeadDef ) {
			PrintJumpFormDialog curDialog = new PrintJumpFormDialog();
			if ( curDialog.ShowDialog() != DialogResult.OK ) return;

			String curPrintReport = curDialog.ReportName;
			if ( curPrintReport.Equals( "JumpSkierSpecForm" ) ) {
				PrintJumpSkierSpecForm curPrintForm = new PrintJumpSkierSpecForm();
				curPrintForm.PrintLandscape = true;
				curPrintForm.ReportHeader = inPrintHeaderNote;
				curPrintForm.DivInfoDataTable = getTrickDivList();
				curPrintForm.TourName = (String)myTourRow["Name"];
				curPrintForm.TourRules = myTourRules;
				if ( inHeadToHeadDef != null && inHeadToHeadDef.Length > 0 ) curPrintForm.TourRules = inHeadToHeadDef;
				curPrintForm.TourRounds = Convert.ToInt32( myTourRow["TrickRounds"] );
				curPrintForm.ShowDataTable = inEventRegDataTable;
				curPrintForm.Print();

			} else {
				PrintJumpRecapJudgeForm curPrintForm = new PrintJumpRecapJudgeForm();
				curPrintForm.PrintLandscape = true;
				curPrintForm.ReportHeader = inPrintHeaderNote;
				curPrintForm.DivInfoDataTable = getJumpDivMaxSpeedRamp();
				curPrintForm.TourRules = myTourRules;
				if ( inHeadToHeadDef != null && inHeadToHeadDef.Length > 0 ) curPrintForm.TourRules = inHeadToHeadDef;
				curPrintForm.TourName = (String)myTourRow["Name"];

				curPrintForm.ShowDataTable = inEventRegDataTable;

				curPrintForm.Print();

			}
		}

		public void PrintTrickForm( DataTable inEventRegDataTable, String inPrintHeaderNote, String inHeadToHeadDef ) {
			PrintTrickFormDialog curDialog = new PrintTrickFormDialog();
			if ( curDialog.ShowDialog() != DialogResult.OK ) return;

			String curPrintReport = curDialog.ReportName;

			if ( curPrintReport.Equals( "TrickOfficialForm" ) ) {
				PrintTrickJudgeForm curPrintForm = new PrintTrickJudgeForm();
				curPrintForm.PrintLandscape = true;
				curPrintForm.ReportHeader = inPrintHeaderNote;
				curPrintForm.DivInfoDataTable = getTrickDivList();
				curPrintForm.TourName = (String)myTourRow["Name"];
				curPrintForm.TourRules = myTourRules;
				if ( inHeadToHeadDef != null && inHeadToHeadDef.Length > 0 ) curPrintForm.TourRules = inHeadToHeadDef;
				curPrintForm.TourRounds = Convert.ToInt32( myTourRow["TrickRounds"] );
				curPrintForm.NumJudges = 3;
				curPrintForm.ShowDataTable = inEventRegDataTable;
				curPrintForm.Print();

			} else if ( curPrintReport.Equals( "TrickTimingForm" ) ) {
				PrintTrickTimingForm curPrintForm = new PrintTrickTimingForm();
				curPrintForm.PrintLandscape = true;
				curPrintForm.ReportHeader = inPrintHeaderNote;
				curPrintForm.DivInfoDataTable = getTrickDivList();
				curPrintForm.TourName = (String)myTourRow["Name"];
				curPrintForm.TourRules = myTourRules;
				if ( inHeadToHeadDef != null && inHeadToHeadDef.Length > 0 ) curPrintForm.TourRules = inHeadToHeadDef;
				curPrintForm.TourRounds = Convert.ToInt32( myTourRow["TrickRounds"] );
				curPrintForm.ShowDataTable = inEventRegDataTable;
				curPrintForm.Print();

			} else if ( curPrintReport.Equals( "TrickSkierSpecForm" ) ) {
				PrintTrickSkierSpecForm curPrintForm = new PrintTrickSkierSpecForm();
				curPrintForm.PrintLandscape = true;
				curPrintForm.ReportHeader = inPrintHeaderNote;
				curPrintForm.DivInfoDataTable = getTrickDivList();
				curPrintForm.TourName = (String)myTourRow["Name"];
				curPrintForm.TourRules = myTourRules;
				if ( inHeadToHeadDef != null && inHeadToHeadDef.Length > 0 ) curPrintForm.TourRules = inHeadToHeadDef;
				curPrintForm.TourRounds = Convert.ToInt32( myTourRow["TrickRounds"] );
				curPrintForm.ShowDataTable = inEventRegDataTable;
				curPrintForm.Print();
			}
		}

		private DataTable getTrickDivList() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName " );
			curSqlStmt.Append( "FROM CodeValueList AS L1 " );
			curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getSlalomDivMaxMinSpeed() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName, L2.CodeValue AS MaxSpeed, L3.CodeValue AS MinSpeed " );
			curSqlStmt.Append( "FROM CodeValueList AS L1 " );
			curSqlStmt.Append( "INNER JOIN CodeValueList AS L2 ON L2.ListCode = L1.ListCode AND L2.ListName IN ('AWSASlalomMax', 'IwwfSlalomMax', 'NcwsaSlalomMax') " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS L3 ON L3.ListCode = L1.ListCode AND L3.ListName IN ('IwwfSlalomMin', 'NcwsaSlalomMin') " );
			curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getJumpDivMaxSpeedRamp() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName, L2.CodeValue AS MaxSpeed, L3.CodeValue AS RampHeight " );
			curSqlStmt.Append( "FROM CodeValueList AS L1 " );
			curSqlStmt.Append( "INNER JOIN CodeValueList AS L2 ON L2.ListCode = L1.ListCode AND L2.ListName IN ('AWSAJumpMax', 'IwwfJumpMax', 'NcwsaJumpMax') " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS L3 ON L3.ListCode = L1.ListCode AND L3.ListName IN ('AWSARampMax', 'IwwfRampMax', 'NcwsaRampMax') " );
			curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

	}
}
