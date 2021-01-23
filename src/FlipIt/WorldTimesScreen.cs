using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScreenSaver
{
    internal class WorldTimesScreen : TimeScreen
    {
        private readonly List<City> _cities;

        private readonly Pen _smallSplitPen = new Pen(Color.Black, SplitWidth);
        private readonly Brush _backFillTop = new SolidBrush(BackColorBottom);
        private readonly Pen _cityBoxPen = new Pen(BackColorBottom);

        private Font _largeFont;
        private Font _smallFont;

        private int _boxHeight;

        private Font LargeFont => _largeFont ?? (_largeFont = new Font(FontFamily, _boxHeight.Percent(80), FontStyle.Regular, GraphicsUnit.Pixel));
        private Font SmallFont => _smallFont ?? (_smallFont = new Font(FontFamily, _boxHeight.Percent(25), FontStyle.Regular, GraphicsUnit.Pixel));

        private const DstIndicatorStyle CityDisplayDstIndicatorStyle = DstIndicatorStyle.DstAsAsterisk; // .SmallDst;
        private const int SplitWidth = 2;
        private const int BoxWidthPercentage = 70;
        private const int HorizontalGapBetweenBoxesPercent = 5;
        private const int VerticalGapBetweenBoxesPercent = 10;
        private const int TimeLengthInChars = 9;

        public WorldTimesScreen(List<City> cities, Form form)
        {
            _cities = cities;
            _form = form;
        }

        internal override void Draw()
        {
            const int dstIndicatorLength = 1;
            const int dayIndicatorLength = 3;
            const int maxBoxHeight = 160;

            var maxNameLengthInChars = _cities.Max(c => c.DisplayName.Length);

            var maxRowLengthInChars = CityDisplayDstIndicatorStyle == DstIndicatorStyle.SmallDst
                ? maxNameLengthInChars + 2 + TimeLengthInChars + 1 + dstIndicatorLength
                : maxNameLengthInChars + 2 + TimeLengthInChars + 1 + dayIndicatorLength;

            var maxWidth = _form.Width - 40; // leave some margin
            var maxHeight = _form.Height - 40;

            var roughBoxWidth = maxWidth / maxRowLengthInChars; // the rough max width of boxes
            var roughBoxHeight = maxHeight / _cities.Count;
            var boxHeight = Math.Min(roughBoxHeight, roughBoxWidth.PercentInv(BoxWidthPercentage));
            boxHeight -= boxHeight.Percent(VerticalGapBetweenBoxesPercent);
            _boxHeight = Math.Min(boxHeight, maxBoxHeight);

            var verticalGap = boxHeight.Percent(VerticalGapBetweenBoxesPercent);

            //var heightForAllRows = (boxHeight + verticalGap) * cities.Count - verticalGap;
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
                var s = city.DisplayName.PadRight(maxNameLengthInChars + 2) + FormatTime(city, CityDisplayDstIndicatorStyle == DstIndicatorStyle.DstAsAsterisk);
                DrawRow(startingX, y, boxSize, horizontalGap, s, city, CityDisplayDstIndicatorStyle);
                y += boxHeight + verticalGap;
            }
        }

        private void DrawRow(int x, int y, Size boxSize, int horizontalGap, string s, City city, DstIndicatorStyle dstIndicatorStyle)
        {
            var boxRectangle = new Rectangle(new Point(x, y), boxSize);
            foreach (var c in s.ToUpperInvariant())
            {
                DrawCharInBox(boxRectangle, c);
                boxRectangle.X = boxRectangle.Right + horizontalGap;
            }

            if (dstIndicatorStyle == DstIndicatorStyle.SmallDst)
            {
                if (city.IsDaylightSavingTime || city.DaysDifference != 0)
                {
                    DrawSmallStringsInBox(boxRectangle,
                        city.IsDaylightSavingTime ? "DST" : null,
                        city.DaysDifference != 0 ? $"{city.DaysDifference:+#;-#}d" : null);
                }
                else
                {
                    DrawCharInBox(boxRectangle, ' ');
                }
            }
            else
            {
                DrawStringInBoxes(boxRectangle, horizontalGap,
                    city.DaysDifference != 0
                        ? " " + city.CurrentTime.ToString("ddd")
                        : "    ");
            }
        }

        private int DrawStringInBoxes(Rectangle boxRectangle, int horizontalGap, string s)
        {
            foreach (var c in s.ToUpperInvariant())
            {
                DrawCharInBox(boxRectangle, c);
                boxRectangle.X = boxRectangle.Right + horizontalGap;
            }
            return boxRectangle.X;
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

        private string FormatTime(City city, bool appendAsteriskIfDst)
        {
            var suffix = appendAsteriskIfDst && city.IsDaylightSavingTime ? "*" : " ";
            var result = $"{city.CurrentTime:h:mm tt}{suffix}";
            return $"{result,TimeLengthInChars}";  // right aligned in 9 chars
        }

        protected override byte[] GetFontResource()
        {
            return Properties.Resources.HelveticaLTStd_Cond;
        }
    }
}