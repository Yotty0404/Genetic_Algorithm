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
        const int PIXEL_SIZE = 32;
        const int PIXEL_COUNT_PER_SQUARE = 10;
        static Bitmap seagull = new Bitmap(@"D:\Image\seagull.png");

        static void Main(string[] args)
        {
            //Bitmap seagull = new Bitmap(@"D:\Image\seagull.png");
            Queue<int> que = new Queue<int>();
            for (int i = 0; i < 256; i++) que.Enqueue(i);
            var test1 = que.Dequeue();
            var test2 = que.Dequeue();

            var folderName = "g_1";
            //GetScore(new Bitmap(PATH + folderName + @"\" + test1 + ".bmp"));
            //CreateRandomImg();

            //var bitmap = new Bitmap(@"D:\Image\test3.bmp");

            //int w = bitmap.Width, h = bitmap.Height;

            var c = new CIEDE2000(50, 2.6772, -79.7751);
            Console.WriteLine(c.DE00(50, 0, -82.7485));

            Color color = Color.FromArgb(0,255,0);
            var test = new CIELAB(color);
            Console.WriteLine(test.L.ToString()+", "+test.A + ", " + test.B);
        }

        static void CreateRandomImg()
        {
            var folderName = "g_1";
            Directory.CreateDirectory(PATH + folderName);

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

                img.Save(PATH + folderName + @"\" + i + ".bmp", ImageFormat.Bmp);
            }
        }

        static int GetScore(Bitmap img)
        {
            var score = 0;

            for (int x = 0; x < PIXEL_SIZE; x++)
            {
                for (int y = 0; y < PIXEL_SIZE; y++)
                {
                    Color pixel = img.GetPixel(x, y);
                    Console.WriteLine(pixel.A.ToString() + pixel.R + pixel.G + pixel.B);
                }
            }
            return score;
        }

        static void SetPixel(Bitmap img, int x, int y, Color c)
        {
            for (int i = 0; i < PIXEL_COUNT_PER_SQUARE; i++)
            {
                for (int j = 0; j < PIXEL_COUNT_PER_SQUARE; j++)
                {
                    img.SetPixel(x + i, y + j, c);
                }
            }
        }
    }
}
