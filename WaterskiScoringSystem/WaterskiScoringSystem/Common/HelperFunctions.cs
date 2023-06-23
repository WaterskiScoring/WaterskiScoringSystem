using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Common {
	class HelperFunctions {
		public static char[] TabDelim = new char[] { '\t' };
		public static char[] SingleQuoteDelim = new char[] { '\'' };
		public static String TabChar = "\t";

		public static String getDatabaseFilenameFromConnectString() {
			String curDatabaseFilename = "";
			String curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;

			String[] curAttrEntry;
			String[] curConnAttrList = curAppConnectString.Split( ';' );
			for ( int idx = 0; idx < curConnAttrList.Length; idx++ ) {
				curAttrEntry = curConnAttrList[idx].Split( '=' );
				if ( curAttrEntry[0].ToLower().Trim().Equals( "data source" ) ) {
					if ( curAttrEntry[1].StartsWith( "|DataDirectory|\\" ) ) {
						curDatabaseFilename = AppDomain.CurrentDomain.GetData( "DataDirectory" ) + "\\" + curAttrEntry[1].Substring( 16 );
					} else {
						curDatabaseFilename = curAttrEntry[1];
					}
					break;
				}
			}
			return curDatabaseFilename;
		}

		public static String getEventGroupFilterNcwsa( String inGroupValue ) {
			if ( inGroupValue.ToUpper().Equals( "MEN A" ) ) return "AgeGroup = 'CM' ";
			if ( inGroupValue.ToUpper().Equals( "WOMEN A" ) ) return "AgeGroup = 'CW' ";
			if ( inGroupValue.ToUpper().Equals( "MEN B" ) ) return "AgeGroup = 'BM' ";
			if ( inGroupValue.ToUpper().Equals( "WOMEN B" ) ) return "AgeGroup = 'BW' ";
			if ( inGroupValue.ToUpper().Equals( "ALL" ) ) return "";
			return "AgeGroup not in ('CM', 'CW', 'BM', 'BW') ";
		}

		public static String getEventGroupFilterNcwsaSql( String inGroupValue ) {
			if ( inGroupValue.ToUpper().Equals( "MEN A" ) ) return "And E.AgeGroup = 'CM' ";
			if ( inGroupValue.ToUpper().Equals( "WOMEN A" ) ) return "And E.AgeGroup = 'CW' ";
			if ( inGroupValue.ToUpper().Equals( "MEN B" ) ) return "And E.AgeGroup = 'BM' ";
			if ( inGroupValue.ToUpper().Equals( "WOMEN B" ) ) return "And E.AgeGroup = 'BW' ";
			if ( inGroupValue.ToUpper().Equals( "ALL" ) ) return "";
			return "And E.AgeGroup not in ('CM', 'CW', 'BM', 'BW') ";
		}

		public static ArrayList buildEventGroupListNcwsa() {
			return new ArrayList() { "All", "Men A", "Women A", "Men B", "Women B", "Non Team" };
		}

		public static ArrayList buildEventGroupList( String inSanctionNum, String inEvent, int inRound) {
			ArrayList curEventGroupList = new ArrayList();
			curEventGroupList.Add( "All" );
			String curSqlStmt = String.Format( "SELECT DISTINCT EventGroup, COALESCE(RunOrderGroup, '') as RunOrderGroup From EventRunOrder WHERE SanctionId = '{0}' And Event = '{1}' And Round = {2} Order by EventGroup, COALESCE(RunOrderGroup, '')", inSanctionNum, inEvent, inRound );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt );
			if ( curDataTable.Rows.Count > 0 ) {
				foreach ( DataRow curRow in curDataTable.Rows ) {
					if ( isObjectPopulated( (String)curRow["RunOrderGroup"] ) ) {
						curEventGroupList.Add( (String)curRow["EventGroup"] + "-" + (String)curRow["RunOrderGroup"] );
					} else {
						curEventGroupList.Add( (String)curRow["EventGroup"] );
					}
				}
			
			} else { 
				curSqlStmt = String.Format( "SELECT DISTINCT EventGroup From EventReg WHERE SanctionId = '{0}' And Event = '{1}' Order by EventGroup", inSanctionNum, inEvent, inRound );
				curDataTable = DataAccess.getDataTable( curSqlStmt );
				foreach ( DataRow curRow in curDataTable.Rows ) {
					curEventGroupList.Add( (String)curRow["EventGroup"] );
				}
			}

			return curEventGroupList;
		}

		public static String buildPublishReportTitle( String inSanctionNum ) {
			DataRow curRow = HelperFunctions.getTourChiefScorer( inSanctionNum );
			return "\nOfficial Results certified by Chief Scorer " + HelperFunctions.getDataRowColValue( curRow, "ChiefScorerName", "N/A" );
		}

		public static bool checkForSkierRoundScore( String inSanctionId, String inEvent, String inMemberId, int inRound, String inAgeGroup ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round " );
			curSqlStmt.Append( "FROM " + inEvent + "Score " );
			curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' " );
			curSqlStmt.Append( " AND MemberId = '" + inMemberId + "' " );
			curSqlStmt.Append( " AND Round = " + inRound + " " );
			if ( inSanctionId.EndsWith( "999" ) || inSanctionId.EndsWith( "998" ) ) {
				curSqlStmt.Append( " AND AgeGroup = '" + inAgeGroup + "' " );
			}
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return true;
			} else {
				return false;
			}
		}

		public static DataRow getTourChiefScorer( String inSanctionNum ) {
			DataRow curRow = null;
			try {
				//Retrieve selected tournament attributes
				StringBuilder curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT T.ChiefScorerMemberId, TourRegCC.SkierName AS ChiefScorerName, T.ChiefScorerAddress, T.ChiefScorerPhone, T.ChiefScorerEmail " );
				curSqlStmt.Append( "FROM Tournament T " );
				curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCC ON T.SanctionId = TourRegCC.SanctionId AND T.ChiefScorerMemberId = TourRegCC.MemberId " );
				curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionNum + "' " );
				curSqlStmt.Append( "ORDER BY T.SanctionId " );

				DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count == 0 ) return null;

				curRow = curDataTable.Rows[0];
				return curRow;

			} catch ( Exception ex ) {
				MessageBox.Show( "Exception retrieving tournament data " + "\n\nError: " + ex.Message );
				return null;
			}
		}

		public static Boolean isGroupNcwsa( String inGroupValue ) {
			return ( inGroupValue.ToLower().Equals( "cm" )
				|| inGroupValue.ToLower().Equals( "cw" )
				|| inGroupValue.ToLower().Equals( "bm" )
				|| inGroupValue.ToLower().Equals( "bw" ) );
		}

		public static bool isObjectEmpty( object inObject ) {
			if ( inObject == null ) return true;
			else if ( inObject == System.DBNull.Value ) return true;
			else if ( inObject.ToString().Length > 0 ) return false;
			return true;
		}
		public static bool isObjectPopulated( object inObject ) {
			if ( inObject == null ) return false;
			else if ( inObject == System.DBNull.Value ) return false;
			else if ( inObject.ToString().Length > 0 ) return true;
			return false;
		}

		public static bool isValueTrue( String inValue ) {
			String checkValue = inValue.Trim().ToLower();
			if ( checkValue.Equals( "true" ) ) return true;
			else if ( checkValue.Equals( "false" ) ) return false;
			else if ( checkValue.Equals( "y" ) ) return true;
			else if ( checkValue.Equals( "n" ) ) return false;
			else if ( checkValue.Equals( "yes" ) ) return true;
			else if ( checkValue.Equals( "no" ) ) return false;
			else if ( checkValue.Equals( "1" ) ) return true;
			else if ( checkValue.Equals( "0" ) ) return false;
			else return false;
		}

		public static bool isCollegiateEvent( String inTourRules ) {
			if ( inTourRules.ToLower().Equals( "ncwsa" ) ) return true;
			return false;
		}
		public static bool isIwwfEvent( String inTourRules ) {
			if ( inTourRules.ToLower().Equals( "iwwf" ) ) return true;
			return false;
		}

		public static String stringReplace( String inValue, char[] inCurValue, String inReplValue ) {
			StringBuilder curNewValue = new StringBuilder( "" );

			String[] curValues = inValue.Split( inCurValue );
			if ( curValues.Length > 1 ) {
				int curCount = 0;
				foreach ( String curValue in curValues ) {
					curCount++;
					if ( curCount < curValues.Length ) {
						curNewValue.Append( curValue + inReplValue );
					} else {
						curNewValue.Append( curValue );
					}
				}
			
			} else {
				curNewValue.Append( inValue );
			}

			return curNewValue.ToString();
		}

		public static string[] cleanInputColName( string[] inputColNames ) {
			string[] returnColNames = new string[inputColNames.Length];
			int curListIdx = 0;
			StringBuilder newColName = new StringBuilder( "" );
			
			foreach ( string curColName in inputColNames ) {
				newColName = new StringBuilder( "" );
				for (int curIdx = 0; curIdx < curColName.Length; curIdx++ ) {
					string curChar = curColName.Substring( curIdx, 1 );
					if ( curChar != " " ) newColName.Append( curChar );
				}
				returnColNames[curListIdx] = newColName.ToString();
				curListIdx++;
			}
			
			return returnColNames;
		}

		public static String escapeString( String inValue ) {
			String curReturnValue = "";
			char[] singleQuoteDelim = new char[] { '\'' };
			curReturnValue = HelperFunctions.stringReplace( inValue, singleQuoteDelim, "''" );
			return curReturnValue;
		}

		public static String getDataRowColValueDecimal( DataRow dataRow, String colName, String defaultValue, int numDecimals ) {
			try {
				if ( dataRow == null ) return defaultValue;
				if ( !( dataRow.Table.Columns.Contains( colName ) ) ) return defaultValue;
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 0 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 1 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.0" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 2 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == -1 ) return ( (decimal)dataRow[colName] ).ToString( "##,####.#" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == -2 ) return ( (decimal)dataRow[colName] ).ToString( "##,####.##" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );
				return getDataRowColValue( dataRow, colName, defaultValue );
			
			} catch {
				return "";
			}
		}
		public static decimal getDataRowColValueDecimal( DataRow dataRow, String colName, decimal defaultValue ) {
			try {
				if ( dataRow == null ) return defaultValue;
				if ( !( dataRow.Table.Columns.Contains( colName ) ) ) return defaultValue;
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return (decimal)dataRow[colName];
				return Convert.ToDecimal( getDataRowColValue(dataRow, colName, defaultValue.ToString( "##,###0.00" ) ) );
			
			} catch {
				return defaultValue;
			}
		}

		public static String getDataRowColValue( DataRow dataRow, String colName, String defaultValue ) {
			try {
				if ( dataRow == null ) return defaultValue;
				if ( !( dataRow.Table.Columns.Contains( colName ) ) ) return defaultValue;
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( String ) ) ) return ( (String)dataRow[colName] ).ToString().Trim();
				if ( dataRow[colName].GetType().Equals( typeof( int ) ) ) return ( (int)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( Int16 ) ) ) return ( (Int16)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( Int64 ) ) ) return ( (Int64)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( byte ) ) ) return ( (byte)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( bool ) ) ) return ( (bool)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );
				if ( dataRow[colName].GetType().Equals( typeof( DateTime ) ) ) return ( (DateTime)dataRow[colName] ).ToString( "yyyy/MM/dd HH:mm:ss" );

				return ( (String)dataRow[colName] ).ToString();
			
			} catch {
				return defaultValue;
			}
		}
		
		public static String getViewRowColValue( DataGridViewRow viewRow, String colName, String defaultValue ) {
			String curColValue = "";
			try {
				if ( viewRow == null ) return defaultValue;
				if ( !(viewRow.DataGridView.Columns.Contains(colName)) ) return defaultValue;
				if ( viewRow.Cells[colName].Value == null ) return defaultValue;
				
				if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( String ) ) ) {
					curColValue = ( (String)viewRow.Cells[colName].Value ).Trim();
				
				} else if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( int ) ) ) {
					curColValue = ( (int)viewRow.Cells[colName].Value ).ToString();

				} else if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( Int16 ) ) ) {
					curColValue = ( (Int16)viewRow.Cells[colName].Value ).ToString();

				} else if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( Int64 ) ) ) {
					curColValue = ( (Int64)viewRow.Cells[colName].Value ).ToString();

				} else if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( byte ) ) ) {
					curColValue = ( (byte)viewRow.Cells[colName].Value ).ToString();

				} else if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( decimal ) ) ) {
					curColValue = ( (decimal)viewRow.Cells[colName].Value ).ToString( "##,###0.00" );
				
				} else if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( bool ) ) ) {
					curColValue = ( (bool)viewRow.Cells[colName].Value ).ToString();
				}
				
				if ( curColValue.Length <= 0 ) return defaultValue;
				return curColValue;
			
			} catch {
				return defaultValue;
			}
		}

		public static decimal getViewRowColValueDecimal( DataGridViewRow viewRow, String colName, String defaultValue ) {
			try {
				return Convert.ToDecimal( getViewRowColValue( viewRow, colName, defaultValue ) );
			
			} catch {
				return Convert.ToDecimal( defaultValue );
			}
		}

		public static String getRegion( String inSanctionId ) {
			String curValue = inSanctionId.Substring( 2, 1 );
			String returnValue = curValue;

			if ( curValue.ToUpper().Equals( "E" ) ) {
				returnValue = "EAST";
			} else if ( curValue.ToUpper().Equals( "W" ) ) {
				returnValue = "WEST";
			} else if ( curValue.ToUpper().Equals( "S" ) ) {
				returnValue = "SOUTH";
			} else if ( curValue.ToUpper().Equals( "C" ) ) {
				returnValue = "SOUTHCENTRAL";
			} else if ( curValue.ToUpper().Equals( "M" ) ) {
				returnValue = "MIDWEST";
			} else {
				returnValue = curValue;
			}
			return returnValue;
		}

		public static Dictionary<string, object> getAttributeDictionary( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return null;
			if ( msgAttributeList[keyName] == null ) return null;
			return (Dictionary<string, object>)msgAttributeList[keyName];
		}

		public static ArrayList getAttributeList( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return null;
			if ( msgAttributeList[keyName] == null ) return null;
			return (ArrayList)msgAttributeList[keyName];
		}

		public static decimal getAttributeValueNum( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return 0;
			if ( msgAttributeList[keyName] == null ) return 0;

			if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Int32" ) ) {
				if ( Decimal.TryParse( ( (int)msgAttributeList[keyName] ).ToString(), out decimal returnValue ) ) {
					return returnValue;
				}

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Decimal" ) ) {
				if ( Decimal.TryParse( ( (decimal)msgAttributeList[keyName] ).ToString(), out decimal returnValue ) ) {
					return returnValue;
				}

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.String" ) ) {
				if ( Decimal.TryParse( (String)msgAttributeList[keyName], out decimal returnValue ) ) {
					return returnValue;
				}
			}

			return 0;
		}

		public static String getAttributeValue( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !msgAttributeList.ContainsKey( keyName ) ) return "";
			if ( msgAttributeList[keyName] == null ) return "";

			if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Int32" ) ) {
				return ( (int)msgAttributeList[keyName] ).ToString();

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Decimal" ) ) {
				return ( (decimal)msgAttributeList[keyName] ).ToString();

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.String" ) ) {
				return ( (String)msgAttributeList[keyName] );

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Boolean" ) ) {
				return ( (Boolean)msgAttributeList[keyName] ).ToString();
			}

			return "";
		}

		public static bool isColumnObsolete( string colName, string tablename ) {
			foreach( DatabaseUpgradeRemovedColumn curEntry in UpgradeDatabase.removedColumns ) {
				if ( curEntry.Equals( colName ) && curEntry.TableName.Equals( tablename ) ) return true;
			}
			return false;
		}

		public static StreamWriter getExportFile() {
			return getExportFile( null );
		}
		public static StreamWriter getExportFile( String inFileFilter ) {
			return getExportFile( inFileFilter, null );
		}
		public static StreamWriter getExportFile( String inFileFilter, String inFileName ) {
			String curMethodName = "getExportFile";
			String returnFileName;
			StreamWriter outBuffer = null;
			String curFileFilter = "";
			if ( inFileFilter == null ) {
				curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			} else {
				curFileFilter = inFileFilter;
			}

			SaveFileDialog myFileDialog = new SaveFileDialog();
			String curPath = Properties.Settings.Default.ExportDirectory;
			myFileDialog.InitialDirectory = curPath;
			myFileDialog.Filter = curFileFilter;
			myFileDialog.FilterIndex = 1;
			if ( inFileName == null ) {
				myFileDialog.FileName = "";
			} else {
				myFileDialog.FileName = inFileName;
			}

			try {
				if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
					returnFileName = myFileDialog.FileName;
					if ( returnFileName != null ) {
						int delimPos = returnFileName.LastIndexOf( '\\' );
						String curFileName = returnFileName.Substring( delimPos + 1 );


						if ( curFileName.IndexOf( '.' ) < 0 ) {
							String curDefaultExt = ".txt";
							String[] curList = curFileFilter.Split( '|' );
							if ( curList.Length > 0 ) {
								int curDelim = curList[1].IndexOf( "." );
								if ( curDelim > 0 ) {
									curDefaultExt = curList[1].Substring( curDelim - 1 );
								}
							}
							returnFileName += curDefaultExt;
						}
						outBuffer = File.CreateText( returnFileName );
					}
				}
			
			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
				String curMsg = curMethodName + ":Exception=" + ex.Message;
				Log.WriteFile( curMsg );
			}

			return outBuffer;
		}

		public static StreamWriter getExportFileByName( String inFileName ) {
			String curMethodName = "getExportFileByName";
			StreamWriter outBuffer = null;

			SaveFileDialog myFileDialog = new SaveFileDialog();
			String curPath = Properties.Settings.Default.ExportDirectory;
			myFileDialog.InitialDirectory = curPath;
			myFileDialog.FileName = inFileName;

			try {
				if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
					String myFileName = myFileDialog.FileName;
					if ( myFileName != null ) {
						int delimPos = myFileName.LastIndexOf( '\\' );
						String curFileName = myFileName.Substring( delimPos + 1 );
						if ( curFileName.IndexOf( '.' ) < 0 ) {
							myFileName += ".wsp";
						}
						outBuffer = File.CreateText( myFileName );
					}
				}
			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
				String curMsg = curMethodName + ":Exception=" + ex.Message;
				Log.WriteFile( curMsg );
			}

			return outBuffer;
		}

		public static void updateEventRegSkierClass( String inSanctionId, String inTourRules, String inTourEditCode, String inTourEventDate
			, String inEvent, String inSkierGroup, String inSkierClassOld, String inSkierClassNew
			, ListSkierClass inSkierClassList ) {
			
			String curMethodName = "HelperFunctions: updateEventRegSkierClass: ";
			int rowsProc = 0, curRowsUpdated = 0;
			DataRow curSkierClassRow;
			DataRow curClassERow = inSkierClassList.SkierClassDataTable.Select( "ListCode = 'E'" )[0];
			StringBuilder curSqlStmt = new StringBuilder( "" );
			
			curSqlStmt.Append( "Select PK, MemberId From EventReg " );
			curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' " );
			if ( !inEvent.ToLower().Equals( "all" ) ) curSqlStmt.Append( "And Event = '" + inEvent + "' " );
			if ( !inSkierClassOld.ToLower().Equals( "all" ) ) curSqlStmt.Append( "And EventClass = '" + inSkierClassOld + "' " );
			if ( !inSkierGroup.ToLower().Equals( "all" ) ) curSqlStmt.Append( "And EventGroup = '" + inSkierGroup + "' " );

			String curSkierClass = "";
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			foreach ( DataRow curRow in curDataTable.Rows ) {
				curSkierClass = inSkierClassNew.ToUpper();
				curSkierClassRow = inSkierClassList.SkierClassDataTable.Select( "ListCode = '" + curSkierClass + "'" )[0];
				if ( (Decimal)curSkierClassRow["ListCodeNum"] > (Decimal)curClassERow["ListCodeNum"] || inTourRules.ToUpper().Equals( "IWWF" ) ) {
					if ( !( IwwfMembership.validateIwwfMembership( inSanctionId, inTourEditCode, (String)curRow["MemberId"], inTourEventDate ) ) ) {
						curSkierClass = "E";
					}
				}

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update EventReg Set " );
				curSqlStmt.Append( "EventClass = '" + curSkierClass + "', LastUpdateDate = GETDATE() " );
				curSqlStmt.Append( "Where PK = " + HelperFunctions.getDataRowColValue( curRow, "PK", "0" ) );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				curRowsUpdated += rowsProc;
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
			}

			MessageBox.Show( "Total skiers updated for class change: " + curRowsUpdated );

		}

		public static String removeInvalidAttr( String inSortCmd, String inAttrName ) {
			String curReturnValue = "";

			int curDelim = inSortCmd.IndexOf( inAttrName );
			if ( curDelim < 0 ) {
				curReturnValue = inSortCmd;
			} else {
				String tmpSortCmd = "";
				int curDelimComma = inSortCmd.IndexOf( ",", curDelim );
				if ( curDelimComma > 0 ) {
					tmpSortCmd = inSortCmd.Substring( 0, curDelim ) + inSortCmd.Substring( curDelimComma + 2 );
				} else {
					tmpSortCmd = inSortCmd.Substring( 0, curDelim - 2 );
				}
				curReturnValue = tmpSortCmd;
			}

			return curReturnValue;
		}

		public static String replaceAttr( String inSortCmd, String inAttrName, String inReplaceValue ) {
			String curReturnValue = "";

			int curDelim = inSortCmd.IndexOf( inAttrName );
			if ( curDelim < 0 ) {
				curReturnValue = inSortCmd;
			} else if ( curDelim > 0 ) {
				curReturnValue = inSortCmd.Substring( 0, curDelim ) + inReplaceValue + inSortCmd.Substring( curDelim + inAttrName.Length );
			} else {
				curReturnValue = inReplaceValue + inSortCmd.Substring( inAttrName.Length );
			}

			return curReturnValue;
		}

	}
}
