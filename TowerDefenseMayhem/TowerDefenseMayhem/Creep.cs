using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Media.Imaging;

///<summary>
/// Notes: 
///   -
///   -
///</summary>
namespace TowerDefenseMayhem
{
    public class Creeps
    {
        public List<Creep> AllCreeps = new List<Creep>();
        public void Update(TimeSpan timeSpan)
        {
            List<Creep> tempAllCreeps = new List<Creep>(AllCreeps);
            foreach (Creep creep in tempAllCreeps)
            {
                if (creep.IsAlive())
                { /* alive = move */
                    creep.Move(timeSpan);
                }
                else
                { /* dead */
                    AllCreeps.Remove(creep);
                }
            }
        }
    }

    

    static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T: ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }

    public class Creep
    {

        
    
    	// Spacial params
        public int PosX; //pixels
        public int PosY; //pixels
        public double Speed; //pixels per millisecond
        public int[,] Path; //pixel positions
        public int LegOfPath; //which leg of path it is currently on (index 0)

        // interaction params
        public double HitPoints;
        public CreepType Type;
        public enum CreepType { Baby, Speedy, Tanky }

        // display params
        public Canvas MyCanvas;
        public BitmapSource MyBitmapSource;
        public System.Windows.Controls.Image MyImage;
        public string RelImagePath;

        private MainWindow MainWindow;
        //private Creeps Collection;

        // Constructor
        public Creep(CreepType type, int[,] path, Canvas myCanvas, MainWindow mainWindow)
        {
            MainWindow = mainWindow; 

            Path = path;
            PosX = path[0, 0];
            PosY = path[0, 1];

            MyCanvas = myCanvas;
            LegOfPath = 0;
        	switch (type)
        	{
        		case CreepType.Baby:
        			HitPoints = 5;
        			Speed = 0.05;
                    RelImagePath = @"..\..\Creep Images\imageedit_1_5013384539.png";
        			break;
        		case CreepType.Speedy:
        			HitPoints = 5;
        			Speed = 0.2;
                    RelImagePath = @"..\..\Creep Images\SpeedyCreep.png";
        			break;
        		case CreepType.Tanky:
        			HitPoints = 30;
        			Speed = 0.02;
                    //RelImagePath = @"..\..\Images\TankyCreep.png";
        			break;
        	}
            MyCanvas.Dispatcher.Invoke(SetImageFromPath);
        }

        private void SetImageFromPath()
        {
            Uri imguri = new Uri(RelImagePath, UriKind.Relative);
            PngBitmapDecoder dec = new PngBitmapDecoder(imguri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            MyBitmapSource = dec.Frames[0];
            MyImage = new System.Windows.Controls.Image();
            MyImage.Source = MyBitmapSource;

            MyCanvas.Children.Add(MyImage);
            MyImage.SetValue(Canvas.LeftProperty, Convert.ToDouble(PosX) - Math.Floor(MyBitmapSource.Width / 2));
            MyImage.SetValue(Canvas.TopProperty, Convert.ToDouble(PosY) - Math.Floor(MyBitmapSource.Height / 2));
        }

        public void Move(TimeSpan timeSpan)
        {
            // determine destination
            int[] nextPoint = {0,0};
            if (LegOfPath < Path.GetLength(0))
            {
                nextPoint[0] = Path[LegOfPath + 1, 0];
                nextPoint[1] = Path[LegOfPath + 1, 1];
            }

            int[] distToNextPoint = {nextPoint[0] - PosX, nextPoint[1] - PosY};
            
            //TODO: there is deffinately a smarter way to do this
            double direction = 0; // degrees
            //direction = Math.Atan2(distToNextPoint[1], distToNextPoint[0]);

            if (distToNextPoint[0] != 0)
            {
                if (distToNextPoint[0] > 0)
                {
                    direction = 0;
                }
                else
                {
                    direction = 180;
                }
            }
            else if (distToNextPoint[1] != 0)
            {
                if (distToNextPoint[1] > 0)
                {
                    direction = 90;
                }
                else
                {
                    direction = 270;
                }
            }

            double projectedDeltaX = Math.Cos(direction) * Speed * timeSpan.TotalMilliseconds;
            double projectedDeltaY = Math.Sin(direction) * Speed * timeSpan.TotalMilliseconds;

            // check if it will pass this or any future points (loop)
            if ((Math.Abs(distToNextPoint[0]) < projectedDeltaX) || (Math.Abs(distToNextPoint[1]) < projectedDeltaY))
            { /* it will go thru nextPoint */
                LegOfPath++;
                double distRemaining = 0;
                if (direction == 0 || direction == 180)
                {
                    distRemaining = Math.Abs(distToNextPoint[0]);
                }
                else if (direction == 90 || direction == 270)
                {
                    distRemaining = Math.Abs(distToNextPoint[1]);
                }

                double timeTaken_ms = distRemaining / Speed;
                PosX = nextPoint[0];
                PosY = nextPoint[1];
                Move(timeSpan - TimeSpan.FromMilliseconds(timeTaken_ms));
            } 
            else 
            { /* move towards nextPoint */
                switch (Convert.ToInt16(direction))
                {
                    case 0:
                        PosX += Convert.ToInt16(Speed * timeSpan.TotalMilliseconds);
                        break;
                    case 90:
                        PosY += Convert.ToInt16(Speed * timeSpan.TotalMilliseconds);
                        break;
                    case 180:
                        PosX -= Convert.ToInt16(Speed * timeSpan.TotalMilliseconds);
                        break;
                    case 270:
                        PosY -= Convert.ToInt16(Speed * timeSpan.TotalMilliseconds);
                        break;
                    default:
                        break;
                }

                // check if final destination reached
                if (PosX == Path[Path.GetLength(0) - 1, 0] && PosY == Path[Path.GetLength(0) - 1, 1])
                {
                    // die and take players life point
                    HitPoints = 0;
                    MyCanvas.Dispatcher.Invoke(Die);
                    MainWindow.Dispatcher.Invoke(MainWindow.LoseLife);                    
                    return;
                }
                else
                {
                    // redraw image
                    MyCanvas.Dispatcher.Invoke(UpdateImage);
                }
            }
        }

        public void UpdateImage()
        {
            MyImage.SetValue(Canvas.LeftProperty,
                        Convert.ToDouble(PosX) - Math.Floor(MyBitmapSource.Width / 2));
            MyImage.SetValue(Canvas.TopProperty,
                Convert.ToDouble(PosY) - Math.Floor(MyBitmapSource.Height / 2));
        }
        
        public void Die()
        {
            MyCanvas.Children.Remove(MyImage);
        }

        public bool IsAlive()
        {
        	bool ans = true;
        	if (HitPoints <= 0)
        	{
        		ans = false;
        	}
        	return ans;
        }
    }
}
