using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BugMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Bug;

        public override string Name => "Bug";

        public override bool AvoidsFire => true;

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Orientation switch
                {
                    EntityOrientation.North => Constants.SpriteCoordinates.Bug_N,
                    EntityOrientation.East => Constants.SpriteCoordinates.Bug_E,
                    EntityOrientation.South => Constants.SpriteCoordinates.Bug_S,
                    EntityOrientation.West => Constants.SpriteCoordinates.Bug_W,
                    _ => throw new NotImplementedException()
                };
            }
        }


        public override void ProcessTick(DateTime timestamp)
        {
            var leftTurn = Left;
            var leftTurnCoordinate = ProposeMove(leftTurn);
            if(leftTurnCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(leftTurn, leftTurnCoordinate);
                return;
            }

            var forward = Forward;
            var forwardCoordinate = ProposeMove(forward);
            if(forwardCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(forward, forwardCoordinate);
                return;
            }

            var rightTurn = Right;
            var rightTurnCoordinate = ProposeMove(rightTurn);
            SetOrientation(rightTurn);
            if(rightTurnCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(rightTurn, rightTurnCoordinate);
                return;
            }
        }
    }
}
