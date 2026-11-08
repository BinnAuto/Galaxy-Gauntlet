using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlueKeyItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.BlueKey;

        public override string Name => "Blue Key";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Archipelago.BlueKeyUnlocked
                    ? GGConstants.SpriteCoordinates.BlueKey
                    : GGConstants.SpriteCoordinates.LockedBlueKey;
            }
        }
    }
}
