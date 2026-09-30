using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ThinWallETile(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => true;

        public override int DataCode => Constants.ByteCodes.Entities.ThinWall_E;

        public override string Name => "Thin Wall (E)";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.ThinWall_E;
    }
}
