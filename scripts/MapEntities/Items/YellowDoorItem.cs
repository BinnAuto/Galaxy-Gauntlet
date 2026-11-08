using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class YellowDoorItem(Vector2I coordinate) : MapDoorItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.YellowDoor;

        public override string Name => "Yellow Door";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.YellowDoor;


        public override bool CanOpenDoor()
        {
            return GameData.Level.ConsumeItem(GGConstants.ByteCodes.Entities.YellowKey);
        }
    }
}
