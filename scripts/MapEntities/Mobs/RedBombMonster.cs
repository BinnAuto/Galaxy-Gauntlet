using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class RedBombMonster(Vector2I coordinate) : MapMob(coordinate)
    {
        public override bool NeedsOrientation => false;

        public override bool NeedsTileSpecification => true;

        public override int DataCode => Constants.ByteCodes.Entities.RedBomb;

        public override string Name => "Red Bomb";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.RedBomb;

        public override void ProcessTick()
        {
            // Do nothing. Ever.
        }
    }
}
