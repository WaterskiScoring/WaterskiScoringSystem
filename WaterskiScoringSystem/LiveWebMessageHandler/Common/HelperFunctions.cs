using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LiveWebMessageHandler.Common {
	public class HelperFunctions {

		public static char[] singleQuoteDelim = new char[] { '\'' };

		public static DataRow getTourData() {
			return getTourData( Properties.Settings.Default.SanctionNum );
		}
		public static DataRow getTourData( String sanctionNum ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation, TourDataLoc" );
			curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + sanctionNum + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable != null && curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];
			return null;
		}
		
		public static String getDataRowColValue( DataRow dataRow, String colName, String defaultValue ) {
			try {
				if ( dataRow == null ) return defaultValue;
				if ( dataRow[colName] == System.DBNull.Value ) return defaultValue;
				if ( dataRow[colName].GetType().Equals( typeof( String ) ) ) return ( (String)dataRow[colName] ).ToString().Trim();
				if ( dataRow[colName].GetType().Equals( typeof( int ) ) ) return ( (int)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( Int16 ) ) ) return ( (Int16)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( byte ) ) ) return ( (byte)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( bool ) ) ) return ( (bool)dataRow[colName] ).ToString();
				if ( dataRow[colName].GetType().Equals( typeof( decimal ) ) ) return ( (decimal)dataRow[colName] ).ToString( "#####0.00" );
				if ( dataRow[colName].GetType().Equals( typeof( DateTime ) ) ) return ( (DateTime)dataRow[colName] ).ToString( "yyyy/MM/dd HH:mm:ss" );

				return ( (String)dataRow[colName] ).ToString();

			} catch {
				return defaultValue;
			}
		}

		public static Dictionary<string, object> getAttributeList( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return null;
			return (Dictionary<string, object>)msgAttributeList[keyName];
		}

		public static decimal getAttributeValueNum( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return 0;

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
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return "";

			if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Int32" ) ) {
				return ( (int)msgAttributeList[keyName] ).ToString();

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Decimal" ) ) {
				return ( (decimal)msgAttributeList[keyName] ).ToString();

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.String" ) ) {
				return ( (String)msgAttributeList[keyName] ).Trim();
			}

			return "";
		}

		public static String getEventGroupFilterSql( String inGroupValue, bool isTrickVideo ) {
			return getEventGroupFilterSql( inGroupValue, isTrickVideo, true );
		}
		public static String getEventGroupFilterSql( String inGroupValue, bool isTrickVideo, bool isOrderByRound ) {
			if ( isObjectEmpty( inGroupValue ) ) return "";
			if ( inGroupValue.ToUpper().Equals( "MEN A" ) ) return "And ER.AgeGroup = 'CM' ";
			if ( inGroupValue.ToUpper().Equals( "WOMEN A" ) ) return "And ER.AgeGroup = 'CW' ";
			if ( inGroupValue.ToUpper().Equals( "MEN B" ) ) return "And ER.AgeGroup = 'BM' ";
			if ( inGroupValue.ToUpper().Equals( "WOMEN B" ) ) return "And ER.AgeGroup = 'BW' ";
			if ( inGroupValue.ToUpper().Equals( "ALL" ) ) return "";
			if ( inGroupValue.ToUpper().Equals( "NON TEAM" ) ) return "AND ER.AgeGroup not in ('CM', 'CW', 'BM', 'BW') ";
			if ( isTrickVideo ) return "And ER.AgeGroup = '" + inGroupValue + "' ";
			if ( isOrderByRound ) return "And COALESCE( O.EventGroup +'-' + O.RunOrderGroup, ER.EventGroup) = '" + inGroupValue + "' ";
			return "And ER.EventGroup = '" + inGroupValue + "' ";
			
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

		public static String stripLineFeedChar( String inValue ) {
			String curValue = inValue;
			curValue = curValue.Replace( '\n', ' ' );
			curValue = curValue.Replace( '\r', ' ' );
			curValue = curValue.Replace( '\t', ' ' );
			return curValue;
		}
	}
}
