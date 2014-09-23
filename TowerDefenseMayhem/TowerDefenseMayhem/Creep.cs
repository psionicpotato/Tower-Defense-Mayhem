using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

///<summary>
/// Notes: 
///   - not sure how we will handle Creep Path -> Orientation
///   -
///   -
///</summary>
namespace TowerDefenseMayhem
{
    class Creep
    {
    	// Spacial params
        public int PosX; //pixels
        public int PosY; //pixels
        public double Theta; //orientation, in degrees
        public double Speed; //pixels per second

        // interaction params
        public double HitPoints;
        public CreepType Type;
        public enum CreepType { Baby, Speedy, Tanky }

        // Constructor
        public Creep(int posX, int posY, CreepType type)
        {
        	switch (type)
        	{
        		case CreepType.Baby:
        			HitPoints = 5;
        			Speed = 5;
        			break;
        		case CreepType.Speedy:
        			HitPoints = 5;
        			Speed = 10;
        			break;
        		case CreepType.Tanky:
        			HitPoints = 30;
        			Speed = 4;
        			break;
        	}
        }

        public void Update(double time_ms)
        {
        	// handle 'living' and 'dying' interactions
        	if (IsAlive())
        	{
	        	Move(time_ms); // etc
        	}
        	else
        	{
        		// die!
        	}
        }

        private void Move(double time_ms)
        {
        	PosX += Convert.ToInt16(Speed * Math.Cos(Theta * Math.PI / 180.0));
        	PosY += Convert.ToInt16(Speed * Math.Sin(Theta * Math.PI / 180.0));
        }

        private bool IsAlive()
        {
        	bool ans = true;
        	if (HitPoints <= 0)
        	{
        		ans = false;
        	}
        	return false;
        }
    }
}
