using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;
using System.Collections.Generic;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class WalkerMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.Walker;

        public override string Name => "Walker";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Orientation switch
                {
                    EntityOrientation.North => GGConstants.SpriteCoordinates.Walker_N,
                    EntityOrientation.East => GGConstants.SpriteCoordinates.Walker_E,
                    EntityOrientation.South => GGConstants.SpriteCoordinates.Walker_S,
                    EntityOrientation.West => GGConstants.SpriteCoordinates.Walker_W,
                    _ => throw new NotImplementedException()
                };
            }
        }


        public override MapMob CreateCopy()
        {
            var entityCoordinate = Coordinate.Clone();
            WalkerMonster result = new(entityCoordinate)
            {
                Orientation = Orientation
            };
            return result;
        }


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

            var forward = Forward;
            var newCoordinate = ProposeMove(forward);
            if(newCoordinate != Coordinate)
            {
                SetCoordinate(newCoordinate);
                return;
            }

            FindNewDirection();
            forward = Forward;
            newCoordinate = ProposeMove(forward);
            SetCoordinate(newCoordinate);
        }


        private void FindNewDirection()
        {
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
                if(newCoordinate != Coordinate)
                {
                    Orientation = orientations[index];
                    return;
                }
                else
                {
                    orientations.RemoveAt(index);
                    if(orientations.Count == 0)
                    {
                        return;
                    }
                }
            }
        }
    }
}
