using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TowerDefenseMayhem
{
    class Money
    {
        int Cash;

        public Money()
        {
            Cash = 1000;
        }

        public bool RequestPurchase(int amount)
        {
            if (amount > Cash)
            {
                System.Windows.MessageBox.Show("You can't afford this tower.");
                return false;
            }
            else
            {
                Cash -= amount;
                return true;
            }
        }

        public void AddMoney(int amount)
        {
            Cash += amount;
        }
    }
}
