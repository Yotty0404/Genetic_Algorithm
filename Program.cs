using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Excel_CEIDE2000;

namespace Genetic_Algorithm
{
    class Program
    {
        const string PATH = @"D:\Image\";
        const string FOLDER_PATH = @"D:\Image\g_{0}";
        const string IMAGE_PATH = @"D:\Image\g_{0}\{1}.bmp";
        const string WINNERS_FOLDER_PATH = @"D:\Image\g_{0}\Winners";
        const string WINNERS_IMAGE_PATH = @"D:\Image\g_{0}\Winners\{1}.bmp";

        const int PIXEL_SIZE = 50;
        const int GENERATIONS_NUM = 300;
        static readonly Bitmap ORIGINAL_IMAGE = new Bitmap(@"D:\Image\original.png");

        static void Main(string[] args)
        {
            CreateRandomImg();
            SelectWinners(0);

            for (int i = 1; i <= GENERATIONS_NUM; i++)
            {
                CreateNewGenarations(i);
                SelectWinners(i);
            }

            var tempPath = @"D:\Image\0_temp";

            CollectWinners(tempPath);
            ExpandImages(tempPath);
        }

        static void CreateRandomImg()
        {
            var generation = 0;
            Directory.CreateDirectory(string.Format(FOLDER_PATH, generation));

            for (int i = 0; i < 256; i++)
            {
                Bitmap img = new Bitmap(PIXEL_SIZE, PIXEL_SIZE);
                Random rnd = new Random();

                for (int x = 0; x < img.Width; x++)
                {
                    for (int y = 0; y < img.Height; y++)
                    {
                        Color c = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                        img.SetPixel(x, y, c);
                    }
                }

                img.Save(string.Format(IMAGE_PATH, generation, i));
            }
        }

        static double GetScore(Bitmap img)
        {
            var score = 0.0;

            for (int x = 0; x < PIXEL_SIZE; x++)
            {
                for (int y = 0; y < PIXEL_SIZE; y++)
                {
                    var lab1 = CIELAB.RGBToLab(ORIGINAL_IMAGE.GetPixel(x, y));
                    var lab2 = CIELAB.RGBToLab(img.GetPixel(x, y));
                    var ciede = new CIEDE2000(lab1.L, lab1.A, lab1.B);
                    score += ciede.DE00(lab2.L, lab2.A, lab2.B);
                }
            }

            return score;
        }

        static void SelectWinners(int generation)
        {
            Queue<int> que = new Queue<int>();
            Queue<int> queWinners = new Queue<int>();
            for (int i = 0; i < 256; i++) que.Enqueue(i);

            //4枚になるまで選別作業を繰り返す
            while (que.Count > 4)
            {
                while (que.Any())
                {
                    var num1 = que.Dequeue();
                    var num2 = que.Dequeue();

                    var score1 = GetScore(new Bitmap(string.Format(IMAGE_PATH, generation, num1)));
                    var score2 = GetScore(new Bitmap(string.Format(IMAGE_PATH, generation, num2)));

                    if (score1 <= score2)
                    {
                        queWinners.Enqueue(num1);
                    }
                    else
                    {
                        queWinners.Enqueue(num2);
                    }
                }

                que = queWinners;
                queWinners = new Queue<int>();
            }

            Directory.CreateDirectory(string.Format(WINNERS_FOLDER_PATH, generation));
            foreach (var num in que)
            {
                File.Copy(string.Format(IMAGE_PATH, generation, num), string.Format(WINNERS_IMAGE_PATH, generation, num));
            }

        }

        static void CreateNewGenarations(int generation)
        {
            //1つ前の世代の、残った4枚を取得
            var images = new List<Bitmap>();
            foreach (var path in Directory.EnumerateFiles(string.Format(WINNERS_FOLDER_PATH, generation - 1), "*"))
            {
                images.Add(new Bitmap(path));
            }

            Directory.CreateDirectory(string.Format(FOLDER_PATH, generation));

            for (int i = 0; i < 256; i++)
            {
                Bitmap img = new Bitmap(PIXEL_SIZE, PIXEL_SIZE);

                for (int x = 0; x < img.Width; x++)
                {
                    for (int y = 0; y < img.Height; y++)
                    {
                        img.SetPixel(x, y, CreateColor(images, x, y));
                    }
                }

                img.Save(string.Format(IMAGE_PATH, generation, i));
            }
        }

        static Color CreateColor(List<Bitmap> images, int x, int y)
        {
            Random rnd = new Random();

            //1%の確率で突然変異
            if (rnd.Next(100) < 1)
            {
                return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            }

            var imageNo = rnd.Next(4);
            return images[imageNo].GetPixel(x, y);
        }

        /// <summary>
        /// ドット絵を拡張するメソッド
        /// </summary>
        /// <param name="folderPath"></param>
        static void ExpandImages(string folderPath)
        {
            var newFolderPath = folderPath + @"\ex";
            Directory.CreateDirectory(newFolderPath);
            foreach (var path in Directory.EnumerateFiles(folderPath, "*"))
            {
                var originalImg = new Bitmap(path);
                var img = new Bitmap(512, 512);
                var g = Graphics.FromImage(img);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(originalImg, 0, 0, 512, 512);
                img.Save(Path.Combine(newFolderPath, Path.GetFileName(path)));
            }
        }

        /// <summary>
        /// 各世代1枚ずつ勝者を集めるメソッド
        /// </summary>
        /// <param name="folderPath"></param>
        static void CollectWinners(string folderPath)
        {
            for (int i = 0; i <= GENERATIONS_NUM; i++)
            {
                var path = Directory.EnumerateFiles(string.Format(WINNERS_FOLDER_PATH, i), "*").FirstOrDefault();

                File.Copy(path, Path.Combine(folderPath, string.Format("{0}.jpg", i)));
            }
        }
    }
}
