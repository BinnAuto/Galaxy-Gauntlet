using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class YellowKeyItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.YellowKey;

        public override string Name => "Yellow Key";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.YellowKeyUnlocked
                    ? Constants.SpriteCoordinates.YellowKey
                    : Constants.SpriteCoordinates.LockedYellowKey;
            }
        }
    }
}
