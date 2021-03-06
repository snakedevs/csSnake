﻿/* Josh Degazio, Cameron Heinz, David Laughton
 * Mon April 6th, 1947
 * Re-make snake in WPF C#
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
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
        //Initialize basic global variables
        private int[] TopHighScores = new int[5];
        private string[] TopHighScorePlayer = new string[5];
        private List<string> AllEntries = new List<string>();
        private string[] AllPlayers = new string[1];
        private int[] AllScores = new int[1];
        private bool goingUp = false;

        //Initialize cooler stuff
        private enum GameState { MainMenu, GameOn, GameOver }
        private GameState gameState;

        public Snake Player { get; private set; }
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private DispatcherTimer keyboardTimer = new DispatcherTimer();
        private Apple apple;
        private Key lastKey;

        //Josh
        //Initialize gameversion
        private string gameVersion = "1.0";

        //David
        //Initialize WPF objects
        private TextBlock tB_MainMenu;
        private TextBlock tB_GameOver;
        private TextBlock tB_Creators;
        private TextBlock tB_PressKey;
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
        //Initialize globals used for leaderboards
        private UserCredential credential;
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
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
        private String spreadsheetId = "1JDoFdfZ9if8r3kmKscUA-gEHQ91aU7ZDSlyY_O_qv8g";
        private String range;
        private string playerName;
        private string playerScore;

        /// <summary>
        /// Cam
        /// Initializes MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //Start at the mainmenu
            gameState = GameState.MainMenu;
            //Create brush in order to manipulate opacity easier
            colorBrush.Color = Colors.White;

            //Edit properties of gametimer
            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 10);

            //Edit properties of keyboardtimer
            keyboardTimer.Tick += keyboardTimer_Tick;
            keyboardTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 120);

            //Start at the mainmenu
            CreateMainMenu();
        }

        //Josh
        //Checks for keyboard input more often than gametimer so there is a smaller chance of missing key presses.
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

            //When game is being played
            //Josh
            if (gameState == GameState.GameOn)
            {
                //Create objects if they don't exist
                if (GameCanvas.Children.Count == 0)
                {
                    GameStart();
                    lastKey = Key.DbeAlphanumeric;
                }
                //If the snake collides with an apple, Generate a new apple, and increase the score
                if (Player.EatsApple(Player.headPos, apple.Position) == true)
                {
                    this.Title = "Snake " + gameVersion + " - Score: " + Player.score;
                    apple.SelfDestruct(GameCanvas);
                    apple = new Apple(GameCanvas, Player);
                }

                //If the last key is a random key that it was set to earlier
                if (lastKey == Key.DbeAlphanumeric)
                {
                    //Make the textblock fade in
                    if (tB_PressKey.Opacity >= 0 && goingUp == true)
                    {
                        tB_PressKey.Opacity += .1;
                        if (tB_PressKey.Opacity == 1)
                        {
                            goingUp = false;
                        }
                    }
                    //and fade out
                    else if (tB_PressKey.Opacity <= 1 && goingUp == false)
                    {
                        tB_PressKey.Opacity -= .1;
                        if (tB_PressKey.Opacity <= .05)
                        {
                            goingUp = true;
                        }
                    }
                }
                //If a key has been pressed, but the textblock is visible, make it visible
                else if (lastKey != Key.DbeAlphanumeric && tB_PressKey.Opacity != 0)
                {
                    tB_PressKey.Opacity = 0;
                    goingUp = false;
                }
                //Otherwise, move the snake
                else if (lastKey == Key.Up || lastKey == Key.Down || lastKey == Key.Left || lastKey == Key.Right)
                {
                    Player.Movement(lastKey);
                }

                //If the head of the snake is no longer within the window
                if (CheckOutOfBounds() == true)
                {
                    //End the game
                    gameState = GameState.GameOver;
                }

                //For each snake trail rectangle
                foreach (Point p in Player.trailPoints)
                {
                    //if the snake head is overtop of the rectangle
                    if (CheckCollision(p, Player.headPos) == true)
                    {
                        //End the game
                        gameState = GameState.GameOver;
                    }
                }
            }

            //Cam
            //When game is over, run gameover method
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
            //Manipulate canvas'
            MainCanvas.Children.Clear();
            displayCanvas.Visibility = Visibility.Hidden;
            Controls.Visibility = Visibility.Visible;

            //Create a button to take user back to mainmenu
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

            //Try to change the style of the button such that when the user's mouse enters the button,
            //The button's background doesn't change color, but instead, the text becomes more transparent.
            try
            {
                btn_Main.Style = this.FindResource("MenuButton") as Style;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        /// <summary>
        /// David
        /// Creates the gamecanvas objects needed to play the game.
        /// </summary>
        private void GameStart()
        {
            //Create gamecanvas objects
            GameCanvas.Visibility = Visibility.Visible;
            Player = new Snake(GameCanvas);
            apple = new Apple(GameCanvas, Player);
            lastKey = new Key();
            lastKey = Key.DbeAlphanumeric;
            
            //Start timers
            gameTimer.Start();
            keyboardTimer.Start();

            //Create Textblock
            tB_PressKey = new TextBlock();
            tB_PressKey.Text = "Press an arrow key to start.";
            tB_PressKey.Foreground = Brushes.White;
            tB_PressKey.Background = Brushes.Transparent;
            tB_PressKey.FontSize = 40;
            GameCanvas.Children.Add(tB_PressKey);
            tB_PressKey.Width = 475;
            tB_PressKey.Height = 60;
            Canvas.SetLeft(tB_PressKey , (this.Width - tB_PressKey.Width) / 2 );
            Canvas.SetTop(tB_PressKey, (this.Height - tB_PressKey.Height) / 2);
        }

        /// <summary>
        /// David
        /// Clears gamecanvas, displays user score, if internet connection is available, 
        /// presents the user with an area to type their name and submit their score to a google sheet
        /// </summary>
        private void GameOver()
        {
            //Clear objects
            GameCanvas.Children.Clear();
            //Change Visibilities
            GameCanvas.Visibility = Visibility.Hidden;
            Score.Visibility = Visibility.Visible;
            //Stop timers
            gameTimer.Stop();
            keyboardTimer.Stop();
            //Change gamestate
            gameState = GameState.MainMenu;
            //Create objects
            Create_btn_Main(Score, 340);
            Create_lbl_Score();
            Create_tB_GameOver();

            //If user is connected to internet, display area for user to enter their name,
            // and submit it to the google sheet
            if (InternetConnection() == true)
            {
                Create_tB_PlayerName();
                Create_btn_Submit();
            }
            else
            {
                MessageBox.Show("Internet connection not found. As a result, leaderboards will not appear.");
            }
        }

        /// <summary>
        /// Cameron
        /// Returns to the mainmenu from the controls canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void returntomenufromcontrols_Click(object sender, RoutedEventArgs e)
        {
            //Manipulate canvas'
            Controls.Visibility = Visibility.Hidden;
            CreateMainMenu();
            displayCanvas.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// David
        /// Creates the objects used in the mainmenu
        /// </summary>
        private void CreateMainMenu()
        {
            //Set window title
            this.Title = "Snake " + gameVersion.ToString();
            //Initialize temporary integer that will later be used in this method
            int i = 1;
            //Create objects
            tB_MainMenu = new TextBlock();
            tB_Creators = new TextBlock();
            btn_StartGame = new Button();
            btn_Leaderboards = new Button();
            btn_Controls = new Button();
            btn_QuitGame = new Button();
            display_Snake = new Rectangle();
            display_Trail1 = new Rectangle();
            display_Trail2 = new Rectangle();
            display_Apple = new Rectangle();

            //Edit properties of the textblock that will display the title at the mainmenu
            tB_MainMenu.FontSize = 50;
            tB_MainMenu.Text = "Snake#";
            tB_MainMenu.TextAlignment = TextAlignment.Center;
            tB_MainMenu.Width = MainCanvas.Width;
            tB_MainMenu.Foreground = Brushes.White;
            MainCanvas.Children.Add(tB_MainMenu);
            
            //Edit properties of the textblock that will display the creators names at the mainmenu
            tB_Creators.FontSize = 17;
            tB_Creators.Text = "Josh, Cameron and David";
            tB_Creators.TextAlignment = TextAlignment.Center;
            tB_Creators.FontWeight = FontWeights.SemiBold;
            tB_Creators.Width = MainCanvas.Width;
            tB_Creators.Foreground = Brushes.White;
            MainCanvas.Children.Add(tB_Creators);
            Canvas.SetTop(tB_Creators, 60);

            //Setup various buttons and rectangles by passing the objects through to a method.
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

        /// <summary>
        /// David
        /// Check if the snake's head has moved over top of it's tail.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool CheckCollision(Point a, Point b)
        {
            //If points that were passed through are over top of each other, return true.
            if (a.X == b.X && a.Y == b.Y)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Josh
        /// Check if the snake is outside of the window.
        /// </summary>
        /// <returns></returns>
        private bool CheckOutOfBounds()
        {
            //If the player is no longer within the bounds of the window return true.
            if (Player.headPos.X >= this.MaxWidth - 44 || Player.headPos.X < 0 ||
                Player.headPos.Y >= this.MaxHeight - 44 || Player.headPos.Y < 0)
            {
                return true;
            }

            else return false;
        }

        /// <summary>
        /// Josh
        /// Creates buttons and edits their properties.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="i"></param>
        private void setupButton(Button b, int i)
        {
            //set button properties
            b.FontSize = 30;
            b.Height = 70;
            b.Width = MainCanvas.Width - 20;
            b.Foreground = Brushes.White;
            b.Background = Brushes.Transparent;

            //set button click events
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
            else if (b == btn_QuitGame)
            {
                btn_QuitGame.Click += Btn_QuitGame_Click;
            }

            //Set the style of the button
            try
            {
                b.Style = this.FindResource("MenuButton") as Style;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

            //Add button
            MainCanvas.Children.Add(b);

            //Position the button
            Canvas.SetTop(b, (90 * i));
            Canvas.SetLeft(b, 10);
            Canvas.SetRight(b, 10);
        }

        /// <summary>
        /// Cam
        /// Closes the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_QuitGame_Click(object sender, RoutedEventArgs e)
        {
            //Close the window
            this.Close();
        }

        /// <summary>
        /// Cameron
        /// If the user has internet connection, display the leaderboards
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Leaderboards_Click(object sender, RoutedEventArgs e)
        {
            //If the user has an internet connection
            if (InternetConnection() == true)
            {
                //Take the player to the leaderboards page
                MainCanvas.Children.Clear();
                displayCanvas.Children.Clear();
                CreateLeaderboards();
            }
            else MessageBox.Show("Please connect to internet to access the leaderboards.");
        }

        /// <summary>
        /// David
        /// Starts the game by manipulating canvas'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_StartGame_Click(object sender, RoutedEventArgs e)
        {
            //Clear objects
            GameCanvas.Children.Clear();
            MainCanvas.Children.Clear();
            displayCanvas.Children.Clear();

            //Turn on timers and change gamestate
            gameState = GameState.GameOn;
            gameTimer.Start();
            keyboardTimer.Start();
        }
  
        /// <summary>
        /// Josh
        /// Edits properties of rectangles that are passed to the method.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="i"></param>
        private void setupRectangle(Rectangle r, int i)
        {
            //Set width and height
            r.Width = 44;
            r.Height = 44;

            //If the rectangle is a specific one, set specific properties
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

            //add rectangle to canvas
            displayCanvas.Children.Add(r);

        }

        /// <summary>
        /// Cameron
        /// Returns the user to the menu from any area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturntoMenu_Click(object sender, RoutedEventArgs e)
        {
            if (Score.Children.Count > 0)
            {
                Score.Children.Clear();
                Score.Visibility = Visibility.Hidden;
            }
            else if (Leaderboards.Children.Count > 0)
            {
                Leaderboards.Children.Clear();
            }
            MainCanvas.Visibility = Visibility.Visible;
            gameState = GameState.MainMenu;
            CreateMainMenu();
        }

        /// <summary>
        /// Josh
        /// Create objects needed to display leaderboards and displays those objects
        /// </summary>
        private void CreateLeaderboards()
        {
            //method
            ReadDataFromGoogleSheets();

            //For each rectangle that should exist
            for(int i = 0; i < standingsRect.Length; i++)
            {
                //Create a new rectangle
                standingsRect[i] = new Rectangle();

                //Create new textblocks
                TextBlock place = new TextBlock();
                TextBlock player = new TextBlock();
                TextBlock score = new TextBlock();

                //Set player properties
                player.Text = TopHighScorePlayer[i];
                player.Width = 200;
                player.Height = standingsRect[i].Height;
                player.Foreground = Brushes.White;
                player.FontSize = 30;

                //Set place properties
                if (i + 1 == 1)
                {
                    place.Text = (i + 1).ToString() + "st";
                }
                else if (i + 1 == 2)
                {
                    place.Text = (i + 1).ToString() + "nd";
                }
                else if (i + 1 == 3)
                {
                    place.Text = (i + 1).ToString() + "rd";
                }
                else if (i + 1 > 3)
                {
                    place.Text = (i + 1).ToString() + "th";
                }
                place.Width = 50;
                place.Height = standingsRect[i].Height;
                place.Foreground = Brushes.White;
                place.FontSize = 24;

                //Set score properties
                score.Text = "Score: " + TopHighScores[i].ToString();
                if (TopHighScores[i] < 10)
                {
                    score.Width = 80;
                }
                else if (TopHighScores[i] < 100)
                {
                    score.Width = 90;
                }
                else { score.Width = 100; }
                score.Height = standingsRect[i].Height;
                score.Foreground = Brushes.White;
                score.FontSize = 20;

                //Add objects to canvas
                Leaderboards.Children.Add(player);
                Leaderboards.Children.Add(place);
                Leaderboards.Children.Add(score);

                //Position objects
                Canvas.SetTop(player, 115 + (80 * i));
                Canvas.SetTop(place, 120 + (80 * i));
                Canvas.SetTop(score, 120 + (80 * i));

                Canvas.SetLeft(player, 175);
                Canvas.SetLeft(place, 125);
                Canvas.SetLeft(score, 410);

                //Color code the standings by Gold, Silver, Bronze.
                if (i == 0)
                {
                    standingsRect[i].Stroke = Brushes.Gold;
                    standingsRect[i].StrokeThickness = 4;
                    standingsRect[i].Fill = Brushes.Transparent;
                }
                else if (i == 1)
                {
                    standingsRect[i].Stroke = Brushes.Silver;
                    standingsRect[i].StrokeThickness = 2;
                    standingsRect[i].Fill = Brushes.Transparent;
                }
                else if (i == 2)
                {
                    standingsRect[i].Stroke = Brushes.Brown;
                    standingsRect[i].StrokeThickness = 1;
                    standingsRect[i].Fill = Brushes.Transparent;
                }
                else
                {
                    standingsRect[i].Stroke = Brushes.DarkGray;
                    standingsRect[i].StrokeThickness = 0.25;
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

        /// <summary>
        /// Josh
        /// Creates textblock
        /// </summary>
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

        /// <summary>
        /// Josh
        /// Creates label
        /// </summary>
        private void Create_lbl_Score()
        {
            lbl_Score = new Label();

            if (Player.score < 10)
            {
                lbl_Score.Width = 210;
            }
            else if (Player.score < 100)
            {
                lbl_Score.Width = 225;
            }
            else { lbl_Score.Width = 240; }
            lbl_Score.Height = 70;
            lbl_Score.FontSize = 30;
            lbl_Score.Foreground = Brushes.White;
            lbl_Score.Background = Brushes.Transparent;

            Score.Children.Add(lbl_Score);
            Canvas.SetTop(lbl_Score, 75);
            Canvas.SetLeft(lbl_Score, (this.Width - lbl_Score.Width) / 2);
            lbl_Score.Content = "Your Score is: " + Player.score;
        }

        /// <summary>
        /// Josh
        /// Creates button, given the canvas to create it in, and position from the top of the window
        /// </summary>
        /// <param name="c"></param>
        /// <param name="top"></param>
        private void Create_btn_Main(Canvas c, int top)
        {
            btn_Main = new Button();
            btn_Main.FontSize = 30;
            btn_Main.Height = 70;
            btn_Main.Width = 290;
            btn_Main.Content = "Main Menu";
            btn_Main.Foreground = Brushes.White;
            btn_Main.Background = Brushes.Transparent;
            btn_Main.Click += ReturntoMenu_Click;
            c.Children.Add(btn_Main);
            Canvas.SetTop(btn_Main, top);
            Canvas.SetLeft(btn_Main, (this.Width - btn_Main.Width) / 2);

            try
            {
                btn_Main.Style = this.FindResource("MenuButton") as Style;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Josh
        /// Creates button to submit user score to leaderboards.
        /// </summary>
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
            btn_Submit.Click += Btn_Submit_Click;

            Score.Children.Add(btn_Submit);
            Canvas.SetTop(btn_Submit, 250);
            Canvas.SetLeft(btn_Submit, (this.Width - btn_Submit.Width) / 2);

            try
            {
                btn_Submit.Style = this.FindResource("MenuButton") as Style;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Josh
        /// Click event for submit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Submit_Click(object sender, RoutedEventArgs e)
        {
            Score.Children.Clear();
            Score.Visibility = Visibility.Hidden;

            playerName = tB_PlayerName.Text;
            playerScore = Player.score.ToString();
            ReadDataFromGoogleSheets();
            WriteDataToGoogleSheets();

            CreateLeaderboards();
        }

        /// <summary>
        /// Josh
        /// Creates textbox for user to enter their name
        /// </summary>
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

        /// <summary>
        /// Cameron
        /// Creates label to display to user that the top scores are being displayed
        /// </summary>
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

        /// <summary>
        /// Josh
        /// Reads data from the google sheet "LeaderboardSheet" and puts them into a list
        /// </summary>
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
                        CancellationToken.None).Result;
                        
                }

                // Create Google Sheets API service.
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                range = "A1:B";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);

                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    AllEntries.Clear();
                    int i = 0;
                    Console.WriteLine("Name, Score");
                    foreach (var row in values)
                    {
                        // Print columns A and B, which correspond to indices 0 and 1.
                        AllEntries.Add(row[0].ToString() + "," + row[1].ToString());

                        i++;
                    }
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
                Console.Read();
                SortData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Josh
        /// Sorts the data from the list "AllEntries"
        /// </summary>
        private void SortData()
        {
            int i = 0;
            //For every entry in AllEntries
            foreach (string a in AllEntries)
            {
                //Create an int array called AllScores
                int.TryParse(a.Split(',')[1], out AllScores[i]);
                Array.Resize(ref AllScores, AllScores.Length + 1);
                i++; 
            }

            //Sort the int array by the default comparer (lowest value) by descending (sorts by highest value instead)
            AllScores = AllScores.OrderByDescending(p => p).ToArray();

            //Find the name that was originally uploaded with the newly sorted score.
            for(int j = 0; j < TopHighScores.Length; j++)
            {
                foreach (string a in AllEntries)
                { 
                    if (a.Contains(AllScores[j].ToString()))
                    {
                        string[] tempString;
                        tempString = a.Split(',');
                        TopHighScorePlayer[j] = tempString[0];
                        int.TryParse(tempString[1], out TopHighScores[j]);
                        AllEntries.RemoveAt(AllEntries.IndexOf(a));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Josh
        /// Inserts data into three different columns, one row long. 
        /// The data contains the player name, the player score,
        /// as well as the date recorded.
        /// </summary>
        private void WriteDataToGoogleSheets()
        {
            try
            {
                range = "A:C" + AllEntries.Count();
                var valueRange = new ValueRange();

                // Create Google Sheets API service.
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                var oblist = new List<object>() { playerName, playerScore, DateTime.Today };
                valueRange.Values = new List<IList<object>> { oblist };

                var request = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
                request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var reponse = request.Execute();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            
        }
    }
}