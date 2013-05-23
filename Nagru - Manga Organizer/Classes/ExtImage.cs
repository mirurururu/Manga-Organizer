using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Nagru___Manga_Organizer
{
    public static class ExtImage
    {
        /* Proper image scaling   
           Author: Alex Aza (Jun 28, 2011) */
        public static Bitmap Scale(Image img, float fMaxWidth, float fMaxHeight)
        {
            float fRatio = Math.Min(
                fMaxWidth / img.Width,
                fMaxHeight / img.Height);

            int iWidth = (int)(img.Width * fRatio);
            int iHeight = (int)(img.Height * fRatio);

            Bitmap bmpNew = new Bitmap(iWidth, iHeight);
            Graphics.FromImage(bmpNew).DrawImage(img, 0, 0, iWidth, iHeight);
            return bmpNew;
        }
    }
}
