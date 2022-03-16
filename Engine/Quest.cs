using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Quest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RewardExperience { get; set; }
        public int RewardGold { get; set; }
        public Item RewardItem { get; set; }
        public List<QuestCompletionItem> CompletionItems { get; set; }

        public Quest(int id, string name, string description, int rewardXP, int rewardGP)
        {
            ID = id;
            Name = name;
            Description = description;
            RewardExperience = rewardXP;
            RewardGold = rewardGP;
            CompletionItems = new List<QuestCompletionItem>();
        }

        public Quest(int id, string name, string description, int rewardXP, int rewardGP, Item rewardItem)
        {
            ID = id;
            Name = name;
            Description = description;
            RewardExperience = rewardXP;
            RewardGold = rewardGP;
            RewardItem = rewardItem;
            CompletionItems = new List<QuestCompletionItem>();
        }

    }
}
