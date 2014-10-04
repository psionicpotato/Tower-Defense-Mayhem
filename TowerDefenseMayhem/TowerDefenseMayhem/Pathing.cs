using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseMayhem
{
    class Pathing
    {

        int[,] path;

        public int[,] GetPath(int level)
        {
            switch (level)
            {
                case 1:
                    GenerateLevel1();
                    break;
                case 2:
                    GenerateLevel2();
                    break;
                case 3:
                    GenerateLevel3();
                    break;
            }
            return path;
        }
            

        private void GenerateLevel1()
        {
            path = new int[,] { { 0, 200 }, { 600, 200 }, { 600, 600 }, { 0, 600 } };
            //cuz LAWL
            
        }

        private void GenerateLevel2()
        {

        }

        private void GenerateLevel3()
        {

        }

    }
}
