using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class DirtTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.Dirt;

        public override string Name => "Dirt";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.Dirt;
    }
}
