using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Jarvis.AvatarService.Support
{
    public static class AvatarBuilder
    {
        public static string RootFolder { get; set; }
        public static string CustomRootFolder
        {
            get { return Path.Combine(RootFolder, "avatars"); }
        }

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

            var original = Path.Combine(CustomRootFolder, userId + ".png");
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

        /// <summary>
        /// Creo un avatar da uno stream mettendolo nella cartella "Avatars"
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="size"></param>
        /// <param name="stream"></param>
        public static void CreateByStream(string userId, int size, Stream stream)
        {
            var pathToFile = Path.Combine(CustomRootFolder, string.Format("{0}.png", userId)).ToLowerInvariant();
            using (var image = Image.FromStream(stream))
            {
                using (var newImage = ScaleImage(image, size, size))
                {
                    newImage.Save(pathToFile, ImageFormat.Png);
                }
            }
            //Clear old image
            ClearOld(userId, size);
        }

        private static void ClearOld(String userId, int size)
        {
            var dir = new DirectoryInfo(Path.Combine(RootFolder, size.ToString()));
            if (dir.Exists)
            {
                var files = dir.GetFiles(String.Format("{0}_*", userId.ToLowerInvariant()));
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        if (file.Exists)
                        {
                            File.Delete(file.FullName);
                        }
                    }
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

            //If you not trim and fullname starts with space it throws.
            var sanitizedFullName = Sanitize(fullname).Trim();

            var tokens = sanitizedFullName
                .ToUpperInvariant()
                .Split(' ')
                .Where(t => !String.IsNullOrEmpty(t))
                .ToArray();
            if (tokens.Length == 0)
            {
                //I have no token after sanitization, we have a string composed only by non char, non number
                return new String(fullname.Trim().Take(2).ToArray());
            }
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

        private static String Sanitize(String stringToSanitize)
        {
            StringBuilder sb = new StringBuilder(stringToSanitize.Length);
            foreach (var c in stringToSanitize)
            {
                if (Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                }
            }
            
            return sb.ToString();
        }
    }
}