using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class CloneMachineTile(Vector2I coordinate) : MapTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.CloneMachine_V1;

        public override string Name => "Clone Machine";

        public override Vector2I TextureCoordinate => GGConstants.SpriteCoordinates.CloneMachineTile;


        public void CloneEntity()
        {
            var entity = GameData.Map.GetMob(Coordinate);
            if(entity is null)
            {
                return;
            }

            var newEntity = entity.CreateCopy();
            newEntity.Coordinate = entity.Coordinate + entity.Forward;
            var existingMob = GameData.Map.GetMob(newEntity.Coordinate);
            if(existingMob is not null)
            {
                return;
            }

            GameData.Map.AddMob(newEntity);
        }
    }
}
