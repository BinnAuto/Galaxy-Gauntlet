using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class SocketItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.Socket;

        public override string Name => "Socket";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return (GameData.ChipsCollected >= GameData.ChipsRequired)
                    ? Constants.SpriteCoordinates.Socket_Open
                    : Constants.SpriteCoordinates.Socket_Closed;
            }
        }
    }
}
