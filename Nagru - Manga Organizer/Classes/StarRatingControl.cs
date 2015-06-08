using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
	/// <summary>
	/// Custom control for displaying ratings
	/// Inspiration: Andrey Butov (Dec 20, 2004)
	/// </summary>
  public class StarRatingControl : Control
  {
    #region Properties
    protected Color cOutln = Color.DarkGray;
    protected Color cHover = Color.Yellow;
    protected Color cFill = Color.Goldenrod;
    protected readonly GraphicsPath gpStar;
    protected readonly Rectangle[] rcArea;
    protected readonly Pen pnOutln;
    protected const int iPadding = 8;
    protected const int iHeight = 14;
    protected const int iWidth = 16;
    protected int iOutThick = 1;
		protected int iHvrStar;
		protected int iSelStar;

		#region Interface
		protected bool IsHovering {
      get;
      private set;
    }

    [DefaultValue(0)]
    [Description("Gets or sets the currently hovered-over star.")]
    public int HoverStar {
      get {
        return iHvrStar;
      }
      set {
        if (value > 0)
          iHvrStar = value;
      }
    }

    [DefaultValue(0)]
    [Description("Gets or sets the top currently selected star.")]
    public int SelectedStar {
      get {
        return iSelStar > 5 ? 5 : iSelStar;
      }
      set {
        if (value >= 0) {
          iSelStar = value;
          Invalidate();
        }
      }
    }

    [DefaultValue(1)]
    [Description("Gets or sets the stars outline thickness.")]
    public int OutlineThickness {
      get {
        return iOutThick;
      }
      set {
        if (value > 0) {
          iOutThick = value;
          Invalidate();
        }
      }
    }

    [DefaultValue(typeof(Color), "DarkGray")]
    [Description("Gets or sets the stars outline color.")]
    public Color ColorOutline {
      get {
        return cOutln;
      }
      set {
        cOutln = value;
      }
    }

    [DefaultValue(typeof(Color), "Yellow")]
    [Description("Gets or sets the stars hover color.")]
    public Color ColorHover {
      get {
        return cHover;
      }
      set {
        cHover = value;
      }
    }

    [DefaultValue(typeof(Color), "Goldenrod")]
    [Description("Gets or sets the stars fill color.")]
    public Color ColorFill {
      get {
        return cFill;
      }
      set {
        cFill = value;
      }
    }
		#endregion

    #endregion

		#region Constructor

		/// <summary>
		/// Initializes the control with default values
		/// </summary>
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
      PointF[] pfStar = new PointF[10];
      pfStar[0] = new PointF(iWidth / 2, 0);											//12:00
      pfStar[1] = new PointF(2 * iWidth / 3, iHeight / 3);        //1:00
      pfStar[2] = new PointF(iWidth, iHeight / 3);                //2:00
      pfStar[3] = new PointF(4 * iWidth / 5, 4 * iHeight / 7);    //3:00
      pfStar[4] = new PointF(5 * iWidth / 6, iHeight);            //4:00
      pfStar[5] = new PointF(iWidth / 2, 4 * iHeight / 5);        //6:00
      pfStar[6] = new PointF(iWidth - pfStar[4].X, pfStar[4].Y);	//8:00
      pfStar[7] = new PointF(iWidth - pfStar[3].X, pfStar[3].Y);	//9:00
      pfStar[8] = new PointF(iWidth - pfStar[2].X, pfStar[2].Y);	//10:00
      pfStar[9] = new PointF(iWidth - pfStar[1].X, pfStar[1].Y);	//11:00
      gpStar.AddLines(pfStar);
      gpStar.CloseFigure();
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
      for (int i = 0; i < 5; ++i) {
        if (IsHovering && iHvrStar > i)
          brFill = new LinearGradientBrush(rcDraw, cHover, BackColor,
              LinearGradientMode.ForwardDiagonal);
        else if (!IsHovering && iSelStar > i)
          brFill = new LinearGradientBrush(rcDraw, cFill, BackColor,
              LinearGradientMode.ForwardDiagonal);
        else
          brFill = new SolidBrush(BackColor);

        GraphicsPath gpTmp = GetPath(rcDraw.X, 0);
        rcDraw.X += rcDraw.Width + iPadding;
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
      IsHovering = true;
      Invalidate();
      base.OnMouseEnter(ea);
    }

		/// <summary>
		/// Re-draws the control to return it to its normal state
		/// </summary>
    protected override void OnMouseLeave(System.EventArgs ea)
    {
      IsHovering = false;
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
          if (iHvrStar != i + 1) {
            iHvrStar = i + 1;
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
          iHvrStar = i + 1;
          iSelStar = (i == 0 && iSelStar == 1) ? 0 : i + 1;
          Invalidate();
          break;
        }
      }

      base.OnClick(args);
    }

		#endregion

		#region Custom Methods

		/// <summary>
		/// Returns a 'Star', cloned and transformed to the indicated position
		/// </summary>
		/// <param name="iX">The new X coordinate</param>
		/// <param name="iY">The new Y coordinate</param>
    protected GraphicsPath GetPath(int iX, int iY)
    {
      GraphicsPath clone = (GraphicsPath)gpStar.Clone();
      Matrix mat = new Matrix();
      mat.Translate(iX, iY);
      clone.Transform(mat);
      return clone;
    }

		#endregion
	}
}
