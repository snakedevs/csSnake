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
    public class Snake
    {
        //Dave        
        Rect[] snake;
        public int trail { get; private set; }
        public Point headPos { get; private set; }
        public int score { get; private set; }
        private int velocity = 3;


        //Cam
        public Snake(Canvas canvas)
        {

            Rectangle a = new Rectangle();
            a.Height = 100;
            a.Width = 100;
            canvas.Children.Add(a);
            Canvas.SetLeft(a, 100);
            Canvas.SetTop(a, 100);
            a.Fill = Brushes.Blue;

        }

        public void Movement()
        {

        }

    }
}