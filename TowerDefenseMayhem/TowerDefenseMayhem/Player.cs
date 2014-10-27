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

        public void LoseLives(int amount)
        {
            Lives += amount;
            OnLifeChange();
        }

    }
}
