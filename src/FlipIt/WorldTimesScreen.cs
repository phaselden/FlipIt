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
        private readonly bool _displayDstIndicator;

        private readonly Pen _smallSplitPen = new Pen(Color.Black, SplitWidth);
        private readonly Brush _backFillTop = new SolidBrush(BackColorTop);
        private readonly Pen _cityBoxPen = new Pen(BackColorBottom);

        private Font _largeFont;
        private Font _smallFont;

        private Font LargeFont => _largeFont ?? (_largeFont = new Font(FontFamily,  _boxSize.Height.Percent(80), FontStyle.Regular, GraphicsUnit.Pixel));
        private Font SmallFont => _smallFont ?? (_smallFont = new Font(FontFamily, _boxSize.Height.Percent(25), FontStyle.Regular, GraphicsUnit.Pixel));

        private const int DayIndicatorLength = 3;
        private const int MaxBoxHeight = 160;
        private const int SplitWidth = 2;
        private const int BoxWidthPercentage = 70;
        private const int HorizontalGapBetweenBoxesPercent = 5;
        private const int VerticalGapBetweenBoxesPercent = 10;

        private int _maxNameLengthInChars;
        private int _timeLengthInChars;  // including optional am/pm indicator and * for DST.
        private Size _boxSize;
        private int _horizontalGap;
        private int _verticalGap;
        private int _startingX;
        private int _startingY;

        public WorldTimesScreen(List<Location> cities, Form form, bool display24HourTime, bool displayDstIndicator)
        {
            _cities = cities;
            _form = form;
            _display24HourTime = display24HourTime;
            _displayDstIndicator = displayDstIndicator;

            CalculateTimeCharCount();
            CalculateLayout();
        }
        
        private void CalculateTimeCharCount()
        {
            // 12hr:    London  11:59:59 PM*
            // 24hr:    London  23:59:59*
            var timeLength = DateTime.Now.ToString("HH:mm").Length;
            _timeLengthInChars = timeLength;
            if (_displayDstIndicator)
                _timeLengthInChars += 1;
            if (!_display24HourTime)
                _timeLengthInChars += 3;
        }

        private void CalculateLayout()
        {
            _maxNameLengthInChars = _cities.Max(c => c.DisplayName.Length);
            var rowLengthInChars = _maxNameLengthInChars + 2 + _timeLengthInChars + 1 + DayIndicatorLength;

            var maxWidth = _form.Width - 40; // leave some margin
            var maxHeight = _form.Height - 40;
            
            var candidateBoxWidth = CalcBoxSize(maxWidth, 0, rowLengthInChars, HorizontalGapBetweenBoxesPercent/100.0);
            var candidateBoxHeight = CalcBoxSize(maxHeight, 0, _cities.Count, VerticalGapBetweenBoxesPercent/100.0);
            var boxHeightIfUsingWidth = candidateBoxWidth.PercentInv(BoxWidthPercentage);
            var boxHeight = Math.Min(candidateBoxHeight, boxHeightIfUsingWidth);
            boxHeight = Math.Min(boxHeight, MaxBoxHeight);
            _boxSize = new Size(boxHeight.Percent(BoxWidthPercentage), boxHeight);

            _horizontalGap = _boxSize.Height.Percent(HorizontalGapBetweenBoxesPercent);
            _verticalGap = _boxSize.Height.Percent(VerticalGapBetweenBoxesPercent);

            var heightForAllRows = CalcSize(_cities.Count, _boxSize.Height, _verticalGap);
            _startingY = (_form.Height - heightForAllRows) / 2;
            
            var rowWidth = CalcSize(rowLengthInChars, _boxSize.Width, _horizontalGap);
            _startingX = (_form.Width - rowWidth) / 2;
        }

        private int CalcBoxSize(int total, int borderPercent, int boxCount, double separatorFraction)
        {
            var borderSize = total.Percent(borderPercent);
            var remainingSpace = total - (borderSize * 2);
            var parts = (1 + separatorFraction) * boxCount - separatorFraction;
            return Convert.ToInt32(remainingSpace / parts);
        }

        internal override void Draw()
        {
            var y = _startingY;
            foreach (var city in _cities.OrderBy(c => c.CurrentTime))
            {
                city.RefreshTime(SystemTime.Now);
                var s = city.DisplayName.PadRight(_maxNameLengthInChars + 2) + FormatTime(city);
                DrawRow(_startingX, y, _boxSize, _horizontalGap, s);
                y += _boxSize.Height + _verticalGap;
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
                return "Error".PadRight(_timeLengthInChars) + new String(' ', DayIndicatorLength + 1);
            }

            var dst = "";
            if (_displayDstIndicator)
                dst = location.IsDaylightSavingTime ? "*" : " ";

            var daySuffix = location.DaysDifference != 0
                ? location.CurrentTime.ToString("ddd")
                : "   ";
            if (daySuffix.Length != DayIndicatorLength)
                daySuffix = FixStringLength(daySuffix, DayIndicatorLength);

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