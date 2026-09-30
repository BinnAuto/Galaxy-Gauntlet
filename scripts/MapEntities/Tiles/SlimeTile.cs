using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class SlimeTile(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Slime;

        public override string Name => "Slime";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Slime;
    }
}
