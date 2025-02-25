using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CrappyBird
{
    public partial class MainWindow : Window
    {
        // Main game timer
        DispatcherTimer mainTimer = new DispatcherTimer();

        // Game variables
        int score;
        int gravity = 8;
        Rect birdHitBox;

        public MainWindow()
        {
            InitializeComponent();

            // Show title screen
            showCanvas(titleScreenCanvas);

            // Setup and start game timer
            mainTimer.Tick += mainTimerTick;
            mainTimer.Interval = TimeSpan.FromMilliseconds(20);
        }

        public void showCanvas(Canvas canvas)
        {
            // Place title screen in front
            canvas.Margin = new Thickness(0, -29, 0, -6);
            Canvas.SetTop(canvas, 0);
            Canvas.SetLeft(canvas, 0);
            Canvas.SetZIndex(canvas, 10);
            canvas.Focus();
        }

        public void hideCanvas(Canvas canvas)
        {
            // Place title screen in front
            canvas.Margin = new Thickness(0, -29, 0, -6);
            Canvas.SetTop(canvas, 10000);  // Just moving this to russia
            Canvas.SetLeft(canvas, 10000); //
        }

        private void mainTimerTick(object sender, EventArgs e)
        {
            // Update UI label
            scoreLBL.Content = "Score: " + score;

            // Update bird hitbox
            birdHitBox = new Rect(Canvas.GetLeft(Bird), Canvas.GetTop(Bird), Bird.Width - 12, Bird.Height);

            // Apply gravity to bird
            Canvas.SetTop(Bird, Canvas.GetTop(Bird) + gravity);

            // Check if bird is out of screen
            if (Canvas.GetTop(Bird) < -40 || Canvas.GetTop(Bird) + Bird.Height > 460)
            {
                endGame();
            }

            // Iterate through all images in mainCanvas
            foreach (var x in mainCanvas.Children.OfType<Image>())
            {
                // Filter for specific tags
                if ((string)x.Tag == "pipe1" || (string)x.Tag == "pipe2" || (string)x.Tag == "pipe3")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);

                    // If pipes are out of screen, reset their position
                    if (Canvas.GetLeft(x) < -100)
                    {
                        Canvas.SetLeft(x, 800);

                        // If survived one iteration, add bonus points
                        score += 5;
                    }

                    // Check for collision
                    Rect pipeHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (birdHitBox.IntersectsWith(pipeHitBox))
                    {
                        endGame();
                    }
                }
            }

            // Add a collisionbox for the ground
            Rect groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);
            if (birdHitBox.IntersectsWith(groundHitBox))
            {
                endGame();
            }

        }

        private void keyIsDown(object sender, KeyEventArgs e)
        {
            // Jump if "Space" pressed
            if (e.Key == Key.Space)
            {
                // Tilt bird
                Bird.RenderTransform = new RotateTransform(-50, Bird.Width / 2, Bird.Height / 2);
                
                // Reduce gravity
                gravity = -5;
            }
        }

        private void keyIsUp(object sender, KeyEventArgs e)
        {
            // Apply downward gravity when space key is released
            Bird.RenderTransform = new RotateTransform(5, Bird.Width / 2, Bird.Height / 2);
            gravity = 8;
        }

        private void startGame()
        {
            // Set focus to game canvas
            mainCanvas.Focus();

            score = 0;

            // Set initial bird pos
            Canvas.SetTop(Bird, 190);

            // Set initial pipe positions
            foreach (var x in mainCanvas.Children.OfType<Image>())
            {
                if ((string)x.Tag == "pipe1")
                {
                    Canvas.SetLeft(x, 500);
                }
                if ((string)x.Tag == "pipe2")
                {
                    Canvas.SetLeft(x, 800);
                }
                if ((string)x.Tag == "pipe3")
                {
                    Canvas.SetLeft(x, 1100);
                }
            }

            // Start the game timer
            mainTimer.Start();
        }

        private void endGame()
        {
            // Stop timer and set gameover
            mainTimer.Stop();

            // Check for highscore
            int currentHighscore = Properties.Settings.Default.highscore;
            bool newHighscore = false;
            if (currentHighscore < score)
            {
                newHighscore = true;
                Properties.Settings.Default.highscore = score;
                Properties.Settings.Default.Save();
            }

            // Show "Game Over" screen
            gameOverScoreLBL.Content = "Score: " + score;
            if (newHighscore)
            {
                gameOverScoreLBL.Content = "NEW HIGHSCORE:  " + score;
            }
            showCanvas(gameOverCanvas);
        }

        private void startGameBTN_Click(object sender, RoutedEventArgs e)
        {
            // Hide title screen
            hideCanvas(titleScreenCanvas);

            // start game
            startGame();
        }

        private void titleScreenBTN_Click(object sender, RoutedEventArgs e)
        {
            // Hide "Game Over" screen
            hideCanvas(gameOverCanvas);

            // Show title screen
            showCanvas(titleScreenCanvas);
        }

        private void retryBTN_Click(object sender, RoutedEventArgs e)
        {
            // Hide "Game Over" screen
            hideCanvas(gameOverCanvas);

            // Start game
            startGame();
        }
    }
}
