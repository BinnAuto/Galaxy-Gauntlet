using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlueWallTile(Vector2I coordinate, bool isReal) : MapTile(coordinate)
    {
        public bool IsReal = isReal;

        public override int DataCode
        {
            get
            {
                return IsReal ? 0x30 : 0x31;
            }
        }

        public override string Name
        {
            get
            {
                return IsReal ? "Blue Wall" : "Fake Blue Wall";
            }
        }

        public override Vector2I TextureCoordinate
        {
            get
            {
                if(IsReal)
                {
                    return Constants.SpriteCoordinates.BlueWall;
                }

                return GameData.XRayMode
                    ? Constants.SpriteCoordinates.Floor
                    : Constants.SpriteCoordinates.BlueWall;
            }
        }
    }
}
