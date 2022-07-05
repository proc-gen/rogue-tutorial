using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Components
{
    internal class SufferDamage
    {
        public List<int> Amount { get; private set; }

        public void NewDamage(int amount)
        {
            if(Amount == null)
            {
                Amount = new List<int>();
            }
            Amount.Add(amount);
        }
    }
}
