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
            if (!_player.HasRequiredItemToEnterThisLocation(newLocation))
            {
                // We didn't find the required item in their inventory, so display a message and stop trying to move
                rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                return;
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
                bool alreadyHasQuest = _player.HasThisQuest(newLocation.QuestAvailableHere);
                bool alreadyCompletedQuest = _player.CompletedThisQuest(newLocation.QuestAvailableHere); ;

                // check if player has quest
                if (alreadyHasQuest)
                {
                    //but not yet completed
                    if (!alreadyCompletedQuest)
                    {
                        //check if player has items needed to complete
                        bool playerHasAllItemsToCompleteQuest = _player.HasAllQuestItems(newLocation.QuestAvailableHere);

                        //player has all items required to complete quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            //display message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You've completed the '" + newLocation.QuestAvailableHere.Name + "' quest." + Environment.NewLine;

                            _player.RemoveQuestItems(newLocation.QuestAvailableHere);

                            //give quest rewards
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperience.ToString() + " experience points." + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperience;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            //add reward item to inventory
                            //bool addedItemToInventory = false;

                            _player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

                            _player.MarkQuestComplete(newLocation.QuestAvailableHere);
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

            //refresh inventory
            UpdateInventoryUI();

            //refresh quests
            UpdateQuestsUI();

            //refresh weapons box
            UpdateWeaponsUI();

            //refresh potions box
            UpdatePotionsUI();
        }

        private void UpdateInventoryUI()
        {
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem ii in _player.Inventory)
            {
                if (ii.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { ii.Details.Name, ii.Quantity.ToString() });
                }
            }
        }

        private void UpdateQuestsUI()
        {
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest pq in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { pq.Details.Name, pq.IsComplete.ToString() });
            }
        }

        private void UpdateWeaponsUI()
        {
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
                //player has no weapons, so hide the box and button
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
        }

        private void UpdatePotionsUI()
        {
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
                //player has no potions, so hide box and button
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
            //get currently selected weapon from box
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            //determine damage to do
            int damageToMonster = RandomGenerator.NumBetween(currentWeapon.MinDam, currentWeapon.MaxDam);

            //apply damage to monster's CurrentHitPoints
            _currentMonster.CurrentHitpoints -= damageToMonster;

            //display message
            rtbMessages.Text += "You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " damage." + Environment.NewLine;

            //check if monster dies
            if(_currentMonster.CurrentHitpoints <= 0)
            {
                //dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You have slain the " + _currentMonster.Name + "!" + Environment.NewLine;

                //give exp
                _player.ExperiencePoints += _currentMonster.RewardExperience;
                rtbMessages.Text += "You receive " + _currentMonster.RewardExperience.ToString() + " experience." + Environment.NewLine;

                //give gold
                _player.Gold += _currentMonster.RewardGold;
                rtbMessages.Text += "You find " + _currentMonster.RewardGold.ToString() + " gold." + Environment.NewLine;

                // give random loot
                List<InventoryItem> loot = new List<InventoryItem>();

                //add item to loot list, comparing random number to drop percentage
                foreach(LootItem lootItem in _currentMonster.LootTable)
                {
                    if(RandomGenerator.NumBetween(1, 100) <= lootItem.DropPercent)
                    {
                        loot.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }

                //if no loot items were selected, then get default
                if(loot.Count == 0)
                {
                    foreach(LootItem lootItem in _currentMonster.LootTable)
                    {
                        if(lootItem.IsDefault)
                        {
                            loot.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                //add the loot item to player's inventory
                foreach(InventoryItem ii in loot)
                {
                    _player.AddItemToInventory(ii.Details);

                    if(ii.Quantity == 1)
                    {
                        rtbMessages.Text += "You loot " + ii.Quantity.ToString() + " " + ii.Details.Name + Environment.NewLine;
                    }
                    else
                    {
                        rtbMessages.Text += "You loot " + ii.Quantity.ToString() + " " + ii.Details.NamePlural + Environment.NewLine;
                    }
                }

                //refresh play info and controls
                lblHitpoints.Text = _player.CurrentHitpoints.ToString();
                lblGold.Text = _player.Gold.ToString();
                lblExperience.Text = _player.ExperiencePoints.ToString();
                lblLevel.Text = _player.Level.ToString();

                UpdateInventoryUI();
                UpdateWeaponsUI();
                UpdatePotionsUI();

                // add a blank line to message box, just for appearance
                rtbMessages.Text += Environment.NewLine;

                // move player to current location (heal and make new monster)
                MoveTo(_player.CurrentLocation);
            }
            else
            {
                //monster is still alive

                //determine damage monster does to player
                int damageToPlayer = RandomGenerator.NumBetween(0, _currentMonster.MaximumDamage);

                //display msg
                rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + "damage to you." + Environment.NewLine;

                //subtract damage from player
                _player.CurrentHitpoints -= damageToPlayer;

                //refresh playa data in UI
                lblHitpoints.Text = _player.CurrentHitpoints.ToString();

                if(_player.CurrentHitpoints <= 0)
                {
                    //display death message
                    rtbMessages.Text += "You have been slain by the " + _currentMonster.Name + "!" + Environment.NewLine;

                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            //get current potion from box
            HealingPotion currentPotion = (HealingPotion)cboPotions.SelectedItem;

            //add healing amount to players hp
            _player.CurrentHitpoints += currentPotion.AmountToHeal;

            //current hp !> max hp
            if(_player.CurrentHitpoints > _player.MaximumHitpoints)
            {
                _player.CurrentHitpoints = _player.MaximumHitpoints;
            }

            //remove potion from inventory
            foreach(InventoryItem ii in _player.Inventory)
            {
                if(ii.Details.ID == currentPotion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }

            //display message
            rtbMessages.Text += "You drink a " + currentPotion.Name + Environment.NewLine;

            //monster takes turn

            //determine damage from monster to player
            int damToPlayer = RandomGenerator.NumBetween(0, _currentMonster.MaximumDamage);

            //display message
            rtbMessages.Text += "The " + _currentMonster.Name + " did " + damToPlayer.ToString() + "  damage to you." + Environment.NewLine;

            //apply damage
            _player.CurrentHitpoints -= damToPlayer;

            if (_player.CurrentHitpoints <= 0)
            {
                //display msg
                rtbMessages.Text += "You were slain by the " + _currentMonster.Name + Environment.NewLine;

                //return to home
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }

            //refresh player data in UI
            lblHitpoints.Text = _player.CurrentHitpoints.ToString();
            UpdateInventoryUI();
            UpdatePotionsUI();
        }

        private void goldLabel_Click(object sender, EventArgs e)
        {

        }

        private void rtbLocation_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
