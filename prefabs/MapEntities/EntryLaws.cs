using System.Collections.Generic;

namespace GalaxyGauntlet.prefabs.MapEntities
{
    public static class EntryLaws
    {
        public static Dictionary<int, EntryLaw> Laws = new()
        {
            /* ----- TILES ----- */
            { GGConstants.ByteCodes.Entities.Floor, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_N, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_E, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_S, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_W, new() }
            , { GGConstants.ByteCodes.Entities.IceTile, new() }
            , { GGConstants.ByteCodes.Entities.Ice_NW, new(BuildEntryLaw("SE")) }
            , { GGConstants.ByteCodes.Entities.Ice_NE, new(BuildEntryLaw("SW")) }
            , { GGConstants.ByteCodes.Entities.Ice_SW, new(BuildEntryLaw("NE")) }
            , { GGConstants.ByteCodes.Entities.Ice_SE, new(BuildEntryLaw("NW")) }
            , { GGConstants.ByteCodes.Entities.Gravel, new(BuildEntryLaw("NWSE"))
                {
                    Creature = 0
                }
            }
            , { GGConstants.ByteCodes.Entities.Dirt, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.Water, new() }
            , { GGConstants.ByteCodes.Entities.Fire, new() }
            , { GGConstants.ByteCodes.Entities.TrapActive, new() }
            , { GGConstants.ByteCodes.Entities.TrapInactive, new() }
            , { GGConstants.ByteCodes.Entities.ToolThief, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.KeyThief, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.HintPanel, new() }
            , { GGConstants.ByteCodes.Entities.BlueButton, new() }
            , { GGConstants.ByteCodes.Entities.GreenButton, new() }
            , { GGConstants.ByteCodes.Entities.RedButton, new() }
            , { GGConstants.ByteCodes.Entities.BrownButton, new() }
            , { GGConstants.ByteCodes.Entities.BlueTeleporter, new() }
            , { GGConstants.ByteCodes.Entities.RedTeleporter, new() }
            , { GGConstants.ByteCodes.Entities.GreenTeleporter, new() }
            , { GGConstants.ByteCodes.Entities.YellowTeleporter, new() }
            , { GGConstants.ByteCodes.Entities.Wall, new(0) }
            , { GGConstants.ByteCodes.Entities.HiddenWall, new(0) }
            , { GGConstants.ByteCodes.Entities.BlueWall, new(0) }
            , { GGConstants.ByteCodes.Entities.FakeBlueWall, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.GreenToggleFloor, new() }
            , { GGConstants.ByteCodes.Entities.GreenToggleWall, new(0) }
            , { GGConstants.ByteCodes.Entities.RecessedWall, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.CloneMachine, new(0) }
            , { GGConstants.ByteCodes.Entities.CloneMachine_V1, new(0) }
            , { GGConstants.ByteCodes.Entities.RedDoor, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.BlueDoor, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.YellowDoor, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.GreenDoor, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.Socket, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.Exit, new()
                {
                    Creature = 0
                }
            }

            /* ----- ITEMS ----- */
            , { GGConstants.ByteCodes.Entities.Chip, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.RedKey, new() }
            , { GGConstants.ByteCodes.Entities.BlueKey, new() }
            , { GGConstants.ByteCodes.Entities.YellowKey, new() }
            , { GGConstants.ByteCodes.Entities.GreenKey, new() }
            , { GGConstants.ByteCodes.Entities.IceSkates, new()
                {
                    Creature = 0
                }
            }
            , { GGConstants.ByteCodes.Entities.SuctionBoots, new()
                {
                    Creature = 0
                }
            }
            , { GGConstants.ByteCodes.Entities.FireBoots, new()
                {
                    Creature = 0
                }
            }
            , { GGConstants.ByteCodes.Entities.Flippers, new()
                {
                    Creature = 0
                }
            }
            
            /* ----- CREATURES ----- */
            , { GGConstants.ByteCodes.Entities.RedBomb, new() }
            , { GGConstants.ByteCodes.Entities.DirtBlock, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.IceBlock, new(0)
                {
                    Player = BuildEntryLaw("NWSE")
                }
            }
        };


        public static int BuildEntryLaw(string input)
        {
            input = input.ToLower();
            int result = 0;
            if (input.Contains('n'))
            {
                result |= 1;
            }
            if (input.Contains('e'))
            {
                result |= 2;
            }
            if (input.Contains('s'))
            {
                result |= 4;
            }
            if (input.Contains('w'))
            {
                result |= 8;
            }
            return result;
        }
    }


    public class EntryLaw
    {
        public int Player = 15;
        public int Block = 15;
        public int Creature = 15;
        public int Ghost = 15;

        private const byte North = 0x01;
        private const byte East = 0x02;
        private const byte South = 0x04;
        private const byte West = 0x08;

        /// <summary>
        /// Creates a new Entry Law instance where all
        /// entities can enter from all directions.
        /// </summary>
        public EntryLaw() { }


        /// <summary>
        /// Creates a new Entry Law instance where all
        /// entities are given the same rule value.
        /// </summary>
        public EntryLaw(int value)
        {
            Player = value;
            Block = value;
            Creature = value;
            Ghost = value;
        }


        public bool CanPlayerEnter(Vector2I direction)
        {
            return CanEnter(Player, direction);
        }


        public bool CanBlockEnter(Vector2I direction)
        {
            return CanEnter(Block, direction);
        }


        public bool CanCreatureEnter(Vector2I direction)
        {
            return CanEnter(Creature, direction);
        }


        private static bool CanEnter(int lawRules, Vector2I direction)
        {

            if ((lawRules & North) != North
                && direction.Y > 0)
            {
                // GD.Print("Cannot enter from the North");
                return false;
            }
            if ((lawRules & South) != South
                && direction.Y < 0)
            {
                // GD.Print("Cannot enter from the South");
                return false;
            }

            if ((lawRules & East) != East
                && direction.X < 0)
            {
                // GD.Print("Cannot enter from the East");
                return false;
            }

            if ((lawRules & West) != West
                && direction.X > 0)
            {
                // GD.Print("Cannot enter from the West");
                return false;
            }

            return true;
        }

    }
}
