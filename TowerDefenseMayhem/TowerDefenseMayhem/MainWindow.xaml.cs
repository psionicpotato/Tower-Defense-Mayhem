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
using System.Threading;
using System.Timers;
using System.ComponentModel;

namespace TowerDefenseMayhem
{
    public partial class MainWindow : Window
    {
        
        private int NextLevel;
        private bool ReadyForNextLevel;
        private Pathing Pathing;
        private Money Money;
        private Creeps Creeps;
        private Level Level;
        private Player Player;

        private bool LevelOver = true;   
        private const double LoopTime = 20;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;            
            StartNewGame(true);
        }

        public void StartNewGame(bool isFirstGame)
        {
            Pathing = new Pathing();
            Money = new Money();
            Level = new Level();
            Creeps = new Creeps();
            Player = new Player();
            Money.CashChange += Source_CashChange;
            Player.LifeChange += Source_LifeChange;
            DisplayMoney = 1000;
            DisplayLives = 5;
            NextLevel = 1;
            ReadyForNextLevel = true;
            
        }

        private BackgroundWorker bw = new BackgroundWorker();
        private BackgroundWorker bw2 = new BackgroundWorker();

        private void StartNextLevel()
        {
            ReadyForNextLevel = false;
            LevelOver = false;

            // Begin move creep worker
            bw2.WorkerReportsProgress = true;

            bw2.DoWork += new DoWorkEventHandler(bw_SpawnCreeps);
            bw.DoWork += new DoWorkEventHandler(bw_MoveCreeps);

            bw2.RunWorkerAsync();
            bw.RunWorkerAsync();
        }

        private void bw_SpawnCreeps(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                // spawn creeps
                for (int i = 0; i < Level.GetCreepCount(NextLevel); i++)
                {
                    double spawnPeriod = 5;// seconds
                    Creep newCreep = new Creep(Creep.CreepType.Baby, Pathing.GetPath(NextLevel), TDMCanvas, this);
                    Creeps.AllCreeps.Add(newCreep);

                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(spawnPeriod));
                }
            }
        }

        private void bw_MoveCreeps(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                // perform loop through each existing creep
                while (!LevelOver)
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(LoopTime));
                    Creeps.Update(TimeSpan.FromMilliseconds(LoopTime));
                }
                LevelOver = true;
                ReadyForNextLevel = true;
                System.Windows.MessageBox.Show("done");
            }
        }

        private void MoveCreeps()
        {
            DateTime dateTime = DateTime.Now;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(LoopTime);
            TimeSpan leftoverTime = TimeSpan.FromMilliseconds(0);
            int waitEightSeconds = 0;

            while (!LevelOver)
            {
                dateTime = DateTime.Now;

                //OnUpdateTime();
                
                if (DateTime.Now < dateTime + timeSpan)
                {
                    leftoverTime = timeSpan - (DateTime.Now - dateTime);
                    if (leftoverTime > TimeSpan.FromSeconds(0))
                    {
                        Thread.Sleep(leftoverTime);
                    }                    
                }
                if (Creeps.AllCreeps.Count() == 0 && waitEightSeconds > 80)
                {
                    break;
                }
                waitEightSeconds++;
            }
            LevelOver = true;
            ReadyForNextLevel = true;
            System.Windows.MessageBox.Show("done");
        }
       
        private void StartNextLevel_Click(object sender, EventArgs e)
        {
            if (ReadyForNextLevel)
            {
                StartNextLevel();
            }
            else
            {
                System.Windows.MessageBox.Show("Breh, you gotta finish this level first!");
            }
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to quit?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Start a New Game?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {                
                StartNewGame(false);
            }
        }

        
        private void DebugMoney_Click(object sender, RoutedEventArgs e)
        {
            Money.AddMoney(1000);           
        }

        private void DebugLife_Click(object sender, RoutedEventArgs e)
        {
            Player.AddLives(5); ;
        }

        public void LoseLife()
        {
            bool lose = Player.LoseLives(-1);
            if (lose) { LoseGame(); }
        }

        private void LoseGame()
        {
            System.Windows.MessageBox.Show("YOU LOST THE GAME, BITCH.");
            StartNewGame(false);
        }

        
        private int DisplayMoney
        {
            get { return (int)GetValue(MoneyProperty); }
            set { SetValue(MoneyProperty, value); }
        }

        public static readonly DependencyProperty MoneyProperty = DependencyProperty.Register("DisplayMoney", typeof(int), typeof(MainWindow), new PropertyMetadata(1000));

        
        private void Source_CashChange(object sender, EventArgs e)
        {
            if (sender.ToString() == Money.ToString())
            {
                DisplayMoney = Money.Cash;
            }
        }

        private int DisplayLives
        {
            get { return (int)GetValue(LifePropery); }
            set { SetValue(LifePropery, value); }
        }

        public static readonly DependencyProperty LifePropery = DependencyProperty.Register("DisplayLives", typeof(int), typeof(MainWindow), new PropertyMetadata(5));


        private void Source_LifeChange(object sender, EventArgs e)
        {
            if (sender.ToString() == Player.ToString())
            {
                DisplayLives = Player.Lives;
            }
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

    }


}
