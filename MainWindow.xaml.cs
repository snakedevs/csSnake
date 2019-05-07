/* Josh Degazio, Cameron Heinz, David Laughton
 * Mon April 6th, 1947
 * Re-make snake in WPF C#
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
        // Cam
        private int[] HighScores = new int[11];
        private string[] HighScorePlayer = new string[11];
        private Point StartingPos;

        private enum GameState { MainMenu, GameOn, GameOver }
        private GameState gameState;

        public Snake Player { get; private set; }
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private Apple apple;

        // David
        private Button btn_StartGame;
        private TextBlock tB_MainMenu;
        private Button btn_Controls;

        // Everyone
        public MainWindow()
        {
            InitializeComponent();
            gameState = GameState.MainMenu;

            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 15);
            gameTimer.Start();

            CreateMainMenu();

        }

        // Josh
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //When game is at mainmenu
            if (gameState == GameState.MainMenu)
            {
                if (btn_StartGame.IsPressed)
                {
                    MainCanvas.Visibility = Visibility.Hidden;
                    MainCanvas.Children.Clear();

                    gameState = GameState.GameOn;
                }
                //David
                if (btn_Controls.IsPressed)
                {
                    MainCanvas.Visibility = Visibility.Hidden;
                    Controls.Visibility = Visibility.Visible;
                }

            }

            //When game is being played
            else if (gameState == GameState.GameOn)
            {
                if (GameCanvas.Children.Count == 0)
                {
                    //Run Game Start method
                    //Which will include
                    CreateGrid();
                    GameCanvas.Visibility = Visibility.Visible;
                    Player = new Snake(GameCanvas);
                }
            }

            //When game has ended
            else if (gameState == GameState.GameOver)
            {
                if (GameCanvas.Children.Count >= 1)
                {
                    //Run Game Over method
                    //Display leaderboards 
                }
            }
        }

        //ToDo (Anyone): Other Methods

        //ToDo (Josh): Leaderboard/Scores Methods

        //ToDo (Cam): Quit Game Method

        //ToDo (Cam): Game Over Method

        //ToDo (Dave): Start Game Method

        //DAvid
        private void returntomenufromcontrols_Click(object sender, RoutedEventArgs e)
        {
            Controls.Visibility = Visibility.Hidden;
            CreateMainMenu();
            MainCanvas.Visibility = Visibility.Visible;
        }

        //David
        private void CreateMainMenu()
        {
            btn_StartGame = new Button();
            tB_MainMenu = new TextBlock();
            btn_Controls = new Button();

            tB_MainMenu.FontSize = 40;
            tB_MainMenu.Text = "  Welcome to Snake! ";

            btn_StartGame.FontSize = 40;
            btn_StartGame.Content = "Click to start";
            btn_StartGame.Height = 100;
            btn_StartGame.Width = 356;

            btn_Controls.FontSize = 40;
            btn_Controls.Content = "Click to see controls";
            btn_Controls.Height = 100;
            btn_Controls.Width = 356;

            MainCanvas.Children.Add(tB_MainMenu);
            MainCanvas.Children.Add(btn_StartGame);
            MainCanvas.Children.Add(btn_Controls);

            Canvas.SetTop(btn_StartGame, 70);
            Canvas.SetLeft(btn_StartGame, 10);
            Canvas.SetRight(btn_StartGame, 10);

            Canvas.SetTop(btn_Controls, 180);
            Canvas.SetLeft(btn_Controls, 10);
            Canvas.SetRight(btn_Controls, 10);
        }

        //David
        private void CreateGrid()
        {
            for (int j = 0; j < 16; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    Rectangle w = new Rectangle();
                    w.Height = 42;
                    w.Width = 42;
                    if ((j + i) % 2 == 0)
                    {
                        w.Fill = Brushes.DarkSeaGreen;
                    }
                    else
                    {
                        w.Fill = Brushes.ForestGreen;
                    }
                    GameCanvas.Children.Add(w);
                    Canvas.SetTop(w, i * 44 + 2);
                    Canvas.SetLeft(w, j * 44 + 2);
                }
            }
        }

       
    }
}