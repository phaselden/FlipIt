using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScreenSaver
{
    internal class CurrentTimeScreen : TimeScreen
    {
        private readonly bool _display24HourTime;
        private readonly bool _isPreviewMode;
        private readonly int _fontSize = 350;
        private readonly bool _showSeconds = false;

        private const int SplitWidth = 4;
        private Font _largeFont;
        private Font _smallFont;

        private Font LargeFont => _largeFont ?? (_largeFont = new Font(FontFamily, _fontSize, FontStyle.Bold));
        private Font SmallFont => _smallFont ?? (_smallFont = new Font(FontFamily, _fontSize / 9, FontStyle.Bold));

        private readonly Brush _backFillTop = new SolidBrush(BackColorTop);
        private readonly Brush _backFillBottom = new SolidBrush(BackColorBottom);
        private readonly Brush _fontBrush = new SolidBrush(Color.FromArgb(255, 183, 183, 183));
        private readonly Pen _splitPen = new Pen(Color.Black, SplitWidth);
        

        public CurrentTimeScreen(Form form, bool display24HourTime, bool isPreviewMode, int fontSize)
        {
            _display24HourTime = display24HourTime;
            _isPreviewMode = isPreviewMode;
            _form = form;
            _fontSize = fontSize;
        }

        // protected override PrivateFontCollection InitFontCollection()
        // {
        //     var pfc = new PrivateFontCollection();
        //     AddFont(pfc, Properties.Resources.HelveticaLTStd_BoldCond);
        //     return pfc;
        // }

        protected override byte[] GetFontResource()
        {
            return Properties.Resources.HelveticaLTStd_BoldCond;
        }
        
        internal override void Draw()
        {
            var height = LargeFont.Height * 10 / 10;
            var width = !_showSeconds ? Convert.ToInt32(2.05 * height) : Convert.ToInt32(3.1 * height);

            var x = (_form.Width - width) / 2;
            var y = (_form.Height - height) / 2;

            if (!_display24HourTime)
            {
                var pm = SystemTime.Now.Hour >= 12;
                DrawIt(x, y, height, SystemTime.Now.ToString("%h"), pm ? null : "AM", pm ? "PM" : null); // The % avoids a FormatException https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx#UsingSingleSpecifiers
            }
            else
            {
                DrawIt(x, y, height, SystemTime.Now.ToString("HH"));
            }

            x += height + (height / 20);
            DrawIt(x, y, height, SystemTime.Now.ToString("mm"));

            if (_showSeconds)
            {
                x += height + (height / 20);
                DrawIt(x, y, height, SystemTime.Now.ToString("ss"));
            }
        }

        private void DrawIt(int x, int y, int size, string s, string topString = null, string bottomString = null)
        {
            // Draw the background
            var diff = size / 10;
            var textRect = new Rectangle(x - diff, y + diff / 2, size + diff * 2, size);

            var radius = size / 20;
            var diameter = radius * 2;
            Gfx.FillEllipse(_backFillTop, x, y, diameter, diameter); // top left
            Gfx.FillEllipse(_backFillTop, x + size - diameter, y, diameter, diameter); // top right
            Gfx.FillEllipse(_backFillBottom, x, y + size - diameter, diameter, diameter); // bottom left
            Gfx.FillEllipse(_backFillBottom, x + size - diameter, y + size - diameter, diameter, diameter); //bottom right

            Gfx.FillRectangle(_backFillTop, x + radius, y, size - diameter, diameter);
            Gfx.FillRectangle(_backFillBottom, x + radius, y + size - diameter, size - diameter, diameter);

            var linGrBrush = new LinearGradientBrush(
                new Point(10, y + radius),
                new Point(10, y + size - radius),
                BackColorTop,
                BackColorBottom);
            Gfx.FillRectangle(linGrBrush, x, y + radius, size, size - diameter);
            linGrBrush.Dispose();

            //			if (s.Length == 1)
            //			{
            //				s = "\u2002" + s; // Add an EN SPACE which is 1/2 em
            //			}

            // Draw the text
            var stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            Gfx.DrawString(s, LargeFont, _fontBrush, textRect, stringFormat);

            if (topString != null)
            {
                //Gfx.DrawString(bottomString, SmallFont, FontBrush, textRect.X, textRect.Bottom - SmallFont.Height);
                Gfx.DrawString(topString, SmallFont, _fontBrush, x + diameter, y + diameter);
            }
            if (bottomString != null)
            {
                Gfx.DrawString(bottomString, SmallFont, _fontBrush, x + diameter, y + size - diameter - SmallFont.Height);
            }

            // Horizontal dividing line
            if (!_isPreviewMode)
            {
                var penY = y + (size / 2) - (SplitWidth / 2);
                Gfx.DrawLine(_splitPen, x, penY, x + size, penY);
            }
            else
            {
                Gfx.DrawLine(Pens.Black, x, y + (size / 2), x + size, y + (size / 2));
            }
        }
    }
}