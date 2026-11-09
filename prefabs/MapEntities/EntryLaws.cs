using System.Collections.Generic;

namespace GalaxyGauntlet.prefabs.MapEntities
{
    public static class EntryLaws
    {
        public static Dictionary<int, EntryLaw> Laws = new()
        {
            { GGConstants.ByteCodes.Entities.Floor, new() }
            , { GGConstants.ByteCodes.Entities.Wall, new(0) }
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

        private const byte North = 0x01;
        private const byte East = 0x02;
        private const byte South = 0x04;
        private const byte West = 0x08;

        public EntryLaw() { }


        public EntryLaw(int value)
        {
            Player = value;
            Block = value;
            Creature = value;
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
