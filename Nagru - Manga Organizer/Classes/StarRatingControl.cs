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
        protected readonly Color cOutln = Color.DarkGray;
        protected readonly Color cHover = Color.Yellow;
        protected readonly Color cFill = Color.Goldenrod;
        protected readonly GraphicsPath gpStar;
        protected readonly Rectangle[] rcArea;
        protected readonly Pen pnOutln;
        protected readonly int iPadding = 8;
        protected const int iHeight = 14;
        protected const int iWidth = 16;
        protected int iOutThick = 1,
            iHvrStar, iSelStar;
        
		protected bool IsHovering { get; private set; }
        public int HoverStar {
            get { return iHvrStar; }
            set {
                if (value > 0)
                    iHvrStar = value;
            }
        }
        public int SelectedStar {
            get { return iSelStar; }
            set {
                if (value >= 0) {
                    iSelStar = value;
                    Invalidate();
                }
            }
        }
        public int OutlineThickness {
            get { return iOutThick; }
            set {
                if (value > 0) {
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

            pnOutln = new Pen(cOutln, iOutThick);
            rcArea = new Rectangle[5];
            for (int i = 0; i < 5; ++i) {
                rcArea[i].X = i * (iWidth + iPadding);
                rcArea[i].Width = iWidth + iPadding;
                rcArea[i].Height = iHeight;
            }

            //setup star shape (from top tip and in thirds)
            gpStar = new GraphicsPath();
            gpStar.AddLines(new PointF[] {
                new PointF(iWidth / 2, 0),                   //A
                new PointF(2 * iWidth / 3, iHeight / 3),     //B
                new PointF(iWidth, iHeight / 3),             //C
                new PointF(4 * iWidth / 5, 4 * iHeight / 7), //D
                new PointF(5 * iWidth / 6, iHeight),         //E
                new PointF(iWidth / 2, 4 * iHeight / 5),     //F
                new PointF(iWidth / 6, iHeight),             //G
                new PointF(iWidth / 5, 4 * iHeight / 7),     //H
                new PointF(0, iHeight / 3),                  //I
                new PointF(iWidth / 3, iHeight / 3)          //J
            });
            gpStar.CloseFigure();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pe.Graphics.Clear(BackColor);

            Brush brFill;
            Rectangle rcDraw = new Rectangle(0, 0, iWidth, iHeight);
            for (int i = 0; i < 5; ++i) {
                if (IsHovering && iHvrStar > i)
                    brFill = new LinearGradientBrush(rcDraw, cHover, BackColor,
                        LinearGradientMode.ForwardDiagonal);
                else if (!IsHovering && iSelStar > i)
                    brFill = new LinearGradientBrush(rcDraw, cFill, BackColor,
                        LinearGradientMode.ForwardDiagonal);
                else brFill = new SolidBrush(BackColor);
                
                GraphicsPath gpTmp = GetPath(gpStar, rcDraw.X, 0);
                rcDraw.X += rcDraw.Width + iPadding;
                pe.Graphics.FillPath(brFill, gpTmp);
                pe.Graphics.DrawPath(pnOutln, gpTmp);
                gpTmp.Dispose();
            }
            base.OnPaint(pe);
        }

        protected static GraphicsPath GetPath(GraphicsPath gpObj, int iX, int iY)
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
                if (rcArea[i].Contains(p)) {
                    if (iHvrStar != i + 1) {
                        iHvrStar = i + 1;
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
                if (rcArea[i].Contains(p)) {
                    iHvrStar = i + 1;
                    iSelStar = (i == 0 && iSelStar == 1) ? 0 : i + 1;
                    Invalidate();
                    break;
                }
            }

            base.OnClick(args);
        }
    }
}
