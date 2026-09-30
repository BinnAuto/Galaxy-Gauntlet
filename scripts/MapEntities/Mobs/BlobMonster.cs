using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlobMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Blob;

        public override string Name => "Blob";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.Blob;
    }
}
