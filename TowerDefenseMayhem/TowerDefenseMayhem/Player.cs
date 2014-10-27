using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseMayhem
{
    class Player
    {

        public int Lives { get; private set; }
        public event EventHandler LifeChange;

        public Player()
        {
            Lives = 5;
        }
        protected virtual void OnLifeChange()
        {
            if (LifeChange != null) { LifeChange(this, EventArgs.Empty); }
        }

        public void AddLives(int amount)
        {
            Lives += amount;
            OnLifeChange();
        }

        object locker;
        public bool LoseLives(int amount)
        {
            lock(locker)
            {
                Lives += amount;
                if (Lives < 0) { Lives = 0; }
                OnLifeChange();

                if (Lives == 0) { return true; }
                return true;
            }
        }

    }
}
