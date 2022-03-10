using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Player
    {
        public int CurrentHitpoints { get; set; }
        public int MaximumHitpoints { get; set; }
        public int Gold { get; set; }
        public int ExperiencePoints { get; set; }
        public int Level { get; set; }
    }
}
