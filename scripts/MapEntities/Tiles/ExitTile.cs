using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ExitTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.Exit;

        public override string Name => "Exit";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.Exit;
    }
}
