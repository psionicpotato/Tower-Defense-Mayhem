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
                // case TowerType.Flame:
                //     // need to add new image and update path
                //     uriPath = @"..\..\Tower Images\tower.png"; 

                //     // set initial weapon
                //     MyWeapon = new Weapon(10, 100, 1);

                //     break;
                // case TowerType.Radial:
                //     // need to add new image and update path
                //     uriPath = @"..\..\Tower Images\tower.png";

                //     // set initial weapon

                //     break;
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
                double hypDistance = GetDistanceToCreep(c);
                if (hypDistance <= MyWeapon.Range) 
                { /* in range */
                    ans.Add(c);
                }
                else
                { /* out of range */
                    ans.Remove(c);
                }
            }

            return ans;
        }

        // maybe include in FindCreepsInRange for now, easy enough to do
        private void SetTargetNearest(List<Creep> creepsInRange)
        {
            // loop through creep collection positions and find nearest one
            foreach (Creep c in creepsInRange)
            {
                if (TargetCreep == null)
                { /* acquire new target */
                    TargetCreep = c;
                }
                else
                { /* check for closer target */
                    if (GetDistanceToCreep(c) < GetDistanceToCreep(TargetCreep))
                    { /* switch to closer target */
                        TargetCreep = c;
                    }
                }
            }
        }

        private double GetDistanceToCreep(Creep c)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(c.PosX - this.PosX), 2)
                        + Math.Abs(c.PosY - this.PosY));
        }

        // call this after current target leaves range or dies
        private void AcquireNextTarget(List<Creep> globalCreeps)
        {
            List<Creep> creepsInRange = new List<Creep>();
            //creepsInRange = FindCreepsInRange(globalCreeps);
            SetTargetNearest(creepsInRange);
        }

        private void AttackCreep(Creep _creep)
        {
            // pythagorean to find hypoteneus distance
            double hypDistance = GetDistanceToCreep(_creep);

            // is in range to attack?
            if (hypDistance <= MyWeapon.Range)
            {
                // attack!
                _creep.HitPoints -= MyWeapon.Damage;

                // trigger cooldown state
                MyWeapon.CooldownCurrent = MyWeapon.CooldownMax;
            }
        }

        public class Weapon
        {
            public double CooldownMax;
            public double CooldownCurrent;
            public double Range;
            public double Damage;

            // Constructor
            public Weapon(double cooldownMax, double range, double damage)
            {
                CooldownMax = cooldownMax;
                CooldownCurrent = cooldownMax; // start in cooldown
                Range = range;
                Damage = damage;
            }
        }

        // scan for and attack creep, goes with creep 'Move' method
        public void Scan(TimeSpan timeSpan)
        {
            // generate "creeps in range" list and acquire target
            AcquireNextTarget(CreepsInRange(MainWindow.Creeps.AllCreeps));

            // attack target if "cooled down"
            if (MyWeapon.CooldownCurrent == 0)
            {
                AttackCreep(TargetCreep);
            }
        }
    }
}
