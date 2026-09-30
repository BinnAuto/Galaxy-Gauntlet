using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class RedTeleporterItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.RedTeleporter;

        public override string Name => "Red Teleporter";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.RedTeleporter;
    }
}
