using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Nagru___Manga_Organizer
{
	/// <summary>
	/// Custom control for showing progress
	/// </summary>
  public class ProgressControl : Control
  {
    #region Properties
		private const int iHeight = 20;
		private const int iWidth = 70;
		private int  iHover = 0;

    protected readonly GraphicsPath gpBlock;
    protected readonly Rectangle[] rcArea;
    protected readonly Pen pnOutln;
    protected Color cFill = SystemColors.MenuHighlight;
    protected Color cOutln = Color.Black;
    protected const int iOutlln = 1;
    protected int iBlocks = 5;
    protected int iStep = 0;
    protected int iPad = 5;

		#region Interface

		public bool Hovering {
      get;
      private set;
    }
    public Color FillColor {
      get {
        return cFill;
      }
      set {
        cFill = value;
      }
    }
    public Color Outline {
      get {
        return cOutln;
      }
      set {
        cOutln = value;
      }
    }
    public int Blocks {
      get {
        return iBlocks;
      }
      set {
        if (value > 0) {
          iBlocks = value;
          Invalidate();
        }
      }
    }
    public int Step {
      get {
        return iStep;
      }
      set {
        if (value >= 0) {
          iStep = value;
          Invalidate();
        }
      }
    }
    public int Buffer {
      get {
        return iPad;
      }
      set {
        if (value >= 0) {
          iPad = value;
          Invalidate();
        }
      }
    }

		#endregion

		#endregion

		#region Constructor
		
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

		#endregion

		#region Events
		
		/// <summary>
		/// Draws the Control to the form
		/// </summary>
		protected override void OnPaint(PaintEventArgs pe)
    {
      pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
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
				else {
          brFill = new SolidBrush(BackColor);
				}

        GraphicsPath gpTmp = GetPath(rcDraw.X, 0);
        rcDraw.X += rcDraw.Width + iPad;
        pe.Graphics.FillPath(brFill, gpTmp);
        pe.Graphics.DrawPath(pnOutln, gpTmp);
        gpTmp.Dispose();
      }
      base.OnPaint(pe);
    }

		/// <summary>
		/// Re-draws the control to highlight the control based on the area hovered over
		/// </summary>
    protected override void OnMouseEnter(System.EventArgs ea)
    {
      Hovering = true;
      Invalidate();
      base.OnMouseEnter(ea);
    }

		/// <summary>
		/// Re-draws the control to return it to its normal state
		/// </summary>
    protected override void OnMouseLeave(System.EventArgs ea)
    {
      Hovering = false;
      Invalidate();
      base.OnMouseLeave(ea);
    }

		/// <summary>
		/// Re-determines the area to highlight
		/// </summary>
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

		/// <summary>
		/// Sets the new progress value of the control
		/// </summary>
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

		#endregion

		#region Custom Methods

		/// <summary>
		/// Returns a 'Progress Block', cloned and transformed to the indicated position
		/// </summary>
		/// <param name="iX">The new X coordinate</param>
		/// <param name="iY">The new Y coordinate</param>
		protected GraphicsPath GetPath(int iX, int iY)
    {
      GraphicsPath clone = (GraphicsPath)gpBlock.Clone();
      Matrix mat = new Matrix();
      mat.Translate(iX, iY);
      clone.Transform(mat);
      return clone;
		}

		#endregion
	}
}
