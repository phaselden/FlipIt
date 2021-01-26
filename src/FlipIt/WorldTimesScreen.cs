using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScreenSaver
{
    internal class WorldTimesScreen : TimeScreen
    {
        private readonly List<Location> _cities;
        private readonly bool _display24HourTime;

        private readonly Pen _smallSplitPen = new Pen(Color.Black, SplitWidth);
        private readonly Brush _backFillTop = new SolidBrush(BackColorBottom);
        private readonly Pen _cityBoxPen = new Pen(BackColorBottom);

        private Font _largeFont;
        private Font _smallFont;

        private int _boxHeight;

        private Font LargeFont => _largeFont ?? (_largeFont = new Font(FontFamily, _boxHeight.Percent(80), FontStyle.Regular, GraphicsUnit.Pixel));
        private Font SmallFont => _smallFont ?? (_smallFont = new Font(FontFamily, _boxHeight.Percent(25), FontStyle.Regular, GraphicsUnit.Pixel));

        private const int SplitWidth = 2;
        private const int BoxWidthPercentage = 70;
        private const int HorizontalGapBetweenBoxesPercent = 5;
        private const int VerticalGapBetweenBoxesPercent = 10;
        
        private readonly int _timeLengthInChars;  // including optional am/pm indicator and * for DST.

        public WorldTimesScreen(List<Location> cities, Form form, bool display24HourTime)
        {
            _cities = cities;
            _form = form;
            _display24HourTime = display24HourTime;

            //var testTime = new DateTime(2000, 1, 1, 23, 59, 59);
            var timeLength = DateTime.Now.ToString("HH:mm").Length;
            
            // 12hr:    London  11:59:59 PM*
            // 24hr:    London  23:59:59*
            _timeLengthInChars = timeLength + 1;
            if (!_display24HourTime)
                _timeLengthInChars += 3;
        }

        internal override void Draw()
        {
            const int dayIndicatorLength = 3;
            const int maxBoxHeight = 160;

            var maxNameLengthInChars = _cities.Max(c => c.DisplayName.Length);

            var maxRowLengthInChars = maxNameLengthInChars + 2 + _timeLengthInChars + 1 + dayIndicatorLength;

            var maxWidth = _form.Width - 40; // leave some margin
            var maxHeight = _form.Height - 40;

            var roughBoxWidth = maxWidth / maxRowLengthInChars; // the rough max width of boxes
            var roughBoxHeight = maxHeight / _cities.Count;
            var boxHeight = Math.Min(roughBoxHeight, roughBoxWidth.PercentInv(BoxWidthPercentage));
            boxHeight -= boxHeight.Percent(VerticalGapBetweenBoxesPercent);
            _boxHeight = Math.Min(boxHeight, maxBoxHeight);

            var verticalGap = boxHeight.Percent(VerticalGapBetweenBoxesPercent);

            var heightForAllRows = CalcSize(_cities.Count, boxHeight, verticalGap);
            var y = (_form.Height - heightForAllRows) / 2;
            if (y < 20)
            {
                y = 20;
            }
            var startingX = (_form.Width - maxRowLengthInChars * boxHeight.Percent(BoxWidthPercentage)).Percent(50);

            var boxSize = new Size(boxHeight.Percent(BoxWidthPercentage), boxHeight);
            var horizontalGap = boxSize.Height.Percent(HorizontalGapBetweenBoxesPercent);

            foreach (var city in _cities.OrderBy(c => c.CurrentTime))
            {
                city.RefreshTime(SystemTime.Now);
                var s = city.DisplayName.PadRight(maxNameLengthInChars + 2) + FormatTime(city);
                DrawRow(startingX, y, boxSize, horizontalGap, s);
                y += boxHeight + verticalGap;
            }
        }

        private void DrawRow(int x, int y, Size boxSize, int horizontalGap, string s)
        {
            var boxRectangle = new Rectangle(new Point(x, y), boxSize);
            foreach (var c in s.ToUpperInvariant())
            {
                DrawCharInBox(boxRectangle, c);
                boxRectangle.X = boxRectangle.Right + horizontalGap;
            }
        }

        private void DrawCharInBox(Rectangle boxRectangle, char theChar)
        {
            DrawBox(boxRectangle);
            DrawString(theChar.ToString(), LargeFont, boxRectangle);
            DrawBoxSplitter(boxRectangle);
        }

        private void DrawSmallStringsInBox(Rectangle boxRectangle, string top, string bottom)
        {
            DrawBox(boxRectangle, top, bottom);
            DrawBoxSplitter(boxRectangle);
        }

        private void DrawBoxSplitter(Rectangle box)
        {
            var penY = box.Y + (box.Height / 2) - (SplitWidth / 2);
            Gfx.DrawLine(_smallSplitPen, box.Left, penY, box.Right + 1, penY);
        }

        private void DrawBox(Rectangle box)
        {
            var radius = box.Height / 20;

            var path = RoundedRectangle.Create(box, radius);
            Gfx.DrawPath(_cityBoxPen, path);
            Gfx.FillPath(_backFillTop, path);
        }

        private void DrawBox(Rectangle box, string topString, string bottomString)
        {
            var radius = box.Height / 20;
            var halfRectangle = new Rectangle(box.X, box.Y, box.Width, box.Height / 2);

            DrawHalfBox(halfRectangle, radius, RectangleCorners.TopLeft | RectangleCorners.TopRight, topString);

            // Move the rect down
            halfRectangle.Y = halfRectangle.Y + halfRectangle.Height + 1;

            DrawHalfBox(halfRectangle, radius, RectangleCorners.BottomLeft | RectangleCorners.BottomRight, bottomString);
        }

        private void DrawHalfBox(Rectangle halfRectangle, int radius, RectangleCorners corners, string s)
        {
            var path = RoundedRectangle.Create(halfRectangle, radius, corners);
            Gfx.DrawPath(_cityBoxPen, path);
            Gfx.FillPath(_backFillTop, path);
            if (!String.IsNullOrEmpty(s))
            {
                DrawString(s, SmallFont, halfRectangle);
            }
        }

        private void DrawString(string s, Font font, Rectangle box)
        {
            var stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // Hacky adjustment to center the text in the box
            var yOffset = box.Height.Percent(5);

            var textRect = new Rectangle(box.X, box.Y + yOffset, box.Width, box.Height);
            Gfx.DrawString(s, font, FontBrush, textRect, stringFormat);
        }

        private int CalcSize(int itemCount, int itemSize, int gapSize)
        {
            return (itemCount * (itemSize + gapSize)) - gapSize;
        }

        private string FormatTime(Location location)
        {
            // 12hr:    London  11:59:59 PM* MON
            // 24hr:    London  23:59:59* MON

            var formatString = $"{{0, {_timeLengthInChars}}}";

            if (location.HasError)
            {
                return "Error".PadRight(_timeLengthInChars) + "    ";
                //return String.Format(formatString, "   Error") + "    "; // 4 spaces for day indicator
            }

            var dst = location.IsDaylightSavingTime ? "*" : " ";

            var daySuffix = location.DaysDifference != 0
                ? location.CurrentTime.ToString("ddd")
                : "   ";
            if (daySuffix.Length != 3)
                daySuffix = FixStringLength(daySuffix, 3);

            var timePart = !_display24HourTime 
                ? $"{location.CurrentTime:h:mm} {FormatAmPm(location.CurrentTime)}{dst}" 
                : $"{location.CurrentTime:HH:mm}{dst}";

            return String.Format(formatString, timePart) + " " + daySuffix;
            
        }

        private string FixStringLength(string s, int length, bool padRight = true)
        {
            if (s == null)
                return new string(' ', length);
            if (s.Length == length)
                return s;
            return s.Length > length 
                ? s.Substring(0, length) 
                : padRight ? s.PadRight(length) : s.PadLeft(length);
        }

        protected override byte[] GetFontResource()
        {
            return Properties.Resources.HelveticaLTStd_Cond;
        }
    }
}