using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ThinWallSETile(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => true;

        public override int DataCode => Constants.ByteCodes.Entities.ThinWall_SE;

        public override string Name => "Thin Wall (SE)";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.ThinWall_SE;
    }
}
