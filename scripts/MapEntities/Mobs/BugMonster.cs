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


        public override MapMob CreateCopy()
        {
            var entityCoordinate = Coordinate.Clone();
            BugMonster result = new(entityCoordinate)
            {
                Orientation = Orientation
            };
            return result;
        }


        public override void ProcessTick()
        {
            var currentTile = GameData.GetMapTile(Coordinate);

            #region Force Floor

            if (currentTile is MapForceFloorTile forceFloor)
            {
                GameData.AddToSlipList(this);
                var forceFloorInfluence = forceFloor.GetInfluence();
                var forceCoordinate = ProposeMove(forceFloorInfluence);
                SetOrientationAndCoordinate(forceFloorInfluence, forceCoordinate);
                return;
            }

            #endregion

            #region Ice Floor

            if (currentTile is MapIceTile iceTile)
            {
                GameData.AddToSlipList(this);
                Orientation = iceTile.SetEntityOrientation(Orientation);
                var iceCoordinate = ProposeMove(Forward);
                if (iceCoordinate == Coordinate)
                {
                    ReverseOrientation();
                    Orientation = iceTile.SetEntityOrientation(Orientation);
                    iceCoordinate = ProposeMove(Forward);
                }
                SetCoordinate(iceCoordinate);
                return;
            }

            #endregion

            GameData.RemoveFromSlipList(this);

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
