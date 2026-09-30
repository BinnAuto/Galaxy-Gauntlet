using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class HintTile(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.Hint;

        public override string Name => "Hint";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Hint;
    }
}
