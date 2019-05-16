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
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.IO;
using System.Threading;
using System.Net;

namespace snakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Cam
        private int[] HighScores = new int[5];
        private string[] HighScorePlayer = new string[5];

        private enum GameState { MainMenu, GameOn, GameOver }
        private GameState gameState;

        public Snake Player { get; private set; }
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private DispatcherTimer keyboardTimer = new DispatcherTimer();
        private Apple apple;
        private Key lastKey;

        //Josh
        private string gameVersion = "1.0";

        //David
        private TextBlock tB_MainMenu;
        private TextBlock tB_GameOver;
        private Label lbl_Score;
        private Label lbl_Leaderboards;
        private TextBox tB_PlayerName;
        private Button btn_StartGame;
        private Button btn_Leaderboards;
        private Button btn_Controls;
        private Button btn_QuitGame;
        private Button btn_Main;
        private Button btn_Submit;
        private Rectangle display_Snake;
        private Rectangle display_Trail1;
        private Rectangle display_Trail2;
        private Rectangle display_Apple;
        private SolidColorBrush colorBrush = new SolidColorBrush();

        //Josh
        private UserCredential credential;
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Snake#";
        public static bool InternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        private Rectangle[] standingsRect = new Rectangle[5];



        //Cam
        public MainWindow()
        {
            InitializeComponent();
            gameState = GameState.MainMenu;
            colorBrush.Color = System.Windows.Media.Colors.White;

            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 10);

            keyboardTimer.Tick += keyboardTimer_Tick;
            keyboardTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);

            CreateMainMenu();

            ReadDataFromGoogleSheets();
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

        /// <summary>
        /// Everyone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //When game is at mainmenu
            //David
            if (gameState == GameState.MainMenu)
            {
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
                //David
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

        /// <summary>
        /// David 
        /// Runs when button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnControls_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
            displayCanvas.Visibility = Visibility.Hidden;
            Controls.Visibility = Visibility.Visible;

            btn_Main = new Button();
            btn_Main.FontSize = 30;
            btn_Main.FontSize = 30;
            btn_Main.Height = 70;
            btn_Main.Width = 290;
            btn_Main.Content = "Main Menu";
            btn_Main.Foreground = Brushes.White;
            btn_Main.Background = Brushes.Transparent;
            btn_Main.Click += returntomenufromcontrols_Click;

            Controls.Children.Add(btn_Main);

            Canvas.SetTop(btn_Main, 390);
            Canvas.SetLeft(btn_Main, 167);

            try
            {
                btn_Main.Style = this.FindResource("MenuButton") as Style;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        /// <summary>
        /// Cam
        /// Closes the program when the method is called
        /// </summary>
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

        //David
        private void GameOver()
        {
            GameCanvas.Children.Clear();
            GameCanvas.Visibility = Visibility.Hidden;
            Score.Visibility = Visibility.Visible;
            gameTimer.Stop();
            keyboardTimer.Stop();
            gameState = GameState.MainMenu;

            if (InternetConnection() == true)
            {
                Create_tB_PlayerName();
                Create_tB_GameOver();
                Create_btn_Submit();
                Create_btn_Main(Score, 340);
                Create_lbl_Score();
            }
            else
            {
                MessageBox.Show("Internet connection not found. As a result, leaderboards will not appear.");
            }
        }

        //David
        private void returntomenufromcontrols_Click(object sender, RoutedEventArgs e)
        {
            Controls.Visibility = Visibility.Hidden;
            CreateMainMenu();
            displayCanvas.Visibility = Visibility.Visible;
        }

        //David
        private void CreateMainMenu()
        {
            this.Title = "Snake " + gameVersion.ToString();
            int i = 1;
            tB_MainMenu = new TextBlock();
            btn_StartGame = new Button();
            btn_Leaderboards = new Button();
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
            setupButton(btn_Leaderboards, i);
            i++;
            btn_Leaderboards.Content = "Leaderboards";

            setupRectangle(display_Trail2, i);
            setupButton(btn_Controls, i);
            i++;
            btn_Controls.Content = "Controls";

            setupRectangle(display_Apple, i);
            setupButton(btn_QuitGame, i);
            btn_QuitGame.Content = "Quit Game";
        }

        //David
        /*private void CreateGrid()
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
        }*/

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
            else if (b == btn_Controls)
            {
                btn_Controls.Click += BtnControls_Click;
            }
            else if (b == btn_Leaderboards)
            {
                btn_Leaderboards.Click += Btn_Leaderboards_Click;
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

        private void Btn_Leaderboards_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
            displayCanvas.Children.Clear();
            CreateLeaderboards();
        }

        //David
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
                Canvas.SetTop(r, 264 - (44 * (i - 1)));
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
            Score.Children.Clear();
            Score.Visibility = Visibility.Hidden;
            MainCanvas.Visibility = Visibility.Visible;
            gameState = GameState.MainMenu;
            CreateMainMenu();
        }

        //Cam
        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            QuitGame();
        }

        /// <summary>
        /// Josh
        /// Create objects needed to display leaderboards and displays those objects
        /// </summary>
        private void CreateLeaderboards()
        {
            ReadDataFromGoogleSheets();

            for(int i = 0; i < standingsRect.Length; i++)
            {
                standingsRect[i] = new Rectangle();

                TextBlock place = new TextBlock();
                TextBlock player = new TextBlock();
                TextBlock score = new TextBlock();

                player.Text = HighScorePlayer[i];
                player.Width = 200;
                player.Height = standingsRect[i].Height;

                place.Text = (i + 1).ToString();
                place.Width = 50;
                place.Height = standingsRect[i].Height;

                score.Text = HighScores[i].ToString();
                score.Width = 75;
                score.Height = standingsRect[i].Height;


                if (i == 0)
                {
                    standingsRect[i].Stroke = Brushes.Gold;
                    standingsRect[i].StrokeThickness = 2;
                    standingsRect[i].Fill = Brushes.Transparent;
                }
                else if (i > 0 && i < 3)
                {
                    standingsRect[i].Stroke = Brushes.Silver;
                    standingsRect[i].StrokeThickness = 1.5;
                    standingsRect[i].Fill = Brushes.Transparent;
                }
                else
                {
                    standingsRect[i].Stroke = Brushes.Brown;
                    standingsRect[i].StrokeThickness = 1;
                    standingsRect[i].Fill = Brushes.Transparent;
                }

                standingsRect[i].Width = 400;
                standingsRect[i].Height = 70;
                Canvas.SetLeft(standingsRect[i], (this.Width - standingsRect[i].Width) / 2);
                Canvas.SetTop(standingsRect[i], 100 + (80 * i));
                Leaderboards.Children.Add(standingsRect[i]);
            }

            Create_btn_Main(Leaderboards, 500);
            Create_lbl_Leaderboards();
        }

        private void Create_tB_GameOver()
        {
            tB_GameOver = new TextBlock();
            tB_GameOver.Width = 310;
            tB_GameOver.Height = 70;
            tB_GameOver.Foreground = Brushes.White;
            tB_GameOver.Background = Brushes.Transparent;
            tB_GameOver.Text = "Game Over!";
            tB_GameOver.FontSize = 60;

            Score.Children.Add(tB_GameOver);
            Canvas.SetLeft(tB_GameOver, (this.Width - tB_GameOver.Width) / 2);
        }

        private void Create_lbl_Score()
        {
            lbl_Score = new Label();
            lbl_Score.Width = 210;
            lbl_Score.Height = 70;
            lbl_Score.FontSize = 30;
            lbl_Score.Foreground = Brushes.White;
            lbl_Score.Background = Brushes.Transparent;

            Score.Children.Add(lbl_Score);
            Canvas.SetTop(lbl_Score, 75);
            Canvas.SetLeft(lbl_Score, (this.Width - lbl_Score.Width) / 2);
            lbl_Score.Content = "Your Score is: " + Player.score;
        }

        private void Create_btn_Main(Canvas c, int top)
        {
            btn_Main = new Button();
            btn_Main.FontSize = 30;
            btn_Main.Height = 70;
            btn_Main.Width = 290;
            btn_Main.Content = "Main Menu";
            btn_Main.Foreground = Brushes.White;
            btn_Main.Background = Brushes.Transparent;
            btn_Main.Click += Returntomenufromscore_Click;
            c.Children.Add(btn_Main);
            Canvas.SetTop(btn_Main, top);
            Canvas.SetLeft(btn_Main, (this.Width - btn_Main.Width) / 2);

            try
            {
                btn_Main.Style = this.FindResource("MenuButton") as Style;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Create_btn_Submit()
        {
            btn_Submit = new Button();
            btn_Submit.Width = 290;
            btn_Submit.Height = 70;
            btn_Submit.FontSize = 30;
            btn_Submit.Foreground = Brushes.White;
            btn_Submit.Background = Brushes.Transparent;
            btn_Submit.Content = "Submit Score";
            btn_Submit.Style = this.FindResource("MenuButton") as Style;

            Score.Children.Add(btn_Submit);
            Canvas.SetTop(btn_Submit, 250);
            Canvas.SetLeft(btn_Submit, (this.Width - btn_Submit.Width) / 2);

            try
            {
                btn_Submit.Style = this.FindResource("MenuButton") as Style;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Create_tB_PlayerName()
        {
            tB_PlayerName = new TextBox();
            tB_PlayerName.Width = 320;
            tB_PlayerName.Height = 40;
            tB_PlayerName.FontSize = 30;
            tB_PlayerName.Foreground = Brushes.White;
            colorBrush.Opacity = 0.1;
            tB_PlayerName.Background = colorBrush;
            tB_PlayerName.BorderBrush = Brushes.Green;
            tB_PlayerName.BorderThickness = new Thickness(1);
            tB_PlayerName.TextAlignment = TextAlignment.Center;

            Score.Children.Add(tB_PlayerName);
            Canvas.SetTop(tB_PlayerName, 180);
            Canvas.SetLeft(tB_PlayerName, (this.Width - tB_PlayerName.Width) / 2);
            tB_PlayerName.Text = "Enter Name";
            tB_PlayerName.SelectionStart = tB_PlayerName.Text.Length;
            tB_PlayerName.Focus();
        }

        private void Create_lbl_Leaderboards()
        {
            lbl_Leaderboards = new Label();
            lbl_Leaderboards.Foreground = Brushes.White;
            lbl_Leaderboards.Background = Brushes.Transparent;
            lbl_Leaderboards.Width = 200;
            lbl_Leaderboards.Height = 70;
            lbl_Leaderboards.FontSize = 40;

            Leaderboards.Children.Add(lbl_Leaderboards);
            Canvas.SetTop(lbl_Leaderboards, 10);
            Canvas.SetLeft(lbl_Leaderboards, (this.Width - lbl_Leaderboards.Width) / 2);
            lbl_Leaderboards.Content = "Top Scores";
        }

        private void ReadDataFromGoogleSheets()
        {
            try
            {
                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Google Sheets API service.
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Define request parameters.
                String spreadsheetId = "1JDoFdfZ9if8r3kmKscUA-gEHQ91aU7ZDSlyY_O_qv8g";
                String range = "A:B";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);
                // Prints the names and majors of students in a sample spreadsheet:
                // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    int i = 0;
                    Console.WriteLine("Name, Score");
                    foreach (var row in values)
                    {
                        // Print columns A and B, which correspond to indices 0 and 1.
                        HighScorePlayer[i] = row[0].ToString();
                        int.TryParse(row[1].ToString(), out HighScores[i]);

                        i++;
                    }
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
                Console.Read();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}