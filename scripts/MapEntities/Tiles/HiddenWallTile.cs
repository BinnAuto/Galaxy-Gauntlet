using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class HiddenWallTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.HiddenWall;

        public override string Name => "Hidden Wall";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GameData.XRayMode
                    ? Constants.SpriteCoordinates.Wall
                    : Constants.SpriteCoordinates.Floor;
            }
        }
    }
}
