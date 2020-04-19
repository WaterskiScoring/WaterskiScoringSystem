using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

class DataGridViewPrinter
{
    private DataGridView [] TheDataGridList; // The DataGridView Control which will be printed
    private PrintDocument ThePrintDocument; // The PrintDocument to be used for printing
    private int TheActiveViewCount = 0;
    private bool IsCenterOnPage; // Determine if the report will be printed in the Top-Center of the page
    private bool IsWithTitle; // Determine if the page contain title text
    private bool IsWithSubtitle; // Determine if the page contain subtitle text
    private bool IsWithGridTitle; //Determine if the page contains a DataGridView Title
    private string TheTitleText; // The title text to be printed in each page (if IsWithTitle is set to true)
    private Font TheTitleFont; // The font to be used with the title text (if IsWithTitle is set to true)
    private Color TheTitleColor; // The color to be used with the title text (if IsWithTitle is set to true)
    private bool IsWithPaging; // Determine if paging is used

    static int CurrentRow; // A static parameter that keep track on which Row (in the DataGridView control) that should be printed

    static int PageNumber;

    private int PageWidth;
    private int PageHeight;
    private int LeftMargin;
    private int TopMargin;
    private int RightMargin;
    private int BottomMargin;

    private float CurrentY; // A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate

    private float RowHeaderHeight;
    private List<float> RowsHeight;
    private List<float> ColumnsWidth;
    private List<StringRowPrinter> mySubtitleList;
    private List<StringRowPrinter> myGridViewTitleList;
    private int mySubtitleIdx;
    private int myGridViewTitleIdx;
    private float TheDataGridViewWidth;
        
    // Maintain a generic list to hold start/stop points for the column printing
    // This will be used for wrapping in situations where the DataGridView will not fit on a single page
    private List<int[]> mColumnPoints;
    private List<float> mColumnPointsWidth;
    private int mColumnPoint;

    // The class constructor
    public DataGridViewPrinter( DataGridView [] aDataGridList
            , PrintDocument aPrintDocument
            , bool CenterOnPage
            , bool WithTitle
            , string aTitleText
            , Font aTitleFont
            , Color aTitleColor
            , bool WithPaging
        ) {
        TheDataGridList = aDataGridList;
        InitializeComponent(aPrintDocument, CenterOnPage, WithTitle, aTitleText, aTitleFont, aTitleColor, WithPaging );
    }

    public DataGridViewPrinter( DataGridView aDataGridView
            , PrintDocument aPrintDocument
            , bool CenterOnPage
            , bool WithTitle
            , string aTitleText
            , Font aTitleFont
            , Color aTitleColor
            , bool WithPaging 
        ) {
        TheDataGridList = new DataGridView[1];
        TheDataGridList[0] = aDataGridView;
        InitializeComponent( aPrintDocument, CenterOnPage, WithTitle, aTitleText, aTitleFont, aTitleColor, WithPaging );
    }

    // The class constructor
    public DataGridViewPrinter( DataGridView inDataGridView
            , PrintDocument aPrintDocument
            , bool inCenterOnPage
            , int inPageHeight
            , int inPageWidth
            , int inTopMargin
            , int inBottomMargin
            , int inLeftMargin
            , int inRightMargin
        ) {
        TheDataGridList = new DataGridView[1];
        TheDataGridList[0] = inDataGridView;
        InitializeComponent( aPrintDocument, inCenterOnPage, false, "", null, Color.Black, false );

        // Page margins
        PageHeight = inPageHeight;
        PageWidth = inPageWidth;
        TopMargin = inTopMargin;
        BottomMargin = inBottomMargin;
        LeftMargin = inLeftMargin;
        RightMargin = inRightMargin;
    }

    private void InitializeComponent( 
            PrintDocument aPrintDocument
            , bool CenterOnPage
            , bool WithTitle
            , string aTitleText
            , Font aTitleFont
            , Color aTitleColor
            , bool WithPaging
        ) {
        ThePrintDocument = aPrintDocument;
        IsCenterOnPage = CenterOnPage;
        IsWithTitle = WithTitle;
        TheTitleText = aTitleText;
        TheTitleFont = aTitleFont;
        TheTitleColor = aTitleColor;
        IsWithPaging = WithPaging;
        TheActiveViewCount = 0;
        PageNumber = 0;

        // Claculating the PageWidth and the PageHeight
        if ( ThePrintDocument.DefaultPageSettings.Landscape ) {
            PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Width;
            PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Height;
        } else {
            PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Width;
            PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Height;
        }

        // Claculating the page margins
        LeftMargin = ThePrintDocument.DefaultPageSettings.Margins.Left;
        TopMargin = ThePrintDocument.DefaultPageSettings.Margins.Top;
        RightMargin = ThePrintDocument.DefaultPageSettings.Margins.Right;
        BottomMargin = ThePrintDocument.DefaultPageSettings.Margins.Bottom;

        // First, the current row to be printed is the first row in the DataGridView control
        CurrentRow = 0;
    }

    //Methods for building subtitle string rows
    public void SubtitleList() {
        mySubtitleList = new List<StringRowPrinter>();
        mySubtitleIdx = 0;
        IsWithSubtitle = true;
    }
    public int SubtitleIdx {
        get {
            return mySubtitleIdx;
        }
        set {
            mySubtitleIdx = value;
        }
    }
    public StringRowPrinter SubtitleRow {
        get {
            return mySubtitleList[mySubtitleIdx];
        }
        set {
            mySubtitleList.Add(value);
        }
    }

    //Methods for building DataGridView title string rows
    public void GridViewTitleList() {
        myGridViewTitleList = new List<StringRowPrinter>();
        myGridViewTitleIdx = 0;
        IsWithGridTitle = true;
    }
    public int GridViewTitleIdx {
        get {
            return myGridViewTitleIdx;
        }
        set {
            myGridViewTitleIdx = value;
        }
    }
    public StringRowPrinter GridViewTitleRow {
        get {
            return myGridViewTitleList[myGridViewTitleIdx];
        }
        set {
            myGridViewTitleList.Add( value );
        }
    }

    // The function that calculate the height of each row (including the header row), 
    // the width of each column (according to the longest text in all its cells including the header cell),
    // and the whole DataGridView width
    private void Calculate( Graphics g, DataGridView inDataGridView ) {
        SizeF tmpSize = new SizeF();
        Font tmpFont;
        float tmpWidth;
        String curColName, curColHdrName;

        RowsHeight = new List<float>();
        ColumnsWidth = new List<float>();

        mColumnPoints = new List<int[]>();
        mColumnPointsWidth = new List<float>();

        TheDataGridViewWidth = 0;
        for ( int i = 0; i < inDataGridView.Columns.Count; i++ ) {
            tmpFont = inDataGridView.ColumnHeadersDefaultCellStyle.Font;
            if ( tmpFont == null ) // If there is no special HeaderFont style, then use the default DataGridView font style
                tmpFont = inDataGridView.DefaultCellStyle.Font;

            curColHdrName = inDataGridView.Columns[i].Name;
            tmpSize = g.MeasureString( inDataGridView.Columns[i].HeaderText, tmpFont );
            tmpWidth = tmpSize.Width;
            RowHeaderHeight = tmpSize.Height;

            if ( inDataGridView.Columns[i].GetType() != typeof( DataGridViewImageColumn ) ) {

                for ( int j = 0; j < inDataGridView.Rows.Count; j++ ) {
                    tmpFont = inDataGridView.Rows[j].DefaultCellStyle.Font;
                    if ( tmpFont == null ) // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
                        tmpFont = inDataGridView.DefaultCellStyle.Font;

                    tmpSize = g.MeasureString( inDataGridView.Rows[j].Cells[i].EditedFormattedValue.ToString(), tmpFont );
                    if ( tmpSize.Width > tmpWidth )
                        tmpWidth = tmpSize.Width;
                }
            
            }
            if ( inDataGridView.Columns[i].Visible ) {
                TheDataGridViewWidth += tmpWidth;
            }
            ColumnsWidth.Add( tmpWidth );
        }

        tmpFont = inDataGridView.DefaultCellStyle.Font;
        for ( int j = 0; j < inDataGridView.Rows.Count; j++ ) {
            if ( inDataGridView.Rows[j].Height < 9 ) {
                RowsHeight.Add( inDataGridView.Rows[j].Height );
            } else {
                tmpSize = g.MeasureString( "Anything", tmpFont );
                RowsHeight.Add( tmpSize.Height + inDataGridView.DefaultCellStyle.Padding.Bottom + inDataGridView.DefaultCellStyle.Padding.Top );
            }
        }
        
        // Define the start/stop column points based on the page width and the DataGridView Width
        // We will use this to determine the columns which are drawn on each page and how wrapping will be handled
        // By default, the wrapping will occurr such that the maximum number of columns for a page will be determine
        int k;

        int mStartPoint = 0;
        for ( k = 0; k < inDataGridView.Columns.Count; k++ )
            if ( inDataGridView.Columns[k].Visible ) {
                mStartPoint = k;
                break;
            }

        int mEndPoint = inDataGridView.Columns.Count;
        for ( k = inDataGridView.Columns.Count - 1; k >= 0; k-- )
            if ( inDataGridView.Columns[k].Visible ) {
                mEndPoint = k + 1;
                break;
            }

        float mTempWidth = TheDataGridViewWidth;
        float mTempPrintArea = (float)PageWidth - (float)LeftMargin - (float)RightMargin;

        // We only care about handling where the total datagridview width is bigger then the print area
        if ( TheDataGridViewWidth > mTempPrintArea ) {
            mTempWidth = 0.0F;
            for ( k = 0; k < inDataGridView.Columns.Count; k++ ) {
                if ( inDataGridView.Columns[k].Visible ) {
                    mTempWidth += ColumnsWidth[k];
                    // If the width is bigger than the page area, then define a new column print range
                    if ( mTempWidth > mTempPrintArea ) {
                        mTempWidth -= ColumnsWidth[k];
                        mColumnPoints.Add( new int[] { mStartPoint, mEndPoint } );
                        mColumnPointsWidth.Add( mTempWidth );
                        mStartPoint = k;
                        mTempWidth = ColumnsWidth[k];
                    }
                }
                // Our end point is actually one index above the current index
                mEndPoint = k + 1;
            }
        }
        // Add the last set of columns
        mColumnPoints.Add( new int[] { mStartPoint, mEndPoint } );
        mColumnPointsWidth.Add( mTempWidth );
        mColumnPoint = 0;
    }

    // The funtion that print the title, page number, and the header row
    private void DrawPageHeader( Graphics g ) {
        CurrentY = (float)TopMargin;

        // Printing the page number (if isWithPaging is set to true)
        if ( IsWithPaging ) {
            PageNumber++;
            string PageString = "Page " + PageNumber.ToString();
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
            RectangleF PageStringRectangle = new RectangleF( (float)LeftMargin, CurrentY,
                (float)PageWidth - (float)RightMargin - (float)LeftMargin, g.MeasureString( PageString, PageStringFont ).Height );
            RectangleF DateStringRectangle = new RectangleF( (float)LeftMargin, CurrentY,
                (float)PageWidth - (float)RightMargin - (float)LeftMargin, g.MeasureString( DateString, PageStringFont ).Height );

            g.DrawString( PageString, PageStringFont, new SolidBrush( Color.Black ), PageStringRectangle, PageStringFormat );
            g.DrawString( DateString, PageStringFont, new SolidBrush( Color.Black ), DateStringRectangle, DateStringFormat );

            CurrentY += g.MeasureString( PageString, PageStringFont ).Height;
        }

        // Printing the title (if IsWithTitle is set to true)
        if ( IsWithTitle ) {
            StringFormat TitleFormat = new StringFormat();
            TitleFormat.Trimming = StringTrimming.Word;
            TitleFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            if ( IsCenterOnPage )
                TitleFormat.Alignment = StringAlignment.Center;
            else
                TitleFormat.Alignment = StringAlignment.Near;

            RectangleF TitleRectangle = new RectangleF( (float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin, g.MeasureString( TheTitleText, TheTitleFont ).Height );
            g.DrawString( TheTitleText, TheTitleFont, new SolidBrush( TheTitleColor ), TitleRectangle, TitleFormat );

            CurrentY += g.MeasureString( TheTitleText, TheTitleFont ).Height;
        }

        // Printing the page number (if isWithPaging is set to true)
        if ( IsWithSubtitle ) {
            RectangleF SubtitleRectangle;
            if ( mySubtitleList.Count > 0 ) {
                float tmpRowHeight = 0, curRowHeight = 0;
                SizeF tmpSize = new SizeF();
                foreach ( StringRowPrinter curSubtitle in mySubtitleList ) {
                    tmpSize = g.MeasureString( curSubtitle.StringToPrint, curSubtitle.StringRowFont );
                    SubtitleRectangle = new RectangleF( curSubtitle.X + (float)LeftMargin, curSubtitle.Y + CurrentY, curSubtitle.Width, tmpSize.Height );
                    SolidBrush SubtitleBackground = new SolidBrush( curSubtitle.BackgroundColor );
                    g.FillRectangle( SubtitleBackground, SubtitleRectangle );
                    g.DrawString( curSubtitle.StringToPrint, curSubtitle.StringRowFont, new SolidBrush( curSubtitle.TextColor ), SubtitleRectangle, curSubtitle.StringRowFormat );
                    curRowHeight = curSubtitle.Y + tmpSize.Height;
                    if ( curRowHeight > tmpRowHeight ) { tmpRowHeight = curRowHeight; }
                }
                CurrentY += curRowHeight;
            }
        }

    }

    // The funtion that print the title, page number, and the header row
    private void DrawColHeader( Graphics g, DataGridView inDataGridView ) {
        // Calculating the starting x coordinate that the printing process will start from
        
        float CurrentX = (float)LeftMargin;
        if (IsCenterOnPage)            
            CurrentX += (((float)PageWidth - (float)RightMargin - (float)LeftMargin) - mColumnPointsWidth[mColumnPoint]) / 2.0F;

        // Setting the HeaderFore style
        Color HeaderForeColor = inDataGridView.ColumnHeadersDefaultCellStyle.ForeColor;
        if (HeaderForeColor.IsEmpty) // If there is no special HeaderFore style, then use the default DataGridView style
            HeaderForeColor = inDataGridView.DefaultCellStyle.ForeColor;
        SolidBrush HeaderForeBrush = new SolidBrush(HeaderForeColor);

        // Setting the HeaderBack style
        Color HeaderBackColor = inDataGridView.ColumnHeadersDefaultCellStyle.BackColor;
        if (HeaderBackColor.IsEmpty) // If there is no special HeaderBack style, then use the default DataGridView style
            HeaderBackColor = inDataGridView.DefaultCellStyle.BackColor;
        SolidBrush HeaderBackBrush = new SolidBrush(HeaderBackColor);

        // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
        Pen TheLinePen = new Pen(inDataGridView.GridColor, 1);

        // Setting the HeaderFont style
        Font HeaderFont = inDataGridView.ColumnHeadersDefaultCellStyle.Font;
        if (HeaderFont == null) // If there is no special HeaderFont style, then use the default DataGridView font style
            HeaderFont = inDataGridView.DefaultCellStyle.Font;

        // Calculating and drawing the HeaderBounds        
        RectangleF HeaderBounds = new RectangleF(CurrentX, CurrentY, mColumnPointsWidth[mColumnPoint], RowHeaderHeight);
        g.FillRectangle(HeaderBackBrush, HeaderBounds);

        // Setting the format that will be used to print each cell of the header row
        StringFormat CellFormat = new StringFormat();
        CellFormat.Trimming = StringTrimming.Word;
        CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;

        // Printing each visible cell of the header row
        RectangleF CellBounds;
        float ColumnWidth;
        for (int i = (int)mColumnPoints[mColumnPoint].GetValue(0); i < (int)mColumnPoints[mColumnPoint].GetValue(1); i++) {
            if (!inDataGridView.Columns[i].Visible) continue; // If the column is not visible then ignore this iteration

            ColumnWidth = ColumnsWidth[i];

            // Check the CurrentCell alignment and apply it to the CellFormat
            if (inDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right"))
                CellFormat.Alignment = StringAlignment.Far;
            else if (inDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center"))
                CellFormat.Alignment = StringAlignment.Center;
            else
                CellFormat.Alignment = StringAlignment.Near;

            CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowHeaderHeight);

            // Printing the cell text
            g.DrawString(inDataGridView.Columns[i].HeaderText, HeaderFont, HeaderForeBrush, CellBounds, CellFormat);

            // Drawing the cell bounds
            if (inDataGridView.RowHeadersBorderStyle != DataGridViewHeaderBorderStyle.None) // Draw the cell border only if the HeaderBorderStyle is not None
                g.DrawRectangle(TheLinePen, CurrentX, CurrentY, ColumnWidth, RowHeaderHeight);

            CurrentX += ColumnWidth;
        }

        CurrentY += RowHeaderHeight;
    }

    // The function that print a bunch of rows that fit in one page
    // When it returns true, meaning that there are more rows still not printed, 
    // so another PagePrint action is required
    // When it returns false, meaning that all rows are printed 
    // (the CureentRow parameter reaches the last row of the DataGridView control) 
    // and no further PagePrint action is required
    private bool DrawRows(Graphics g, DataGridView inDataGridView) {
        // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
        Pen TheLinePen = new Pen(inDataGridView.GridColor, 1);

        // The style paramters that will be used to print each cell
        Font RowFont;
        Color RowForeColor;
        Color RowBackColor;
        SolidBrush RowForeBrush;
        SolidBrush RowBackBrush;
        SolidBrush RowAlternatingBackBrush;

        // Setting the format that will be used to print each cell
        StringFormat CellFormat = new StringFormat();
        CellFormat.Trimming = StringTrimming.Word;
        CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

        // Printing each visible cell
        String curCellValue = "";
        RectangleF RowBounds;
        float CurrentX;
        float ColumnWidth;
        while (CurrentRow < inDataGridView.Rows.Count)
        {
            if (inDataGridView.Rows[CurrentRow].Visible) // Print the cells of the CurrentRow only if that row is visible
            {
                // Setting the row font style
                RowFont = inDataGridView.Rows[CurrentRow].DefaultCellStyle.Font;
                if (RowFont == null) // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
                    RowFont = inDataGridView.DefaultCellStyle.Font;

                // Setting the RowFore style
                RowForeColor = inDataGridView.Rows[CurrentRow].DefaultCellStyle.ForeColor;
                if (RowForeColor.IsEmpty) // If the there is no special RowFore style of the CurrentRow, then use the default one associated with the DataGridView control
                    RowForeColor = inDataGridView.DefaultCellStyle.ForeColor;
                RowForeBrush = new SolidBrush(RowForeColor);

                // Setting the RowBack (for even rows) and the RowAlternatingBack (for odd rows) styles
                RowBackColor = inDataGridView.Rows[CurrentRow].DefaultCellStyle.BackColor;
                if (RowBackColor.IsEmpty) // If the there is no special RowBack style of the CurrentRow, then use the default one associated with the DataGridView control
                {
                    RowBackBrush = new SolidBrush(inDataGridView.DefaultCellStyle.BackColor);
                    RowAlternatingBackBrush = new SolidBrush(inDataGridView.AlternatingRowsDefaultCellStyle.BackColor);
                }
                else // If the there is a special RowBack style of the CurrentRow, then use it for both the RowBack and the RowAlternatingBack styles
                {
                    RowBackBrush = new SolidBrush(RowBackColor);
                    RowAlternatingBackBrush = new SolidBrush(RowBackColor);
                }

                // Calculating the starting x coordinate that the printing process will start from
                CurrentX = (float)LeftMargin;
                if (IsCenterOnPage)                    
                    CurrentX += (((float)PageWidth - (float)RightMargin - (float)LeftMargin) - mColumnPointsWidth[mColumnPoint]) / 2.0F;

                // Calculating the entire CurrentRow bounds                
                RowBounds = new RectangleF(CurrentX, CurrentY, mColumnPointsWidth[mColumnPoint], RowsHeight[CurrentRow]);

                // Filling the back of the CurrentRow
                if (CurrentRow % 2 == 0)
                    g.FillRectangle(RowBackBrush, RowBounds);
                else
                    g.FillRectangle(RowAlternatingBackBrush, RowBounds);

                // Printing each visible cell of the CurrentRow                
                for (int CurrentCell = (int)mColumnPoints[mColumnPoint].GetValue(0); CurrentCell < (int)mColumnPoints[mColumnPoint].GetValue(1); CurrentCell++)
                {
                    if (!inDataGridView.Columns[CurrentCell].Visible) continue; // If the cell is belong to invisible column, then ignore this iteration

                    // Check the CurrentCell alignment and apply it to the CellFormat
                    if (inDataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Right"))
                        CellFormat.Alignment = StringAlignment.Far;
                    else if (inDataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Center"))
                        CellFormat.Alignment = StringAlignment.Center;
                    else
                        CellFormat.Alignment = StringAlignment.Near;

                    if (inDataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Top"))
                        CellFormat.LineAlignment = StringAlignment.Near;
                    else if (inDataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Middle"))
                        CellFormat.LineAlignment = StringAlignment.Center;
                    else
                        CellFormat.LineAlignment = StringAlignment.Far;

                    ColumnWidth = ColumnsWidth[CurrentCell];
                    RectangleF CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowsHeight[CurrentRow]);

                    if ( inDataGridView.Rows[CurrentRow].Cells[CurrentCell].GetType() == typeof( DataGridViewImageCell ) ) {
                        DataGridViewImageColumn curImageCell = (DataGridViewImageColumn)inDataGridView.Columns [CurrentCell];
                        if ( curImageCell != null ) {
                            if ( curImageCell.Image != null ) {
                                g.DrawImage( curImageCell.Image, CurrentX, CurrentY );
                            }
                        }
                    } else {
                        // Printing the cell text
                        curCellValue = inDataGridView.Rows[CurrentRow].Cells[CurrentCell].EditedFormattedValue.ToString();
                        if (curCellValue.IndexOf( "\\n" ) >= 0) {
                            curCellValue = "";
                            CurrentY = PageHeight - TopMargin - BottomMargin + 1;
                        }
                        g.DrawString( curCellValue, RowFont, RowForeBrush, CellBounds, CellFormat );
                    }

                    // Drawing the cell bounds
                    // Draw the cell border only if the CellBorderStyle is not None
                    if (inDataGridView.CellBorderStyle != DataGridViewCellBorderStyle.None) 
                        g.DrawRectangle(TheLinePen, CurrentX, CurrentY, ColumnWidth, RowsHeight[CurrentRow]);

                    CurrentX += ColumnWidth;
                }
                CurrentY += RowsHeight[CurrentRow];

                // Checking if the CurrentY is exceeds the page boundries
                // If so then exit the function and returning true meaning another PagePrint action is required
                if ((int)CurrentY > (PageHeight - TopMargin - BottomMargin)) {
                    CurrentRow++;
                    return true;
                }
            }
            CurrentRow++;
        }

        CurrentRow = 0;
        mColumnPoint++; // Continue to print the next group of columns

        if ( mColumnPoint == mColumnPoints.Count ) // Which means all columns are printed
        {
            mColumnPoint = 0;
            return false;
        } else
            return false;
            //return true;
    }

    // The method that calls all other functions
    public bool DrawDataGridView(Graphics inGraphicObj ) {
        try {
            DataGridView curView;
            RectangleF GridTitleRectangle;
            StringRowPrinter curGridTitle;
            bool curInProgress = true;
            bool curContinue = false;
            int curIdx;
            int curListSize = TheDataGridList.Length;
            float tmpRowHeight = 0, curRowHeight = 0;
            SizeF tmpSize = new SizeF();

            if ( TheActiveViewCount == 0 ) {
                TheActiveViewCount = 1;
            }

            DrawPageHeader( inGraphicObj );

            while ( curInProgress ) {
                curIdx = TheActiveViewCount - 1;
                curView = TheDataGridList[curIdx];

                if ( IsWithGridTitle ) {
                    if ( myGridViewTitleList.Count > curIdx ) {
                        myGridViewTitleIdx = curIdx;
                        curGridTitle = myGridViewTitleList[myGridViewTitleIdx];
                        tmpSize = inGraphicObj.MeasureString( curGridTitle.StringToPrint, curGridTitle.StringRowFont );
                        GridTitleRectangle = new RectangleF( curGridTitle.X + (float)LeftMargin, curGridTitle.Y + CurrentY, curGridTitle.Width, tmpSize.Height );
                        SolidBrush GridTitleBackground = new SolidBrush( curGridTitle.BackgroundColor );
                        inGraphicObj.FillRectangle( GridTitleBackground, GridTitleRectangle );
                        inGraphicObj.DrawString( curGridTitle.StringToPrint, curGridTitle.StringRowFont, new SolidBrush( curGridTitle.TextColor ), GridTitleRectangle, curGridTitle.StringRowFormat );
                        curRowHeight = curGridTitle.Y + tmpSize.Height;
                        if ( curRowHeight > tmpRowHeight ) { tmpRowHeight = curRowHeight; }
                        CurrentY += curRowHeight;
                    }
                } else {
                    if ( curIdx > 0 ) {
                        CurrentY += 15;
                    }
                }

                Calculate( inGraphicObj, curView );
                DrawColHeader( inGraphicObj, curView );

                curContinue = DrawRows( inGraphicObj, curView );
                if ( curContinue ) {
                    curInProgress = false;
                } else {
                    if ( TheActiveViewCount < curListSize ) {
                        TheActiveViewCount++;
                        curInProgress = true;
                    } else {
                        curInProgress = false;
                        TheActiveViewCount = 0;
                        PageNumber = 0;
                        CurrentRow = 0;
                    }
                }
            }

            return curContinue;

        } catch (Exception ex) {
            MessageBox.Show("Operation failed: " + ex.Message.ToString(), Application.ProductName + " - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
