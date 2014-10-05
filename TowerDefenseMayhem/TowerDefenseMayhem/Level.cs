using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseMayhem
{
    class Level
    {
        public int GetCreepCount(int level)
        {
            switch (level)
            {
                case 1:
                    return 5;
                case 2:
                    return 10;
                case 3:
                    return 15;
            }
            return 1000;
        }
    }
}
