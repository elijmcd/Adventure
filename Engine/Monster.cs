using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class Monster
    {
        public int ID { get; set; }
        public string Name{get; set; }
        public int MaximumHitpoints { get; set; }    
        public int CurrentHitpoints { get; set; }
        public int MaximumDamage { get; set; }
        public int RewardExperience { get; set; }
        public int RewardGold { get; set; }
    }
}
