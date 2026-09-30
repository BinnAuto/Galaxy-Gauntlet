using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class FlippersItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Flippers;

        public override string Name => "Flippers";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.FlippersUnlocked
                    ? Constants.SpriteCoordinates.Flippers
                    : Constants.SpriteCoordinates.LockedFlippers;
            }
        }
    }
}
