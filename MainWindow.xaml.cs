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
        //Cam
        private int[] HighScores = new int[11];
        private string[] HighScorePlayer = new string[11];

        private enum GameState { MainMenu, GameOn, GameOver }
        private GameState gameState;

        public Snake Player { get; private set; }
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private DispatcherTimer keyboardTimer = new DispatcherTimer();
        private Apple apple;
        private Key lastKey;

        //Josh
        private double gameVersion = 1.0;

        //David
        private TextBlock tB_MainMenu;
        private TextBlock tb_GameOver;
        private TextBlock tb_Score;
        private Button btn_StartGame;
        private Button btn_Options;
        private Button btn_Controls;
        private Button btn_QuitGame;
        private Button btn_ExitGame;
        private Rectangle display_Snake;
        private Rectangle display_Trail1;
        private Rectangle display_Trail2;
        private Rectangle display_Apple;

        //Cam
        public MainWindow()
        {
            InitializeComponent();
            gameState = GameState.MainMenu;

            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 10);

            keyboardTimer.Tick += keyboardTimer_Tick;
            keyboardTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);

            CreateMainMenu();


        }

        //Josh
        private void keyboardTimer_Tick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Down))
            {
                lastKey = Key.Down;
            }
            else if (Keyboard.IsKeyDown(Key.Left))
            {
                lastKey = Key.Left;
            }
            else if (Keyboard.IsKeyDown(Key.Right))
            {
                lastKey = Key.Right;
            }
            else if (Keyboard.IsKeyDown(Key.Up))
            {
                lastKey = Key.Up;
            }
        }

        //Everyone
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //When game is at mainmenu
            //David
            if (gameState == GameState.MainMenu)
            {
                //David
                if (btn_Controls.IsPressed)
                {
                    MainCanvas.Visibility = Visibility.Hidden;
                    Controls.Visibility = Visibility.Visible;
                }
                //Cam
                if (btn_QuitGame.IsPressed)
                {
                    QuitGame();
                }
            }

            //When game is being played
            //Josh
            else if (gameState == GameState.GameOn)
            {
                if (GameCanvas.Children.Count == 0)
                {
                    GameStart();
                }

                if (Player.EatsApple(Player.headPos, apple.Position) == true)
                {
                    this.Title = "Snake " + gameVersion + " - Score: " + Player.score;
                    apple.SelfDestruct(GameCanvas);
                    apple = new Apple(GameCanvas, Player);
                }

                Player.Movement(lastKey);

                if (CheckOutOfBounds() == true)
                {
                    gameState = GameState.GameOver;
                }

                foreach (Point p in Player.trailPoints)
                {
                    if (CheckCollision(p, Player.headPos) == true)
                    {
                        gameState = GameState.GameOver;
                    }
                }
            }

            //When game has ended
            //Cam
            else if (gameState == GameState.GameOver)
            {
                if (GameCanvas.Children.Count >= 1)
                {
                    GameOver();
                    //Display leaderboards 
                }
            }
        }

        //Cam
        private void QuitGame()
        {
            this.Close();
        }

        //David
        private void GameStart()
        {
            //CreateGrid();
            GameCanvas.Visibility = Visibility.Visible;
            Player = new Snake(GameCanvas);
            apple = new Apple(GameCanvas, Player);
            lastKey = new Key();
            gameTimer.Start();
            keyboardTimer.Start();
        }

        //ToDo (Anyone): Other Methods

        //ToDo (Josh): Leaderboard/Scores Methods

        //Cam
        private void GameOver()
        {
            GameCanvas.Children.Clear();
            GameCanvas.Visibility = Visibility.Hidden;
            Score.Visibility = Visibility.Visible;
            gameTimer.Stop();
            keyboardTimer.Stop();
            gameState = GameState.MainMenu;
            CreateMainMenu();
            score.Content = "Your Score is: " + Player.score;
        }

        //David
        private void returntomenufromcontrols_Click(object sender, RoutedEventArgs e)
        {
            Controls.Visibility = Visibility.Hidden;
            CreateMainMenu();
            MainCanvas.Visibility = Visibility.Visible;
        }

        //David
        private void CreateMainMenu()
        {
            this.Title = "Snake " + gameVersion + ".0";
            int i = 1;
            tB_MainMenu = new TextBlock();
            btn_StartGame = new Button();
            btn_Options = new Button();
            btn_Controls = new Button();
            btn_QuitGame = new Button();
            display_Snake = new Rectangle();
            display_Trail1 = new Rectangle();
            display_Trail2 = new Rectangle();
            display_Apple = new Rectangle();

            tB_MainMenu.FontSize = 50;
            tB_MainMenu.Text = "Snake#";
            tB_MainMenu.TextAlignment = TextAlignment.Center;
            tB_MainMenu.Width = MainCanvas.Width;
            tB_MainMenu.Foreground = Brushes.White;
            MainCanvas.Children.Add(tB_MainMenu);

            setupRectangle(display_Snake, i);
            setupButton(btn_StartGame, i);
            i++;
            btn_StartGame.Content = "Start Game";

            setupRectangle(display_Trail1, i);
            setupButton(btn_Options, i);
            i++;
            btn_Options.Content = "Options";

            setupRectangle(display_Trail2, i);
            setupButton(btn_Controls, i);
            i++;
            btn_Controls.Content = "Controls";

            setupRectangle(display_Apple, i);
            setupButton(btn_QuitGame, i);
            btn_QuitGame.Content = "Quit Game";
        }

        //David
        private void CreateGrid()
        {
            for (int j = 0; j < 14; j++)
            {
                for (int i = 0; i < 14; i++)
                {
                    Rectangle w = new Rectangle();
                    w.Height = 42;
                    w.Width = 42;

                    if ((j + i) % 2 == 0)
                    {
                        w.Fill = Brushes.Black;
                    }
                    else
                    {
                        w.Fill = Brushes.Black;
                    }

                    GameCanvas.Children.Add(w);
                    Canvas.SetTop(w, i * 44 + 2);
                    Canvas.SetLeft(w, j * 44 + 2);
                }
            }
        }

        //David
        private bool CheckCollision(Point a, Point b)
        {
            if (a.X == b.X && a.Y == b.Y)
            {
                return true;
            }
            else return false;
        }

        //Josh
        private bool CheckOutOfBounds()
        {
            if (Player.headPos.X >= this.MaxWidth - 44 || Player.headPos.X < 0 ||
                Player.headPos.Y >= this.MaxHeight - 44 || Player.headPos.Y < 0)
            {
                return true;
            }

            else return false;
        }

        //Josh
        private void setupButton(Button b, int i)
        {
            b.FontSize = 30;
            b.Height = 70;
            b.Width = MainCanvas.Width - 20;
            b.Foreground = Brushes.White;
            b.Background = Brushes.Transparent;

            if (b == btn_StartGame)
            {
                btn_StartGame.Click += Btn_StartGame_Click;
            }

            try
            {
                b.Style = this.FindResource("MenuButton") as Style;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

            MainCanvas.Children.Add(b);

            Canvas.SetTop(b, (90 * i));
            Canvas.SetLeft(b, 10);
            Canvas.SetRight(b, 10);
        }

        private void Btn_StartGame_Click(object sender, RoutedEventArgs e)
        {
            GameCanvas.Children.Clear();
            MainCanvas.Children.Clear();
            displayCanvas.Children.Clear();

            gameState = GameState.GameOn;
            gameTimer.Start();
            keyboardTimer.Start();
        }

        //Josh
        private void setupRectangle(Rectangle r, int i)
        {
            r.Width = 44;
            r.Height = 44;


            if (r == display_Snake)
            {
                Canvas.SetTop(r, 264);
                Canvas.SetLeft(r, 88);
                r.Fill = Brushes.LightGreen;
            }
            else if (r == display_Trail1 || r == display_Trail2)
            {
                Canvas.SetTop(r, 264 - (44 * (i-1)));
                Canvas.SetLeft(r, 88);
                r.Fill = Brushes.ForestGreen;
            }
            else
            {
                Canvas.SetTop(r, 528);
                Canvas.SetLeft(r, 484);
                r.Fill = Brushes.Red;
            }


            displayCanvas.Children.Add(r);

        }

        //Cam
        private void Returntomenufromscore_Click(object sender, RoutedEventArgs e)
        {
            Score.Visibility = Visibility.Hidden;
            MainCanvas.Visibility = Visibility.Visible;
            gameState = GameState.MainMenu;
            CreateMainMenu();
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            QuitGame();
        }
    }
}