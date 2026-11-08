using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BallMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.Ball;

        public override string Name => "Ball";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.Ball;


        public override void ProcessTick()
        {
            var currentTile = GameData.Map.GetTile(Coordinate);

            #region Force Floor

            if (currentTile is MapForceFloorTile forceFloor)
            {
                GameData.Map.AddToSlipList(this);
                var forceFloorInfluence = forceFloor.GetInfluence();
                var forceCoordinate = ProposeMove(forceFloorInfluence);
                SetOrientationAndCoordinate(forceFloorInfluence, forceCoordinate);
                return;
            }

            #endregion

            #region Ice Floor

            if (currentTile is MapIceTile iceTile)
            {
                GameData.Map.AddToSlipList(this);
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

            GameData.Map.RemoveFromSlipList(this);

            var forwardCoordinate = ProposeMove(Forward);
            if(forwardCoordinate != Coordinate)
            {
                SetCoordinate(forwardCoordinate);
                return;
            }

            ReverseOrientation();
            forwardCoordinate = ProposeMove(Forward);
            SetCoordinate(forwardCoordinate);
        }


        public override MapMob CreateCopy()
        {
            var entityCoordinate = Coordinate.Clone();
            BallMonster result = new(entityCoordinate)
            {
                Orientation = Orientation
            };
            return result;
        }
    }
}
