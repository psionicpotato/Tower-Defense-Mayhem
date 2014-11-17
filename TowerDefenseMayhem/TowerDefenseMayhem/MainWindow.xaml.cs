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
using System.Drawing;

namespace TowerDefenseMayhem
{
    public partial class MainWindow : Window
    {
        
        private int NextLevel;
        private bool ReadyForNextLevel;
        private Pathing Pathing;
        private Money Money;
        public Creeps Creeps;
        private Level Level;
        private Player Player;
        private List<Tower> AllTowers;

        private bool LevelOver = true;   
        private const double LoopTime = 20;


        public int NumberOfCreeps
        {
            get { return Creeps.AllCreeps.Count(); }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;            
            StartNewGame(true);
        }

        public void StartNewGame(bool isFirstGame)
        {
            NextLevel = 1;
            if (!isFirstGame)
            {
                bw.Dispose();
                bw2.Dispose();
            }
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
            AllTowers = new List<Tower>();
            
        }

        private BackgroundWorker bw;
        private BackgroundWorker bw2;
        private BackgroundWorker bw3;

        private void StartNextLevel()
        {
            if (NumberOfCreeps != 0)
            {
                System.Windows.MessageBox.Show("Breh, you gotta finish this level first!");
                return;
            }
            bw = new BackgroundWorker();
            bw2 = new BackgroundWorker();
            bw3 = new BackgroundWorker();

            ReadyForNextLevel = false;
            LevelOver = false;

            bw = new BackgroundWorker();
            bw2 = new BackgroundWorker();
            bw3 = new BackgroundWorker();

            bw2.WorkerReportsProgress = true;
            bw3.WorkerReportsProgress = true;

            bw3.DoWork += new DoWorkEventHandler(bw_TowersScan);
            bw2.DoWork += new DoWorkEventHandler(bw_SpawnCreeps);
            bw.DoWork += new DoWorkEventHandler(bw_MoveCreeps);

            bw3.RunWorkerAsync();
            bw2.RunWorkerAsync();
            bw.RunWorkerAsync();
        }

        private void bw_TowersScan(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if ((worker.CancellationPending))
            {
                e.Cancel = true;
            }
            else
            {
                // towers scan
                while (!LevelOver)
                {
                    foreach (Tower t in AllTowers)
                    {
                        t.Scan(TimeSpan.FromMilliseconds(LoopTime));
                    }
                }
            }
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

        private void Window_KeyDown(object sender, KeyEventArgs eKey)
        {
            System.Windows.Point pos = Mouse.GetPosition(this.TDMCanvas);

            if (eKey.Key == Key.F3)
            {
                Money.AddMoney(1000);
            }
            else if (eKey.Key == Key.F4)
            {
                Player.AddLives(5);
            }

            else if (eKey.Key == Key.A)
            {                             
                if (CheckIfLegalLocation(pos))
                {
                    if (Money.RequestPurchase(200))
                    {
                        Tower t = new Tower(Tower.TowerType.Arrow, Convert.ToInt16(pos.X), Convert.ToInt16(pos.Y), this.TDMCanvas, this);
                        AllTowers.Add(t);
                        //System.Windows.MessageBox.Show(pos.X + ", " + pos.Y);
                    }
                    // instantiate tower here
                    
                    
                }
            }
        }

        private bool CheckIfLegalLocation(System.Windows.Point position)
        {
            

            if (position.X < 25 || position.X > 775 || position.Y < 25 || position.X > 775)
            {
                return false;
            }
            else if (position.X < 638 && position.Y < 238 && position.Y > 162)
            {
                return false;
            }
            else if (position.X < 638 && position.X > 562 && position.Y > 162 && position.Y < 638)
            {
                return false;
            }
            else if (position.X < 638 && position.Y < 638 && position.Y > 562)
            {
                return false;
            }


            foreach (Tower t in AllTowers)
            {
                if ((Math.Abs(position.X - t.PosX) < 50) && (Math.Abs(position.Y - t.PosY) < 50))
                {
                    return false;
                }
            }

            return true;
        }
       
        private void StartNextLevel_Click(object sender, EventArgs e)
        {            
            StartNextLevel();            
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
