using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class FireTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Fire;

        public override string Name => "Fire";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Fire;
    }
}
