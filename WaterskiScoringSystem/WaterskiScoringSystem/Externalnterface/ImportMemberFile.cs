using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Windows.Forms;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Externalnterface {

	class ImportMemberFile {
		private string mySanctionNum;
		private bool isTourNcwsa = false;
		private ProgressWindow myProgressInfo;
		private ImportMember myImportMember;

		public ImportMemberFile() {
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) mySanctionNum = "";
			else if ( mySanctionNum.Length < 6 ) mySanctionNum = "";

			if ( HelperFunctions.isCollegiateSanction( mySanctionNum ) )  isTourNcwsa = true;

			myImportMember = new ImportMember( null );
		}
		
		public void importData() {
			ArrayList curFileList = new ArrayList();
			curFileList = getImportFileList();

			if ( curFileList.Count == 0 ) return;

			myProgressInfo = new ProgressWindow();
			foreach ( String curFileName in curFileList ) {
				importDataFile( curFileName );
			}
			myProgressInfo.Close();
		}

		private void importDataFile( string curFileName ) {
			string inputBuffer;
			string[] inputCols = null, inputColNames = null;
			int curInputLineCount = 0;
			StreamReader curReader = null;
			
			curReader = getImportFile( curFileName );
			if ( curReader == null ) return;

			inputBuffer = curReader.ReadLine();
			if ( inputBuffer == null ) return;
			inputColNames = HelperFunctions.cleanInputColName( inputBuffer.Split( HelperFunctions.TabDelim ) );

			curInputLineCount = 0;
			while ( ( inputBuffer = curReader.ReadLine() ) != null ) {
				curInputLineCount++;
				myProgressInfo.setProgressValue( curInputLineCount );

				inputCols = inputBuffer.Split( HelperFunctions.TabDelim );

				Dictionary<string, dynamic> curImportMemberEntry = new Dictionary<string, dynamic>();
				int idx = 0;
				foreach(string colName in inputColNames ) {
					curImportMemberEntry.Add( colName, inputCols[idx] );
					idx++;
					if ( idx > inputCols.Length ) break;
				}

				myImportMember.importMemberFromAwsa( curImportMemberEntry, true, isTourNcwsa );

			}
		}

		private ArrayList getImportFileList() {
			ArrayList curFileNameList = new ArrayList();
			String curPath = "";
			OpenFileDialog myFileDialog = new OpenFileDialog();
			try {
				curPath = Properties.Settings.Default.ExportDirectory;
				if ( curPath.Length < 2 ) {
					curPath = Directory.GetCurrentDirectory();
				}
				myFileDialog.InitialDirectory = curPath;
				myFileDialog.Multiselect = true;
				myFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
				myFileDialog.FilterIndex = 2;
				if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
					String[] curFileNames = myFileDialog.FileNames;
					foreach ( String curFileName in curFileNames ) {
						curFileNameList.Add( curFileName );
					}
				}
			
			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not get an import file to process " + "\n\nError: " + ex.Message );
			}

			return curFileNameList;
		}

		private StreamReader getImportFile( String curFileName ) {
			StreamReader curReader = null;

			try {
				myProgressInfo.setProgessMsg( "File selected " + curFileName );
				myProgressInfo.Show();
				myProgressInfo.Refresh();

				string inputBuffer = "";
				int curInputLineCount = 0;
				curReader = new StreamReader( curFileName );
				while ( ( inputBuffer = curReader.ReadLine() ) != null ) {
					curInputLineCount++;
				}
				curReader.Close();
				curReader = null;
				myProgressInfo.setProgressMin( 1 );
				myProgressInfo.setProgressMax( curInputLineCount );

				return new StreamReader( curFileName );

			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not read file" + curFileName + "\n\nError: " + ex.Message );
				return null;

			} finally {
				if ( curReader != null ) curReader.Close();
			}
		}

	}
}
