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
        List<Rectangle> trailRects = new List<Rectangle>();
        List<Point> trailPoints = new List<Point>();
        public Point trail { get; private set; }
        public Point headPos { get; private set; }
        public int score { get; private set; }
        private int velocityX;
        private int velocityY;
        private Rectangle Player;
        private Point position;
        private Canvas GameCanvas;

        //Cam
        public Snake(Canvas canvas)
        {
            GameCanvas = canvas;
            velocityX = 44;
            velocityY = 0;
            trail = new Point(0,-44);

            Player = new Rectangle();
            Player.Fill = Brushes.Blue;
            Player.Width = 42;
            Player.Height = 42;
            GameCanvas.Children.Add(Player);
            Canvas.SetLeft(Player, position.X + 2);
            Canvas.SetTop(Player, position.Y + 2);
        }

        public void Movement()
        {
            if (Keyboard.IsKeyDown(Key.Left) && velocityX != 44)
            {
                velocityY = 0;
                velocityX = -44;
            }

            else if (Keyboard.IsKeyDown(Key.Right) && velocityX != -44)
            {
                velocityY = 0;
                velocityX = 44;
            }

            else if (Keyboard.IsKeyDown(Key.Up) && velocityY != 44)
            {
                velocityX = 0;
                velocityY = -44;
            }

            else if (Keyboard.IsKeyDown(Key.Down) && velocityY != -44)
            {
                velocityX = 0;
                velocityY = 44;
            }

            position.X += velocityX;
            position.Y += velocityY;

            //Update Pos
            Canvas.SetLeft(Player, position.X + 2);
            Canvas.SetTop(Player, position.Y + 2);

            int i = 0;
            foreach(Rectangle rect in trailRects)
            {
                if (i == 0)
                {
                    Canvas.SetLeft(rect, headPos.X + 2);
                    Canvas.SetTop(rect, headPos.Y + 2);
                    trailPoints[i] = trail;
                    trail = headPos;
                }
                else if (i > 0)
                {
                    Canvas.SetLeft(rect, trailPoints[i - 1].X + 2);
                    Canvas.SetTop(rect, trailPoints[i - 1].Y + 2);
                    trailPoints[i] = trailPoints[i - 1];
                    trailPoints[i] = trail;
                }
                i++;
            }

            headPos = position;
            
        }

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

        public void Grow()
        {
            Rectangle trailRect = new Rectangle();
            Point trailPoint = new Point();
            trailPoint = trail;
            trailRect.Fill = Brushes.Blue;
            trailRect.Width = 42;
            trailRect.Height = 42;
            trailRects.Add(trailRect);
            trailPoints.Add(trailPoint);
            GameCanvas.Children.Add(trailRect);
            Canvas.SetLeft(trailRect, position.X + 2);
            Canvas.SetTop(trailRect, position.Y + 2);
        }
        //ToDo: Snake

    }
}