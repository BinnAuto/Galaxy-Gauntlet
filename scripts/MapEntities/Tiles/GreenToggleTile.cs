using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GreenToggleTile(Vector2I coordinate, bool isWall) : MapTile(coordinate)
    {
        public bool IsWall = isWall;

        public override int DataCode
        {
            get
            {
                return IsWall
                    ? GGConstants.ByteCodes.Entities.GreenToggleWall 
                    : GGConstants.ByteCodes.Entities.GreenToggleFloor;
            }
        }

        public override string Name
        {
            get
            {
                return IsWall ? "Green Toggle Wall" : "Green Toggle Floor";
            }
        }

        public override Vector2I TextureCoordinate
        {
            get
            {
                return IsWall
                    ? GGConstants.SpriteCoordinates.GreenToggleWall
                    : GGConstants.SpriteCoordinates.GreenToggleFloor;
            }
        }
    }
}
