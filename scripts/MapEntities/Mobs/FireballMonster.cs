using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class FireballMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Fireball;

        public override string Name => "Fireball";

        public override bool DiesToFire => false;

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Orientation switch
                {
                    EntityOrientation.North => Constants.SpriteCoordinates.Fireball_N,
                    EntityOrientation.East => Constants.SpriteCoordinates.Fireball_E,
                    EntityOrientation.South => Constants.SpriteCoordinates.Fireball_S,
                    EntityOrientation.West => Constants.SpriteCoordinates.Fireball_W,
                    _ => throw new NotImplementedException()
                };
            }
        }


        public override void ProcessTick(DateTime timestamp)
        {
            var forward = Forward;
            var forwardCoordinate = ProposeMove(forward);
            if(forwardCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(forward, forwardCoordinate);
                return;
            }

            var rightTurn = Right;
            var rightTurnCoordinate = ProposeMove(rightTurn);
            if(rightTurnCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(rightTurn, rightTurnCoordinate);
                return;
            }

            var leftTurn = Left;
            var leftTurnCoordinate = ProposeMove(leftTurn);
            if(leftTurnCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(leftTurn, leftTurnCoordinate);
                return;
            }

            var backwards = Backward;
            var backwardCoordinate = ProposeMove(backwards);
            if(backwardCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(backwards, backwardCoordinate);
            }
        }


        public override MapMob CreateCopy()
        {
            var entityCoordinate = Coordinate.Clone();
            FireballMonster result = new(entityCoordinate)
            {
                Orientation = Orientation
            };
            return result;
        }
    }
}
