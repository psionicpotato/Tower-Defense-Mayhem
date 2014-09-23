using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace TowerDefenseMayhem
{
    class Tower
    {
        // spacial params
        public int PosX;
        public int PosY;

        public TowerType MyTowerType;
        public enum TowerType { Basic, Flame, Radial }
        public Weapon MyWeapon;
        public Creep TargetCreep;

        // Constructor
        public Tower(TowerType myTowerType, int posX, int posY) 
        {
            // read args
            MyTowerType = myTowerType;
            PosX = posX;
            PosY = posY;
            
            // use case select based on towertype

                // set sprite according to myTowerType

                // set initial weapon

            // instantiate at posX, posY
        }

        // change to dictionary instead? save the range already calculated and
        //  pass it on to SetTargetNearest?        
        private List<Creep> FindCreepsInRange(List<Creep> creeps)
        {
            // loop through creeps, create collection of those close enough
            //  to attack
            List<Creep> ans = new List<Creep>();

            foreach (Creep c in creeps)
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
            creepsInRange = FindCreepsInRange(globalCreeps);
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
