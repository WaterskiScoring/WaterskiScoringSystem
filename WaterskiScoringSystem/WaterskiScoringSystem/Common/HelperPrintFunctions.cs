using System;
using System.Drawing;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
	class HelperPrintFunctions {
		public static Single[] PrintText( Graphics inGraphicControl, String inText, RectangleF inBox, Font inFont, Brush inBrush, StringFormat inFormat ) {
			inGraphicControl.DrawString( inText, inFont, inBrush, inBox, inFormat );
			return new Single[2] { inBox.X + inBox.Width, inBox.Y + inBox.Height };
		}
		public static Single[] PrintText( Graphics inGraphicControl, String inText, Single inTextLocX, Single inTextLocY, Single inTextWidth, Single inTextHeight, Single inPageAdjX, Single inPageAdjY, Font inFont, Brush inBrush, StringFormat inFormat ) {
			Single curPosX = inPageAdjX + inTextLocX;
			Single curPosY = inPageAdjY + inTextLocY;
			SizeF curTextSize = StringSize( inGraphicControl, inText, inFont );
			Single curBoxHeight = curTextSize.Height;
			if ( curBoxHeight < inTextHeight ) curBoxHeight = inTextHeight;
			Single curBoxWidth = curTextSize.Width;
			if ( curBoxWidth < inTextWidth ) curBoxWidth = inTextWidth;
			RectangleF curBox = new RectangleF( curPosX, curPosY, curBoxWidth, curBoxHeight );
			//inGraphicControl.DrawRectangle( Pens.Red, curPosX, curPosY, curBoxWidth, curBoxHeight ); 
			inGraphicControl.DrawString( inText, inFont, inBrush, curBox, inFormat );
			return new Single[2] { curPosX + curBoxWidth, curPosY + curBoxHeight };
		}
		public static Single[] PrintText( Graphics inGraphicControl, String inText, Single inTextLocX, Single inTextLocY, Single inPageAdjX, Single inPageAdjY, Font inFont, Brush inBrush, bool isVAlignCenter, HorizontalAlignment inHAlign ) {
			Single curPosX = inPageAdjX + inTextLocX;
			Single curPosY = inPageAdjY + inTextLocY;
			SizeF curTextSize = inGraphicControl.MeasureString( inText, inFont );
			int curWidth = Convert.ToInt32( curTextSize.Width );
			int curHeight = Convert.ToInt32( curTextSize.Height );

			// Print Text
			if ( isVAlignCenter ) {
				curPosY += 2;
				curHeight = curHeight + 4;
			}

			PointF curPosition = new PointF( curPosX, curPosY );
			inGraphicControl.DrawString( inText, inFont, inBrush, curPosition );

			return new Single[2] { curPosX + curWidth, curPosY + curHeight };
		}

		public static Single FontHeight( Graphics inGraphicControl, Font font ) {
			return font.GetHeight( inGraphicControl );
		}
		
		public static SizeF StringSize( Graphics inGraphicControl, String inText, Font inFont ) {
			return inGraphicControl.MeasureString( inText, inFont );
		}

		public static StringFormat BuildFormat( HorizontalAlignment hAlignment ) {
			StringFormat drawFormat = new StringFormat();
			switch ( hAlignment ) {
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

	}
}
