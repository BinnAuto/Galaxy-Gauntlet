using System;

namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
    public class MapTeleporter(Vector2I coordinate) : MapItem(coordinate)
    {
        public virtual Vector2I SearchForExit(Vector2I checkPoint)
        {
            throw new NotImplementedException();
        }
    }
}
