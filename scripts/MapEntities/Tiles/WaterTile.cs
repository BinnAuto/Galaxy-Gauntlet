using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class WaterTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Water;

        public override string Name => "Water";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Water;
    }
}
