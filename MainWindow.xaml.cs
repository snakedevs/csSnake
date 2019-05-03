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
         //ToDo (Cam): Globals,(Done)
        private int HighScores;
        private string HighScorePlayer;
        private Point StartingPos;
        private enum GameState {MainMenu, GameOn, GameOver, Settings}
        public Point Player { get; private set; }
        private int GameTimer;
        private Apple apple;
        
        //ToDo (Josh): GameTimer, Enum
        public MainWindow()
        {
            InitializeComponent();
            CreateMainMenu();
            CreateGrid();
        }
        //ToDo (Josh): Tick Method


        //ToDo (Anyone): Other Methods

        //ToDo (Josh): Leaderboard/Scores Methods

        //ToDo (Cam): Quit Game Method

        //ToDo (Cam): Game Over Method

        //ToDo (Dave): Start Game Method
        //ToDO (Dave): Create Main menu
        private void CreateMainMenu()
        {
            Button btn_StartGame = new Button();
            btn_StartGame.Content = "Click to start";
        }
        //David
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
                        w.Fill = Brushes.DarkSeaGreen;
                    }
                    else
                    {
                        w.Fill = Brushes.ForestGreen;
                    }
                    MainCanvas.Children.Add(w);
                    Canvas.SetTop(w, i * 46 + 2);
                    Canvas.SetLeft(w, j * 46 + 2);
                }
            }
        }

    }
}
