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
        public int posX;
        public int posY;
        public Weapon myWeapon;
        public Creep TargetCreep;

        // Constructor
        public Tower(TowerType _type, int _posX, int _posY) 
        {
            // set sprite position
            posX = _posX;
            posY = _posY;

            // set sprite according to _type

            // set weapon

            // instantiate at posX, posY            
        }

        // Target creep
        public void SetTargetNearest( /* collection of creeps */ )
        {
            // loop through creep collection positions and find nearest one
        }

        public void AttackCreep(Creep _creep)
        {
            double hypDistance = Math.Sqrt(Math.Pow(Math.Abs(_creep.posX - this.posX), 2) + Math.Abs(_creep.posY - this.posY));

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
            public Weapon(Tower.TowerType type, double cd, double rng, double dmg)
            {
                CooldownMax = cd;
                Range = rng;
                Damage = dmg;
            }
        }

        // attack creep
    }
}
