using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class SuctionBootsItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.SuctionBoots;

        public override string Name => "Suction Boots";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.SuctionBootsUnlocked
                    ? Constants.SpriteCoordinates.SuctionBoots
                    : Constants.SpriteCoordinates.LockedSuctionBoots;
            }
        }
    }
}
