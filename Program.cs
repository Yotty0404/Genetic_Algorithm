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

        const int PIXEL_SIZE = 32;
        const int WINNERS_COUNT = 4;
        static readonly Bitmap SEAGULL = new Bitmap(@"D:\Image\seagull.png");

        static void Main(string[] args)
        {
            CreateRandomImg();

            for (int i = 0; i < 100; i++)
            {
                SelectWinners(i);
                CreateNewGenarations(i);
            }
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
                    var lab1 = CIELAB.RGBToLab(SEAGULL.GetPixel(x, y));
                    var lab2 = CIELAB.RGBToLab(img.GetPixel(x, y));
                    var ciede = new CIEDE2000(lab1.L, lab1.A, lab1.B);
                    score += ciede.DE00(lab2.L, lab2.A, lab2.B);
                }
            }

            return score;
        }

        static void SelectWinners(int generation)
        {
            //★
            var isFirst = true;

            Queue<int> que = new Queue<int>();
            Queue<int> queWinners = new Queue<int>();
            for (int i = 0; i < 256; i++) que.Enqueue(i);

            while (que.Count != 4)
            {
                while (que.Any())
                {
                    var num1 = que.Dequeue();
                    var num2 = que.Dequeue();

                    var score1 = GetScore(new Bitmap(string.Format(IMAGE_PATH, generation, num1)));
                    var score2 = GetScore(new Bitmap(string.Format(IMAGE_PATH, generation, num2)));

                    //★
                    if (isFirst)
                    {
                        Console.WriteLine(num1.ToString() + "," + score1.ToString());
                        Console.WriteLine(num2.ToString() + "," + score2.ToString());
                    }


                    if (score1 <= score2)
                    {
                        queWinners.Enqueue(num1);
                    }
                    else
                    {
                        queWinners.Enqueue(num2);
                    }
                }

                //★
                isFirst = false;

                que = queWinners;
                queWinners = new Queue<int>();
            }


            Directory.CreateDirectory(string.Format(WINNERS_FOLDER_PATH, generation));
            foreach (var num in que)
            {
                File.Copy(string.Format(IMAGE_PATH, generation, num), string.Format(WINNERS_IMAGE_PATH, generation, num));
            }

        }

        static void CreateNewGenarations(int oldGeneration)
        {
            //1つ前の世代の、残った4枚を取得
            var images = new List<Bitmap>();
            foreach (var path in Directory.EnumerateFiles(string.Format(WINNERS_FOLDER_PATH, oldGeneration), "*"))
            {
                images.Add(new Bitmap(path));
            }

            var generation = oldGeneration + 1;
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
    }
}
