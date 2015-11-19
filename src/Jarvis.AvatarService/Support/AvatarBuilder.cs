using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Jarvis.AvatarService.Support
{
    public static class AvatarBuilder
    {
        public static string RootFolder { get; set; }

        private static readonly Color[] Colors = new[] { 
            Color.DeepSkyBlue, 
            Color.DarkGreen, 
            Color.CadetBlue,
            Color.CornflowerBlue,
            Color.DarkBlue,
            Color.DarkSalmon,
            Color.DarkCyan,
            Color.DarkGoldenrod,
            Color.DarkMagenta,
            Color.DarkOrchid,
        };

        private static StringFormat _stringFormat;

        static AvatarBuilder()
        {
            _stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
        }

        public static string CreateFor(string userId, int size, string fullname)
        {
            var initials = GetInitials(fullname);
            var pathToFile = Path.Combine(RootFolder, size.ToString(), string.Format("{0}_{1}.png", userId, initials)).ToLowerInvariant();

            if (File.Exists(pathToFile))
                return pathToFile;

            Directory.CreateDirectory(Path.GetDirectoryName(pathToFile));

            using (var bitmap = new Bitmap(size, size, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var backgroundColor = GetColorFor(userId, fullname);
                g.Clear(backgroundColor);

                var units = GraphicsUnit.Pixel;

                using (var rectangleFont = new Font("Arial", (int)(size/4), FontStyle.Bold))
                {
                    g.DrawString(initials, rectangleFont, Brushes.Beige, bitmap.GetBounds(ref units), _stringFormat);
                }

                bitmap.Save(pathToFile, ImageFormat.Png);
            }

            return pathToFile;
        }

        public static string GetInitials(string fullname)
        {
            if (String.IsNullOrWhiteSpace(fullname))
                throw new Exception("Invalid name");

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

        public static Color GetColorFor(string userId, string fullName)
        {
            var hash = (Math.Abs(userId.GetHashCode() ^ fullName.GetHashCode())) % Colors.Length;
            return Colors[hash];
        }
    }
}