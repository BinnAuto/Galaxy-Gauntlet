using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class SecretEyeItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.SecretEye;

        public override string Name => "Secret Eye";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return GGConstants.SpriteCoordinates.SecretEye;
            }
        }
    }
}
