using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    public static class ZipUtil {
        public static void ZipFiles( string inputFolderPath, string outputPathAndFile ) {
            ZipFiles( inputFolderPath, outputPathAndFile, null, null );
        }
        public static void ZipFiles( string inputFolderPath, string outputPathAndFile, ArrayList inFileFilterList ) {
            ZipFiles( inputFolderPath, outputPathAndFile, null, inFileFilterList );
        }
        public static void ZipFiles( string inputFolderPath, string outputPathAndFile, string password ) {
            ZipFiles(inputFolderPath, outputPathAndFile, password, null);
        }
        public static void ZipFiles( string inputFolderPath, string outputPathAndFile, string password, ArrayList inFileFilterList ) {
            /* ------------------------------------------------------
             ------------------------------------------------------ */
            // generate file list
            ArrayList curFileList = GenerateFileList( inputFolderPath );

            // find number of chars to remove from orginal file path
            //int TrimLength = ( Directory.GetParent( inputFolderPath ) ).ToString().Length;
            int TrimLength = inputFolderPath.Length;
            if ( !(inputFolderPath.Substring( inputFolderPath.Length - 1 ).Equals( "\\" )) ) {
                TrimLength++;
            }
            //TrimLength += 1; //remove '\'

            FileStream curOutputStream;
            byte[] curOutputBuffer;
            string outPath = inputFolderPath + @"\" + outputPathAndFile;

            // create zip stream
            ZipOutputStream curOutputZipStream = new ZipOutputStream( File.Create( outPath ) ); 
            if ( password != null && password != String.Empty )
                curOutputZipStream.Password = password;

            // maximum compression
            curOutputZipStream.SetLevel( 9 ); 
            ZipEntry oZipEntry;
            //oZipEntry.ExternalFileAttributes;
            //oZipEntry.

            ProgressWindow curProgressInfo = new ProgressWindow();
            curProgressInfo.setProgressMin( 1 );
            curProgressInfo.setProgressMax( curFileList.Count );
            int curFileCount = 0;
            int curFileIndex = 0;
            String curFileNameOnly, curIwwfHomFileName = "";

            // for each file, generate a zipentry
            foreach (string curFileFilter in inFileFilterList) {
                curFileIndex = 1;
                if (curFileFilter.EndsWith( "hd.txt" )) {
                    curIwwfHomFileName = curFileFilter.ToUpper();
                }
                foreach (string curFileName in curFileList) {
                    curFileNameOnly = curFileName.Remove( 0, TrimLength );

                    if (System.Text.RegularExpressions.Regex.IsMatch( curFileNameOnly, curFileFilter, System.Text.RegularExpressions.RegexOptions.IgnoreCase )) {
                        //MessageBox.Show( "Index=" + curFileIndex + ", FileName=" + curFileNameOnly + ", File Filter=" + curFileFilter );
                        if (curFileFilter.Equals( ".hom$" )) {
                            curFileNameOnly = curIwwfHomFileName;
                        }
                        
                        curFileCount++;
                        curProgressInfo.setProgessMsg( "Processing " + curFileName );
                        curProgressInfo.setProgressValue( curFileCount );
                        curProgressInfo.Show();
                        curProgressInfo.Refresh();

                        oZipEntry = new ZipEntry( ( curFileNameOnly ) );
                        oZipEntry.DateTime = File.GetLastWriteTime( curFileName );
                        curOutputZipStream.PutNextEntry( oZipEntry );

                        // if a file ends with '/' its a directory
                        if (!curFileName.EndsWith( @"/" )) {
                            try {
                                curOutputStream = File.OpenRead( curFileName );
                                curOutputBuffer = new byte[curOutputStream.Length];
                                curOutputStream.Read( curOutputBuffer, 0, curOutputBuffer.Length );
                                curOutputZipStream.Write( curOutputBuffer, 0, curOutputBuffer.Length );
                                curOutputStream.Close();
                            } catch (Exception ex) {
                                curProgressInfo.setProgessMsg( "Bypassing busy file " + curFileName );
                                curProgressInfo.Show();
                                curProgressInfo.Refresh();
                            }
                        }
                    }
                    curFileIndex++;
                }
            }

            curOutputZipStream.Finish();
            curOutputZipStream.Close();
            curProgressInfo.Close();
        }

        private static ArrayList GenerateFileList( string Dir ) {
            ArrayList fils = new ArrayList();
            /* ------------------------------------------------------
             ------------------------------------------------------ */
            bool Empty = true;
            foreach ( string file in Directory.GetFiles( Dir ) ) // add each file in directory
            {
                fils.Add( file );
                Empty = false;
            }

            if ( Empty ) {
                if ( Directory.GetDirectories( Dir ).Length == 0 )
                // if directory is completely empty, add it
                {
                    fils.Add( Dir + @"/" );
                }
            }

            foreach ( string dirs in Directory.GetDirectories( Dir ) ) // recursive
            {
                foreach ( object obj in GenerateFileList( dirs ) ) {
                    fils.Add( obj );
                }
            }
            return fils; // return file list
        }

        public static void UnZipFiles( string zipPathAndFile, string outputFolder, string password, bool deleteZipFile ) {
            /* ------------------------------------------------------
             ------------------------------------------------------ */
            ZipInputStream s = new ZipInputStream( File.OpenRead( zipPathAndFile ) );
            if ( password != null && password != String.Empty )
                s.Password = password;
            ZipEntry theEntry;
            string tmpEntry = String.Empty;
            while ( ( theEntry = s.GetNextEntry() ) != null ) {
                string directoryName = outputFolder;
                string fileName = Path.GetFileName( theEntry.Name );
                // create directory 
                if ( directoryName != "" ) {
                    Directory.CreateDirectory( directoryName );
                }
                if ( fileName != String.Empty ) {
                    if ( theEntry.Name.IndexOf( ".ini" ) < 0 ) {
                        string fullPath = directoryName + "\\" + theEntry.Name;
                        fullPath = fullPath.Replace( "\\ ", "\\" );
                        string fullDirPath = Path.GetDirectoryName( fullPath );
                        if ( !Directory.Exists( fullDirPath ) ) Directory.CreateDirectory( fullDirPath );
                        FileStream streamWriter = File.Create( fullPath );
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while ( true ) {
                            size = s.Read( data, 0, data.Length );
                            if ( size > 0 ) {
                                streamWriter.Write( data, 0, size );
                            } else {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
            }
            s.Close();
            if ( deleteZipFile )
                File.Delete( zipPathAndFile );
        }
    }
}
