using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Monster> Monsters = new List<Monster>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();

        public const int ITEM_ID_RUSTYSWORD = 1;
        public const int ITEM_ID_RATTAIL = 2;
        public const int ITEM_ID_PIECEFUR = 3;
        public const int ITEM_ID_SNAKEFANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALINGPOTION = 7;
        public const int ITEM_ID_SPIDERFANG = 8;
        public const int ITEM_ID_SPIDERSILK = 9;
        public const int ITEM_ID_ADVENTURERPASS = 10;

        public const int MONSTER_ID_RAT = 1;
        public const int MONSTER_ID_SNAKE = 2;
        public const int MONSTER_ID_GIANTSPIDER = 3;

        public const int QUEST_ID_ALCHEMIST = 1;
        public const int QUEST_ID_FARMER = 2;

        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWNSQUARE = 2;
        public const int LOCATION_ID_GUARDPOST = 3;
        public const int LOCATION_ID_ALCHEMISTHUT = 4;
        public const int LOCATION_ID_ALCHEMISTGARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARMFIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDERFIELD = 9;

        static World()
        {
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();
        }

        private static void PopulateItems()
        {
            Items.Add(new Weapon(ITEM_ID_RUSTYSWORD, "Rusty Sword", "Rusty Swords", 0, 5));
            Items.Add(new Item(ITEM_ID_RATTAIL, "Rat tail", "Rat tails"));
            Items.Add(new Item(ITEM_ID_PIECEFUR, "Piece of fur", "Pieces of fur"));
            Items.Add(new Item(ITEM_ID_SNAKEFANG, "Snake fang", "Snake fangs"));
            Items.Add(new Item(ITEM_ID_SNAKESKIN, "Snake skin", "Snake skins"));
            Items.Add(new Weapon(ITEM_ID_CLUB, "Club", "Clubs", 3, 10));
            Items.Add(new HealingPotion(ITEM_ID_HEALINGPOTION, "HealingPotion", "Healing Potions", 5));
            Items.Add(new Item(ITEM_ID_SPIDERFANG, "Spider fang", "Spider fangs"));
            Items.Add(new Item(ITEM_ID_SPIDERSILK, "Spider silk", "Spider silks"));
            Items.Add(new Item(ITEM_ID_ADVENTURERPASS, "Adventurer Pass", "Adventurer Passes"));
        }

        private static void PopulateMonsters()
        {
            Monster rat = new Monster(MONSTER_ID_RAT, "Rat", 5, 3, 10, 3, 3);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RATTAIL), 75, false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECEFUR), 75, true));

            Monster snake = new Monster(MONSTER_ID_SNAKE, "Snake", 5, 3, 10, 3, 3);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKEFANG), 75, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 75, true));

            Monster giantSpider = new Monster(MONSTER_ID_GIANTSPIDER, "Giant spider", 20, 5, 40, 10, 10);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDERFANG), 75, true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDERSILK), 25, false));

            Monsters.Add(rat);
            Monsters.Add(snake);
            Monsters.Add(giantSpider);
        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden =
                new Quest(
                    QUEST_ID_ALCHEMIST,
                    "Clear the alchemist's garden",
                    "Kill rats in the alchemist's garden and return with 3 tails. Reward: 20xp + 10gp + Healing Potion", 20, 10);
            clearAlchemistGarden.CompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RATTAIL), 3));
            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALINGPOTION);

            Quest clearFarmersField =
                new Quest(
                    QUEST_ID_FARMER,
                    "Clear the farmer's field",
                    "Kill snakes in the farmer's field and return with 3 fangs. Reward: 20xp + 20gp + Adventurer's Pass", 20, 20);
            clearFarmersField.CompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKEFANG), 3));
            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURERPASS);

            Quests.Add(clearAlchemistGarden);
            Quests.Add(clearFarmersField);
        }

        private static void PopulateLocations()
        {
            Location home = new Location(LOCATION_ID_HOME, "Home", "Your house. You really need to clean up the place.");
            Location townSquare = new Location(LOCATION_ID_TOWNSQUARE, "Town square", "You see a fountain.");

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMISTHUT, "Alchemist's hut", "There are many strange plants on the shelves.");
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_ALCHEMIST);
            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTGARDEN, "Alchemist's garden", "Many plants are growing here.");
            alchemistsGarden.MonsterLivingHere = MonsterByID(MONSTER_ID_RAT);

            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "Farmhouse", "There is a small farmhouse, with a farmer in front.");
            farmhouse.QuestAvailableHere = QuestByID(QUEST_ID_FARMER);
            Location farmersField = new Location(LOCATION_ID_FARMFIELD, "Farmer's field", "You see rows of vegetables growing here.");
            farmersField.MonsterLivingHere = MonsterByID(MONSTER_ID_SNAKE);

            Location guardPost = new Location(LOCATION_ID_GUARDPOST, "Guard post", "There is a large, tough-looking guard here.", ItemByID(ITEM_ID_ADVENTURERPASS));
            Location bridge = new Location(LOCATION_ID_BRIDGE, "Bridge", "A stone bridge crosses a wide river.");

            Location spiderField = new Location(LOCATION_ID_SPIDERFIELD, "Forest", "You see spider webs covering covering the trees in this forest.");
            spiderField.MonsterLivingHere = MonsterByID(MONSTER_ID_GIANTSPIDER);

            //link locations together
            home.LocationNorth = townSquare;

            townSquare.LocationNorth = alchemistHut;
            townSquare.LocationEast = guardPost;
            townSquare.LocationSouth = home;
            townSquare.LocationWest = farmhouse;

            farmhouse.LocationEast = townSquare;
            farmhouse.LocationWest = farmersField;

            farmersField.LocationEast = farmhouse;

            alchemistHut.LocationSouth = townSquare;
            alchemistHut.LocationNorth = alchemistsGarden;

            alchemistsGarden.LocationSouth = alchemistHut;

            guardPost.LocationEast = bridge;
            guardPost.LocationWest = townSquare;

            bridge.LocationEast = spiderField;
            bridge.LocationWest = townSquare;

            spiderField.LocationWest = bridge;

            //Add locations to static list
            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistsGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
        }

        public static Item ItemByID(int id)
        {
            foreach (Item item in Items)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }

            return null;
        }

        public static Monster MonsterByID(int id)
        {
            foreach (Monster monster in Monsters)
            {
                if (monster.ID == id)
                {
                    return monster;
                }
            }

            return null;
        }

        public static Quest QuestByID(int id)
        {
            foreach (Quest quest in Quests)
            {
                if (quest.ID == id)
                {
                    return quest;
                }
            }

            return null;
        }

        public static Location LocationByID(int id)
        {
            foreach (Location location in Locations)
            {
                if (location.ID == id)
                {
                    return location;
                }
            }

            return null;
        }

    }
}
