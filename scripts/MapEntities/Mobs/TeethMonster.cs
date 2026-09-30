using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class TeethMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Teeth;

        public override string Name => "Teeth";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Orientation switch
                {
                    EntityOrientation.North => Constants.SpriteCoordinates.Teeth_N,
                    EntityOrientation.East => Constants.SpriteCoordinates.Teeth_E,
                    EntityOrientation.South => Constants.SpriteCoordinates.Teeth_S,
                    EntityOrientation.West => Constants.SpriteCoordinates.Teeth_W,
                    _ => throw new NotImplementedException()
                };
            }
        }


        public override void ProcessTick(DateTime timestamp)
        {
            if(GameData.PingPongStep == 0)
            {
                // Teeth moves 2.5 times per second instead of the default 5
                return;
            }

            var playerDifference = Coordinate - GameData.PlayerCoordinate;
            if(Mathf.Abs(playerDifference.Y) >= Mathf.Abs(playerDifference.X))
            {
                var moved = MoveVertically(playerDifference.Y);
                if(false == moved)
                {
                    MoveHorizontally(playerDifference.X);
                }
            }
            else
            {
                var moved = MoveHorizontally(playerDifference.X);
                if(false == moved)
                {
                    MoveVertically(playerDifference.Y);
                }
            }
        }


        private bool MoveVertically(int amount)
        {
            if(amount == 0)
            {
                return false;
            }

            Vector2I direction = (amount > 0)
                ? new(0, -1)
                : new(0, 1);
            return ProposeDirection(direction);
        }


        private bool MoveHorizontally(int amount)
        {
            if (amount == 0)
            {
                return false;
            }

            Vector2I direction = (amount > 0)
                ? new(-1, 0)
                : new(1, 0);
            return ProposeDirection(direction);
        }


        private bool ProposeDirection(Vector2I direction)
        {
            var newCoordinate = ProposeMove(direction);
            SetOrientation(direction);
            if (newCoordinate != Coordinate)
            {
                SetCoordinate(newCoordinate);
                return true;
            }

            return false;
        }
    }
}
