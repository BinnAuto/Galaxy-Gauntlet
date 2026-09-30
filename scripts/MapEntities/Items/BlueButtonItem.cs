using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlueButtonItem(Vector2I coordinate) : MapButtonItem(coordinate, Color.Blue)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.BlueButton;

        public override string Name => "Blue Button";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.BlueButton;


        public override void OnPressed()
        {
            GameData.FlipBlueTanks();
        }
    }
}
