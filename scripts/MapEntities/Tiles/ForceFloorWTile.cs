using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ForceFloorWTile(Vector2I coordinate) : MapForceFloorTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.ForceFloor_W;

        public override string Name => "Force Floor (W)";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.ForceFloor_W;

        public override Vector2I GetInfluence()
        {
            return new(-1, 0);
        }
    }
}
