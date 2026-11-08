using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ForceFloorRTile(Vector2I coordinate) : MapForceFloorTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.ForceFloor_R;

        public override string Name => "Force Floor (Random)";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.ForceFloor_R;

        public override Vector2I GetInfluence()
        {
            EntityOrientation[] orientations = [
                EntityOrientation.North,
                EntityOrientation.South,
                EntityOrientation.East,
                EntityOrientation.West
            ];
            RandomNumberGenerator rng = new();
            int index = rng.RandiRange(0, orientations.Length - 1);
            return orientations[index].ToVector();
        }
    }
}
