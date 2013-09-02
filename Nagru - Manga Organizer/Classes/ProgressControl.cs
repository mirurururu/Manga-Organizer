using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Nagru___Manga_Organizer.Classes
{
    public class ProgressControl : Control
    {
        #region Properties
        protected readonly GraphicsPath gpBlock;
        protected readonly Rectangle[] rcArea;
        protected readonly Pen pnOutln;
        readonly int iHeight = 20;
        readonly int iWidth = 70;

        protected Color cFill = SystemColors.MenuHighlight;
        protected Color cOutln = Color.Black;
        protected const int iOutlln = 1;
        protected int iBlocks = 5;
        protected int iStep = 0;
        protected int iPad = 5;
        protected int iHover = 0;

        public bool Hovering { get; private set; }
        public Color FillColor {
            get { return cFill; }
            set { cFill = value; }
        }
        public Color Outline {
            get { return cOutln; }
            set { cOutln = value; }
        }
        public int Blocks {
            get { return iBlocks; }
            set { 
                if(value > 0) {
                    iBlocks = value;
                    Invalidate();
                }
            }
        }
        public int Step {
            get { return iStep; }
            set {
                if (value >= 0) {
                    iStep = value;
                    Invalidate();
                }
            }
        }
        public int Buffer {
            get { return iPad; }
            set { 
                if(value >= 0) {
                    iPad = value;
                    Invalidate();
                }
            }
        }
        #endregion

        public ProgressControl()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            pnOutln = new Pen(cOutln, iOutlln);
            rcArea = new Rectangle[5];
            for (int i = 0; i < 5; ++i) {
                rcArea[i].X = i * (iWidth + iPad);
                rcArea[i].Width = iWidth + iPad;
                rcArea[i].Height = iHeight;
            }
            
            //setup arrow shape
            gpBlock = new GraphicsPath();
            gpBlock.AddLines(new PointF[] {
                new PointF(0, 0),                   //TL
                new PointF(iWidth - iPad, 0),       //TR
                new PointF(iWidth, iHeight / 2),    //MR
                new PointF(iWidth - iPad, iHeight), //BR
                new PointF(0, iHeight),             //BL
                new PointF(iPad, iHeight / 2)       //ML
            });
            gpBlock.CloseFigure();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.Clear(BackColor);

            Brush brFill;
            Rectangle rcDraw = new Rectangle(0, 0, iWidth, iHeight);
            for (int i = 0; i < iBlocks; i++) {
                if (Hovering && iHover > i) {
                    brFill = new LinearGradientBrush(rcDraw, cFill, BackColor,
                        LinearGradientMode.ForwardDiagonal);
                }
                else if (!Hovering && iStep > i) {
                    brFill = new SolidBrush(cFill);
                }
                else brFill = new SolidBrush(BackColor);

                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                GraphicsPath gpTmp = GetPath(gpBlock, rcDraw.X, rcDraw.Y);
                rcDraw.X += rcDraw.Width + iPad;
                pe.Graphics.FillPath(brFill, gpTmp);
                pe.Graphics.DrawPath(pnOutln, gpTmp);
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
            Hovering = true;
            Invalidate();
            base.OnMouseEnter(ea);
        }

        protected override void OnMouseLeave(System.EventArgs ea)
        {
            Hovering = false;
            Invalidate();
            base.OnMouseLeave(ea);
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            Point p = PointToClient(MousePosition);

            for (int i = 0; i < 5; ++i) {
                if (rcArea[i].Contains(p)) {
                    if (iHover != i + 1) {
                        iHover = i + 1;
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
                    iHover = i + 1;
                    iStep = i;
                    Invalidate();
                    break;
                }
            }

            base.OnClick(args);
        }
    }
}
