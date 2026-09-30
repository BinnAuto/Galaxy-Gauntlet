using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GravelTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Gravel;

        public override string Name => "Gravel";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Gravel;
    }
}
