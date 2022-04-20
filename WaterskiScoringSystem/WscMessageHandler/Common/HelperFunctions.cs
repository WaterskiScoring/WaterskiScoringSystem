using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WscMessageHandler.Common {
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

		public static void cleanMsgQueues() {
			StringBuilder curSqlStmt = new StringBuilder( "Delete From WscMsgSend Where SanctionId = '" + Properties.Settings.Default.SanctionNum + "' " );
			DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			
			curSqlStmt = new StringBuilder( "Delete From WscMsgListen Where SanctionId = '" + Properties.Settings.Default.SanctionNum + "' " );
			DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			curSqlStmt = new StringBuilder( "Delete From WscMonitor Where SanctionId = '" + Properties.Settings.Default.SanctionNum + "' " );
			DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

		public static void addMsgSendQueue( String msgType, String msgData ) {
			String curMsgData = stringReplace( msgData, singleQuoteDelim, "''" );

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Insert WscMsgSend ( " );
			curSqlStmt.Append( "SanctionId, MsgType, MsgData, CreateDate " );
			curSqlStmt.Append( ") Values ( " );
			curSqlStmt.Append( "'" + Properties.Settings.Default.SanctionNum + "'" );
			curSqlStmt.Append( ", '" + msgType + "'" );
			curSqlStmt.Append( ", '" + curMsgData + "'" );
			curSqlStmt.Append( ", getdate()" );
			curSqlStmt.Append( ")" );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

		public static void addMsgListenQueue( String msgType, String msgData ) {
			String curMsgData = stringReplace( msgData, singleQuoteDelim, "''" );

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Insert WscMsgListen ( " );
			curSqlStmt.Append( "SanctionId, MsgType, MsgData, CreateDate " );
			curSqlStmt.Append( ") Values ( " );
			curSqlStmt.Append( "'" + Properties.Settings.Default.SanctionNum + "'" );
			curSqlStmt.Append( ", '" + msgType + "'" );
			curSqlStmt.Append( ", '" + curMsgData + "'" );
			curSqlStmt.Append( ", getdate()" );
			curSqlStmt.Append( ")" );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

		public static void updateMonitorHeartBeat( String monitorName ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Update WscMonitor " );
			curSqlStmt.Append( "Set HeartBeat = getdate() " );
			curSqlStmt.Append( "Where SanctionId = '" + Properties.Settings.Default.SanctionNum + "' " );
			curSqlStmt.Append( "And MonitorName = '" + monitorName + "'" );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			if ( rowsProc == 0 ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert WscMonitor ( " );
				curSqlStmt.Append( "SanctionId, MonitorName, HeartBeat " );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + Properties.Settings.Default.SanctionNum + "'" );
				curSqlStmt.Append( ", '" + monitorName + "'" );
				curSqlStmt.Append( ", getdate()" );
				curSqlStmt.Append( ")" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}
		}

		public static void deleteMonitorHeartBeat( String monitorName ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Delete From WscMonitor " );
			curSqlStmt.Append( "Where SanctionId = '" + Properties.Settings.Default.SanctionNum + "' " );
			curSqlStmt.Append( "And MonitorName = '" + monitorName + "'" );
			DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

		public static DataRow getMonitorHeartBeat( String monitorName ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MonitorName,HeartBeat " );
			curSqlStmt.Append( "FROM WscMonitor " );
			curSqlStmt.Append( "WHERE SanctionId = '" + Properties.Settings.Default.SanctionNum + "' " );
			curSqlStmt.Append( "And MonitorName = '" + monitorName + "'" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable != null && curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];
			return null;
		}

		public static DataTable getMonitorHeartBeatAll() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MonitorName,HeartBeat " );
			curSqlStmt.Append( "FROM WscMonitor " );
			curSqlStmt.Append( "WHERE SanctionId = '" + Properties.Settings.Default.SanctionNum + "' " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
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
	}
}
