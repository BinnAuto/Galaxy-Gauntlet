using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class DebugEntity(Vector2I coordinate, int code, string name) : MapEntity(coordinate)
    {
        public override int DataCode => code;

        public override string Name => name;

        public override Vector2I TextureCoordinate => Vector2I.Zero;
    }
}
