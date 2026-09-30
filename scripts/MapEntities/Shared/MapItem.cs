
namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
    public class MapItem(Vector2I coordinate) : MapEntity(coordinate)
    {
        public override bool NeedsTileSpecification => true;

        public MapTile LowerLayer = new FloorTile(coordinate);
    }
}
