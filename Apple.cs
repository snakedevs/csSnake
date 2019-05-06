using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace snakeGame
{
    /// <summary>
    /// Apple object
    /// </summary>
    public class Apple
    {
        //David
        private static int randomNumberMethod()
        {
            //word generation for easy + hard
            int randomnumber;
            //generates random number within 0-20
            Random r = new Random();
            randomnumber = r.Next(17);
            return randomnumber;
        }
        private Snake s;

        public Point Position { get; private set; }
        private Rectangle apple;
        private int height = 44;
        private int width = 44;

        public Apple(MainWindow window)
        {
            Generate();
            window.GameCanvas.Children.Add(apple);
            Canvas.SetTop(apple, Position.Y);
            Canvas.SetLeft(apple, Position.X);
        }

        private void Generate()
        {
            Position = RandomPos(s);
            apple = new Rectangle();
            apple.Height = height;
            apple.Width = width;
            apple.Fill = Brushes.DarkRed;
        }

        private Point RandomPos(Snake s)
        {
            bool isOnSnake = false;
            Point tempPoint = new Point();

            tempPoint.X = randomNumberMethod();
            tempPoint.Y = randomNumberMethod();

            //for (int i = 0; i < s.Snake.count; i++)
            //{
            //    if (s.pos.x == tempPoint.X && s.pos.y == tempPoint.Y)
            //    {
            //        isOnSnake = true;
            //    }
            //}
            if (isOnSnake == false)
            {
                return tempPoint;
            }
            else
            {
                return RandomPos(s);
            }

        }

    }
}
