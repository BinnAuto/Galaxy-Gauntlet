using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class DirtBlockEntity(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.DirtBlock;

        public override string Name => "Dirt Block";

        public override bool CanWalkOnDirt => false;

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.XRayMode
                    ? Constants.SpriteCoordinates.DirtBlock_XRay
                    : Constants.SpriteCoordinates.DirtBlock;
            }
        }

        public override MapMob CreateCopy()
        {
            var entityCoordinate = Coordinate.Clone();
            DirtBlockEntity result = new(entityCoordinate)
            {
                Orientation = Orientation
            };
            return result;
        }
    }
}
