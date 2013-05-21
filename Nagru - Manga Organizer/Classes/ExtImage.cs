using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Nagru___Manga_Organizer
{
    public static class ExtImage
    {
        /* Proper image scaling   
           Author: MBigglesWorth (May 5, 2011) */
        public static Image Resize(Image img, float fWidth, float fHeight)
        {
            if (img.Width > img.Height && fWidth > img.Width) return img;

            float fPerWidth = fWidth / (float)img.Width;
            float fPerHeight = fHeight / (float)img.Height;
            float fAdj = fPerHeight < fPerWidth ? fPerHeight : fPerWidth;
            int iNewWidth = (int)(img.Width * fAdj);
            int iNewHeight = (int)(img.Height * fAdj);

            Image newImage = new Bitmap(iNewWidth, iNewHeight);
            using (Graphics g = Graphics.FromImage(newImage)) {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, iNewWidth, iNewHeight);
            }
            return newImage;
        }
    }
}
