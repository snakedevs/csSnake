using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace snakeGame
{
    public class Snake
    {
        //Dave        
        private List<Rectangle> trailRects = new List<Rectangle>();
        public List<Point> trailPoints { get; private set; }
        public Point trail { get; private set; }
        public int score { get; private set; }
        private int velocityX;
        private int velocityY;
        private Rectangle Player;
        public Point headPos { get => position; }
        private Point position;
        private Canvas GameCanvas;

        //Cam
        public Snake(Canvas canvas)
        {
            trailPoints = new List<Point>();
            GameCanvas = canvas;
            velocityX = 44;
            velocityY = 0;

            Player = new Rectangle();
            Player.Fill = Brushes.LightGreen;
            Player.Width = 42;
            Player.Height = 42;
            GameCanvas.Children.Add(Player);
            Canvas.SetLeft(Player, headPos.X + 2);
            Canvas.SetTop(Player, headPos.Y + 2);
        }

        //David and Josh
        public void Movement(Key last)
        {
            if (last == Key.Left && velocityX != 44)
            {
                velocityY = 0;
                velocityX = -44;
            }
            else if (last == Key.Right && velocityX != -44)
            {
                velocityY = 0;
                velocityX = 44;
            }
            else if (last == Key.Up && velocityY != 44)
            {
                velocityX = 0;
                velocityY = -44;
            }
            else if (last == Key.Down && velocityY != -44)
            {
                velocityX = 0;
                velocityY = 44;
            }

            position.X += velocityX;
            position.Y += velocityY;

            //Update Pos
            Canvas.SetLeft(Player, position.X + 2);
            Canvas.SetTop(Player, position.Y + 2);

            int i = trailRects.Count() - 1;
            foreach (Rectangle rect in trailRects)
            {
                if (i == 0)
                {
                    Canvas.SetLeft(rect, trail.X + 2);
                    Canvas.SetTop(rect, trail.Y + 2);
                    trailPoints[i] = trail;
                }
                else if (i > 0)
                {
                    trailPoints[i] = trailPoints[i - 1];
                    Canvas.SetLeft(rect, trailPoints[i].X + 2);
                    Canvas.SetTop(rect, trailPoints[i].Y + 2);
                }
                i--;
            }
            trail = position;
        }

        //Dave
        public bool EatsApple(Point a, Point b)
        {
            if (a.X == b.X && a.Y == b.Y)
            {
                score++;
                Grow();
                return true;
            }
            else
            {
                return false;
            }
        }

        //Josh
        public void Grow()
        {
            Rectangle trailRect = new Rectangle();
            Point trailPoint = new Point();
            trailPoint = trail;

            trailRect.Fill = Brushes.ForestGreen;
            trailRect.Width = 42;
            trailRect.Height = 42;
            trailRects.Add(trailRect);
            trailPoints.Add(trailPoint);
            GameCanvas.Children.Add(trailRect);
            Canvas.SetLeft(trailRect, position.X + 2);
            Canvas.SetTop(trailRect, position.Y + 2);
        }
    }
}