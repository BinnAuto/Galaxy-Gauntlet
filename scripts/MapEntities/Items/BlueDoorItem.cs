using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlueDoorItem(Vector2I coordinate) : MapDoorItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.BlueDoor;

        public override string Name => "Blue Door";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.BlueDoor;


        public override bool CanOpenDoor()
        {
            return GameData.Level.ConsumeItem(GGConstants.ByteCodes.Entities.BlueKey);
        }
    }
}
