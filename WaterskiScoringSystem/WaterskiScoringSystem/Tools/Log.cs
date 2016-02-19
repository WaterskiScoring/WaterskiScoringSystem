using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {

    class Log {
        private static readonly Log myInstance = new Log();
        private static StreamWriter myLogOutBuffer = null;
        private static String myLogFileName = "-log.log";

        private Log() {
            // Load data rating values
        }

        public static Log getInstance {
            get {
                return myInstance;
            }
        }

        public static bool WriteFile( String[] inLogRecords ) {
            return WriteFile( inLogRecords, null );
        }
        public static bool WriteFile( String[] inLogRecords, String inSanctionId ) {
            bool curFileOpen = false;
            bool curReturnStatus = false;

            try {
                if ( inLogRecords.Length > 0 ) {
                    if ( myLogOutBuffer == null ) {
                        curFileOpen = OpenFile( inSanctionId );
                        curReturnStatus = curFileOpen;
                    }
                    if ( curReturnStatus ) {
                        for ( int curIdx = 0; curIdx < inLogRecords.Length; curIdx++ ) {
                            curReturnStatus = WriteFile( inLogRecords[curIdx] );
                        }
                        if ( curFileOpen ) {
                            CloseFile();
                        }
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered writing to log file: " + "\n\nError: " + ex.Message );
                curReturnStatus = false;
            }
            return curReturnStatus;
        }
        public static bool WriteFile( String inLogRecord ) {
            try {
                if ( myLogOutBuffer == null ) {
                    return false;
                } else {
                    string DateString = DateTime.Now.ToString("MMM dd yyyy hh:mm:ss tt");
                    myLogOutBuffer.WriteLine( DateString + ": " + inLogRecord );
                    return true;
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered writing to log file: " + "\n\nError: " + ex.Message );
                return false;
            }
        }

        public static bool OpenFile( String inFileName ) {
            try {
                if ( myLogOutBuffer == null ) {
                    String curFilename = "";
                    if ( inFileName == null ) {
                        curFilename = Properties.Settings.Default.AppSanctionNum.Trim() + myLogFileName;
                    } else {
                        curFilename = inFileName;
                    }

                    myLogOutBuffer = null;
                    myLogOutBuffer = getFile( curFilename );
                    return true;
                } else {
                    return false;
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered opening log file: " + "\n\nError: " + ex.Message );
                return false;
            }
        }

        public static bool CloseFile() {
            try {

                if (myLogOutBuffer != null) {
                    myLogOutBuffer.Close();
                    myLogOutBuffer = null;
                    return true;
                } else {
                    return true;
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered closing log file: " + "\n\nError: " + ex.Message );
                return false;
            }
        }

        private static StreamWriter getFile( String inFileName ) {
            String curFullFileName = "";
            StreamWriter outBuffer = null;

            String curPath = Properties.Settings.Default.ExportDirectory;
            if ( inFileName.IndexOf( "\\" ) > -1 ) {
                curFullFileName = inFileName;
            } else {
                if ( curPath.EndsWith( "\\" ) ) {
                    curFullFileName = curPath + inFileName;
                } else {
                    curFullFileName = curPath + "\\" + inFileName;
                }
            }
            try {
                if ( File.Exists( curFullFileName ) ) {
                    outBuffer = File.AppendText( curFullFileName );
                } else {
                    outBuffer = File.CreateText( curFullFileName );
                }
            } catch ( Exception ex ) {
                outBuffer = null;
                MessageBox.Show( "Exception encountered opening existing log file for extending: " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

        public static Boolean isDirectoryValid( String inLocName ) {
            try {
                if ( Directory.Exists( inLocName ) ) {
                    return true;
                } else {
                    return false;
                }
            } catch {
                return false;
            }
        }

    }
}
