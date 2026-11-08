using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class FireBootsItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.FireBoots;

        public override string Name => "Fire Boots";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Archipelago.FireBootsUnlocked
                    ? GGConstants.SpriteCoordinates.FireBoots
                    : GGConstants.SpriteCoordinates.LockedFireBoots;
            }
        }
    }
}
