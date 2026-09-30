using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ToolThief(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.ToolThief;

        public override string Name => "Tool Thief";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.ToolThief;
    }
}
