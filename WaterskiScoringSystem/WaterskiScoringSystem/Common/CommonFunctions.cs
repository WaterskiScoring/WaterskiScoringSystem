using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
	class CommonFunctions {

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
			ArrayList curEventGroupList = new ArrayList();
			curEventGroupList.Add( "All" );
			curEventGroupList.Add( "Men A" );
			curEventGroupList.Add( "Women A" );
			curEventGroupList.Add( "Men B" );
			curEventGroupList.Add( "Women B" );
			curEventGroupList.Add( "Non Team" );
			return curEventGroupList;
		}

		public static ArrayList buildEventGroupList( String inSanctionNum, String inEvent, int inRound) {
			ArrayList curEventGroupList = new ArrayList();
			curEventGroupList.Add( "All" );
			String curSqlStmt = String.Format( "SELECT DISTINCT EventGroup From EventRunOrder WHERE SanctionId = '{0}' And Event = '{1}' And Round = {2} Order by EventGroup", inSanctionNum, inEvent, inRound );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt );
			if ( curDataTable.Rows.Count == 0 ) {
				curSqlStmt = String.Format( "SELECT DISTINCT EventGroup From EventReg WHERE SanctionId = '{0}' And Event = '{1}' Order by EventGroup", inSanctionNum, inEvent, inRound );
				curDataTable = DataAccess.getDataTable( curSqlStmt );
			}

			foreach ( DataRow curRow in curDataTable.Rows ) {
				curEventGroupList.Add( (String)curRow["EventGroup"] );
			}
			return curEventGroupList;
		}

		public static Boolean isGroupNcwsa( String inGroupValue ) {
			return ( inGroupValue.ToLower().Equals( "cm" )
				|| inGroupValue.ToLower().Equals( "cw" )
				|| inGroupValue.ToLower().Equals( "bm" )
				|| inGroupValue.ToLower().Equals( "bw" ) );
		}

		public static bool isObjectEmpty( object inObject ) {
			bool curReturnValue = false;
			if ( inObject == null ) {
				curReturnValue = true;
			} else if ( inObject == System.DBNull.Value ) {
				curReturnValue = true;
			} else if ( inObject.ToString().Length > 0 ) {
				curReturnValue = false;
			} else {
				curReturnValue = true;
			}
			return curReturnValue;
		}

		public static String getDataRowColValueDecimal( DataRow dataRow, String colName, String defaultValue, int numDecimals ) {
			try {
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 0 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 1 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.0" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) && numDecimals == 2 ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );

				return ( (String)dataRow[colName] ).ToString();
			} catch {
				return "";
			}
		}

		public static String getDataRowColValue( DataRow dataRow, String colName, String defaultValue ) {
			try {
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( String ) ) ) return ( (String)dataRow[colName] ).ToString().Trim();
				if ( dataRow[colName].GetType().Equals( typeof( int ) ) ) return ( (int)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( Int16 ) ) ) return ( (Int16)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( byte ) ) ) return ( (byte)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( bool ) ) ) return ( (bool)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return ( (decimal)dataRow[colName] ).ToString( "##,###0.00" );

				return ( (String)dataRow[colName] ).ToString();
			
			} catch {
				return defaultValue;
			}
		}
		public static String getViewRowColValue( DataGridViewRow viewRow, String colName, String defaultValue ) {
			String returnValue = defaultValue;
			try {
				if ( viewRow.Cells[colName].Value == null ) return defaultValue;
				if ( viewRow.Cells[colName].Value.GetType().Equals( typeof(String) ) ) returnValue = ((String)viewRow.Cells[colName].Value).Trim();
				if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( int ) ) ) returnValue = ( (int)viewRow.Cells[colName].Value ).ToString();
				if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( Int16 ) ) ) returnValue = ( (Int16)viewRow.Cells[colName].Value ).ToString();
				if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( byte ) ) ) returnValue = ( (byte)viewRow.Cells[colName].Value ).ToString();
				if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( decimal ) ) ) returnValue = ( (decimal)viewRow.Cells[colName].Value ).ToString( "##,###0.00" );
				if ( viewRow.Cells[colName].Value.GetType().Equals( typeof( bool ) ) ) returnValue = ( (bool)viewRow.Cells[colName].Value ).ToString();

				if ( returnValue.Length < 1 ) return defaultValue;
				return returnValue;

			} catch {
				return defaultValue;
			}
		}
	}
}
