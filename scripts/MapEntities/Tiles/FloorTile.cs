using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class FloorTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.Floor;

        public override string Name => "Floor";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.Floor;
    }
}
