using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BallMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Ball;

        public override string Name => "Ball";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Ball;


        public override void ProcessTick()
        {
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
