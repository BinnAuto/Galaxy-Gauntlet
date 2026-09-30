using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class IceSkatesItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.IceSkates;

        public override string Name => "Ice Skates";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.IceSkatesUnlocked
                    ? Constants.SpriteCoordinates.IceSkates
                    : Constants.SpriteCoordinates.LockedIceSkates;
            }
        }
    }
}
