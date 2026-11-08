using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class SocketItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => GGConstants.ByteCodes.Entities.Socket;

        public override string Name => "Socket";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.NoSprite;
    }
}
