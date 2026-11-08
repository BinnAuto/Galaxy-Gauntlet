using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class InvisibleWallTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.InvisibleWall;

        public override string Name => "Invisible Wall";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.Level.XRayMode
                    ? GGConstants.SpriteCoordinates.Wall
                    : GGConstants.SpriteCoordinates.Floor;
            }
        }
    }
}
