using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Jarvis.AvatarService.Support
{
    public class AvatarBuilder
    {
        private string RootFolder { get; set; }

        public string CreateFor(string userId, int size, string fullname)
        {
            if(String.IsNullOrWhiteSpace(fullname))
                throw new Exception("Invalid name");

            var initials = GetInitials(fullname);
            //"root\\size\\userId_initials.png";
            var pathToFile = Path.Combine(RootFolder, size.ToString(), string.Format("{0}_{1}.png", userId, initials)).ToLowerInvariant();


            using (var rectangleFont = new Font("Arial", 36, FontStyle.Bold))
            using (var bitmap = new Bitmap(size, size, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var backgroundColor = Color.DeepSkyBlue;
                g.Clear(backgroundColor);
                g.DrawString(initials, rectangleFont, Brushes.Beige, new PointF(4, 16));

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                }
            }

            return pathToFile;
        }

        public static string GetInitials(string fullname)
        {
            var tokens = fullname.ToUpperInvariant().Split(' ');
            string initials = tokens[0].Substring(0, 1);
            if (tokens.Length >= 3)
            {
                initials += tokens[2].Substring(0, 1);
            }
            else
            {
                if (tokens.Length > 1)
                {
                    initials += tokens[1].Substring(0, 1);
                }
            }
            return initials;
        }
    }
}