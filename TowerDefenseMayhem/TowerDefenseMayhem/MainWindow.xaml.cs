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

        private const double LoopTime = 100;

        

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
            Money.CashChange += Source_CashChange;
            DisplayMoney = 1000;
            NextLevel = 1;
            ReadyForNextLevel = true;
            
            //clear everything from old game if bool = false
        }

        private void StartNextLevel()
        {
            ReadyForNextLevel = false;

            Creeps = new Creeps();

            
            bool LevelOver = false;            
            ThreadStart startSpawning = new ThreadStart(SpawnCreeps);
            Thread thread = new Thread(startSpawning);
            thread.Start();
            
            DateTime dateTime = DateTime.Now;
            // time loop

                // update (ms)
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(LoopTime);
            TimeSpan leftoverTime = TimeSpan.FromMilliseconds(0);
            int waitTwentyLoops = 0;
            while (!LevelOver)
            {
                dateTime = DateTime.Now;
                Creeps.Update(timeSpan);
                if (DateTime.Now < dateTime + timeSpan)
                {
                    leftoverTime = timeSpan - (DateTime.Now - dateTime);
                    Thread.Sleep(leftoverTime);
                }
                if (Creeps.AllCreeps.Count() == 0 && waitTwentyLoops > 20)
                {
                    break;
                }
                waitTwentyLoops++;
                ReadyForNextLevel = true;
            }
            
            
        }
        
        private void SpawnCreeps()
        {
            for (int i=0; i<Level.GetCreepCount(NextLevel); i++)
            {
                Creep newCreep = new Creep(Creep.CreepType.Baby, Pathing.GetPath(NextLevel));
                Creeps.AllCreeps.Add(newCreep);
                Thread.Sleep(500);
            }
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

        
        private int DisplayMoney
        {
            get { return (int)GetValue(MoneyProperty); }
            set { SetValue(MoneyProperty, value); }
        }

        public static readonly DependencyProperty MoneyProperty = DependencyProperty.Register("DisplayMoney", typeof(int), typeof(MainWindow), new PropertyMetadata(1000));

        
        private void Source_CashChange(object sender, EventArgs e)
        {
            DisplayMoney = Money.Cash;
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
