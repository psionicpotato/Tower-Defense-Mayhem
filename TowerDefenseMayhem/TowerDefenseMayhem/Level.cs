using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseMayhem
{
    class Level
    {
        private int CurrentLevel;

        private const int NumberOfCreeps = 3;
        public Level()
        {
            CurrentLevel = 1;
        }


        public int[][] GetNextLevel(out int Level)
        {
            //Format:
            //CreepsForLevel[BABY] = new int[] { number, time interval };
            //CreepsForLevel[SPEEDY] = new int[] { number, time interval };
            //CreepsForLevel[TANKY] = new int[] { number, time interval };
            Level = CurrentLevel;
            CurrentLevel++;

            int[][] CreepsForLevel = new int[NumberOfCreeps][];

            switch (Level)
            {
                case 1:
                    CreepsForLevel[0] = new int[] { 5, 3000 };
                    CreepsForLevel[1] = new int[] { 0, 0 };
                    CreepsForLevel[2] = new int[] { 0, 0 };
                    break;
                case 2:
                    CreepsForLevel[0] = new int[] { 10, 2000 };
                    CreepsForLevel[1] = new int[] { 0, 0 };
                    CreepsForLevel[2] = new int[] { 0, 0 };
                    break;
                case 3:
                    CreepsForLevel[0] = new int[] { 0, 0 };
                    CreepsForLevel[1] = new int[] { 5, 2000 };
                    CreepsForLevel[2] = new int[] { 0, 0 };
                    break;
                case 4:
                    CreepsForLevel[0] = new int[] { 0, 0 };
                    CreepsForLevel[1] = new int[] { 0, 0 };
                    CreepsForLevel[2] = new int[] { 5, 2000 };
                    break;
                case 5:
                    CreepsForLevel[0] = new int[] { 2, 1000 };
                    CreepsForLevel[1] = new int[] { 2, 2000 };
                    CreepsForLevel[2] = new int[] { 2, 3000 };
                    break;
                default:
                    CreepsForLevel[0] = new int[] { 5, 2000 };
                    CreepsForLevel[1] = new int[] { 5, 3000 };
                    CreepsForLevel[2] = new int[] { 5, 3000 };
                    break;
            }
            return CreepsForLevel;
        }


        public int GetCreepCount(int level)
        {
            if (level < 10)
            {
                return level * 5;
            }
            if (level < 20)
            {
                return level * 10;
            }
            return 1000;
        }
    }
}
