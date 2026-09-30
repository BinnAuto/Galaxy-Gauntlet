using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class YellowTeleporterItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.YellowTeleporter;

        public override string Name => "Yellow Teleporter";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.YellowTeleporter;
    }
}
