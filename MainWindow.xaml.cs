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
using System.Windows.Threading;

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
        private enum GameState { MainMenu, GameOn, GameOver, Settings }
        public Point Player { get; private set; }
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private Apple apple;

        //ToDo (Josh): GameTimer, Enum
        public MainWindow()
        {
            InitializeComponent();

            GameState gameState;
            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 15);
            gameTimer.Start();

            CreateMainMenu();
            CreateGrid();
        }
        //ToDo (Josh): Tick Method
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //if (gamest)
        }

        //ToDo (Anyone): Other Methods

        //ToDo (Josh): Leaderboard/Scores Methods

        //ToDo (Cam): Quit Game Method

        //ToDo (Cam): Game Over Method

        //ToDo (Dave): Start Game Method
        //ToDO (Dave): Create Main menu
        private void CreateMainMenu()
        {
            TextBlock MainMenu = new TextBlock();
            MainMenu.FontSize = 40;
            MainMenu.Text = "  Welcome to Snake! ";
            Button btn_StartGame = new Button();

            btn_StartGame.FontSize = 40;
            btn_StartGame.Content = "Click to start";
            btn_StartGame.Height = 100;
            btn_StartGame.Width = 356;

            MainCanvas.Children.Add(MainMenu);
            MainCanvas.Children.Add(btn_StartGame);
            Canvas.SetTop(btn_StartGame, 70);
            Canvas.SetLeft(btn_StartGame, 10);
            Canvas.SetRight(btn_StartGame, 10);
            if (btn_StartGame.IsPressed)
            {
                MainCanvas.Visibility = Visibility.Hidden;
                GameCanvas.Visibility = Visibility.Visible;
            }
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