using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class NoTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => 0;

        public override string Name => "none";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.NoSprite;
    }
}
