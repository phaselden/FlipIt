using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenSaver
{
    internal abstract class TimeScreen
    {
        protected Form _form;
        private Graphics _graphics;
        private PrivateFontCollection _pfc = null;
        private FontFamily _fontFamily = null;

        protected abstract byte[] GetFontResource();

        internal abstract void Draw();

        protected Graphics Gfx
        {
            get
            {
                if (_graphics == null)
                {
                    _graphics = _form.CreateGraphics();
                    _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    _graphics.SmoothingMode = SmoothingMode.HighQuality;
                }
                return _graphics ;
            }
        }

        protected FontFamily FontFamily
        {
            get
            {
                if (_fontFamily == null)
                {
                    if (_pfc == null)
                    {
                        _pfc = InitFontCollection();
                    }
                    _fontFamily = _pfc.Families[0];
                }
                return _fontFamily ?? (_fontFamily = _pfc.Families[0]);
            }
        }

        // Experimental
        protected int GetFontAscentPercent()
        {
            var ascent = FontFamily.GetCellAscent(FontStyle.Regular);
            var all =  FontFamily.GetEmHeight(FontStyle.Regular);
            return ascent * 100 / all;
        }
        
        private PrivateFontCollection InitFontCollection()
        {
            // We don't add both fonts at the same time because I can only get the private font collection
            // to return the first one we add. If the first one is the non-bold one and we ask for a bold one
            // then it seems to have an (inadequate) go at generating bold rather than using the one we gave it.
            // The system font collection does not seem to have this problem.
            // protected abstract PrivateFontCollection InitFontCollection();

            var pfc = new PrivateFontCollection();
            AddFont(pfc, GetFontResource());
            return pfc;
        }
        
        protected static readonly Color BackColorTop = Color.FromArgb(255, 18, 18, 18);
        protected static readonly Color BackColorBottom = Color.FromArgb(255, 10, 10, 10);
        protected static readonly Brush FontBrush = new SolidBrush(Color.FromArgb(255, 183, 183, 183));
        
        protected static void AddFont(PrivateFontCollection pfc, byte[] fontResource)
        {
            IntPtr ptr = Marshal.AllocCoTaskMem(fontResource.Length);  // create an unsafe memory block for the font data
            Marshal.Copy(fontResource, 0, ptr, fontResource.Length);  // copy the bytes to the unsafe memory block
            pfc.AddMemoryFont(ptr, fontResource.Length);    // pass the font to the font collection
            Marshal.FreeCoTaskMem(ptr);
        }

        protected string FormatAmPm(DateTime time)
        {
            // We format this ourselves because some cultures, such as nl-NL produce, results longer than 2 chars. ie. "a.m."
            // and we want to be consistent with the current time screen
            return time.Hour >= 12 ? "PM" : "AM";
        }

        internal void DisposeResources()
        {
            if (_fontFamily != null)
            {
                _fontFamily.Dispose();
                _fontFamily = null;
            }
            if (_pfc != null)
            {
                _pfc.Dispose();
                _pfc = null;
            }
        }
    }
}