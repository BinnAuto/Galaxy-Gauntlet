using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class FireBootsItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.FireBoots;

        public override string Name => "Fire Boots";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.FireBootsUnlocked
                    ? Constants.SpriteCoordinates.FireBoots
                    : Constants.SpriteCoordinates.LockedFireBoots;
            }
        }
    }
}
