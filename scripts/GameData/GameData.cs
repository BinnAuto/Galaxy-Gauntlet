using GalaxyGauntlet.scripts.MapEntities;

namespace GalaxyGauntlet.scripts
{
    /*
     * Facilitates communication between different properties of GameData
     */
    public static partial class GameData
    {

        public static void Initialize()
        {
            Archipelago = new();
            Archipelago.OnDeathMessage += OnSetDeathMessage;
            Archipelago.OnItemReceived += OnArchipelagoItemReceived;
            LoadSettings();
        }


        private static void OnSetDeathMessage(string message)
        {
            DeathMessage = message;
        }


        private static void OnArchipelagoItemReceived(string itemName)
        {
            switch(itemName)
            {
                case Constants.Archipelago.TrapNames.ConfusionTrap:
                    RandomNumberGenerator rng = new();
                    ConfusionTimer = rng.RandiRange(20, 40);
                    break;

                case Constants.Archipelago.ItemNames.TimeBonus:
                    if(TimeLimit == 0)
                    {
                        // Turn untimed levels into timed levels (hurry up!!!)
                        TimeLimit = 10;
                    }
                    Timer += 10;
                    break;

                case Constants.Archipelago.TrapNames.TimePenalty:
                    if(TimeLimit == 0)
                    {
                        // Instant death on untimed levels
                        DeathMessage = Constants.DeathMessages.OutOfTime;
                        return;
                    }

                    Timer -= 10;
                    break;

                case Constants.Archipelago.TrapNames.WalkerTrap:
                    AddMobAtRandom(new WalkerMonster(Vector2I.Zero));
                    break;

                case Constants.Archipelago.TrapNames.BlobTrap:
                    AddMobAtRandom(new BlobMonster(Vector2I.Zero));
                    break;

                case Constants.Archipelago.TrapNames.TeethTrap:
                    AddMobAtRandom(new TeethMonster(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.BigMap:
                    MapViewportSize = Mathf.Min(15, MapViewportSize + 6);
                    break;

                case Constants.Archipelago.TrapNames.SmallMapTrap:
                    MapViewportSize = Mathf.Max(5, MapViewportSize - 4);
                    break;

                default:
                    // Item is either a progression item or has already
                    // been processed in the Archipelago subclass.
                    break;
            }
        }
    }
}
