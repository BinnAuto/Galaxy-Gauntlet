using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;
using System.Collections.Generic;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class WalkerMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Walker;

        public override string Name => "Walker";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Orientation switch
                {
                    EntityOrientation.North => Constants.SpriteCoordinates.Walker_N,
                    EntityOrientation.East => Constants.SpriteCoordinates.Walker_E,
                    EntityOrientation.South => Constants.SpriteCoordinates.Walker_S,
                    EntityOrientation.West => Constants.SpriteCoordinates.Walker_W,
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
                if(GameData.LynxBehavior)
                {
                    // Walkers in Lynx implementations only search
                    // one orientation per game tick, as opposed to
                    // MS which searches as many as necessary.
                    return;
                }
            }
        }
    }
}
