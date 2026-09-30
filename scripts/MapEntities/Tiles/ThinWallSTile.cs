using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ThinWallSTile(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => true;

        public override int DataCode => Constants.ByteCodes.Entities.ThinWall_S;

        public override string Name => "Thin Wall (S)";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.ThinWall_S;
    }
}
