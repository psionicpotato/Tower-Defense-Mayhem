using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

///<summary>
///Note: use Queue rather than List for creeps in a tower's range
///</summary>
namespace TowerDefenseMayhem
{
    class Tower
    {
        // spacial params
        public int PosX;
        public int PosY;

        public int Range;
        public int Damage;
        public int SplashRange;

        public TowerType MyTowerType;
        public enum TowerType { Arrow, Flame, Radial, Bomb }
        public Weapon MyWeapon;
        public Creep TargetCreep;
        public Canvas MyCanvas;
        public BitmapSource MyBitmapSource;
        public Image MyImage;
        public MainWindow MainWindow;

        // Constructor
        public Tower(TowerType myTowerType, int posX, int posY, Canvas myCanvas, MainWindow mainWindow) 
        {
            MainWindow = mainWindow;
            MyCanvas = myCanvas;
            MyTowerType = myTowerType;

            PosX = posX;
            PosY = posY;

            string uriPath;
            // use case select based on towertype
            switch (myTowerType)
            {
                case TowerType.Arrow:
                    // need to add new image and update path
                    uriPath = @"..\..\Tower Images\tower.png"; 

                    // set initial weapon
                    MyWeapon = new Weapon(100, 100, 2);

                    break;
                case TowerType.Flame:
                    // need to add new image and update path
                    uriPath = @"..\..\Tower Images\tower.png"; 

                    // set initial weapon
                    MyWeapon = new Weapon(10, 100, 1);

                    break;
                case TowerType.Radial:
                    // need to add new image and update path
                    uriPath = @"..\..\Tower Images\tower.png";

                    // set initial weapon

                    break;
                default:
                    uriPath = "";
                    break;
            }

            // instantiate at posX, posY
            MyCanvas.Dispatcher.Invoke(new Action(() => DrawTower(uriPath)));

        }

        private void DrawTower(string uriPath)
        {
            Uri imguri = new Uri(uriPath, UriKind.Relative);
            PngBitmapDecoder dec = new PngBitmapDecoder(imguri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            MyBitmapSource = dec.Frames[0];
            MyImage = new System.Windows.Controls.Image();
            MyImage.Source = MyBitmapSource;
            MyCanvas.Children.Add(MyImage);
            MyImage.SetValue(Canvas.LeftProperty, (double) PosX - Math.Floor(MyBitmapSource.Width / 2));
            MyImage.SetValue(Canvas.TopProperty, (double) PosY - Math.Floor(MyBitmapSource.Height / 2));
        }

        // change to dictionary instead? save the range already calculated and
        //  pass it on to SetTargetNearest?        
        private List<Creep> CreepsInRange(List<Creep> allCreeps)
        {
            // loop through creeps, create collection of those close enough
            //  to attack
            List<Creep> ans = new List<Creep>();

            foreach (Creep c in allCreeps)
            {
                double hypDistance = 
                    Math.Sqrt(Math.Pow(Math.Abs(c.PosX - this.PosX), 2) 
                        + Math.Abs(c.PosY - this.PosY));
                if (hypDistance <= MyWeapon.Range) 
                {
                    ans.Add(c);
                }
            }

            return ans;
        }

        // maybe include in FindCreepsInRange for now, easy enough to do
        public void SetTargetNearest(List<Creep> creepsInRange)
        {
            // loop through creep collection positions and find nearest one
        }

        // call this after current target leaves range or dies
        public void AcquireNextTarget(List<Creep> globalCreeps)
        {
            List<Creep> creepsInRange = new List<Creep>();
            //creepsInRange = FindCreepsInRange(globalCreeps);
            SetTargetNearest(creepsInRange);
        }

        public void AttackCreep(Creep _creep)
        {
            // pythagorean to find hypoteneus distance
            double hypDistance = 
                Math.Sqrt(Math.Pow(Math.Abs(_creep.PosX - this.PosX), 2) 
                    + Math.Abs(_creep.PosY - this.PosY));

            // is in range to attack?
            if (hypDistance <= MyWeapon.Range)
            {
                // attack!
                _creep.HitPoints -= MyWeapon.Damage;
            }
        }

        public class Weapon
        {
            private double CooldownMax;
            public double CooldownCurrent;
            public double Range;
            public double Damage;

            // Constructor
            public Weapon(double cooldownMax, double range, double damage)
            {
                CooldownMax = cooldownMax;
                Range = range;
                Damage = damage;
            }
        }

        // attack creep
    }
}
