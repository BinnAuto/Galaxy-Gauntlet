using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlueDoorItem(Vector2I coordinate) : MapDoorItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.BlueDoor;

        public override string Name => "Blue Door";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.BlueDoor;


        public override bool CanOpenDoor()
        {
            return GameData.ConsumeItem(Constants.ByteCodes.Entities.BlueKey);
        }
    }
}
