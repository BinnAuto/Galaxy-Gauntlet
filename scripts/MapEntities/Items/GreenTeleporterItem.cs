using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GreenTeleporterItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.GreenTeleporter;

        public override string Name => "Green Teleporter";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.GreenTeleporter;
    }
}
