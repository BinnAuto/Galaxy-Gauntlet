using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ExitTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Exit;

        public override string Name => "Exit";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Exit;
    }
}
