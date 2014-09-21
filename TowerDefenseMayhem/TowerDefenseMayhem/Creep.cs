using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Math;

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
        public double Theta; //orientation, in radians
        public double Speed; //pixels per second

        // interaction params
        public double HitPoints;
        public CreepType Type;
        public enum CreepType { Baby, Speedy, Tanky }

        // Constructor
        public Creep(int posX, int posY, Type type )
        {
        	switch (type)
        	{
        		case Baby:
        			HitPoints = 5;
        			Speed = 5;
        			break;
        		case Speedy:
        			HitPoints = 5;
        			Speed = 10;
        			break;
        		case Tanky:
        			HitPoints = 30;
        			Speed = 4;
        			break;
        	}
        }

        public void Update()
        {
        	// handle 'living' and 'dying' interactions
        	if (isAlive())
        	{
	        	Move(); // etc
        	}
        	else
        	{
        		// die!
        	}
        }

        private void Move()
        {
        	PosX += Speed * Math.Cos(Theta);
        	PosY += Speed * Math.Sin(Theta);
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
