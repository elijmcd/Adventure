using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Player : LivingCreature
    {
        public int Gold { get; set; }
        public int ExperiencePoints { get; set; }
        public int Level { get; set; }
        public Location CurrentLocation { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public List<PlayerQuest> Quests { get; set; }

        public Player(int currentHP, int maxHP, int gold, int experiencePoints, int level) : base(currentHP, maxHP)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Level = level;

            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null)
            {
                // There is no required item for this location, so return "true"
                return true;
            }

            // See if the player has the required item in their inventory
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == location.ItemRequiredToEnter.ID)
                {
                    // We found the required item, so return "true"
                    return true;
                }
            }

            // We didn't find the required item in their inventory, so return "false"
            return false;
        }

        public bool HasThisQuest(Quest quest)
        {
            foreach(PlayerQuest playerQuest in Quests)
            {
                if(playerQuest.Details.ID == quest.ID)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CompletedThisQuest(Quest quest)
        {
            foreach(PlayerQuest playerQuest in Quests)
            {
                if (playerQuest.Details.ID == quest.ID)
                {
                    return playerQuest.IsComplete;
                }
            }

            return false;
        }

        public bool HasAllQuestItems(Quest quest)
        {
            //check if the player has all the items required to complete quest
            foreach (QuestCompletionItem qci in quest.CompletionItems)
            {
                bool foundItemInInventory = false;

                foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == qci.Details.ID) //player has item
                    {
                        if (ii.Quantity < qci.Quantity) // but not enough quantity
                        {
                            return false;
                        }
                        else if(ii.Quantity >= qci.Quantity)
                        {
                            return true;
                        }
                    }
                }

                //player has none of this quest's items
                if (!foundItemInInventory)
                {
                    return false;
                }
            }

            //if we get here, player has all required items
            return true;
        }

        public void RemoveQuestItems(Quest quest)
        {
            foreach(QuestCompletionItem qci in quest.CompletionItems)
            {
                foreach(InventoryItem ii in Inventory)
                {
                    if(ii.Details.ID == qci.Details.ID)
                    {
                        //remove quantity from inventory
                        ii.Quantity -= qci.Quantity;
                        break;
                    }
                }
            }
        }

        public void AddItemToInventory(Item itemToAdd)
        {
            foreach(InventoryItem ii in Inventory)
            {
                if(ii.Details.ID == itemToAdd.ID)
                {
                    //they have it already, so increment by 1
                    ii.Quantity++;

                    return; //item added, done here so get out of function
                }
            }

            //player didnt have item, so add 1 to inventory
            Inventory.Add(new InventoryItem(itemToAdd, 1));
        }

        public void MarkQuestComplete(Quest quest)
        {
            //find quest in player's quest list
            foreach(PlayerQuest pq in Quests)
            {
                if (pq.Details.ID == quest.ID)
                {
                    //mark it complete
                    pq.IsComplete = true;

                    return;
                }
            }
        }



    }
}
