using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Genetic_Algorithm
{
    class Program
    {
        const int PIXEL_COUNT_PER_SQUARE = 10;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Bitmap img = new Bitmap(300, 300);

            Random rnd = new Random();
            for (int i = 0; i < img.Width; i += PIXEL_COUNT_PER_SQUARE)
            {
                for (int j = 0; j < img.Height; j += PIXEL_COUNT_PER_SQUARE)
                {
                    Color c = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    SetPixel(img, i, j, c);
                }
                //img.SetPixel(rnd.Next(img.Width), rnd.Next(img.Height), c);
            }

            img.Save(@"D:\Image\test3.bmp", ImageFormat.Bmp);

            var bitmap = new Bitmap(@"D:\Image\test3.bmp");

            int w = bitmap.Width, h = bitmap.Height;
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    Console.WriteLine(pixel);
                }
            }
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
