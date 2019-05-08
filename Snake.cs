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
    enum PlayerState { alive, dead };
    public class Snake
    {
        //Dave        
        Rect[] snake;
        public int trail { get; private set; }
        public Point headPos { get; private set; }
        public int score { get; private set; }
        private int velocityX;
        private int velocityY;
        private Rectangle Player;
        private Point position;
        private PlayerState state;

        //Cam
        public Snake(Canvas canvas)
        {
            state = PlayerState.alive;
            velocityX = 44;
            velocityY = 0;

            Player = new Rectangle();
            Player.Fill = Brushes.Blue;
            Player.Width = 42;
            Player.Height = 42;
            canvas.Children.Add(Player);
            Canvas.SetLeft(Player, position.Y + 2);
            Canvas.SetTop(Player, position.X + 2);

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
            headPos = position;
        }

        public bool EatsApple(Point a, Point b)
        {
            if (a.X == b.X && a.Y == b.Y)
            {
                score++;
                return true;
            }
            else
            {
                return false;
            }
        }
        //ToDo: Snake

    }
}