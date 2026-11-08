using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GreenDoorItem(Vector2I coordinate) : MapDoorItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.GreenDoor;

        public override string Name => "Green Door";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.NoSprite;


        public override bool CanOpenDoor()
        {
            return GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.GreenKey);
        }
    }
}
