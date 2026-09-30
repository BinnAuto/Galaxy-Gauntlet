using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    internal class WallTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Wall;

        public override string Name => "Wall";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Wall;
    }
}
