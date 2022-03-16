using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;

namespace Adventure
{
    public partial class Adventure : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public Adventure()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0, 1);

            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTYSWORD), 1));

            lblHitpoints.Text = _player.CurrentHitpoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationNorth);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationEast);
        }
        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationSouth);
        }
        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationWest);
        }

        private void MoveTo(Location newLocation)
        {
            //Does the location have any required items
            if (newLocation.ItemRequiredToEnter != null)
            {
                bool playerHasRequiredItem = false;

                foreach (InventoryItem ii in _player.Inventory)
                {
                    // We found required item
                    if (ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        playerHasRequiredItem = true;
                        break; //Exit from loop
                    }
                }

                if (!playerHasRequiredItem)
                {
                    // We didn't find the required item in their inventory, so display a message and stop trying to move
                    rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                    return;
                }
            }

            // Update player current location
            _player.CurrentLocation = newLocation;

            // show/hide applicable movement buttons
            btnNorth.Visible = (newLocation.LocationNorth != null);
            btnEast.Visible = (newLocation.LocationEast != null);
            btnSouth.Visible = (newLocation.LocationSouth != null);
            btnWest.Visible = (newLocation.LocationWest != null);

            // Display current location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // heal the player
            _player.CurrentHitpoints = _player.MaximumHitpoints;

            // update hitpoint UI
            lblHitpoints.Text = _player.CurrentHitpoints.ToString();

            // does newLocation have a quest?
            if (newLocation.QuestAvailableHere != null)
            {
                //check if player already has quest, and has finished it
                bool alreadyHasQuest = false;
                bool alreadyCompletedQuest = false;

                foreach (PlayerQuest quest in _player.Quests)
                {
                    if (quest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        alreadyHasQuest = true;

                        if (quest.IsComplete)
                        {
                            alreadyCompletedQuest = true;
                        }
                    }
                }

                // check if player has quest
                if (alreadyHasQuest)
                {
                    //but not yet completed
                    if (!alreadyCompletedQuest)
                    {
                        //check if player has items needed to complete
                        bool playerHasAllItemsToCompleteQuest = true;

                        foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.CompletionItems)
                        {
                            bool foundInInventory = false;

                            //check if item in inventory, see if they have it and if yes is it enough
                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                //if player has item in inventory
                                if (ii.Details.ID == qci.Details.ID)
                                {
                                    foundInInventory = true;

                                    if (ii.Quantity < qci.Quantity)
                                    {
                                        //not enough
                                        playerHasAllItemsToCompleteQuest = false;

                                        //no reason to continue checking
                                        break;
                                    }

                                    //found the item, so stop checking inventory
                                    break;
                                }
                            }

                            //if item not found, set variable to stop searching
                            if (!foundInInventory)
                            {
                                // The player does not have this item in their inventory
                                playerHasAllItemsToCompleteQuest = false;

                                //no reason to continue checking
                                break;
                            }
                        }

                        //player has all items required to complete quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            //display message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You've completed the '" + newLocation.QuestAvailableHere.Name + "' quest." + Environment.NewLine;

                            //remove quest items from iventory
                            foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.CompletionItems)
                            {
                                foreach (InventoryItem ii in _player.Inventory)
                                {
                                    if (ii.Details.ID == qci.Details.ID)
                                    {
                                        //subtract appropriate quantity from inventory
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }

                            //give quest rewards
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperience.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperience;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            //add reward item to inventory
                            bool addedItemToInventory = false;

                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                if (ii.Details.ID == newLocation.QuestAvailableHere.RewardItem.ID)
                                {
                                    //item is in inventory, so add 1 to the pile
                                    ii.Quantity++;

                                    addedItemToInventory = true;

                                    break;
                                }
                            }

                            //didn't have item, so add 1 to inventory
                            if (!addedItemToInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1));
                            }

                            //mark quest complete
                            //find quest in player's list
                            foreach (PlayerQuest pq in _player.Quests)
                            {
                                if (pq.Details.ID == newLocation.QuestAvailableHere.ID)
                                {
                                    pq.IsComplete = true;

                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //player does not already have quest

                    //display messages
                    rtbMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name + "quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with: " + Environment.NewLine;
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.CompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;

                    //add quest to player's questlist
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            //does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                //make a new monster from World.Monster list
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperience, standardMonster.RewardGold, standardMonster.CurrentHitpoints, standardMonster.MaximumHitpoints);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }
            //refresh player inventory list
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }

            //refresh player's quest ist
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsComplete.ToString() });
            }

            //refresh player's combo boxes
            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details is Weapon)
                {
                    if (ii.Quantity > 0)
                    {
                        weapons.Add((Weapon)ii.Details);
                    }
                }
            }

            if (weapons.Count == 0)
            {
                //the player has no weapons, so hide the box and button
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                cboWeapons.SelectedIndex = 0;
            }

            //refresh player's potion box
            List<HealingPotion> potions = new List<HealingPotion>();

            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Details is HealingPotion)
                {
                    if (ii.Quantity > 0)
                    {
                        potions.Add((HealingPotion)ii.Details);
                    }
                }
            }

            if (potions.Count == 0)
            {
                //no potions, so hide box and button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = potions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }

    }
}
