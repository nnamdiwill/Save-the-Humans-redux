using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace Save_the_Humans_redux
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        bool humanCaptured = false;
    
        public MainWindow()
        {
            InitializeComponent();

            enemyTimer.Tick += EnemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            targetTimer.Tick += TargetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);
        }

        private void TargetTimer_Tick(object sender, EventArgs e)
        {
            ProgressBar.Value += 1;
            if (ProgressBar.Value >= ProgressBar.Maximum)
                EndTheGame();
        }

        private void EndTheGame()
        {
            if(!playArea.Children.Contains(gamEoverText))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                playArea.Children.Add(gamEoverText);
            }
            throw new NotImplementedException();
        }

        private void EnemyTimer_Tick(object sender, EventArgs e)
        {
            AddEnemy();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            human.IsHitTestVisible = true;
            humanCaptured = false;
            ProgressBar.Value = 0;
            startButton.Visibility = Visibility.Collapsed;
            playArea.Children.Clear();
            playArea.Children.Add(Target);
            playArea.Children.Add(human);
            enemyTimer.Start();
            targetTimer.Start();
        }

        private void AddEnemy()
        {
            ContentControl enemy = new ContentControl();
        enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
        AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.left)");
        AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100),
            random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
        playArea.Children.Add(enemy);
            enemy.MouseEnter += Enemy_MouseEnter;
        }

         void Enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
                EndTheGame();
        }

        private void AnimateEnemy(ContentControl enemy,double from, double to, string propertyToAnimate)
    {
        Storyboard storyboard = new Storyboard()
        {
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };
        DoubleAnimation animation = new DoubleAnimation()
        {
            From = from,
            To = to,
            Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6))),
        };
        Storyboard.SetTarget(animation, enemy);
        Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }

        private void human_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                human.IsHitTestVisible = false;
            }
        }

        private void target_MouseEnter(object sender, MouseEventArgs e)
        {
            if(targetTimer.IsEnabled && humanCaptured)
            {
                ProgressBar.Value = 0;
                Canvas.SetLeft(Target, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(Target, random.Next(100, (int)playArea.ActualHeight - 100));
                Canvas.SetLeft(human, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)playArea.ActualHeight - 100));
                humanCaptured = false;
                human.IsHitTestVisible = true;
            }
        }

        private void playArea_MouseMove(object sender, MouseEventArgs e)
        {
         
            if(humanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(playArea).Transform(pointerPosition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth * 3) 
                      ||  (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight * 3)){

                    humanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }

            }   
        }

        private void playArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (humanCaptured) EndTheGame();

        }
    }
}
