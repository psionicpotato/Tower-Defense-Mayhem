using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
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
    class Creeps
    {
        public List<Creep> AllCreeps = new List<Creep>();
        public void Update(TimeSpan timeSpan)
        {
            foreach (Creep creep in AllCreeps)
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

    class Creep
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
        public Image MyImage = new Image();

        // Constructor
        public Creep(CreepType type, int[,] path, Canvas myCanvas)
        {
            Path = path;
            PosX = path[0, 0];
            PosY = path[0, 1];

            MyCanvas = myCanvas;
            LegOfPath = 0;
        	switch (type)
        	{
        		case CreepType.Baby:
        			HitPoints = 5;
        			Speed = 0.01;
                    SetImageFromPath(@"..\..\Images\BabyCreep.png");
        			break;
        		case CreepType.Speedy:
        			HitPoints = 5;
        			Speed = 0.02;
                    //SetImageFromPath(@"Images\SpeedyCreep.png");
        			break;
        		case CreepType.Tanky:
        			HitPoints = 30;
        			Speed = 0.01;
                    //SetImageFromPath(@"Images\TankyCreep.png");
        			break;
        	}
        }

        private void SetImageFromPath(string relPath)
        {
            Uri imguri = new Uri(relPath, UriKind.Relative);
            PngBitmapDecoder dec = new PngBitmapDecoder(imguri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = dec.Frames[0];
            MyImage.Source = bitmapSource;

            MyCanvas.Children.Add(MyImage);
            MyImage.SetValue(Canvas.LeftProperty, Convert.ToDouble(PosX) - Math.Floor(bitmapSource.Width / 2));
            MyImage.SetValue(Canvas.TopProperty, Convert.ToDouble(PosY) - Math.Floor(bitmapSource.Height / 2));
        }

        public void Move(TimeSpan timeSpan)
        {
            // determine destination
            int[] nextPoint = {Path[LegOfPath + 1, 0], Path[LegOfPath + 1, 1]};
            int[] distToNextPoint = {nextPoint[0] - PosX, nextPoint[1] - PosY};
            
            //TODO: there is deffinately a smarter way to do this
            double direction = 0; // degrees
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

                // redraw image
                //MyImage.SetValue(Canvas.LeftProperty, Convert.ToDouble(PosX));
                //MyImage.SetValue(Canvas.TopProperty, Convert.ToDouble(PosY));
            }
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
