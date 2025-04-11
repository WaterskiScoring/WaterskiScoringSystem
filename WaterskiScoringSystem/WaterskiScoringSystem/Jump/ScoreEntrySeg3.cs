using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Slalom;

namespace WaterskiScoringSystem.Jump {
	public partial class ScoreEntrySeg3 : Form {
		#region instance variables 
		private bool isAddRecapRowInProg = false;
		private bool isRecapRowEnterHandled = false;
		private bool isDataModified;
		private bool isLoadInProg = false;
		private bool isOrderByRoundActive = false;

		private Decimal myMetericFactor = 3.2808M;
		private Decimal myMeterDistTol;
		private Decimal myMeterZeroTol;
		private Decimal myJumpHeight;
		private Decimal myMeter6Tol;

		private int myEventRegViewIdx = 0;
		private int myStartCellIndex;
		private int myBoatListIdx = 0;
		private Int16 myMaxSpeed;
		private Int16 myNumJudges;

		private String myTitle = "";
		private String myModCellValue;
		private String myOrigCellValue;
		private String mySortCommand = "";
		private String myFilterCmd = "";
		private String myPrevEventGroup = "";
		private String mySkierBoatPathSelect = "";
		private String myDriverMemberId = "";

		private int mySkierRunCount = 0;
		private int myPassRunCount = 0;
		private DateTime myEventStartTime;
		private DateTime myEventDelayStartTime;
		private int myEventDelaySeconds = 0;

		private DataGridViewRow myRecapRow;
		private DataRow myScoreRow;
		private DataRow myBoatPathDataRow;

		private DataTable myTourEventRegDataTable;
		private DataTable myScoreDataTable;
		private DataTable myRecapDataTable;

		private List<String> myCompletedNotices = new List<String>();

		private SortDialogForm sortDialogForm;
		private FilterDialogForm filterDialogForm;
		private EventGroupSelect eventGroupSelect;
		private SkierDoneReason skierDoneReasonDialogForm;
		private RerideReason rerideReasonDialogForm;
		private CheckOfficials myCheckOfficials;

		private TourProperties myTourProperties;
		private CalcNops appNopsCalc;
		private JumpCalc myJumpCalc;
		private List<Common.ScoreEntry> ScoreList = new List<Common.ScoreEntry>();
		private PrintDocument myPrintDoc;
		private DataGridViewPrinter myPrintDataGrid;

		private JumpSetAnalysis myJumpSetAnalysis;

		private String[] loadBoatPathArgs = null;
		#endregion

		public ScoreEntrySeg3() {
			InitializeComponent();

			appNopsCalc = CalcNops.Instance;
			appNopsCalc.LoadDataForTour();
			ScoreList.Add( new Common.ScoreEntry( "Jump", 0, "", 0 ) );
			TimeInTolImg.DisplayIndex = 17;

			jumpRecapDataGridView.AutoGenerateColumns = false;
		}

		public Int16 NumJudges {
			get {
				return myNumJudges;
			}
			set {
				//jumpRecapDataGridView.Width = 988;
				jumpRecapDataGridView.Width = 988 + 75;
				myNumJudges = value;
				if ( myNumJudges == 0 ) {
					jumpRecapDataGridView.Visible = true;
					ScoreFeetRecap.ReadOnly = false;
					ScoreMetersRecap.ReadOnly = false;
					ScoreTriangleRecap.Visible = false;

					Meter6Recap.Visible = false;
					Meter5Recap.Visible = false;
					Meter4Recap.Visible = false;
					Meter3Recap.Visible = false;
					Meter2Recap.Visible = false;
					Meter1Recap.Visible = false;
					jumpRecapDataGridView.Width += -285;
				
				} else if ( myNumJudges == 3 ) {
					ScoreFeetRecap.ReadOnly = true;
					ScoreMetersRecap.ReadOnly = true;
					ScoreTriangleRecap.Visible = true;

					Meter6Recap.Visible = false;
					Meter5Recap.Visible = false;
					Meter4Recap.Visible = false;
					Meter3Recap.Visible = true;
					Meter2Recap.Visible = true;
					Meter1Recap.Visible = true;
					jumpRecapDataGridView.Width += -120;
				
				} else {
					ScoreFeetRecap.ReadOnly = true;
					ScoreMetersRecap.ReadOnly = true;
					ScoreTriangleRecap.Visible = true;

					Meter6Recap.Visible = true;
					Meter5Recap.Visible = true;
					Meter4Recap.Visible = true;
					Meter3Recap.Visible = true;
					Meter2Recap.Visible = true;
					Meter1Recap.Visible = true;
				}
				myStartCellIndex = jumpRecapDataGridView.Columns["ResultsRecap"].Index;

				if ( myNumJudges > 0 ) {
					DataTable curTolDataTable = JumpEventData.getDataForMeters();
					if ( curTolDataTable.Rows.Count > 0 ) {
						DataRow myTolRow = curTolDataTable.Select( "ListName = 'JumpTriangle'" )[0];
						myMeterDistTol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"] );
						myTolRow = curTolDataTable.Select( "ListName = 'JumpTriangleZero'" )[0];
						myMeterZeroTol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"] );
						myTolRow = curTolDataTable.Select( "ListName = 'JumpMeter6Tol'" )[0];
						myMeter6Tol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"] );

						bool isAllDataValid = true;
						myJumpCalc = new JumpCalc( JumpEventData.mySanctionNum );
						if ( myJumpCalc.distAtoB > 0 ) {
							isAllDataValid = myJumpCalc.ValidateMeterSetup();
							if ( isAllDataValid ) {
								if ( myMeterZeroTol < myJumpCalc.TriangleZero ) {
									jumpRecapDataGridView.Visible = false;
									isAllDataValid = false;
									MessageBox.Show( "Setup triangle at jump is not within allowed tolerance"
										+ "\nCurrent calculated triangle at jump is " + myJumpCalc.TriangleZero.ToString( "#0.00" )
										+ "\nAllowed tolerance for tournament class " + JumpEventData.myTourClass + " is " + myMeterZeroTol.ToString( "#0.00" )
										);
									if ( myMeterZeroTol < myJumpCalc.Triangle15M ) {
										jumpRecapDataGridView.Visible = false;
										isAllDataValid = false;
										MessageBox.Show( "Setup triangle at 15 meter timing buoy is not within allowed tolerance"
											+ "\nCurrent calculated triangle at 15meter timing buoy is " + myJumpCalc.Triangle15M.ToString( "#0.00" )
											+ "\nAllowed tolerance for tournament class " + JumpEventData.myTourClass + " is " + myMeterZeroTol.ToString( "#0.00" )
											);
									}
								} else {
									if ( myMeterZeroTol < myJumpCalc.Triangle15M ) {
										jumpRecapDataGridView.Visible = false;
										isAllDataValid = false;
										MessageBox.Show( "Setup triangle at 15 meter timing buoy is not within allowed tolerance"
											+ "\nCurrent calculated triangle at 15meter timing buoy is " + myJumpCalc.Triangle15M.ToString( "#0.00" )
											+ "\nAllowed tolerance for tournament class " + JumpEventData.myTourClass + " is " + myMeterZeroTol.ToString( "#0.00" )
											);
									}
								}
							} else {
								jumpRecapDataGridView.Visible = false;
								isAllDataValid = false;
								MessageBox.Show( "Jump meter setup has not been successfully completed."
									+ "\n Setup must be complete before distances can be calculated."
									);
							}
						} else {
							jumpRecapDataGridView.Visible = false;
							isAllDataValid = false;
							MessageBox.Show( "Jump meter setup has not been successfully completed."
								+ "\n Setup must be complete before distances can be calculated."
								);
						}
					} else {
						myNumJudges = 0;
						jumpRecapDataGridView.Visible = true;
						ScoreFeetRecap.ReadOnly = false;
						ScoreMetersRecap.ReadOnly = false;
						ScoreTriangleRecap.Visible = false;

						Meter6Recap.Visible = false;
						Meter5Recap.Visible = false;
						Meter4Recap.Visible = false;
						Meter3Recap.Visible = false;
						Meter2Recap.Visible = false;
						Meter1Recap.Visible = false;
						jumpRecapDataGridView.Width = 868;
						jumpRecapDataGridView.Width += -285;
						JumpCalcMetersCB.Checked = false;
						JumpCalcVideoCB.Checked = false;
					}
				}
				myTourProperties.JumpEntryNumJudges = myNumJudges.ToString();

			}
		}

		private void ScoreEntry_Load( object sender, EventArgs e ) {
			if ( !( JumpEventData.setEventData() ) ) {
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 15;
				curTimerObj.Tick += new EventHandler( CloseWindowTimer );
				curTimerObj.Start();
				return;
			}
			
			Cursor.Current = Cursors.WaitCursor;
			if ( Properties.Settings.Default.JumpEntry_Width > 0 ) this.Width = Properties.Settings.Default.JumpEntry_Width;
			if ( Properties.Settings.Default.JumpEntry_Height > 0 ) this.Height = Properties.Settings.Default.JumpEntry_Height;
			if ( Properties.Settings.Default.JumpEntry_Location.X > 0
				&& Properties.Settings.Default.JumpEntry_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.JumpEntry_Location;
			}

			myTitle = this.Text;
			TeamCode.Visible = false;
			if ( JumpEventData.isCollegiateEvent() ) TeamCode.Visible = true;

			myTourProperties = TourProperties.Instance;
			mySortCommand = getSortCommand();

			ResizeNarrow.Visible = false;
			ResizeNarrow.Enabled = false;
			ResizeWide.Visible = true;
			ResizeWide.Enabled = true;
			ResizeNarrow.Location = ResizeWide.Location;

			EventRunInfoLabel.Text = "   Event Start:\n" + "   Event Delay:\n" + "Skiers, Passes:";
			EventRunInfoData.Text = "";
			EventRunPerfLabel.Text = "Event Duration:\n" + "Mins Per Skier:\n" + " Mins Per Pass:";
			EventRunPerfData.Text = "";
			EventDelayReasonTextBox.Visible = false;
			EventDelayReasonLabel.Visible = false;
			StartTimerButton.Visible = false;
			PauseTimerButton.Visible = true;
			StartTimerButton.Location = PauseTimerButton.Location;

			String[] curList = { "SkierName", "Div", "DivOrder", "EventGroup", "RunOrder", "TeamCode", "EventClass"
					, "ReadyForPlcmt", "RankingScore", "RankingRating", "HCapBase", "HCapScore", "JumpHeight" };
			sortDialogForm = new SortDialogForm();
			sortDialogForm.ColumnListArray = curList;

			filterDialogForm = new Common.FilterDialogForm();
			filterDialogForm.ColumnListArray = curList;

			eventGroupSelect = new EventGroupSelect();
			rerideReasonDialogForm = new RerideReason();
			skierDoneReasonDialogForm = new SkierDoneReason();

			//Load round selection list based on number of rounds specified for the tournament
			roundActiveSelect.SelectList_LoadHorztl( JumpEventData.myTourRow["JumpRounds"].ToString(), roundActiveSelect_Click );
			roundActiveSelect.RoundValue = "1";

			//Load jump speed selection list
			JumpSpeedSelect.SelectList_Load( JumpSpeedSelect_Change );

			//Load ramp height selection list
			RampHeightSelect.SelectList_Load( RampHeightSelect_Change );

			//Load skier boat path selection list
			SkierBoatPathSelect.SelectList_Load( SkierBoatPathSelect_Change );

			myCheckOfficials = new CheckOfficials();

			scoreEventClass.DataSource = JumpEventData.mySkierClassList.DropdownList;
			scoreEventClass.DisplayMember = "ItemName";
			scoreEventClass.ValueMember = "ItemValue";

			//Setup for meter input if using meters to calculate distances
			NumJudges = Convert.ToInt16( myTourProperties.JumpEntryNumJudges );
			if ( NumJudges > 0 ) {
				JumpCalcMetersCB.Checked = true;
				JumpCalcVideoCB.Checked = false;
			} else {
				JumpCalcMetersCB.Checked = false;
				JumpCalcVideoCB.Checked = true;
			}

			//Get list of approved tow boats
			getApprovedTowboats();
			approvedBoatSelectGroupBox.Visible = false;
			listApprovedBoatsDataGridView.Visible = false;

			//Retrieve and load tournament event entries
			loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

			if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
				LiveWebLabel.Visible = true;
			} else {
				LiveWebLabel.Visible = false;
			}

			navWaterSkiConnect_Click( null, null );

			Cursor.Current = Cursors.Default;
		}
		private void CloseWindowTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( CloseWindowTimer );
			this.Close();
		}

		private void ScoreEntry_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.JumpEntry_Width = this.Size.Width;
				Properties.Settings.Default.JumpEntry_Height = this.Size.Height;
				Properties.Settings.Default.JumpEntry_Location = this.Location;
			}
			if ( myPassRunCount <= 0 ) return;

			if ( StartTimerButton.Visible ) StartTimerButton_Click( null, null );
			int curHours, curMins, curSeconds, curTimeDiff;
			String curMethodName = "Jump:ScoreEntry:FormClosed: ";
			try {
				curTimeDiff = Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventStartTime ) ).TotalSeconds ) - myEventDelaySeconds;
			} catch {
				myEventStartTime = DateTime.Now;
				EventRunInfoData.Text = "";
				EventRunPerfData.Text = "";
				curTimeDiff = Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventStartTime ) ).TotalSeconds ) - myEventDelaySeconds;
			}
			Log.WriteFile( curMethodName + ":    Event Start=" + myEventStartTime.ToString( "MM/dd/yyyy HH:mm" ) );
			Log.WriteFile( curMethodName + ":      Event End=" + DateTime.Now.ToString( "MM/dd/yyyy HH:mm" ) );
			curHours = Math.DivRem( curTimeDiff, 3600, out curSeconds );
			curMins = Math.DivRem( curSeconds, 60, out curSeconds );
			Log.WriteFile( curMethodName + ": Event Duration=" + curHours.ToString( "00" ) + ":" + curMins.ToString( "00" ) );
			curHours = Math.DivRem( myEventDelaySeconds, 3600, out curSeconds );
			curMins = Math.DivRem( curSeconds, 60, out curSeconds );
			Log.WriteFile( curMethodName + ":    Event Delay=" + curHours.ToString( "00" ) + ":" + curMins.ToString( "00" ) );
			Log.WriteFile( curMethodName + ":         Skiers=" + mySkierRunCount.ToString( "##0" ) );
			Log.WriteFile( curMethodName + ":         Passes=" + myPassRunCount.ToString( "##0" ) );
			curMins = Math.DivRem( ( curTimeDiff / mySkierRunCount ), 60, out curSeconds );
			Log.WriteFile( curMethodName + ": Mins Per Skier=" + ( curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" ) ) );
			curMins = Math.DivRem( ( curTimeDiff / myPassRunCount ), 60, out curSeconds );
			Log.WriteFile( curMethodName + ": Mins Per Pass =" + ( curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" ) ) );
		}

		private void ScoreEntry_FormClosing( object sender, FormClosingEventArgs e ) {
			if ( isDataModified ) {
				saveScore();
			}
			e.Cancel = false;
		}

		private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
			MessageBox.Show( "Error happened " + e.Context.ToString() + "\n Exception Message: " + e.Exception.Message );
			if ( ( e.Exception ) is ConstraintException ) {
				DataGridView view = (DataGridView)sender;
				view.Rows[e.RowIndex].ErrorText = "an error";
				e.ThrowException = false;
			}
		}

		private void ResizeNarrow_Click( object sender, EventArgs e ) {
			ResizeNarrow.Visible = false;
			ResizeNarrow.Enabled = false;
			ResizeWide.Visible = true;
			ResizeWide.Enabled = true;

			TourEventRegDataGridView.Width = 265;
		}

		private void ResizeWide_Click( object sender, EventArgs e ) {
			ResizeNarrow.Visible = true;
			ResizeNarrow.Enabled = true;
			ResizeWide.Visible = false;
			ResizeWide.Enabled = false;

			if ( TeamCode.Visible ) {
				TourEventRegDataGridView.Width = 585;
			} else {
				TourEventRegDataGridView.Width = 535;
			}
		}

		private void navSaveItem_Click( object sender, EventArgs e ) {
			activeSkierName.Focus();
			jumpRecapDataGridView.EndEdit();
			jumpRecapDataGridView.Focus();
			try {
				if ( saveScore() ) {
					isDataModified = false;
				}
			} catch ( Exception excp ) {
				MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
			}
		}

		private bool saveScore() {
			String curMethodName = "Jump:ScoreEntrySeg3:saveScore: ";
			myScoreDataTable = null;

			if ( myRecapRow == null || myRecapRow.Index < 0 || TourEventRegDataGridView.Rows.Count == 0 ) {
				isDataModified = false;
				return false;
			}

			if ( StartTimerButton.Visible ) StartTimerButton_Click( null, null );

			int curIdx = myRecapRow.Index;
			String curUpdateStatus = HelperFunctions.getViewRowColValue( myRecapRow, "Updated", "N" );
			if ( !( curUpdateStatus.ToUpper().Equals( "Y" ) ) ) {
				isDataModified = false;
				return false;
			}

			String curSanctionId = HelperFunctions.getViewRowColValue( myRecapRow, "SanctionIdRecap", "" );
			String curMemberId = HelperFunctions.getViewRowColValue( myRecapRow, "MemberIdRecap", "" );
			String curAgeGroup = HelperFunctions.getViewRowColValue( myRecapRow, "AgeGroupRecap", "" );
			byte curRound = Convert.ToByte( HelperFunctions.getViewRowColValue( myRecapRow, "RoundRecap", "0" ) );
			Int16 curPassNum = Convert.ToInt16( (String)myRecapRow.Cells["PassNumRecap"].Value );

			// Update skier total score for round
			bool saveResults = saveSkierScore();
			if ( saveResults ) myScoreDataTable = getSkierScoreByRound( curMemberId, curAgeGroup, curRound );
			if ( myScoreDataTable == null || myScoreDataTable.Rows.Count <= 0 ) {
				MessageBox.Show( String.Format( "{0}Jump score update failed for Skier {1} {2} Round {3}" +
					", no score entry found for skier and round, terminating update"
					, curMethodName, curMemberId, curAgeGroup, curRound ) );
				return false;
			}

			myScoreRow = myScoreDataTable.Rows[0];
			
			if ( JumpEventData.isIwwfEvent() ) {
				TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = scoreMetersTextBox.Text;
			} else {
				TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = scoreFeetTextBox.Text;
			}
			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = this.hcapScoreTextBox.Text;


			//  Update skier score for current pass
			saveResults = saveSkierRecapScore();
			if ( !saveResults ) return false;

			DataRow curRecapRow = getSkierRecapEntry( curMemberId, curAgeGroup, curRound, curPassNum );
			if ( curRecapRow == null ) {
				MessageBox.Show( String.Format( "{0}Jump recap score update failed for Skier {1} {2} Round {3}" +
					", no recap entries found for skier and round, terminating update"
					, curMethodName, curMemberId, curAgeGroup, curRound ) );
				return false;
			}

			myRecapRow.Cells["Updated"].Value = "N";
			myRecapRow.Cells["PKRecap"].Value = ( (Int64)curRecapRow["PK"] ).ToString();

			refreshScoreSummaryWindow();

			int curSeconds = 0, curMins = 0, curHours, curTimeDiff = 0;
			if ( myPassRunCount > 1 ) {
				try {
					curTimeDiff = Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventStartTime ) ).TotalSeconds ) - myEventDelaySeconds;
				} catch {
					myEventStartTime = DateTime.Now;
					EventRunInfoData.Text = "";
					EventRunPerfData.Text = "";
					curTimeDiff = Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventStartTime ) ).TotalSeconds ) - myEventDelaySeconds;
				}
				curHours = Math.DivRem( curTimeDiff, 3600, out curSeconds );
				curMins = Math.DivRem( curSeconds, 60, out curSeconds );
				String curEventDuration = curHours.ToString( "00" ) + ":" + curMins.ToString( "00" );
				curMins = Math.DivRem( ( curTimeDiff / mySkierRunCount ), 60, out curSeconds );
				String curMinsPerSkier = curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" );
				curMins = Math.DivRem( ( curTimeDiff / myPassRunCount ), 60, out curSeconds );
				String curMinsPerPass = curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" );
				EventRunPerfData.Text = curEventDuration + "\n" + curMinsPerSkier + "\n" + curMinsPerPass;

			} else {
				myEventStartTime = DateTime.Now;
				EventRunInfoData.Text = "";
				EventRunPerfData.Text = "";
			}
			curHours = Math.DivRem( myEventDelaySeconds, 3600, out curSeconds );
			curMins = Math.DivRem( curSeconds, 60, out curSeconds );
			EventRunInfoData.Text = myEventStartTime.ToString( "HH:mm" )
				+ "\n" + curHours.ToString( "00" ) + ":" + curMins.ToString( "00" )
				+ "\n" + mySkierRunCount.ToString( "##0" )
				+ ", " + myPassRunCount.ToString( "##0" );

			transmitExternalScoreboard( curSanctionId, curMemberId, curAgeGroup, curRound );

			// Check to see if score is equal to or great than divisions current record score
			String curCheckRecordMsg = JumpEventData.myCheckEventRecord.checkRecordJump( curAgeGroup, scoreFeetTextBox.Text, scoreMetersTextBox.Text, (byte)myScoreRow["SkiYearAge"], (string)myScoreRow["Gender"] );
			if ( curCheckRecordMsg == null ) curCheckRecordMsg = "";
			if ( curCheckRecordMsg.Length > 1 ) MessageBox.Show( curCheckRecordMsg );

			return true;
		}

		// Update skier total score for round
		private bool saveSkierScore() {
			String curMethodName = "Jump:ScoreEntrySeg3:saveSkierScore: ";
			int rowsProc = 0;

			String curSanctionId = HelperFunctions.getViewRowColValue( myRecapRow, "SanctionIdRecap", "" );
			String curMemberId = HelperFunctions.getViewRowColValue( myRecapRow, "MemberIdRecap", "" );
			String curAgeGroup = HelperFunctions.getViewRowColValue( myRecapRow, "AgeGroupRecap", "" );
			String curTeamCode = TeamCodeTextBox.Text;
			String curEventClass = (String)scoreEventClass.SelectedValue;
			byte curRound = Convert.ToByte( HelperFunctions.getViewRowColValue( myRecapRow, "RoundRecap", "0" ) );

			Int64 curScorePK = 0;
			myScoreDataTable = getSkierScoreByRound( curMemberId, curAgeGroup, curRound );
			if ( myScoreDataTable.Rows.Count > 0 ) {
				myScoreRow = myScoreDataTable.Rows[0];
				curScorePK = (Int64)myScoreRow["PK"];
			} else {
				myScoreRow = null;
				curScorePK = -1;
			}

			String curScoreFeet = scoreFeetTextBox.Text;
			if ( curScoreFeet.Length == 0 ) curScoreFeet = "Null";
			String curScoreMeters = scoreMetersTextBox.Text;
			if ( curScoreMeters.Length == 0 ) curScoreMeters = "Null";
			decimal curNopsScoreNum = Convert.ToDecimal( nopsScoreTextBox.Text );
			if ( curNopsScoreNum > 99999m ) {
				curNopsScoreNum = 99999m;
				scoreMetersTextBox.Text = curNopsScoreNum.ToString( "####0.0" ); ;
			}
			String curNopsScore = curNopsScoreNum.ToString("####0.0");
			if ( curNopsScore.Length == 0 ) curNopsScore = "Null";
			String curRampHeight = RampHeightSelect.CurrentValue.ToString( "0.0" );
			if ( curRampHeight.Length == 0 ) curRampHeight = "Null";
			String curBoatSpeed = BoatSpeedTextBox.Text;
			if ( curBoatSpeed.Length == 0 ) curBoatSpeed = "Null";

			String curTourBoat = calcBoatCodeFromDisplay( TourBoatTextbox.Text );

			String curScoreNote = "";
			try {
				curScoreNote = noteTextBox.Text;
				curScoreNote = curScoreNote.Replace( "'", "''" );
			} catch {
				curScoreNote = "";
			}

			String curStatus = setEventRegRowStatus( TourEventRegDataGridView.Rows[myEventRegViewIdx] );

			if ( curBoatSpeed.ToLower().Equals( "null" ) || curScoreFeet.ToLower().Equals( "null" ) || curScoreMeters.ToLower().Equals( "null" ) ) {
				isDataModified = false;
				return false;
			}

			try {
				StringBuilder curSqlStmt = new StringBuilder( "" );
				if ( curScorePK > 0 ) {
					curSqlStmt.Append( "Update JumpScore Set " );
					curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
					curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
					curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
					curSqlStmt.Append( ", Round = " + curRound );
					curSqlStmt.Append( ", EventClass = '" + curEventClass + "'" );
					curSqlStmt.Append( ", BoatSpeed = " + curBoatSpeed );
					curSqlStmt.Append( ", RampHeight = " + curRampHeight );
					curSqlStmt.Append( ", ScoreFeet = " + curScoreFeet );
					curSqlStmt.Append( ", ScoreMeters = " + curScoreMeters );
					curSqlStmt.Append( ", NopsScore = " + curNopsScore );
					curSqlStmt.Append( ", Boat = '" + curTourBoat + "'" );
					curSqlStmt.Append( ", Status = '" + curStatus + "'" );
					curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
					curSqlStmt.Append( ", Note = '" + curScoreNote + "'" );
					curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt.Append( "Insert JumpScore ( " );
					curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, EventClass, BoatSpeed, RampHeight" );
					curSqlStmt.Append( ", ScoreFeet, ScoreMeters, NopsScore, Status, Boat, LastUpdateDate, InsertDate, Note" );
					curSqlStmt.Append( ") Values ( " );
					curSqlStmt.Append( "'" + curSanctionId + "'" );
					curSqlStmt.Append( ", '" + curMemberId + "'" );
					curSqlStmt.Append( ", '" + curAgeGroup + "'" );
					curSqlStmt.Append( ", " + curRound );
					curSqlStmt.Append( ", '" + curEventClass + "'" );
					curSqlStmt.Append( ", " + curBoatSpeed );
					curSqlStmt.Append( ", " + curRampHeight );
					curSqlStmt.Append( ", " + curScoreFeet );
					curSqlStmt.Append( ", " + curScoreMeters );
					curSqlStmt.Append( ", " + curNopsScore );
					curSqlStmt.Append( ", '" + curTourBoat + "'" );
					curSqlStmt.Append( ", '" + curStatus + "'" );
					curSqlStmt.Append( ", GETDATE(), GETDATE()" );
					curSqlStmt.Append( ", '" + curScoreNote + "'" );
					curSqlStmt.Append( ")" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				}
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
				if ( rowsProc > 0 ) return true;
				return false;

			} catch ( Exception excp ) {
				String curMsg = String.Format( "Jump score update failed for Skier {0} {1} Round {2}, exception encountered: {3}", curMemberId, curAgeGroup, curRound, excp.Message );
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				isDataModified = false;
				return false;
			}
		}

		private bool saveSkierRecapScore() {
			String curMethodName = "Jump:ScoreEntrySeg3:saveSkierRecapScore: ";
			int rowsProc = 0;

			String curSanctionId = HelperFunctions.getViewRowColValue( myRecapRow, "SanctionIdRecap", "" );
			String curMemberId = HelperFunctions.getViewRowColValue( myRecapRow, "MemberIdRecap", "" );
			String curAgeGroup = HelperFunctions.getViewRowColValue( myRecapRow, "AgeGroupRecap", "" );
			String curTeamCode = TeamCodeTextBox.Text;
			String curEventClass = (String)scoreEventClass.SelectedValue;
			byte curRound = Convert.ToByte( HelperFunctions.getViewRowColValue( myRecapRow, "RoundRecap", "0" ) );
			String curRampHeight = RampHeightSelect.CurrentValue.ToString( "0.0" );
			if ( curRampHeight.Length == 0 ) curRampHeight = "Null";

			Int64 curRecapPK = Convert.ToInt64( myRecapRow.Cells["PKRecap"].Value );
			Int16 curPassNum = Convert.ToInt16( (String)myRecapRow.Cells["PassNumRecap"].Value );

			String curMeter1 = HelperFunctions.getViewRowColValue( myRecapRow, "Meter1Recap", "Null" );
			String curMeter2 = HelperFunctions.getViewRowColValue( myRecapRow, "Meter2Recap", "Null" );
			String curMeter3 = HelperFunctions.getViewRowColValue( myRecapRow, "Meter3Recap", "Null" );
			String curMeter4 = HelperFunctions.getViewRowColValue( myRecapRow, "Meter4Recap", "Null" );
			String curMeter5 = HelperFunctions.getViewRowColValue( myRecapRow, "Meter5Recap", "Null" );
			String curMeter6 = HelperFunctions.getViewRowColValue( myRecapRow, "Meter6Recap", "Null" );

			String curResults = HelperFunctions.getViewRowColValue( myRecapRow, "ResultsRecap", "Jump" );
			String curScoreTriangle = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreTriangleRecap", "Null" );
			String curBoatSpeed = HelperFunctions.getViewRowColValue( myRecapRow, "BoatSpeedRecap", "Null" );
			String curScoreFeet = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreFeetRecap", "Null" );
			String curScoreMeters = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreMetersRecap", "Null" );
			String curReride = HelperFunctions.getViewRowColValue( myRecapRow, "RerideRecap", "Null" );
			String curRerideIfBest = HelperFunctions.getViewRowColValue( myRecapRow, "RerideIfBestRecap", "N" );
			String curRerideCanImprove = HelperFunctions.getViewRowColValue( myRecapRow, "RerideCanImproveRecap", "N" );

			String curBoatSplitTime = HelperFunctions.getViewRowColValue( myRecapRow, "BoatSplitTimeRecap", "Null" );
			String curBoatSplitTime2 = HelperFunctions.getViewRowColValue( myRecapRow, "BoatSplitTime2Recap", "Null" );
			String curBoatEndTime = HelperFunctions.getViewRowColValue( myRecapRow, "BoatEndTimeRecap", "Null" );

			String curSplit52TimeTol = HelperFunctions.getViewRowColValue( myRecapRow, "Split52TimeTolRecap", "0" );
			String curSplit82TimeTol = HelperFunctions.getViewRowColValue( myRecapRow, "Split82TimeTolRecap", "0" );
			String curSplit41TimeTol = HelperFunctions.getViewRowColValue( myRecapRow, "Split41TimeTolRecap", "0" );

			String curTimeInTol = HelperFunctions.getViewRowColValue( myRecapRow, "TimeInTolRecap", "N" );
			String curScoreProt = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreProtRecap", "N" );
			String curReturnToBase = HelperFunctions.getViewRowColValue( myRecapRow, "ReturnToBaseRecap", "N" );

			String curSkierBoatPath = HelperFunctions.getViewRowColValue( myRecapRow, "SkierBoatPathRecap", SkierBoatPathSelect.DefaultValue );
			String curRerideReason = HelperFunctions.getViewRowColValue( myRecapRow, "RerideReasonRecap", "" );
			curRerideReason = curRerideReason.Replace( "'", "''" );
			String curNote = HelperFunctions.getViewRowColValue( myRecapRow, "NoteRecap", "" );
			curNote = curNote.Replace( "'", "''" );

			try {
				//RerideIfBest, RerideCanImprove
				StringBuilder curSqlStmt = new StringBuilder( "" );
				if ( curRecapPK > 0 ) {
					curSqlStmt.Append( "Update JumpRecap Set " );
					curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
					curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
					curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
					curSqlStmt.Append( ", Round = " + curRound );
					curSqlStmt.Append( ", PassNum = " + curPassNum );
					curSqlStmt.Append( ", Reride = '" + curReride + "'" );
					curSqlStmt.Append( ", BoatSpeed = " + curBoatSpeed );
					curSqlStmt.Append( ", SkierBoatPath = '" + curSkierBoatPath + "'" );
					curSqlStmt.Append( ", RampHeight = " + curRampHeight );
					curSqlStmt.Append( ", Meter1 = " + curMeter1 );
					curSqlStmt.Append( ", Meter2 = " + curMeter2 );
					curSqlStmt.Append( ", Meter3 = " + curMeter3 );
					curSqlStmt.Append( ", Meter4 = " + curMeter4 );
					curSqlStmt.Append( ", Meter5 = " + curMeter5 );
					curSqlStmt.Append( ", Meter6 = " + curMeter6 );
					curSqlStmt.Append( ", ScoreTriangle = " + curScoreTriangle );
					curSqlStmt.Append( ", ScoreFeet = " + curScoreFeet );
					curSqlStmt.Append( ", ScoreMeters = " + curScoreMeters );
					curSqlStmt.Append( ", BoatSplitTime = " + curBoatSplitTime );
					curSqlStmt.Append( ", BoatSplitTime2 = " + curBoatSplitTime2 );
					curSqlStmt.Append( ", BoatEndTime = " + curBoatEndTime );
					curSqlStmt.Append( ", TimeInTol = '" + curTimeInTol + "'" );
					curSqlStmt.Append( ", ScoreProt = '" + curScoreProt + "'" );
					curSqlStmt.Append( ", ReturnToBase = '" + curReturnToBase + "'" );
					curSqlStmt.Append( ", RerideIfBest = '" + curRerideIfBest + "'" );
					curSqlStmt.Append( ", RerideCanImprove = '" + curRerideCanImprove + "'" );
					curSqlStmt.Append( ", Split52TimeTol = " + curSplit52TimeTol );
					curSqlStmt.Append( ", Split82TimeTol = " + curSplit82TimeTol );
					curSqlStmt.Append( ", Split41TimeTol = " + curSplit41TimeTol );
					curSqlStmt.Append( ", Results = '" + curResults + "'" );
					curSqlStmt.Append( ", LastUpdateDate = getdate()" );
					curSqlStmt.Append( ", RerideReason = '" + curRerideReason + "'" );
					curSqlStmt.Append( ", Note = '" + curNote + "' " );
					curSqlStmt.Append( " Where PK = " + curRecapPK.ToString() );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt.Append( "Insert JumpRecap ( " );
					curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, PassNum" );
					curSqlStmt.Append( ", BoatSpeed, RampHeight, Meter1, Meter2, Meter3, Meter4, Meter5, Meter6, ScoreTriangle" );
					curSqlStmt.Append( ", ScoreFeet, ScoreMeters, BoatSplitTime, BoatSplitTime2, BoatEndTime, Split52TimeTol, Split82TimeTol, Split41TimeTol" );
					curSqlStmt.Append( ", Reride, TimeInTol, ScoreProt, ReturnToBase, RerideIfBest, RerideCanImprove, SkierBoatPath, Results, LastUpdateDate, InsertDate, RerideReason, Note" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( " '" + curSanctionId + "'" );
					curSqlStmt.Append( ", '" + curMemberId + "'" );
					curSqlStmt.Append( ", '" + curAgeGroup + "'" );
					curSqlStmt.Append( ", " + curRound );
					curSqlStmt.Append( ", " + curPassNum );
					curSqlStmt.Append( ", " + curBoatSpeed );
					curSqlStmt.Append( ", " + curRampHeight );
					curSqlStmt.Append( ", " + curMeter1 );
					curSqlStmt.Append( ", " + curMeter2 );
					curSqlStmt.Append( ", " + curMeter3 );
					curSqlStmt.Append( ", " + curMeter4 );
					curSqlStmt.Append( ", " + curMeter5 );
					curSqlStmt.Append( ", " + curMeter6 );
					curSqlStmt.Append( ", " + curScoreTriangle );
					curSqlStmt.Append( ", " + curScoreFeet );
					curSqlStmt.Append( ", " + curScoreMeters );
					curSqlStmt.Append( ", " + curBoatSplitTime );
					curSqlStmt.Append( ", " + curBoatSplitTime2 );
					curSqlStmt.Append( ", " + curBoatEndTime );
					curSqlStmt.Append( ", " + curSplit52TimeTol );
					curSqlStmt.Append( ", " + curSplit82TimeTol );
					curSqlStmt.Append( ", " + curSplit41TimeTol );
					curSqlStmt.Append( ", '" + curReride + "'" );
					curSqlStmt.Append( ", '" + curTimeInTol + "'" );
					curSqlStmt.Append( ", '" + curScoreProt + "'" );
					curSqlStmt.Append( ", '" + curReturnToBase + "'" );
					curSqlStmt.Append( ", '" + curRerideIfBest + "'" );
					curSqlStmt.Append( ", '" + curRerideCanImprove + "'" );
					curSqlStmt.Append( ", '" + curSkierBoatPath + "'" );
					curSqlStmt.Append( ", '" + curResults + "'" );
					curSqlStmt.Append( ", getdate(), getdate()" );
					curSqlStmt.Append( ", '" + curRerideReason + "'" );
					curSqlStmt.Append( ", '" + curNote + "'" );
					curSqlStmt.Append( ")" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				}
				Log.WriteFile( curMethodName + "Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
				if ( rowsProc > 0 ) return true;
				return false;

			} catch ( Exception excp ) {
				String curMsg = String.Format( "{0}Jump recap score update failed for Skier {1} {2} Round {3}" +
					", exception encountered: {4}"
					, curMethodName, curMemberId, curAgeGroup, curRound, excp.Message );
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				isDataModified = false;
				return false;
			}
		}

		private String setEventRegRowStatus( DataGridViewRow curViewRow ) {
			String curStatus = "TBD";
			String curValue = HelperFunctions.getViewRowColValue( curViewRow, "Status", "TBD" );
			if ( curValue.Equals( "4-Done" ) ) curStatus = "Complete";
			if ( curValue.Equals( "2-InProg" ) ) curStatus = "InProg";
			if ( curValue.Equals( "3-Error" ) ) curStatus = "Error";
			if ( curValue.Equals( "1-TBD" ) ) curStatus = "TBD";
			return curStatus;
		}

		private void transmitExternalScoreboard( String curSanctionId, String curMemberId, String curAgeGroup, byte curRound ) {
			//, String curTeamCode, byte curRound, short curPassNum, String curStatus, String curResults, String curScoreFeet, String curScoreMeters) {
			if ( LiveWebLabel.Visible ) {
				LiveWebHandler.sendCurrentSkier( "Jump", JumpEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0 );
			}
			if ( WscHandler.isConnectActive ) {
				if ( myRecapRow == null ) return;

				if ( !( WscHandler.useJumpTimes ) ) {
					decimal curBoatSplitTime = 0, curBoatSplitTime2 = 0, curBoatEndTime = 0;
					if ( ( (String)myRecapRow.Cells["BoatSplitTimeRecap"].Value ).Length > 0 ) {
						Decimal.TryParse( (String)myRecapRow.Cells["BoatSplitTimeRecap"].Value, out curBoatSplitTime );
					}
					if ( ( (String)myRecapRow.Cells["BoatSplitTime2Recap"].Value ).Length > 0 ) {
						Decimal.TryParse( (String)myRecapRow.Cells["BoatSplitTime2Recap"].Value, out curBoatSplitTime2 );
					}
					if ( ( (String)myRecapRow.Cells["BoatEndTimeRecap"].Value ).Length > 0 ) {
						Decimal.TryParse( (String)myRecapRow.Cells["BoatEndTimeRecap"].Value, out curBoatEndTime );
					}
					WscHandler.sendJumpBoatTimes( curMemberId, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value, "Jump"
						, Convert.ToInt16( (String)myRecapRow.Cells["RoundRecap"].Value )
						, Convert.ToInt16( (String)myRecapRow.Cells["PassNumRecap"].Value )
						, Convert.ToInt16( (String)myRecapRow.Cells["BoatSpeedRecap"].Value )
						, curBoatSplitTime, curBoatSplitTime2, curBoatEndTime );
				}

				String skierFed = (String)JumpEventData.myTourRow["Federation"];
				if ( ( (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Federation"].Value ).Length > 1 ) {
					skierFed = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Federation"].Value;
				}
				WscHandler.sendAthleteScore( curMemberId
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value
					, "Jump"
					, skierFed
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["State"].Value
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value
					, Convert.ToInt16( (String)myRecapRow.Cells["RoundRecap"].Value )
					, Convert.ToInt16( (String)myRecapRow.Cells["PassNumRecap"].Value )
					, Convert.ToInt16( (String)myRecapRow.Cells["BoatSpeedRecap"].Value )
					, ""
					, (String)myRecapRow.Cells["ScoreMetersRecap"].Value );
			} else if ( !WaterskiConnectLabel.Visible ) WaterskiConnectLabel.Visible = false;
		}

		private void JumpCalcVideoCB_CheckedChanged( object sender, EventArgs e ) {
			CheckBox curControl = (CheckBox)sender;
			if ( curControl.Checked ) {
				NumJudges = 0;
				JumpCalcMetersCB.Checked = false;
			}
		}

		private void JumpCalcMetersCB_CheckedChanged( object sender, EventArgs e ) {
			CheckBox curControl = (CheckBox)sender;
			if ( curControl.Checked ) {
				//Determine required number of judges for event
				StringBuilder curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT ListCode, CodeValue, MaxValue, MinValue, CodeDesc FROM CodeValueList WHERE ListName = 'JumpJudgesNum' ORDER BY SortSeq" );
				DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					int curNumJudges = Convert.ToInt16( (Decimal)curDataTable.Rows[0]["MaxValue"] );
					//Override to 3 judges for now because 6 meter calculations are not yet supported
					if ( curNumJudges > 3 ) {
						MessageBox.Show( "Distance calculations for more than 3 judges is not currently supported. \n Defaulting to 3 meters." );
					}
					NumJudges = 3;
				} else {
					NumJudges = 3;
				}
			}
		}

		private void refreshScoreSummaryWindow() {
			// Check for open instance of selected form
			//SystemMain curMdiWindow = (SystemMain)this.Parent.Parent;
			//for ( int idx = 0; idx < curMdiWindow.MdiChildren.Length; idx++ ) {
			//    Form curForm = (Form)( curMdiWindow.MdiChildren[idx] );
			//    if ( curForm.Name.Equals( "JumpSummary" ) ) {
			//        JumpSummary curSummaryWindow = (JumpSummary)curForm;
			//        curSummaryWindow.navRefresh_Click( null, null );
			//    } else if ( curForm.Name.Equals( "JumpSummaryTeam" ) ) {
			//        JumpSummaryTeam curSummaryWindow = (JumpSummaryTeam)curForm;
			//        curSummaryWindow.navRefresh_Click( null, null );
			//    } else if ( curForm.Name.Equals( "MasterSummaryOverall" ) ) {
			//        MasterSummaryOverall curSummaryWindow = (MasterSummaryOverall)curForm;
			//        curSummaryWindow.navRefresh_Click( null, null );
			//    } else if ( curForm.Name.Equals( "MasterSummaryV2" ) ) {
			//        MasterSummaryV2 curSummaryWindow = (MasterSummaryV2)curForm;
			//        curSummaryWindow.navRefresh_Click( null, null );
			//    } else if ( curForm.Name.Equals( "MasterSummaryOverallUS" ) ) {
			//        MasterSummaryOverallUS curSummaryWindow = (MasterSummaryOverallUS)curForm;
			//        curSummaryWindow.navRefresh_Click( null, null );
			//    } else if ( curForm.Name.Equals( "MasterSummaryTeam" ) ) {
			//        MasterSummaryTeam curSummaryWindow = (MasterSummaryTeam)curForm;
			//        curSummaryWindow.navRefresh_Click( null, null );
			//    }
			//}
		}

		private void scoreEntryInprogress() {
			RampHeightSelect.Enabled = true;
			addPassButton.Enabled = true;
		}

		private void scoreEntryBegin() {
			RampHeightSelect.Enabled = true;
			addPassButton.Enabled = true;
		}

		private void deletePassButton_Click( object sender, EventArgs e ) {
			if ( jumpRecapDataGridView == null || jumpRecapDataGridView.Rows.Count == 0 ) return;

			int curRowIdx = jumpRecapDataGridView.Rows.Count - 1;
			myRecapRow = jumpRecapDataGridView.Rows[curRowIdx];
			Int64 curRecapPK = Convert.ToInt64( (String)myRecapRow.Cells["PKRecap"].Value );
			if ( curRecapPK > 0 ) {
				// Delete row that has been saved to the database
				deletePassSaved( curRowIdx, curRecapPK );

			} else {
				// Delete row that hasn't been saved to the database
				deletePassNotSaved( curRowIdx );
			}

			jumpRecapDataGridView.Select();
		}

		private void deletePassSaved( int curRowIdx, Int64 curRecapPK ) {
			String curMethodName = "Jump:ScoreEntrySeg3:deletePassSaved: ";
			String curMsg = "";
			int rowsProc = 0;
			StringBuilder curSqlStmt = new StringBuilder( "" );

			// Delete row from database
			try {
				curSqlStmt = new StringBuilder( "Delete JumpRecap Where PK = " + curRecapPK.ToString() );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + "Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

			} catch ( Exception excp ) {
				curMsg = curMethodName + "Error deleting jump pass \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			}

			if ( rowsProc <= 0 ) return;

			/*
			 * Delete row from data grid
			 * Update score
			 */
			jumpRecapDataGridView.Rows.RemoveAt( curRowIdx );
			if ( curRowIdx > 0 ) {
				// Remove row from data grid when not the first pass
				curRowIdx--;
				jumpRecapDataGridView.Rows[curRowIdx].Cells["Updated"].Value = "Y";
				myRecapRow = (DataGridViewRow)jumpRecapDataGridView.Rows[curRowIdx];
				myJumpSetAnalysis = new JumpSetAnalysis( TourEventRegDataGridView.Rows[myEventRegViewIdx], jumpRecapDataGridView.Rows, myRecapRow );

				setRecapCellsForEdit();
				// Calculate score for jump set and determine if set complete
				if ( RoundCommpleteUpdateScore() ) {
					scoreEntryInprogress();
				} else {
					addPassButton.Enabled = false;
				}
				saveScore();
				return;
			}

			/*
			 * Remove first pass row from data grid
			 * Update score by remove score record when deleting first pass
			 */
			try {
				myRecapRow = null;
				myJumpSetAnalysis = null;

				Int64 curScorePK = (Int64)myScoreRow["PK"];
				String curMemberId = (String)myScoreRow["MemberId"];
				String curAgeGroup = (String)myScoreRow["AgeGroup"];
				String curTeamCode = TeamCodeTextBox.Text;
				byte curRound = (byte)myScoreRow["Round"];
				curSqlStmt = new StringBuilder( "Delete JumpScore Where PK = " + curScorePK.ToString() );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + "Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

				transmitExternalScoreboard( JumpEventData.mySanctionNum, curMemberId, curAgeGroup, curRound );

			} catch ( Exception excp ) {
				curMsg = curMethodName + "Error deleting skier score \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			}

			myScoreRow = null;
			scoreFeetTextBox.Text = "";
			hcapScoreTextBox.Text = "";
			scoreMetersTextBox.Text = "";
			nopsScoreTextBox.Text = "";
			noteTextBox.Text = "";
			RampHeightTextBox.Text = "";
			BoatSpeedTextBox.Text = "";
			TeamCodeTextBox.Text = "";
			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = "";
			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = "";

			JumpSpeedSelect.CurrentValue = myMaxSpeed;
			scoreEntryBegin();

			setEventRegRowStatus( "1-TBD" );
			refreshScoreSummaryWindow();
			scoreEntryBegin();
		}

		private void deletePassNotSaved( int curRowIdx ) {
			String curMethodName = "Jump:ScoreEntrySeg3:deletePassNotSaved: ";

			jumpRecapDataGridView.Rows.RemoveAt( curRowIdx );
			if ( curRowIdx > 0 ) {
				// Delete pass but not the first pass
				curRowIdx--;
				myRecapRow = (DataGridViewRow)jumpRecapDataGridView.Rows[curRowIdx];
				setRecapCellsForEdit();

				// Calculate score for jump set and determine if set complete
				if ( RoundCommpleteUpdateScore() ) {
					scoreEntryInprogress();
				} else {
					addPassButton.Enabled = false;
				}
				saveScore();
				return;
			}

			/*
			 * First pass being deleted
			 * Delete score entry if previously saved
			 */
			if ( myScoreRow != null ) {
				try {
					myRecapRow = null;
					myJumpSetAnalysis = null;

					Int64 curScorePK = (Int64)myScoreRow["PK"];
					StringBuilder curSqlStmt = new StringBuilder( "Delete JumpScore Where PK = " + curScorePK.ToString() );
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

				} catch ( Exception excp ) {
					String curMsg = curMethodName + "Error deleting skier score \n" + excp.Message;
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );
				}
			}

			myScoreRow = null;
			scoreFeetTextBox.Text = "";
			hcapScoreTextBox.Text = "";
			scoreMetersTextBox.Text = "";
			nopsScoreTextBox.Text = "";
			noteTextBox.Text = "";
			RampHeightTextBox.Text = "";
			TeamCodeTextBox.Text = "";
			BoatSpeedTextBox.Text = "";
			JumpSpeedSelect.CurrentValue = myMaxSpeed;

			scoreEntryBegin();

			setEventRegRowStatus( "1-TBD" );
			refreshScoreSummaryWindow();
		}

		private void setRecapCellsForEdit() {
			foreach ( DataGridViewCell curCell in myRecapRow.Cells ) {
				if ( myNumJudges == 0 ) {
					if ( curCell.OwningColumn.Name.StartsWith( "BoatSplit" )
						|| curCell.OwningColumn.Name.StartsWith( "BoatEnd" )
						|| curCell.OwningColumn.Name.StartsWith( "ScoreFeet" )
						|| curCell.OwningColumn.Name.StartsWith( "ScoreMeters" )
						|| curCell.OwningColumn.Name.StartsWith( "Note" )
						|| curCell.OwningColumn.Name.StartsWith( "Updated" )
						|| curCell.OwningColumn.Name.StartsWith( "Results" )
						|| curCell.OwningColumn.Name.StartsWith( "Reride" )
						|| curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
						|| curCell.OwningColumn.Name.StartsWith( "ReturnToBase" )
						|| curCell.OwningColumn.Name.StartsWith( "TimeInTol" )
						) {
						curCell.ReadOnly = false;
					}
				} else if ( myNumJudges == 3 ) {
					if ( curCell.OwningColumn.Name.StartsWith( "Meter1" )
						|| curCell.OwningColumn.Name.StartsWith( "Meter2" )
						|| curCell.OwningColumn.Name.StartsWith( "Meter3" )
						|| curCell.OwningColumn.Name.StartsWith( "BoatSplit" )
						|| curCell.OwningColumn.Name.StartsWith( "BoatEnd" )
						|| curCell.OwningColumn.Name.StartsWith( "Note" )
						|| curCell.OwningColumn.Name.StartsWith( "Updated" )
						|| curCell.OwningColumn.Name.StartsWith( "Results" )
						|| curCell.OwningColumn.Name.StartsWith( "Reride" )
						|| curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
						|| curCell.OwningColumn.Name.StartsWith( "ReturnToBase" )
						|| curCell.OwningColumn.Name.StartsWith( "TimeInTol" )
						) {
						curCell.ReadOnly = false;
					}
				} else if ( myNumJudges == 6 ) {
					if ( curCell.OwningColumn.Name.StartsWith( "Meter" )
						|| curCell.OwningColumn.Name.StartsWith( "BoatSplit" )
						|| curCell.OwningColumn.Name.StartsWith( "BoatEnd" )
						|| curCell.OwningColumn.Name.StartsWith( "Note" )
						|| curCell.OwningColumn.Name.StartsWith( "Updated" )
						|| curCell.OwningColumn.Name.StartsWith( "Results" )
						|| curCell.OwningColumn.Name.StartsWith( "Reride" )
						|| curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
						|| curCell.OwningColumn.Name.StartsWith( "ReturnToBase" )
						|| curCell.OwningColumn.Name.StartsWith( "TimeInTol" )
						) {
						curCell.ReadOnly = false;
					}
				}
			}
		}

		private void addPassButton_Click( object sender, EventArgs e ) {
			if ( myBoatListIdx == 0 ) {
				MessageBox.Show( "Boat selection has not been made.  This is required before skier can be scored." );
				return;
			}

			if ( isDataModified ) saveScore();

			SimulationPassButton.Enabled = false;
			SimulationPassButton.Visible = false;

			if ( jumpRecapDataGridView.Rows.Count > 0 ) {
				int rowIndex = jumpRecapDataGridView.Rows.Count - 1;
				jumpRecapDataGridView.CurrentCell = jumpRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex];

			} else {
				if ( checkForSkierRoundScore( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString()
					, Convert.ToInt32( roundActiveSelect.RoundValue )
					, TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString() ) ) {
					MessageBox.Show( "Skier already has a score in this round" );
					return;
				}

				String curEventGroup = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value.ToString();
                if ( HelperFunctions.isCollegiateEvent( JumpEventData.myTourRules ) ) curEventGroup = HelperFunctions.getEventGroupValueNcwsa( EventGroupList.SelectedItem.ToString() );
                if ( WscHandler.isConnectActive ) WscHandler.sendOfficialsAssignments( "Jump", curEventGroup, Convert.ToInt16( roundActiveSelect.RoundValue ) );
				if ( !WaterskiConnectLabel.Visible ) WaterskiConnectLabel.Visible = false;

				if ( !( curEventGroup.Equals( myPrevEventGroup ) ) ) {
					if ( WscHandler.isConnectActive ) {
                        DataTable curStartListDataTable = getEventGroupStartList( curEventGroup, Convert.ToInt32( roundActiveSelect.RoundValue ) );
                        WscHandler.sendRunningOrder( "Jump", Convert.ToInt32( roundActiveSelect.RoundValue ), curStartListDataTable );
					}
                    if ( WaterskiConnectLabel.Visible && !WscHandler.isConnectActive ) WaterskiConnectLabel.Visible = false;

                    if ( LiveWebLabel.Visible ) {
                        LiveWebHandler.sendRunOrder( "Jump", JumpEventData.mySanctionNum, curEventGroup, Convert.ToByte( roundActiveSelect.RoundValue ) );
                    }

                    /*
					 * Provide a warning message for class R events when official assignments have not been entered for the round and event group
					 * These assignments are not mandatory but they are strongly preferred and are very helpful for the TCs
					 */
                    if ( (Decimal)JumpEventData.myClassRowTour["ListCodeNum"] >= (Decimal)JumpEventData.myClassERow["ListCodeNum"] ) {
						String curWarnMsg = String.Format( "Warn:Officials:Round:{0}:EventGroup:{1}", roundActiveSelect.RoundValue, curEventGroup );
						if ( !( myCompletedNotices.Contains( curWarnMsg ) ) ) {
							if ( myCheckOfficials.officialAsgmtCount == 0 ) {
								String curAgeGroup = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString();
								if ( !checkLoadOfficialAssgmt( curAgeGroup, curEventGroup ) ) {
									MessageBox.Show( "No officials have been assigned for this event group and round "
										+ "\n\nThese assignments are not mandatory but they are strongly recommended and are very helpful for the TCs" );
									myCompletedNotices.Add( curWarnMsg );
								}
							}
						}
					}

				}
				myPrevEventGroup = curEventGroup;

				Decimal cur1stRampHeight = getSkier1stRoundRampHeight( (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value );
				if ( cur1stRampHeight > 0 ) {
					if ( cur1stRampHeight != myJumpHeight ) {
						MessageBox.Show( " Skier used a ramp height of " + cur1stRampHeight.ToString( "0.0" ) + " in the first round."
							+ "\n Current ramp height is " + myJumpHeight.ToString( "0.0" )
							+ "\n Ensure these selections are desired."
							);
					}
				}
			}

			if ( jumpRecapDataGridView.Rows.Count > 0 ) {
				if ( checkRoundContinue() ) {
					isAddRecapRowInProg = true;
					Timer curTimerObj = new Timer();
					curTimerObj.Interval = 15;
					curTimerObj.Tick += new EventHandler( addRecapRowTimer );
					curTimerObj.Start();
				}

			} else {
				isAddRecapRowInProg = true;
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 15;
				curTimerObj.Tick += new EventHandler( addRecapRowTimer );
				curTimerObj.Start();
			}
		}

		private void navSort_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			mySortCommand = getSortCommand();
			sortDialogForm.SortCommand = mySortCommand;
			sortDialogForm.ShowDialog( this );

			// Determine if the OK button was clicked on the dialog box.
			if ( sortDialogForm.DialogResult == DialogResult.OK ) {
				mySortCommand = sortDialogForm.SortCommand;
				if ( isOrderByRoundActive ) {
					myTourProperties.RunningOrderSortJumpRound = mySortCommand;
				} else {
					myTourProperties.RunningOrderSortJump = mySortCommand;
				}
				winStatusMsg.Text = "Sorted by " + mySortCommand;
				loadTourEventRegView();
			}
		}

		private void navFilter_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			filterDialogForm.ShowDialog( this );

			// Determine if the OK button was clicked on the dialog box.
			if ( filterDialogForm.DialogResult == DialogResult.OK ) {
				myFilterCmd = filterDialogForm.FilterCommand;
				winStatusMsg.Text = "Filtered by " + myFilterCmd;
				loadTourEventRegView();
			}
		}

		private void navExportRecord_Click( object sender, EventArgs e ) {
			ExportRecordAppData myExportData = new ExportRecordAppData();
			DataGridViewRow curViewRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
			String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
			String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
			String curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
			byte curRound = Convert.ToByte( (String)myRecapRow.Cells["RoundRecap"].Value );
			myExportData.ExportData( JumpEventData.mySanctionNum, "Jump", curMemberId, curAgeGroup, curEventGroup, curRound );
		}

		private void navExportLw_Click( object sender, EventArgs e ) {
			navExport_Click( null, null );
			ExportLiveWeb.uploadExportFile( "Export", "Jump", JumpEventData.mySanctionNum );
		}

		/* ----------------------------------------
		 * Export data based on defined specificatoins
		 * ---------------------------------------- */
		private void navExport_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			String[] curTableName = { "TourReg", "EventReg", "EventRunOrder", "JumpScore", "JumpRecap", "TourReg", "OfficialWork", "OfficialWorkAsgmt", "BoatTime", "BoatPath", "JumpMeasurement" };
			String[] curSelectCommand;
			String curEventGroup = EventGroupList.SelectedItem.ToString();
			if (HelperFunctions.isGroupValueNcwsa( EventGroupList.SelectedItem.ToString() )) {
				curEventGroup = HelperFunctions.getEventGroupValueNcwsa( EventGroupList.SelectedItem.ToString() );
				curSelectCommand = JumpEventData.buildScoreExport( roundActiveSelect.RoundValue, curEventGroup, true );
			
			} else if (JumpEventData.isCollegiateEvent()) {
                curSelectCommand = JumpEventData.buildScoreExport( roundActiveSelect.RoundValue, curEventGroup, true );

            } else {
				curSelectCommand = JumpEventData.buildScoreExport( roundActiveSelect.RoundValue, curEventGroup, false );
			}
			myExportData.exportData( curTableName, curSelectCommand );
		}

		private void navRefreshTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( navRefreshTimer );
			navRefresh_Click( null, null );
		}

		private void navRefresh_Click( object sender, EventArgs e ) {
			// Retrieve data from database
			String curGroupValue = "All";
			curGroupValue = EventGroupList.SelectedItem.ToString();

			// Retrieve data from database
			getEventRegData( curGroupValue, Convert.ToByte( roundActiveSelect.RoundValue ) );
			loadTourEventRegView();
		}

		private void navLiveWeb_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			LiveWebHandler.LiveWebDialog.ShowDialog( this );

			// Determine if the OK button was clicked on the dialog box.
			if ( LiveWebHandler.LiveWebDialog.DialogResult != DialogResult.OK ) return;

			if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "Connect" ) ) {
				if ( LiveWebHandler.connectLiveWebHandler( JumpEventData.mySanctionNum ) ) {
					LiveWebLabel.Visible = true;

				} else {
					ExportLiveTwitter.TwitterLocation = "";
					LiveWebLabel.Visible = false;
				}

			} else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "Disable" ) ) {
				LiveWebHandler.disconnectLiveWebHandler();
				LiveWebLabel.Visible = false;

			} else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "Resend" ) ) {
				if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
					String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
					String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
					String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
					byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
					LiveWebHandler.sendCurrentSkier( "Jump", JumpEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0 );
				}

			} else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "ResendAll" ) ) {
				if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
					String curEventGroup = EventGroupList.SelectedItem.ToString();
					byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
					LiveWebHandler.sendSkiers( "Jump", JumpEventData.mySanctionNum, curRound, curEventGroup );
					// ExportLiveWeb.exportCurrentSkiers( "Jump", JumpEventData.mySanctionNum, curRound, curEventGroup
				}

			} else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "DiableSkier" ) ) {
				if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
					String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
					String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
					String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
					byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
					LiveWebHandler.sendDisableCurrentSkier( "Jump", JumpEventData.mySanctionNum, curMemberId, curAgeGroup, curRound );
					//ExportLiveWeb.exportCurrentSkierJump( JumpEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup
				}

			} else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "DiableAllSkier" ) ) {
				if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
					String curEventGroup = EventGroupList.SelectedItem.ToString();
					byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
					LiveWebHandler.sendDisableSkiers( "Jump", JumpEventData.mySanctionNum, curRound, curEventGroup );
					//ExportLiveWeb.exportCurrentSkiers( "Jump", JumpEventData.mySanctionNum, curRound, curEventGroup )
				}
			}
		}

		private void navWaterSkiConnect_Click( object sender, EventArgs e ) {
			WscHandler.checkWscConnectStatus( sender != null );
			if ( WscHandler.isConnectActive ) {
				WaterskiConnectLabel.Visible = true;
				return;
			}

			WaterskiConnectLabel.Visible = false;
			return;
		}

		private void roundActiveSelect_Click( object sender, EventArgs e ) {
			//Update data if changes are detected
			if ( isDataModified ) {
				MessageBox.Show( "You must save your changes before selecting a new round" );
				return;
			}

			if ( sender != null ) {
				String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
				try {
					roundActiveSelect.RoundValue = curValue;
				} catch ( Exception ex ) {
					curValue = ex.Message;
				}
			}

			//Retrieve and load tournament event entries
			loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

			int curSaveEventRegViewIdx = myEventRegViewIdx;
			navRefresh_Click( null, null );

			if ( curSaveEventRegViewIdx > 0 ) {
				jumpRecapDataGridView.Rows.Clear();
				myEventRegViewIdx = curSaveEventRegViewIdx;
				if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
					myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
				}
				TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
				roundActiveSelect.RoundValue = roundActiveSelect.RoundValue;
				setJumpScoreEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
				setJumpRecapEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 15;
				curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
				curTimerObj.Start();
			}
		}

		private void JumpSpeedSelect_Change( object sender, EventArgs e ) {
			if ( sender != null ) {
				String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
				try {
					JumpSpeedSelect.CurrentValue = Convert.ToInt16( Convert.ToDecimal( curValue ) );
				} catch ( Exception ex ) {
					curValue = ex.Message;
				}
			}

			DataRow curClassRowSkier = JumpEventData.getSkierClass( (String)scoreEventClass.SelectedValue );
			String curEventClass = HelperFunctions.getDataRowColValue( curClassRowSkier, "ListCode", JumpEventData.myTourClass );

			loadBoatTimeView( curEventClass, JumpSpeedSelect.CurrentValue );

			if ( myRecapRow != null && jumpRecapDataGridView.Rows.Count > 0 ) {
				if ( myRecapRow.Index == ( jumpRecapDataGridView.Rows.Count - 1 ) ) {
					isDataModified = true;
					myRecapRow.Cells["BoatSpeedRecap"].Value = JumpSpeedSelect.CurrentValue.ToString();
					loadBoatTimeView( curEventClass, JumpSpeedSelect.CurrentValue );
					checkNeedTimeValidate();
				}
			}
		}

		private void RampHeightSelect_Change( object sender, EventArgs e ) {
			String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
			RampHeightSelect.CurrentValue = Convert.ToDecimal( curValue );
			myJumpHeight = Convert.ToDecimal( curValue );
			RampHeightTextBox.Text = myJumpHeight.ToString( "0.0" );

			try {
				DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
				String curMemberId = (String)curEventRegRow.Cells["MemberId"].Value;
				String curAgeGroup = (String)curEventRegRow.Cells["AgeGroup"].Value;
				Int16 curRound = Convert.ToInt16( roundActiveSelect.RoundValue );

				StringBuilder curSqlStmt = new StringBuilder( "Update JumpScore " );
				curSqlStmt.Append( "Set RampHeight = " + RampHeightTextBox.Text + " " );
				curSqlStmt.Append( "Where SanctionId = '" + JumpEventData.mySanctionNum + "' " );
				curSqlStmt.Append( "  And MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "' " );
				curSqlStmt.Append( "  And Round = '" + curRound.ToString() + "' " );
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "Update JumpRecap " );
				curSqlStmt.Append( "Set RampHeight = " + RampHeightTextBox.Text + " " );
				curSqlStmt.Append( "Where SanctionId = '" + JumpEventData.mySanctionNum + "' " );
				curSqlStmt.Append( "  And MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "' " );
				curSqlStmt.Append( "  And Round = '" + curRound.ToString() + "' " );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			} catch ( Exception excp ) {
				MessageBox.Show( "Error updating skier jump height \n" + excp.Message );
			}

		}

		private void SkierBoatPathSelect_Change( object sender, EventArgs e ) {
			mySkierBoatPathSelect = ( (RadioButtonWithValue)sender ).Value.ToString();
			SkierBoatPathSelect.CurrentValue = mySkierBoatPathSelect;
			SkierBoatPathSelect.Text = mySkierBoatPathSelect;
			if ( jumpRecapDataGridView.Rows.Count > 0 && myRecapRow != null ) myRecapRow.Cells["SkierBoatPathRecap"].Value = mySkierBoatPathSelect;
		}

		private void scoreEventClass_SelectedIndexChanged( object sender, EventArgs e ) {
			addPassButton.Focus();
		}

		private void scoreEventClass_Validating( object sender, CancelEventArgs e ) {
			String curMethodName = "Jump:ScoreEntrySeg3:scoreEventClass_Validating: ";
			int rowsProc = 0;
			if ( TourEventRegDataGridView.Rows.Count == 0 ) return;

			ListItem curItem = (ListItem)scoreEventClass.SelectedItem;
			String curSkierClass = curItem.ItemValue;
			if ( JumpEventData.mySkierClassList.compareClassChange( curSkierClass, JumpEventData.myTourClass ) < 0 ) {
				String curMsg = String.Format( "Class {0} cannot be assigned to a skier in a class {1} tournament", curSkierClass, JumpEventData.myTourClass );
				MessageBox.Show( curMsg );
				e.Cancel = true;
				return;
			}

			DataRow curClassRowSkier = JumpEventData.getSkierClass( (String)scoreEventClass.SelectedValue );
			String curEventClass = HelperFunctions.getDataRowColValue( curClassRowSkier, "ListCode", JumpEventData.myTourClass );

			if ( (Decimal)curClassRowSkier["ListCodeNum"] > (Decimal)JumpEventData.myClassERow["ListCodeNum"] || JumpEventData.isIwwfEvent() ) {
				bool iwwfMembership = IwwfMembership.validateIwwfMembership(
					JumpEventData.mySanctionNum, (String)JumpEventData.myTourRow["SanctionEditCode"]
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value
					, (String)JumpEventData.myTourRow["EventDates"] );
				if ( !( iwwfMembership ) ) {
					curEventClass = "E";
					scoreEventClass.SelectedValue = curEventClass;
				}
			}

			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value = curSkierClass;
			try {
				Int64 curScorePK = 0;
				if ( myScoreRow == null ) {
					curScorePK = -1;
				} else {
					curScorePK = (Int64)myScoreRow["PK"];
				}
				StringBuilder curSqlStmt = new StringBuilder( "" );
				if ( curScorePK > 0 ) {
					curSqlStmt.Append( "Update JumpScore Set " );
					curSqlStmt.Append( "EventClass = '" + curSkierClass + "'" );
					curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
					curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
				}
			} catch ( Exception excp ) {
				String curMsg = ":Error attempting to update skier class \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			}
		}

		private void tourEventRegDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			DataGridView myDataView = (DataGridView)sender;

			//Update data if changes are detected
			if ( isDataModified && ( myEventRegViewIdx != e.RowIndex ) ) {
				try {
					navSaveItem_Click( null, null );
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}
			if ( !( isDataModified ) ) {
				if ( myEventRegViewIdx != e.RowIndex ) {
					boatPathDataGridView.Visible = false;
					InvalidateBoatPathButton.Visible = false;
					BpmsDriverLabel.Visible = false;
					BpmsDriverLabel.Enabled = false;
					BpmsDriver.Visible = false;
					BpmsDriver.Enabled = false;
					BpmsDriver.Text = "";

					jumpRecapDataGridView.Rows.Clear();
					skierPassMsg.Text = "";

					myEventRegViewIdx = e.RowIndex;
					int curRowPos = e.RowIndex + 1;
					RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + myDataView.Rows.Count.ToString();
					if ( HelperFunctions.isObjectPopulated( myDataView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) {
						setJumpScoreEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
						setJumpRecapEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );

						Timer curTimerObj = new Timer();
						curTimerObj.Interval = 15;
						curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
						curTimerObj.Start();
					}
				}
			}
		}

		private void EventGroupList_SelectedIndexChanged( object sender, EventArgs e ) {
			Int16 curRound = 0;
			try {
				curRound = Convert.ToInt16( roundActiveSelect.RoundValue );
			} catch {
				curRound = 0;
			}
			if ( curRound >= 25 ) {
				MessageBox.Show( "You currently have the runoff round selected.\nChange the round if that is not desired." );
			}
			if ( isDataModified ) {
				try {
					navSaveItem_Click( null, null );
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}

			if ( isDataModified ) return;
			if ( isLoadInProg ) return;

			int curSaveEventRegViewIdx = myEventRegViewIdx;
			navRefresh_Click( null, null );
			if ( curSaveEventRegViewIdx > 0 ) {
				jumpRecapDataGridView.Rows.Clear();
				myEventRegViewIdx = curSaveEventRegViewIdx;
				if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
				if ( myEventRegViewIdx >= 0 ) {
					TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];

					roundActiveSelect.RoundValue = roundActiveSelect.RoundValue;
					setJumpScoreEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
					setJumpRecapEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );

					Timer curTimerObj = new Timer();
					curTimerObj.Interval = 15;
					curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
					curTimerObj.Start();
				}
			}
		}

		/* 
		 * Retrieve all registrations depending on filtering options
		 */
		private void loadTourEventRegView() {
			isDataModified = false;

			try {
				if ( TourEventRegDataGridView.Rows.Count > 0 ) TourEventRegDataGridView.Rows.Clear();
				if ( myTourEventRegDataTable == null || myTourEventRegDataTable.Rows.Count == 0 ) return;
				if ( jumpRecapDataGridView.Rows.Count > 0 ) {
					try {
						jumpRecapDataGridView.Rows.Clear();
					} catch {
						jumpRecapDataGridView.EndEdit();
						while ( jumpRecapDataGridView.Rows.Count > 0 ) {
							jumpRecapDataGridView.Rows.RemoveAt( jumpRecapDataGridView.Rows.Count - 1 );
						}
					}
				}

				winStatusMsg.Text = "Retrieving tournament entries";
				Cursor.Current = Cursors.WaitCursor;
				isOrderByRoundActive = isRunOrderByRound( Convert.ToByte( roundActiveSelect.RoundValue ) );
				mySortCommand = getSortCommand();

				myTourEventRegDataTable.DefaultView.Sort = mySortCommand;
				myTourEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
				DataTable curDataTable = myTourEventRegDataTable.DefaultView.ToTable();

				DataGridViewRow curViewRow;
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					myEventRegViewIdx = TourEventRegDataGridView.Rows.Add();
					curViewRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];

					curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
					curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
					curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
					curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];
					curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];

					String curEventGroup = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
					String curRunOrderGroup = HelperFunctions.getDataRowColValue( curDataRow, "RunOrderGroup", "" );
					if ( curRunOrderGroup.Length > 0 ) {
						curViewRow.Cells["EventGroup"].Value = curEventGroup + "-" + curRunOrderGroup;
					} else {
						curViewRow.Cells["EventGroup"].Value = curEventGroup;
					}

					curViewRow.Cells["AgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
					curViewRow.Cells["Gender"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Gender", "" );
					curViewRow.Cells["EventClass"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventClass", "" );
					curViewRow.Cells["TeamCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TeamCode", "" );
					curViewRow.Cells["Order"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Order", "0" );
					if ( JumpEventData.isIwwfEvent() ) {
						curViewRow.Cells["Score"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "ScoreMeters", "", 1 );
					} else {
						curViewRow.Cells["Score"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScoreFeet", "" );
					}

					try {
						hcapScoreTextBox.Text = "0";
						String curHCapScore = HelperFunctions.getDataRowColValue( curDataRow, "HCapScore", "" );
						if ( HelperFunctions.isObjectPopulated( curHCapScore ) ) {
							if ( JumpEventData.isIwwfEvent() ) {
								curViewRow.Cells["ScoreWithHcap"].Value = ( (Decimal)curDataRow["ScoreMeters"] + Decimal.Parse( curHCapScore ) ).ToString( "##,###0.0" );
							} else {
								curViewRow.Cells["ScoreWithHcap"].Value = ( (Decimal)curDataRow["ScoreFeet"] + Decimal.Parse( curHCapScore ) ).ToString( "##,###0.0" );
							}
							hcapScoreTextBox.Text = HelperFunctions.getViewRowColValue( curViewRow, "ScoreWithHcap", "0" );
						}

					} catch {
						curViewRow.Cells["ScoreWithHcap"].Value = "";
						hcapScoreTextBox.Text = "";
					}

					curViewRow.Cells["RankingScore"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "RankingScore", "", 1 );
					curViewRow.Cells["HCapBase"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HCapBase", "0", 1 );
					curViewRow.Cells["HCapScore"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HCapScore", "0", 1 );
					curViewRow.Cells["RankingRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RankingRating", "" );
					curViewRow.Cells["JumpHeight"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpHeight", "" );
					curViewRow.Cells["SkiYearAge"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SkiYearAge", "" );
					if ( curViewRow.Cells["SkiYearAge"].Value.ToString().Equals( "" ) ) curViewRow.Cells["SkiYearAge"].Value = "1";
					setEventRegRowStatus( HelperFunctions.getDataRowColValue( curDataRow, "Status", "" ) );
					curViewRow.Cells["State"].Value = HelperFunctions.getDataRowColValue( curDataRow, "State", "" );
					curViewRow.Cells["Federation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" );
				}
				myEventRegViewIdx = 0;
				TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];

				jumpRecapDataGridView.Rows.Clear();
				setJumpScoreEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
				setJumpRecapEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 15;
				curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
				curTimerObj.Start();

			} catch ( Exception ex ) {
				MessageBox.Show( "Error retrieving jump tournament entries \n" + ex.Message );
			} finally {
				Cursor.Current = Cursors.Default;
			}

			if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
				LiveWebLabel.Visible = true;
			} else {
				LiveWebLabel.Visible = false;
			}

			int curRowPos = myEventRegViewIdx + 1;
			RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + TourEventRegDataGridView.Rows.Count.ToString();

			String curWarnMsg = "Warn:RunOrder:Round:" + roundActiveSelect.RoundValue;
			if ( !( myCompletedNotices.Contains( curWarnMsg ) ) ) {
				if ( isOrderByRoundActive ) {
					MessageBox.Show( "WARNING \nThis running order is specific for this round" );
					myCompletedNotices.Add( curWarnMsg );
				}
			}
		}

		private void loadEventGroupList( int inRound ) {
			isLoadInProg = true;
			if ( EventGroupList.DataSource == null ) {
				if ( JumpEventData.isCollegiateEvent() ) {
					EventGroupList.DataSource = HelperFunctions.buildEventGroupListNcwsa();
				} else {
					loadEventGroupListFromData( inRound );
				}

			} else if ( ( (ArrayList)EventGroupList.DataSource ).Count > 0 ) {
				if ( JumpEventData.isCollegiateEvent() ) {
				} else {
					loadEventGroupListFromData( inRound );
				}
			
			} else {
				if ( JumpEventData.isCollegiateEvent() ) {
					EventGroupList.DataSource = HelperFunctions.buildEventGroupListNcwsa();
				} else {
					loadEventGroupListFromData( inRound );
				}
			}
			isLoadInProg = false;
		}

		private void loadEventGroupListFromData( int inRound ) {
			String curGroupValue = "";
			if ( EventGroupList.DataSource != null ) {
				if ( ( (ArrayList)EventGroupList.DataSource ).Count > 0 ) {
					try {
						curGroupValue = EventGroupList.SelectedItem.ToString();
					} catch {
						curGroupValue = "";
					}
				}
			}

			EventGroupList.DataSource = HelperFunctions.buildEventGroupList( JumpEventData.mySanctionNum, "Jump", inRound );
			if ( curGroupValue.Length > 0 ) {
				foreach ( String curValue in (ArrayList)EventGroupList.DataSource ) {
					if ( curValue.Equals( curGroupValue ) ) {
						EventGroupList.SelectedItem = curGroupValue;
						EventGroupList.Text = curGroupValue;
						return;
					}
				}
				EventGroupList.SelectedItem = "All";
				EventGroupList.Text = "All";
			} else {
				EventGroupList.SelectedItem = "All";
				EventGroupList.Text = "All";
			}
		}

		private void PauseTimerButton_Click( object sender, EventArgs e ) {
			myEventDelayStartTime = DateTime.Now;
			EventDelayReasonTextBox.Visible = true;
			EventDelayReasonLabel.Visible = true;
			EventDelayReasonTextBox.Text = "";
			EventDelayReasonTextBox.Focus();
			StartTimerButton.Visible = true;
			PauseTimerButton.Visible = false;
		}

		private void StartTimerButton_Click( object sender, EventArgs e ) {
			try {
				myEventDelaySeconds += Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventDelayStartTime ) ).TotalSeconds );
				if ( EventDelayReasonTextBox.Text.Length > 1 ) {
					Log.WriteFile( "Jump:ScoreEntrySeg3:Event delayed: " + EventDelayReasonTextBox.Text );
				}
			} catch {
			}
			EventDelayReasonTextBox.Visible = false;
			EventDelayReasonLabel.Visible = false;
			StartTimerButton.Visible = false;
			PauseTimerButton.Visible = true;
		}

		private void ForceCompButton_Click( object sender, EventArgs e ) {
			String curMethodName = "Jump:ScoreEntrySeg3:ForceCompButton_Click: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			if ( TourEventRegDataGridView.Rows.Count > 0 ) {
				if ( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value.Equals( "2-InProg" ) ) {
					String myBoatTime = "";
					try {
						myBoatTime = myRecapRow.Cells["BoatSplitTimeRecap"].Value.ToString();
					} catch {
						myBoatTime = "";
					}
					if ( myBoatTime.Length < 2 ) {
						deletePassButton_Click( null, null );
					}
					try {
						if ( myScoreRow != null ) {
							skierDoneReasonDialogForm.ReasonText = noteTextBox.Text;
							if ( skierDoneReasonDialogForm.ShowDialog() == DialogResult.OK ) {
								String curReason = skierDoneReasonDialogForm.ReasonText;
								if ( curReason.Length > 3 ) {
									noteTextBox.Text = curReason;
									myScoreRow["Note"] = curReason;
									curReason = curReason.Replace( "'", "''" );
									Int64 curScorePK = (Int64)myScoreRow["PK"];
									String curStatus = "TBD";
									try {
										setEventRegRowStatus( "4-Done", TourEventRegDataGridView.Rows[myEventRegViewIdx] );
										String curValue = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value;
										if ( curValue.Equals( "4-Done" ) ) curStatus = "Complete";
										if ( curValue.Equals( "2-InProg" ) ) curStatus = "InProg";
										if ( curValue.Equals( "3-Error" ) ) curStatus = "Error";
										if ( curValue.Equals( "1-TBD" ) ) curStatus = "TBD";
									} catch {
										curStatus = "TBD";
									}

									curSqlStmt.Append( "Update JumpScore Set " );
									curSqlStmt.Append( "Status = '" + curStatus + "'" );
									curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
									curSqlStmt.Append( ", Note = '" + curReason + "'" );
									curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
									int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
									Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
								} else {
									MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
								}
							} else {
								MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
							}
						}
					} catch ( Exception excp ) {
						String curMsg = ":Error attempting to perform skier complete \n" + excp.Message + "\n" + curSqlStmt.ToString();
						MessageBox.Show( curMsg );
						Log.WriteFile( curMethodName + curMsg );
					}

					isDataModified = false;
				}
			}

		}

		/*
		 * Retrieve data for current tournament
		 * Used for initial load and to refresh data after updates
		 */
		private void loadBoatTimeView( String inSkierClass, Int16 inSpeed ) {
			winStatusMsg.Text = "Retrieving boat times";

			try {
				listBoatTimesDataGridView.Rows.Clear();
				JumpEventData.myTimesDataTable.DefaultView.RowFilter = "ListCode like '" + inSpeed.ToString() + "-" + inSkierClass + "-%'";
				DataTable curDataTable = JumpEventData.myTimesDataTable.DefaultView.ToTable();
				if ( curDataTable.Rows.Count == 0 ) {
					JumpEventData.myTimesDataTable.DefaultView.RowFilter = "ListCode like '" + inSpeed.ToString() + "-C-%'";
					curDataTable = JumpEventData.myTimesDataTable.DefaultView.ToTable();
					if ( curDataTable.Rows.Count == 0 ) return;
				}

				Cursor.Current = Cursors.WaitCursor;
				DataGridViewRow curViewRow;
				int curViewIdx = 0;
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					curViewIdx = listBoatTimesDataGridView.Rows.Add();
					curViewRow = listBoatTimesDataGridView.Rows[curViewIdx];

					curViewRow.Cells["BoatTimeKey"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ListCode", "" );
					curViewRow.Cells["ListCodeNum"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ListCodeNum", "" );
					curViewRow.Cells["ActualTime"].Value = HelperFunctions.getDataRowColValue( curDataRow, "CodeValue", "" );
					curViewRow.Cells["FastTimeTol"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "MinValue", "", 2 );
					curViewRow.Cells["SlowtimeTol"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "MaxValue", "", 2 );
					curViewRow.Cells["TimeKeyDesc"].Value = HelperFunctions.getDataRowColValue( curDataRow, "CodeDesc", "" );
				}
				
				listBoatTimesDataGridView.CurrentCell = listBoatTimesDataGridView.Rows[0].Cells["BoatTimeKey"];

			} catch ( Exception ex ) {
				MessageBox.Show( "Error retrieving boat times \n" + ex.Message );
			
			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		public void setJumpScoreEntry( int inRound ) {
			Decimal curMaxRamp = Convert.ToDecimal( "5.0" );

			Cursor.Current = Cursors.WaitCursor;
			if ( !( WscHandler.isConnectActive ) ) navWaterSkiConnect_Click( null, null );

			DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
			String curMemberId = (String)curEventRegRow.Cells["MemberId"].Value;
			String curAgeGroup = (String)curEventRegRow.Cells["AgeGroup"].Value;
			String curEventGroup = (String)curEventRegRow.Cells["EventGroup"].Value;
			
			activeSkierName.Text = (String)curEventRegRow.Cells["SkierName"].Value;
			this.Text = myTitle + " - " + activeSkierName.Text;

			DataRow[] curFoundRows = JumpEventData.myMaxSpeedDataTable.Select( "ListCode = '" + curAgeGroup + "'" );
			if ( curFoundRows.Length == 0 ) {
				MessageBox.Show( "Max speed entry not found for divsion code " + curAgeGroup );
				return;
			}
			DataRow curDataRow = curFoundRows[0];
			
			myMaxSpeed = Convert.ToInt16( (Decimal)curDataRow["MaxValue"] );
			Int16 curMinSpeed = 0;
			if ( JumpEventData.myMinSpeedDataTable.Rows.Count > 0 ) {
				curDataRow = JumpEventData.myMinSpeedDataTable.Rows[0];
				curMinSpeed = Convert.ToInt16( (String)curDataRow["Speed"] );
			}
			JumpSpeedSelect.SetMinMaxValue( curMinSpeed, myMaxSpeed );

			curDataRow = JumpEventData.myMaxRampDataTable.Select( "ListCode = '" + curAgeGroup + "'" )[0];
			curMaxRamp = Convert.ToDecimal( (Decimal)curDataRow["MaxValue"] );
			RampHeightSelect.SetMaxValue( curMaxRamp );

			if ( myJumpHeight == 0 ) {
				myJumpHeight = curMaxRamp;
			} if ( myJumpHeight > curMaxRamp ) {
				myJumpHeight = curMaxRamp;
			}

			driverDropdown.SelectedValue = "";
			driverDropdown.Text = "";
			checkLoadOfficialAssgmt( curAgeGroup, curEventGroup );

			myScoreDataTable = getSkierScoreByRound( curMemberId, curAgeGroup, inRound );
			if ( myScoreDataTable.Rows.Count > 0 ) {
				myScoreRow = myScoreDataTable.Rows[0];
				scoreEventClass.SelectedValue = HelperFunctions.getDataRowColValue( myScoreRow, "EventClass", HelperFunctions.getViewRowColValue( curEventRegRow, "EventClass", JumpEventData.myTourClass ) );
				roundActiveSelect.RoundValue = HelperFunctions.getDataRowColValue( myScoreRow, "EventClass", "1" );

				scoreFeetTextBox.Text = HelperFunctions.getDataRowColValueDecimal( myScoreRow, "ScoreFeet", "0", 0 );
				scoreMetersTextBox.Text = HelperFunctions.getDataRowColValueDecimal( myScoreRow, "ScoreMeters", "0", 1 );
				nopsScoreTextBox.Text = HelperFunctions.getDataRowColValueDecimal( myScoreRow, "NopsScore", "0", 1 );
				try {
					hcapScoreTextBox.Text = "0";
					String curHCapScore = HelperFunctions.getDataRowColValue( myScoreRow, "HCapScore", "" );
					if ( HelperFunctions.isObjectPopulated( curHCapScore ) ) {
						if ( JumpEventData.isIwwfEvent() ) {
							hcapScoreTextBox.Text = ( (Decimal)myScoreRow["ScoreMeters"] + Decimal.Parse( curHCapScore ) ).ToString( "##,###0.0" );
						} else {
							hcapScoreTextBox.Text = ( (Decimal)myScoreRow["ScoreFeet"] + Decimal.Parse( curHCapScore ) ).ToString( "##,###0.0" );
						}
						hcapScoreTextBox.Text = HelperFunctions.getDataRowColValue( curDataRow, "ScoreWithHcap", "0" );
					}
				} catch {
					hcapScoreTextBox.Text = "";
				}
				noteTextBox.Text = HelperFunctions.getDataRowColValue( myScoreRow, "Note", "" );
				RampHeightSelect.CurrentValue = Convert.ToDecimal( HelperFunctions.getDataRowColValue( myScoreRow, "RampHeight", myJumpHeight.ToString( "0.0" ) ) );
				myJumpHeight = RampHeightSelect.CurrentValue;
				RampHeightTextBox.Text = myJumpHeight.ToString( "0.0" );

				JumpSpeedSelect.CurrentValue = Convert.ToInt16( HelperFunctions.getDataRowColValue( myScoreRow, "BoatSpeed", myMaxSpeed.ToString( "00" ) ) );
				BoatSpeedTextBox.Text = "";

				TeamCodeTextBox.Text = HelperFunctions.getDataRowColValue( myScoreRow, "TeamCode", "" );

				TourBoatTextbox.Text = HelperFunctions.getDataRowColValue( myScoreRow, "Boat", "" );
				if ( TourBoatTextbox.Text.Length > 0 ) {
					TourBoatTextbox.Text = setApprovedBoatSelectEntry( (String)myScoreRow["Boat"] );
					TourBoatTextbox.Select( 0, 0 );
				}

				scoreEntryInprogress();

			} else {
				myScoreRow = null;

				scoreEventClass.SelectedValue = (String)curEventRegRow.Cells["EventClass"].Value;
				scoreFeetTextBox.Text = "";
				scoreMetersTextBox.Text = "";
				hcapScoreTextBox.Text = "";
				nopsScoreTextBox.Text = "";
				noteTextBox.Text = "";
				RampHeightTextBox.Text = "";
				BoatSpeedTextBox.Text = "";
				TeamCodeTextBox.Text = "";
				RampHeightSelect.CurrentValue = myJumpHeight;
				JumpSpeedSelect.CurrentValue = myMaxSpeed;
				SkierBoatPathSelect.CurrentValue = SkierBoatPathSelect.DefaultValue;

				TourBoatTextbox.Text = setApprovedBoatSelectEntry( (String)listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatCode"].Value );
				TourBoatTextbox.Select( 0, 0 );

				scoreEntryBegin();
			}

			DataRow curClassRowSkier = JumpEventData.getSkierClass( (String)scoreEventClass.SelectedValue );
			String curEventClass = HelperFunctions.getDataRowColValue( curClassRowSkier, "ListCode", JumpEventData.myTourClass );
			loadBoatTimeView( curEventClass, JumpSpeedSelect.CurrentValue );
			Cursor.Current = Cursors.Default;
		}

		public void setJumpRecapEntry( int inRound ) {
			Int16 curBoatSpeed = JumpSpeedSelect.CurrentValue;

			skierPassMsg.Text = "";
			SimulationPassButton.Enabled = false;
			SimulationPassButton.Visible = false;
			SimulationPassButton.Text = "Simulation Pass";
			DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
			String curMemberId = (String)curEventRegRow.Cells["MemberId"].Value;
			String curAgeGroup = (String)curEventRegRow.Cells["AgeGroup"].Value;

			jumpRecapDataGridView.Rows.Clear();
			myRecapDataTable = getSkierRecapByRound( curMemberId, curAgeGroup, inRound );
			if ( myRecapDataTable == null || myRecapDataTable.Rows.Count == 0 ) {
				skierPassMsg.Text = "";
				myRecapRow = null;
				scoreEntryBegin();
				
				if ( WscHandler.isConnectActive ) {
					SimulationPassButton.Enabled = true;
					SimulationPassButton.Visible = true;
					SimulationPassButton.Text = "Simulation Pass";
				}
			}

			Cursor.Current = Cursors.WaitCursor;
			DataGridViewRow curViewRow;
			int curViewIdx = 0;
			int curViewIdxMax = myRecapDataTable.Rows.Count - 1;
			foreach ( DataRow curDataRow in myRecapDataTable.Rows ) {
				curViewIdx = jumpRecapDataGridView.Rows.Add();
				curViewRow = jumpRecapDataGridView.Rows[curViewIdx];

				curViewRow.Cells["Updated"].Value = "N";
				curViewRow.Cells["PKRecap"].Value = ( (Int64)curDataRow["PK"] ).ToString();
				curViewRow.Cells["SanctionIdRecap"].Value = (String)curDataRow["SanctionId"];
				curViewRow.Cells["MemberIdRecap"].Value = (String)curDataRow["MemberId"];
				curViewRow.Cells["AgeGroupRecap"].Value = (String)curDataRow["AgeGroup"];
				curViewRow.Cells["RoundRecap"].Value = ( (Byte)curDataRow["Round"] ).ToString();
				curViewRow.Cells["RoundRecap"].ToolTipText = ( (DateTime)curDataRow["LastUpdateDate"] ).ToString( "MM/dd HH:mm:ss" );
				curViewRow.Cells["PassNumRecap"].Value = ( curViewIdx + 1 ).ToString();

				curViewRow.Cells["BoatSpeedRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "BoatSpeed", JumpSpeedSelect.CurrentValue.ToString( "00" ) );
				curViewRow.Cells["SkierBoatPathRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SkierBoatPath", SkierBoatPathSelect.DefaultValue );
				SkierBoatPathSelect.CurrentValue = (String)curViewRow.Cells["SkierBoatPathRecap"].Value;
				curViewRow.Cells["ReturnToBaseRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ReturnToBase", "" );
				curViewRow.Cells["ResultsRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Results", "Jump" );

				curViewRow.Cells["Meter1Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Meter1", "" );
				curViewRow.Cells["Meter2Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Meter2", "" );
				curViewRow.Cells["Meter3Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Meter3", "" );
				curViewRow.Cells["Meter4Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Meter4", "" );
				curViewRow.Cells["Meter5Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Meter5", "" );
				curViewRow.Cells["Meter6Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Meter6", "" );

				curViewRow.Cells["BoatSplitTimeRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "BoatSplitTime", "" );
				curViewRow.Cells["BoatSplitTime2Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "BoatSplitTime2", "" );
				curViewRow.Cells["BoatEndTimeRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "BoatEndTime", "" );

				curViewRow.Cells["Split52TimeTolRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Split52TimeTol", "" );
				curViewRow.Cells["Split82TimeTolRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Split82TimeTol", "" );
				curViewRow.Cells["Split41TimeTolRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Split41TimeTol", "" );

				curViewRow.Cells["ScoreFeetRecap"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "ScoreFeet", "", 0 );
				curViewRow.Cells["ScoreMetersRecap"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "ScoreMeters", "", 1 );

				curViewRow.Cells["TimeInTolRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TimeInTol", "N" );
				if ( curViewRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "Y" ) ) {
					curViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;
				} else {
					curViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
				}
				curViewRow.Cells["RerideRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Reride", "N" );
				curViewRow.Cells["RerideIfBestRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RerideIfBest", "N" );
				curViewRow.Cells["RerideCanImproveRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RerideCanImprove", "N" );
				curViewRow.Cells["ScoreProtRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScoreProt", "N" );
				curViewRow.Cells["ScoreTriangleRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScoreTriangle", "0" );
				curViewRow.Cells["RampHeightRecap"].Value = RampHeightTextBox.Text;

				curViewRow.Cells["RerideReasonRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RerideReason", "" );
				curViewRow.Cells["NoteRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Note", "" );

				if ( curViewIdx < curViewIdxMax ) {
					foreach ( DataGridViewCell curCell in curViewRow.Cells ) {
						if ( curCell.Visible && !( curCell.ReadOnly ) ) {
							if ( !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "RerideRecap" ) )
								&& !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "RerideReasonRecap" ) )
								&& !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "ScoreProtRecap" ) )
								) {
								curCell.ReadOnly = true;
							}
						}
					}
				} else {
					try {
						JumpSpeedSelect.CurrentValue = Convert.ToInt16( myRecapRow.Cells["BoatSpeedRecap"].Value.ToString() );
					} catch {
						JumpSpeedSelect.CurrentValue = myMaxSpeed;
					}
				}
				myRecapRow = curViewRow;
			}

			/* 
			 * Retrieve boat path data
			 */
			if ( HelperFunctions.getViewRowColValue( myRecapRow, "BoatSplitTime2Recap", "" ).Length > 0
				|| HelperFunctions.getViewRowColValue( myRecapRow, "BoatSplitTimeRecap", "" ).Length > 0
				|| HelperFunctions.getViewRowColValue( myRecapRow, "BoatEndTimeRecap", "" ).Length > 0
				) {
				loadBoatPathDataGridView( "Jump"
					, HelperFunctions.getViewRowColValue( myRecapRow, "MemberIdRecap", "" )
					, HelperFunctions.getViewRowColValue( myRecapRow, "RoundRecap", "" )
					, HelperFunctions.getViewRowColValue( myRecapRow, "PassNumRecap", "" ) );
			}

			myJumpSetAnalysis = new JumpSetAnalysis( curEventRegRow, jumpRecapDataGridView.Rows, myRecapRow );
			Cursor.Current = Cursors.Default;
		}

		private void setEventRegRowStatus( String inStatus ) {
			setEventRegRowStatus( inStatus, TourEventRegDataGridView.Rows[myEventRegViewIdx] );
		}
		private void setEventRegRowStatus( String inStatus, DataGridViewRow inRow ) {
			DataGridViewRow curRow = inRow;
			if ( inStatus.Equals( "4-Done" ) || inStatus.Equals( "Done" ) || inStatus.Equals( "Complete" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
				curRow.Cells["SkierName"].Style.Font = curFont;
				curRow.Cells["SkierName"].Style.ForeColor = Color.DarkBlue;
				curRow.Cells["SkierName"].Style.BackColor = SystemColors.Window;
				curRow.Cells["Status"].Value = "4-Done";
			
			} if ( inStatus.Equals( "2-InProg" ) || inStatus.Equals( "InProg" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
				curRow.Cells["SkierName"].Style.Font = curFont;
				curRow.Cells["SkierName"].Style.ForeColor = Color.White;
				curRow.Cells["SkierName"].Style.BackColor = Color.LimeGreen;
				curRow.Cells["Status"].Value = "2-InProg";
			
			} if ( inStatus.Equals( "3-Error" ) || inStatus.Equals( "Error" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
				curRow.Cells["SkierName"].Style.Font = curFont;
				curRow.Cells["SkierName"].Style.ForeColor = Color.White;
				curRow.Cells["SkierName"].Style.BackColor = Color.Red;
				curRow.Cells["Status"].Value = "3-Error";
			
			} if ( inStatus.Equals( "1-TBD" ) || inStatus.Equals( "TBD" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Regular );
				curRow.Cells["SkierName"].Style.Font = curFont;
				curRow.Cells["SkierName"].Style.ForeColor = SystemColors.ControlText;
				curRow.Cells["SkierName"].Style.BackColor = SystemColors.Window;
				curRow.Cells["Status"].Value = "1-TBD";
			}
		}

		private void jumpRecapDataGridView_Leave( object sender, EventArgs e ) {
			jumpRecapDataGridView.EndEdit();
		}

		private void jumpRecapDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView)sender;
			myRecapRow = curView.Rows[e.RowIndex];
			if ( HelperFunctions.isObjectEmpty( (String)myRecapRow.Cells["PKRecap"].Value ) ) return;

			myJumpSetAnalysis = new JumpSetAnalysis( TourEventRegDataGridView.Rows[myEventRegViewIdx], jumpRecapDataGridView.Rows, myRecapRow );
			Int64 curPK = Convert.ToInt64( (String)myRecapRow.Cells["PKRecap"].Value );
			if ( curPK > 0 ) skierPassMsg.Text = (String)myRecapRow.Cells["RerideReasonRecap"].Value;

			/* 
			 * Rettrieve boat path data
			 */
			if ( HelperFunctions.getViewRowColValue( myRecapRow, "BoatSplitTime2Recap", "" ).Length > 0 
				|| HelperFunctions.getViewRowColValue( myRecapRow, "BoatSplitTimeRecap", "" ).Length > 0
				|| HelperFunctions.getViewRowColValue( myRecapRow, "BoatEndTimeRecap", "" ).Length > 0
				) {
				loadBoatPathDataGridView( "Jump"
					, HelperFunctions.getViewRowColValue( myRecapRow, "MemberIdRecap", "" )
					, HelperFunctions.getViewRowColValue( myRecapRow, "RoundRecap", "" )
					, HelperFunctions.getViewRowColValue( myRecapRow, "PassNumRecap", "" ) );
			}

		}

		private void jumpRecapDataGridView_KeyUp( object sender, KeyEventArgs e ) {
			DataGridView curView = (DataGridView)sender;

			if ( e.KeyCode == Keys.Enter ) {
				if ( isRecapRowEnterHandled || isAddRecapRowInProg ) {
					isAddRecapRowInProg = false;
					isRecapRowEnterHandled = false;
				
				} else {
					if  ( ((DataGridView)sender ).CurrentCell == null ) return;
					String curColName = curView.Columns[( (DataGridView)sender ).CurrentCell.ColumnIndex].Name;
					if ( ( curColName.Equals( "ScoreFeetRecap" ) || curColName.Equals( "ScoreMetersRecap" ) )
							&& ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
								|| myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" ) )
						) {
					} else {
						SendKeys.Send( "{TAB}" );
					}
				}
				e.Handled = true;

			} else if ( e.KeyCode == Keys.Tab ) {
				if ( isAddRecapRowInProg ) {
					isAddRecapRowInProg = false;
					e.Handled = true;
				} else {
					e.Handled = true;
				}

			} else if ( e.KeyCode == Keys.Escape ) {
				isAddRecapRowInProg = false;
				e.Handled = true;

			} else if ( e.KeyCode == Keys.Delete ) {
				isAddRecapRowInProg = false;
				if ( !( curView.CurrentCell.ReadOnly ) ) {
					curView.CurrentCell.Value = "";
					e.Handled = true;
				}

			} else {
				isAddRecapRowInProg = false;
			}
		}

		private void jumpRecapDataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {
			if ( myRecapRow == null || e.RowIndex < 0 || e.RowIndex < myRecapRow.Index ) return;
			if ( jumpRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly ) return;

			DataGridView curView = (DataGridView)sender;
			DataGridViewRow curViewRow = curView.Rows[e.RowIndex];
			String curColumnName = curView.Columns[e.ColumnIndex].Name;
			String curBoatSplitTime = (String)curViewRow.Cells["BoatSplitTimeRecap"].Value;
			String curBoatSplitTime2 = (String)curViewRow.Cells["BoatSplitTime2Recap"].Value;
			String curBoatEndTime = (String)curViewRow.Cells["BoatEndTimeRecap"].Value;

			if ( curColumnName.Equals( "ReturnToBaseRecap" )
				 || curColumnName.Equals( "RerideRecap" )
			   ) {
					SendKeys.Send( "{TAB}" );

			} else if ( curColumnName.StartsWith( "TimeInTolImg" ) ) {
				if ( curBoatSplitTime.Length == 0 && curBoatSplitTime2.Length == 0 && curBoatEndTime.Length == 0
					&& WscHandler.isConnectActive
					) checkTimeFromBpms( curViewRow, e.ColumnIndex );

			} else if ( curColumnName.StartsWith( "ScoreFeetRecap" ) && curColumnName.StartsWith( "ScoreMetersRecap" ) ) {
				if ( curBoatSplitTime.Length > 0 && curBoatSplitTime2.Length > 0 && curBoatEndTime.Length > 0
					&& WscHandler.isConnectActive
					) checkTimeFromBpms( curViewRow, e.ColumnIndex );
			}
		}

		private void checkTimeFromBpms( DataGridViewRow curViewRow, int colIdx ) {
			if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" )
				&& ( (String)curViewRow.Cells["BoatSplitTimeRecap"].Value ).Length > 0
				&& ( (String)curViewRow.Cells["BoatSplitTime2Recap"].Value ).Length > 0
				) {
				checkRoundCalcSkierScore();
			
			} else if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
				&& ( (String)curViewRow.Cells["BoatSplitTimeRecap"].Value ).Length > 0
				&& ( (String)curViewRow.Cells["BoatSplitTime2Recap"].Value ).Length > 0
				&& ( (String)curViewRow.Cells["BoatEndTimeRecap"].Value ).Length > 0 
				) {
				checkRoundCalcSkierScore();
			}

			if ( WscHandler.useJumpTimes ) {
				if ( HelperFunctions.isObjectEmpty(  HelperFunctions.getViewRowColValue(curViewRow, "BoatSplitTimeRecap", "" ) )
					&& HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( curViewRow, "BoatSplitTime2Recap", "" ) )
					&& HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( curViewRow, "BoatEndTimeRecap", "" ) )
					&& !( ( (String)curViewRow.Cells["SkierBoatPathRecap"].Value ).Equals( "CS" ) )
					) {
					Decimal[] curBoatTimes = WscHandler.getBoatTime( "Jump", (String)curViewRow.Cells["MemberIdRecap"].Value
						, (String)curViewRow.Cells["RoundRecap"].Value, (String)curViewRow.Cells["PassNumRecap"].Value
						, Convert.ToDecimal( "0.0" ), Convert.ToInt16( jumpRecapDataGridView.CurrentRow.Cells["BoatSpeedRecap"].Value.ToString() ), 0 );
					if ( curBoatTimes.Length > 0 ) {
						isDataModified = true;
						myRecapRow.Cells["BoatSplitTimeRecap"].Value = curBoatTimes[0].ToString( "#0.00" );
						myRecapRow.Cells["BoatSplitTime2Recap"].Value = curBoatTimes[1].ToString( "#0.00" );

						if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
							|| myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" )
							) {
							myRecapRow.Cells["BoatSplitTimeRecap"].Value = curBoatTimes[0].ToString( "#0.00" );
							myRecapRow.Cells["BoatSplitTime2Recap"].Value = curBoatTimes[1].ToString( "#0.00" );
							myRecapRow.Cells["BoatEndTimeRecap"].Value = curBoatTimes[2].ToString( "#0.00" );
						}
						checkNeedTimeValidate();
					}
				}
			}

			/* 
			 * Retrieve boat path data
			 * Note: No monitoring for collegiate split boat path
			 */
			if ( WscHandler.isConnectActive
				&& myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" )
				&& ( (String)curViewRow.Cells["BoatSplitTimeRecap"].Value ).Length > 0
				&& ( (String)curViewRow.Cells["BoatSplitTime2Recap"].Value ).Length > 0
				&& ( (String)curViewRow.Cells["BoatEndTimeRecap"].Value ).Length > 0
				&& ( (String)curViewRow.Cells["ScoreFeetRecap"].Value ).Length == 0
				&& ( (String)curViewRow.Cells["ScoreMetersRecap"].Value ).Length == 0
					) {
				Decimal[] curScores = WscHandler.getJumpMeasurement( "Jump", (String)curViewRow.Cells["MemberIdRecap"].Value, (String)curViewRow.Cells["RoundRecap"].Value, (String)curViewRow.Cells["PassNumRecap"].Value );
				if ( curScores.Length > 0 ) {
					myRecapRow.Cells["ScoreFeetRecap"].Value = curScores[0].ToString( "##0" );
					myRecapRow.Cells["ScoreMetersRecap"].Value = curScores[1].ToString( "#0.0" );
					checkRoundCalcSkierScore();
				}
			}

			if ( !( ( (String)curViewRow.Cells["SkierBoatPathRecap"].Value ).Equals( "CS" ) ) ) {
				loadBoatPathArgs = new string[] { "Jump"
					, HelperFunctions.getViewRowColValue( curViewRow, "MemberIdRecap", "")
					, HelperFunctions.getViewRowColValue( curViewRow, "RoundRecap", "")
					, HelperFunctions.getViewRowColValue( curViewRow, "PassNumRecap", "") };
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 5;
				curTimerObj.Tick += new EventHandler( loadBoatPathDataTimer );
				curTimerObj.Start();
			}
		}

		private void jumpRecapDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView)sender;
			myRecapRow = (DataGridViewRow)curView.Rows[e.RowIndex];
			String curColName = curView.Columns[e.ColumnIndex].Name;
			if ( curColName.StartsWith( "Meter" )
				|| curColName.Equals( "BoatEndTimeRecap" )
				|| curColName.Equals( "BoatSplitTimeRecap" )
				|| curColName.Equals( "BoatSplitTime2Recap" )
				) {
				myOrigCellValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );

			} else if ( curColName.Equals( "ScoreFeetRecap" )
			   || curColName.Equals( "ScoreMetersRecap" ) ) {
				myOrigCellValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );

				if ( WscHandler.isConnectActive
					&& myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" )
					&& !(HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "BoatSplitTimeRecap", "" ) ) )
					&& HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( myRecapRow, "BoatSplitTime2Recap", "" ) )
					&& HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( myRecapRow, "BoatEndTimeRecap", "" ) )
					&& HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreFeetRecap", "" ) )
					&& HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreMetersRecap", "" ) )
						) {
					Decimal[] curScores = WscHandler.getJumpMeasurement( "Jump"
						, HelperFunctions.getViewRowColValue( myRecapRow, "MemberIdRecap", "" )
						, HelperFunctions.getViewRowColValue( myRecapRow, "RoundRecap", "" )
						, HelperFunctions.getViewRowColValue( myRecapRow, "PassNumRecap", "" ) );
					if ( curScores.Length > 0 ) {
						myRecapRow.Cells["ScoreFeetRecap"].Value = curScores[0].ToString( "##0" );
						myRecapRow.Cells["ScoreMetersRecap"].Value = curScores[1].ToString( "#0.0" );
						checkRoundCalcSkierScore();
					}
				}

			} else if ( curColName.Equals( "ScoreProtRecap" )
				|| curColName.Equals( "RerideRecap" )
				|| curColName.Equals( "ReturnToBaseRecap" )
				|| curColName.Equals( "ResultsRecap" )
				) {
				try {
					myOrigCellValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
				} catch {
					myOrigCellValue = "";
				}

			} else if ( curColName.Equals( "RerideReasonRecap" ) ) {
				if ( myRecapRow.Cells["Updated"].Value.Equals( "Y" ) ) checkRoundCalcSkierScore();

			}
		}

		private void loadBoatPathDataTimer( object sender, EventArgs e ) {
			if ( loadBoatPathArgs == null ) return;
			if ( loadBoatPathArgs.Length < 4 ) return;

			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( loadBoatPathDataTimer );
			//loadBoatPathArgs
			loadBoatPathDataGridView( loadBoatPathArgs[0], loadBoatPathArgs[1], loadBoatPathArgs[2], loadBoatPathArgs[3] );
			loadBoatPathArgs = null;
		}
		
		private void InvalidateBoatPathButton_Click( object sender, EventArgs e ) {
			if ( jumpRecapDataGridView.Rows.Count == 0 ) {
				MessageBox.Show( "No recap row specified" );
				return;
			}
			if ( jumpRecapDataGridView.CurrentCell.RowIndex != ( jumpRecapDataGridView.Rows.Count - 1 ) ) {
				MessageBox.Show( "Only the current pass can be invalidated" );
				return;
			}

			DataGridViewRow curViewRow = jumpRecapDataGridView.Rows[jumpRecapDataGridView.CurrentCell.RowIndex];
			try {
				WscHandler.invalidateBoatPath( "Jump"
					, HelperFunctions.getViewRowColValue( curViewRow, "MemberIdRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "RoundRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "PassNumRecap", "" ) );
				WscHandler.invalidateBoatTime( "Jump"
					, HelperFunctions.getViewRowColValue( curViewRow, "MemberIdRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "RoundRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "PassNumRecap", "" ) );
				boatPathDataGridView.Rows.Clear();
				boatPathDataGridView.Visible = false;
				InvalidateBoatPathButton.Visible = false;
				InvalidateBoatPathButton.Enabled = false;
				BpmsDriverLabel.Visible = false;
				BpmsDriverLabel.Enabled = false;
				BpmsDriver.Visible = false;
				BpmsDriver.Enabled = false;
				BpmsDriver.Text = "";

				curViewRow.Cells["BoatPathGoodRecap"].Value = "Y";
				curViewRow.Cells["ScoreProtRecap"].Value = "N";
				curViewRow.Cells["RerideRecap"].Value = "N";
				curViewRow.Cells["ProtectedScoreRecap"].Value = "";

			} catch ( Exception ex ) {
				MessageBox.Show( "Attempt to invalidate boat path data failed: " + ex.Message );
				return;
			}
		}

		/*
		 * Retrieve data for current tournament
		 * Used for initial load and to refresh data after updates
		 */
		private void loadBoatPathDataGridView( String curEvent, String curMemberId, String curRound, String curPassNum ) {
			winStatusMsg.Text = "Retrieving boat times";
			boatPathDataGridView.Visible = false;
			boatPathDataGridView.Visible = false;
			InvalidateBoatPathButton.Visible = false;
			InvalidateBoatPathButton.Enabled = false;
			BpmsDriverLabel.Visible = false;
			BpmsDriverLabel.Enabled = false;
			BpmsDriver.Visible = false;
			BpmsDriver.Enabled = false;
			BpmsDriver.Text = "";
			Cursor.Current = Cursors.WaitCursor;

			/*
			 */

			try {
				boatPathDataGridView.Rows.Clear();
				if ( jumpRecapDataGridView.CurrentRow == null && !( curMemberId.Equals( "000000000" ) ) ) {
					boatPathDataGridView.Visible = false;
					return;
				}
				Int16 curPassSpeedKph;
				if ( curMemberId.Equals( "000000000" ) ) {
					curPassSpeedKph = (Int16)57;
				} else {
					curPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValue( myRecapRow, "BoatSpeedRecap", "57" ) );
				}
				myBoatPathDataRow = WscHandler.getBoatPath( curEvent, curMemberId, curRound, curPassNum, Convert.ToDecimal( "0.0" ), curPassSpeedKph );
				if ( myBoatPathDataRow == null ) {
					boatPathDataGridView.Visible = false;
					InvalidateBoatPathButton.Visible = false;
					InvalidateBoatPathButton.Enabled = false;
					BpmsDriverLabel.Visible = false;
					BpmsDriverLabel.Enabled = false;
					BpmsDriver.Visible = false;
					BpmsDriver.Enabled = false;
					BpmsDriver.Text = "";
					return;
				}

				Font curFontBold = new Font( "Arial Narrow", 9, FontStyle.Bold );
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Regular );
				decimal curDevNT = 0, curDevMT = 0, curDevET = 0;

				/*
				 * CUT = mt – nt
				 * FLIGHT = et-mt
				 * CUMULATIVE = CUT + FLIGHT
				 */
				int curViewIdx = -1;
				while ( curViewIdx < 5 ) {
					curViewIdx = boatPathDataGridView.Rows.Add();
					DataGridViewRow curViewRow = boatPathDataGridView.Rows[curViewIdx];
					curViewRow.Cells["boatPathBuoy"].Style.Font = curFont;
					curViewRow.Cells["boatTimeBuoy"].Style.Font = curFont;
					curViewRow.Cells["boatTimeBuoy"].Style.ForeColor = Color.DarkGreen;
					curViewRow.Cells["boatPathDev"].Style.Font = curFont;
					curViewRow.Cells["boatPathDev"].Style.ForeColor = Color.DarkGreen;
					curViewRow.Cells["boatPathZone"].Style.Font = curFont;
					curViewRow.Cells["boatPathZone"].Style.ForeColor = Color.DarkGreen;
					curViewRow.Cells["boatPathZoneTol"].Style.Font = curFont;
					curViewRow.Cells["boatPathZoneTol"].Style.ForeColor = Color.DarkGray;
					curViewRow.Cells["boatPathNote"].Style.Font = curFontBold;
					curViewRow.Cells["boatPathNote"].Style.ForeColor = Color.Black;
					curViewRow.Cells["boatPathCutFlightCum"].Style.Font = curFont;
					curViewRow.Cells["boatPathCutFlightCum"].Style.ForeColor = Color.DarkGreen;
					

					//180,ST,NT,MT,ET and EC is good for me as points
					if ( curViewIdx == 0 ) {
						curViewRow.Cells["boatPathBuoy"].Value = "180M";

					} else if ( curViewIdx == 1 ) {
						curViewRow.Cells["boatPathBuoy"].Value = "ST";

					} else if ( curViewIdx == 2 ) {
						curViewRow.Cells["boatPathBuoy"].Value = "52M";
						curDevNT = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "PathDevBuoy" + curViewIdx, 0 );

					} else if ( curViewIdx == 3 ) {
						curViewRow.Cells["boatPathBuoy"].Value = "82M";
						curDevMT = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "PathDevBuoy" + curViewIdx, 0 ); 
						curViewRow.Cells["boatPathNote"].Value = "Cut";
						curViewRow.Cells["boatPathCutFlightCum"].Value = (curDevMT - curDevNT);

					} else if ( curViewIdx == 4 ) {
						curViewRow.Cells["boatPathBuoy"].Value = "41M";
						curDevET = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "PathDevBuoy" + curViewIdx, 0 );
						curViewRow.Cells["boatPathNote"].Value = "Flight";
						curViewRow.Cells["boatPathCutFlightCum"].Value = ( curDevET - curDevMT );

					} else if ( curViewIdx == 5 ) {
						curViewRow.Cells["boatPathBuoy"].Value = "EC";
						curViewRow.Cells["boatPathNote"].Value = "Cumul";
						curViewRow.Cells["boatPathCutFlightCum"].Value = ( curDevET - curDevNT );
					}

					curViewRow.Cells["boatPathDev"].Value = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "PathDevBuoy" + curViewIdx, 0 );
					if ( curViewIdx > 1 && curViewIdx < 5 && ( myBoatPathDataRow["boatTimeBuoy" + ( curViewIdx - 1 )] != System.DBNull.Value ) ) {
						curViewRow.Cells["boatTimeBuoy"].Value = HelperFunctions.getDataRowColValueDecimal( myBoatPathDataRow, "boatTimeBuoy" + ( curViewIdx - 1 ), 0 );
					}
				}

				if ( curViewIdx >= 0 ) {
					boatPathDataGridView.Visible = true;
					InvalidateBoatPathButton.Visible = true;
					InvalidateBoatPathButton.Enabled = true;
					BpmsDriverLabel.Visible = true;
					BpmsDriverLabel.Enabled = true;
					BpmsDriver.Visible = true;
					BpmsDriver.Enabled = true;
					BpmsDriver.Text = "";
					BpmsDriver.Text = HelperFunctions.getDataRowColValue( myBoatPathDataRow, "DriverName", "" );
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "Error retrieving boat times \n" + ex.Message );

			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		private void jumpRecapDataGridView_SetNewRowCell( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( jumpRecapDataGridView_SetNewRowCell );

			if ( jumpRecapDataGridView.Rows.Count > 0 ) {
				int rowIndex = jumpRecapDataGridView.Rows.Count - 1;
				jumpRecapDataGridView.Select();
				jumpRecapDataGridView.CurrentCell = jumpRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex];
				JumpSpeedSelect.CurrentValue = Convert.ToInt16( jumpRecapDataGridView.CurrentRow.Cells["BoatSpeedRecap"].Value.ToString() );
			}

		}

		private void jumpRecapDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
			DataGridView curView = (DataGridView)sender;
			if ( myRecapRow.Index != e.RowIndex ) return; 
			myRecapRow.ErrorText = "";
			isRecapRowEnterHandled = false;

			String curColName = curView.Columns[e.ColumnIndex].Name;
			if ( myRecapRow.Cells[e.ColumnIndex].Value == null ) return;

			if ( curColName.StartsWith( "Meter" ) ) {
				if ( HelperFunctions.isObjectEmpty( e.FormattedValue ) ) return;

				try {
					myModCellValue = "";
					Decimal curCellNumValue = Convert.ToDecimal( e.FormattedValue );
					String curCellValue = (String)e.FormattedValue;
					int delimPos = curCellValue.IndexOf( '.' );
					if ( delimPos < 0 ) {
						myModCellValue = curCellValue.Substring( 0, curCellValue.Length - 1 ) + "." + curCellValue.Substring( curCellValue.Length - 1, 1 );
					} else {
						myModCellValue = curCellValue;
					}

				} catch ( Exception ex ) {
					myRecapRow.ErrorText = String.Format("Value in {0} must be numeric: {1}", curColName, ex.Message);
					MessageBox.Show( myRecapRow.ErrorText );
					e.Cancel = true;
				}

			} else if ( curColName.Equals( "BoatSplitTimeRecap" ) 
				|| curColName.Equals( "BoatSplitTime2Recap" ) 
				|| curColName.Equals( "BoatEndTimeRecap" ) ) {
				String curValue = (String)e.FormattedValue;
				if ( HelperFunctions.isObjectEmpty( curValue ) || curValue.Equals( myOrigCellValue ) ) return;

				myModCellValue = "";
				if ( curValue.ToUpper().Equals( "OK" ) ) {
					myModCellValue = "";
				} else if ( curValue.ToUpper().Equals( "NONE" ) ) {
					myModCellValue = "";
				} else {
					try {
						Decimal curCellNumValue = Convert.ToDecimal( curValue );
					} catch ( Exception ex ) {
						myRecapRow.ErrorText = String.Format( "Value in {0} must be numeric: {1}", curColName, ex.Message );
						MessageBox.Show( myRecapRow.ErrorText );
						e.Cancel = true;
					}
				}

			} else if ( curColName.Equals( "ScoreFeetRecap" ) ) {
				if ( ( myNumJudges > 0 ) || HelperFunctions.isObjectEmpty( e.FormattedValue ) ) return;

				try {
					Decimal scoreFeet = Convert.ToDecimal( e.FormattedValue );

					String curCellValue = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreMetersRecap", "" );
					if ( HelperFunctions.isObjectEmpty( curCellValue ) ) return;

					// Check that feet and meters are properly equivalent, otherwise error
					Decimal scoreMeters = Convert.ToDecimal( curCellValue );
					String curMsg = CalcDistValidate( scoreFeet, scoreMeters );
					if ( curMsg.Length > 0 ) {
						myRecapRow.ErrorText = curMsg;
						MessageBox.Show( myRecapRow.ErrorText );
						e.Cancel = true;
					}

				} catch ( Exception ex ) {
					myRecapRow.ErrorText = String.Format( "Value in {0} must be numeric: {1}", curColName, ex.Message );
					MessageBox.Show( myRecapRow.ErrorText );
					e.Cancel = true;
				}

			} else if ( curColName.Equals( "ScoreMetersRecap" ) ) {
				if ( ( myNumJudges > 0 ) || HelperFunctions.isObjectEmpty( e.FormattedValue ) ) return;

				try {
					//isRecapRowEnterHandled = true;
					//Decimal scoreMeters = Convert.ToDecimal( e.FormattedValue );

					String curCellValue = (String)e.FormattedValue;
					int delimPos = curCellValue.IndexOf( '.' );
					if ( delimPos < 0 ) {
						myModCellValue = curCellValue.Substring( 0, curCellValue.Length - 1 ) + "." + curCellValue.Substring( curCellValue.Length - 1, 1 );
					} else {
						myModCellValue = curCellValue;
					}
					Decimal scoreMeters = Convert.ToDecimal( myModCellValue );

					curCellValue = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreFeetRecap", "" );
					if ( HelperFunctions.isObjectEmpty( curCellValue ) ) return;
					Decimal scoreFeet = Convert.ToDecimal( (String)myRecapRow.Cells["ScoreFeetRecap"].Value );

					// Check that feet and meters are properly equivalent, otherwise error
					String curMsg = CalcDistValidate( scoreFeet, scoreMeters );
					if ( curMsg.Length > 0 ) {
						myRecapRow.ErrorText = curMsg;
						MessageBox.Show( myRecapRow.ErrorText );
						e.Cancel = true;
					}

				} catch ( Exception ex ) {
					myRecapRow.ErrorText = String.Format( "Value in {0} must be numeric: {1}", curColName, ex.Message );
					MessageBox.Show( myRecapRow.ErrorText );
					e.Cancel = true;
				}
			}
		}

		private void jumpRecapDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView)sender;
			myRecapRow.ErrorText = "";
			String curColName = curView.Columns[e.ColumnIndex].Name;

			if ( myRecapRow.Index != e.RowIndex || e.RowIndex < ( curView.Rows.Count - 1 ) ) {
				// Check for changes to information on passes other than the last one
				if ( !( curColName.Equals( "RerideRecap" ) ) ) return;
				if ( ( (String)myRecapRow.Cells["RerideRecap"].Value ).Equals( myOrigCellValue ) ) return;
				jumpRecapDataGridView_CellValidatedPrevRow();
				return;
			}

			if ( curColName.StartsWith( "Meter" ) ) {
				// Validate BoatSplitTime (52M segment) if entered
				jumpRecapDataGridView_CellValidatedMeters( curColName );

			} else if ( curColName.Equals( "BoatSplitTimeRecap" ) 
				|| curColName.Equals( "BoatSplitTime2Recap" ) 
				|| curColName.Equals( "BoatEndTimeRecap" ) ) {
				jumpRecapDataGridView_CellValidatedBoatTimes( curColName );

			} else if ( curColName.Equals( "ScoreFeetRecap" ) ) {
				if ( myNumJudges > 0 ) return;

				String curColValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );
				if ( HelperFunctions.isObjectEmpty( curColValue ) || curColValue.Equals( myOrigCellValue ) ) return;

				myOrigCellValue = (String)myRecapRow.Cells["ScoreFeetRecap"].Value;
				isDataModified = true;
				myRecapRow.Cells["Updated"].Value = "Y";
				if ( HelperFunctions.isObjectPopulated( myRecapRow.Cells["ScoreMetersRecap"].Value ) ) checkNeedTimeValidate();

			} else if ( curColName.Equals( "ScoreMetersRecap" ) ) {
				if ( myNumJudges > 0 ) return;
				
				String curColValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );
				if ( HelperFunctions.isObjectEmpty( curColValue ) || curColValue.Equals( myOrigCellValue ) ) return;

				jumpRecapDataGridView_CellValidatedScoreMeters( curColName, curColValue );

			} else if ( curColName.Equals( "RerideRecap" ) ) {
				String curColValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );
				if ( HelperFunctions.isObjectEmpty( curColValue ) || curColValue.Equals( myOrigCellValue ) ) return;
				jumpRecapDataGridView_CellValidatedReride( curColName, curColValue );

			} else if ( curColName.Equals( "ResultsRecap" ) ) {
				String curColValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );
				jumpRecapDataGridView_CellValidatedResults( curColName, curColValue, e.ColumnIndex );
			
			} else if ( curColName.Equals( "ReturnToBaseRecap" ) ) {
				String curColValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );
				if ( curColValue.Equals( myOrigCellValue ) ) return;

				winStatusMsg.Text = "ReturnToBaseRecap = " + curColValue;
				isDataModified = true;
				myRecapRow.Cells["Updated"].Value = "Y";
				checkNeedTimeValidate();
			}
		}

		/*
		 * Check for changes to information on passes other than the last one
		 * Only indications for rerides are allowed to be updated on these rows
		 */
		private void jumpRecapDataGridView_CellValidatedPrevRow() {
			if ( myRecapRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
				rerideReasonDialogForm.RerideReasonText = (String)myRecapRow.Cells["RerideReasonRecap"].Value;
				if ( rerideReasonDialogForm.ShowDialog() == DialogResult.OK ) {
					String curCommand = rerideReasonDialogForm.Command;
					isDataModified = true;
					myRecapRow.Cells["Updated"].Value = "Y";
					myRecapRow.Cells["RerideReasonRecap"].Value = rerideReasonDialogForm.RerideReasonText;
					myRecapRow.Cells["RerideCanImproveRecap"].Value = "Y";
					if ( curCommand.ToLower().Equals( "updatewithprotect" ) ) myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
				
				} else {
					isDataModified = true;
					myRecapRow.Cells["Updated"].Value = "Y";
					rerideReasonDialogForm.RerideReasonText = "";
					MessageBox.Show( "Reride can not be granted without a reason being specified." );
					myRecapRow.Cells["RerideRecap"].Value = "N";
				}
			
			} else {
				isDataModified = true;
				myRecapRow.Cells["Updated"].Value = "Y";
			}

			checkRoundCalcSkierScore();
		}

		// Validate meter attributes
		private void jumpRecapDataGridView_CellValidatedMeters( String curColName ) {
			String curColValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );
			if ( HelperFunctions.isObjectEmpty( curColValue ) ) {
				if ( curColValue.Equals( myOrigCellValue ) ) return;

				isDataModified = true;
				myRecapRow.Cells["Updated"].Value = "Y";

			} else {
				try {
					Decimal curMeterScore = Convert.ToDecimal( curColValue );
					if ( myModCellValue.Length > 0 ) {
						curMeterScore = Convert.ToDecimal( myModCellValue );
						myRecapRow.Cells[curColName].Value = curMeterScore.ToString( "##0.00" );
						if ( curColValue.Equals( myOrigCellValue ) ) return;

						isDataModified = true;
						myRecapRow.Cells["Updated"].Value = "Y";
					}

				} catch {
					MessageBox.Show( "Meter value must be numeric" );
					return;
				}
			}

			if ( HelperFunctions.isObjectPopulated( myRecapRow.Cells["Meter1Recap"].Value )
				&& HelperFunctions.isObjectPopulated( myRecapRow.Cells["Meter2Recap"].Value )
				&& HelperFunctions.isObjectPopulated( myRecapRow.Cells["Meter3Recap"].Value )
				) {
				if ( myNumJudges == 3 ) {
					bool curCalcComp = CalcDistByMeters();
					checkRoundCalcSkierScore();

				} else if ( myNumJudges == 6 ) {
					if ( HelperFunctions.isObjectPopulated( myRecapRow.Cells["Meter4Recap"].Value )
						&& HelperFunctions.isObjectPopulated( myRecapRow.Cells["Meter5Recap"].Value )
						&& HelperFunctions.isObjectPopulated( myRecapRow.Cells["Meter6Recap"].Value )
						) {
						bool curCalcComp = CalcDistByMeters();
						checkRoundCalcSkierScore();
					}
				}
			}
		}

		// Validate boat times if entered
		private void jumpRecapDataGridView_CellValidatedBoatTimes( String curColName ) {
			String curColValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );

			if ( HelperFunctions.isObjectEmpty( curColValue ) ) {
				myRecapRow.Cells["TimeInTolRecap"].Value = "N";
				isDataModified = true;
				return;
			}

			skierPassMsg.Text = "";
			if ( curColName.Equals( "BoatSplitTimeRecap" ) ) {
				myJumpSetAnalysis.SplitTimeFormat();
			} else if ( curColName.Equals( "BoatSplitTime2Recap" ) ) {
				myJumpSetAnalysis.Split2TimeFormat();
			} else if ( curColName.Equals( "BoatEndTimeRecap" ) ) {
				myJumpSetAnalysis.EndTimeFormat();
			}

			if ( !( curColValue.Equals( myOrigCellValue ) ) ) {
				myOrigCellValue = curColValue;
				isDataModified = true;
			}

			/*
			* If all 3 times have been entered then they will be validated
			* Also if scores available determine if score for round should be updated.
			*/
			checkNeedTimeValidate();
			if ( isDataModified ) myOrigCellValue = HelperFunctions.getViewRowColValue( myRecapRow, curColName, "" );
		}

		private void jumpRecapDataGridView_CellValidatedScoreMeters( String curColName, String curColValue ) {
			if ( HelperFunctions.isObjectEmpty( curColValue ) ) {
				if ( curColValue.Equals( myOrigCellValue ) ) {
					if ( myRecapRow.Cells["Updated"].Value.Equals( "Y" ) ) {
						if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
							|| myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" )
							) {
							checkRoundCalcSkierScore();
						}
					}

				} else {
					myOrigCellValue = curColValue;
					isRecapRowEnterHandled = true;
					isDataModified = true;
					myRecapRow.Cells["Updated"].Value = "Y";
					if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
						|| myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" )
						) {
						checkRoundCalcSkierScore();
					}
				}

			} else {
				try {
					Decimal curScore = Convert.ToDecimal( curColValue );
					if ( myModCellValue.Length > 0 ) {
						curScore = Convert.ToDecimal( myModCellValue );
						myRecapRow.Cells[curColName].Value = curScore.ToString( "#0.0" );
						curColValue = (String)myRecapRow.Cells[curColName].Value;
					}
				} catch {
					MessageBox.Show( "Score for meters must be numeric" );
					return;
				}
				if ( curColValue.Equals( myOrigCellValue ) ) {
					if ( myRecapRow.Cells["Updated"].Value.Equals( "Y" ) ) {
						if ( HelperFunctions.isObjectPopulated( myRecapRow.Cells["ScoreFeetRecap"].Value ) ) {
							if ( Convert.ToDecimal( (String)myRecapRow.Cells["ScoreFeetRecap"].Value ) > 0
								&& Convert.ToDecimal( curColValue ) > 0
							) {
								isDataModified = true;
								checkNeedTimeValidate();
							}
						}

					} else {
						if ( checkRoundContinue() ) {
							isAddRecapRowInProg = true;
							Timer curTimerObj = new Timer();
							curTimerObj.Interval = 15;
							curTimerObj.Tick += new EventHandler( addRecapRowTimer );
							curTimerObj.Start();
						}
					}

				} else {
					myOrigCellValue = curColValue;
					isRecapRowEnterHandled = true;
					isDataModified = true;
					myRecapRow.Cells["Updated"].Value = "Y";
					if ( HelperFunctions.isObjectPopulated( myRecapRow.Cells["ScoreFeetRecap"].Value ) ) {
						if ( Convert.ToDecimal( (String)myRecapRow.Cells["ScoreFeetRecap"].Value ) > 0
							&& Convert.ToDecimal( curColValue ) > 0 ) 
							checkNeedTimeValidate();
					}
				}
			}
		}

		private void jumpRecapDataGridView_CellValidatedReride( String curColName, String curColValue ) {
			if ( curColValue.Equals( "Y" ) ) {
				if ( HelperFunctions.isObjectEmpty( myRecapRow.Cells["ScoreFeetRecap"].Value )
					|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["ScoreMetersRecap"].Value )
					|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTimeRecap"].Value )
					|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatEndTimeRecap"].Value )
					) {
					myRecapRow.Cells["ResultsRecap"].Value = "Pass";
				}
				rerideReasonDialogForm.RerideReasonText = (String)myRecapRow.Cells["RerideReasonRecap"].Value;
				if ( rerideReasonDialogForm.ShowDialog() == DialogResult.OK ) {
					String curCommand = rerideReasonDialogForm.Command;
					isDataModified = true;
					myRecapRow.Cells["Updated"].Value = "Y";
					myRecapRow.Cells["RerideReasonRecap"].Value = rerideReasonDialogForm.RerideReasonText;
					if ( curCommand.ToLower().Equals( "updatewithprotect" ) ) myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
					myRecapRow.Cells["RerideCanImproveRecap"].Value = "Y";
					checkRoundCalcSkierScore( "Y" );

				} else {
					isDataModified = true;
					myRecapRow.Cells["Updated"].Value = "Y";
					rerideReasonDialogForm.RerideReasonText = "";
					MessageBox.Show( "Reride can not be granted without a reason being specified." );
					myRecapRow.Cells["RerideRecap"].Value = "N";
					checkRoundCalcSkierScore();
				}

			} else {
				isDataModified = true;
				myRecapRow.Cells["Updated"].Value = "Y";
				checkRoundCalcSkierScore();
			}
		}

		private void jumpRecapDataGridView_CellValidatedResults( String curColName, String curColValue, int curColIdx ) {
			if ( HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatEndTimeRecap"].Value )
				&& HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTimeRecap"].Value )
				&& HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTime2Recap"].Value )
				) {
				checkTimeFromBpms( myRecapRow, curColIdx );
			}

			if ( curColValue.Equals( "Jump" )
				&& !( curColValue.Equals( myOrigCellValue ) )
				) {
				if ( HelperFunctions.isObjectPopulated( myRecapRow.Cells["BoatEndTimeRecap"].Value )
					&& HelperFunctions.isObjectPopulated( myRecapRow.Cells["BoatSplitTimeRecap"].Value )
					&& HelperFunctions.isObjectPopulated( myRecapRow.Cells["BoatSplitTime2Recap"].Value )
					) {
					isDataModified = true;
					myRecapRow.Cells["Updated"].Value = "Y";
					checkNeedTimeValidate();
				}
			
			} else if ( curColValue.Equals( "Fall" ) && !( curColValue.Equals( myOrigCellValue ) ) ) {
				if ( HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatEndTimeRecap"].Value )
					|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTimeRecap"].Value )
					|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTime2Recap"].Value ) 
					) {
					return;

				} else {
					checkNeedTimeValidate();
				}

			} else if ( curColValue.Equals( "Pass" ) && !( curColValue.Equals( myOrigCellValue ) ) ) {
				if ( HelperFunctions.isObjectPopulated( myRecapRow.Cells["BoatSplitTimeRecap"].Value ) ) {
					isDataModified = true;
					myRecapRow.Cells["Updated"].Value = "Y";
					checkNeedTimeValidate();
				}
			}
		}

		/*
		 * Method used when a time entry attribute for a pass has been modified
		 * Check all 3 time attributes and validate the times if all have been entered
		*/
		private void checkNeedTimeValidate() {
			skierPassMsg.Text = "";
			if ( myJumpSetAnalysis.checkNeedTimeValidate() ) {
				// Check to determine if score for round should be update
				//isDataModified = true;
				checkRoundCalcSkierScore();
			}

			if ( HelperFunctions.isObjectPopulated( myJumpSetAnalysis.TimeTolMsg) ) skierPassMsg.Text = myJumpSetAnalysis.TimeTolMsg;
		}

		/*
		 * Calculate score for jump set and determine if set complete
		 */
		private void checkRoundCalcSkierScore() {
            checkRoundCalcSkierScore("");
        }
        private void checkRoundCalcSkierScore(String inRerideInd) {
			if ( RoundCommpleteUpdateScore() ) {
				saveScore();
				if ( HelperFunctions.isObjectPopulated( inRerideInd ) && inRerideInd.Equals( "Y" ) ) {
					isAddRecapRowInProg = true;
					Timer curTimerObj = new Timer();
					curTimerObj.Interval = 15;
					curTimerObj.Tick += new EventHandler( addRecapRowTimer );
					curTimerObj.Start();
					return;
				}

			} else {
				saveScore();

				isAddRecapRowInProg = true;
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 15;
				curTimerObj.Tick += new EventHandler( addRecapRowTimer );
				curTimerObj.Start();
			}
        }

		private bool checkRoundContinue() {
			if ( myRecapRow.Cells["ResultsRecap"].Value.Equals( "Jump" )
				&& (
				HelperFunctions.isObjectEmpty( scoreFeetTextBox.Text )
				|| HelperFunctions.isObjectEmpty( scoreMetersTextBox.Text )
				|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTimeRecap"].Value )
				|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTime2Recap"].Value )
				|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatEndTimeRecap"].Value )
				) ) {
				return false;
			}

			if ( myRecapRow.Cells["ResultsRecap"].Value.Equals( "fall" )
				&& (
				HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTimeRecap"].Value )
				|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatSplitTime2Recap"].Value )
				|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatEndTimeRecap"].Value )
				) ) {
				return false;
			}

			if ( myRecapRow.Cells["ResultsRecap"].Value.Equals( "Jump" ) && ( scoreFeetTextBox.Text.Length == 0 || scoreMetersTextBox.Text.Length == 0 ) ) {
				return false;
			}

			 return true;
		}

		private void addRecapRowTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( addRecapRowTimer );
            addPass();
        }

        private void addPass() {
            Int16 curPassNum = 0, curBoatSpeed;
            Decimal curRampHeight;
            String curRTBValue = "N";
			String curSkierBoatPath = SkierBoatPathSelect.DefaultValue;;

            isAddRecapRowInProg = true;
            isRecapRowEnterHandled = false;
            addPassButton.Enabled = false;
            DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
            String curMemberId = (String)curEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)curEventRegRow.Cells["AgeGroup"].Value;

            curRampHeight = RampHeightSelect.CurrentValue;
            curBoatSpeed = JumpSpeedSelect.CurrentValue;

            if (jumpRecapDataGridView.Rows.Count > 0) {
                addPassButton.Enabled = false;
                myRecapRow = (DataGridViewRow)jumpRecapDataGridView.Rows[jumpRecapDataGridView.Rows.Count - 1];

				curRTBValue = myRecapRow.Cells["ReturnToBaseRecap"].Value.ToString();
                curPassNum = Convert.ToInt16( jumpRecapDataGridView.Rows.Count + 1 );
				curSkierBoatPath = myRecapRow.Cells["SkierBoatPathRecap"].Value.ToString();

			} else {
                scoreEntryInprogress();
                curPassNum = 1;
				curSkierBoatPath = SkierBoatPathSelect.CurrentValue;

				if (ExportLiveScoreboard.ScoreboardLocation.Length > 1) {
                    String curSkierName = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value;
                    String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                    ExportLiveScoreboard.exportCurrentSkierJump(JumpEventData.mySanctionNum, curMemberId, curAgeGroup, curTeamCode, Convert.ToByte(roundActiveSelect.RoundValue),
                        curSkierName, JumpSpeedSelect.CurrentValueDesc, RampHeightSelect.CurrentValueDesc, curPassNum.ToString("#0"), null, null, null, null, null, null);
                }
            }

			int curViewIdx = jumpRecapDataGridView.Rows.Add();
            myRecapRow = jumpRecapDataGridView.Rows[curViewIdx];
			myJumpSetAnalysis = new JumpSetAnalysis( curEventRegRow, jumpRecapDataGridView.Rows, myRecapRow );

			myRecapRow.Cells["PKRecap"].Value = "-1";
            myRecapRow.Cells["SanctionIdRecap"].Value = JumpEventData.mySanctionNum;
            myRecapRow.Cells["MemberIdRecap"].Value = curMemberId;
            myRecapRow.Cells["AgeGroupRecap"].Value = curAgeGroup;
            myRecapRow.Cells["RoundRecap"].Value = roundActiveSelect.RoundValue;
            myRecapRow.Cells["PassNumRecap"].Value = curPassNum.ToString("#0");
            myRecapRow.Cells["BoatSpeedRecap"].Value = curBoatSpeed.ToString("00");
            myRecapRow.Cells["RampHeightRecap"].Value = curRampHeight.ToString("0.0");

            myRecapRow.Cells["Meter1Recap"].Value = "";
            myRecapRow.Cells["Meter2Recap"].Value = "";
            myRecapRow.Cells["Meter3Recap"].Value = "";
            myRecapRow.Cells["Meter4Recap"].Value = "";
            myRecapRow.Cells["Meter5Recap"].Value = "";
            myRecapRow.Cells["Meter6Recap"].Value = "";
            myRecapRow.Cells["ScoreTriangleRecap"].Value = "";
            myRecapRow.Cells["ScoreFeetRecap"].Value = "";
            myRecapRow.Cells["ScoreMetersRecap"].Value = "";

            myRecapRow.Cells["BoatSplitTimeRecap"].Value = "";
            myRecapRow.Cells["BoatSplitTime2Recap"].Value = "";
            myRecapRow.Cells["BoatEndTimeRecap"].Value = "";

            myRecapRow.Cells["Split52TimeTolRecap"].Value = "0";
            myRecapRow.Cells["Split82TimeTolRecap"].Value = "0";
            myRecapRow.Cells["Split41TimeTolRecap"].Value = "0";

            myRecapRow.Cells["RerideRecap"].Value = "N";
            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapRow.Cells["ReturnToBaseRecap"].Value = curRTBValue;
			myRecapRow.Cells["SkierBoatPathRecap"].Value = curSkierBoatPath;

			myRecapRow.Cells["ResultsRecap"].Value = "Jump";
            myRecapRow.Cells["RerideReasonRecap"].Value = "";
            myRecapRow.Cells["NoteRecap"].Value = "";
            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.clock;
            myRecapRow.Cells["Updated"].Value = "N";
            isDataModified = false;
            isAddRecapRowInProg = false;

            myPassRunCount++;
            if ( curViewIdx == 0 ) {
                mySkierRunCount++;
            } else {
                if ( mySkierRunCount == 0 ) {
                    mySkierRunCount++;
                }
            }

            curViewIdx = 0;
            int curViewIdxMax = jumpRecapDataGridView.Rows.Count - 1;
            foreach ( DataGridViewRow curViewRow in jumpRecapDataGridView.Rows ) {
                curViewIdx = curViewRow.Index;
                if ( curViewIdx < curViewIdxMax ) {
                    foreach ( DataGridViewCell curCell in curViewRow.Cells ) {
                        if ( curCell.Visible && !( curCell.ReadOnly ) ) {
                            if ( !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "RerideRecap" ) )
                                && !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "RerideReasonRecap" ) )
                                && !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "ScoreProtRecap" ) )
                                ) {
                                curCell.ReadOnly = true;
                            }
                        }
                    }
                }
            }

			sendPassDataEwsc( TourEventRegDataGridView.Rows[myEventRegViewIdx] );

			Timer curTimerObj = new Timer();
            curTimerObj.Interval = 15;
            curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
            curTimerObj.Start();
        }

		private void ResendPassButton_Click( object sender, EventArgs e ) {
			sendPassDataEwsc( TourEventRegDataGridView.Rows[myEventRegViewIdx] );
		}

		private void sendPassDataEwsc( DataGridViewRow inTourEventRegRow ) {
			if ( WscHandler.isConnectActive ) {
				String skierFed = HelperFunctions.getDataRowColValue(JumpEventData.myTourRow, "Federation", "" );
				if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( inTourEventRegRow, "Federation", "" ) ) ) {
					skierFed = HelperFunctions.getViewRowColValue( inTourEventRegRow, "Federation", "" );
				}

				WscHandler.sendPassData( (String)myRecapRow.Cells["MemberIdRecap"].Value
					, HelperFunctions.getViewRowColValue( inTourEventRegRow, "SkierName", "" )
					, "Jump", skierFed
					, HelperFunctions.getViewRowColValue( inTourEventRegRow, "State", "" )
					, HelperFunctions.getViewRowColValue( inTourEventRegRow, "EventGroup", "" )
					, HelperFunctions.getViewRowColValue( inTourEventRegRow, "AgeGroup", "" )
					, HelperFunctions.getViewRowColValue( inTourEventRegRow, "Gender", "" )
					, (String)scoreEventClass.SelectedValue, roundActiveSelect.RoundValue
					, Convert.ToInt16( HelperFunctions.getViewRowColValue( myRecapRow, "PassNumRecap", "0" ) )
					, Convert.ToInt16( HelperFunctions.getViewRowColValue( myRecapRow, "BoatSpeedRecap", "25" ) )
					, "", HelperFunctions.getViewRowColValue( myRecapRow, "SkierBoatPathRecap", "" )
					, (String)driverDropdown.SelectedValue );
			
			} else if ( !WaterskiConnectLabel.Visible ) WaterskiConnectLabel.Visible = false;
		}

		private void SimulationPassButton_Click( object sender, EventArgs e ) {
			String curSimMemberId = "000000000";
			Int16 curPassSpeedKph = 57;

			if ( SimulationPassButton.Text.ToLower().Equals( "check bpms" ) ) {
				loadBoatPathDataGridView( "Jump", curSimMemberId, "1", "1" );
				if ( InvalidateBoatPathButton.Visible ) {
					SimulationPassButton.Text = "Simulation Pass";
				} else {
					DialogResult dialogResult = MessageBox.Show( "No deviations found.  Do you want to reset the dialog?", "Simulation Reset", MessageBoxButtons.YesNo );
					if ( dialogResult == DialogResult.Yes ) SimulationPassButton.Text = "Simulation Pass";
				}

			} else {
				StringBuilder curSqlStmt = new StringBuilder( String.Format( "Delete From BoatPath Where SanctionId = '{0}' AND MemberId = '{1}' AND Event = 'Jump'", JumpEventData.mySanctionNum, curSimMemberId ) );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				curSqlStmt = new StringBuilder( String.Format( "Delete From BoatTime Where SanctionId = '{0}' AND MemberId = '{1}' AND Event = 'Jump'", JumpEventData.mySanctionNum, curSimMemberId ) );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				SimulationPassButton.Text = "Check BPMS";
				WscHandler.sendPassData( curSimMemberId
					, "Simulation Pass"
					, "Jump"
					, "USA"
					, ""
					, ""
					, ""
					, "n"
					, ""
					, "1"
					, 1
					, curPassSpeedKph
					, ""
					, "S"
					, (String)driverDropdown.SelectedValue );
			}

		}

		private String CalcDistValidate( Decimal inScoreFeet, Decimal inScoreMeters ) {
            String returnMsg = "";
            if ( ( inScoreFeet > 0 ) && ( inScoreMeters > 0 ) ) {
                Decimal convScoreMeters = Convert.ToDecimal( inScoreFeet / myMetericFactor );
                convScoreMeters = Math.Round( convScoreMeters, 1 );
                Decimal convScoreMetersMin = convScoreMeters - .55M;
                Decimal convScoreMetersMax = convScoreMeters + .55M;
                if ( ( inScoreMeters >= convScoreMetersMin ) && ( inScoreMeters <= convScoreMetersMax ) ) {
                    returnMsg = "";
                } else {
                    Decimal convScoreFeet = Convert.ToDecimal( inScoreMeters * myMetericFactor );
                    returnMsg = "Meters don't convert to recorded feet plus or minus .55M" 
                    + "\n Input " + inScoreMeters + " meters converts to " + convScoreFeet.ToString( "##0" ) + " feet"
                    + "\n Input " + inScoreFeet + " feet converts to " + convScoreMeters.ToString( "##0.0" ) + " meters"
                    ;
                }
            } else {
                returnMsg = "";
            }
            return returnMsg;
        }

		private bool RoundCommpleteUpdateScore() {
			JumpPassResultStatus curReturnResult = myJumpSetAnalysis.AnalyzeRecapPasses();

			scoreFeetTextBox.Text = curReturnResult.bestScoreFeet.ToString();
			scoreMetersTextBox.Text = curReturnResult.bestScoreMeters.ToString();
			setEventRegRowStatus( curReturnResult.skierSetStatus, TourEventRegDataGridView.Rows[myEventRegViewIdx] );

			hcapScoreTextBox.Text = "0";
			String curHCapScore = HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "HCapScore", "" );
			if ( HelperFunctions.isObjectPopulated( curHCapScore ) ) {
				if ( JumpEventData.isIwwfEvent() ) {
					hcapScoreTextBox.Text = ( curReturnResult.bestScoreMeters + Decimal.Parse( curHCapScore ) ).ToString( "##,###0.0" );
				} else {
					hcapScoreTextBox.Text = ( curReturnResult.bestScoreFeet + Decimal.Parse( curHCapScore ) ).ToString( "##,###0.0" );
				}
			}

			String curAgeGroup = HelperFunctions.getViewRowColValue( myRecapRow, "AgeGroupRecap", "" );
			Int16 skierBoatSpeed = Convert.ToInt16( HelperFunctions.getViewRowColValue( myRecapRow, "BoatSpeedRecap", "0" ) );
			BoatSpeedTextBox.Text = skierBoatSpeed.ToString( "00" );

			ScoreList[0].Score = curReturnResult.bestScoreFeet;
			appNopsCalc.calcNops( curAgeGroup, ScoreList );
			nopsScoreTextBox.Text = Math.Round( ScoreList[0].Nops, 1 ).ToString();

			if ( curReturnResult.isSkierSetComplete() ) {
				setEventRegRowStatus( curReturnResult.skierSetStatus );
				return true;
			}

			return false;
		}

        private bool CalcDistByMeters() {
            bool curReturnValue = false;
            Decimal curDistExtd;
            curReturnValue = true;
            myJumpSetAnalysis.TriangleTolMsg = "";

            try {
                Double numAngleA = Convert.ToDouble( myRecapRow.Cells["Meter1Recap"].Value.ToString() );
                Double numAngleB = Convert.ToDouble( myRecapRow.Cells["Meter2Recap"].Value.ToString() );
                Double numAngleC = Convert.ToDouble( myRecapRow.Cells["Meter3Recap"].Value.ToString() );
                Int32[] calcDistResults = myJumpCalc.calcDistance( numAngleA, numAngleB, numAngleC );
                Int32 curDist = calcDistResults[0];
                if ( myMeterDistTol < myJumpCalc.TriangleJump ) {
					myJumpSetAnalysis.TriangleTolMsg = "Wide triangle, from = " + calcDistResults[2].ToString( "##0" ) + " to " + calcDistResults[1].ToString( "##0" );
                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                    curDist = calcDistResults[2];
                    curDistExtd = calcDistResults[4];
                } else {
                    myRecapRow.Cells["RerideRecap"].Value = "N";
                    curDistExtd = calcDistResults[3];
                }
                Decimal convScoreMeters = ( curDistExtd / 100 ) / myMetericFactor;
                myRecapRow.Cells["ScoreFeetRecap"].Value = curDist.ToString( "##0" );
                myRecapRow.Cells["ScoreMetersRecap"].Value = Math.Round( convScoreMeters, 1 ).ToString( "##0.#" );
                myRecapRow.Cells["ScoreTriangleRecap"].Value = myJumpCalc.TriangleJump.ToString( "##0.##" );
            
			} catch ( Exception exp ) {
                MessageBox.Show( "Exception encountered when calculating distance using meter readings \n\n " + exp.Message );
                curReturnValue = false;
            }

            skierPassMsg.Text = myJumpSetAnalysis.TimeTolMsg;
			if ( HelperFunctions.isObjectPopulated( myJumpSetAnalysis.TimeTolMsg ) ) skierPassMsg.Text += " : " + myJumpSetAnalysis.TimeTolMsg;

			myRecapRow.Cells["RerideReasonRecap"].Value = skierPassMsg.Text;
            return curReturnValue;
        }
 
        private void BoatSelectButton_Click( object sender, EventArgs e ) {
			if ( approvedBoatSelectGroupBox.Visible ) {
				approvedBoatSelectGroupBox.Visible = false;
				listApprovedBoatsDataGridView.Visible = false;
			} else {
				approvedBoatSelectGroupBox.Visible = true;
				listApprovedBoatsDataGridView.Visible = true;
				listApprovedBoatsDataGridView.Focus();
				listApprovedBoatsDataGridView.CurrentCell = listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatModelApproved"];
			}
		}

		private void selectBoatButton_Click( object sender, EventArgs e ) {
			myBoatListIdx = listApprovedBoatsDataGridView.CurrentRow.Index;
			if ( myBoatListIdx > 0 ) {
				updateBoatSelect();
			}
		}

		private void refreshBoatListButton_Click( object sender, EventArgs e ) {
			getApprovedTowboats();
			calcBoatCodeFromDisplay( TourBoatTextbox.Text );
		}

		private void listApprovedBoatsDataGridView_CellContentDoubleClick( object sender, DataGridViewCellEventArgs e ) {
			if ( e.RowIndex > 0 ) {
				myBoatListIdx = e.RowIndex;
				updateBoatSelect();
			}
		}

		private void updateBoatSelect() {
			String curMethodName = "Jump:ScoreEntry:updateBoatSelect: ";
			String curMsg = "";

			TourBoatTextbox.Text = buildBoatModelNameDisplay();
			TourBoatTextbox.Focus();
			TourBoatTextbox.Select( 0, 0 );
			String curBoatCode = calcBoatCodeFromDisplay( TourBoatTextbox.Text );

			if ( myScoreRow != null ) {
				try {
					Int64 curScorePK = (Int64) myScoreRow["PK"];

					StringBuilder curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update JumpScore Set Boat = '" + curBoatCode + "'" );
					curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

					//String boatId, String boatManufacturer, String boatModel, Int16 boatYear, String boatColor, String boatComment
					if (WscHandler.isConnectActive) {
						String curBoatModelName = HelperFunctions.getViewRowColValue( listApprovedBoatsDataGridView.Rows[myBoatListIdx], "BoatModelApproved", "" );
						Int16 curBoatModelYear = Convert.ToInt16( HelperFunctions.getViewRowColValue( listApprovedBoatsDataGridView.Rows[myBoatListIdx], "ModelYear", "2000" ) );
						String curBoatNotes = HelperFunctions.getViewRowColValue( listApprovedBoatsDataGridView.Rows[myBoatListIdx], "BoatNotes", "" );

						String curManufacturer = "Unknown";
						if (curBoatModelName.Contains("Malibu")) curManufacturer = "Malibu";
						if (curBoatModelName.Contains("Nautique")) curManufacturer = "Nautique";
						if (curBoatModelName.Contains("Master")) curManufacturer = "Masctercraft";
						WscHandler.sendBoatData(curBoatCode, curManufacturer, curBoatModelName, curBoatModelYear, "Color", curBoatNotes);
					
					} else if ( !WaterskiConnectLabel.Visible ) WaterskiConnectLabel.Visible = false;

				} catch ( Exception excp ) {
					curMsg = ":Error attempting to update boat selection \n" + excp.Message;
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );
				}
			}

			listApprovedBoatsDataGridView.Visible = false;
			approvedBoatSelectGroupBox.Visible = false;
			jumpRecapDataGridView.Focus();
		}

		private String calcBoatCodeFromDisplay( String inBoatModelNameDisplay ) {
			try {
				if ( inBoatModelNameDisplay.Equals( "--Select--" ) ) {
					myBoatListIdx = 0;
					return "--Select--";

				} else if ( inBoatModelNameDisplay.Equals( "" ) ) {
					myBoatListIdx = 0;
					return "--Select--";

				} else {
					int delimIdx = inBoatModelNameDisplay.IndexOf( " (KEY: " );
					delimIdx += 7;
					int lenBoatCode = inBoatModelNameDisplay.Length - delimIdx - 1;
					String curBoatCode = inBoatModelNameDisplay.Substring( delimIdx, lenBoatCode );
					curBoatCode = curBoatCode.Replace( "'", "''" );
					setApprovedBoatSelectEntry( curBoatCode );
					return curBoatCode;
				}

			} catch {
				return "";
			}
		}

		private String setApprovedBoatSelectEntry( String inBoatCode ) {
			try {
				foreach ( DataGridViewRow curBoatRow in listApprovedBoatsDataGridView.Rows ) {
					if ( ( (String) curBoatRow.Cells["BoatCode"].Value ).ToUpper().Equals( inBoatCode.ToUpper() ) ) {
						myBoatListIdx = curBoatRow.Index;
						return buildBoatModelNameDisplay();
					}
				}

				myBoatListIdx = 0;
				return "";

			} catch {
				TourBoatTextbox.Text = "";
				foreach ( DataGridViewRow curBoatRow in listApprovedBoatsDataGridView.Rows ) {
					if ( ( (String) curBoatRow.Cells["BoatCode"].Value ).ToUpper().Equals( "UNDEFINED" ) ) {
						myBoatListIdx = curBoatRow.Index;
						return buildBoatModelNameDisplay();
					}
				}

				myBoatListIdx = 0;
				return "";

			}
		}

		private String buildBoatModelNameDisplay() {
			if ( myBoatListIdx > 0 ) {
				String curBoatCode = HelperFunctions.getViewRowColValue( listApprovedBoatsDataGridView.Rows[myBoatListIdx], "BoatCode", "" );
				String curBoatModelName = HelperFunctions.getViewRowColValue( listApprovedBoatsDataGridView.Rows[myBoatListIdx], "BoatModelApproved", "" );
				return curBoatModelName + " (KEY: " + curBoatCode + ")";
			} else {
				return "";
			}
		}

		private void navPrintResults_Click( object sender, EventArgs e ) {
			Timer curTimerObj = new Timer();
			curTimerObj.Interval = 5;
			curTimerObj.Tick += new EventHandler( printReportTimer );
			curTimerObj.Start();
		}

		private void printReportTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( printReportTimer );
			printReport();
		}
		private void printReport() {
			PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            DataGridView[] myViewList = new DataGridView[1];
            myViewList[0] = jumpRecapDataGridView;

            bool CenterOnPage = false;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

			PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
			if ( curPrintDialog == null ) return;
			
			String printTitle = Properties.Settings.Default.Mdi_Title + " - " + this.Text;
			myPrintDoc = new PrintDocument();

			String curDocName = "";
			String curAgeGroup = (String)TourEventRegDataGridView.CurrentRow.Cells["AgeGroup"].Value;
			String curSkierName = (String)TourEventRegDataGridView.CurrentRow.Cells["SkierName"].Value;
			String[] curNameParts = curSkierName.Split( ',' );
			if ( curNameParts.Length > 1 ) {
				curDocName = curNameParts[1].Trim() + curNameParts[0].Trim() + "_" + curAgeGroup;
			} else if ( curNameParts.Length == 1 ) {
				curDocName = curNameParts[1].Trim() + curNameParts[0].Trim() + "_" + curAgeGroup;
			} else {
				curDocName = "Unknown";
			}
			curSkierName += "  " + curAgeGroup;

			myPrintDoc.DocumentName = curDocName;
			myPrintDoc.DefaultPageSettings.Margins = new Margins( 125, 125, 25, 25 );
			myPrintDoc.DefaultPageSettings.Landscape = true;
			myPrintDataGrid = new DataGridViewPrinter( myViewList, myPrintDoc,
				CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

			//Build titles for each data grid view
			myPrintDataGrid.GridViewTitleList();
			StringRowPrinter myGridTitle;
			StringFormat GridTitleStringFormat = new StringFormat();
			GridTitleStringFormat.Trimming = StringTrimming.Word;
			GridTitleStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			GridTitleStringFormat.Alignment = StringAlignment.Near;

			myGridTitle = new StringRowPrinter( "Jump Pass Round " + roundActiveSelect.RoundValue,
				0, 20, 325, fontPrintTitle.Height,
				Color.DarkBlue, Color.White, fontPrintTitle, GridTitleStringFormat );
			myPrintDataGrid.GridViewTitleRow = myGridTitle;

			//Build report page subtitles
			myPrintDataGrid.SubtitleList();
			StringRowPrinter mySubtitle;
			StringFormat SubtitleStringFormatLeft = new StringFormat();
			SubtitleStringFormatLeft.Trimming = StringTrimming.Word;
			SubtitleStringFormatLeft.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			SubtitleStringFormatLeft.Alignment = StringAlignment.Near;

			StringFormat SubtitleStringFormatCenter = new StringFormat();
			SubtitleStringFormatCenter.Trimming = StringTrimming.Word;
			SubtitleStringFormatCenter.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
			SubtitleStringFormatCenter.Alignment = StringAlignment.Center;

			mySubtitle = new StringRowPrinter( curSkierName, 0, 0, 325, fontPrintTitle.Height,
				Color.DarkBlue, Color.White, fontPrintTitle, SubtitleStringFormatLeft );
			myPrintDataGrid.SubtitleRow = mySubtitle;

			mySubtitle = new StringRowPrinter( scoreFeetLabel.Text,
				0, 25, 50, scoreFeetLabel.Size.Height,
				Color.Black, Color.White, scoreFeetLabel.Font, SubtitleStringFormatCenter );
			myPrintDataGrid.SubtitleRow = mySubtitle;
			mySubtitle = new StringRowPrinter( scoreFeetTextBox.Text,
				0, 40, 50, scoreFeetTextBox.Size.Height,
				Color.Black, Color.White, scoreFeetTextBox.Font, SubtitleStringFormatCenter );
			myPrintDataGrid.SubtitleRow = mySubtitle;

			mySubtitle = new StringRowPrinter( scoreMetersLabel.Text,
				170, 25, 50, scoreMetersLabel.Size.Height,
				Color.Black, Color.White, scoreMetersLabel.Font, SubtitleStringFormatCenter );
			myPrintDataGrid.SubtitleRow = mySubtitle;
			mySubtitle = new StringRowPrinter( scoreMetersTextBox.Text,
				170, 40, 50, scoreMetersTextBox.Size.Height,
				Color.Black, Color.White, scoreMetersTextBox.Font, SubtitleStringFormatCenter );
			myPrintDataGrid.SubtitleRow = mySubtitle;

			mySubtitle = new StringRowPrinter( nopsScoreLabel.Text,
				370, 25, 50, nopsScoreLabel.Size.Height,
				Color.Black, Color.White, nopsScoreLabel.Font, SubtitleStringFormatCenter );
			myPrintDataGrid.SubtitleRow = mySubtitle;
			mySubtitle = new StringRowPrinter( nopsScoreTextBox.Text,
				370, 40, 55, nopsScoreTextBox.Size.Height,
				Color.Black, Color.White, nopsScoreTextBox.Font, SubtitleStringFormatCenter );
			myPrintDataGrid.SubtitleRow = mySubtitle;

			myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
			myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
			myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
			curPreviewDialog.Document = myPrintDoc;
			curPreviewDialog.Size = new System.Drawing.Size( this.Width, this.Height );
			curPreviewDialog.Focus();
			curPreviewDialog.ShowDialog();
		}

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private void getEventRegData(int inRound) {
            getEventRegData( "All", inRound );
        }
        private void getEventRegData(String inEventGroup, int inRound) {
            myPrevEventGroup = "";
            myTourEventRegDataTable = getEventGroupStartList( inEventGroup, inRound );
		}

        private DataTable getEventGroupStartList( String inEventGroup, int inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, T.Federation, T.State, E.AgeGroup, E.AgeGroup as Div, T.Gender, O.RunOrder, E.TeamCode" );
            curSqlStmt.Append( ", COALESCE(O.EventGroup, E.EventGroup) as EventGroup, COALESCE(O.RunOrderGroup, '') as RunOrderGroup" );
            curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, COALESCE(O.RankingScore, E.RankingScore) as RankingScore, E.RankingRating" );
            curSqlStmt.Append( ", E.HCapBase, E.HCapScore, T.JumpHeight, T.SkiYearAge, S.ScoreFeet as Score, S.ScoreFeet, S.ScoreMeters, S.NopsScore" );
            curSqlStmt.Append( ", COALESCE (S.Status, 'TBD') AS Status, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt " );
            curSqlStmt.Append( "FROM EventReg E " );
            curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
            curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event AND O.Round = " + inRound.ToString() + " " );
            curSqlStmt.Append( "     LEFT OUTER JOIN JumpScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
            curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + JumpEventData.mySanctionNum + "' AND E.Event = 'Jump' " );

            if ( !( inEventGroup.ToLower().Equals( "all" ) ) ) {
                if ( JumpEventData.isCollegiateEvent() ) {
                    curSqlStmt.Append( HelperFunctions.getEventGroupFilterNcwsaSql( inEventGroup ) );

                } else {
                    curSqlStmt.Append( "And (O.EventGroup = '" + inEventGroup + "' AND (O.RunOrderGroup IS NULL OR O.RunOrderGroup = '')" );
                    curSqlStmt.Append( "     OR O.EventGroup + '-' + O.RunOrderGroup = '" + inEventGroup + "' ) " );
                }
            }
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

            if ( curDataTable.Rows.Count == 0 ) {
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, T.Federation, T.State, E.AgeGroup, T.Gender, E.EventGroup, '' as RunOrderGroup, E.RunOrder, E.TeamCode" );
                curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, E.RankingScore, E.RankingRating, E.HCapBase, E.HCapScore" );
                curSqlStmt.Append( ", T.JumpHeight, T.SkiYearAge, S.ScoreFeet as Score, S.ScoreFeet, S.ScoreMeters, S.NopsScore" );
                curSqlStmt.Append( ", COALESCE (S.Status, 'TBD') AS Status, E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt " );
                curSqlStmt.Append( "FROM EventReg E " );
                curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                curSqlStmt.Append( "     LEFT OUTER JOIN JumpScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                curSqlStmt.Append( "WHERE E.SanctionId = '" + JumpEventData.mySanctionNum + "' AND E.Event = 'Jump' " );
                if ( !( inEventGroup.ToLower().Equals( "all" ) ) ) {
                    if ( JumpEventData.isCollegiateEvent() ) {
                        curSqlStmt.Append( HelperFunctions.getEventGroupFilterNcwsaSql( inEventGroup ) );

                    } else {
                        curSqlStmt.Append( "And E.EventGroup = '" + inEventGroup + "' " );
                    }
                }
                curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            }

            return curDataTable;
        }

        private Boolean isRunOrderByRound( int inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select count(*) as SkierCount From EventRunOrder " );
			curSqlStmt.Append( "WHERE SanctionId = '" + JumpEventData.mySanctionNum + "' AND Event = 'Jump' AND Round = " + inRound );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( (int) curDataTable.Rows[0]["SkierCount"] > 0 ) {
				return true;
			} else {
				return false;
			}

		}

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append( "SELECT SanctionId, SanctionEditCode, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + inSanctionId + "' ");
            DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
            return curDataTable;
        }

        private void getApprovedTowboats() {
			int curIdx = 0;
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq, Owner, Notes, HullId, ModelYear, BoatModel " );
			curSqlStmt.Append( "FROM TourBoatUse BU " );
			curSqlStmt.Append( "INNER JOIN CodeValueList BL ON BL.ListCode = BU.HullId " );
			curSqlStmt.Append( "WHERE BL.ListName = 'ApprovedBoats' AND BU.SanctionId = '" + JumpEventData.mySanctionNum + "' " );
			curSqlStmt.Append( "Union " );
			curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq, '' as Owner, '' as Notes, ListCode as HullId, '' as ModelYear, CodeValue as BoatModel " );
			curSqlStmt.Append( "FROM CodeValueList BL " );
			curSqlStmt.Append( "WHERE ListName = 'ApprovedBoats' AND ListCode IN ('--Select--', 'Unlisted') " );
			curSqlStmt.Append( "ORDER BY SortSeq DESC" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			listApprovedBoatsDataGridView.Rows.Clear();
			foreach ( DataRow curRow in curDataTable.Rows ) {
				String[] boatSpecList = curRow["CodeDesc"].ToString().Split( '|' );
				listApprovedBoatsDataGridView.Rows.Add( 1 );
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatCode"].Value = curRow["HullId"].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatOwner"].Value = curRow["Owner"].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatNotes"].Value = curRow["Notes"].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatModelApproved"].Value = curRow["BoatModel"].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["HullStatus"].Value = boatSpecList[0].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["EngineSpec"].Value = boatSpecList[1].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["FuelDel"].Value = boatSpecList[2].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["Transmission"].Value = boatSpecList[3].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["Prop"].Value = boatSpecList[4].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["SpeedControl"].Value = boatSpecList[5].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["ModelYear"].Value = curRow["ModelYear"].ToString();
				curIdx++;
			}
			myBoatListIdx = 0;
            
            if (curDataTable.Rows.Count < 3) {
                MessageBox.Show( "You have no boats defined for your tournament."
                + "\nUse the Boat Use window to add boats to your tournament "
                + "and make them available for event selection" );
            }

        }

        private DataTable getSkierScoreByRound( String inMemberId, String inAgeGroup, int inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT S.PK, S.SanctionId, S.MemberId, S.AgeGroup, Round, S.EventClass, COALESCE(E.TeamCode, '') as TeamCode" );
            curSqlStmt.Append( ", ScoreFeet, ScoreMeters , NopsScore, HCapScore, Rating, BoatSpeed, RampHeight, Status, Boat, Note" );
            curSqlStmt.Append(", Gender, SkiYearAge ");
            curSqlStmt.Append( "FROM JumpScore S " );
            curSqlStmt.Append( "  INNER JOIN EventReg E ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND E.Event = 'Jump'" );
            curSqlStmt.Append( "  INNER JOIN TourReg T ON S.SanctionId = T.SanctionId AND S.MemberId = T.MemberId AND S.AgeGroup = T.AgeGroup ");
            curSqlStmt.Append( "WHERE S.SanctionId = '" + JumpEventData.mySanctionNum + "' AND S.MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "  AND S.AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() + " " );
            curSqlStmt.Append( "ORDER BY S.SanctionId, S.MemberId" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

		private DataRow getSkierRecapEntry( String inMemberId, String inAgeGroup, byte inRound, Int16 inPassNum ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT PK, SanctionId, MemberId, AgeGroup, Round, PassNum" );
			curSqlStmt.Append( ", BoatSpeed, SkierBoatPath, RampHeight, Meter1, Meter2, Meter3, Meter4, Meter5, Meter6" );
			curSqlStmt.Append( ", ScoreFeet, ScoreMeters, ScoreTriangle, Results" );
			curSqlStmt.Append( ", BoatSplitTime, BoatEndTime, BoatSplitTime2" );
			curSqlStmt.Append( ", Split52TimeTol, Split82TimeTol, Split41TimeTol" );
			curSqlStmt.Append( ", ReturnToBase, ScoreProt, TimeInTol, Reride, RerideIfBest, RerideCanImprove, RerideReason, Note, LastUpdateDate " );
			curSqlStmt.Append( "FROM JumpRecap " );
			curSqlStmt.Append( "WHERE SanctionId = '" + JumpEventData.mySanctionNum + "' AND MemberId = '" + inMemberId + "'" );
			curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() + " AND PassNum = " + inPassNum.ToString() );
			curSqlStmt.Append( "ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];
			return null;
		}

		private Boolean checkForSkierRoundScore( String inMemberId, int inRound, String inAgeGroup ) {
			return HelperFunctions.checkForSkierRoundScore( JumpEventData.mySanctionNum, "Jump", inMemberId, inRound, inAgeGroup );
		}

		private bool checkLoadOfficialAssgmt( String inAgeGroup, String inEventGroup ) {
			myCheckOfficials.readOfficialAssignments( JumpEventData.mySanctionNum, "Jump", inAgeGroup, inEventGroup, roundActiveSelect.RoundValue );
			isLoadInProg = true;
			driverDropdown.DataSource = myCheckOfficials.driverAsgmtDataTable;
			driverDropdown.DisplayMember = "MemberName";
			driverDropdown.ValueMember = "MemberId";
			if ( myCheckOfficials.driverAsgmtDataTable.Rows.Count > 1 ) {
				driverLabel.ForeColor = Color.Red;
			} else {
				driverLabel.ForeColor = Color.Black;
			}
			isLoadInProg = false;
			if ( myDriverMemberId.Length > 0 ) {
				for ( int curIdx = 0; curIdx < myCheckOfficials.driverAsgmtDataTable.Rows.Count; curIdx++ ) {
					if ( myCheckOfficials.driverAsgmtDataTable.Rows[curIdx]["MemberId"].Equals( myDriverMemberId ) ) {
						driverDropdown.SelectedValue = myDriverMemberId;
						driverDropdown.SelectedIndex = curIdx;
						break;
					}
				}
			} else if ( myCheckOfficials.driverAsgmtDataTable.Rows.Count > 0 ) {
				driverDropdown.SelectedIndex = 0;
			}

			if ( myCheckOfficials.officialAsgmtCount > 0 ) return true;
			return false;
		}

		private Decimal getSkier1stRoundRampHeight(String inMemberId, String inAgeGroup) {
            Decimal curReturnValue = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT RampHeight FROM JumpScore " );
            curSqlStmt.Append( "WHERE SanctionId = '" + JumpEventData.mySanctionNum + "' AND MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = 1 " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                curReturnValue = (Decimal)curDataTable.Rows[0]["RampHeight"];
            }
            return curReturnValue;
        }

        private DataTable getSkierRecapByRound(String inMemberId, String inAgeGroup, int inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, MemberId, AgeGroup, Round, PassNum" );
            curSqlStmt.Append( ", BoatSpeed, SkierBoatPath, RampHeight, Meter1, Meter2, Meter3, Meter4, Meter5, Meter6" );
            curSqlStmt.Append( ", ScoreFeet, ScoreMeters, ScoreTriangle, Results" );
            curSqlStmt.Append( ", BoatSplitTime, BoatEndTime, BoatSplitTime2" );
            curSqlStmt.Append( ", Split52TimeTol, Split82TimeTol, Split41TimeTol" );
            curSqlStmt.Append( ", ReturnToBase, ScoreProt, TimeInTol, Reride, RerideIfBest, RerideCanImprove, RerideReason, Note, LastUpdateDate " );
            curSqlStmt.Append( "FROM JumpRecap " );
            curSqlStmt.Append( "WHERE SanctionId = '" + JumpEventData.mySanctionNum + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() + " " );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

		private void TeamCodeTextBox_TextChanged( object sender, EventArgs e ) {

		}

		private void driverDropdown_SelectedValueChanged( object sender, EventArgs e ) {
			if ( isLoadInProg ) return;
			if ( ( (ComboBox)sender ).Items.Count == 0 ) return;
			if ( ( (ComboBox)sender ).SelectedValue == null ) return;
			String curSelectedValue = ( (ComboBox)sender ).SelectedValue.ToString();
			if ( curSelectedValue.Length == 0 ) return;
			myDriverMemberId = curSelectedValue;
		}

		private String getSortCommand() {
			String curSortCommand = myTourProperties.RunningOrderSortJump;
			if ( isOrderByRoundActive ) curSortCommand = myTourProperties.RunningOrderSortJumpRound;

			int curDelim = mySortCommand.IndexOf( "AgeGroup" );
			if ( curDelim == 0 ) {
				curSortCommand = "DivOrder" + mySortCommand.Substring( "AgeGroup".Length );
				if ( isOrderByRoundActive ) {
					myTourProperties.RunningOrderSortJumpRound = curSortCommand;
				} else {
					myTourProperties.RunningOrderSortJump = curSortCommand;
				}

			} else if ( curDelim > 0 ) {
				curSortCommand = mySortCommand.Substring( 0, curDelim ) + "DivOrder" + mySortCommand.Substring( curDelim + "AgeGroup".Length );
				if ( isOrderByRoundActive ) {
					myTourProperties.RunningOrderSortJumpRound = curSortCommand;
				} else {
					myTourProperties.RunningOrderSortJump = curSortCommand;
				}
			}
			return curSortCommand;
		}

	}
}
