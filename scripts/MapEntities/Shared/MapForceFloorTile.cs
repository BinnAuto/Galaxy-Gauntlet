
namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
    public class MapForceFloorTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public virtual Vector2I GetInfluence()
        {
            return Vector2I.Zero;
        }
    }
}
