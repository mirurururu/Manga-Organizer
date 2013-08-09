using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Nagru___Manga_Organizer
{
    /* Inspiration: Andrey Butov (Dec 20, 2004) */
    public class StarRatingControl : Control
    {
        #region Properties
        protected readonly Pen pnOutline;
        protected readonly GraphicsPath gpStar;
        protected readonly Rectangle[] rcStarAreas;
        protected readonly int iPadding = 8;
        protected readonly int iStarWidth = 16;
        protected readonly int iStarHeight = 14;
        protected readonly Color cOutline = Color.DarkGray;
        protected readonly Color cHover = Color.Yellow;
        protected readonly Color cSelected = Color.Goldenrod;
        
		protected bool IsHovering { get; private set; }
		protected int HoverStar { get; set; }

        protected int iSelStar = 0;
        public int SelectedStar {
            get { return iSelStar; }
            set {
                if (iSelStar != value) {
                    iSelStar = value;
                    Invalidate();
                }
            }
        }

        protected int iOutThick = 1;
        public int OutlineThickness {
            get { return iOutThick; }
            set {
                if (iOutThick != value) {
                    iOutThick = value;
                    Invalidate();
                }
            }
        }
        #endregion

        public StarRatingControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            pnOutline = new Pen(cOutline, OutlineThickness);
            rcStarAreas = new Rectangle[5];
            for (int i = 0; i < 5; ++i) {
                rcStarAreas[i].X = i * (iStarWidth + iPadding);
                rcStarAreas[i].Width = iStarWidth + iPadding;
                rcStarAreas[i].Height = iStarHeight;
            }

            gpStar = new GraphicsPath();
            gpStar.AddLines(new PointF[] {
                new PointF(iStarWidth / 2, 0),
                new PointF(42 * iStarWidth / 64, 19 * iStarHeight / 64),
                new PointF(iStarWidth, 22 * iStarHeight / 64),
                new PointF(48 * iStarWidth / 64, 38 * iStarHeight / 64),
                new PointF(52 * iStarWidth / 64, iStarHeight),
                new PointF(iStarWidth / 2, 52 * iStarHeight / 64),
                new PointF(12 * iStarWidth / 64, iStarHeight),
                new PointF(iStarWidth / 4, 38 * iStarHeight / 64),
                new PointF(0, 22 * iStarHeight / 64),
                new PointF(22 * iStarWidth / 64, 19 * iStarHeight / 64)
            });
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.Clear(BackColor);

            Brush fillBrush;
            Rectangle drawArea = new Rectangle(0, 0, iStarWidth, iStarHeight);
            for (int i = 0; i < 5; ++i) {
                if (IsHovering && HoverStar > i) {
                    fillBrush = new LinearGradientBrush(drawArea, cHover, BackColor,
                        LinearGradientMode.ForwardDiagonal);
                }
                else if (!IsHovering && SelectedStar > i) {
                    fillBrush = new LinearGradientBrush(drawArea, cSelected, BackColor,
                        LinearGradientMode.ForwardDiagonal);
                }
                else fillBrush = new SolidBrush(BackColor);
                
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                GraphicsPath gpTmp = GetPath(gpStar, drawArea.X, drawArea.Y);
                drawArea.X += drawArea.Width + iPadding;
                pe.Graphics.FillPath(fillBrush, gpTmp);
                pe.Graphics.DrawPath(pnOutline, gpTmp);
                gpTmp.Dispose();
            }
            base.OnPaint(pe);
        }

        public static GraphicsPath GetPath(GraphicsPath gpObj, int iX, int iY)
        {
            GraphicsPath clone = (GraphicsPath)gpObj.Clone();
            Matrix mat = new Matrix();
            mat.Translate(iX, iY);
            clone.Transform(mat);
            return clone;
        }

        protected override void OnMouseEnter(System.EventArgs ea)
        {
            IsHovering = true;
            Invalidate();
            base.OnMouseEnter(ea);
        }

        protected override void OnMouseLeave(System.EventArgs ea)
        {
            IsHovering = false;
            Invalidate();
            base.OnMouseLeave(ea);
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            Point p = PointToClient(MousePosition);

            for (int i = 0; i < 5; ++i) {
                if (rcStarAreas[i].Contains(p)) {
                    if (HoverStar != i + 1) {
                        HoverStar = i + 1;
                        Invalidate();
                    }
                    break;
                }
            }

            base.OnMouseMove(args);
        }

        protected override void OnClick(System.EventArgs args)
        {
            Point p = PointToClient(MousePosition);

            for (int i = 0; i < 5; ++i) {
                if (rcStarAreas[i].Contains(p)) {
                    HoverStar = i + 1;
                    SelectedStar = (i == 0 && SelectedStar == 1) ? 0 : i + 1;
                    Invalidate();
                    break;
                }
            }

            base.OnClick(args);
        }
    }
}
