using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseMayhem
{
    class Tower
    {
        // enum TowerType
        public enum TowerType { Basic, Flame, Radial }
        public int PosX;
        public int PosY;
        public TowerType MyTowerType;
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
        private Collection<Creep> FindCreepsInRange(Collection<Creep> creeps)
        {
            // loop through creeps, create collection of those close enough
            //  to attack
            Collection<Creep> ans = new Collection<Creep>();

            foreach (Creep c in creeps)
            {
                double hypDistance = 
                    Math.Sqrt(Math.Pow(Math.Abs(c.PosX - this.PosX), 2) 
                        + Math.Abs(c.PosY - this.PosY));
                if (hypDistance <= Range) 
                {
                    ans.Add(c);
                }
            }

            return ans;
        }

        // maybe include in FindCreepsInRange for now, easy enough to do
        public void SetTargetNearest(Collection<Creep> creepsInRange)
        {
            // loop through creep collection positions and find nearest one
        }

        // call this after current target leaves range or dies
        public void AcquireNextTarget(Collection<Creep> globalCreeps)
        {
            Collection<Creep> creepsInRange = new Collection<Creep>();
            creepsInRange = FindCreepsInRange(globalCreeps);
            SetTargetNearest(creepsInRange);
        }

        public void AttackCreep(Creep _creep)
        {
            // pythagorean to find hypoteneus distance
            double hypDistance = 
                Math.Sqrt(Math.Pow(Math.Abs(_creep.posX - this.posX), 2) 
                    + Math.Abs(_creep.posY - this.posY));

            // is in range to attack?
            if (hypDistance <= myWeapon.Range)
            {
                // attack!
                _creep.HP -= myWeapon.Damage;
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
