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
using System.Threading;
using System.Timers;

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

        private bool LevelOver = true;   

        //public event EventHandler UpdateTime;
        //protected virtual void OnUpdateTime()
        //{
        //    if (UpdateTime != null) { UpdateTime(this, EventArgs.Empty); }
        //}

        private const double LoopTime = 100;
        //private bool UpdateCreeps = false;

        

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;            
            StartNewGame(true);

            System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
            myTimer.Interval = 100;
            myTimer.Tick += new EventHandler(TimerTickEvent);
            myTimer.Start();
        }

        public void StartNewGame(bool isFirstGame)
        {
            Pathing = new Pathing();
            Money = new Money();
            Level = new Level();
            Creeps = new Creeps();
            Money.CashChange += Source_CashChange;
            //UpdateTime += Source_UpdateCreeps;
            DisplayMoney = 1000;
            NextLevel = 1;
            ReadyForNextLevel = true;
            
            //clear everything from old game if bool = false
        }

        private void StartNextLevel()
        {
            ReadyForNextLevel = false;
            LevelOver = false;

                                 

            //ThreadStart startMoving = new ThreadStart(MoveCreeps);
            //Thread thread = new Thread(startMoving);
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();

            for (int i = 0; i < 1; i++) //(int i = 0; i < Level.GetCreepCount(NextLevel); i++)
            {                                
                //Thread.Sleep(500);
                Creep newCreep = new Creep(Creep.CreepType.Baby, Pathing.GetPath(NextLevel), TDMCanvas);
                Creeps.AllCreeps.Add(newCreep);
            }
            //MoveCreeps();
        }

        private object locker = new object();

        private void TimerTickEvent(object sender, System.EventArgs e)
        {
            if (!LevelOver)
            {
                MoveCreeps();
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
                Creeps.Update(timeSpan);
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

        //private void Source_UpdateCreeps(object sender, EventArgs e)
        //{
        //    if (sender.ToString() != Money.ToString())
        //    {
        //        TimeSpan timeSpan = TimeSpan.FromMilliseconds(LoopTime);
        //        Creeps.Update(timeSpan);
        //    }
        //}
       
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
