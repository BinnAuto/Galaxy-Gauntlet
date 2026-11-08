using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class HintPanelItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => GGConstants.ByteCodes.Entities.HintPanel;

        public override string Name => "Hint Panel";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.HintPanel;
    }
}
