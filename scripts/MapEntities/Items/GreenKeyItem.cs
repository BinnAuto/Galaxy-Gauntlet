using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GreenKeyItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.GreenKey;

        public override string Name => "Green Key";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Archipelago.GreenKeyUnlocked
                    ? GGConstants.SpriteCoordinates.GreenKey
                    : GGConstants.SpriteCoordinates.LockedGreenKey;
            }
        }
    }
}
