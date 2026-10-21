using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class HelmetItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Helmet;

        public override string Name => "Helmet";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Constants.SpriteCoordinates.Helmet;
            }
        }
    }
}
