using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class DirtTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Dirt;

        public override string Name => "Dirt";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Dirt;
    }
}
