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
    class Apple
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
            apple.Height = 44;
            apple.Width = 44;
            apple.Fill = Brushes.DarkRed;
        }

        private Point RandomPos(Snake s)
        {
            Point tempPoint = new Point();

            tempPoint.X = randomNumberMethod();
            tempPoint.Y = randomNumberMethod();

            if (s.pos.x == tempPoint.X && s.pos.y == tempPoint.Y)
            {
                tempPoint = RandomPos(s);
            }

            return tempPoint;
        }

    }
}
