using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Nagru___Manga_Organizer
{
    /* Author: Andrey Butov (Dec 20, 2004) */
    public class StarRatingControl : Control
    {
        #region Fields
        protected readonly Pen pnOutline;
        protected readonly GraphicsPath gpStar;
        protected const int iMargin = 2;
        protected const int iStarWidth = 16;
        protected const int iStarHeight = 14;
        
        protected bool bHover = false;
        protected int iOutlineThickness = 1;
        protected int iSelectedStar = 0;
        protected int iHoverStar = 0;
        protected int iPadding = 8;
        protected Rectangle[] rcStarAreas;
        protected Color cOutline = Color.DarkGray;
        protected Color cHover = Color.Yellow;
        protected Color cSelected = Color.RoyalBlue;
        #endregion

        #region Properties
        public int StarSpacing
        {
            get { return iPadding; }
            set 
            {
                if (iPadding != value) {
                    iPadding = value;
                    Invalidate();
                }
            }
        }

        public bool IsHovering
        {
            get { return bHover; }
        }

        public Color OutlineColor
        {
            get { return cOutline; }
            set
            {
                if (cOutline != value)
                {
                    cOutline = value;
                    Invalidate();
                }
            }
        }

        public Color HoverColor
        {
            get { return cHover; }
            set
            {
                if (cHover != value)
                {
                    cHover = value;
                    Invalidate();
                }
            }
        }

        public Color SelectedColor
        {
            get { return cSelected; }
            set
            {
                if (cSelected != value)
                {
                    cSelected = value;
                    Invalidate();
                }
            }
        }

        public int OutlineThickness
        {
            get { return iOutlineThickness; }
            set
            {
                if (iOutlineThickness != value)
                {
                    iOutlineThickness = value;
                    Invalidate();
                }
            }
        }

        public int HoverStar
        {
            get { return iHoverStar; }
            set { iHoverStar = value; }
        }

        public int SelectedStar
        {
            get { return iSelectedStar; }
            set
            {
                iSelectedStar = value;
                Invalidate();
            }
        }
        #endregion

        public StarRatingControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            Rectangle rcArea = new Rectangle(iMargin, iMargin, iStarWidth, iStarHeight);
            pnOutline = new Pen(OutlineColor, OutlineThickness);
            Width = 120;
            Height = 18;

            rcStarAreas = new Rectangle[5];
            for (int i = 0; i < 5; ++i) {
                rcStarAreas[i].Y = rcArea.Y;
                rcStarAreas[i].Width = rcArea.Width + iPadding / 2;
                rcStarAreas[i].Height = rcArea.Height;
            }

            gpStar = new GraphicsPath();
            gpStar.AddLines(new PointF[] {
                new PointF(rcArea.X + (rcArea.Width / 2), rcArea.Y),
                new PointF(rcArea.X + (42 * rcArea.Width / 64), rcArea.Y + (19 * rcArea.Height / 64)),
                new PointF(rcArea.X + rcArea.Width, rcArea.Y + (22 * rcArea.Height / 64)),
                new PointF(rcArea.X + (48 * rcArea.Width / 64), rcArea.Y + (38 * rcArea.Height / 64)),
                new PointF(rcArea.X + (52 * rcArea.Width / 64), rcArea.Y + rcArea.Height),
                new PointF(rcArea.X + (rcArea.Width / 2), rcArea.Y + (52 * rcArea.Height / 64)),
                new PointF(rcArea.X + (12 * rcArea.Width / 64), rcArea.Y + rcArea.Height),
                new PointF(rcArea.X + rcArea.Width / 4, rcArea.Y + (38 * rcArea.Height / 64)),
                new PointF(rcArea.X, rcArea.Y + (22 * rcArea.Height / 64)),
                new PointF(rcArea.X + (22 * rcArea.Width / 64), rcArea.Y + (19 * rcArea.Height / 64))
            });
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.Clear(BackColor);

            Rectangle drawArea = new Rectangle(iMargin, iMargin, iStarWidth, iStarHeight);
            for (int i = 0; i < 5; ++i) {
                rcStarAreas[i].X = drawArea.X - iPadding / 2;
                DrawStar(pe.Graphics, drawArea, i);
                drawArea.X += drawArea.Width + iPadding;
            }
            base.OnPaint(pe);
        }

        protected void DrawStar(Graphics g, Rectangle rect, int iAreaIndx)
        {
            Brush fillBrush;

            if (bHover && HoverStar > iAreaIndx)
                fillBrush = new LinearGradientBrush(rect, HoverColor, BackColor, 
                    LinearGradientMode.ForwardDiagonal);
            else if ((!bHover) && iSelectedStar > iAreaIndx)
                fillBrush = new LinearGradientBrush(rect, SelectedColor, BackColor, 
                    LinearGradientMode.ForwardDiagonal);
            else fillBrush = new SolidBrush(BackColor);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicsPath gpTmp = GetPath(gpStar, rect.X, rect.Y);
            g.FillPath(fillBrush, gpTmp);
            g.DrawPath(pnOutline, gpTmp);
            gpTmp.Dispose();
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
            bHover = true;
            Invalidate();
            base.OnMouseEnter(ea);
        }

        protected override void OnMouseLeave(System.EventArgs ea)
        {
            bHover = false;
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
                    iSelectedStar = (i == 0 && iSelectedStar == 1) ? 0 : i + 1;
                    Invalidate();
                    break;
                }
            }

            base.OnClick(args);
        }
    }
}
