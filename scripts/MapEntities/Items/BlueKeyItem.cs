using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlueKeyItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.BlueKey;

        public override string Name => "Blue Key";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.BlueKeyUnlocked
                    ? Constants.SpriteCoordinates.BlueKey
                    : Constants.SpriteCoordinates.LockedBlueKey;
            }
        }
    }
}
