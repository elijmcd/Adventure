using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Location
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Item ItemRequiredToEnter { get; set; }
        public Quest QuestAvailableHere { get; set; }
        public Monster MonsterLivingHere { get; set; }
        public Location LocationNorth { get; set; }
        public Location LocationEast { get; set; }
        public Location LocationSouth { get; set; }
        public Location LocationWest { get; set; }


        public Location(int id, string name, string description)
        {
            ID = id;
            Name = name;
            Description = description;
        }
        public Location(int id, string name, string description, Item itemRequired)
        {
            ID = id;
            Name = name;
            Description = description;
            ItemRequiredToEnter = itemRequired;
        }

    }
}
