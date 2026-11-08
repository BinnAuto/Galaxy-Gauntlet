using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class RedDoorItem(Vector2I coordinate) : MapDoorItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.RedDoor;

        public override string Name => "Red Door";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.NoSprite;


        public override bool CanOpenDoor()
        {
            return GameData.Level.ConsumeItem(GGConstants.ByteCodes.Entities.RedKey);
        }
    }
}
