using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GreenKeyItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.GreenKey;

        public override string Name => "Green Key";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.GreenKeyUnlocked
                    ? Constants.SpriteCoordinates.GreenKey
                    : Constants.SpriteCoordinates.LockedGreenKey;
            }
        }
    }
}
