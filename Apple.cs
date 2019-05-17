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
        public Point Position { get; private set; }
        private Random r = new Random();
        private Rectangle apple;
        private int height = 42;
        private int width = 42;

        //David
        private static int randomNumberMethod(Random r)
        {
            //word generation for easy + hard
            int randomnumber;
            //generates random number within 1-16
            randomnumber = r.Next(1, 14);
            return randomnumber;
        }

        /// <summary>
        /// Cam
        /// Apple constructor method
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="s"></param>
        public Apple(Canvas canvas, Snake s)
        {
            Generate(s);
            canvas.Children.Add(apple);
            Canvas.SetTop(apple, Position.Y + 2);
            Canvas.SetLeft(apple, Position.X + 2);
        }

        /// <summary>
        /// Cam
        /// Generates new apple given the snakes current position
        /// so that the apple does not generate on the snake.
        /// </summary>
        /// <param name="s"></param>
        private void Generate(Snake s)
        {
            Position = RandomPos(s);
            apple = new Rectangle();
            apple.Height = height;
            apple.Width = width;
            apple.Fill = Brushes.Red;
        }

        /// <summary>
        /// Cam
        /// Generate a random point for the apple to be generated at.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Point RandomPos(Snake s)
        {
            bool isOnSnake = false;
            Point tempPoint = new Point();

            tempPoint.X = randomNumberMethod(r) * (height + 2);
            tempPoint.Y = randomNumberMethod(r) * (width + 2);

            foreach (Point p in s.trailPoints)
            {
                if (p.X == tempPoint.X && p.Y == tempPoint.Y)
                {
                    isOnSnake = true;
                }
            }
            if (isOnSnake == false)
            {
                return tempPoint;
            }
            else
            {
                return RandomPos(s);
            }

        }

        /// <summary>
        /// Cam
        /// Remove the apple from the canvas in which it exists
        /// </summary>
        /// <param name="canvas"></param>
        public void SelfDestruct(Canvas canvas)
        {
            canvas.Children.Remove(apple);
        }

    }
}