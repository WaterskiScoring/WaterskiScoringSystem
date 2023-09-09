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

namespace WaterskiScoringSystem.Slalom {
	public partial class ScoreEntry : Form {
		#region instance variables
		private bool myWindowFocus = true;
		private bool isDataModified = false;
		private bool isLoadInProg = false;
		private bool isAddRecapRowInProg = false;
		private bool isRecapRowEnterHandled = false;
		private bool myGateValueChg = false;
		private bool isLastPassSelectActive = false;
		private bool isOrderByRoundActive = false;

		private int myEventRegViewIdx = 0;
		private int myStartCellIndex = 0;
		private int myBoatListIdx = 0;
		private int mySkierRunCount = 0;
		private int myPassRunCount = 0;
		private int myEventDelaySeconds = 0;

		private Int16 myNumJudges;

		private DateTime myEventStartTime;
		private DateTime myEventDelayStartTime;

		private String myTitle = "";
		private String myOrigCellValue = "";
		private String mySortCommand = "";
		private String myFilterCmd = "";
		private String myModCellValue = "";
		private String myRecapColumn;
		private String myLastPassSelectValue = "";
		private String myPrevEventGroup = "";
		private String myDriverMemberId = "";

		private DataGridViewRow myRecapRow;
		private DataRow myScoreRow;
		private DataRow myPassRow;
		private DataRow myPassRowLastCompleted;
		private DataRow myBoatPathDataRow;

		private TourProperties myTourProperties;
		private DataTable myTourEventRegDataTable;
		private DataTable myScoreDataTable;
		private DataTable myRecapDataTable;

		private List<String> myCompletedNotices = new List<String>();

		private SortDialogForm sortDialogForm;
		private FilterDialogForm filterDialogForm;
		private RerideReason rerideReasonDialogForm;
		private SkierDoneReason skierDoneReasonDialogForm;
		private SlalomOptUp OptUpDialogForm;
		private CalcNops appNopsCalc;
		private CheckOfficials myCheckOfficials;
		private NextPassDialog myNextPassDialog;
		private SlalomSetAnalysis mySlalomSetAnalysis;

		private List<Common.ScoreEntry> ScoreList = new List<Common.ScoreEntry>();
		private PrintDocument myPrintDoc;
		private DataGridViewPrinter myPrintDataGrid;
		#endregion

		public ScoreEntry() {
			InitializeComponent();

			appNopsCalc = CalcNops.Instance;
			appNopsCalc.LoadDataForTour();
			ScoreList.Add( new Common.ScoreEntry( "Slalom", 0, "", 0 ) );
		}

		private void ScoreEntry_Load( object sender, EventArgs e ) {
			if ( !( SlalomEventData.setEventData() ) ) {
                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 15;
                curTimerObj.Tick += new EventHandler( CloseWindowTimer );
                curTimerObj.Start();
                return;
            }

			myTitle = this.Text;

			Cursor.Current = Cursors.WaitCursor;
			if ( Properties.Settings.Default.SlalomEntry_Width > 0 ) this.Width = Properties.Settings.Default.SlalomEntry_Width;
			if ( Properties.Settings.Default.SlalomEntry_Height > 0 ) this.Height = Properties.Settings.Default.SlalomEntry_Height;
			if ( Properties.Settings.Default.SlalomEntry_Location.X > 0
				&& Properties.Settings.Default.SlalomEntry_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.SlalomEntry_Location;
			}
			myTitle = this.Text;
			TeamCode.Visible = false;
			if ( SlalomEventData.isCollegiateEvent() ) TeamCode.Visible = true;
			
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

			String[] curList = { "SkierName", "EventGroup", "Div", "DivOrder", "RunOrder", "TeamCode", "EventClass", "ReadyForPlcmt"
					, "RankingScore", "RankingRating", "HCapBase", "HCapScore" };
			sortDialogForm = new SortDialogForm();
			sortDialogForm.ColumnListArray = curList;

			filterDialogForm = new Common.FilterDialogForm();
			filterDialogForm.ColumnListArray = curList;

			rerideReasonDialogForm = new RerideReason();
			skierDoneReasonDialogForm = new SkierDoneReason();
			OptUpDialogForm = new SlalomOptUp();

			myNextPassDialog = new NextPassDialog();
			myNextPassDialog.StartPosition = FormStartPosition.Manual;

			//Load round selection list based on number of rounds specified for the tournament
			roundActiveSelect.SelectList_LoadHorztl( SlalomEventData.myTourRow["SlalomRounds"].ToString(), roundActiveSelect_Click );
			roundActiveSelect.RoundValue = "1";

			myCheckOfficials = new CheckOfficials();

			scoreEventClass.DataSource = SlalomEventData.mySkierClassList.DropdownList;
			scoreEventClass.DisplayMember = "ItemName";
			scoreEventClass.ValueMember = "ItemValue";

			//Load rope length selection list
			SlalomLineSelect.SelectList_Load( SlalomLineSelect_Change );

			//Load slalom speed selection list
			SlalomSpeedSelection.SelectList_Load( SlalomSpeedSelect_Change );

			//Determine required number of judges for event
			setShowByNumJudges( SlalomEventData.getJudgeCountByClass() );

			//Retrieve list of approved tournament boats
			getApprovedTowboats();
			listApprovedBoatsDataGridView.Visible = false;
			approvedBoatSelectGroupBox.Visible = false;

			//Retrieve and load tournament event entries
			loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

			navWaterSkiConnect_Click( null, null );
			
			if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
				LiveWebLabel.Visible = true;
			} else {
				LiveWebLabel.Visible = false;
			}
			
			Cursor.Current = Cursors.Default;
		}
		private void CloseWindowTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( CloseWindowTimer );
			this.Close();
		}

		public void setShowByNumJudges( Int16 inNumJudges ) {
			myNumJudges = inNumJudges;

			RoundRecap.DisplayIndex = 0;
			skierPassRecap.DisplayIndex = 1;
			CellBorder1.DisplayIndex = 2;
			GateEntry2Recap.DisplayIndex = 3;
			Judge2ScoreRecap.DisplayIndex = 4;
			GateExit2Recap.DisplayIndex = 5;
			CellBorder2.DisplayIndex = 6;
			GateEntry3Recap.DisplayIndex = 7;
			Judge3ScoreRecap.DisplayIndex = 8;
			GateExit3Recap.DisplayIndex = 9;
			CellBorder3.DisplayIndex = 10;
			Judge4ScoreRecap.DisplayIndex = 11;
			CellBorder4.DisplayIndex = 12;
			Judge5ScoreRecap.DisplayIndex = 13;
			CellBorder5.DisplayIndex = 14;
			GateEntry1Recap.DisplayIndex = 15;
			Judge1ScoreRecap.DisplayIndex = 16;
			GateExit1Recap.DisplayIndex = 17;
			CellBorder6.DisplayIndex = 18;
			TimeInTolRecap.DisplayIndex = 19;
			TimeInTolImg.DisplayIndex = 20;
			BoatPathGoodRecap.DisplayIndex = 21;
			BoatTimeRecap.DisplayIndex = 22;
			ScoreRecap.DisplayIndex = 23;
			ScoreProtRecap.DisplayIndex = 24;
			RerideRecap.DisplayIndex = 25;
			NoteRecap.DisplayIndex = 26;
			RerideReasonRecap.DisplayIndex = 27;
			PassSpeedKphRecap.DisplayIndex = 28;
			PassLineLengthRecap.DisplayIndex = 29;
			SanctionIdRecap.DisplayIndex = 30;
			MemberIdRecap.DisplayIndex = 31;
			Updated.DisplayIndex = 32;

			Judge5ScoreRecap.Visible = true;
			CellBorder5.Visible = true;
			Judge4ScoreRecap.Visible = true;
			CellBorder4.Visible = true;
			Judge3ScoreRecap.Visible = true;
			GateEntry3Recap.Visible = true;
			GateExit3Recap.Visible = true;
			CellBorder3.Visible = true;
			Judge2ScoreRecap.Visible = true;
			GateEntry2Recap.Visible = true;
			GateExit2Recap.Visible = true;
			CellBorder2.Visible = true;

			if ( myNumJudges == 1 ) {
				myStartCellIndex = slalomRecapDataGridView.Columns["Judge1ScoreRecap"].Index;
			} else {
				myStartCellIndex = slalomRecapDataGridView.Columns["Judge2ScoreRecap"].Index;
			}

			if ( myNumJudges < 5 ) {
				Judge5ScoreRecap.Visible = false;
				CellBorder5.Visible = false;
				if ( myNumJudges < 4 ) {
					Judge4ScoreRecap.Visible = false;
					CellBorder4.Visible = false;

					if ( myNumJudges < 3 ) {
						Judge3ScoreRecap.Visible = false;
						GateEntry3Recap.Visible = false;
						GateExit3Recap.Visible = false;
						CellBorder3.Visible = false;

						if ( myNumJudges < 2 ) {
							Judge2ScoreRecap.Visible = false;
							GateEntry2Recap.Visible = false;
							GateExit2Recap.Visible = false;
							CellBorder2.Visible = false;
						}
					}
				}
			
			} else {
				RoundRecap.DisplayIndex = 0;
				skierPassRecap.DisplayIndex = 1;
				CellBorder1.DisplayIndex = 2;
				GateEntry2Recap.DisplayIndex = 3;
				Judge2ScoreRecap.DisplayIndex = 4;
				CellBorder2.DisplayIndex = 5;
				GateEntry3Recap.DisplayIndex = 6;
				Judge3ScoreRecap.DisplayIndex = 7;
				CellBorder3.DisplayIndex = 8;
				Judge4ScoreRecap.DisplayIndex = 9;
				GateExit3Recap.DisplayIndex = 10;
				CellBorder4.DisplayIndex = 11;
				Judge5ScoreRecap.DisplayIndex = 12;
				GateExit2Recap.DisplayIndex = 13;
				CellBorder5.DisplayIndex = 14;
				GateEntry1Recap.DisplayIndex = 15;
				Judge1ScoreRecap.DisplayIndex = 16;
				GateExit1Recap.DisplayIndex = 17;
				CellBorder6.DisplayIndex = 18;
				TimeInTolRecap.DisplayIndex = 19;
				TimeInTolImg.DisplayIndex = 20;
				BoatPathGoodRecap.DisplayIndex = 21;
				BoatTimeRecap.DisplayIndex = 22;
				ScoreRecap.DisplayIndex = 23;
				ScoreProtRecap.DisplayIndex = 24;
				RerideRecap.DisplayIndex = 25;
				NoteRecap.DisplayIndex = 26;
				RerideReasonRecap.DisplayIndex = 27;
				PassSpeedKphRecap.DisplayIndex = 28;
				PassLineLengthRecap.DisplayIndex = 29;
				SanctionIdRecap.DisplayIndex = 30;
				MemberIdRecap.DisplayIndex = 31;
				Updated.DisplayIndex = 32;
			}

			if ( myNumJudges > 3 ) {
				ClassC5JudgeCB.Visible = false;
				ClassC5JudgeCB.Enabled = false;
				ClassR3JudgeCB.Visible = true;
				ClassR3JudgeCB.Enabled = true;
				ClassR3JudgeCB.Checked = true;
			} else {
				ClassC5JudgeCB.Visible = true;
				ClassC5JudgeCB.Enabled = true;
				ClassR3JudgeCB.Visible = false;
				ClassR3JudgeCB.Enabled = false;
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
				TourEventRegDataGridView.Width = 540;
			} else {
				TourEventRegDataGridView.Width = 490;
			}
		}

		private void ClassR3JudgeCB_CheckedChanged( object sender, EventArgs e ) {
			CheckBox curControl = (CheckBox)sender;
			if ( curControl.Checked ) {
				setShowByNumJudges( 3 );
			} else {
				setShowByNumJudges( 5 );
			}
		}

		private void ClassC5JudgeCB_CheckedChanged( object sender, EventArgs e ) {
			CheckBox curControl = (CheckBox)sender;
			if ( curControl.Checked ) {
				setShowByNumJudges( 5 );
			} else {
				setShowByNumJudges( 3 );
			}
		}

		private void ScoreEntry_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.SlalomEntry_Width = this.Size.Width;
				Properties.Settings.Default.SlalomEntry_Height = this.Size.Height;
				Properties.Settings.Default.SlalomEntry_Location = this.Location;
			}
			if ( myPassRunCount == 0 ) return;

			if ( StartTimerButton.Visible ) {
				StartTimerButton_Click( null, null );
			}
			int curHours, curMins, curSeconds, curTimeDiff;
			String curMethodName = "Slalom:ScoreEntry:FormClosed";
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
				if ( e.RowIndex >= 0 ) {
					view.Rows[e.RowIndex].ErrorText = "an error";
				}
				e.ThrowException = false;
			}
		}

		private void navSaveItem_Click( object sender, EventArgs e ) {
			try {
				if ( saveScore() ) isDataModified = false;

			} catch ( Exception excp ) {
				MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
			}
		}

		private bool saveScore() {
			String curMethodName = "Slalom:ScoreEntry:saveScore: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			if ( StartTimerButton.Visible ) StartTimerButton_Click( null, null );

			if ( myRecapRow == null || myRecapRow.Index < 0 ) return true;
			if ( slalomRecapDataGridView.Rows.Count <= 0 ) return true;

			try {
				String curMemberId = (String)myRecapRow.Cells["MemberIdRecap"].Value;
				String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;
				byte curRound = Convert.ToByte( (String)myRecapRow.Cells["RoundRecap"].Value );

				if ( !( curMemberId.Equals( (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) ) {
					// Search grid to set current skier position
					MessageBox.Show( "Unable to continue save processing"
						+ "\n Current id of skier being scored       = " + curMemberId
						+ "\n Current id skier in running order list = " + (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value );
					return false;
				}

				// Update skier score for current pass
				decimal curScoreRecap = saveSkierPassScore( curMemberId, curAgeGroup, curRound );
				if ( curScoreRecap < 0M ) {
					MessageBox.Show( "Attempt to update score for current pass has failed" );
					return false;
				}

				// Update skier total score for round
				int rowsProc = saveSkierScore( curMemberId, curAgeGroup, curRound, curScoreRecap );
				if ( rowsProc == 0 ) return false;

				//Send scores and information to any defined external reporting system
				transmitExternalScoreboard( curMemberId, curAgeGroup, curRound, myRecapRow.Index + 1 );

				// Check to see if score is equal to or great than divisions current record score
				String curCheckRecordMsg = SlalomEventData.myCheckEventRecord.checkRecordSlalom( curAgeGroup, scoreTextBox.Text, (byte)myScoreRow["SkiYearAge"], (string)myScoreRow["Gender"] );
				if ( curCheckRecordMsg == null ) curCheckRecordMsg = "";
				if ( curCheckRecordMsg.Length > 1 ) {
					MessageBox.Show( curCheckRecordMsg );
				}

				// Update run statistics
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

				return true;

			} catch ( Exception excp ) {
				String curMsg = "Error attempting to update skier score \n" + excp.Message + "\n" + curSqlStmt.ToString();
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return false;
			}
		}

		/*
		 * Update skier score for current pass
		 */
		private int saveSkierScore( String curMemberId, String curAgeGroup, byte curRound, decimal curScoreRecap ) {
			String curMethodName = "saveSkierScore: ";
			int rowsProc = 0;
			String curStatus = "TBD", curBoatCode = "", curNote = ""; ;
			
			String curEventClass = (String)scoreEventClass.SelectedValue;
			String curTimeInTol = HelperFunctions.getViewRowColValue( myRecapRow, "TimeInTolRecap", "N" );
			String curReride = HelperFunctions.getViewRowColValue( myRecapRow, "RerideRecap", "N" );

			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( curAgeGroup, SlalomSpeedSelection.MaxSpeedKph );
			String curStartLengthMeters = SlalomLineSelect.CurrentValue;
			Int16 curStartSpeedKph = SlalomSpeedSelection.SelectSpeekKph;
			if ( myScoreRow != null ) {
				curStartLengthMeters = (String)myScoreRow["StartLen"];
				curStartSpeedKph = (byte)myScoreRow["StartSpeed"];
			}

			Decimal curScore = 0, curNopsScore = 0;
			try {
				curScore = Convert.ToDecimal( scoreTextBox.Text );
			} catch {
				curScore = 0;
			}
			try {
				curNopsScore = Convert.ToDecimal( nopsScoreTextBox.Text );
			} catch {
				curNopsScore = 0;
			}
			
			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = curScore.ToString( "###.00" );
			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = hcapScoreTextBox.Text;

			try {
				curNote = noteTextBox.Text;
				curNote = curNote.Replace( "'", "''" );
			} catch {
				curNote = "";
			}

			curBoatCode = calcBoatCodeFromDisplay( TourBoatTextbox.Text );

			try {
				String curValue = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value;
				if ( curValue.Equals( "4-Done" ) ) curStatus = "Complete";
				if ( curValue.Equals( "2-InProg" ) ) curStatus = "InProg";
				if ( curValue.Equals( "3-Error" ) ) curStatus = "Error";
				if ( curValue.Equals( "1-TBD" ) ) curStatus = "TBD";
			} catch {
				curStatus = "TBD";
			}

			String tempLineLengthDesc = HelperFunctions.getDataRowColValue( myPassRow, "LineLengthOffDesc", "" );
			String curFinalRopeOff = tempLineLengthDesc.Substring( 0, tempLineLengthDesc.IndexOf( " - " ) );
			Decimal curPassSpeedMph = HelperFunctions.getDataRowColValueDecimal( myPassRow, "SpeedMph", 0 );
			Int16 curPassSpeedKph = Convert.ToInt16( HelperFunctions.getDataRowColValueDecimal( myPassRow, "SpeedKph", 0 ) );
			Decimal curPassLineLengthMeters = HelperFunctions.getDataRowColValueDecimal( myPassRow, "LineLengthMeters", 0 );

			if ( curScoreRecap == 6M && curStatus != "Complete" && curReride != "Y" ) {
				myPassRowLastCompleted = SlalomEventData.getPassRow( curPassSpeedKph, SlalomLineSelect.CurrentShowValueNum );

			} else {
				myPassRowLastCompleted = myPassRow;
				for ( int idx = myRecapRow.Index - 1; idx >= 0; idx-- ) {
					DataGridViewRow tempRecapRow = slalomRecapDataGridView.Rows[idx];
					decimal tempScoreRecap = HelperFunctions.getViewRowColValueDecimal( tempRecapRow, "ScoreRecap", "0" );
					bool curRerideInd = false, curScoreProtCkd = false;
					if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "RerideRecap", "N" ) ) ) curRerideInd = true;
					if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( tempRecapRow, "ScoreProtRecap", "N" ) ) ) curScoreProtCkd = true;
					if ( tempScoreRecap == 6M ) {
						if ( curRerideInd ) {
							if ( curScoreProtCkd ) {
								Int16 tempPassSpeedKph = Convert.ToInt16( (String)tempRecapRow.Cells["PassSpeedKphRecap"].Value );
								Decimal tempPassLineLengthMeters = Convert.ToDecimal( (String)tempRecapRow.Cells["PassLineLengthRecap"].Value );
								myPassRowLastCompleted = SlalomEventData.getPassRow( tempPassSpeedKph, tempPassLineLengthMeters );
								break;
							}

						} else {
							Int16 tempPassSpeedKph = Convert.ToInt16( (String)tempRecapRow.Cells["PassSpeedKphRecap"].Value );
							Decimal tempPassLineLengthMeters = Convert.ToDecimal( (String)tempRecapRow.Cells["PassLineLengthRecap"].Value );
							myPassRowLastCompleted = SlalomEventData.getPassRow( tempPassSpeedKph, tempPassLineLengthMeters );
							break;
						}
					}
				}
			}

			if ( ( (int)myPassRow["SlalomSpeedNum"] + (int)myPassRow["SlalomLineNum"] - 1 )
				> ( (int)myPassRowLastCompleted["SlalomSpeedNum"] + (int)myPassRowLastCompleted["SlalomLineNum"] ) ) {
				DataRow tempPassRow = null;
				if ( (int)myPassRow["SlalomSpeedNum"] == (int)myPassRowLastCompleted["SlalomSpeedNum"] ) {
					tempPassRow = SlalomEventData.getPassRow( (int)myPassRowLastCompleted["SlalomSpeedNum"], ( (int)myPassRowLastCompleted["SlalomLineNum"] + 1 ) );
				} else {
					int tempSpeedNum = (int)myPassRowLastCompleted["SlalomSpeedNum"];
					tempSpeedNum++;
					tempPassRow = SlalomEventData.getPassRow( tempSpeedNum, (int)myPassRowLastCompleted["SlalomLineNum"] );
				}
				curFinalRopeOff = ( (String)tempPassRow["LineLengthOffDesc"] ).Substring( 0, ( (String)tempPassRow["LineLengthOffDesc"] ).IndexOf( " - " ) );
				curPassLineLengthMeters = (Decimal)tempPassRow["LineLengthMeters"];
				curPassSpeedMph = Convert.ToInt16( (Decimal)tempPassRow["SpeedMph"] );
				curPassSpeedKph = Convert.ToInt16( (Decimal)tempPassRow["SpeedKph"] );
			}

			StringBuilder curSqlStmt = new StringBuilder( "" );
			if ( isScoreRowExist( SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound ) ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update SlalomScore Set " );
				curSqlStmt.Append( "Score = " + curScore.ToString( "##0.00" ) );
				curSqlStmt.Append( ", NopsScore = " + curNopsScore.ToString( "##0.00" ) );
				curSqlStmt.Append( ", StartSpeed = " + curStartSpeedKph.ToString( "00" ) );
				curSqlStmt.Append( ", StartLen = '" + curStartLengthMeters + "'" );
				curSqlStmt.Append( ", MaxSpeed = " + curMaxSpeedKphDiv.ToString( "00" ) );
				curSqlStmt.Append( ", Boat = '" + curBoatCode + "'" );
				curSqlStmt.Append( ", EventClass = '" + curEventClass + "'" );

				curSqlStmt.Append( ", Status = '" + curStatus + "'" );
				curSqlStmt.Append( ", FinalSpeedMph = " + curPassSpeedMph.ToString( "00.0" ) );
				curSqlStmt.Append( ", FinalSpeedKph = " + curPassSpeedKph.ToString( "00" ) );
				curSqlStmt.Append( ", FinalLen = '" + curPassLineLengthMeters.ToString( "00.00" ) + "'" );
				curSqlStmt.Append( ", FinalLenOff = '" + curFinalRopeOff.ToString() + "'" );
				curSqlStmt.Append( ", FinalPassScore = " + curScoreRecap );

				if ( myPassRowLastCompleted != null ) {
					curSqlStmt.Append( ", CompletedSpeedMph = " + Convert.ToInt16( (Decimal)myPassRowLastCompleted["SpeedMph"] ).ToString() );
					curSqlStmt.Append( ", CompletedSpeedKph = " + Convert.ToInt16( (Decimal)myPassRowLastCompleted["SpeedKph"] ).ToString() );
				}

				curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
				curSqlStmt.Append( ", Note = '" + curNote + "' " );
				curSqlStmt.Append( "Where SanctionId = '" + SlalomEventData.mySanctionNum + "' " );
				curSqlStmt.Append( "AND MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( "AND AgeGroup = '" + curAgeGroup + "' " );
				curSqlStmt.Append( "AND Round = " + curRound );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

			} else { 
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert SlalomScore ( " );
				curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, EventClass, Score, NopsScore" );
				curSqlStmt.Append( ", StartSpeed, StartLen, MaxSpeed, Boat" );
				curSqlStmt.Append( ", Status, FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff, FinalPassScore" );
				if ( myPassRowLastCompleted != null ) {
					curSqlStmt.Append( ", CompletedSpeedMph, CompletedSpeedKph" );
				}
				curSqlStmt.Append( ", LastUpdateDate, InsertDate, Note" );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + SlalomEventData.mySanctionNum + "'" );
				curSqlStmt.Append( ", '" + curMemberId + "'" );
				curSqlStmt.Append( ", '" + curAgeGroup + "'" );
				curSqlStmt.Append( ", " + curRound );
				curSqlStmt.Append( ", '" + curEventClass + "'" );
				curSqlStmt.Append( ", " + curScore.ToString( "##0.00" ) );
				curSqlStmt.Append( ", " + curNopsScore.ToString( "##0.00" ) );
				curSqlStmt.Append( ", " + curStartSpeedKph.ToString( "00" ) );
				curSqlStmt.Append( ", '" + curStartLengthMeters + "'" );
				curSqlStmt.Append( ", " + curMaxSpeedKphDiv.ToString( "00" ) );
				curSqlStmt.Append( ", '" + curBoatCode + "'" );

				curSqlStmt.Append( ", '" + curStatus + "'" );
				curSqlStmt.Append( ", " + curPassSpeedMph.ToString( "00.0" ) );
				curSqlStmt.Append( ", " + curPassSpeedKph.ToString( "00" ) );
				curSqlStmt.Append( ", '" + curPassLineLengthMeters + "'" );
				curSqlStmt.Append( ", '" + curFinalRopeOff + "'" );
				curSqlStmt.Append( ", '" + curScoreRecap + "'" );

				if ( myPassRowLastCompleted != null ) {
					curSqlStmt.Append( ", " + Convert.ToInt16( (Decimal)myPassRowLastCompleted["SpeedMph"] ).ToString() );
					curSqlStmt.Append( ", " + Convert.ToInt16( (Decimal)myPassRowLastCompleted["SpeedKph"] ).ToString() );
				}

				curSqlStmt.Append( ", GETDATE(), GETDATE()" );
				curSqlStmt.Append( ", '" + curNote + "'" );
				curSqlStmt.Append( ")" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
			}

			if ( rowsProc == 0 ) {
				MessageBox.Show( "Warning: Score for skier was not successfully updated.  Plese try again." );
				return rowsProc;
			}

			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = curScore.ToString( "###.00" );
			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = hcapScoreTextBox.Text;

			getSkierScoreByRound( curMemberId, curAgeGroup, curRound );
			if ( myScoreDataTable.Rows.Count > 0 ) myScoreRow = myScoreDataTable.Rows[0];

			return rowsProc;
		}

		/*
		 * Update skier score for current pass
		 */
		private decimal saveSkierPassScore( String curMemberId, String curAgeGroup, byte curRound ) {
			String curMethodName = "saveSkierPassScore: ";
			int rowsProc = 0;

			Int16 curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
			Decimal curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
			myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			int curSkierRunNum = myRecapRow.Index + 1;

			String curReride = HelperFunctions.getViewRowColValue( myRecapRow, "RerideRecap", "N" );
			String curJudge1Score = HelperFunctions.getViewRowColValue( myRecapRow, "Judge1ScoreRecap", "Null" );
			String curJudge2Score = HelperFunctions.getViewRowColValue( myRecapRow, "Judge2ScoreRecap", "Null" );
			String curJudge3Score = HelperFunctions.getViewRowColValue( myRecapRow, "Judge3ScoreRecap", "Null" );
			String curJudge4Score = HelperFunctions.getViewRowColValue( myRecapRow, "Judge4ScoreRecap", "Null" );
			String curJudge5Score = HelperFunctions.getViewRowColValue( myRecapRow, "Judge5ScoreRecap", "Null" );

			String curEntryGate1 = "0";
			if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "GateEntry1Recap", "False" ) ) ) curEntryGate1 = "1";
			String curEntryGate2 = "0";
			if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "GateEntry2Recap", "False" ) ) ) curEntryGate2 = "1";
			String curEntryGate3 = "0";
			if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "GateEntry3Recap", "False" ) ) ) curEntryGate3 = "1";
			String curExitGate1 = "0";
			if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "GateExit1Recap", "False" ) ) ) curExitGate1 = "1";
			String curExitGate2 = "0";
			if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "GateExit2Recap", "False" ) ) ) curExitGate2 = "1";
			String curExitGate3 = "0";
			if ( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "GateExit3Recap", "False" ) ) ) curExitGate3 = "1";

			String curScoreRecap = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" );
			if ( curScoreRecap.Length < 1 ) curScoreRecap = "0";
			String curTimeInTol = HelperFunctions.getViewRowColValue( myRecapRow, "TimeInTolRecap", "N" );
			String curBoatTime = HelperFunctions.getViewRowColValue( myRecapRow, "BoatTimeRecap", "0" );
			if ( curBoatTime.Length == 0 ) curBoatTime = "0";

			String curBoatPathGood = HelperFunctions.getViewRowColValue( myRecapRow, "BoatPathGoodRecap", "Y" );
			String curProtectedScoreRecap = HelperFunctions.getViewRowColValue( myRecapRow, "ProtectedScoreRecap", "0" );

			String curScoreProt = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreProtRecap", "N" );
			String curRerideReason = HelperFunctions.getViewRowColValue( myRecapRow, "RerideReasonRecap", "" );
			curRerideReason = curRerideReason.Replace( "'", "''" );
			String curNote = HelperFunctions.getViewRowColValue( myRecapRow, "NoteRecap", "" );
			curNote = curNote.Replace( "'", "''" );

			StringBuilder curSqlStmt = new StringBuilder( "" );
			if ( isRecapRowExist( SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, curSkierRunNum ) ) {
				curSqlStmt.Append( "Update SlalomRecap Set " );
				curSqlStmt.Append( "PassSpeedKph = " + curPassSpeedKph.ToString( "00" ) );
				curSqlStmt.Append( ", PassLineLength = " + curPassLineLengthMeters.ToString( "00.00" ) );
				curSqlStmt.Append( ", Reride = '" + curReride + "'" );
				curSqlStmt.Append( ", Judge1Score = " + curJudge1Score );
				curSqlStmt.Append( ", Judge2Score = " + curJudge2Score );
				curSqlStmt.Append( ", Judge3Score = " + curJudge3Score );
				curSqlStmt.Append( ", Judge4Score = " + curJudge4Score );
				curSqlStmt.Append( ", Judge5Score = " + curJudge5Score );
				curSqlStmt.Append( ", EntryGate1 = " + curEntryGate1 );
				curSqlStmt.Append( ", EntryGate2 = " + curEntryGate2 );
				curSqlStmt.Append( ", EntryGate3 = " + curEntryGate3 );
				curSqlStmt.Append( ", ExitGate1 = " + curExitGate1 );
				curSqlStmt.Append( ", ExitGate2 = " + curExitGate2 );
				curSqlStmt.Append( ", ExitGate3 = " + curExitGate3 );
				curSqlStmt.Append( ", BoatTime = " + curBoatTime );
				curSqlStmt.Append( ", Score = " + curScoreRecap );
				curSqlStmt.Append( ", ProtectedScore = " + curProtectedScoreRecap );
				curSqlStmt.Append( ", TimeInTol = '" + curTimeInTol + "'" );
				curSqlStmt.Append( ", ScoreProt = '" + curScoreProt + "'" );
				curSqlStmt.Append( ", BoatPathGood = '" + curBoatPathGood + "'" );
				curSqlStmt.Append( ", LastUpdateDate = getdate()" );
				curSqlStmt.Append( ", RerideReason = '" + curRerideReason + "'" );
				curSqlStmt.Append( ", Note = '" + curNote + "' " );
				curSqlStmt.Append( "Where SanctionId = '" + SlalomEventData.mySanctionNum + "' " );
				curSqlStmt.Append( "AND MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( "AND AgeGroup = '" + curAgeGroup + "' " );
				curSqlStmt.Append( "AND Round = " + curRound + " " );
				curSqlStmt.Append( "AND SkierRunNum = " + curSkierRunNum );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			} else {
				curSqlStmt.Append( "Insert SlalomRecap ( " );
				curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round" );
				curSqlStmt.Append( ", SkierRunNum, Reride, PassSpeedKph, PassLineLength" );
				curSqlStmt.Append( ", Judge1Score, Judge2Score, Judge3Score, Judge4Score, Judge5Score" );
				curSqlStmt.Append( ", EntryGate1, EntryGate2, EntryGate3" );
				curSqlStmt.Append( ", ExitGate1, ExitGate2, ExitGate3" );
				curSqlStmt.Append( ", BoatTime, Score, ProtectedScore, TimeInTol, ScoreProt, BoatPathGood, LastUpdateDate, InsertDate, RerideReason, Note" );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + SlalomEventData.mySanctionNum + "'" );
				curSqlStmt.Append( ", '" + curMemberId + "'" );
				curSqlStmt.Append( ", '" + curAgeGroup + "'" );
				curSqlStmt.Append( ", " + curRound );
				curSqlStmt.Append( ", " + curSkierRunNum );
				curSqlStmt.Append( ", '" + curReride + "'" );
				curSqlStmt.Append( ", " + curPassSpeedKph.ToString( "00" ) );
				curSqlStmt.Append( ", " + curPassLineLengthMeters.ToString( "00.00" ) );
				curSqlStmt.Append( ", " + curJudge1Score );
				curSqlStmt.Append( ", " + curJudge2Score );
				curSqlStmt.Append( ", " + curJudge3Score );
				curSqlStmt.Append( ", " + curJudge4Score );
				curSqlStmt.Append( ", " + curJudge5Score );
				curSqlStmt.Append( ", " + curEntryGate1 );
				curSqlStmt.Append( ", " + curEntryGate2 );
				curSqlStmt.Append( ", " + curEntryGate3 );
				curSqlStmt.Append( ", " + curExitGate1 );
				curSqlStmt.Append( ", " + curExitGate2 );
				curSqlStmt.Append( ", " + curExitGate3 );
				curSqlStmt.Append( ", " + curBoatTime );
				curSqlStmt.Append( ", " + curScoreRecap );
				curSqlStmt.Append( ", " + curProtectedScoreRecap );
				curSqlStmt.Append( ", '" + curTimeInTol + "'" );
				curSqlStmt.Append( ", '" + curScoreProt + "'" );
				curSqlStmt.Append( ", '" + curBoatPathGood + "'" );
				curSqlStmt.Append( ", getdate(), getdate()" );
				curSqlStmt.Append( ", '" + curRerideReason + "'" );
				curSqlStmt.Append( ", '" + curNote + "'" );
				curSqlStmt.Append( ")" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			}
			Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
			if ( rowsProc > 0 ) {
				isDataModified = false;
				myRecapRow.Cells["Updated"].Value = "N";
				refreshScoreSummaryWindow();
				return Convert.ToDecimal( curScoreRecap );
			}

			String curMsg = ":Zero rows added or updated.  This is unexpected.  "
				+ "Review data for current pass and correct any input errors.  "
				+ "If error persists then send an email to the development team and include the tournament log and note the time of the issue.";
			Log.WriteFile( curMethodName + curMsg );
			MessageBox.Show( curMethodName + curMsg );
			return 0;
		}

		private void transmitExternalScoreboard( String curMemberId, String curAgeGroup, byte curRound, int curSkierRunNum ) {
			String curSkierName = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value;
			String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;

			if ( LiveWebLabel.Visible ) {
				LiveWebHandler.sendCurrentSkier( "Slalom", SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, curSkierRunNum );
			}
			if ( WscHandler.isConnectActive ) {
				String skierFed = HelperFunctions.getDataRowColValue( SlalomEventData.myTourRow, "Federation", "" );
				if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "Federation", "" ) ) ) {
					skierFed = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Federation"].Value;
				}
				WscHandler.sendAthleteScore( (String)myRecapRow.Cells["MemberIdRecap"].Value
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value
					, "Slalom"
					, skierFed
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["State"].Value
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value
					, Convert.ToInt16( (String)myRecapRow.Cells["RoundRecap"].Value )
					, Convert.ToInt16( (String)myRecapRow.Cells["skierPassRecap"].Value )
					, Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value )
					, (String)myRecapRow.Cells["PassLineLengthRecap"].Value
					, (String)myRecapRow.Cells["ScoreRecap"].Value );
			}
			if ( WaterskiConnectLabel.Visible && !WscHandler.isConnectActive ) WaterskiConnectLabel.Visible = false;
		}

		/*
		 * Retrieve data for current tournament
		 * Used for initial load and to refresh data after updates
		 */
		private void loadTourEventRegView() {

			try {
				isDataModified = false;

				if ( myTourEventRegDataTable == null || myTourEventRegDataTable.Rows.Count == 0 ) {
					slalomRecapDataGridView.Rows.Clear();
					TourEventRegDataGridView.Rows.Clear();
					return;
				}

				winStatusMsg.Text = "Retrieving tournament entries";
				Cursor.Current = Cursors.WaitCursor;

				isOrderByRoundActive = isRunOrderByRound( Convert.ToByte( roundActiveSelect.RoundValue ) );
				mySortCommand = getSortCommand();

				slalomRecapDataGridView.Rows.Clear();
				TourEventRegDataGridView.Rows.Clear();
				myTourEventRegDataTable.DefaultView.Sort = mySortCommand;
				myTourEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
				DataTable curDataTable = myTourEventRegDataTable.DefaultView.ToTable();

				DataGridViewRow curViewRow;
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					myEventRegViewIdx = TourEventRegDataGridView.Rows.Add();
					curViewRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];

					curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
					curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
					curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];
					curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
					curViewRow.Cells["AgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );

					String curEventGroup = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
					String curRunOrderGroup = HelperFunctions.getDataRowColValue( curDataRow, "RunOrderGroup", "" );
					if ( HelperFunctions.isObjectPopulated( curRunOrderGroup ) ) {
						curViewRow.Cells["EventGroup"].Value = curEventGroup + "-" + curRunOrderGroup;
					} else {
						curViewRow.Cells["EventGroup"].Value = curEventGroup;
					}

					curViewRow.Cells["Gender"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Gender", "" );
					curViewRow.Cells["EventClass"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventClass", "" );
					curViewRow.Cells["TeamCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TeamCode", "" );
					curViewRow.Cells["Order"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RunOrder", "0" );
					curViewRow.Cells["Score"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Score", "" );
					curViewRow.Cells["ScoreWithHcap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "HCapScore", "" );
					curViewRow.Cells["RankingScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RankingScore", "" );
					curViewRow.Cells["RankingRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RankingRating", "" );
					setEventRegRowStatus( HelperFunctions.getDataRowColValue( curDataRow, "Status", "TBD" ), curViewRow );
					curViewRow.Cells["HCapBase"].Value = HelperFunctions.getDataRowColValue( curDataRow, "HCapBase", "" );
					curViewRow.Cells["HCapScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "HCapScore", "" );
					curViewRow.Cells["State"].Value = HelperFunctions.getDataRowColValue( curDataRow, "State", "" );
					curViewRow.Cells["Federation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" );
				}

				if ( curDataTable.Rows.Count > 0 ) {
					myEventRegViewIdx = 0;
					TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
					setSlalomScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
					setSlalomRecapEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
					setNewRowPos();
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "Error retrieving slalom tournament entries \n" + ex.Message );
			}
			Cursor.Current = Cursors.Default;
			try {
				int curRowPos = myEventRegViewIdx + 1;
				RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + TourEventRegDataGridView.Rows.Count.ToString();
			} catch {
				RowStatusLabel.Text = "";
			}

			LiveWebLabel.Visible = false;
			if ( LiveWebHandler.LiveWebMessageHandlerActive ) LiveWebLabel.Visible = true;

			String curWarnMsg = "Warn:RunOrder:Round:" + roundActiveSelect.RoundValue;
			if ( !( myCompletedNotices.Contains( curWarnMsg ) ) ) {
				if ( isOrderByRoundActive ) {
					MessageBox.Show( "WARNING \nThis running order is specific for this round" );
					myCompletedNotices.Add( curWarnMsg );
				}
			}
		}

		/*
		 * Retrieve data for current tournament
		 * Used for initial load and to refresh data after updates
		 */
		private void loadBoatTimeView( Int16 inSpeed ) {
			winStatusMsg.Text = "Retrieving boat times";
			Cursor.Current = Cursors.WaitCursor;
			try {
				listBoatTimesDataGridView.Rows.Clear();
				SlalomEventData.myTimesDataTable.DefaultView.RowFilter = "ListCode like '" + inSpeed.ToString() + "-" + (String)mySlalomSetAnalysis.ClassRowSkier["ListCode"] + "-%'";
				DataTable curDataTable = SlalomEventData.myTimesDataTable.DefaultView.ToTable();
				if ( curDataTable.Rows.Count == 0 ) {
					SlalomEventData.myTimesDataTable.DefaultView.RowFilter = "ListCode like '" + inSpeed.ToString() + "-C-%'";
					curDataTable = SlalomEventData.myTimesDataTable.DefaultView.ToTable();
					if ( curDataTable.Rows.Count == 0 ) {
						MessageBox.Show( "Boat times not available.  Pleaes contact the development team as this is an error." );
					}
				}

				DataGridViewRow curViewRow;
				int curViewIdx = 0;
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					curViewIdx = listBoatTimesDataGridView.Rows.Add();
					curViewRow = listBoatTimesDataGridView.Rows[curViewIdx];

					curViewRow.Cells["BoatTimeKey"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ListCode", "" );
					curViewRow.Cells["ListCodeNum"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ListCodeNum", "" );
					curViewRow.Cells["ActualTime"].Value = HelperFunctions.getDataRowColValue( curDataRow, "CodeValue", "" );
					curViewRow.Cells["FastTimeTol"].Value = HelperFunctions.getDataRowColValue( curDataRow, "MinValue", "" );
					curViewRow.Cells["SlowtimeTol"].Value = HelperFunctions.getDataRowColValue( curDataRow, "MaxValue", "" );
					curViewRow.Cells["TimeKeyDesc"].Value = HelperFunctions.getDataRowColValue( curDataRow, "CodeDesc", "" ).Substring(6);
				}
				listBoatTimesDataGridView.CurrentCell = listBoatTimesDataGridView.Rows[0].Cells["BoatTimeKey"];

			} catch ( Exception ex ) {
				MessageBox.Show( "Error retrieving boat times \n" + ex.Message );

			} finally {
				Cursor.Current = Cursors.Default;
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
			myEventDelaySeconds += Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventDelayStartTime ) ).TotalSeconds );
			if ( EventDelayReasonTextBox.Text.Length > 1 ) {
				Log.WriteFile( "Slalom:ScoreEntry:Event delayed: " + EventDelayReasonTextBox.Text );
			}
			EventDelayReasonTextBox.Visible = false;
			EventDelayReasonLabel.Visible = false;
			StartTimerButton.Visible = false;
			PauseTimerButton.Visible = true;
		}

		private void setSlalomScoreEntry( DataGridViewRow inTourEventRegRow, int inRound ) {
			Int16 curMinSpeedKph = 0, curMaxSpeedKph = 0;

			SimulationPassButton.Enabled = false;
			SimulationPassButton.Visible = false;
			if ( !(WscHandler.isConnectActive) ) navWaterSkiConnect_Click( null, null );

			String curMemberId = (String)inTourEventRegRow.Cells["MemberId"].Value;
			String curAgeGroup = (String)inTourEventRegRow.Cells["AgeGroup"].Value;
			String curEventGroup = (String)inTourEventRegRow.Cells["EventGroup"].Value;
			String curSkierEventClass = (String)inTourEventRegRow.Cells["EventClass"].Value;
			DataRow curClassRowSkier = SlalomEventData.getSkierClass( curSkierEventClass );

			activeSkierName.Text = (String)inTourEventRegRow.Cells["SkierName"].Value;
			this.Text = myTitle + " - " + activeSkierName.Text;

			Int16 curDefaultStartSpeed = SlalomEventData.getMaxSpeedOrigData( curAgeGroup, SlalomSpeedSelection.MaxSpeedKph );
			if ( (Decimal)curClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
				curMaxSpeedKph = curDefaultStartSpeed;
			} else {
				curMaxSpeedKph = SlalomEventData.getMaxSpeedData( curAgeGroup, SlalomSpeedSelection.MaxSpeedKph );
			}

			SlalomSpeedSelection.refreshSelectionList( curAgeGroup, curMaxSpeedKph, curMinSpeedKph );
			SlalomLineSelect.showLongLine();

			driverDropdown.SelectedValue = "";
			driverDropdown.Text = "";

			checkLoadOfficialAssgmt( curAgeGroup, curEventGroup );

			getSkierScoreByRound( curMemberId, curAgeGroup, inRound );
			if ( myScoreDataTable.Rows.Count > 0 ) {
				myScoreRow = myScoreDataTable.Rows[0];
				roundActiveSelect.RoundValue = ( (Byte)myScoreRow["Round"] ).ToString();

				SlalomLineSelect.CurrentValue = HelperFunctions.getDataRowColValue( myScoreRow, "StartLen", "" );
				if ( SlalomLineSelect.CurrentValue.Length == 0 ) {
					DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select( "ListCodeNum = 18.25" );
					SlalomLineSelect.CurrentValue = (String)curLineRow[0]["ListCode"];
				}
				
				SlalomSpeedSelection.SelectSpeekKph = Convert.ToByte( HelperFunctions.getDataRowColValue( myScoreRow, "StartSpeed", curMaxSpeedKph.ToString() ) );
				scoreEventClass.SelectedValue = HelperFunctions.getDataRowColValue( myScoreRow, "EventClass", HelperFunctions.getViewRowColValue( inTourEventRegRow, "EventClass", SlalomEventData.myTourClass ) );
				/*
				 * Disabling long line hide
				 * It is not a vaid line length for IWWF but not for AWSA scoring
				 * Skier should be able to select it and then only matters when sending scores to IWWF which they will handle
				DataRow curClassRowSkier = SlalomEventData.getSkierClass( (String)scoreEventClass.SelectedValue );
				if ( ((Decimal)curClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"]) || SlalomEventData.isIwwfEvent() ) {
					SlalomLineSelect.hideLongLine();
				}
				 */
				scoreTextBox.Text = HelperFunctions.getDataRowColValue( myScoreRow, "Score", "" );
				TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = scoreTextBox.Text;

				nopsScoreTextBox.Text = HelperFunctions.getDataRowColValue( myScoreRow, "NopsScore", "" );
				hcapScoreTextBox.Text = "";
				String curHCapScore = HelperFunctions.getViewRowColValue( inTourEventRegRow, "HCapScore", "" );
				if ( HelperFunctions.isObjectPopulated( curHCapScore ) ) {
					hcapScoreTextBox.Text = ( (Decimal)myScoreRow["Score"] + Decimal.Parse( curHCapScore ) ).ToString( "##,###0.0" );
				}

				noteTextBox.Text = HelperFunctions.getDataRowColValue( myScoreRow, "Note", "" );

				if ( myScoreRow["Boat"] == System.DBNull.Value ) {
					TourBoatTextbox.Text = "";
				} else {
					TourBoatTextbox.Text = setApprovedBoatSelectEntry( (String)myScoreRow["Boat"] );
					TourBoatTextbox.Select( 0, 0 );
				}

			} else {
				myScoreRow = null;
				DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select( "ListCodeNum = 18.25" );

				if ( WscHandler.isConnectActive ) {
					SimulationPassButton.Enabled = true;
					SimulationPassButton.Visible = true;
					SimulationPassButton.Text = "Simulation Pass";
				}

				SlalomLineSelect.CurrentValue = (String)curLineRow[0]["ListCode"];
				SlalomSpeedSelection.SelectSpeekKph = curDefaultStartSpeed;
				scoreEventClass.SelectedValue = (String)inTourEventRegRow.Cells["EventClass"].Value;
				TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = "";
				TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = "";
				mySlalomSetAnalysis = new SlalomSetAnalysis( inTourEventRegRow, null, null, myNumJudges );

				/*
				 * Disabling long line hide
				 * It is not a vaid line length for IWWF but not for AWSA scoring
				 * Skier should be able to select it and then only matters when sending scores to IWWF which they will handle
				DataRow curClassRowSkier = SlalomEventData.getSkierClass( (String)scoreEventClass.SelectedValue );
				if ( ((Decimal)curClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"]) || SlalomEventData.isIwwfEvent() ) {
					SlalomLineSelect.hideLongLine();
				} else {
					SlalomLineSelect.showLongLine();
				}
				 */

				scoreTextBox.Text = "";
				nopsScoreTextBox.Text = "";
				hcapScoreTextBox.Text = "";
				noteTextBox.Text = "";
				TourBoatTextbox.Text = buildBoatModelNameDisplay();
				TourBoatTextbox.Select( 0, 0 );
				scoreEntryBegin();
				loadBoatTimeView( curMaxSpeedKph );
			}
		}

		private void setSlalomRecapEntry( DataGridViewRow inTourEventRegRow, int inRound ) {
			isLastPassSelectActive = false;

			if ( slalomRecapDataGridView.Rows.Count > 0 ) slalomRecapDataGridView.Rows.Clear();
			String curMemberId = (String)inTourEventRegRow.Cells["MemberId"].Value;
			String curAgeGroup = (String)inTourEventRegRow.Cells["AgeGroup"].Value;
			String curSkierClass = (String)inTourEventRegRow.Cells["EventClass"].Value;
			
			Cursor.Current = Cursors.WaitCursor;
			myRecapDataTable = getSkierRecapByRound( curMemberId, curAgeGroup, inRound );

			if ( myRecapDataTable == null || myRecapDataTable.Rows.Count == 0 ) {
				if ( scoreTextBox.Text.Length > 0 ) {
					int rowsProc = cleanSkierSlalomScore( (String)inTourEventRegRow.Cells["MemberId"].Value, (String)inTourEventRegRow.Cells["AgeGroup"].Value, inRound );
					Log.WriteFile( String.Format( "setSlalomRecapEntry:Found score with no passes.  Deleting round {0} score for {1}"
						, inRound, (String)inTourEventRegRow.Cells["SkierName"].Value ) );
					setSlalomScoreEntry( inTourEventRegRow, inRound );
					myScoreRow = null;
				}
				scoreEntryBegin();
				Cursor.Current = Cursors.Default;
				return;
			}

			foreach ( DataRow curDataRow in myRecapDataTable.Rows ) {
				setSlalomRecapRow( inTourEventRegRow, curDataRow );
			}

			scoreEntryInprogress();

			/* 
			 * Retrieve boat path data
			 */
			decimal curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) );
			loadBoatPathDataGridView( "Slalom", curSkierClass
				, (String)myRecapRow.Cells["MemberIdRecap"].Value
				, (String)myRecapRow.Cells["RoundRecap"].Value
				, (String)myRecapRow.Cells["skierPassRecap"].Value
				, curPassScore );

			if ( !( HelperFunctions.isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) )
				&& !( HelperFunctions.isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) )
				&& curPassScore == 6
				&& HelperFunctions.isObjectEmpty( noteTextBox.Text )
				&& (String)myRecapRow.Cells["TimeInTolRecap"].Value == "Y"
				) {
				if ( mySlalomSetAnalysis.isExitGatesGood() ) {
					isAddRecapRowInProg = true;
					isRecapRowEnterHandled = false;
					Timer curTimerObj = new Timer();
					curTimerObj.Interval = 5;
					curTimerObj.Tick += new EventHandler( addRecapRowTimer );
					curTimerObj.Start();

				} else {
					skierPassMsg.ForeColor = Color.Red;
					skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
				}
			}

			Cursor.Current = Cursors.Default;
		}

		private void setSlalomRecapRow( DataGridViewRow inTourEventRegRow, DataRow curDataRow ) {
			String curFlag;
			Int16 curPassSpeedKph = 0;
			Decimal curPassLineLengthMeters = 0M;

			Int16 curMaxSpeed = SlalomSpeedSelection.MaxSpeedKph;
			curPassLineLengthMeters = 0M;
			curPassSpeedKph = 0;
			
			int curViewIdx = slalomRecapDataGridView.Rows.Add();
			DataGridViewRow curViewRow = slalomRecapDataGridView.Rows[curViewIdx];
			myRecapRow = curViewRow;
			mySlalomSetAnalysis = new SlalomSetAnalysis( inTourEventRegRow, slalomRecapDataGridView.Rows, myRecapRow, myNumJudges );

			try {
				curPassLineLengthMeters = (Decimal)curDataRow["PassLineLength"];
				curViewRow.Cells["PassLineLengthRecap"].Value = curPassLineLengthMeters.ToString( "00.00" );

				curPassSpeedKph = (Int16)( (Byte)curDataRow["PassSpeedKph"] );
				curViewRow.Cells["PassSpeedKphRecap"].Value = curPassSpeedKph.ToString();

			} catch {
				MessageBox.Show( "Current pass has invalid line length or speed defined and can't be process.  Please note skier information and contact support" );
				return;
			}

			curViewRow.Cells["Updated"].Value = "N";
			curViewRow.Cells["SanctionIdRecap"].Value = (String)curDataRow["SanctionId"];
			curViewRow.Cells["MemberIdRecap"].Value = (String)curDataRow["MemberId"];
			curViewRow.Cells["AgeGroupRecap"].Value = (String)curDataRow["AgeGroup"];
			curViewRow.Cells["RoundRecap"].Value = ( (Byte)curDataRow["Round"] ).ToString();
			curViewRow.Cells["RoundRecap"].ToolTipText = ( (DateTime)curDataRow["LastUpdateDate"] ).ToString( "MM/dd HH:mm:ss" );

			curViewRow.Cells["skierPassRecap"].Value = ( curViewIdx + 1 ).ToString();
			if ( curPassLineLengthMeters == 0 || curPassSpeedKph == 0 ) {
				MessageBox.Show( "Current pass has invalid line length or speed defined and can't be process.  Please note skier information and contact support" );
				return;
			}

			curFlag = HelperFunctions.getDataRowColValue( curDataRow, "TimeInTol", "N" );
			curViewRow.Cells["TimeInTolRecap"].Value = curFlag;
			if ( curFlag.Equals( "Y" ) ) {
				curViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;
			} else {
				curViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
			}

			curViewRow.Cells["BoatTimeRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "BoatTime", "" );
			curViewRow.Cells["ProtectedScoreRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ProtectedScore", "" );
			curViewRow.Cells["ScoreRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Score", "" );
			curViewRow.Cells["BoatPathGoodRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "BoatPathGood", "Y" );
			curViewRow.Cells["ScoreProtRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScoreProt", "N" );
			curViewRow.Cells["RerideRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Reride", "N" );
			curViewRow.Cells["RerideReasonRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RerideReason", "" );

			curViewRow.Cells["Judge1ScoreRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Judge1Score", "" );
			curViewRow.Cells["Judge2ScoreRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Judge2Score", "" );
			curViewRow.Cells["Judge3ScoreRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Judge3Score", "" );
			curViewRow.Cells["Judge4ScoreRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Judge4Score", "" );
			curViewRow.Cells["Judge5ScoreRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Judge5Score", "" );

			curViewRow.Cells["GateEntry1Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EntryGate1", "false" );
			curViewRow.Cells["GateEntry2Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EntryGate2", "false" );
			curViewRow.Cells["GateEntry3Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EntryGate3", "false" );

			curViewRow.Cells["GateExit1Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ExitGate1", "false" );
			curViewRow.Cells["GateExit2Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ExitGate2", "false" );
			curViewRow.Cells["GateExit3Recap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ExitGate3", "false" );

			curViewRow.Cells["NoteRecap"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Note", "" );

			mySlalomSetAnalysis.PassTimeRow = mySlalomSetAnalysis.getBoatTimeRow( curPassSpeedKph, Convert.ToInt16( HelperFunctions.getDataRowColValue( curDataRow, "Score", "6" ).Substring( 0, 1 ) ) );
			loadBoatTimeView( curPassSpeedKph );
			ActivePassDesc.Text = (String)curViewRow.Cells["NoteRecap"].Value;
			if ( curViewIdx < (myRecapDataTable.Rows.Count - 1) ) {
				foreach ( DataGridViewCell curCell in curViewRow.Cells ) {
					if ( curCell.Visible && !( curCell.ReadOnly ) ) {
						curCell.ReadOnly = true;
					}
				}
			
			} else {
				skierPassMsg.ForeColor = Color.DarkBlue;
				skierPassMsg.Text = mySlalomSetAnalysis.getBoatTimeMsg();
				if ( curPassLineLengthMeters < SlalomLineSelect.CurrentValueNum ) {
					SlalomLineSelect.showCurrentValue( curPassLineLengthMeters );
				}
				if ( curPassSpeedKph > SlalomSpeedSelection.SelectSpeekKph ) {
					SlalomSpeedSelection.showActiveValue( curPassSpeedKph );
				}
			}
		}

		private void setEventRegRowStatus( DataGridViewRow curRow ) {
			String curStatus = "";
			Decimal curScore;
			try {
				bool curTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "TimeInTolRecap", "Y" ) );
				bool curRerideReqd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "RerideRecap", "Y" ) );
				bool curScoreProtCkd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreProtRecap", "Y" ) );
				bool curBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "BoatPathGoodRecap", "Y" ) );

				curScore = Convert.ToDecimal( (String)curRow.Cells["ScoreRecap"].Value );
				if ( curTimeGood && curBoatPathGood ) {
					if ( curScore < 6 ) {
						curStatus = "4-Done";
					} else {
						if ( myNumJudges == 1 ) {
							if ( (Boolean)curRow.Cells["GateExit1Recap"].Value ) {
								curStatus = "2-InProg";
							}
						} else {
							int curGateExitValue = 0;
							if ( !( (Boolean)curRow.Cells["GateExit1Recap"].Value ) ) curGateExitValue++;
							if ( !( (Boolean)curRow.Cells["GateExit2Recap"].Value ) ) curGateExitValue++;
							if ( !( (Boolean)curRow.Cells["GateExit3Recap"].Value ) ) curGateExitValue++;
							if ( curGateExitValue < 2 ) {
								curStatus = "2-InProg";
							} else {
								curStatus = "4-Done";
							}
						}
					}
				
				} else if ( !(curBoatPathGood) ) {
					curStatus = "2-InProg";

				} else if ( curScoreProtCkd ) {
						curStatus = "4-Done";
				
				} else {
					curStatus = "3-Error";
				}
			
			} catch {
				curStatus = "1-TBD";
			}
			setEventRegRowStatus( curStatus );
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
				return;
			}
			if ( inStatus.Equals( "2-InProg" ) || inStatus.Equals( "InProg" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
				curRow.Cells["SkierName"].Style.Font = curFont;
				curRow.Cells["SkierName"].Style.ForeColor = Color.White;
				curRow.Cells["SkierName"].Style.BackColor = Color.LimeGreen;
				curRow.Cells["Status"].Value = "2-InProg";
				return;
			}
			if ( inStatus.Equals( "3-Error" ) || inStatus.Equals( "Error" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
				curRow.Cells["SkierName"].Style.Font = curFont;
				curRow.Cells["SkierName"].Style.ForeColor = Color.White;
				curRow.Cells["SkierName"].Style.BackColor = Color.Red;
				curRow.Cells["Status"].Value = "3-Error";
				return;
			}
			if ( inStatus.Equals( "1-TBD" ) || inStatus.Equals( "TBD" ) ) {
				Font curFont = new Font( "Arial Narrow", 9, FontStyle.Regular );
				curRow.Cells["SkierName"].Style.Font = curFont;
				curRow.Cells["SkierName"].Style.ForeColor = SystemColors.ControlText;
				curRow.Cells["SkierName"].Style.BackColor = SystemColors.Window;
				curRow.Cells["Status"].Value = "1-TBD";
				return;
			}
		}

		private void ResendPassButton_Click( object sender, EventArgs e ) {
			if ( myRecapRow == null ) return;

			Int16 curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
			Decimal curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
			sendPassDataEwsc( TourEventRegDataGridView.Rows[myEventRegViewIdx], curPassSpeedKph, curPassLineLengthMeters );
		}
		
		private void SimulationPassButton_Click( object sender, EventArgs e ) {
			String curSimMemberId = "000000000";
			Int16 curPassSpeedKph = 58;
			Decimal curPassLineLengthMeters = (decimal)18.25;

			if ( SimulationPassButton.Text.ToLower().Equals("check bpms") ) {
				loadBoatPathDataGridView( "Slalom", "L", curSimMemberId, "1", "1", (decimal)6 );
				if ( InvalidateBoatPathButton.Visible ) SimulationPassButton.Text = "Simulation Pass";
				DialogResult dialogResult = MessageBox.Show( "No deviations found.  Do you want to reset the dialog?", "Simulation Reset", MessageBoxButtons.YesNo );
				if ( dialogResult == DialogResult.Yes ) SimulationPassButton.Text = "Simulation Pass";

			} else {
				StringBuilder curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( String.Format("Delete From BoatPath Where SanctionId = '{0}' AND MemberId = '{1}'", SlalomEventData.mySanctionNum, curSimMemberId ) );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				SimulationPassButton.Text = "Check BPMS";
				WscHandler.sendPassData( curSimMemberId
					, "Simulation Pass"
					, "Slalom"
					, "USA"
					, ""
					, ""
					, ""
					, ""
					, ""
					, "1"
					, 1
					, curPassSpeedKph
					, curPassLineLengthMeters.ToString( "00.00" )
					, ""
					, (String)driverDropdown.SelectedValue );
			}


			/*
			 */

		}

		private void addPassButton_Click( object sender, EventArgs e ) {
			DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
			int cellIndex = 0, rowIndex = 0;
			if ( slalomRecapDataGridView.Rows.Count > 0 ) {
				rowIndex = slalomRecapDataGridView.Rows.Count - 1;
				cellIndex = 0;
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[rowIndex].Cells[cellIndex];

			} else {
				if ( checkForSkierRoundScore( curEventRegRow.Cells["MemberId"].Value.ToString()
						, Convert.ToInt32( roundActiveSelect.RoundValue )
						, curEventRegRow.Cells["AgeGroup"].Value.ToString() )
					) {
					MessageBox.Show( "Skier already has a score in this round" );
					return;
				}

				setEventRegRowStatus( "2-InProg" );

				String curEventGroup = curEventRegRow.Cells["EventGroup"].Value.ToString();
				if ( WscHandler.isConnectActive ) WscHandler.sendOfficialsAssignments( "Slalom", curEventGroup, Convert.ToInt16( roundActiveSelect.RoundValue ) );
				if ( WaterskiConnectLabel.Visible && !WscHandler.isConnectActive ) WaterskiConnectLabel.Visible = false;

				if ( !( curEventGroup.Equals( myPrevEventGroup ) ) ) {
					/*
					 * Provide a warning message for class R events when official assignments have not been entered for the round and event group
					 * These assignments are not mandatory but they are strongly preferred and are very helpful for the TCs
					 */
					if ( (Decimal)SlalomEventData.myClassRowTour["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
						String curWarnMsg = String.Format( "Warn:Officials:Round:{0}:EventGroup:{1}", roundActiveSelect.RoundValue, curEventGroup );
						if ( !( myCompletedNotices.Contains( curWarnMsg ) ) ) {
							if ( myCheckOfficials.officialAsgmtCount == 0 ) {
								String curAgeGroup = curEventRegRow.Cells["AgeGroup"].Value.ToString();
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
			}

			myLastPassSelectValue = "";
			addRecapRow();
			setNewRowPos();
		}

		private void addRecapRowTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( addRecapRowTimer );

			isAddRecapRowInProg = false;
			isRecapRowEnterHandled = false;

			addRecapRow();
			setNewRowPos();
		}

		private void addRecapRow() {
			String curMethodName = "Slalom:ScoreEntry:addRecapRow: ";
			Int16 curSkierPass, curPassSpeedKph, curStartSpeedKph;
			Decimal curPassLineLengthMeters = 0M, curPassScore = 0M;
			isAddRecapRowInProg = true;
			addPassButton.Enabled = false;

			DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
			Int16 curMaxSpeedKph = SlalomSpeedSelection.MaxSpeedKph;

			if ( slalomRecapDataGridView.Rows.Count > 0 ) {
				// Add new pass when at least one pass already exists
				try {
					curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
					curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
					myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );

					if ( myPassRow == null ) {
						myPassRow = null;
						MessageBox.Show( String.Format( "Unable to continue, missing slalom data for speed(kph) {0} Line Length {1}", curPassSpeedKph.ToString(), curPassLineLengthMeters.ToString( "00.00" ) ) );
						return;
					}

					curSkierPass = (Int16)int.Parse( (String)myRecapRow.Cells["skierPassRecap"].Value );
					String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;
					curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) );

					bool curTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "TimeInTolRecap", "Y" ) );
					bool curRerideReqd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "RerideRecap", "Y" ) );
					bool curScoreProtCkd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreProtRecap", "Y" ) );
					bool curBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "BoatPathGoodRecap", "Y" ) );

					if ( ( curTimeGood && curBoatPathGood && !( curRerideReqd ) && curPassScore == 6 )
							|| ( !( curTimeGood ) && curBoatPathGood && curScoreProtCkd && curPassScore == 6 )
							|| ( curTimeGood && curBoatPathGood && curRerideReqd && curScoreProtCkd && curPassScore == 6 )
							|| ( curRerideReqd && curTimeGood && curSkierPass == 1 && mySlalomSetAnalysis.isExitGatesGood() && !( mySlalomSetAnalysis.isEntryGatesGood() ) )
							) {
						if ( curPassSpeedKph < curMaxSpeedKph ) {
							/*
							Determine if skier division and class qualify the skier to use the alternate scoring method.
							This alternate scoring method no longer requires that the skier be scored at long line when at less than max speed
							Rule 10.06 for ski year 2017
							 */
							DataRow curClassRowSkier = SlalomEventData.getSkierClass( HelperFunctions.getViewRowColValue( curEventRegRow, "EventClass", SlalomEventData.myTourClass ) );
							if ( SlalomEventData.isQualifiedAltScoreMethod( curAgeGroup, (String)curClassRowSkier["ListCode"] ) ) {
								if ( !( curRerideReqd )
									|| ( curRerideReqd && curTimeGood && curSkierPass == 1 && mySlalomSetAnalysis.isExitGatesGood() && !( mySlalomSetAnalysis.isEntryGatesGood() ) ) ) {
									nextPassWithOption();
								} else {
									AddNewRecapRow();
								}

							} else {
								// Next pass as determined by the standard scoring rules
								Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( curAgeGroup, SlalomSpeedSelection.MaxSpeedKph );
								if ( curPassSpeedKph < curMaxSpeedKphDiv ) {
									addPassNextSpeed();
								} else {
									addPassNextLine();
								}
							}

						} else {
							addPassNextLine();
						}

					} else {
						if ( curRerideReqd ) {
							AddNewRecapRow();
						
						} else {
							Log.WriteFile( String.Format( curMethodName + ":Unexpected fall thru when attempting to determine the speed and line length for the next pass "
								+ "MemberId: {0}, AgeGroup: {1}, Round: {2}, PassNum: {3}, Speed: {4}, LineLenth: {5}"
								, HelperFunctions.getViewRowColValue( myRecapRow, "MemberIdRecap", "" )
								, HelperFunctions.getViewRowColValue( myRecapRow, "AgeGroupRecap", "" )
								, HelperFunctions.getViewRowColValue( myRecapRow, "RoundRecap", "" )
								, curSkierPass
								, curPassSpeedKph
								, curPassLineLengthMeters
								) );
							MessageBox.Show( "Check new pass because it might be the same as the previous pass.  If yes please send log file to David Allen" );
						}

					}

				} catch ( Exception ex ) {
					myPassRow = null;
					MessageBox.Show( String.Format( "Exception encountered on current pass : Exception: {0}", ex.Message ) );
				}

				return;
			}

			// Add new pass when this is the first pass being added
			scoreEntryInprogress();

			curPassLineLengthMeters = SlalomLineSelect.CurrentValueNum;
			curStartSpeedKph = SlalomSpeedSelection.SelectSpeekKph;
			if ( curStartSpeedKph > curMaxSpeedKph ) {
				SlalomSpeedSelection.SelectSpeekKph = curStartSpeedKph;

				DataRow curClassRowSkier = SlalomEventData.getSkierClass( HelperFunctions.getViewRowColValue( curEventRegRow, "EventClass", SlalomEventData.myTourClass ) );
				if ( (Decimal)curClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] || SlalomEventData.isIwwfEvent() ) {
					MessageBox.Show( "You have selected a starting speed above the division maximum."
					+ "\n" + "This is allowed although the skier will only be scored at the division maximum speed."
					+ "\n" + "If this was not intended you should delete the pass and re-select the starting speed."
					);

				} else {
					MessageBox.Show( "You have selected a starting speed above the division maximum."
					+ "\n" + "This is allowed for Class C with the scored at the division maximum speed."
					);
				}
			}

			myPassRow = SlalomEventData.getPassRow( curStartSpeedKph, curPassLineLengthMeters );

			AddNewRecapRow();
			return;
		}

		private void setNewRowPos() {
			if ( slalomRecapDataGridView == null || slalomRecapDataGridView.Rows.Count == 0 ) {
				optionUpButton.Enabled = false;
				deletePassButton.Enabled = false;
				ForceCompButton.Enabled = false;
				ResendPassButton.Enabled = false;
				ResendPassButton.Visible = false;
				return;
			}

			optionUpButton.Enabled = false;
			if ( slalomRecapDataGridView.Rows.Count > 1 ) optionUpButton.Enabled = true;
			deletePassButton.Enabled = true;
			ForceCompButton.Enabled = true;

			if ( WscHandler.isConnectActive ) {
				ResendPassButton.Enabled = true;
				ResendPassButton.Visible = true;
			}

			int rowIndex = 0;
			if ( slalomRecapDataGridView.Rows.Count > 0 ) {
				rowIndex = slalomRecapDataGridView.Rows.Count - 1;
				slalomRecapDataGridView.Select();
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex];
				myRecapRow = slalomRecapDataGridView.Rows[rowIndex];
				myOrigCellValue = (String)slalomRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex].Value;
			}
		}

		private void deletePassButton_Click( object sender, EventArgs e ) {
			String curMethodName = "Slalom:ScoreEntry:deletePassButton_Click: ";
			String curMsg = "";
			int curRowIdx = 0;

			if ( slalomRecapDataGridView.Rows.Count <= 0 ) {
				slalomRecapDataGridView.Focus();
				return;
			}

			try {
				myRecapRow = slalomRecapDataGridView.Rows[slalomRecapDataGridView.Rows.Count - 1];
				curRowIdx = myRecapRow.Index;

				String curMemberId = (String)myRecapRow.Cells["MemberIdRecap"].Value;
				String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;
				byte curRound = Convert.ToByte( (String)myRecapRow.Cells["RoundRecap"].Value );
				Int16 curSkierRunNum = -1;
				curSkierRunNum = Convert.ToInt16( HelperFunctions.getViewRowColValue( myRecapRow, "skierPassRecap", "-1" ));
				if ( isRecapRowExist( SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, curSkierRunNum ) ) {
					// Delete row that has previously been scored
					deleteScoredPass( curMemberId, curAgeGroup, curRound, curSkierRunNum, curRowIdx );

				} else {
					deleteUnscoredPass( curMemberId, curAgeGroup, curRound, curSkierRunNum, curRowIdx );
				}

			} catch ( Exception excp ) {
				curMsg = ":Error attempting to save changes \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			}
			slalomRecapDataGridView.Focus();
		}

		private void deleteUnscoredPass( String curMemberId, String curAgeGroup, byte curRound, Int16 curSkierRunNum, int curRowIdx ) {
			String curMethodName = "Slalom:ScoreEntry:deleteUnscoredPass: ";
			String curMsg = "";
			Int16 curPassSpeedKph = 0;
			Decimal curPassLineLengthMeters = 0M;
			StringBuilder curSqlStmt = new StringBuilder( "" );

			curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
			curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
			myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			if ( myPassRow == null ) {
				curMsg = curMethodName + String.Format( "Invalid data detected, slalom pass reference not found for KPH={0}, Line={1}", curPassSpeedKph, curPassLineLengthMeters );
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return;
			}

			if ( curPassSpeedKph != SlalomSpeedSelection.CurrentShowSpeedKph ) SlalomSpeedSelection.showActiveValue( curPassSpeedKph );
			if ( curPassLineLengthMeters < SlalomLineSelect.CurrentShowValueNum ) curPassLineLengthMeters = SlalomLineSelect.CurrentShowValueNum;
			if ( curPassLineLengthMeters != SlalomLineSelect.CurrentShowValueNum ) SlalomLineSelect.showCurrentValue( curPassLineLengthMeters );

			// Remove deleted row from display gird
			slalomRecapDataGridView.Rows.RemoveAt( curRowIdx );

			if ( curRowIdx <= 0 ) {
				deleteScore( curMemberId, curAgeGroup, curRound );
				return;
			}

			// Delete requested pass and then reset score and positioning to new last pass
			curRowIdx--;
			myRecapRow = (DataGridViewRow)slalomRecapDataGridView.Rows[curRowIdx];
			curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
			curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
			myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			if ( myPassRow == null ) {
				curMsg = curMethodName + String.Format( "Invalid data detected, slalom pass reference not found for KPH={0}, Line={1}", curPassSpeedKph, curPassLineLengthMeters );
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			} else {
				if ( curPassSpeedKph != SlalomSpeedSelection.CurrentShowSpeedKph ) SlalomSpeedSelection.showActiveValue( curPassSpeedKph );
				if ( curPassLineLengthMeters < SlalomLineSelect.CurrentShowValueNum ) curPassLineLengthMeters = SlalomLineSelect.CurrentShowValueNum;
				if ( curPassLineLengthMeters != SlalomLineSelect.CurrentShowValueNum ) SlalomLineSelect.showCurrentValue( curPassLineLengthMeters );
			}

			// Force score reclaculation using new last available pass score
			mySlalomSetAnalysis = new SlalomSetAnalysis( TourEventRegDataGridView.Rows[myEventRegViewIdx], slalomRecapDataGridView.Rows, myRecapRow, myNumJudges );
			decimal curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) );
			SlalomScoreCalc( curPassScore );

			scoreEntryInprogress();
			setEventRegRowStatus( "2-InProg" );
			foreach ( DataGridViewCell curCell in myRecapRow.Cells ) {
				if ( curCell.OwningColumn.Name.StartsWith( "Judge" )
					|| curCell.OwningColumn.Name.StartsWith( "Gate" )
					|| curCell.OwningColumn.Name.StartsWith( "Time" )
					|| curCell.OwningColumn.Name.StartsWith( "BoatTime" )
					|| curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
					|| curCell.OwningColumn.Name.StartsWith( "Reride" )
					|| curCell.OwningColumn.Name.StartsWith( "Note" )
					|| curCell.OwningColumn.Name.StartsWith( "Updated" )
					) {
					curCell.ReadOnly = false;
				}
			}
			setNewRowPos();

			ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;
			if ( curPassSpeedKph > 0 ) {
				mySlalomSetAnalysis.PassTimeRow = mySlalomSetAnalysis.getBoatTimeRow( curPassSpeedKph, 6 );
				loadBoatTimeView( curPassSpeedKph );
				skierPassMsg.ForeColor = Color.DarkBlue;
				skierPassMsg.Text = mySlalomSetAnalysis.getBoatTimeMsg();
			}
		}

		private void deleteScoredPass( String curMemberId, String curAgeGroup, byte curRound, Int16 curSkierRunNum, int curRowIdx ) {
			String curMethodName = "Slalom:ScoreEntry:deleteScoredPass: ";
			String curMsg = "";
			int rowsProc = 0;
			Int16 curPassSpeedKph = 0;
			Decimal curPassLineLengthMeters = 0M;
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				curSqlStmt = new StringBuilder(
					String.Format( "Delete SlalomRecap Where SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3} AND SkierRunNum = {4}"
					, SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, curSkierRunNum ).ToString() );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
				if ( rowsProc == 0 ) return;

			} catch ( Exception excp ) {
				curMsg = curMethodName + "Error deleting slalom pass \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return;
			}

			DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
			myRecapRow = (DataGridViewRow)slalomRecapDataGridView.Rows[curRowIdx];
			curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
			curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
			myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			if ( myPassRow == null ) {
				curMsg = curMethodName + String.Format( "Invalid data detected, slalom pass reference not found for KPH={0}, Line={1}", curPassSpeedKph, curPassLineLengthMeters );
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return;
			}

			if ( curPassSpeedKph != SlalomSpeedSelection.CurrentShowSpeedKph ) SlalomSpeedSelection.showActiveValue( curPassSpeedKph );
			if ( curPassLineLengthMeters < SlalomLineSelect.CurrentShowValueNum ) curPassLineLengthMeters = SlalomLineSelect.CurrentShowValueNum;
			if ( curPassLineLengthMeters != SlalomLineSelect.CurrentShowValueNum ) SlalomLineSelect.showCurrentValue( curPassLineLengthMeters );

			// Remove deleted row (last row in display grid)
			slalomRecapDataGridView.Rows.RemoveAt( curRowIdx );

			if ( curRowIdx <= 0 ) {
				deleteScore( curMemberId, curAgeGroup, curRound );

			} else {
				#region Set position to last row in grid and recalculate the score
				curRowIdx--;
				myRecapRow = (DataGridViewRow)slalomRecapDataGridView.Rows[curRowIdx];
				curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
				curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
				myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
				if ( myPassRow == null ) {
					curMsg = curMethodName + String.Format( "Invalid data detected, slalom pass reference not found for KPH={0}, Line={1}", curPassSpeedKph, curPassLineLengthMeters );
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );

				} else {
					if ( curPassSpeedKph != SlalomSpeedSelection.CurrentShowSpeedKph ) SlalomSpeedSelection.showActiveValue( curPassSpeedKph );
					if ( curPassLineLengthMeters < SlalomLineSelect.CurrentShowValueNum ) curPassLineLengthMeters = SlalomLineSelect.CurrentShowValueNum;
					if ( curPassLineLengthMeters != SlalomLineSelect.CurrentShowValueNum ) SlalomLineSelect.showCurrentValue( curPassLineLengthMeters );
				}

				// Force score reclaculation using new last available pass score
				mySlalomSetAnalysis = new SlalomSetAnalysis( curEventRegRow, slalomRecapDataGridView.Rows, myRecapRow, myNumJudges );
				decimal curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) );
				SlalomScoreCalc( curPassScore );

				scoreEntryInprogress();
				setEventRegRowStatus( "2-InProg" );
				foreach ( DataGridViewCell curCell in myRecapRow.Cells ) {
					if ( curCell.OwningColumn.Name.StartsWith( "Judge" )
						|| curCell.OwningColumn.Name.StartsWith( "Gate" )
						|| curCell.OwningColumn.Name.StartsWith( "Time" )
						|| curCell.OwningColumn.Name.StartsWith( "BoatTime" )
						|| curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
						|| curCell.OwningColumn.Name.StartsWith( "Reride" )
						|| curCell.OwningColumn.Name.StartsWith( "Note" )
						|| curCell.OwningColumn.Name.StartsWith( "Updated" )
						) {
						curCell.ReadOnly = false;
					}
				}
				setNewRowPos();
				ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;
				if ( curPassSpeedKph > 0 ) {
					mySlalomSetAnalysis.PassTimeRow = mySlalomSetAnalysis.getBoatTimeRow( curPassSpeedKph, 6 );
					loadBoatTimeView( curPassSpeedKph );
					skierPassMsg.ForeColor = Color.DarkBlue;
					skierPassMsg.Text = mySlalomSetAnalysis.getBoatTimeMsg();
				}
				#endregion
			}

			if ( LiveWebLabel.Visible ) {
				LiveWebHandler.sendCurrentSkier( "Slalom", SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0 );
			}
		}

		private void deleteScore( String curMemberId, String curAgeGroup, byte curRound ) {
			String curMethodName = "Slalom:ScoreEntry:deleteScore: ";
			myRecapRow = null;
			String curMsg = "";
			int rowsProc = 0;
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				curSqlStmt = new StringBuilder( "Delete SlalomScore " );
				curSqlStmt.Append( "Where SanctionId = '" + SlalomEventData.mySanctionNum + "' " );
				curSqlStmt.Append( " AND MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( " AND AgeGroup = '" + curAgeGroup + "' " );
				curSqlStmt.Append( " AND Round = '" + curRound + "' " );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

			} catch ( Exception excp ) {
				curMsg = ":Error deleting skier score \n" + excp.Message;
				Log.WriteFile( curMethodName + curMsg );
			}
			myScoreRow = null;

			SlalomSpeedSelection.SelectSpeekKph = SlalomSpeedSelection.SelectSpeekKph;
			SlalomLineSelect.CurrentValue = SlalomLineSelect.CurrentValue;

			addPassButton.Enabled = true;
			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = "";
			scoreTextBox.Text = "";
			nopsScoreTextBox.Text = "";
			hcapScoreTextBox.Text = "";
			noteTextBox.Text = "";
			scoreEntryBegin();
			setEventRegRowStatus( "1-TBD" );
			refreshScoreSummaryWindow();
			setNewRowPos();
		}

		private void nextPassWithOption() {
			if ( isLastPassSelectActive ) return;

			String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;
			Int16 curMaxSpeed = SlalomEventData.getMaxSpeedOrigData( curAgeGroup, SlalomSpeedSelection.MaxSpeedKph );
			Int16 curPassSpeedKph = Convert.ToInt16( (Decimal)myPassRow["SpeedKph"] );
			Decimal curPassLineLengthMeters = (Decimal)myPassRow["LineLengthMeters"];
			if ( curPassSpeedKph < curMaxSpeed ) {
				int curRowIdx = slalomRecapDataGridView.Rows.Count - 1;
				//
				int curLocationX = this.MdiParent.Location.X + this.Location.X
					+ slalomRecapDataGridView.Location.X
					+ slalomRecapDataGridView.GetCellDisplayRectangle( NoteRecap.Index, curRowIdx, false ).Location.X;
				// 40 + 25 + 60 + 
				int curLocationY = 85 + this.MdiParent.Location.Y + this.Location.Y
					+ slalomRecapDataGridView.Location.Y
					+ slalomRecapDataGridView.GetCellDisplayRectangle( NoteRecap.Index, curRowIdx, false ).Location.Y + 22;
				myNextPassDialog.setWindowLocation( curLocationX, curLocationY );

				if ( myLastPassSelectValue.Equals( "Speed" ) ) {
					myNextPassDialog.setNextSpeedButtonDefault();
				} else if ( myLastPassSelectValue.Equals( "Line" ) ) {
					myNextPassDialog.setNextLineButtonDefault();
				} else {
					myNextPassDialog.setNextSpeedButtonDefault();
				}

				isAddRecapRowInProg = false;
				isRecapRowEnterHandled = false;
				isLastPassSelectActive = true;
				if ( myNextPassDialog.ShowDialog() == DialogResult.OK ) {
					if ( myNextPassDialog.getDialogResult().Equals( "Line" ) ) {
						myLastPassSelectValue = myNextPassDialog.getDialogResult();
						addPassNextLine();
						isAddRecapRowInProg = true;

					} else if ( myNextPassDialog.getDialogResult().Equals( "Speed" ) ) {
						myLastPassSelectValue = myNextPassDialog.getDialogResult();
						addPassNextSpeed();
						isAddRecapRowInProg = true;
					}
				}
				isLastPassSelectActive = false;

			} else {
				addPassNextLine();
				isAddRecapRowInProg = true;
			}

			return;
		}

		private void addPassNextSpeed() {
			myLastPassSelectValue = "Speed";

			Int16 curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
			Decimal curPassLineLengthMeters = (Decimal)myPassRow["LineLengthMeters"];

			curPassSpeedKph += 3;
			myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
			if ( curPassSpeedKph != SlalomSpeedSelection.SelectSpeekKph ) {
				SlalomSpeedSelection.showActiveValue( curPassSpeedKph );
			}

			AddNewRecapRow();
		}

		private void addPassNextLine() {
			myLastPassSelectValue = "Line";
			int curViewIdx = myRecapRow.Index;
			Int16 curPassSpeedKph = Convert.ToInt16( (Decimal)myPassRow["SpeedKph"] );
			Decimal curPassLineLengthMeters = (Decimal)myPassRow["LineLengthMeters"];

			Decimal nextPassLineLengthMeters = SlalomLineSelect.getNextValue( curPassLineLengthMeters );
			myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, nextPassLineLengthMeters );

			if ( nextPassLineLengthMeters < SlalomLineSelect.CurrentValueNum ) {
				SlalomLineSelect.showCurrentValue( nextPassLineLengthMeters );
			}

			AddNewRecapRow();
		}

		/*
         * Add new slalom pass row to data grid view
         */
		private void AddNewRecapRow() {
			bool prevRerideReqd = false;
			if ( myRecapRow != null ) {
				foreach ( DataGridViewCell curCell in myRecapRow.Cells ) {
					if ( curCell.Visible && !( curCell.ReadOnly ) ) {
						curCell.ReadOnly = true;
					}
				}

				prevRerideReqd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "RerideRecap", "Y" ) );
			}

			DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
			Int16 curPassSpeedKph = Convert.ToInt16( (Decimal)myPassRow["SpeedKph"] );
			Decimal curPassLineLengthMeters = (Decimal)myPassRow["LineLengthMeters"];

			slalomRecapDataGridView.Focus();
			int curViewIdx = slalomRecapDataGridView.Rows.Add();
			myRecapRow = slalomRecapDataGridView.Rows[curViewIdx];
			mySlalomSetAnalysis = new SlalomSetAnalysis( curEventRegRow, slalomRecapDataGridView.Rows, myRecapRow, myNumJudges );

			myRecapRow.Cells["Updated"].Value = "N";
			myRecapRow.Cells["SanctionIdRecap"].Value = (String)curEventRegRow.Cells["SanctionId"].Value;
			myRecapRow.Cells["MemberIdRecap"].Value = (String)curEventRegRow.Cells["MemberId"].Value;
			myRecapRow.Cells["AgeGroupRecap"].Value = (String)curEventRegRow.Cells["AgeGroup"].Value;
			myRecapRow.Cells["RoundRecap"].Value = roundActiveSelect.RoundValue;
			myRecapRow.Cells["RoundRecap"].ToolTipText = "";

			myRecapRow.Cells["skierPassRecap"].Value = Convert.ToInt16( myRecapRow.Index + 1 ).ToString( "#0" );
			myRecapRow.Cells["PassSpeedKphRecap"].Value = curPassSpeedKph.ToString();
			myRecapRow.Cells["PassLineLengthRecap"].Value = curPassLineLengthMeters.ToString( "00.00" );

			myRecapRow.Cells["TimeInTolRecap"].Value = "N";
			myRecapRow.Cells["BoatPathGoodRecap"].Value = "Y";
			myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.clock;
			myRecapRow.Cells["BoatTimeRecap"].Value = "";
			myRecapRow.Cells["ProtectedScoreRecap"].Value = "";
			myRecapRow.Cells["ScoreRecap"].Value = "";
			myRecapRow.Cells["ScoreProtRecap"].Value = "N";
			myRecapRow.Cells["RerideRecap"].Value = "N";
			myRecapRow.Cells["RerideReasonRecap"].Value = "";
			myRecapRow.Cells["Judge1ScoreRecap"].Value = "";
			myRecapRow.Cells["Judge2ScoreRecap"].Value = "";
			myRecapRow.Cells["Judge3ScoreRecap"].Value = "";
			myRecapRow.Cells["Judge4ScoreRecap"].Value = "";
			myRecapRow.Cells["Judge5ScoreRecap"].Value = "";

			myRecapRow.Cells["GateEntry1Recap"].Value = true;
			myRecapRow.Cells["GateEntry2Recap"].Value = true;
			myRecapRow.Cells["GateEntry3Recap"].Value = true;
			myRecapRow.Cells["GateExit1Recap"].Value = true;
			myRecapRow.Cells["GateExit2Recap"].Value = true;
			myRecapRow.Cells["GateExit3Recap"].Value = true;

			myRecapRow.Cells["NoteRecap"].Value = curPassLineLengthMeters.ToString( "00.00" ) + "M"
				+ "," + (String)myPassRow["SpeedKphDesc"]
				+ "," + ( (String)myPassRow["LineLengthOffDesc"] ).Substring( 0, ( (String)myPassRow["LineLengthOffDesc"] ).IndexOf( " - " ) )
				+ "," + (String)myPassRow["SpeedMphDesc"];

			mySlalomSetAnalysis.PassTimeRow = mySlalomSetAnalysis.getBoatTimeRow( curPassSpeedKph, 6 );
			loadBoatTimeView( curPassSpeedKph );

			if ( !prevRerideReqd ) {
				skierPassMsg.ForeColor = Color.DarkBlue;
				skierPassMsg.Text = mySlalomSetAnalysis.getBoatTimeMsg();
			}

			setNewRowPos();

			ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;
			isAddRecapRowInProg = false;
			myPassRunCount++;
			if ( curViewIdx == 0 ) {
				mySkierRunCount++;
			} else {
				if ( mySkierRunCount == 0 ) {
					mySkierRunCount++;
				}
			}

			sendPassDataEwsc( curEventRegRow, curPassSpeedKph, curPassLineLengthMeters );
		}

		private void optionUpButton_Click( object sender, EventArgs e ) {
			Int16 curMaxSpeedKph = 0, curPassSpeedKph = 0;
			bool curRerideReqd = false, curScoreProtCkd = false, curTimeGood = false;
			Decimal curPassLineLengthMeters = 23M;
			if ( slalomRecapDataGridView.Rows.Count == 0 ) return;

			bool curOptAllow = true;
			String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;
			curMaxSpeedKph = SlalomSpeedSelection.MaxSpeedKph;
			Int16 curMaxSpeedKphDiv = SlalomEventData.getMaxSpeedOrigData( curAgeGroup, SlalomSpeedSelection.MaxSpeedKph );

			if ( !( HelperFunctions.isObjectEmpty( (String)myRecapRow.Cells["ScoreRecap"].Value ) ) ) {
				#region Check for next action when a score is detected in current pass
				decimal curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) );

				String curColValue = HelperFunctions.getViewRowColValue( myRecapRow, "TimeInTolRecap", "Y" );
				if ( curColValue.Equals( "Y" ) ) curTimeGood = true;
				curColValue = HelperFunctions.getViewRowColValue( myRecapRow, "RerideRecap", "Y" );
				if ( curColValue.Equals( "Y" ) ) curRerideReqd = true;
				curColValue = HelperFunctions.getViewRowColValue( myRecapRow, "ScoreProtRecap", "Y" );
				if ( curColValue.Equals( "Y" ) ) curScoreProtCkd = true;
				//curColValue = HelperFunctions.getViewRowColValue( myRecapRow, "BoatPathGoodRecap", "Y" );
				//if ( curColValue.Equals( "Y" ) ) curBoatPathGood = true;

				if ( curScoreProtCkd && curPassScore == 6 ) {
					addRecapRow();
				} else if ( !(curRerideReqd) ) {
					if ( curTimeGood ) {
						MessageBox.Show( "Opting up is not allowed when current pass has a valid time"
							+ "\nand no explict request and reason for a reride" );
						curOptAllow = false;
					} else {
						MessageBox.Show( "Opting up is not allowed when current pass has a time out of tolerance"
							+ "\nand no explict request and reason for a reride" );
						curOptAllow = false;
					}
				} else if ( curRerideReqd ) {
					addRecapRow();
				}
				#endregion
			}

			if ( curOptAllow ) {
				String curOptUpType = "next";
				curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
				curPassLineLengthMeters = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
				myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
				if ( curPassSpeedKph < curMaxSpeedKph
					|| ( curAgeGroup.StartsWith( "M" ) && curPassSpeedKph < 58 )
					|| ( curAgeGroup.StartsWith( "W" ) && curPassSpeedKph < 55 )
					) {
					if ( OptUpDialogForm.ShowDialog() == DialogResult.OK ) {
						String curResponse = OptUpDialogForm.Response;
						if ( curResponse.Equals( "speed" ) ) {
							if ( curPassSpeedKph < curMaxSpeedKphDiv ) {
								curOptUpType = "next";
							} else {
								curOptUpType = curResponse;
							}
						} else if ( curResponse.Equals( "line" ) ) {
							curOptUpType = curResponse;
						} else {
							curOptUpType = "next";
						}
					} else {
						curOptUpType = "";
					}
				}

				if ( curOptUpType.Equals( "next" ) ) {
					#region Determine next speed and line length based on current attributes
					curPassSpeedKph = Convert.ToInt16( (Decimal)myPassRow["SpeedKph"] );
					curPassLineLengthMeters = (Decimal)myPassRow["LineLengthMeters"];
					curMaxSpeedKph = SlalomSpeedSelection.MaxSpeedKph;
					if ( curMaxSpeedKph > curPassSpeedKph ) {
						curPassSpeedKph += 3;

					} else if ( curPassSpeedKph > curMaxSpeedKph ) {
						curPassSpeedKph += 3;

					} else {
						curPassLineLengthMeters = SlalomLineSelect.getNextValue( curPassLineLengthMeters );
					}
					myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );

					if ( curPassSpeedKph != SlalomSpeedSelection.SelectSpeekKph ) {
						SlalomSpeedSelection.showActiveValue( curPassSpeedKph );
					}
					if ( curPassLineLengthMeters < SlalomLineSelect.CurrentValueNum ) {
						SlalomLineSelect.showCurrentValue( curPassLineLengthMeters );
					}

					sendPassDataEwsc( TourEventRegDataGridView.Rows[myEventRegViewIdx], curPassSpeedKph, curPassLineLengthMeters );

					myRecapRow.Cells["PassSpeedKphRecap"].Value = curPassSpeedKph.ToString( "00" );
					myRecapRow.Cells["PassLineLengthRecap"].Value = curPassLineLengthMeters.ToString( "00.00" );
					//e.g. 23.00M,25kph,Long,15.5mph  OR  18.25M,46kph,15 Off,28.6mph
					myRecapRow.Cells["NoteRecap"].Value = "Opt up to "
						+ curPassLineLengthMeters.ToString( "00.00" ) + "M"
						+ "," + (String)myPassRow["SpeedKphDesc"]
						+ "," + ( (String)myPassRow["LineLengthOffDesc"] ).Substring( 0, ( (String)myPassRow["LineLengthOffDesc"] ).IndexOf( " - " ) )
						+ "," + (String)myPassRow["SpeedMphDesc"];
					ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;
					setNewRowPos();
					loadBoatTimeView( curPassSpeedKph );

					#endregion

				} else if ( curOptUpType.Equals( "speed" ) ) {
					#region Calculate next speed 
					curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
					curPassLineLengthMeters = (Decimal)myPassRow["LineLengthMeters"];

					if ( curPassSpeedKph >= curMaxSpeedKphDiv ) {
						SlalomLineSelect.resetCurrentValue( curPassLineLengthMeters );
						curPassLineLengthMeters = Convert.ToDecimal( (String)slalomRecapDataGridView.Rows[myRecapRow.Index - 1].Cells["PassLineLengthRecap"].Value );
						if ( curPassLineLengthMeters < SlalomLineSelect.CurrentValueNum ) {
							SlalomLineSelect.showCurrentValue( curPassLineLengthMeters );
						}
					}

					curPassSpeedKph += 3;
					myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );
					if ( curPassSpeedKph != SlalomSpeedSelection.SelectSpeekKph ) {
						SlalomSpeedSelection.showActiveValue( curPassSpeedKph );
					}

					sendPassDataEwsc( TourEventRegDataGridView.Rows[myEventRegViewIdx], curPassSpeedKph, curPassLineLengthMeters );

					myRecapRow.Cells["PassSpeedKphRecap"].Value = curPassSpeedKph.ToString( "00" );
					myRecapRow.Cells["PassLineLengthRecap"].Value = curPassLineLengthMeters.ToString( "00.00" );
					//e.g. 23.00M,25kph,Long,15.5mph  OR  18.25M,46kph,15 Off,28.6mph
					myRecapRow.Cells["NoteRecap"].Value = "Opt up to "
						+ curPassLineLengthMeters.ToString( "00.00" ) + "M"
						+ "," + (String)myPassRow["SpeedKphDesc"]
						+ "," + ( (String)myPassRow["LineLengthOffDesc"] ).Substring( 0, ( (String)myPassRow["LineLengthOffDesc"] ).IndexOf( " - " ) )
						+ "," + (String)myPassRow["SpeedMphDesc"];
					ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;
					setNewRowPos();
					loadBoatTimeView( curPassSpeedKph );

					MessageBox.Show( "Opting up to " + curPassLineLengthMeters + " at " + curPassSpeedKph + "kph" );
					#endregion

				} else if ( curOptUpType.Equals( "line" ) ) {
					#region Calculate next line length 
					curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );
					curPassLineLengthMeters = (Decimal)myPassRow["LineLengthMeters"];

					Decimal nextPassLineLengthMeters = SlalomLineSelect.getNextValue( curPassLineLengthMeters );
					myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, nextPassLineLengthMeters );
					if ( nextPassLineLengthMeters < SlalomLineSelect.CurrentValueNum ) {
						SlalomLineSelect.showCurrentValue( nextPassLineLengthMeters );
					}

					sendPassDataEwsc( TourEventRegDataGridView.Rows[myEventRegViewIdx], curPassSpeedKph, curPassLineLengthMeters );

					myRecapRow.Cells["PassLineLengthRecap"].Value = nextPassLineLengthMeters.ToString( "00.00" );
					//e.g. 23.00M,25kph,Long,15.5mph  OR  18.25M,46kph,15 Off,28.6mph
					myRecapRow.Cells["NoteRecap"].Value = "Opt up to "
							+ nextPassLineLengthMeters.ToString( "00.00" ) + "M"
							+ "," + (String)myPassRow["SpeedKphDesc"]
							+ "," + ( (String)myPassRow["LineLengthOffDesc"] ).Substring( 0, ( (String)myPassRow["LineLengthOffDesc"] ).IndexOf( " - " ) )
							+ "," + (String)myPassRow["SpeedMphDesc"];
					ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;

					MessageBox.Show( "Opting up to " + nextPassLineLengthMeters + " at " + curPassSpeedKph + "kph" );

					#endregion
				}
			}
			slalomRecapDataGridView.Focus();
		}

		private void sendPassDataEwsc( DataGridViewRow tourEventRegRow, Int16 curPassSpeedKph, decimal curPassLineLengthMeters ) {
			if ( WscHandler.isConnectActive ) {
					String skierFed = HelperFunctions.getDataRowColValue( SlalomEventData.myTourRow, "Federation", "" );
					if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( tourEventRegRow, "Federation", "" ) ) ) {
						skierFed = HelperFunctions.getViewRowColValue( tourEventRegRow, "Federation", "" );
					}
					WscHandler.sendPassData( (String)myRecapRow.Cells["MemberIdRecap"].Value
						, (String)tourEventRegRow.Cells["SkierName"].Value
						, "Slalom"
						, skierFed
						, (String)tourEventRegRow.Cells["State"].Value
						, (String)tourEventRegRow.Cells["EventGroup"].Value
						, (String)tourEventRegRow.Cells["AgeGroup"].Value
						, (String)tourEventRegRow.Cells["Gender"].Value
						, (String)scoreEventClass.SelectedValue
						, roundActiveSelect.RoundValue
						, Convert.ToInt16( (String)myRecapRow.Cells["skierPassRecap"].Value )
						, curPassSpeedKph
						, curPassLineLengthMeters.ToString( "00.00" )
						, "0"
						, (String)driverDropdown.SelectedValue );
			}
			if ( WaterskiConnectLabel.Visible && !WscHandler.isConnectActive ) WaterskiConnectLabel.Visible = false;

		}

		private void refreshScoreSummaryWindow() {
			// Check for open instance of selected form
			//SystemMain curMdiWindow = (SystemMain)this.Parent.Parent;
			//for ( int idx = 0; idx < curMdiWindow.MdiChildren.Length; idx++ ) {
			//    Form curForm = (Form)( curMdiWindow.MdiChildren[idx] );
			//    if ( curForm.Name.Equals( "SlalomSummary" ) ) {
			//        SlalomSummary curSummaryWindow = (SlalomSummary)curForm;
			//        curSummaryWindow.navRefresh_Click( null, null );
			//    } else if ( curForm.Name.Equals( "SlalomSummaryTeam" ) ) {
			//        SlalomSummaryTeam curSummaryWindow = (SlalomSummaryTeam)curForm;
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

		private void navSort_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			mySortCommand = getSortCommand();
			sortDialogForm.SortCommand = mySortCommand;
			sortDialogForm.ShowDialog( this );

			// Determine if the OK button was clicked on the dialog box.
			if ( sortDialogForm.DialogResult == DialogResult.OK ) {
				mySortCommand = sortDialogForm.SortCommand;
				if ( isOrderByRoundActive ) {
					myTourProperties.RunningOrderSortSlalomRound = mySortCommand;
				} else {
					myTourProperties.RunningOrderSortSlalom = mySortCommand;
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
			myExportData.ExportData( SlalomEventData.mySanctionNum, "Slalom", curMemberId, curAgeGroup, curEventGroup, curRound );
		}

		private void navExportLw_Click( object sender, EventArgs e ) {
			navExport_Click( null, null );
			ExportLiveWeb.uploadExportFile( "Export", "Slalom", SlalomEventData.mySanctionNum );
		}

		private void navExport_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			String[] curTableName = { "TourReg", "EventReg", "EventRunOrder", "SlalomScore", "SlalomRecap", "TourReg", "OfficialWork", "OfficialWorkAsgmt", "BoatTime", "BoatPath" };
			String[] curSelectCommand = SlalomEventData.buildScoreExport( roundActiveSelect.RoundValue, EventGroupList.SelectedItem.ToString(), myFilterCmd );
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
				if ( LiveWebHandler.connectLiveWebHandler( SlalomEventData.mySanctionNum ) ) {
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
						String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
						byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
						LiveWebHandler.sendCurrentSkier( "Slalom", SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0 );
				}

			} else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "ResendAll" ) ) {
				if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
					String curEventGroup = EventGroupList.SelectedItem.ToString();
					byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
					LiveWebHandler.sendSkiers( "Slalom", SlalomEventData.mySanctionNum, curRound, curEventGroup );
				}

			} else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "DiableSkier" ) ) {
				if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
					String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
					String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
					String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
					String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
					byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
					LiveWebHandler.sendDisableCurrentSkier( "Slalom", SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound );
					//ExportLiveWeb.exportCurrentSkierSlalom( SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup
				}

			} else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "DiableAllSkier" ) ) {
				if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
					String curEventGroup = EventGroupList.SelectedItem.ToString();
					byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
					LiveWebHandler.sendDisableSkiers( "Slalom", SlalomEventData.mySanctionNum, curRound, curEventGroup );
					//ExportLiveWeb.exportCurrentSkiers( "Slalom", SlalomEventData.mySanctionNum, curRound, curEventGroup )
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

		private void loadEventGroupList( int inRound ) {
			isLoadInProg = true;

			if ( EventGroupList.DataSource == null ) {
				if ( SlalomEventData.isCollegiateEvent() ) {
					EventGroupList.DataSource = HelperFunctions.buildEventGroupListNcwsa();
				} else {
					loadEventGroupListFromData( inRound );
				}
			} else {
				if ( ( (ArrayList)EventGroupList.DataSource ).Count > 0 ) {
					if ( SlalomEventData.isCollegiateEvent() ) {
					} else {
						loadEventGroupListFromData( inRound );
					}
				} else {
					if ( SlalomEventData.isCollegiateEvent() ) {
						EventGroupList.DataSource = HelperFunctions.buildEventGroupListNcwsa();
					} else {
						loadEventGroupListFromData( inRound );
					}
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

			EventGroupList.DataSource = HelperFunctions.buildEventGroupList( SlalomEventData.mySanctionNum, "Slalom", inRound );
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

		private void slalomRecapDataGridView_KeyUp( object sender, KeyEventArgs e ) {
			if ( sender == null || ((DataGridView)sender ).CurrentCell == null ) {
				isAddRecapRowInProg = false;
				e.Handled = true;
				return;
			}

			try {
				myRecapColumn = slalomRecapDataGridView.Columns[( (DataGridView)sender ).CurrentCell.ColumnIndex].Name;
				if ( slalomRecapDataGridView != null ) {
					if ( e.KeyCode == Keys.Enter ) {
						if ( isRecapRowEnterHandled || isAddRecapRowInProg ) {
							isAddRecapRowInProg = false;
							isRecapRowEnterHandled = false;

						} else {
							Timer curTimerObj = new Timer();
							curTimerObj.Interval = 15;
							curTimerObj.Tick += new EventHandler( setRecapNextCell );
							curTimerObj.Start();
						}
						e.Handled = true;

					} else if ( e.KeyCode == Keys.Tab ) {
						isAddRecapRowInProg = false;
						if ( myRecapColumn.Equals( "ScoreRecap" ) ) {
							slalomRecapDataGridView.CurrentCell = myRecapRow.Cells[myStartCellIndex];
							this.myOrigCellValue = slalomRecapDataGridView.CurrentCell.Value.ToString();

						} else if ( myRecapColumn.StartsWith( "CellBorder" ) ) {
							this.myOrigCellValue = "";
							if ( isRecapRowEnterHandled || isAddRecapRowInProg ) {
								isAddRecapRowInProg = false;
								isRecapRowEnterHandled = false;
							} else {
								Timer curTimerObj = new Timer();
								curTimerObj.Interval = 15;
								curTimerObj.Tick += new EventHandler( setRecapNextCell );
								curTimerObj.Start();
							}

						} else {
							this.myOrigCellValue = slalomRecapDataGridView.CurrentCell.Value.ToString();
						}
						e.Handled = true;

					} else if ( e.KeyCode == Keys.Escape ) {
						isAddRecapRowInProg = false;
						e.Handled = true;

					} else if ( e.KeyCode == Keys.Delete ) {
						isAddRecapRowInProg = false;
						isDataModified = true;
						myRecapRow.Cells["Updated"].Value = "Y";
						if ( myRecapColumn.StartsWith( "Judge" ) ) {
							myRecapRow.Cells[myRecapColumn].Value = "";
							myRecapRow.Cells["BoatTimeRecap"].Value = "";
							skierPassMsg.Text = "";
							e.Handled = true;

						} else if ( myRecapColumn.Equals( "BoatTimeRecap" ) ) {
							myRecapRow.Cells[myRecapColumn].Value = "";
							skierPassMsg.Text = "";
							e.Handled = true;
						}

					} else {
						isAddRecapRowInProg = false;
					}
				}
			
			} catch ( Exception exp ) {
				MessageBox.Show( "Exception in slalomRecapDataGridView_KeyUp \n" + exp.Message );
			}
		}

		private void setRecapNextCell( object sender, EventArgs e ) {
			Timer curTimerObj = null;
			if ( sender != null ) {
				curTimerObj = (Timer)sender;
				try {
					curTimerObj.Stop();
					curTimerObj.Tick -= new EventHandler( setRecapNextCell );
				} catch ( Exception ex ) {
					MessageBox.Show( "setRecapNextCell: Exception encountered at top of method: " + ex.Message );
				}
			}

			if ( myRecapColumn == null || myRecapRow == null ) return;

			if ( myRecapColumn.Equals( "Judge1ScoreRecap" ) ) {
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["BoatTimeRecap"];

			} else if ( myRecapColumn.Equals( "Judge2ScoreRecap" ) ) {
				if ( myNumJudges > 2 ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge3ScoreRecap"];
				} else {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
				}

			} else if ( myRecapColumn.Equals( "Judge3ScoreRecap" ) ) {
				if ( myNumJudges > 3 ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge4ScoreRecap"];
				} else {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
				}

			} else if ( myRecapColumn.Equals( "Judge4ScoreRecap" ) ) {
				if ( myNumJudges > 4 ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge5ScoreRecap"];
				} else {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
				}

			} else if ( myRecapColumn.Equals( "Judge5ScoreRecap" ) ) {
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];

			} else if ( myRecapColumn.Equals( "BoatTimeRecap" ) ) {
				if ( !( HelperFunctions.isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) )
					&& !( HelperFunctions.isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) )
					&& Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) ) == 6
					&& HelperFunctions.getViewRowColValue(myRecapRow, "TimeInTolRecap", "N" ) == "Y"
					) {
					isRecapRowEnterHandled = true;
					isAddRecapRowInProg = true;
					try {
						curTimerObj = new Timer();
						curTimerObj.Interval = 5;
						curTimerObj.Tick += new EventHandler( addRecapRowTimer );
						curTimerObj.Start();
					} catch ( Exception ex ) {
						MessageBox.Show( "setRecapNextCell: Exception encountered at BoatTimeRecap of method: " + ex.Message );
					}
				} else {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells[myStartCellIndex];
				}

			} else if ( myRecapColumn.Equals( "GateEntry2Recap" ) ) {
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge2ScoreRecap"];

			} else if ( myRecapColumn.Equals( "GateEntry3Recap" ) ) {
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge3ScoreRecap"];

			} else if ( myRecapColumn.Equals( "GateEntry1Recap" ) ) {
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];

			} else if ( myRecapColumn.Equals( "GateExit2Recap" ) ) {
				if ( myNumJudges > 2 ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge3ScoreRecap"];
				} else {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
				}

			} else if ( myRecapColumn.Equals( "GateExit3Recap" ) ) {
				if ( myNumJudges > 3 ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge4ScoreRecap"];
				} else {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
				}

			} else if ( myRecapColumn.Equals( "GateExit1Recap" ) ) {
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["BoatTimeRecap"];

			} else if ( myRecapColumn.StartsWith( "CellBorder" ) ) {
				if ( myRecapColumn.Equals( "CellBorder1" ) ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["GateEntry2Recap"];

				} else if ( myRecapColumn.Equals( "CellBorder2" ) ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["GateEntry3Recap"];

				} else if ( myRecapColumn.Equals( "CellBorder3" ) ) {
					if ( slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge4ScoreRecap"].Visible ) {
						slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge4ScoreRecap"];
					} else {
						slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["GateEntry1Recap"];
					}

				} else if ( myRecapColumn.Equals( "CellBorder4" ) ) {
					if ( slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge5ScoreRecap"].Visible ) {
						slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge5ScoreRecap"];
					} else {
						slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["GateEntry1Recap"];
					}

				} else if ( myRecapColumn.Equals( "CellBorder5" ) ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["GateEntry1Recap"];

				} else if ( myRecapColumn.Equals( "CellBorder6" ) ) {
					slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["BoatTimeRecap"];
				}

			} else {
				slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["BoatTimeRecap"];
			}

			this.myOrigCellValue = slalomRecapDataGridView.CurrentCell.Value.ToString();
		}

		private void TourEventRegDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			DataGridView myDataView = (DataGridView)sender;

			//Update data if changes are detected
			if ( isDataModified && ( myEventRegViewIdx != e.RowIndex ) ) {
				try {
					navSaveItem_Click( null, null );
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}
			if ( isDataModified ) {
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 5;
				curTimerObj.Tick += new EventHandler( TourEventRegDataGridView_RowEnterReset );
				curTimerObj.Start();

			} else {
				boatPathDataGridView.Visible = false;
				InvalidateBoatPathButton.Visible = false;
				BpmsDriverLabel.Visible = false;
				BpmsDriverLabel.Enabled = false;
				BpmsDriver.Visible = false;
				BpmsDriver.Enabled = false;
				BpmsDriver.Text = "";
				myRecapRow = null;
				myScoreRow = null;

				if ( e.RowIndex >= 0 ) {
					if ( myEventRegViewIdx != e.RowIndex ) {
						skierPassMsg.Text = "";
						ActivePassDesc.Text = "";
						myEventRegViewIdx = e.RowIndex;
						int curRowPos = e.RowIndex + 1;
						RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + myDataView.Rows.Count.ToString();
						if ( !( HelperFunctions.isObjectEmpty( myDataView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) ) {
							setSlalomScoreEntry( myDataView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
							setSlalomRecapEntry( myDataView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
							setNewRowPos();
						}
					}
				}
			}
		}

		private void TourEventRegDataGridView_RowEnterReset( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( TourEventRegDataGridView_RowEnterReset );

			TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
		}

		private void roundActiveSelect_Load( object sender, EventArgs e ) {
		}

		private void roundActiveSelect_Click( object sender, EventArgs e ) {
			//Update data if changes are detected
			if ( isDataModified ) {
				try {
					navSaveItem_Click( null, null );
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}

			if ( !( isDataModified ) ) {
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
					myEventRegViewIdx = curSaveEventRegViewIdx;
					if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
						myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
					}
					TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
					setSlalomScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
					setSlalomRecapEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
					setNewRowPos();
				}
			}
		}

		private void EventGroupList_SelectedIndexChanged( object sender, EventArgs e ) {
			if ( isDataModified ) {
				try {
					navSaveItem_Click( null, null );
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}

			if ( !( isDataModified ) ) {
				if ( !( isLoadInProg ) ) {
					int curSaveEventRegViewIdx = myEventRegViewIdx;
					navRefresh_Click( null, null );
					if ( curSaveEventRegViewIdx > 0 ) {
						myEventRegViewIdx = curSaveEventRegViewIdx;
						if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
							myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
						}
						if ( myEventRegViewIdx >= 0 ) {
							TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
							setSlalomScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
							setSlalomRecapEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
							setNewRowPos();
						}
					}
					Int16 curRound = 0;
					try {
						curRound = Convert.ToInt16( roundActiveSelect.RoundValue );
					} catch {
						curRound = 0;
					}
					if ( curRound >= 25 ) {
						MessageBox.Show( "You currently have the runoff round selected.\nChange the round if that is not desired." );
					}
				}
			}
		}

		private void SlalomSpeedSelect_Load( object sender, EventArgs e ) {
			if ( myScoreRow != null ) {
				SlalomSpeedSelection.SelectSpeekKph = (Byte)myScoreRow["StartSpeed"];
			}
		}

		private void SlalomLineSelect_Load( object sender, EventArgs e ) {
			if ( myScoreRow != null ) {
				SlalomLineSelect.CurrentValue = (String)myScoreRow["StartLen"];
			}
		}

		private void SlalomSpeedSelect_Change( object sender, EventArgs e ) {
			if ( sender != null ) {
				String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
				try {
					SlalomSpeedSelection.SelectSpeekKph = Convert.ToInt16( curValue );

					if ( slalomRecapDataGridView.RowCount > 1 ) {
						updateCurrentPassSpeedLine();

					} else if ( slalomRecapDataGridView.RowCount == 0 ) {
						addPassButton.Focus();
					}

				} catch {
					curValue = e.ToString();
				}
			}
		}

		private void SlalomLineSelect_Change( object sender, EventArgs e ) {
			if ( sender != null ) {
				String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
				try {
					SlalomLineSelect.CurrentValue = curValue;
					if ( slalomRecapDataGridView.RowCount > 1 ) {
						updateCurrentPassSpeedLine();

					} else if ( slalomRecapDataGridView.RowCount == 0 ) {
						addPassButton.Focus();
					}

				} catch {
					curValue = e.ToString();
				}
			}
		}

		private void updateCurrentPassSpeedLine() {
			if ( slalomRecapDataGridView.RowCount > 1
				&& myRecapRow.Cells["Updated"].Value.Equals( "N" )
				&& slalomRecapDataGridView.Rows[myRecapRow.Index - 1].Cells["RerideRecap"].Value.ToString().Equals( "Y" )
				) {
				Int16 curPassSpeedKph = SlalomSpeedSelection.SelectSpeekKph;
				Decimal curPassLineLengthMeters = SlalomLineSelect.CurrentValueNum;
				myPassRow = SlalomEventData.getPassRow( curPassSpeedKph, curPassLineLengthMeters );

				mySlalomSetAnalysis.PassTimeRow = mySlalomSetAnalysis.getBoatTimeRow( curPassSpeedKph, 6 );
				loadBoatTimeView( curPassSpeedKph );
				skierPassMsg.ForeColor = Color.DarkBlue;
				skierPassMsg.Text = mySlalomSetAnalysis.getBoatTimeMsg();

				myRecapRow.Cells["PassSpeedKphRecap"].Value = curPassSpeedKph.ToString();
				myRecapRow.Cells["PassLineLengthRecap"].Value = curPassLineLengthMeters.ToString( "00.00" );

				myRecapRow.Cells["NoteRecap"].Value = curPassLineLengthMeters.ToString( "00.00" ) + "M"
					+ "," + (String)myPassRow["SpeedKphDesc"]
					+ "," + ( (String)myPassRow["LineLengthOffDesc"] ).Substring( 0, ( (String)myPassRow["LineLengthOffDesc"] ).IndexOf( " - " ) )
					+ "," + (String)myPassRow["SpeedMphDesc"];

				ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;

				if ( curPassSpeedKph > SlalomSpeedSelection.MaxSpeedKph ) {
					SlalomSpeedSelection.SelectSpeekKph = SlalomSpeedSelection.MaxSpeedKph;

					DataRow curClassRowSkier = SlalomEventData.getSkierClass( HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "EventClass", SlalomEventData.myTourClass ) );
					if ( (Decimal)curClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
						MessageBox.Show( "You have selected a starting speed above the division maximum."
						+ "\n" + "This is allowed although the skier will only be scored at the division maximum speed."
						+ "\n" + "If this was not intended you should delete the pass and re-select the starting speed."
						);

					} else {
						MessageBox.Show( "You have selected a starting speed above the division maximum."
						+ "\n" + "This is allowed for Class C with the scored at the division maximum speed."
						);
					}
				}

				setNewRowPos();

			} else {
				SlalomLineSelect.Enabled = false;
				SlalomSpeedSelection.Enabled = false;
			}
		}

		private void scoreEventClass_SelectedIndexChanged( object sender, EventArgs e ) {
			addPassButton.Focus();
		}

		private void scoreEventClass_Validating( object sender, CancelEventArgs e ) {
			String curMethodName = "Slalom:ScoreEntry:scoreEventClass_Validating";
			int rowsProc = 0;

			ListItem curItem = (ListItem)scoreEventClass.SelectedItem;
			String curEventClass = curItem.ItemValue;
			if ( SlalomEventData.mySkierClassList.compareClassChange( curEventClass, SlalomEventData.myTourClass ) < 0 ) {
				MessageBox.Show( "Class " + curEventClass + " cannot be assigned to a skier in a class " + SlalomEventData.myTourClass + " tournament" );
				e.Cancel = true;
				return;
			}

			DataRow curClassRowSkier = SlalomEventData.getSkierClass( curEventClass );
			if ( (Decimal)curClassRowSkier["ListCodeNum"] >= (Decimal)SlalomEventData.myClassERow["ListCodeNum"] ) {
				bool iwwfMembership = IwwfMembership.validateIwwfMembership(
					SlalomEventData.mySanctionNum, HelperFunctions.getDataRowColValue( SlalomEventData.myTourRow, "SanctionEditCode", "" )
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value
					, HelperFunctions.getDataRowColValue( SlalomEventData.myTourRow, "EventDates", "" ) );
				if ( !( iwwfMembership ) ) {
					curEventClass = "E";
					scoreEventClass.SelectedValue = curEventClass;
				}
			}

			TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value = curEventClass;
			try {
				if ( myScoreRow != null ) {
					String curMemberId = (String)myScoreRow["MemberId"];
					String curAgeGroup = (String)myScoreRow["AgeGroup"];
					byte curRound = (byte)myScoreRow["Round"];
					StringBuilder curSqlStmt = new StringBuilder( "" );
					curSqlStmt = new StringBuilder( String.Format( "Update SlalomScore Set "
						+ "EventClass = '{0}', LastUpdateDate = GETDATE() "
						+ "Where SanctionId = '{1}' AND MemberId = '{2}' AND AgeGroup = '{3}' AND Round = {4}"
						, curEventClass, SlalomEventData.mySanctionNum, curMemberId, curAgeGroup, curRound ).ToString() );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
				}

			} catch ( Exception excp ) {
				String curMsg = ":Error attempting to update skier class \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			}
			isDataModified = true;
		}

		private void slalomRecapDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
			String curColumnName = slalomRecapDataGridView.Columns[e.ColumnIndex].Name;
			if ( !( curColumnName.Equals( "BoatTimeRecap" ) ) ) return;
			if ( myRecapRow == null ) return;

			DataGridViewRow curViewRow = slalomRecapDataGridView.Rows[e.RowIndex];
			if ( myRecapRow.Index == e.RowIndex ) {
				String cellValue = (String)curViewRow.Cells[e.ColumnIndex].Value;
				if ( cellValue.Length == 0 ) checkTimeFromBpms( curViewRow );
			}
		}

		private void slalomRecapDataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {
			if ( myRecapRow == null || e.RowIndex < 0 || e.RowIndex < myRecapRow.Index ) return;
			if ( slalomRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly ) return;

			DataGridViewRow curViewRow = slalomRecapDataGridView.Rows[e.RowIndex];
			String curColumnName = slalomRecapDataGridView.Columns[e.ColumnIndex].Name;
			if ( curColumnName.Equals( "RerideRecap" ) ) {
				if ( curViewRow.Cells[e.ColumnIndex].Value.Equals( "N" ) ) {
					int rowIndex = slalomRecapDataGridView.Rows.Count - 1;
					slalomRecapDataGridView.Select();
					slalomRecapDataGridView.CurrentCell = curViewRow.Cells[myStartCellIndex];
				}

			} else if ( curColumnName.StartsWith( "Gate" ) ) {
				myGateValueChg = true;

			} else if ( curColumnName.StartsWith( "BoatTimeRecap" ) ) {
				myOrigCellValue = (String)curViewRow.Cells[e.ColumnIndex].Value;

			} else if ( curColumnName.StartsWith( "TimeInTolImg" ) ) {
				String cellValue = (String)curViewRow.Cells["BoatTimeRecap"].Value;
				if ( cellValue.Length == 0 ) checkTimeFromBpms( curViewRow );
			}
		}
		
		private void slalomRecapDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			if ( e.RowIndex >= slalomRecapDataGridView.Rows.Count || myRecapRow == null || isLastPassSelectActive ) return;

			DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
			DataGridViewRow curViewRow = slalomRecapDataGridView.Rows[e.RowIndex];
			mySlalomSetAnalysis = new SlalomSetAnalysis( curEventRegRow, slalomRecapDataGridView.Rows, myRecapRow, myNumJudges );

			if ( HelperFunctions.getViewRowColValue( curViewRow, "BoatTimeRecap", "" ).Length > 0 
				&& HelperFunctions.getViewRowColValue( curViewRow, "ScoreRecap", "" ).Length > 0
				) {
				/* 
				 * Rettrieve boat path data
				 */
				Decimal curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( curViewRow, "ScoreRecap", "0" ) );
				loadBoatPathDataGridView( "Slalom"
					, HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "EventClass", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "MemberIdRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "RoundRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "skierPassRecap", "" )
					, curPassScore );

			}
		}

		private void checkTimeFromBpms( DataGridViewRow curViewRow ) {
			String curMethodName = "checkTimeFromBpms: ";
			Decimal curPassScore = mySlalomSetAnalysis.calcScoreForPass();

			if ( curPassScore < 0 ) return;
			curViewRow.Cells["ScoreRecap"].Value = curPassScore.ToString( "0.00" );
			decimal[] curBoatTime = new decimal[] { };

			/* 
			 * Rettrieve boat path data
			 * Rettrieve boat time data
			 */
			try {
				curBoatTime = WscHandler.getBoatTime( "Slalom", (String)curViewRow.Cells["MemberIdRecap"].Value
					, (String)curViewRow.Cells["RoundRecap"].Value, (String)curViewRow.Cells["skierPassRecap"].Value
					, Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value )
					, Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value ), curPassScore );
				
				loadBoatPathDataGridView( "Slalom"
					, TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value.ToString()
					, (String)curViewRow.Cells["MemberIdRecap"].Value
					, (String)curViewRow.Cells["RoundRecap"].Value
					, (String)curViewRow.Cells["skierPassRecap"].Value
					, curPassScore );

				if ( curBoatTime.Length == 0 ) return;

				curViewRow.Cells["BoatTimeRecap"].Value = curBoatTime[0].ToString( "#0.00" );
				boatTimeValidation();
				checkAddNewPass();

			} catch (Exception ex) {
				Log.WriteFile( curMethodName + " :Exception=" + ex.Message );
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
		}

		private void InvalidateBoatPathButton_Click( object sender, EventArgs e ) {
			if ( slalomRecapDataGridView.Rows.Count == 0 ) {
				MessageBox.Show( "No recap row specified" );
				return;
			}
			if ( slalomRecapDataGridView.CurrentCell.RowIndex != ( slalomRecapDataGridView.Rows.Count - 1 ) ) {
				MessageBox.Show( "Only the current pass can be invalidated" );
				return;
			}

			DataGridViewRow curViewRow = slalomRecapDataGridView.Rows[slalomRecapDataGridView.CurrentCell.RowIndex];
			try {
				WscHandler.invalidateBoatPath( "Slalom"
					, HelperFunctions.getViewRowColValue( curViewRow, "MemberIdRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "RoundRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "skierPassRecap", "" ) );
				WscHandler.invalidateBoatTime( "Slalom"
					, HelperFunctions.getViewRowColValue( curViewRow, "MemberIdRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "RoundRecap", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "skierPassRecap", "" ) );
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
		private Decimal loadBoatPathDataGridView( String curEvent, String curSkierClass, String curMemberId, String curRound, String curPassNum, Decimal curPassScore ) {
			winStatusMsg.Text = "Retrieving boat path data";
			boatPathDataGridView.Visible = false;
			InvalidateBoatPathButton.Visible = false;
			InvalidateBoatPathButton.Enabled = false;
			BpmsDriverLabel.Visible = false;
			BpmsDriverLabel.Enabled = false;
			BpmsDriver.Visible = false;
			BpmsDriver.Enabled = false;
			BpmsDriver.Text = "";
			Cursor.Current = Cursors.WaitCursor;

			try {
				boatPathDataGridView.Rows.Clear();
				if ( (myRecapRow == null || slalomRecapDataGridView.CurrentRow == null) && !(curMemberId.Equals("000000000")) ) {
					boatPathDataGridView.Visible = false;
					InvalidateBoatPathButton.Visible = false;
					InvalidateBoatPathButton.Enabled = false;
					BpmsDriverLabel.Visible = false;
					BpmsDriverLabel.Enabled = false;
					BpmsDriver.Visible = false;
					BpmsDriver.Enabled = false;
					BpmsDriver.Text = "";
					return curPassScore;
				}
				Int16 curPassSpeedKph;
				decimal curPassLineLength;
				if ( curMemberId.Equals( "000000000" ) ) {
					curPassLineLength = (decimal)18.25;
					curPassSpeedKph = (Int16)58;
				} else {
					curPassLineLength = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
					curPassSpeedKph = Convert.ToInt16( (String)myRecapRow.Cells["PassSpeedKphRecap"].Value );

				}
				myBoatPathDataRow = WscHandler.getBoatPath( curEvent, curMemberId, curRound, curPassNum, curPassLineLength, curPassSpeedKph );
				if ( myBoatPathDataRow == null ) {
					boatPathDataGridView.Visible = false;
					InvalidateBoatPathButton.Visible = false;
					InvalidateBoatPathButton.Enabled = false;
					BpmsDriverLabel.Visible = false;
					BpmsDriverLabel.Enabled = false;
					BpmsDriver.Visible = false;
					BpmsDriver.Enabled = false;
					BpmsDriver.Text = "";
					return curPassScore;
				}

				bool isRerideReqd = false;
				int curViewIdx = -1;
				while ( curViewIdx < 7 ) {
					curViewIdx = boatPathDataGridView.Rows.Add();
					if ( curPassScore < ( curViewIdx - 1 ) ) break;

					DataGridViewRow curViewRow = boatPathDataGridView.Rows[curViewIdx];
					bool isPassGood = loadBoatPathDataGridRow( curViewRow, curViewIdx, curEvent, curSkierClass, curMemberId, curRound, curPassNum, curPassScore );
					if ( !( isPassGood ) ) isRerideReqd = true;
				}

				if ( curViewIdx < 0 ) return curPassScore;
				boatPathDataGridView.Visible = true;
				InvalidateBoatPathButton.Visible = true;
				InvalidateBoatPathButton.Enabled = true;
				BpmsDriverLabel.Visible = true;
				BpmsDriverLabel.Enabled = true;
				BpmsDriver.Visible = true;
				BpmsDriver.Enabled = true;
				BpmsDriver.Text = HelperFunctions.getDataRowColValue( myBoatPathDataRow, "DriverName", "" );

				if ( !( curMemberId.Equals( "000000000" ) ) ) myRecapRow.Cells["BoatPathGoodRecap"].Value = "Y";
				if ( isRerideReqd && Convert.ToInt32(curPassNum) == slalomRecapDataGridView.RowCount) {
					return checkBoatPathReride( curSkierClass, curPassScore );
				}

				return curPassScore;

			} catch ( Exception ex ) {
				MessageBox.Show( "Error loading boat path view \n" + ex.Message );
				return curPassScore;

			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		/*
		 * All passes (E/L/R) at 11.25 and shorter shall be monitored with applicable buoy and cumulative deviation tolerances applied
		 * 
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
		 */
		private decimal checkBoatPathReride( String curSkierClass, Decimal curPassScore ) {
			Int16 curViewIdx  = 0, curRerideFlag = 0, curRerideOptional = 0, curRerideMandatory = 0;
			decimal curMinRopeLength = 14.25M;
			DataRow curMinRopeLengthRow = SlalomEventData.getSlalomBoatPathRerideRopeMin( curSkierClass );
			if ( curMinRopeLengthRow == null ) return curPassScore;
			curMinRopeLength = HelperFunctions.getDataRowColValueDecimal( curMinRopeLengthRow, "MinRopeLength", 14.25M );

			foreach ( DataGridViewRow curViewRow in boatPathDataGridView.Rows ) {
				curRerideFlag = Convert.ToInt16( HelperFunctions.getViewRowColValue( curViewRow, "boatPathRerideFlag", "0" ) );
				if ( curRerideFlag < 0 && curRerideOptional == 0 ) curRerideOptional = curViewIdx;
				if ( curRerideFlag > 0 && curRerideMandatory == 0 ) curRerideMandatory = curViewIdx;
				curViewIdx++;
			}

			if ( curRerideOptional == 0 && curRerideMandatory == 0 ) return curPassScore;
			
			if ( curRerideMandatory > 0 ) {
				// Mandatory reride indicated based on boat path data
				decimal curPassLineLength = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
				if ( curPassLineLength <= curMinRopeLength ) {
					myRecapRow.Cells["ScoreProtRecap"].Value = "N";
					myRecapRow.Cells["RerideRecap"].Value = "Y";
					myRecapRow.Cells["BoatPathGoodRecap"].Value = "N";

					String curProtectMsg = "Cannot improve score";
					Decimal curScoreProtected = curRerideMandatory - 1;
					if ( curScoreProtected < curPassScore ) {
						curProtectMsg = String.Format( "score {0} protected", curScoreProtected );
						myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
						myRecapRow.Cells["ProtectedScoreRecap"].Value = curScoreProtected.ToString( "#.00" );
					}
					if ( curPassScore < 6 ) curProtectMsg += String.Format( ", can't improve score {0}", curPassScore );
					String errMsg = String.Format( "Mandatory reride based on boat path deviation at buoy {0}, {1}"
						, curRerideMandatory, curProtectMsg );
					myRecapRow.Cells["RerideReasonRecap"].Value = errMsg;
					
					skierPassMsg.ForeColor = Color.Red;
					skierPassMsg.Text = errMsg;
					SlalomScoreCalc( curPassScore );
					return curPassScore;
				}
			}
			
			if ( curPassScore == 6 ) return curPassScore;
			if ( curRerideOptional > 0 ) {
				// Mandatory reride indicated based on boat path data
				String errMsg = String.Format( "Optional reride based on boat path deviation at buoy {0}" +
					", score {1} protected, can improve "
					, curRerideOptional, curPassScore );
				myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
				myRecapRow.Cells["ProtectedScoreRecap"].Value = curPassScore;
				myRecapRow.Cells["RerideRecap"].Value = "Y";
				myRecapRow.Cells["RerideReasonRecap"].Value = errMsg;
				skierPassMsg.ForeColor = Color.Red;
				skierPassMsg.Text = errMsg;
			}
			return curPassScore;
		}

		/*
		 * Retrieve data for current tournament
		 * Used for initial load and to refresh data after updates
		 */
		private bool loadBoatPathDataGridRow( DataGridViewRow curViewRow, int curViewIdx, String curEvent, String curSkierClass
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
				return true ;
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
				if ( curPathBuoyDev > 0 ) {
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

		private void scoreEntryInprogress() {
			SlalomSpeedSelection.Enabled = false;
			SlalomLineSelect.Enabled = false;
			addPassButton.Enabled = false;
		}

		private void scoreEntryBegin() {
			SlalomSpeedSelection.Enabled = true;
			SlalomLineSelect.Enabled = true;
			addPassButton.Enabled = true;
		}

		private void slalomRecapDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
			if ( myWindowFocus ) {
				if ( myRecapRow == null ) return;
				if ( e.RowIndex != myRecapRow.Index ) return;
				if ( slalomRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly ) return;

				myRecapRow.ErrorText = "";
				myModCellValue = "";

				String curColName = slalomRecapDataGridView.Columns[e.ColumnIndex].Name;
				if ( curColName.StartsWith( "Judge" ) ) {
					if ( validateJudgeScoreInput( curColName, (String)e.FormattedValue ) ) return;
					e.Cancel = true;

				} else if ( curColName.StartsWith( "BoatTime" ) && myRecapRow.Cells[e.ColumnIndex].Value != null ) {
					#region Validate boat time
					String curValue = (String)e.FormattedValue;
					if ( HelperFunctions.isObjectEmpty( curValue ) ) {
						myModCellValue = "";
					} else {
						if ( curValue != myOrigCellValue ) {
							if ( curValue.ToUpper().Equals( "OK" ) ) {
								myModCellValue = "00.00";
							} else if ( curValue.ToUpper().Equals( "NONE" ) ) {
								myModCellValue = "-1.00";
							} else {
								try {
									Decimal curNum = Convert.ToDecimal( curValue );
								} catch ( Exception ex ) {
									myRecapRow.ErrorText = "Value in " + curColName + " must be numeric";
									Log.WriteFile( myRecapRow.ErrorText + " :Exception=" + ex.Message );
									e.Cancel = true;
								}
							}
						}
					}
					#endregion
				}
			}
		}

		/* 
		 * Validate any of the judge score cells
		 */
		private bool validateJudgeScoreInput( String curColName, String curJudgeEntry ) {
			if ( HelperFunctions.isObjectEmpty( curJudgeEntry ) ) {
				myModCellValue = "";
				return true;
			}
			if ( curJudgeEntry.Equals( myOrigCellValue ) ) return true;

			if ( curJudgeEntry.IndexOf( '.' ) < 0 ) {
				String modJudgeEntry = "";
				String curLastChar = curJudgeEntry.Substring( curJudgeEntry.Length - 1, 1 ).ToUpper();
				if ( curLastChar.Equals( "Q" ) ) {
					if ( curJudgeEntry.Length > 1 ) {
						modJudgeEntry = curJudgeEntry.Substring( 0, curJudgeEntry.Length - 1 ) + ".25";
					} else {
						modJudgeEntry = ".25";
					}
				} else if ( curLastChar.Equals( "H" ) ) {
					if ( curJudgeEntry.Length > 1 ) {
						modJudgeEntry = curJudgeEntry.Substring( 0, curJudgeEntry.Length - 1 ) + ".5";
					} else {
						modJudgeEntry = ".5";
					}
				} else {
					modJudgeEntry = curJudgeEntry.Substring( 0, 1 ) + "." + curJudgeEntry.Substring( 1 );
				}
				curJudgeEntry = modJudgeEntry;
				myModCellValue = curJudgeEntry;
			}
			try {
				Decimal curJudgeScore = Convert.ToDecimal( curJudgeEntry );
				if ( curJudgeScore > 6 ) {
					myRecapRow.ErrorText = " Value in " + curColName + " must be less than or equal to 6";
					MessageBox.Show( myRecapRow.ErrorText );
					return false;
				}
				Int32 posDelim = curJudgeEntry.IndexOf( '.' );
				if ( posDelim > -1 ) {
					if ( curJudgeEntry.Substring( posDelim ).Equals( ".25" )
						|| curJudgeEntry.Substring( posDelim ).Equals( ".5" )
						|| curJudgeEntry.Substring( posDelim ).Equals( ".50" )
						|| curJudgeEntry.Substring( posDelim ).Equals( ".0" )
						|| curJudgeEntry.Substring( posDelim ).Equals( ".00" )
						|| curJudgeEntry.Substring( posDelim ).Equals( "." )
						) {
						//Value is valid
						myModCellValue = curJudgeScore.ToString( "0.00" );
					} else if ( curJudgeEntry.Substring( posDelim ).Equals( ".75" ) ) {
						if ( SlalomEventData.isCollegiateEvent() ) {
							if ( myRecapRow.Cells["AgeGroupRecap"].Value.Equals( "CM" )
								|| myRecapRow.Cells["AgeGroupRecap"].Value.Equals( "CW" )
								|| myRecapRow.Cells["AgeGroupRecap"].Value.Equals( "BM" )
								|| myRecapRow.Cells["AgeGroupRecap"].Value.Equals( "BW" )
								) {
								//Value is valid
								myModCellValue = curJudgeScore.ToString( "0.00" );
							} else {
								myRecapRow.ErrorText = " Decimal value in " + curColName + " must only contain .0 or .25 or .5 or .75";
								MessageBox.Show( myRecapRow.ErrorText );
								return false;
							}
						} else {
							myRecapRow.ErrorText = " Decimal value in " + curColName + " must only contain .0 or .25 or .5 or .75";
							MessageBox.Show( myRecapRow.ErrorText );
							return false;
						}
					} else {
						myRecapRow.ErrorText = " Decimal value in " + curColName + " must only contain .0 or .25 or .5 or .75";
						MessageBox.Show( myRecapRow.ErrorText );
						return false;
					}
				}
			
			} catch ( Exception ex ) {
				myRecapRow.ErrorText = "Value in " + curColName + " must be numeric (if Q or H must be last characer";
				MessageBox.Show( myRecapRow.ErrorText + " :Exception=" + ex.Message );
				return false;
			}

			return true;
		}

		private void slalomRecapDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
			if ( myWindowFocus ) {
				if ( myRecapRow == null ) return;
				if ( e.RowIndex != myRecapRow.Index ) return;
				if ( slalomRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly ) return;

				String curColName = slalomRecapDataGridView.Columns[e.ColumnIndex].Name;
				if ( curColName.StartsWith( "Judge" ) ) {
					validateJudgeScore( curColName, e.ColumnIndex );

				} else if ( curColName.Equals( "BoatTimeRecap" ) ) {
					#region Validate boat time
					String curInputValue = (String)myRecapRow.Cells[e.ColumnIndex].Value;
					if ( HelperFunctions.isObjectEmpty( curInputValue ) ) {
						if ( HelperFunctions.isObjectEmpty( myOrigCellValue ) ) {
							//No new data input, no action required
						
						} else if ( boatPathDataGridView.Visible ) {
							//Take no action if boat path data is visible

						} else {
							isRecapRowEnterHandled = true;
							isDataModified = true;
							myRecapRow.Cells["Updated"].Value = "Y";
							myRecapRow.Cells["TimeInTolRecap"].Value = "N";
							myRecapRow.Cells["ScoreProtRecap"].Value = "N";
							myRecapRow.Cells["RerideRecap"].Value = "N";
							myRecapRow.Cells["ScoreRecap"].Value = "";
						}

					} else {
						if ( myModCellValue.Length > 0 ) curInputValue = myModCellValue;
						if ( curInputValue != myOrigCellValue ) {
							boatTimeValidation();

							if ( boatPathDataGridView.Visible ) {
								Decimal curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) );
								loadBoatPathDataGridView( "Slalom"
									, TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value.ToString()
									, (String)myRecapRow.Cells["MemberIdRecap"].Value
									, (String)myRecapRow.Cells["RoundRecap"].Value
									, (String)myRecapRow.Cells["skierPassRecap"].Value
									, curPassScore );
							}
							checkAddNewPass();
						}
					}
					#endregion

				} else if ( curColName.StartsWith( "Gate" ) ) {
					#region Validate entrance and exit gate cells
					try {
						if ( myGateValueChg ) {
							Boolean curGateValue = (Boolean)myRecapRow.Cells[curColName].Value;
							//myRecapRow.Cells["ScoreRecap"].Value = "";

							if ( HelperFunctions.isObjectEmpty( myRecapRow.Cells["BoatTimeRecap"].Value ) ) return;

							SlalomTimeValidate();
							if ( ( !( HelperFunctions.isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) ) )
									&& ( !( HelperFunctions.isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) ) ) ) {
								if ( Convert.ToDecimal( myRecapRow.Cells["ScoreRecap"].Value ) == 6 ) {
									if ( (String)myRecapRow.Cells["TimeInTolRecap"].Value == "Y"
										&& (String)myRecapRow.Cells["BoatPathGoodRecap"].Value == "Y"
										) {
										if ( mySlalomSetAnalysis.isExitGatesGood() ) {
											if ( curGateValue ) {
												isAddRecapRowInProg = true;
												Timer curTimerObj = new Timer();
												curTimerObj.Interval = 5;
												curTimerObj.Tick += new EventHandler( addRecapRowTimer );
												curTimerObj.Start();
											}
										} else {
											skierPassMsg.ForeColor = Color.Red;
											skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
											MessageBox.Show( skierPassMsg.Text );
										}
									}
								}
							}
							myGateValueChg = false;
						}

					} catch {
						//This situation only seems to be possible when the window is closing 
						//so just ending method to allow window to close.
						return;
					}
					#endregion

				} else if ( curColName.Equals( "RerideRecap" ) ) {
					#region Validate reride cell check box
					if ( myRecapRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
						if ( HelperFunctions.isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) ) {
							myRecapRow.Cells["ScoreRecap"].Value = "0";
						} else {
						}
						rerideReasonDialogForm.RerideReasonText = (String)myRecapRow.Cells["RerideReasonRecap"].Value;
						if ( rerideReasonDialogForm.ShowDialog() == DialogResult.OK ) {
							setEventRegRowStatus( "InProg" );
							String curCommand = rerideReasonDialogForm.Command;
							myRecapRow.Cells["Updated"].Value = "Y";
							myRecapRow.Cells["RerideReasonRecap"].Value = rerideReasonDialogForm.RerideReasonText;
							if ( curCommand.ToLower().Equals( "updatewithprotect" ) ) {
								myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
								myRecapRow.Cells["ProtectedScoreRecap"].Value = (String)myRecapRow.Cells["ScoreRecap"].Value; ;
							}
							if ( myRecapRow.Index == 0 ) {
								SlalomSpeedSelection.Enabled = true;
								SlalomLineSelect.Enabled = true;
							}

							saveScore();
							Timer curTimerObj = new Timer();
							curTimerObj.Interval = 5;
							curTimerObj.Tick += new EventHandler( addRecapRowTimer );
							curTimerObj.Start();
						} else {
							myRecapRow.Cells["Updated"].Value = "Y";
							rerideReasonDialogForm.RerideReasonText = "";
							MessageBox.Show( "Reride can not be granted without a reason being specified." );
							myRecapRow.Cells["RerideRecap"].Value = "N";
							myRecapRow.Cells["Updated"].Value = "Y";
							saveScore();
						}
					} else {
						myRecapRow.Cells["Updated"].Value = "Y";
						setEventRegRowStatus( myRecapRow );
						saveScore();
					}
					#endregion
				}
			}
		}

		private void boatTimeValidation() {
			if ( !( isJudgeScoresAvailable() ) ) {
				MessageBox.Show( "All required judge scores have not been entered" );
				return;
			}

			isRecapRowEnterHandled = true;
			isDataModified = true;
			myRecapRow.Cells["Updated"].Value = "Y";

			SlalomTimeValidate();
		}

		private bool isJudgeScoresAvailable() {
			if ( myNumJudges == 1 ) return HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( myRecapRow, "Judge1ScoreRecap", "" ) );

			if ( myNumJudges > 1 ) {
				if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "Judge1ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "Judge2ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "Judge3ScoreRecap", "" ) )
					) return false;

			} 
			if ( myNumJudges > 3 ) {
				if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "Judge1ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "Judge2ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "Judge3ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "Judge4ScoreRecap", "" ) )
					|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "Judge5ScoreRecap", "" ) )
					) return false;
			}

			return true;
		}

		/*
		 * Validate any of the judge score cells
		 */
		private void validateJudgeScore( String inColName, int inColIndex ) {
			Decimal curPassScore;
			String curInputValue = (String)myRecapRow.Cells[inColIndex].Value;
			
			if ( HelperFunctions.isObjectEmpty( curInputValue ) ) {
				if ( HelperFunctions.isObjectEmpty( myOrigCellValue ) ) return; //No new data input, no action required

				// Judge score cell input has been deleted
				isDataModified = true;
				myRecapRow.Cells["Updated"].Value = "Y";
				myRecapRow.Cells["ScoreRecap"].Value = "";
				
				curPassScore = mySlalomSetAnalysis.calcScoreForPass();
				if ( curPassScore >= 0 ) myRecapRow.Cells["ScoreRecap"].Value = curPassScore.ToString( "#.00" );
				return;
			}

			if ( SlalomLineSelect.Enabled || SlalomSpeedSelection.Enabled ) {
				SlalomLineSelect.Enabled = false;
				SlalomSpeedSelection.Enabled = false;
			}

			if ( curInputValue.Equals( myOrigCellValue ) ) return; //No new data input, no action required

			isDataModified = true;
			myRecapRow.Cells["Updated"].Value = "Y";
			myRecapRow.Cells["ScoreRecap"].Value = "";
			curPassScore = mySlalomSetAnalysis.calcScoreForPass();
			if ( curPassScore >= 0 ) myRecapRow.Cells["ScoreRecap"].Value = curPassScore.ToString( "#.00" );
			try {
				if ( myModCellValue.Length > 0 ) curInputValue = myModCellValue;
				Decimal curJudgeScore = Convert.ToDecimal( curInputValue );
				myRecapRow.Cells[inColIndex].Value = curJudgeScore.ToString( "#.00" );
				if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( myRecapRow, "BoatTimeRecap", "" ) )
					&& HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "" ) )
					) {
					SlalomTimeValidate();
				}
				myOrigCellValue = (String)myRecapRow.Cells[inColIndex].Value;
			
			} catch ( Exception ex ) {
				myRecapRow.ErrorText = "Value in " + inColName + " must be numeric";
				MessageBox.Show( "Exception: " + ex.Message );
				return;
			}

			if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "TimeInTolRecap", "" ) )
				|| HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "" ) ) ) return;

			curPassScore = Convert.ToDecimal( myRecapRow.Cells["ScoreRecap"].Value );
			if ( curPassScore == 6 ) {
				if ( (String)myRecapRow.Cells["TimeInTolRecap"].Value == "Y"
					&& (String)myRecapRow.Cells["BoatPathGoodRecap"].Value == "Y"
					) {
					if ( mySlalomSetAnalysis.isExitGatesGood() ) {
						isAddRecapRowInProg = true;
						Timer curTimerObj = new Timer();
						curTimerObj.Interval = 5;
						curTimerObj.Tick += new EventHandler( addRecapRowTimer );
						curTimerObj.Start();
					} else {
						skierPassMsg.ForeColor = Color.Red;
						skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
					}
				}
			}
		}

		private void checkAddNewPass() {
			if ( HelperFunctions.isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value )
				|| HelperFunctions.isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) ) return;
			if ( myRecapRow.Cells["BoatTimeRecap"].ReadOnly ) return;

			bool curTimeGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "TimeInTolRecap", "Y" ) );
			bool curRerideReqd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "RerideRecap", "Y" ) );
			bool curScoreProtCkd = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreProtRecap", "Y" ) );
			bool curBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "BoatPathGoodRecap", "Y" ) );
			decimal curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) );

			if ( curTimeGood  && curBoatPathGood ) {
				if ( curPassScore < 6 ) return;

				if ( mySlalomSetAnalysis.isExitGatesGood() ) {
					isAddRecapRowInProg = true;
					Timer curTimerObj = new Timer();
					curTimerObj.Interval = 5;
					curTimerObj.Tick += new EventHandler( addRecapRowTimer );
					curTimerObj.Start();

				} else {
					skierPassMsg.ForeColor = Color.Red;
					skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
				}
				return;
			}

			if ( curScoreProtCkd ) {
				if ( curPassScore == 6 && mySlalomSetAnalysis.isExitGatesGood() ) {
					isAddRecapRowInProg = true;
					Timer curTimerObj = new Timer();
					curTimerObj.Interval = 5;
					curTimerObj.Tick += new EventHandler( addRecapRowTimer );
					curTimerObj.Start();
				}
			}

			if ( curRerideReqd ) {
				isAddRecapRowInProg = true;
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 5;
				curTimerObj.Tick += new EventHandler( addRecapRowTimer );
				curTimerObj.Start();
				return;
			}

			return;
		}

		// Analyze the boat time and fill when determined that short hand entry has been used
		private void SlalomTimeValidate() {

			bool curTimeValid = mySlalomSetAnalysis.SlalomTimeValidate();
			bool curBoatPathGood = HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( myRecapRow, "BoatPathGoodRecap", "Y" ) );

			Int16 curPassSpeedKph = Convert.ToInt16( HelperFunctions.getViewRowColValueDecimal( myRecapRow, "PassSpeedKphRecap", "0" ) );
			loadBoatTimeView( curPassSpeedKph );

			if ( HelperFunctions.isObjectEmpty( mySlalomSetAnalysis.SkierPassMsg ) ) {
				skierPassMsg.ForeColor = Color.DarkBlue;

			} else {
				if ( curTimeValid && curBoatPathGood ) {
					skierPassMsg.ForeColor = Color.DarkBlue;
				} else {
					skierPassMsg.ForeColor = Color.Red;
				}
				skierPassMsg.Text = mySlalomSetAnalysis.SkierPassMsg;
			}

			decimal curPassScore = Convert.ToDecimal( HelperFunctions.getViewRowColValue( myRecapRow, "ScoreRecap", "0" ) );
			SlalomScoreCalc( curPassScore );
		}

		private void SlalomScoreCalc( Decimal inScore ) {
			String curMethodName = "SlalomScoreCalc";

			String curMemberId = (String)myRecapRow.Cells["MemberIdRecap"].Value;
			if ( !( curMemberId.Equals( (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) ) {
				MessageBox.Show( curMethodName + ": Unable to continue processing"
					+ "\n Current id of skier being scored       = " + curMemberId
					+ "\n Current id skier in running order list = " + (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value );
				return;
			}

			try {
				myRecapRow.Cells["ScoreRecap"].Value = inScore.ToString( "#.00" );

				Decimal skierScore = mySlalomSetAnalysis.SlalomScoreCalc( inScore, SlalomSpeedSelection.MaxSpeedKph );
				myRecapRow.Cells["Updated"].Value = "Y";

				if ( HelperFunctions.isObjectPopulated( mySlalomSetAnalysis.SkierPassMsg ) ) skierPassMsg.Text = mySlalomSetAnalysis.SkierPassMsg;
				scoreTextBox.Text = skierScore.ToString();
				TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = skierScore.ToString( "###.00" );
				hcapScoreTextBox.Text = "";
				String curHCapScore = HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "HCapScore", "" );
				if ( HelperFunctions.isObjectPopulated( curHCapScore ) ) {
					hcapScoreTextBox.Text = ( skierScore + Decimal.Parse( curHCapScore )).ToString( "##,###0.0" );
				}

				ScoreList[0].Score = Convert.ToDecimal( skierScore.ToString() );

				String curAgeGroup = HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "AgeGroup", "" );
				appNopsCalc.calcNops( curAgeGroup, ScoreList );
				nopsScoreTextBox.Text = Math.Round( ScoreList[0].Nops, 1 ).ToString();

				isDataModified = true;

				saveScore();

			} catch ( Exception ex ) {
				MessageBox.Show( curMethodName + ": \n" + ex.Message );
			}
		}

		private void ForceCompButton_Click( object sender, EventArgs e ) {
			String curMethodName = "Slalom:ScoreEntry:ForceCompButton_Click";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			if ( TourEventRegDataGridView.Rows.Count > 0 ) {
				if ( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value.Equals( "2-InProg" )
					|| TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value.Equals( "3-Error" )
					|| noteTextBox.Text.Length > 0
					) {

					String myBoatTime = "";
					try {
						myBoatTime = myRecapRow.Cells["BoatTimeRecap"].Value.ToString();
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

									curSqlStmt.Append( "Update SlalomScore Set " );
									curSqlStmt.Append( "Status = '" + curStatus + "'" );
									curSqlStmt.Append( ", Note = '" + curReason + "'" );
									curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
									curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3}"
										, SlalomEventData.mySanctionNum
										, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value
										, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value
										, roundActiveSelect.RoundValue ).ToString() );
									int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
									Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

								} else {
									MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
								}
							}

						} else {
							MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
						}

					} catch ( Exception excp ) {
						String curMsg = ":Error attempting to update skier score status\n" + excp.Message + "\n" + curSqlStmt.ToString();
						MessageBox.Show( curMsg );
						Log.WriteFile( curMethodName + curMsg );
					}

					isDataModified = false;
				}
			}
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
			String curMethodName = "Slalom:ScoreEntry:updateBoatSelect";
			String curMsg = "";

			TourBoatTextbox.Text = buildBoatModelNameDisplay();
			TourBoatTextbox.Focus();
			TourBoatTextbox.Select( 0, 0 );
			String curBoatCode = calcBoatCodeFromDisplay( TourBoatTextbox.Text );

			//if ( myScoreRow != null ) {
			//}
			try {
				StringBuilder curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update SlalomScore Set Boat = '" + curBoatCode + "'" );
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3}"
					, SlalomEventData.mySanctionNum
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value
					, (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value
					, roundActiveSelect.RoundValue ).ToString() );
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

				//String boatId, String boatManufacturer, String boatModel, Int16 boatYear, String boatColor, String boatComment
				if ( WscHandler.isConnectActive ) {
					String curBoatModelName = (String)listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatModelApproved"].Value;
					Int16 curBoatModelYear = Convert.ToInt16( (String)listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["ModelYear"].Value );
					String curBoatNotes = (String)listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatNotes"].Value;
					String curManufacturer = "Unknown";
					if ( curBoatModelName.Contains( "Malibu" ) ) curManufacturer = "Malibu";
					if ( curBoatModelName.Contains( "Nautique" ) ) curManufacturer = "Nautique";
					if ( curBoatModelName.Contains( "Master" ) ) curManufacturer = "Masctercraft";
					WscHandler.sendBoatData( curBoatCode, curManufacturer, curBoatModelName, curBoatModelYear, "Color", curBoatNotes );
				}
				if ( WaterskiConnectLabel.Visible && !WscHandler.isConnectActive ) WaterskiConnectLabel.Visible = false;

			} catch ( Exception excp ) {
				curMsg = ":Error attempting to update boat selection \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			}

			listApprovedBoatsDataGridView.Visible = false;
			approvedBoatSelectGroupBox.Visible = false;
			slalomRecapDataGridView.Focus();
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
					if ( ( (String)curBoatRow.Cells["BoatCode"].Value ).ToUpper().Equals( inBoatCode.ToUpper() ) ) {
						myBoatListIdx = curBoatRow.Index;
						return buildBoatModelNameDisplay();
					}
				}

				myBoatListIdx = 0;
				return "";

			} catch {
				TourBoatTextbox.Text = "";
				foreach ( DataGridViewRow curBoatRow in listApprovedBoatsDataGridView.Rows ) {
					if ( ( (String)curBoatRow.Cells["BoatCode"].Value ).ToUpper().Equals( "UNDEFINED" ) ) {
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
				String curBoatCode = (String)listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatCode"].Value;
				String curBoatModelName = (String)listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatModelApproved"].Value;
				return curBoatModelName + " (KEY: " + curBoatCode + ")";
			}
			return "";
		}

		private void printButton_Click( object sender, EventArgs e ) {
			PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
			PrintDialog curPrintDialog = new PrintDialog();

			myPrintDoc = new PrintDocument();
			curPrintDialog.Document = myPrintDoc;
			CaptureScreen();

			curPrintDialog.AllowCurrentPage = false;
			curPrintDialog.AllowPrintToFile = false;
			curPrintDialog.AllowSelection = false;
			curPrintDialog.AllowSomePages = false;
			curPrintDialog.PrintToFile = false;
			curPrintDialog.ShowHelp = false;
			curPrintDialog.ShowNetwork = false;
			curPrintDialog.UseEXDialog = true;
			curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

			if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
				myPrintDoc.DocumentName = this.Text;
				myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintScreen );
				myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
				myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
				myPrintDoc.DefaultPageSettings.Margins = new Margins( 10, 10, 10, 10 );
				myPrintDoc.DefaultPageSettings.Landscape = true;

				curPreviewDialog.Document = myPrintDoc;
				curPreviewDialog.ShowDialog();
			}
		}

		private void printDoc_PrintScreen( object sender, PrintPageEventArgs e ) {
			int curXPos = 0;
			int curYPos = 25;
			Font fontPrintTitle = new Font( "Arial", 16, FontStyle.Bold );
			Font fontPrintFooter = new Font( "Times New Roman", 10 );

			//display a title
			curXPos = 100;
			e.Graphics.DrawString( myPrintDoc.DocumentName, fontPrintTitle, Brushes.Black, curXPos, curYPos );

			//display form
			curXPos = 50;
			curYPos += 50;
			e.Graphics.DrawImage( memoryImage, curXPos, curYPos );

			curXPos = 50;
			curYPos = curYPos + memoryImage.Height + 25;
			String curFooter = "Date: " + DateTime.Now.ToString( "MM/dd/yyyy hh:mm" );
			e.Graphics.DrawString( curFooter, fontPrintFooter, Brushes.Black, curXPos, curYPos );
		}

		[System.Runtime.InteropServices.DllImport( "gdi32.dll" )]
		public static extern long BitBlt( IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop );
		private Bitmap memoryImage;
		private void CaptureScreen() {
			Graphics mygraphics = this.CreateGraphics();
			Size s = this.Size;
			memoryImage = new Bitmap( s.Width, s.Height, mygraphics );
			Graphics memoryGraphics = Graphics.FromImage( memoryImage );
			IntPtr dc1 = mygraphics.GetHdc();
			IntPtr dc2 = memoryGraphics.GetHdc();
			BitBlt( dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376 );
			mygraphics.ReleaseHdc( dc1 );
			memoryGraphics.ReleaseHdc( dc2 );
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
			PrintDialog curPrintDialog = new PrintDialog();
			DataGridView[] myViewList = new DataGridView[1];
			myViewList[0] = slalomRecapDataGridView;

			bool CenterOnPage = false;
			bool WithTitle = true;
			bool WithPaging = true;
			Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
			Font fontPrintFooter = new Font( "Times New Roman", 10 );

			curPrintDialog.AllowCurrentPage = true;
			curPrintDialog.AllowPrintToFile = false;
			curPrintDialog.AllowSelection = true;
			curPrintDialog.AllowSomePages = true;
			curPrintDialog.PrintToFile = false;
			curPrintDialog.ShowHelp = false;
			curPrintDialog.ShowNetwork = false;
			curPrintDialog.UseEXDialog = true;
			curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

			if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
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


				myGridTitle = new StringRowPrinter( "Slalom Pass Round " + roundActiveSelect.RoundValue,
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

				mySubtitle = new StringRowPrinter( scoreLabel.Text,
					0, 25, 50, scoreLabel.Size.Height,
					Color.Black, Color.White, scoreLabel.Font, SubtitleStringFormatCenter );
				myPrintDataGrid.SubtitleRow = mySubtitle;
				mySubtitle = new StringRowPrinter( scoreTextBox.Text,
					0, 40, 50, scoreTextBox.Size.Height,
					Color.Black, Color.White, scoreTextBox.Font, SubtitleStringFormatCenter );
				myPrintDataGrid.SubtitleRow = mySubtitle;

				mySubtitle = new StringRowPrinter( nopsScoreLabel.Text,
					170, 25, 50, nopsScoreLabel.Size.Height,
					Color.Black, Color.White, nopsScoreLabel.Font, SubtitleStringFormatCenter );
				myPrintDataGrid.SubtitleRow = mySubtitle;
				mySubtitle = new StringRowPrinter( nopsScoreTextBox.Text,
					170, 40, 55, nopsScoreTextBox.Size.Height,
					Color.Black, Color.White, nopsScoreTextBox.Font, SubtitleStringFormatCenter );
				myPrintDataGrid.SubtitleRow = mySubtitle;

				myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
				myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
				myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
				curPreviewDialog.Document = myPrintDoc;
				curPreviewDialog.ShowDialog();
			}
		}

		// The PrintPage action for the PrintDocument control
		private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
			bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
			if ( more == true )
				e.HasMorePages = true;
		}

		private void ScoreEntry_Deactivate( object sender, EventArgs e ) {
			myWindowFocus = false;
		}

		private void ScoreEntry_Activated( object sender, EventArgs e ) {
			myWindowFocus = true;
		}

		private DataTable getTourData( String inSanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, SanctionEditCode, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			return curDataTable;
		}

		private void getApprovedTowboats() {
			int curIdx = 0;
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq, Owner, Notes, HullId, ModelYear, BoatModel " );
			curSqlStmt.Append( "FROM TourBoatUse BU " );
			curSqlStmt.Append( "INNER JOIN CodeValueList BL ON BL.ListCode = BU.HullId " );
			curSqlStmt.Append( "WHERE BL.ListName = 'ApprovedBoats' AND BU.SanctionId = '" + SlalomEventData.mySanctionNum + "' " );
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

			if ( curDataTable.Rows.Count < 3 ) {
				MessageBox.Show( "You have no boats defined for your tournament."
				+ "\nUse the Boat Use window to add boats to your tournament "
				+ "and make them available for event selection" );
			}
		}

		private void getSkierScoreByRound( String inMemberId, String inAgeGroup, int inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT S.PK, S.SanctionId, S.MemberId, S.AgeGroup, S.Round, S.EventClass" );
			curSqlStmt.Append( ", S.Score, S.NopsScore, S.Rating, S.MaxSpeed, S.StartLen, S.StartSpeed, S.Boat, S.Status" );
			curSqlStmt.Append( ", S.FinalSpeedMph, S.FinalSpeedKph, S.FinalLen, S.FinalLenOff, S.FinalPassScore, S.Note, S.LastUpdateDate" );
			curSqlStmt.Append( ", Gender, SkiYearAge " );
			curSqlStmt.Append( "FROM SlalomScore S " );
			curSqlStmt.Append( "     INNER JOIN TourReg T ON S.SanctionId = T.SanctionId AND S.MemberId = T.MemberId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "WHERE S.SanctionId = '" + SlalomEventData.mySanctionNum + "' AND S.MemberId = '" + inMemberId + "'" );
			curSqlStmt.Append( "  AND S.AgeGroup = '" + inAgeGroup + "' AND S.Round = " + inRound.ToString() );
			curSqlStmt.Append( " ORDER BY S.SanctionId, S.MemberId" );
			myScoreDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getSkierRecapByRound( String inMemberId, String inAgeGroup, int inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SR.PK, SanctionId, MemberId, AgeGroup, Round, SkierRunNum, " );
			curSqlStmt.Append( "BoatTime, EntryGate1, EntryGate2, EntryGate3, ExitGate1, ExitGate2, ExitGate3, " );
			curSqlStmt.Append( "Judge1Score, Judge2Score, Judge3Score, Judge4Score, Judge5Score, PassSpeedKph, PassLineLength, " );
			curSqlStmt.Append( "Reride, RerideReason, Score, ProtectedScore, ScoreProt, TimeInTol, BoatPathGood, Note, LastUpdateDate" );
			curSqlStmt.Append( ", (12 - SS.SortSeq + 1) as SlalomSpeedNum, (SL.SortSeq - 1) as SlalomLineNum " );
			curSqlStmt.Append( "FROM SlalomRecap SR" );
			curSqlStmt.Append( "  INNER JOIN CodeValueList as SS ON SS.ListName = 'SlalomSpeeds'  AND SS.MaxValue = PassSpeedKph" );
			curSqlStmt.Append( "  INNER JOIN CodeValueList as SL ON SL.ListName = 'SlalomLines' AND SL.MaxValue = PassLineLength " );
			curSqlStmt.Append( "WHERE SanctionId = '" + SlalomEventData.mySanctionNum + "' AND MemberId = '" + inMemberId + "' " );
			curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() );
			curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, SkierRunNum" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private Boolean checkForSkierRoundScore( String inMemberId, int inRound, String inAgeGroup ) {
			return HelperFunctions.checkForSkierRoundScore( SlalomEventData.mySanctionNum, "Slalom", inMemberId, inRound, inAgeGroup );
		}

		private void getEventRegData( int inRound ) {
			getEventRegData( "All", inRound );
		}
		private void getEventRegData( String inEventGroup, int inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, T.Federation, T.State, T.Gender, E.AgeGroup, E.AgeGroup as Div, O.RunOrder, E.TeamCode" );
			curSqlStmt.Append( ", O.EventGroup, COALESCE(O.RunOrderGroup, '') as RunOrderGroup" );
			curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, COALESCE(O.RankingScore, E.RankingScore) as RankingScore, E.RankingRating, E.HCapBase, E.HCapScore" );
			curSqlStmt.Append( ", COALESCE (S.Status, 'TBD') AS Status, S.Score, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event AND O.Round = " + inRound.ToString() + " " );
			curSqlStmt.Append( "     LEFT OUTER JOIN SlalomScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
			curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
			curSqlStmt.Append( "WHERE E.SanctionId = '" + SlalomEventData.mySanctionNum + "' AND E.Event = 'Slalom' " );

			if ( !( inEventGroup.ToLower().Equals( "all" ) ) ) {
				if ( SlalomEventData.isCollegiateEvent() ) {
					curSqlStmt.Append( HelperFunctions.getEventGroupFilterNcwsaSql( inEventGroup ) );

				} else {
					curSqlStmt.Append( "And (O.EventGroup = '" + inEventGroup + "' AND (O.RunOrderGroup IS NULL OR O.RunOrderGroup = '')" );
					curSqlStmt.Append( "     OR O.EventGroup + '-' + O.RunOrderGroup = '" + inEventGroup + "' ) " );
				}
			}
			myTourEventRegDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			if ( myTourEventRegDataTable.Rows.Count == 0 ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, T.Federation, T.State, T.Gender, E.AgeGroup, E.EventGroup, '' as RunOrderGroup,  E.RunOrder, E.TeamCode" );
				curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, E.RankingScore, E.RankingRating, E.HCapBase, E.HCapScore" );
				curSqlStmt.Append( ", COALESCE (S.Status, 'TBD') AS Status, S.Score, E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt " );
				curSqlStmt.Append( "FROM EventReg E " );
				curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
				curSqlStmt.Append( "     LEFT OUTER JOIN SlalomScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
				curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
				curSqlStmt.Append( "WHERE E.SanctionId = '" + SlalomEventData.mySanctionNum + "' AND E.Event = 'Slalom' " );

				if ( !( inEventGroup.ToLower().Equals( "all" ) ) ) {
					if ( SlalomEventData.isCollegiateEvent() ) {
						curSqlStmt.Append( HelperFunctions.getEventGroupFilterNcwsaSql( inEventGroup ) );

					} else {
						curSqlStmt.Append( "And E.EventGroup = '" + inEventGroup + "' " );
					}
				}
				myTourEventRegDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			}
		}

		private int cleanSkierSlalomScore(String inMemberId, String inAgeGroup, int inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "Delete SlalomScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}' AND MemberId = '{2}' AND MemberId = '{3}'"
				, SlalomEventData.mySanctionNum, inMemberId, inAgeGroup, inRound ) );
			return DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

		private bool checkLoadOfficialAssgmt( String inAgeGroup, String inEventGroup ) {
			myCheckOfficials.readOfficialAssignments( SlalomEventData.mySanctionNum, "Slalom", inAgeGroup, inEventGroup, roundActiveSelect.RoundValue );
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

		private Boolean isScoreRowExist( String curSanctionId, String curMemberId, String curAgeGroup, byte curRound ) {
			StringBuilder curSqlStmt = new StringBuilder( String.Format( "Select PK From SlalomScore Where SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3}"
				, curSanctionId, curMemberId, curAgeGroup, curRound ).ToString() );
			DataTable curDataTAble = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTAble.Rows.Count > 0 ) return true;
			return false;
		}

		private Boolean isRecapRowExist( String curSanctionId, String curMemberId, String curAgeGroup, byte curRound, int curSkierRunNum ) {
			StringBuilder curSqlStmt = new StringBuilder(
				String.Format( "Select PK From SlalomRecap Where SanctionId = '{0}' AND MemberId = '{1}' AND AgeGroup = '{2}' AND Round = {3} AND SkierRunNum = {4}"
				, curSanctionId, curMemberId, curAgeGroup, curRound, curSkierRunNum ).ToString() );
			DataTable curDataTAble = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTAble.Rows.Count > 0 ) return true;
			return false;
		}

		private Boolean isRunOrderByRound( int inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select count(*) as SkierCount From EventRunOrder " );
			curSqlStmt.Append( "WHERE SanctionId = '" + SlalomEventData.mySanctionNum + "' AND Event = 'Slalom' AND Round = " + inRound );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( (int)curDataTable.Rows[0]["SkierCount"] > 0 ) {
				return true;
			} else {
				return false;
			}
		}

		private String getSortCommand() {
			String curSortCommand = myTourProperties.RunningOrderSortSlalom;
			if ( isOrderByRoundActive ) curSortCommand = myTourProperties.RunningOrderSortSlalomRound;

			int curDelim = mySortCommand.IndexOf( "AgeGroup" );
			if ( curDelim == 0 ) {
				curSortCommand = "DivOrder" + mySortCommand.Substring( "AgeGroup".Length );
				if ( isOrderByRoundActive ) {
					myTourProperties.RunningOrderSortSlalomRound = curSortCommand;
				} else {
					myTourProperties.RunningOrderSortSlalom = curSortCommand;
				}

			} else if ( curDelim > 0 ) {
				curSortCommand = mySortCommand.Substring( 0, curDelim ) + "DivOrder" + mySortCommand.Substring( curDelim + "AgeGroup".Length );
				if ( isOrderByRoundActive ) {
					myTourProperties.RunningOrderSortSlalomRound = curSortCommand;
				} else {
					myTourProperties.RunningOrderSortSlalom = curSortCommand;
				}
			}
			return curSortCommand;
		}

		private void driverDropdown_SelectedValueChanged( object sender, EventArgs e ) {
			if ( isLoadInProg ) return;
			if ( ( (ComboBox)sender ).Items.Count == 0 ) return;
			if ( ((ComboBox)sender ).SelectedValue == null ) return;
			String curSelectedValue = ( (ComboBox)sender ).SelectedValue.ToString();
			if ( curSelectedValue.Length == 0 ) return;
			myDriverMemberId = curSelectedValue;
		}

	}
}
