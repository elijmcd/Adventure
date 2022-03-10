using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LivingCreature
    {
        public int CurrentHitpoints { get; set; }
        public int MaximumHitpoints { get; set; }

        public LivingCreature(int currentHP, int maximumHP)
        {
            CurrentHitpoints = currentHP;
            MaximumHitpoints = maximumHP;
        }
    }
}
