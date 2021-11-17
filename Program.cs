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
        static readonly Bitmap seagull = new Bitmap(@"D:\Image\seagull.png");

        static void Main(string[] args)
        {
            //CreateRandomImg();

            SelectWinners();
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

        static double GetScore(Bitmap img)
        {
            var score = 0.0;

            for (int x = 0; x < PIXEL_SIZE; x++)
            {
                for (int y = 0; y < PIXEL_SIZE; y++)
                {
                    var lab1 = CIELAB.RGBToLab(seagull.GetPixel(x, y));
                    var lab2 = CIELAB.RGBToLab(img.GetPixel(x, y));
                    var ciede = new CIEDE2000(lab1.L, lab1.A, lab1.B);
                    score += ciede.DE00(lab2.L, lab2.A, lab2.B);
                }
            }

            return score;
        }

        static void SelectWinners()
        {
            //★
            var isFirst = true;

            Queue<int> que = new Queue<int>();
            Queue<int> queWinners = new Queue<int>();
            for (int i = 0; i < 256; i++) que.Enqueue(i);

            var folderName = "g_1";

            while (que.Count != 4)
            {
                while (que.Any())
                {
                    var num1 = que.Dequeue();
                    var num2 = que.Dequeue();

                    var score1 = GetScore(new Bitmap(PATH + folderName + @"\" + num1 + ".bmp"));
                    var score2 = GetScore(new Bitmap(PATH + folderName + @"\" + num2 + ".bmp"));

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


            //★
            foreach (var item in que)
            {
                Console.WriteLine(item);
            }
        }
    }
}
