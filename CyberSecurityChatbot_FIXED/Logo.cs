using System;
using System.Drawing;

namespace CyberSecurityChatbot
{//start of namespace

    public class Logo
    {//start of class

        //get base path
        string path_logo = AppDomain.CurrentDomain.BaseDirectory;

        //constructor
        public Logo()
        {//start constructor

            asci();

        }//end constructor


        //ascii method
        private void asci()
        {//start method
            Console.ForegroundColor = ConsoleColor.Cyan; // set cyan color
            //fix path to logo
            string full_path = path_logo.Replace(@"\bin\Debug\", @"\logo.jpg");

            //load image
            Bitmap image = new Bitmap(full_path);

            //resize image
            int width = 50;
            int height = 20;
            Bitmap resized = new Bitmap(image, new Size(width, height));

            //ascii characters
            string asciiChars = "@#S%?*+;:,. ";

            //loop height
            for (int y = 0; y < resized.Height; y++)
            {
                //loop width
                for (int x = 0; x < resized.Width; x++)
                {
                    //get pixel
                    Color pixel = resized.GetPixel(x, y);

                    //convert to gray
                    int gray = (pixel.R + pixel.G + pixel.B) / 3;

                    //map to ascii
                    int index = (gray * (asciiChars.Length - 1)) / 255;

                    Console.Write(asciiChars[index]);
                }

                Console.WriteLine();
            }
            Console.ResetColor(); // reset after logo

        }//end method

    }//end class

}//end namespace