/* ToDo (Anyone): Add stuff here
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace snakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //ToDo (Cam): Globals

        //ToDo (Josh): GameTimer, Enum

        public MainWindow()
        {
            InitializeComponent();
            CreateGrid();

        }
        //ToDo (Josh): Tick Method

        //ToDo (Anyone): Other Methods

        //ToDo (Josh): Leaderboard/Scores Methods

        //ToDo (Cam): Quit Game Method

        //ToDo (Cam): Game Over Method

        //ToDo (Dave): Start Game Method

        //ToDo (Dave): Create Grid Method
        private void CreateGrid()
        {
            for (int j = 0; j < 17; j++)
            {
                for (int i = 0; i < 17; i++)
                {
                    Rectangle w = new Rectangle();
                    w.Height = 44;
                    w.Width = 44;
                    if ((j + i) % 2 == 0)
                    {
                        w.Fill = Brushes.Gray;
                    }
                    else
                    {
                        w.Fill = Brushes.DarkGray;
                    }
                    MainCanvas.Children.Add(w);
                    Canvas.SetTop(w, i * 46 + 2);
                    Canvas.SetLeft(w, j * 46 + 2);
                }
            }
        }

    }
}