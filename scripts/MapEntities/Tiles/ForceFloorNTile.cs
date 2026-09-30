using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ForceFloorNTile(Vector2I coordinate) : MapForceFloorTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.ForceFloor_N;

        public override string Name => "Force Floor (N)";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.ForceFloor_N;

        public override Vector2I GetInfluence()
        {
            return new(0, -1);
        }
    }
}
