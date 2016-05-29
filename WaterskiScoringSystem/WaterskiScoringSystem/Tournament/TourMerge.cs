using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using System.Reflection;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Tournament;

namespace WaterskiScoringSystem.Tournament {
    class TourMerge {
        private ProgressWindow myProgressInfo;

        public TourMerge() {
        }

        public Boolean mergeTourFiles(String inSanctionId) {
            Boolean returnValue = true;
            String curAdminSanctionId = "";
            TourMergeSelect mergeDialogForm = new TourMergeSelect();
            mergeDialogForm.ShowDialog();

            // Determine if the OK button was clicked on the dialog box.
            if ( mergeDialogForm.DialogResult == DialogResult.OK ) {
                curAdminSanctionId = mergeDialogForm.SanctionNumToMerge;
            } else {
                return false;
            }

            DataRow curTourRow = getTourData(inSanctionId);

            DataRow curTourAdminRow = getTourData(curAdminSanctionId);

            if ( curTourRow == null || curTourAdminRow == null ) {
                returnValue = false;
            } else {
                String curTourRules = (String) curTourRow["Rules"];
                String curTourClass = (String) curTourRow["Class"];
                String curTourDataLoc = (String) curTourRow["TourDataLoc"];
                ArrayList curFileFilterList = getEndOfTourReportList(inSanctionId, curTourClass);

                String curAdminTourClass = (String) curTourAdminRow["Class"];
                String curAdminTourDataLoc = (String) curTourAdminRow["TourDataLoc"];
                ArrayList curAdminFileFilterList = getEndOfTourReportList(curAdminSanctionId, curAdminTourClass);

                int curCountComplete = 0, curCountFailed = 0;
                for(int curIdx = 0; curIdx < curFileFilterList.Count; curIdx++ ) {
                    Boolean results = mergeTourFile(inSanctionId, curAdminSanctionId, (String) curFileFilterList[curIdx], (String) curAdminFileFilterList[curIdx], curTourDataLoc, curAdminTourDataLoc);
                    if ( results ) {
                        curCountComplete++;
                    } else {
                        curCountFailed++;
                        MessageBox.Show(String.Format("Errors encountered merging {0} to {1}", (String) curAdminFileFilterList[curIdx], (String) curFileFilterList[curIdx]));
                    }
                }

                Cursor.Current = Cursors.WaitCursor;
                ZipUtil.ZipFiles(curTourDataLoc, inSanctionId + curTourClass + ".zip", curFileFilterList);

                MessageBox.Show(String.Format(" Files successfully merged: {0} \n File merges failed: {1}", curCountComplete, curCountFailed));
            }

            return returnValue;
        }

        private Boolean mergeTourFile( String curSanctionId, String curAdminSanctionId, String curTourFile, String curAdminTourFile, String curTourDataLoc, String curAdminTourDataLoc ) {
            Boolean returnValue = true;

            if ( curTourFile.ToLower().EndsWith(".tny")
                || curTourFile.ToLower().EndsWith(".log")
                || curTourFile.ToLower().EndsWith("hd.txt")
                || curTourFile.ToLower().EndsWith(".log")
                || curTourFile.ToLower().EndsWith("cj.txt")
                || curTourFile.ToLower().EndsWith("sd.txt")
                ) {
                return true;

            } else if ( curTourFile.ToLower().EndsWith("cj.txt")
                || curTourFile.ToLower().EndsWith("tu.txt")
                || curTourFile.ToLower().EndsWith("jt.csv")
                || curTourFile.ToLower().EndsWith("st.csv")
                ) {
                return mergeFile(curSanctionId, curAdminSanctionId, curTourFile, curAdminTourFile, curTourDataLoc, curAdminTourDataLoc, true, true);

            } else if ( curTourFile.ToLower().EndsWith("tu.prn")
                || curTourFile.ToLower().EndsWith("ts.prn")
                || curTourFile.ToLower().EndsWith("bt.prn")
                || curTourFile.ToLower().EndsWith("cj.prn")
                || curTourFile.ToLower().EndsWith("sd.prn")
                || curTourFile.ToLower().EndsWith("ts.txt")
                ) {
                return mergeFile(curSanctionId, curAdminSanctionId, curTourFile, curAdminTourFile, curTourDataLoc, curAdminTourDataLoc, false, false);

            } else if ( curTourFile.ToLower().EndsWith(".wsp") ) {
                return mergeFile(curSanctionId, curAdminSanctionId, curTourFile, curAdminTourFile, curTourDataLoc, curAdminTourDataLoc, true, false);

            } else if ( curTourFile.ToLower().EndsWith(".sbk")
                ) {
                return mergeFile(curSanctionId, curAdminSanctionId, curTourFile, curAdminTourFile, curTourDataLoc, curAdminTourDataLoc, false, true);

            } else if ( curTourFile.ToLower().EndsWith("cs.htm") ) {
                return mergeHtmFile(curSanctionId, curAdminSanctionId, curTourFile, curAdminTourFile, curTourDataLoc, curAdminTourDataLoc);

            } else if ( curTourFile.ToLower().EndsWith("od.txt") ) {
                return mergeOdFile(curSanctionId, curAdminSanctionId, curTourFile, curAdminTourFile, curTourDataLoc, curAdminTourDataLoc);

            //} else if ( curTourFile.ToLower().EndsWith("tu.txt") ) {
            //    return mergeTuFile(curSanctionId, curAdminSanctionId, curTourFile, curAdminTourFile, curTourDataLoc, curAdminTourDataLoc);

            } else {

            }

            return returnValue;
        }

        private Boolean mergeFile( String inSanctionId, String curAdminSanctionId, String curTourFile, String curAdminTourFile, String curTourDataLoc, String curAdminTourDataLoc, Boolean skipHeader, Boolean changeSanction ) {
            Boolean returnValue = true;
            //myProgressInfo = new ProgressWindow();

            //Open admin file for reading
            //Open main file for write
            int curInputLineCount = 0;
            String inputBuffer = "", outputBuffer = "";

            try {
                using ( StreamReader curReader = new StreamReader(Path.Combine(curAdminTourDataLoc, curAdminTourFile)) ) {

                    using ( StreamWriter writer = File.AppendText(Path.Combine(curTourDataLoc, curTourFile)) ) {

                        while ( ( inputBuffer = curReader.ReadLine() ) != null ) {
                            curInputLineCount++;
                            if ( skipHeader && curInputLineCount == 1 ) {
                            } else {
                                if ( changeSanction ) {
                                    outputBuffer = inputBuffer.Replace(curAdminSanctionId, inSanctionId) + Environment.NewLine;
                                    writer.Write(outputBuffer);
                                } else {
                                    outputBuffer = inputBuffer + Environment.NewLine;
                                    writer.Write(outputBuffer);
                                }
                            }
                        }

                        writer.Close();
                    }
                }

            } catch (IOException ioe) {
                MessageBox.Show(String.Format("IOException encountered merging {0} to {1} : {2}", curAdminTourFile, curTourFile, ioe.Message));
                returnValue = false;
            } catch ( Exception ex ) {
                MessageBox.Show(String.Format("Exception encountered merging {0} to {1} : {2}", curAdminTourFile, curTourFile, ex.Message));
                returnValue = false;
            }

            return returnValue;
        }

        private Boolean mergeOdFile( String inSanctionId, String curAdminSanctionId, String curTourFile, String curAdminTourFile, String curTourDataLoc, String curAdminTourDataLoc ) {
            Boolean returnValue = true;
            //myProgressInfo = new ProgressWindow();
            //StreamReader

            try {
                int filePos = 0;
                using ( FileStream tourFileStream = File.Open(Path.Combine(curTourDataLoc, curTourFile), FileMode.OpenOrCreate, FileAccess.ReadWrite) ) {
                    using ( StreamReader tourFileReader = new StreamReader(tourFileStream) )
                    using ( StreamReader adminFileReader = new StreamReader(Path.Combine(curAdminTourDataLoc, curAdminTourFile)) ) 
                    using ( StreamWriter tourFileWriter = new StreamWriter(tourFileStream) ) {
                        String inputLine = "";
                        while ( ( inputLine = tourFileReader.ReadLine() ) != null ) {
                            if ( inputLine.Contains("*****") ) {
                                tourFileStream.Position = filePos;
                                break;
                            } else {
                                filePos += inputLine.Length;
                            }
                        }
                        tourFileWriter.Write(Environment.NewLine);

                        //Open admin file for reading
                            int curInputLineCount = 0, curHdrLinePos = 0;
                            String inputBuffer = "", outputBuffer = "";
                            while ( ( inputBuffer = adminFileReader.ReadLine() ) != null ) {
                                curInputLineCount++;
                                if ( curHdrLinePos == 0 ) {
                                    if ( inputBuffer.ToUpper().Contains("NAME OF OFFICIAL") ) {
                                        curHdrLinePos = curInputLineCount;
                                    }
                                } else {
                                    if ( curInputLineCount > ( curHdrLinePos + 1 ) ) {
                                        outputBuffer = inputBuffer + Environment.NewLine;
                                        tourFileWriter.Write(outputBuffer);
                                    }
                                }
                            }
                        }
                    }

            } catch ( IOException ioe ) {
                MessageBox.Show(String.Format("IOException encountered merging {0} to {1} : {2}", curAdminTourFile, curTourFile, ioe.Message));
                returnValue = false;
            } catch ( Exception ex ) {
                MessageBox.Show(String.Format("Exception encountered merging {0} to {1} : {2}", curAdminTourFile, curTourFile, ex.Message));
                returnValue = false;
            }

            return returnValue;
        }

        private Boolean mergeHtmFile( String inSanctionId, String curAdminSanctionId, String curTourFile, String curAdminTourFile, String curTourDataLoc, String curAdminTourDataLoc ) {
            Boolean returnValue = true;
            try {
                int filePos = 0;
                using ( FileStream tourFileStream = File.Open(Path.Combine(curTourDataLoc, curTourFile), FileMode.OpenOrCreate, FileAccess.ReadWrite) ) {
                    using ( StreamReader tourFileReader = new StreamReader(tourFileStream) )
                    using ( StreamReader adminFileReader = new StreamReader(Path.Combine(curAdminTourDataLoc, curAdminTourFile)) )

                    using ( StreamWriter tourFileWriter = new StreamWriter(tourFileStream) ) {

                        String outputBuffer = "";
                        String inputLine = "";
                        while ( ( inputLine = tourFileReader.ReadLine() ) != null ) {
                            if ( inputLine.Contains("Class=\"footer\"") ) {
                                //tourFileStream.Position = filePos;
                                break;
                            } else {
                                filePos += inputLine.Length;
                            }
                        }
                        tourFileWriter.Write(Environment.NewLine + Environment.NewLine + Environment.NewLine);

                        //Open admin file for reading
                        int curInputLineCount = 0, curHdrLinePos = 0;
                        while ( ( inputLine = adminFileReader.ReadLine() ) != null ) {
                            curInputLineCount++;
                            if ( curHdrLinePos == 0 ) {
                                if ( inputLine.Contains("<tr><th>SkierName</th>") ) {
                                    curHdrLinePos = curInputLineCount;
                                }
                            } else {
                                if ( inputLine.Contains("Class=\"footer\"") ) {
                                    break;
                                } else {
                                    outputBuffer = inputLine + Environment.NewLine;
                                    tourFileWriter.Write(outputBuffer);
                                }
                            }
                        }
                    }
                }

            } catch ( IOException ioe ) {
                MessageBox.Show(String.Format("IOException encountered merging {0} to {1} : {2}", curAdminTourFile, curTourFile, ioe.Message));
                returnValue = false;
            } catch ( Exception ex ) {
                MessageBox.Show(String.Format("Exception encountered merging {0} to {1} : {2}", curAdminTourFile, curTourFile, ex.Message));
                returnValue = false;
            }

            return returnValue;
        }

        private Boolean mergeTuFile( String inSanctionId, String curAdminSanctionId, String curTourFile, String curAdminTourFile, String curTourDataLoc, String curAdminTourDataLoc ) {
            Boolean returnValue = true;
            //myProgressInfo = new ProgressWindow();

            //Open admin file for reading
            //Open main file to append data
            int curInputLineCount = 0;
            String inputBuffer = "", outputBuffer = "", tempBuffer = "";
            String curRegion = getRegion(inSanctionId);

            try {
                using ( StreamReader curReader = new StreamReader(Path.Combine(curAdminTourDataLoc, curAdminTourFile)) ) {

                    using ( StreamWriter writer = File.AppendText(Path.Combine(curTourDataLoc, curTourFile)) ) {

                        while ( ( inputBuffer = curReader.ReadLine() ) != null ) {
                            curInputLineCount++;
                            if ( curInputLineCount > 1 ) {
                                inputBuffer += Environment.NewLine;
                                tempBuffer = inputBuffer.Replace(curAdminSanctionId, inSanctionId);
                                outputBuffer = tempBuffer.Substring(0, 14) + curRegion + tempBuffer.Substring(15);
                                writer.Write(outputBuffer);
                            }
                        }

                        writer.Close();
                    }
                }

            } catch ( IOException ioe ) {
                MessageBox.Show(String.Format("IOException encountered merging {0} to {1} : {2}", curAdminTourFile, curTourFile, ioe.Message));
                returnValue = false;
            } catch ( Exception ex ) {
                MessageBox.Show(String.Format("Exception encountered merging {0} to {1} : {2}", curAdminTourFile, curTourFile, ex.Message));
                returnValue = false;
            }

            return returnValue;
        }

        private ArrayList getEndOfTourReportList( String inSanctionNum, String inTourClass ) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCode, SortSeq, CodeValue, CodeDesc");
            curSqlStmt.Append(" FROM CodeValueList");
            curSqlStmt.Append(" WHERE ListName = 'EndOfTourReports'");
            curSqlStmt.Append(" ORDER BY SortSeq");
            DataTable curDataTable = getData(curSqlStmt.ToString());

            String curFileName, curFileMask;
            ArrayList curFileList = new ArrayList();
            foreach ( DataRow curRow in curDataTable.Rows ) {
                curFileMask = (String) curRow["CodeValue"];
                if ( curFileMask.StartsWith("######") ) {
                    curFileName = inSanctionNum;
                    if ( curFileMask.Substring(6, 1).Equals("*") ) {
                        curFileName += inTourClass + curFileMask.Substring(7);
                    } else {
                        curFileName += curFileMask.Substring(6);
                    }
                } else {
                    curFileName = curFileMask;
                }
                curFileList.Add(curFileName.ToLower());
            }

            return curFileList;
        }

        private DataRow getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT SanctionId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation");
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, TourDataLoc ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + inSanctionId + "' ");
            DataTable curDataTable = getData(curSqlStmt.ToString());
            if ( curDataTable.Rows.Count > 0 ) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable(inSelectStmt);
        }

        private String getRegion(String inSanctionId) {
            String curValue = inSanctionId.Substring(2, 1);
            String returnValue = curValue;

            if ( curValue.ToUpper().Equals("E") ) {
                returnValue = "EAST";
            } else if ( curValue.ToUpper().Equals("W") ) {
                returnValue = "WEST";
            } else if ( curValue.ToUpper().Equals("S") ) {
                returnValue = "SOUTH";
            } else if ( curValue.ToUpper().Equals("C") ) {
                returnValue = "SOUTHCENTRAL";
            } else if ( curValue.ToUpper().Equals("M") ) {
                returnValue = "MIDWEST";
            } else {
                returnValue = curValue;
            }
            return returnValue;
        }

        private String stringReplace( String inValue, char inCurValue, String inReplValue ) {
            StringBuilder curNewValue = new StringBuilder("");

            String[] curValues = inValue.Split(inCurValue);
            if ( curValues.Length > 1 ) {
                int curCount = 0;
                foreach ( String curValue in curValues ) {
                    curCount++;
                    if ( curValues.Length < curCount ) {
                        curNewValue.Append(curValue);
                    } else {
                        curNewValue.Append(curValue + inReplValue);
                    }
                }
            } else {
                curNewValue.Append(inValue);
            }

            return curNewValue.ToString();
        }

    }
}
