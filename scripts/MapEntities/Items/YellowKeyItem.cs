using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class YellowKeyItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.YellowKey;

        public override string Name => "Yellow Key";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Archipelago.YellowKeyUnlocked
                    ? GGConstants.SpriteCoordinates.YellowKey
                    : GGConstants.SpriteCoordinates.LockedYellowKey;
            }
        }
    }
}
