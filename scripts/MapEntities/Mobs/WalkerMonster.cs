using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class WalkerMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.Walker;

        public override string Name => "Walker";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return Orientation switch
                {
                    EntityOrientation.North => Constants.SpriteCoordinates.Walker_N,
                    EntityOrientation.East => Constants.SpriteCoordinates.Walker_E,
                    EntityOrientation.South => Constants.SpriteCoordinates.Walker_S,
                    EntityOrientation.West => Constants.SpriteCoordinates.Walker_W,
                    _ => throw new System.NotImplementedException()
                };
            }
        }
    }
}
