using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class RecessedWallItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.RecessedWall;

        public override string Name => "Recessed Wall";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.RecessedWall;
    }
}
