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

        private static readonly StringFormat Center;

        static AvatarBuilder()
        {
            Center = new StringFormat
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

            var original = Path.Combine(RootFolder, "avatars", userId + ".png");
            if (File.Exists(original))
            {
                CreateAvatarWithImage(original, size, pathToFile);
            }
            else
            {
                CreateAvatarWithInitials(userId, size, fullname, initials, pathToFile);
            }

            return pathToFile;
        }

        public static void CreateByStream(string userId, int size, string path, Stream stream)
        {
            using (var image = Image.FromStream(stream))
            {
                using (var newImage = ScaleImage(image, size, size))
                {
                    newImage.Save(path, ImageFormat.Png);
                }
            }
        }

        private static void CreateAvatarWithImage(string sourceImage, int size, string pathToFile)
        {
            using (var image = Image.FromFile(sourceImage))
            using (var newImage = ScaleImage(image, size, size))
            {
                newImage.Save(pathToFile, ImageFormat.Png);
            }
        }

        private static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        private static void CreateAvatarWithInitials(string userId, int size, string fullname, string initials, string pathToFile)
        {
            using (var bitmap = new Bitmap(size, size, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var backgroundColor = GetColorFor(userId, fullname);
                g.Clear(backgroundColor);

                var units = GraphicsUnit.Pixel;

                using (var rectangleFont = new Font("Arial", (int)(size / 4), FontStyle.Bold))
                {
                    g.DrawString(initials, rectangleFont, Brushes.Beige, bitmap.GetBounds(ref units), Center);
                }

                bitmap.Save(pathToFile, ImageFormat.Png);
            }
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