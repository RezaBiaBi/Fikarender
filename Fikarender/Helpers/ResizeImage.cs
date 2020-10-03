using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace ResizeImageASPNETCore
{
    public static class ResizeImage
    {
        public static Image Resize(this Image current, int maxWidth, int maxHeight, bool force)
        {
            int width, height;
            #region reckon size
            if (force)
            {
                width = maxWidth;
                height = maxHeight;
            }
            else
            {
                if (current.Width > current.Height)
                {
                    width = maxWidth;
                    height = Convert.ToInt32(current.Height * maxHeight / (double)current.Width);
                }
                else
                {
                    width = Convert.ToInt32(current.Width * maxWidth / (double)current.Height);
                    height = maxHeight;
                }
            }
            #endregion

            #region get resized bitmap
            var canvas = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(canvas))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(current, 0, 0, width, height);
            }
            return canvas;
            #endregion
        }

        public static byte[] ToByteArray(this Image current, string format)
        {
            using (var stream = new MemoryStream())
            {
                if (format.Contains("jpeg"))
                {
                    current.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else
                {
                    current.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                }
                return stream.ToArray();
            }
        }
    }
}