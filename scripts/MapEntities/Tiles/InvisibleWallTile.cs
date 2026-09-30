using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class InvisibleWallTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.InvisibleWall;

        public override string Name => "Invisible Wall";

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
