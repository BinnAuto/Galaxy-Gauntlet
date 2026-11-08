using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ForceFloorSTile(Vector2I coordinate) : MapForceFloorTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.ForceFloor_S;

        public override string Name => "Force Floor (S)";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.ForceFloor_S;

        public override Vector2I GetInfluence()
        {
            return new(0, 1);
        }
    }
}
