using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Windows.Forms;

namespace WscMessageHandler.Common {
	class HandlerDataAccess {
		private static int DataAccessOpenCount = 0;
		private static int DataAccessEmbeddedTransactionCount = 0;

		private static SqlCeConnection DataAccessConnection = null;
		private static SqlCeTransaction DataAccessTransaction = null;
		private static SqlCeDataAdapter DataAccessDataAdapter = null;
		private static SqlCeCommand DataAccessCommand = null;

		public HandlerDataAccess() {
		}

		public static bool DataAccessOpen() {
			String curMethodName = "DataAccess:DataAccessOpen";
			// Open database connection if needed
			try {
				if ( DataAccessConnection == null ) {
					DataAccessOpenCount = 1;
					DataAccessConnection = new SqlCeConnection( global::WscMessageHandler.Properties.Settings.Default.DatabaseConnectionString );
					DataAccessConnection.Open();

				} else {
					DataAccessOpenCount++;
				}
				return true;
			
			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + ":Exception Performing SQL operation: " + ex.Message );
				MessageBox.Show( curMethodName + "\nException Performing SQL operation" + "\n\nError: " + ex.Message );
				return false;
			}
		}

		public static bool DataAccessClose() {
			return DataAccessClose( false );
		}
		public static bool DataAccessClose( bool inCloseAll ) {
			String curMethodName = "DataAccess:DataAccessClose";
			try {
				if ( DataAccessConnection == null ) {
					DataAccessOpenCount = 0;
				} else {
					if ( inCloseAll ) {
						DataAccessOpenCount = 0;
						DataAccessConnection.Close();
						DataAccessConnection.Dispose();
						DataAccessConnection = null;
						DataAccessDataAdapter = null;
						DataAccessTransaction = null;
						DataAccessCommand = null;
					
					} else {
						if ( DataAccessOpenCount > 1 ) {
							DataAccessOpenCount--;
						
						} else {
							DataAccessOpenCount = 0;
							DataAccessConnection.Close();
							DataAccessConnection.Dispose();
							DataAccessConnection = null;
							DataAccessDataAdapter = null;
							DataAccessTransaction = null;
							DataAccessCommand = null;
						}
					}
				}
				
				return true;
			
			} catch ( Exception ex ) {
				MessageBox.Show( curMethodName + ":Error:\n\n" + ex.Message );
				Log.WriteFile( curMethodName + ":Exception:" + ex.Message );
				return false;
			}
		}

		public static DataTable getDataTable( String inSelectStmt ) {
			return getDataTable( inSelectStmt, true );
		}
		public static DataTable getDataTable( String inSelectStmt, bool inShowMsg ) {
			String curMethodName = "DataAccess:getDataTable";
			DataTable curDataTable = new DataTable();

			try {
				// Open connection if needed
				if ( DataAccessConnection == null ) {
					DataAccessOpen();
				}
				// Create data adapter
				DataAccessDataAdapter = new SqlCeDataAdapter( inSelectStmt, DataAccessConnection );
				DataAccessDataAdapter.Fill( curDataTable );

			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { ExcpMsg += "\nSQL=" + inSelectStmt; }
				Log.WriteFile( curMethodName + ":Exception Performing SQL operation:" + ExcpMsg );
				if ( inShowMsg ) {
					MessageBox.Show( curMethodName + "\nException Performing SQL operation" + "\n\nError: " + ExcpMsg );
				}
				return null;
			}

			return curDataTable;
		}

		public static int ExecuteCommand( String inSelectStmt ) {
			String curMethodName = "DataAccess:ExecuteCommand";
			int curReturnValue = 0;

			try {
				// Open connection if needed
				if ( DataAccessConnection == null ) {
					DataAccessOpen();
				}
				// Create data adapter
				DataAccessCommand = new SqlCeCommand( inSelectStmt, DataAccessConnection );
				curReturnValue = DataAccessCommand.ExecuteNonQuery();

			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { ExcpMsg += "\nSQL=" + inSelectStmt; }
				Log.WriteFile( curMethodName + ":Exception Performing SQL operation:" + ExcpMsg );
				MessageBox.Show( curMethodName + "\nException Performing SQL operation" + "\n\nError: " + ExcpMsg );
				curReturnValue = -1;
			}

			return curReturnValue;
		}

		public static Object ExecuteScalarCommand( String inSelectStmt ) {
			String curMethodName = "DataAccess:ExecuteScalarCommand";
			Object curReturnValue = null;

			try {
				// Open connection if needed
				if ( DataAccessConnection == null ) {
					DataAccessOpen();
				}
				// Create data adapter
				DataAccessCommand = new SqlCeCommand( inSelectStmt, DataAccessConnection );
				curReturnValue = DataAccessCommand.ExecuteScalar();

			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { ExcpMsg += "\nSQL=" + inSelectStmt; }
				Log.WriteFile( curMethodName + ":Exception Performing SQL operation:" + ExcpMsg );
				MessageBox.Show( curMethodName + "\nException Performing SQL operation" + "\n\nError: " + ExcpMsg );
				curReturnValue = -1;
			}

			return curReturnValue;
		}

		public static Boolean BeginTransaction( String inSelectStmt ) {
			String curMethodName = "DataAccess:BeginTransaction";
			Boolean curReturnValue = false;

			try {
				// Open connection if needed
				if ( DataAccessConnection == null ) {
					DataAccessOpen();
				}
				// Create transaction if needed
				if ( DataAccessTransaction == null ) {
					DataAccessTransaction = DataAccessConnection.BeginTransaction();
				}
				DataAccessEmbeddedTransactionCount++;
				curReturnValue = true;

			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { ExcpMsg += "\nSQL=" + inSelectStmt; }
				Log.WriteFile( curMethodName + ":Exception Performing SQL operation:" + ExcpMsg );
				MessageBox.Show( curMethodName + "\nException Performing SQL operation" + "\n\nError: " + ExcpMsg );
				curReturnValue = false;
			}

			return curReturnValue;
		}

		public static Boolean CommitTransaction( String inSelectStmt ) {
			String curMethodName = "DataAccess:CommitTransaction";
			Boolean curReturnValue = false;

			try {
				if ( DataAccessTransaction == null ) {
					Log.WriteFile( curMethodName + ":No active transaction, Commit bypassed" );
					MessageBox.Show( curMethodName + ":No active transaction, Commit bypassed" );
					curReturnValue = false;
					DataAccessEmbeddedTransactionCount = 0;
				} else {
					// if we have more than 1 in the count, then we have embedded Transactions and should not commit yet
					if ( DataAccessEmbeddedTransactionCount > 1 ) {
						DataAccessEmbeddedTransactionCount--;
					} else {
						DataAccessTransaction.Commit();
						DataAccessEmbeddedTransactionCount = 0;
						DataAccessTransaction = null;
					}

				}
			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { ExcpMsg += "\nSQL=" + inSelectStmt; }
				Log.WriteFile( curMethodName + ":Exception Performing SQL operation:" + ExcpMsg );
				MessageBox.Show( curMethodName + "\nException Performing SQL operation" + "\n\nError: " + ExcpMsg );
				DataAccessEmbeddedTransactionCount = 0;
				DataAccessTransaction = null;
				curReturnValue = false;
			}

			return curReturnValue;
		}

		public static Boolean RollbackTransaction( String inSelectStmt ) {
			String curMethodName = "DataAccess:RollbackTransaction";
			Boolean curReturnValue = false;

			try {
				if ( DataAccessTransaction == null ) {
					Log.WriteFile( curMethodName + ":No active transaction.  Unable to Rollback Transaction" );
					MessageBox.Show( curMethodName + ":No active transaction.  Unable to Rollback Transaction" );
					curReturnValue = false;
					DataAccessEmbeddedTransactionCount = 0;
				} else {
					// if we have more than 1 in the count, then we have embedded Transactions and should not commit yet
					if ( DataAccessEmbeddedTransactionCount > 1 ) {
						DataAccessEmbeddedTransactionCount--;
					} else {
						DataAccessTransaction.Rollback();
						DataAccessEmbeddedTransactionCount = 0;
						DataAccessTransaction = null;
					}

				}
			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( inSelectStmt != null ) { ExcpMsg += "\nSQL=" + inSelectStmt; }
				Log.WriteFile( curMethodName + ":Exception Performing SQL operation:" + ExcpMsg );
				MessageBox.Show( curMethodName + "\nException Performing SQL operation" + "\n\nError: " + ExcpMsg );
				DataAccessEmbeddedTransactionCount = 0;
				DataAccessTransaction = null;
				curReturnValue = false;
			}

			return curReturnValue;
		}

	}
}
