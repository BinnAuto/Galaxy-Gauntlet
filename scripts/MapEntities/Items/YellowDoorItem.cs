using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class YellowDoorItem(Vector2I coordinate) : MapDoorItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.YellowDoor;

        public override string Name => "Yellow Door";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.YellowDoor;
    }
}
