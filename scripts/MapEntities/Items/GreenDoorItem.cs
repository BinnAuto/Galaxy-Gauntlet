using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GreenDoorItem(Vector2I coordinate) : MapDoorItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.GreenDoor;

        public override string Name => "Green Door";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.GreenDoor;


        public override bool CanOpenDoor()
        {
            return GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.GreenKey);
        }
    }
}
