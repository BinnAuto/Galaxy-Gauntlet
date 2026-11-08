using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class RedKeyItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.RedKey;

        public override string Name => "Red Key";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Archipelago.RedKeyUnlocked
                    ? GGConstants.SpriteCoordinates.RedKey
                    : GGConstants.SpriteCoordinates.LockedRedKey;
            }
        }
    }
}
