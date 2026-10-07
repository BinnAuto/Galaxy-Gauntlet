using GalaxyGauntlet.scripts.MapEntities.Shared;
using System.Collections.Generic;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlobMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Blob;

        public override string Name => "Blob";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Blob;

        public override MapMob CreateCopy()
        {
            var entityCoordinate = Coordinate.Clone();
            BlobMonster result = new(entityCoordinate)
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

            if (GameData.PingPongStep == 0)
            {
                // Blob moves 2.5 times per second instead of the default 5
                return;
            }

            List<EntityOrientation> orientations = [
                EntityOrientation.North,
                EntityOrientation.South,
                EntityOrientation.East,
                EntityOrientation.West
            ];
            int listSize = orientations.Count;
            RandomNumberGenerator rng = new();
            while(listSize > 0)
            {
                int index = rng.RandiRange(0, orientations.Count - 1);
                Vector2I direction = orientations[index].ToVector();
                var newCoordinate = ProposeMove(direction);
                if (newCoordinate != Coordinate)
                {
                    SetOrientationAndCoordinate(direction, newCoordinate);
                    return;
                }
                else
                {
                    orientations.RemoveAt(index);
                    if (orientations.Count == 0)
                    {
                        // No move
                        return;
                    }
                }
                if (GameData.LynxBehavior)
                {
                    // Blobs in Lynx implementations only search
                    // one orientation per game tick, as opposed to
                    // MS which searches as many as necessary.
                    return;
                }
            }
        }
    }
}
