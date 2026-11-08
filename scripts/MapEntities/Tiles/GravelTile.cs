using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GravelTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.Gravel;

        public override string Name => "Gravel";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.Gravel;
    }
}
