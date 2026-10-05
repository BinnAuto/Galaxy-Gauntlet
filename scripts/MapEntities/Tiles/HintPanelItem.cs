using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class HintPanelItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.HintPanel;

        public override string Name => "Hint Panel";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.HintPanel;
    }
}
