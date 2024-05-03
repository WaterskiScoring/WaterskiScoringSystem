using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    class PrintHeadToHeadAwards {
        #region  "Class instance variables"
        private Graphics myGraphicControl;

        private bool myCenterHeaderOnPage;
        private bool myCenterReportOnPage;
        private bool myPrintLandscape;
        private bool myPrintPreview;
        private bool myUsePageNum;
        private bool myShowHeader;
        private bool myShowFooter;

        private int myRowIdx;
        private int myPageNumber;
        private int myTourPrelimRounds;
        private int myTourBracketFirst;
        private int myTourBracket;
        private Single myPageWidth;
        private Single myPageHeight;
        private Single myLeftMargin;
        private Single myTopMargin;
        private Single myRightMargin;
        private Single myBottomMargin;

        private String myReportName;
        private String myReportHeader;
        private String myReportFooter;
        private String myTourRules;
        private String myTourName;
        private String myTourEvent;
        private String myTourPlcmtOrg;
        private String myTourPlcmtMethod;

        private Font myReportHeaderFont;
        private Font myReportFooterFont;
        private Font myReportBodyFont;
        private Font myReportBodyRegFont;
        private Font myReportBodyBoldFont;
        private Font myReportSubTitleFont;
        private Color myReportHeaderTextColor;
        private Color myReportFooterTextColor;
        private Color myReportBodyTextColor;

        private Pen myPen;
        private Brush myBrush;
        private ArrayList myTextBoxLikeControls;
        private ArrayList myPrintDelegates;
        private StringBuilder mytraceLog;
        private PrintDocument myPrintDoc;

        private DataTable myShowDataTable;
        private DataTable myDivInfoDataTable;
        private DataTable myShowPosDataTable;

        #endregion

        public PrintHeadToHeadAwards() {
            myReportName = "PrintHeadToHeadAwards";
            myReportHeader = "PrintHeadToHeadAwards";
            myReportFooter = "";
            myTourRules = "";
            myTourName = "";
            myReportHeaderFont = new Font( "Tahoma", 13, FontStyle.Bold, GraphicsUnit.Point );
            myReportHeaderTextColor = Color.Black;
            myReportFooterFont = new Font( "Tahoma", 8, FontStyle.Bold, GraphicsUnit.Point );
            myReportFooterTextColor = Color.Black;
            myReportBodyRegFont = new Font( "Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point );
            myReportBodyBoldFont = new Font( "Tahoma", 9, FontStyle.Bold, GraphicsUnit.Point );
            myReportSubTitleFont = new Font( "Tahoma", 11, FontStyle.Bold, GraphicsUnit.Point );

            myReportBodyTextColor = Color.Black;
            myCenterHeaderOnPage = false;
            myCenterReportOnPage = false;
            myPrintLandscape = true;
            myPrintPreview = true;
            myUsePageNum = true;
            myShowHeader = true;
            myShowFooter = true;

            myPageWidth = 0;
            myPageHeight = 0;
            myLeftMargin = 25;
            myTopMargin = 25;
            myRightMargin = 25;
            myBottomMargin = 35;
            myPageNumber = 0;
            myTourPrelimRounds = 0;
            myTourBracketFirst = 0;

            myTextBoxLikeControls = new ArrayList();
            myPrintDelegates = new ArrayList();
            myPen = new Pen( Color.Black );
            myBrush = Brushes.Black;
        }

        public bool Print() {
            return Print( true );
        }
        public bool Print(bool inShowPreview) {
            bool curReturnValue = true;
            try {
                PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
                PrintDialog curPrintDialog = new PrintDialog();

                curPrintDialog.AllowCurrentPage = true;
                curPrintDialog.AllowPrintToFile = true;
                curPrintDialog.AllowSelection = false;
                curPrintDialog.AllowSomePages = true;
                curPrintDialog.PrintToFile = false;
                curPrintDialog.ShowHelp = false;
                curPrintDialog.ShowNetwork = false;
                curPrintDialog.UseEXDialog = true;

                buildReportPositionDataTable();

                if (curPrintDialog.ShowDialog() == DialogResult.OK) {
                    myPrintDoc = new PrintDocument();
                    myPrintDoc.DocumentName = myReportName;
                    myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                    myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                    InitPrinting( ref myPrintDoc );
                    myPrintDoc.PrintPage += new PrintPageEventHandler( this.PrintReport );

                    if (inShowPreview) {
                        curPreviewDialog.Document = myPrintDoc;
						curPreviewDialog.Size = new System.Drawing.Size( 750, 750 );
						curPreviewDialog.WindowState = FormWindowState.Normal;
                        curPreviewDialog.Focus();
                        curPreviewDialog.ShowDialog();
                    } else {
                        myPrintDoc.Print();
                    }
                }

            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in print method of PrintHeadToHeadAwards \n Exception: " + ex.ToString() );
                curReturnValue = false;
            }
            return curReturnValue;
        }

        private void PrintReport(object inSender, System.Drawing.Printing.PrintPageEventArgs inPrintEventArgs) {
            int curPageSkierCount = 0, curPageSkierMax = 9999, curRound = 0, prevRound = 0;
            Single[] curPagePos;
            Single curTextBoxHeight = Convert.ToSingle( 36.0 );
            Single curTextBoxWidth = Convert.ToSingle( 220.0 );
            Single curRoundMargin = Convert.ToSingle( 40.0 );
            bool hasMorePages = false;
            bool showRow = true;
            DataRow curRow = null;
            DataRow[] curFindRows = null;
            myGraphicControl = inPrintEventArgs.Graphics;

            String curRoundName = "Round" + myTourEvent;
            String curFilterStmt = "";
            Int16 curSkierRound = 0;
			if ( myTourPlcmtOrg.ToLower().Equals( "div" ) ) {
				if ( myShowDataTable.Rows[myRowIdx][curRoundName].GetType() == System.Type.GetType( "System.Byte" ) ) {
					curSkierRound = Convert.ToInt16( (byte) myShowDataTable.Rows[myRowIdx][curRoundName] );
				} else if ( myShowDataTable.Rows[myRowIdx][curRoundName].GetType() == System.Type.GetType( "System.Int16" ) ) {
					curSkierRound = (Int16) myShowDataTable.Rows[myRowIdx][curRoundName];
				} else {
					curSkierRound = Convert.ToInt16( (Int32) myShowDataTable.Rows[myRowIdx][curRoundName] );
				}
				if ( curSkierRound == Convert.ToInt16( myTourPrelimRounds + 1 ) ) {
					curFilterStmt = "Round" + myTourEvent + " = " + curSkierRound + " AND AgeGroup = '" + (String) myShowDataTable.Rows[myRowIdx]["AgeGroup"] + "' ";
					curFindRows = myShowDataTable.Select( curFilterStmt );
					if ( curFindRows.Length > 0 ) {
						myTourBracketFirst = curFindRows.Length;
						myTourBracket = myTourBracketFirst;
					} else {
						myTourBracketFirst = 0;
						myTourBracket = 0;
					}
				}

			} else if ( myTourPlcmtOrg.ToLower().Equals( "group" ) ) {
				if ( myShowDataTable.Rows[myRowIdx][curRoundName].GetType() == System.Type.GetType( "System.Byte" ) ) {
					curSkierRound = Convert.ToInt16( (byte) myShowDataTable.Rows[myRowIdx][curRoundName] );
				} else if ( myShowDataTable.Rows[myRowIdx][curRoundName].GetType() == System.Type.GetType( "System.Int16" ) ) {
					curSkierRound = (Int16) myShowDataTable.Rows[myRowIdx][curRoundName];
				} else {
					curSkierRound = Convert.ToInt16( (Int32) myShowDataTable.Rows[myRowIdx][curRoundName] );
				}
				if ( curSkierRound == Convert.ToInt16( myTourPrelimRounds + 1 ) ) {
					curFilterStmt = "Round" + myTourEvent + " = " + curSkierRound + " AND EventGroup = '" + (String) myShowDataTable.Rows[myRowIdx]["EventGroup"] + "' ";
					curFindRows = myShowDataTable.Select( curFilterStmt );
					if ( curFindRows.Length > 0 ) {
						myTourBracketFirst = curFindRows.Length;
						myTourBracket = myTourBracketFirst;
					} else {
						myTourBracketFirst = 0;
						myTourBracket = 0;
					}
				}

			} else {
                if (myRowIdx == 0) {
                    curSkierRound = Convert.ToInt16( myTourPrelimRounds + 1);
                    curFilterStmt = "Round" + myTourEvent + " = " + curSkierRound;
                    curFindRows = myShowDataTable.Select( curFilterStmt );
                    if (curFindRows.Length > 0) {
                        myTourBracketFirst = curFindRows.Length;
                        myTourBracket = myTourBracketFirst;
                    } else {
                        myTourBracketFirst = 0;
                        myTourBracket = 0;
                    }
                }
            }
            
            String curText = "", curDiv = "", prevDiv = "", curRunOrderGroup = "", prevRunOrderGroup = "";
            Int16 curRunOrder = 0;
            Decimal RankingScore = 0;
            StringFormat curTextFormat = new StringFormat();
            curTextFormat.Alignment = StringAlignment.Center;
            curTextFormat.LineAlignment = StringAlignment.Center;

            Single curPosX = myLeftMargin + Convert.ToSingle( 6 );
            Single curPosY = DrawHeader();
            Single curPosYPageTop = curPosY;

            while (curPageSkierCount <= curPageSkierMax && myRowIdx < myShowDataTable.Rows.Count && !hasMorePages) {
                curRow = myShowDataTable.Rows[myRowIdx];
				if ( myTourPlcmtOrg.ToLower().Equals( "div" ) ) {
					curDiv = (String) curRow["AgeGroup"];
				} else if ( myTourPlcmtOrg.ToLower().Equals( "group" ) ) {
					curDiv = (String) curRow["EventGroup"];
				} else {
                    curDiv = "";
                }
				curRunOrderGroup = (String)curRow["RunOrderGroup"];
                if (curRow[curRoundName].GetType() == System.Type.GetType( "System.Byte" )) {
                    curRound = Convert.ToInt16( (byte)curRow[curRoundName] );
                } else if (curRow[curRoundName].GetType() == System.Type.GetType( "System.Int16" )) {
                    curRound = (Int16)curRow[curRoundName];
                } else {
                    curRound = Convert.ToInt16( (Int32)curRow[curRoundName] );
                }
                if (curDiv == prevDiv || curPageSkierCount == 0) {
                    if (curRound != prevRound && curPageSkierCount > 0) {
                        curPosY = curPosYPageTop;
                        curPosX += curTextBoxWidth + curRoundMargin;
                        myTourBracket = myTourBracket / 2;
						prevRunOrderGroup = "";
                    }

                    if ( curRunOrderGroup == prevRunOrderGroup ) {
                        if (showRow) {
                            curPosY = (Single)curFindRows[0]["Box2PosY"];
                            myReportBodyFont = myReportBodyRegFont;
                        }
                    } else {
                        curFindRows = myShowPosDataTable.Select( "BracketSize = " + myTourBracket + " AND RunOrderGroup = '" + curRunOrderGroup + "'" );
                        if (myTourPlcmtMethod.Equals( "RunOrder" )) {
                            myReportBodyFont = myReportBodyRegFont;
                        } else {
                            myReportBodyFont = myReportBodyBoldFont;
                        }
                        if (curFindRows.Length > 0) {
                            showRow = true;
                            curPosY = (Single)curFindRows[0]["Box1PosY"];
                        } else {
                            showRow = false;
                        }
                    }
                    if (showRow) {
                        try {
                            curRunOrder = (Int16)curRow["RunOrder"];
                        } catch {
                            curRunOrder = 0;
                        }
                        try {
                            RankingScore = (Decimal)curRow["RankingScore"];
                        } catch {
                            RankingScore = 0;
                        }

                        curText = (String)curRow["SkierName"] + " (" + (Int16)curRow["RunOrder"] + "-" + curRunOrder + ")";
                        if (myTourPlcmtMethod.Equals( "RunOrder" )) {
                            curText += "\n";
                        } else {
                            if (myTourEvent.ToLower().Equals( "jump" )) {
                                if (myTourPlcmtMethod.ToLower().Equals( "points" )) {
                                    curText += "\n  Points: " + ( (Decimal)curRow["Points" + myTourEvent] ).ToString( "##0.00" );
                                } else {
                                    if (myTourRules.ToLower().Equals( "iwwf" )) {
                                        curText += "\n  Meters: " + ( (Decimal)curRow["ScoreMeters"] ).ToString( "##0.00" );
                                    } else {
                                        curText += "\n  Feet: " + ( (Decimal)curRow["ScoreFeet"] ).ToString( "##0.00" );
                                    }
                                }
                            } else if (myTourEvent.ToLower().Equals( "trick" )) {
                                if (myTourPlcmtMethod.ToLower().Equals( "points" )) {
                                    curText += "\n  Points: " + ( (Decimal)curRow["Points" + myTourEvent] ).ToString( "##0.00" );
                                } else {
                                    curText += "\n  Score: " + ( (Int16)curRow["Score" + myTourEvent] ).ToString( "##,##0" );
                                }
                            } else {
                                if (myTourPlcmtMethod.ToLower().Equals( "points" )) {
                                    curText += "\n  Points: " + ( (Decimal)curRow["Points" + myTourEvent] ).ToString( "##0.00" );
                                } else {
                                    curText += "\n  Score: " + ( (Decimal)curRow["Score" + myTourEvent] ).ToString( "##0.00" );
                                }
                            }
                        }
                        curText += "  Rank: " + RankingScore.ToString( "###0.0" );
                        curPagePos = PrintText( curText, curPosX, curPosY, curTextBoxWidth, curTextBoxHeight, 0, 0, myReportBodyFont, new SolidBrush( myReportBodyTextColor ), curTextFormat, true );
                        if ( curRunOrderGroup == prevRunOrderGroup && myTourBracket > 2) {
                            DrawBoxConnect( new Pen( myReportBodyTextColor ), curPagePos[0], (Single)curFindRows[0]["Box1PosY"], (Single)curFindRows[0]["ConnectHeight"] );
                        }
                        curPageSkierCount++;
                        myRowIdx++;
                        if (curPageSkierCount >= curPageSkierMax) hasMorePages = true;
                    } else {
                        myRowIdx++;
                    }
                } else {
                    hasMorePages = true;
                    myTourBracket = myTourBracketFirst;
                }
                if (hasMorePages) break;
                prevDiv = curDiv;
				prevRunOrderGroup = curRunOrderGroup;
                prevRound = curRound;
            }

            inPrintEventArgs.HasMorePages = hasMorePages;
            if (myRowIdx >= myShowDataTable.Rows.Count) {
                myRowIdx = 0;
                myPageNumber = 0;
                myTourBracket = myTourBracketFirst;
            }
        }

        private void InitPrinting(ref PrintDocument inPrintDoc) {
            mytraceLog = new System.Text.StringBuilder();

            myRowIdx = 0;
            // Calculate Form position for printing
            // Set page area
            if (myTopMargin == 0) {
                myTopMargin = inPrintDoc.DefaultPageSettings.Margins.Top;
            }
            if (myBottomMargin == 0) {
                myBottomMargin = inPrintDoc.DefaultPageSettings.Margins.Bottom;
            }
            if (myLeftMargin == 0) {
                myLeftMargin = inPrintDoc.DefaultPageSettings.Margins.Left;
            }
            if (myRightMargin == 0) {
                myRightMargin = inPrintDoc.DefaultPageSettings.Margins.Right;
            }

            if (myPrintLandscape) {
                inPrintDoc.DefaultPageSettings.Landscape = true;
            } else {
                //Int32 curPrintAreaWidth = inPrintDoc.DefaultPageSettings.Bounds.Width - myRightMargin - myLeftMargin;
                inPrintDoc.DefaultPageSettings.Landscape = false;
            }

            myPageHeight = inPrintDoc.DefaultPageSettings.Bounds.Height - myTopMargin - myBottomMargin;
            myPageWidth = inPrintDoc.DefaultPageSettings.Bounds.Width - myLeftMargin - myRightMargin;
            if (myCenterReportOnPage) {
                Int32 curMarginAdj = Convert.ToInt32( Math.Round( ( Convert.ToDouble( myPageWidth ) / 2 ) ) );
                myLeftMargin -= curMarginAdj;
                myRightMargin -= curMarginAdj;
            }
        }

        // The funtion that print the title, page number, and the header row
        private Single DrawHeader() {
            Single curPosY = myTopMargin;
            Single curPosX = myLeftMargin;
            Single curTextBoxHeight = Convert.ToSingle( 18.00 );
            Single curHeaderBufferHeight = Convert.ToSingle( 12.00 );
            String curText = "";

            StringFormat TitleFormat = new StringFormat();
            TitleFormat.Alignment = StringAlignment.Center;
            TitleFormat.LineAlignment = StringAlignment.Center;

            // Print the title if available and indicated
            if (myShowHeader) {
                curText = myTourName;
                curPosX = myLeftMargin;
                Single[] curPagePos = PrintText( curText, curPosX, curPosY, myPageWidth, curTextBoxHeight, 0, 0, myReportHeaderFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat, false );
                curPosY = curPagePos[1];

                if (myReportHeader.Length > 1) {
                    curText = myReportHeader;
                    if (myTourPlcmtOrg.ToLower().Equals( "div" )) {
                        DataRow[] curFindRows;
                        String curDiv = (String)myShowDataTable.Rows[myRowIdx]["AgeGroup"];
                        curFindRows = myDivInfoDataTable.Select( "Div = '" + curDiv + "'" );
                        if (curFindRows.Length > 0) {
                            curText += " - " + (String)curFindRows[0]["DivName"];
                        }
                    }
                    TitleFormat.Alignment = StringAlignment.Center;
                    TitleFormat.LineAlignment = StringAlignment.Center;
                    curPosX = myLeftMargin;
                    curPagePos = PrintText( curText, curPosX, curPosY, myPageWidth, curTextBoxHeight, 0, 0, myReportSubTitleFont, new SolidBrush( myReportHeaderTextColor ), TitleFormat, false );
                    curPosY = curPagePos[1] + curHeaderBufferHeight;
                } else {
                    curPosY += curTextBoxHeight + curHeaderBufferHeight;
                }
            } else {
                curPosY += curTextBoxHeight + curTextBoxHeight + curHeaderBufferHeight;
            }

            // Printing the page number if required
            if (myShowFooter) {
                if (myUsePageNum) {
                    myPageNumber++;
                    string PageString = "Page " + myPageNumber.ToString();
                    string DateString = DateTime.Now.ToString();

                    StringFormat PageStringFormat = new StringFormat();
                    PageStringFormat.Trimming = StringTrimming.Word;
                    PageStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    PageStringFormat.Alignment = StringAlignment.Far;

                    StringFormat DateStringFormat = new StringFormat();
                    DateStringFormat.Trimming = StringTrimming.Word;
                    DateStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    DateStringFormat.Alignment = StringAlignment.Near;

                    Font PageStringFont = new Font( "Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point );
                    Single curFontHeight = FontHeight( new Font( PageStringFont.Name, PageStringFont.Size ) );

                    Single curPosYFooter = myPageHeight + myTopMargin;
                    PrintText( DateString, curPosX, curPosYFooter, 0, 0, PageStringFont, false, HorizontalAlignment.Left );
                    PrintText( PageString, curPosX, curPosYFooter, myPageWidth, 0, 0, 0, PageStringFont, new SolidBrush( myReportFooterTextColor ), BuildFormat( HorizontalAlignment.Center ), false );
                    curPosYFooter += curFontHeight;
                }
            }

            return curPosY;
        }

        private void DrawBoxConnect(Pen inPen, Single inPosX, Single inPosY, Single inBoxHeight) {
            Single curPosY = inPosY + 18;
            Single curPosXBox = inPosX + 15;
            Single curPosYPointer = curPosY + ( inBoxHeight / 2 );
            myGraphicControl.DrawLine( inPen, inPosX, curPosY, curPosXBox, curPosY );
            myGraphicControl.DrawLine( inPen, inPosX, curPosY + inBoxHeight, curPosXBox, curPosY + inBoxHeight );
            myGraphicControl.DrawLine( inPen, curPosXBox, curPosY, curPosXBox, curPosY + inBoxHeight );
            myGraphicControl.DrawLine( inPen, curPosXBox, curPosYPointer, curPosXBox + 25, curPosYPointer );
        }

        public Single[] PrintText(String inText, RectangleF inBox, Font inFont, Brush inBrush, StringFormat inFormat) {
            myGraphicControl.DrawString( inText, inFont, inBrush, inBox, inFormat );
            return new Single[2] { inBox.X + inBox.Width, inBox.Y + inBox.Height };
        }
        public Single[] PrintText(String inText, Single inTextLocX, Single inTextLocY, Single inTextWidth, Single inTextHeight, Single inPageAdjX, Single inPageAdjY, Font inFont, Brush inBrush, StringFormat inFormat) {
            return PrintText( inText, inTextLocX, inTextLocY, inTextWidth, inTextHeight, inPageAdjX, inPageAdjY, inFont, inBrush, inFormat, true );
        }
        public Single[] PrintText(String inText, Single inTextLocX, Single inTextLocY, Single inTextWidth, Single inTextHeight, Single inPageAdjX, Single inPageAdjY, Font inFont, Brush inBrush, StringFormat inFormat, bool inDrawBox) {
            Single curPosX = inPageAdjX + inTextLocX;
            Single curPosY = inPageAdjY + inTextLocY;
            SizeF curTextSize = StringSize( inText, inFont );
            Single curBoxHeight = curTextSize.Height;
            if (curBoxHeight < inTextHeight) curBoxHeight = inTextHeight;
            Single curBoxWidth = curTextSize.Width;
            if (curBoxWidth < inTextWidth) curBoxWidth = inTextWidth;
            RectangleF curBox = new RectangleF( curPosX, curPosY, curBoxWidth, curBoxHeight );
            if (inDrawBox) myGraphicControl.DrawRectangle( Pens.Gray, curPosX, curPosY, curBoxWidth, curBoxHeight );
            myGraphicControl.DrawString( inText, inFont, inBrush, curBox, inFormat );
            return new Single[2] { curPosX + curBoxWidth, curPosY + curBoxHeight };
        }
        public Single[] PrintText(String inText, Single inTextLocX, Single inTextLocY, Single inPageAdjX, Single inPageAdjY, Font inFont, bool isVAlignCenter, HorizontalAlignment inHAlign) {
            Single curPosX = inPageAdjX + inTextLocX;
            Single curPosY = inPageAdjY + inTextLocY;
            SizeF curTextSize = myGraphicControl.MeasureString( inText, inFont );
            int curWidth = Convert.ToInt32( curTextSize.Width );
            int curHeight = Convert.ToInt32( curTextSize.Height );

            // Print Text
            if (isVAlignCenter) {
                curPosY += 2;
                curHeight = curHeight + 4;
            }

            PointF curPosition = new PointF( curPosX, curPosY );
            myGraphicControl.DrawString( inText, inFont, myBrush, curPosition );

            return new Single[2] { curPosX + curWidth, curPosY + curHeight };
        }

        private Single FontHeight(Font font) {
            return font.GetHeight( myGraphicControl );
        }
        private SizeF StringSize(String inText, Font inFont) {
            return myGraphicControl.MeasureString( inText, inFont );
        }

        private StringFormat BuildFormat(HorizontalAlignment hAlignment) {
            StringFormat drawFormat = new StringFormat();
            switch (hAlignment) {
                case HorizontalAlignment.Left:
                    drawFormat.Alignment = StringAlignment.Near;
                    break;
                case HorizontalAlignment.Center:
                    drawFormat.Alignment = StringAlignment.Center;
                    break;
                case HorizontalAlignment.Right:
                    drawFormat.Alignment = StringAlignment.Far;
                    break;
            }
            return drawFormat;
        }

        private void buildReportPositionDataTable() {
            myShowPosDataTable = new DataTable();

            #region Setup table to hold bracket positions
            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "BracketSize";
            curCol.DataType = System.Type.GetType( "System.Int32" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 16;
            myShowPosDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunOrderGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "HH1";
            myShowPosDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Box1PosY";
            curCol.DataType = System.Type.GetType( "System.Single" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            myShowPosDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Box2PosY";
            curCol.DataType = System.Type.GetType( "System.Single" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            myShowPosDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "ConnectHeight";
            curCol.DataType = System.Type.GetType( "System.Single" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            myShowPosDataTable.Columns.Add( curCol );
            #endregion

            DataRowView newDataRow = null;

            #region Bracket of 16 text box positions
            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 16;
            newDataRow["RunOrderGroup"] = "HH1";
            newDataRow["Box1PosY"] = Convert.ToSingle( 81.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 123.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 42.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 16;
            newDataRow["RunOrderGroup"] = "HH8";
            newDataRow["Box1PosY"] = Convert.ToSingle( 171.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 213.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 42.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 16;
            newDataRow["RunOrderGroup"] = "HH5";
            newDataRow["Box1PosY"] = Convert.ToSingle( 261.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 303.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 42.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 16;
            newDataRow["RunOrderGroup"] = "HH4";
            newDataRow["Box1PosY"] = Convert.ToSingle( 351.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 393.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 42.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 16;
            newDataRow["RunOrderGroup"] = "HH2";
            newDataRow["Box1PosY"] = Convert.ToSingle( 441.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 483.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 42.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 16;
            newDataRow["RunOrderGroup"] = "HH7";
            newDataRow["Box1PosY"] = Convert.ToSingle( 531.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 573.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 42.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 16;
            newDataRow["RunOrderGroup"] = "HH6";
            newDataRow["Box1PosY"] = Convert.ToSingle( 621.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 663.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 42.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 16;
            newDataRow["RunOrderGroup"] = "HH3";
            newDataRow["Box1PosY"] = Convert.ToSingle( 711.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 753.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 42.0 );
            newDataRow.EndEdit();
            #endregion

            #region Bracket of 8 text box positions
            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 8;
            newDataRow["RunOrderGroup"] = "HH1";
            newDataRow["Box1PosY"] = Convert.ToSingle( 102.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 192.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 90.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 8;
            newDataRow["RunOrderGroup"] = "HH4";
            newDataRow["Box1PosY"] = Convert.ToSingle( 282.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 372.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 90.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 8;
            newDataRow["RunOrderGroup"] = "HH2";
            newDataRow["Box1PosY"] = Convert.ToSingle( 462.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 552.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 90.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 8;
            newDataRow["RunOrderGroup"] = "HH3";
            newDataRow["Box1PosY"] = Convert.ToSingle( 642.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 732.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 90.0 );
            newDataRow.EndEdit();
            #endregion

            #region Bracket of 4 text box positions
            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 4;
            newDataRow["RunOrderGroup"] = "HH1";
            newDataRow["Box1PosY"] = Convert.ToSingle( 147.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 327.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 180.0 );
            newDataRow.EndEdit();

            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 4;
            newDataRow["RunOrderGroup"] = "HH2";
            newDataRow["Box1PosY"] = Convert.ToSingle( 507.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 687.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 180.0 );
            newDataRow.EndEdit();
            #endregion

            #region Bracket of 2 text box positions
            newDataRow = myShowPosDataTable.DefaultView.AddNew();
            newDataRow["BracketSize"] = 2;
            newDataRow["RunOrderGroup"] = "HH1";
            newDataRow["Box1PosY"] = Convert.ToSingle( 236.0 );
            newDataRow["Box2PosY"] = Convert.ToSingle( 597.0 );
            newDataRow["ConnectHeight"] = Convert.ToSingle( 54.0 );
            newDataRow.EndEdit();
            #endregion

        }

        #region  "Getter and Setter for all instance objects"

        public bool ShowHeader {
            get { return myShowHeader; }
            set { myShowHeader = value; }
        }
        public bool ShowFooter {
            get { return myShowFooter; }
            set { myShowFooter = value; }
        }
        public bool UsePageNum {
            get { return myUsePageNum; }
            set { myUsePageNum = value; }
        }
        public String ReportName {
            get { return myReportName; }
            set { myReportName = value; }
        }
        public String ReportHeader {
            get { return myReportHeader; }
            set { myReportHeader = value; }
        }
        public String ReportFooter {
            get { return myReportFooter; }
            set { myReportFooter = value; }
        }
        public String TourRules {
            get { return myTourRules; }
            set { myTourRules = value; }
        }
        public String TourName {
            get { return myTourName; }
            set { myTourName = value; }
        }
        public String TourEvent {
            get { return myTourEvent; }
            set { myTourEvent = value; }
        }
        public String TourPlcmtOrg {
            get { return myTourPlcmtOrg; }
            set { myTourPlcmtOrg = value; }
        }
        public String TourPlcmtMethod {
            get { return myTourPlcmtMethod; }
            set { myTourPlcmtMethod = value; }
        }
        public Font ReportHeaderFont {
            get { return myReportHeaderFont; }
            set { myReportHeaderFont = value; }
        }
        public Color ReportHeaderTextColor {
            get { return myReportHeaderTextColor; }
            set { myReportHeaderTextColor = value; }
        }
        public bool CenterHeaderOnPage {
            get { return myCenterHeaderOnPage; }
            set { myCenterHeaderOnPage = value; }
        }
        public bool CenterReportOnPage {
            get { return myCenterReportOnPage; }
            set { myCenterReportOnPage = value; }
        }
        public bool PrintLandscape {
            get { return myPrintLandscape; }
            set { myPrintLandscape = value; }
        }
        public bool PrintPreview {
            get { return myPrintPreview; }
            set { myPrintPreview = value; }
        }
        public Single PageWidth {
            get { return myPageWidth; }
            set { myPageWidth = value; }
        }
        public Single PageHeight {
            get { return myPageHeight; }
            set { myPageHeight = value; }
        }
        public Single LeftMargin {
            get { return myLeftMargin; }
            set { myLeftMargin = value; }
        }
        public Single TopMargin {
            get { return myTopMargin; }
            set { myTopMargin = value; }
        }
        public Single RightMargin {
            get { return myRightMargin; }
            set { myRightMargin = value; }
        }
        public Single BottomMargin {
            get { return myBottomMargin; }
            set { myBottomMargin = value; }
        }
        public int TourPrelimRounds {
            get { return myTourPrelimRounds; }
            set { myTourPrelimRounds = value; }
        }
        public DataTable ShowDataTable {
            get { return myShowDataTable; }
            set { myShowDataTable = value; }
        }

        public DataTable DivInfoDataTable {
            get { return myDivInfoDataTable; }
            set { myDivInfoDataTable = value; }
        }

        #endregion
    }
}
