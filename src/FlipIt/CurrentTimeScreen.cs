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
        private readonly bool _showSeconds = false;

        private const int SplitWidth = 4;
        private const double BoxSeparationPercent = 0.05; // ie. 5%

        private Font _largeFont;
        private Font _smallFont;

        private Font LargeFont => _largeFont ?? (_largeFont = new Font(FontFamily, _boxSize.Percent(85), FontStyle.Bold, GraphicsUnit.Pixel));
        private Font SmallFont => _smallFont ?? (_smallFont = new Font(FontFamily, _boxSize.Percent(9), FontStyle.Bold, GraphicsUnit.Pixel));

        private readonly Brush _fontBrush = new SolidBrush(Color.FromArgb(255, 183, 183, 183));
        private readonly Pen _splitPen = new Pen(Color.Black, SplitWidth);

        private readonly int _boxSize;
        private readonly int _separatorWidth;
        private readonly int _startingX;
        private readonly int _startingY;

        private const bool DrawGuideLines = false;


        public CurrentTimeScreen(Form form, bool display24HourTime, bool isPreviewMode, int scalePercent)
        {
            _display24HourTime = display24HourTime;
            _isPreviewMode = isPreviewMode;
            _form = form;
            
            // The border is between 5% and 30% of the screen
            //  * A scale of 0 = 5% 
            //  * A scale of 100 = 30%
            var borderPercent = (100 - scalePercent) / 4 + 5;
            
            var boxSizeWidth = CalcBoxSize(form.Width, borderPercent, 2);
            var boxSizeHeight = CalcBoxSize(form.Height, borderPercent, 1);
            
            _boxSize = Math.Min(boxSizeWidth, boxSizeHeight);
            _separatorWidth = Convert.ToInt32(_boxSize * BoxSeparationPercent);

            _startingX = CalcOffset(form.Width, 2, _boxSize, _separatorWidth);
            _startingY = CalcOffset(form.Height, 1, _boxSize, 0);
        }

        private int CalcBoxSize(int total, int borderPercent, int boxCount)
        {
            var borderSize = total.Percent(borderPercent);
            var remainingSpace = total - (borderSize * 2);
            var parts = (1 + BoxSeparationPercent) * boxCount - BoxSeparationPercent;
            return Convert.ToInt32(remainingSpace / parts);
        }

        private int CalcOffset(int total, int boxCount, int boxSize, int seperatorSize )
        {
            var sizeOfAllBoxes = (boxSize + seperatorSize) * boxCount - seperatorSize;
            return (total - sizeOfAllBoxes) / 2;
        }
        
        protected override byte[] GetFontResource()
        {
            return Properties.Resources.HelveticaLTStd_BoldCond;
        }
        
        internal override void Draw()
        {
            var boxRect = new Rectangle(_startingX, _startingY, _boxSize, _boxSize);

            if (!_display24HourTime)
            {
                var pm = SystemTime.Now.Hour >= 12;
                DrawIt(boxRect, SystemTime.Now.ToString("%h"), pm ? null : "AM", pm ? "PM" : null); // The % avoids a FormatException https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx#UsingSingleSpecifiers
            }
            else
            {
                DrawIt(boxRect, SystemTime.Now.ToString("HH"));
            }

            boxRect.X += _boxSize + _separatorWidth;
            DrawIt(boxRect, SystemTime.Now.ToString("mm"));

            if (_showSeconds)
            {
                boxRect.X += _boxSize + _separatorWidth;
                DrawIt(boxRect, SystemTime.Now.ToString("ss"));
            }
        }

        private void DrawIt(Rectangle rect, string s, string topString = null, string bottomString = null)
        {
            DrawBox(rect);
            DrawTextInRect(rect, s, topString, bottomString);
        }
        
        private void DrawBox(Rectangle rect)
        {
            var radius = rect.Width / 20;
            using (var path = RoundedRectangle.Create(rect, radius))
            using (var brush = new LinearGradientBrush(rect,
                BackColorTop,
                BackColorBottom, LinearGradientMode.Vertical))
            {
                Gfx.FillPath(brush, path);
            }
        }
        
        private void DrawTextInRect(Rectangle rect, string s, string topString = null, string bottomString = null)
        {
            var diff = rect.Width / 10;
            
            // Some hacky adjustments to center the text in the box
            var xOffset = rect.Width.Percent(1);
            var yOffset = rect.Height.Percent(4);
            
            var textRect = new Rectangle(rect.Left - diff + xOffset, rect.Y + yOffset, rect.Width + diff * 2, rect.Height);

            if (DrawGuideLines)
            {
                Gfx.DrawRectangle(Pens.Red, textRect);
            }
            
            // Draw the text
            var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center, 
                LineAlignment = StringAlignment.Center,
                //FormatFlags = StringFormatFlags.NoWrap
            };

            Gfx.DrawString(s, LargeFont, _fontBrush, textRect, stringFormat);

            if (topString != null)
            {
                var leftOffset = diff / 2;
                Gfx.DrawString(topString, SmallFont, _fontBrush, rect.X + leftOffset, rect.Y + diff);
            }
            if (bottomString != null)
            {
                var leftOffset = diff / 2;
                Gfx.DrawString(bottomString, SmallFont, _fontBrush, rect.X + leftOffset, rect.Bottom - diff - SmallFont.Height);
            }

            // Horizontal dividing line
            if (!_isPreviewMode)
            {
                var y = rect.Y + (rect.Height / 2) - (SplitWidth / 2);
                Gfx.DrawLine(_splitPen, rect.Left, y, rect.Right, y);
            }
            else
            {
                var y = rect.Y + (rect.Height / 2);
                Gfx.DrawLine(Pens.Black, rect.Left, y, rect.Right, y);
            }
        }
    }
}