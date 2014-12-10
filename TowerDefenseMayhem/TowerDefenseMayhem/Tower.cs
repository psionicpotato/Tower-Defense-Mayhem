using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

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
        public string pathImage;
        public string pathImage_attack;
        public double attackAnimationLength;

        // Constructor
        public Tower(TowerType myTowerType, int posX, int posY, Canvas myCanvas, MainWindow mainWindow) 
        {
            MainWindow = mainWindow;
            MyCanvas = myCanvas;
            MyTowerType = myTowerType;

            PosX = posX;
            PosY = posY;

            // use case select based on towertype
            switch (myTowerType)
            {
                case TowerType.Arrow:
                    // need to add new image and update path
                    pathImage = @"..\..\Tower Images\tower.png";
                    pathImage_attack = @"..\..\Tower Images\tower_attack.png";
                    attackAnimationLength = 200;

                    // set initial weapon
                    MyWeapon = new Weapon(500, 100, 2);

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
                    pathImage = "";
                    break;
            }

            // instantiate at posX, posY
            MyCanvas.Dispatcher.Invoke(new Action(() => DrawTower(pathImage)));

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
            //System.Diagnostics.Debug.Print("Passin in " + allCreeps.Count + "creeps to CreepsInRange.");
            // create temp instance of list
            List<Creep> tempAllCreeps = new List<Creep>(allCreeps);

            // loop through creeps, create collection of those close enough
            //  to attack
            List<Creep> ans = new List<Creep>();

            foreach (Creep c in tempAllCreeps)
            {
                double hypDistance = GetDistanceToCreep(c);
                if (hypDistance <= MyWeapon.Range) 
                { /* in range */                    
                    ans.Add(c);
                    //System.Diagnostics.Debug.Print("Creeps in range of tower: " + ans.Count);
                }
            }

            return ans;
        }

        // maybe include in FindCreepsInRange for now, easy enough to do
        private Creep GetTargetNearest(List<Creep> creepsInRange)
        {
            // loop through creep collection positions and find nearest one
            foreach (Creep c in creepsInRange)
            {
                if (TargetCreep == null)
                { /* acquire new target */
                    return c;
                }
                else
                { /* check for closer target */
                    if (GetDistanceToCreep(c) < GetDistanceToCreep(TargetCreep))
                    { /* switch to closer target */
                        return c;
                    }
                }
            }
            return null;
        }

        private double GetDistanceToCreep(Creep c)
        {
            if (c != null)
            {
                return Math.Sqrt(Math.Pow(Math.Abs(c.PosX - this.PosX), 2)
                        + Math.Pow(Math.Abs(c.PosY - this.PosY), 2));
            }
            else
            {
                return 0;
            }
        }

        //// call this after current target leaves range or dies
        //private Creep GetNextTarget(List<Creep> globalCreeps)
        //{
        //    List<Creep> creepsInRange = new List<Creep>();
        //    ////creepsInRange = FindCreepsInRange(globalCreeps);
        //    return GetTargetNearest(creepsInRange);
        //}

        private void AttackCreep(Creep _creep)
        {
            // pythagorean to find hypoteneus distance
            double hypDistance = GetDistanceToCreep(_creep);

            // is in range to attack?
            if (hypDistance <= MyWeapon.Range)
            {
                // attack!
                _creep.HitPoints -= MyWeapon.Damage;

                // check for kill condition
                if (_creep.HitPoints <= 0)
                {
                    //System.Diagnostics.Debug.Print("Creep hitpoints: " + _creep.HitPoints);
                    MyCanvas.Dispatcher.Invoke(_creep.Killed);
                }

                // trigger cooldown state
                MyWeapon.CooldownCurrent = MyWeapon.CooldownMax;

                // switch image to 'attack'
                Thread t1 = new Thread(new ThreadStart(
                delegate()
                {
                    MyCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        new Action(delegate()
                        {
                            MyCanvas.Dispatcher.Invoke(new Action(() => DrawTower(pathImage_attack)));

                        }));
                    Thread.Sleep(TimeSpan.FromMilliseconds(attackAnimationLength));
                    MyCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        new Action(delegate()
                        {
                            MyCanvas.Dispatcher.Invoke(new Action(() => DrawTower(pathImage)));
                        }));
                }));
                t1.Start();
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
            List<Creep> creepsInRange = CreepsInRange(MainWindow.myCreeps.AllCreeps);
            //System.Diagnostics.Debug.Print("Creeps in Range are: " + creepsInRange.Count);
            if (creepsInRange.Count > 0)
            {
                TargetCreep = GetTargetNearest(creepsInRange);

                // attack target if "cooled down"
                if (TargetCreep != null && MyWeapon.CooldownCurrent == 0)
                {
                    AttackCreep(TargetCreep);
                }
            }

            //if (TargetCreep != null)
            //{
            //    //attack target if "cooled down"
            //    if (MyWeapon.CooldownCurrent == 0)
            //    {
            //        AttackCreep(TargetCreep);
            //    }
            //}

            if (MyWeapon.CooldownCurrent >= 10)
            {
                MyWeapon.CooldownCurrent -= 10;
            }
            else
            {
                MyWeapon.CooldownCurrent = 0;
            }
        }
    }
}
