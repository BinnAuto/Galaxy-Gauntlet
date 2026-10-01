namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
    public class MapDoorItem(Vector2I coordinate) : MapItem(coordinate)
    {
        public virtual bool CanOpenDoor()
        {
            return false;
        }
    }
}
