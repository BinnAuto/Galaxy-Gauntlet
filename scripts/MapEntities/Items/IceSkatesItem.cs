using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class IceSkatesItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.IceSkates;

        public override string Name => "Ice Skates";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Archipelago.IceSkatesUnlocked
                    ? GGConstants.SpriteCoordinates.IceSkates
                    : GGConstants.SpriteCoordinates.LockedIceSkates;
            }
        }
    }
}
