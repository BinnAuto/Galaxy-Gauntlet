using System.Collections.Generic;

namespace GalaxyGauntlet.prefabs.MapEntities.MapObject
{
    public class EntryLaws
    {
        public static readonly Dictionary<int, EntryLaw> Laws = new()
        {
            { GGConstants.ByteCodes.Entities.Floor, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_N, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_W, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_S, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_E, new() }
            , { GGConstants.ByteCodes.Entities.ForceFloor_R, new()
                {
                    CreatureLaw = 0
                }
            }
            , { GGConstants.ByteCodes.Entities.IceTile, new() }
            , { GGConstants.ByteCodes.Entities.Ice_NW, new(BuildBitmask("SE")) }
            , { GGConstants.ByteCodes.Entities.Ice_NE, new(BuildBitmask("SW")) }
            , { GGConstants.ByteCodes.Entities.Ice_SW, new(BuildBitmask("NE")) }
            , { GGConstants.ByteCodes.Entities.Ice_SE, new(BuildBitmask("NW")) }
            , { GGConstants.ByteCodes.Entities.Gravel, new()
                {
                    CreatureLaw = 0
                }
            }
            , { GGConstants.ByteCodes.Entities.Dirt, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.Water, new() }
            , { GGConstants.ByteCodes.Entities.Fire, new() }
            , { GGConstants.ByteCodes.Entities.RedBomb, new() }
            , { GGConstants.ByteCodes.Entities.Trap, new() }
            , { GGConstants.ByteCodes.Entities.ToolThief, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.HintPanel, new() }
            , { GGConstants.ByteCodes.Entities.BlueButton, new() }
            , { GGConstants.ByteCodes.Entities.GreenButton, new() }
            , { GGConstants.ByteCodes.Entities.RedButton, new() }
            , { GGConstants.ByteCodes.Entities.BrownButton, new() }
            , { GGConstants.ByteCodes.Entities.BlueTeleporter, new() }
            , { GGConstants.ByteCodes.Entities.RedTeleporter, new() }
            , { GGConstants.ByteCodes.Entities.YellowTeleporter, new() }
            , { GGConstants.ByteCodes.Entities.GreenTeleporter, new() }
            , { GGConstants.ByteCodes.Entities.Wall, new(0) }
            , { GGConstants.ByteCodes.Entities.HiddenWall, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.BlueWall, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.FakeBlueWall, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.RedDoor, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.BlueDoor, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.YellowDoor, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.GreenDoor, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
            , { GGConstants.ByteCodes.Entities.Socket, new(0)
                {
                    PlayerLaw = BuildBitmask("NWSE")
                }
            }
        };

        public const byte North = 0x01;
        public const byte East = 0x02;
        public const byte South = 0x04;
        public const byte West = 0x08;

        public static byte BuildBitmask(string directions)
        {
            directions = directions.ToUpper();
            byte result = 0;
            if (directions.Contains('N'))
            {
                result = (byte)(result | North);
            }
            if (directions.Contains('E'))
            {
                result = (byte)(result | East);
            }
            if (directions.Contains('S'))
            {
                result = (byte)(result | South);
            }
            if (directions.Contains('W'))
            {
                result = (byte)(result | West);
            }
            return result;
        }
    }


    /// <summary>
    /// Contains rules to determine if a given entity can enter a tile 
    /// from the given carindal direction.
    /// </summary>
    public class EntryLaw
    {
        public byte PlayerLaw = 0;
        public byte BlockLaw = 0;
        public byte CreatureLaw = 0;
        
        public EntryLaw()
        {
            PlayerLaw = EntryLaws.BuildBitmask("NWSE");
            BlockLaw = EntryLaws.BuildBitmask("NWSE");
            CreatureLaw = EntryLaws.BuildBitmask("NWSE");
        }


        public EntryLaw(byte bitmask)
        {
            PlayerLaw = bitmask;
            BlockLaw = bitmask;
            CreatureLaw = bitmask;
        }


        public bool CanPlayerEnter(Vector2I direction)
        {
            return CanEnter(PlayerLaw, direction);
        }


        public bool CanBlockEnter(Vector2I direction)
        {
            return CanEnter(BlockLaw, direction);
        }


        public bool CanCreatureEnter(Vector2I direction)
        {
            return CanEnter(CreatureLaw, direction);
        }


        private bool CanEnter(int lawRules, Vector2I direction)
        {
            if ((lawRules & EntryLaws.North) != EntryLaws.North
                && direction.Y > 0)
            {
                GD.Print("Cannot enter from the North");
                return false;
            }
            if ((lawRules & EntryLaws.South) != EntryLaws.South
                && direction.Y < 0)
            {
                GD.Print("Cannot enter from the South");
                return false;
            }

            if ((lawRules & EntryLaws.East) != EntryLaws.East
                && direction.X < 0)
            {
                GD.Print("Cannot enter from the East");
                return false;
            }

            if ((lawRules & EntryLaws.West) != EntryLaws.West
                && direction.X > 0)
            {
                GD.Print("Cannot enter from the West");
                return false;
            }

            return true;
        }
    }
}
