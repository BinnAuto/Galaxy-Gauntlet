using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class RedDoorItem(Vector2I coordinate) : MapDoorItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.RedDoor;

        public override string Name => "Red Door";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.RedDoor;


        public override bool CanOpenDoor()
        {
            return GameData.ConsumeItem(Constants.ByteCodes.Entities.RedKey);
        }
    }
}
