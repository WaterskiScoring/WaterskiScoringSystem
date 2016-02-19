using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;

namespace WaterskiScoringSystem.Common {
    public class StringRowPrinter {
        private String myStringToPrint;
        private float myX;
        private float myY;
        private float myWidth;
        private float myHeight;
        private Color myTextColor;
        private Color myBackgroundColor;
        private Font myStringRowFont;
        private StringFormat myStringRowFormat;

        public StringRowPrinter() {
        }

        public StringRowPrinter(
            String inStringToPrint,
            float inX,
            float inY,
            float inWidth,
            float inHeight,
            Color inTextColor,
            Color inBackgroundColor,
            Font inStringRowFont,
            StringFormat inStringRowFormat
            ) {
            myStringToPrint = inStringToPrint;
            myX = inX;
            myY = inY;
            myWidth = inWidth;
            myHeight = inHeight;
            myTextColor = Color.Black;
            myBackgroundColor = inBackgroundColor;
            myStringRowFont = inStringRowFont;
            myStringRowFormat = inStringRowFormat;
        }
        public String StringToPrint {
            get {
                return myStringToPrint;
            }
            set {
                myStringToPrint = value;
            }
        }

        public StringFormat StringRowFormat {
            get {
                return myStringRowFormat;
            }
            set {
                myStringRowFormat = value;
            }
        }

        public Font StringRowFont {
            get {
                return myStringRowFont;
            }
            set {
                myStringRowFont = value;
            }
        }

        public float X {
            get {
                return myX;
            }
            set {
                myX = value;
            }
        }

        public float Y {
            get {
                return myY;
            }
            set {
                myY = value;
            }
        }

        public float Width {
            get {
                return myWidth;
            }
            set {
                myWidth = value;
            }
        }

        public float Height {
            get {
                return myHeight;
            }
            set {
                myHeight = value;
            }
        }

        public Color TextColor {
            get {
                return myTextColor;
            }
            set {
                myTextColor = value;
            }
        }

        public Color BackgroundColor {
            get {
                return myBackgroundColor;
            }
            set {
                myBackgroundColor = value;
            }
        }

    }
}
