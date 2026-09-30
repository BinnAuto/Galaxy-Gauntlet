using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ForceFloorETile(Vector2I coordinate) : MapForceFloorTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.ForceFloor_E;

        public override string Name => "Force Floor (E)";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.ForceFloor_E;

        public override Vector2I GetInfluence()
        {
            return new(1, 0);
        }
    }
}
