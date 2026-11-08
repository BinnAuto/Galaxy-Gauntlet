using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class IceBlockEntity(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.IceBlock;

        public override string Name => "Ice Block";

        public override bool CanMoveSelf => false;

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Level.XRayMode
                    ? GGConstants.SpriteCoordinates.IceBlock_XRay
                    : GGConstants.SpriteCoordinates.IceBlock;
            }
        }


        public override MapMob CreateCopy()
        {
            var entityCoordinate = Coordinate.Clone();
            IceBlockEntity result = new(entityCoordinate)
            {
                Orientation = Orientation
            };
            return result;
        }
    }
}
