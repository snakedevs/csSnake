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
        private int velocity = 3;
        private Rectangle Player;
        private Point position;
        private PlayerState state;

        //Cam
        public Snake(Canvas canvas)
        {
            state = PlayerState.alive;
            velocity = 3;

            Player = new Rectangle();
            Player.Fill = Brushes.Blue;
            Player.Width = 42;
            Player.Height = 42;
            canvas.Children.Add(Player);
            Canvas.SetLeft(Player, position.Y + 2);
            Canvas.SetTop(Player, position.X + 2);

            if (state == PlayerState.alive)
            {
                if (Keyboard.IsKeyDown(Key.Left))
                {
                    position.X += 10;
                    state = PlayerState.alive;
                }
                if (Keyboard.IsKeyDown(Key.Right))
                {
                    position.X -= 10;
                    state = PlayerState.alive;
                }
                if (Keyboard.IsKeyDown(Key.Up))
                {
                    position.Y += 10;
                    state = PlayerState.alive;
                }
                if (Keyboard.IsKeyDown(Key.Down))
                {
                    position.Y -= 10;
                    state = PlayerState.alive;
                }
            }
        }
        //ToDo: Snake

    }
}