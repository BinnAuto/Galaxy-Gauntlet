using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class FlippersItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.Flippers;

        public override string Name => "Flippers";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Archipelago.FlippersUnlocked
                    ? GGConstants.SpriteCoordinates.Flippers
                    : GGConstants.SpriteCoordinates.LockedFlippers;
            }
        }
    }
}
