using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Weapon : Item
    {
        public int MinDam { get; set; }
        public int MaxDam { get; set; }

        public Weapon(int id, string name, string namePlural, int minDam, int maxDam) : base(id, name, namePlural)
        {
            MinDam = minDam;
            MaxDam = maxDam;
        }
    }
}
