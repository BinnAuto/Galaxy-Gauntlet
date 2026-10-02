using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ParameciumMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Paramecium;

        public override string Name => "Paramecium";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Orientation switch
                {
                    EntityOrientation.North => Constants.SpriteCoordinates.Paramecium_N,
                    EntityOrientation.East => Constants.SpriteCoordinates.Paramecium_E,
                    EntityOrientation.South => Constants.SpriteCoordinates.Paramecium_S,
                    EntityOrientation.West => Constants.SpriteCoordinates.Paramecium_W,
                    _ => throw new NotImplementedException()
                };
            }
        }


        public override void ProcessTick()
        {
            var rightTurn = Right;
            var rightTurnCoordinate = ProposeMove(rightTurn);
            if (rightTurnCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(rightTurn, rightTurnCoordinate);
                return;
            }

            var forward = Forward;
            var forwardCoordinate = ProposeMove(forward);
            if (forwardCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(forward, forwardCoordinate);
                return;
            }

            var leftTurn = Left;
            var leftTurnCoordinate = ProposeMove(leftTurn);
            SetOrientation(leftTurn);
            if (leftTurnCoordinate != Coordinate)
            {
                SetOrientationAndCoordinate(leftTurn, leftTurnCoordinate);
                return;
            }
        }
    }
}
