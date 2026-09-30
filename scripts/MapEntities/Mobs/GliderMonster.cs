using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GliderMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Glider;

        public override string Name => "Glider";

        public override bool DiesToWater => false;

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Orientation switch
                {
                    EntityOrientation.North => Constants.SpriteCoordinates.Glider_N,
                    EntityOrientation.East => Constants.SpriteCoordinates.Glider_E,
                    EntityOrientation.South => Constants.SpriteCoordinates.Glider_S,
                    EntityOrientation.West => Constants.SpriteCoordinates.Glider_W,
                    _ => throw new System.NotImplementedException()
                };
            }
        }


        public override void ProcessTick(DateTime timestamp)
        {
            if(CheckIfOnTrap(Forward))
            {
                return;
            }

            var forward = Forward;
            var forwardCoordinate = ProposeMove(forward);
            if(forwardCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(forward, forwardCoordinate);
                return;
            }

            var leftTurn = Left;
            var leftTurnCoordinate = ProposeMove(leftTurn);
            if (leftTurnCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(leftTurn, leftTurnCoordinate);
                return;
            }

            var rightTurn = Right;
            var rightTurnCoordinate = ProposeMove(rightTurn);
            if (rightTurnCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(rightTurn, rightTurnCoordinate);
                return;
            }

            var backward = Backward;
            var backwardCoordinate = ProposeMove(backward);
            if (backwardCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(backward, backwardCoordinate);
                return;
            }
        }
    }
}
