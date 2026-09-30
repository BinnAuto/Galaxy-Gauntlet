using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class GreenButtonItem(Vector2I coordinate) : MapButtonItem(coordinate, Color.Green)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.GreenButton;

        public override string Name => "Green Button";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.GreenButton;


        public override void OnPressed()
        {
            GameData.FlipGreenToggles();
        }
    }
}
