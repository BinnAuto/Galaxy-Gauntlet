using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class RedButtonItem(Vector2I coordinate) : MapButtonItem(coordinate, Color.Red)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.RedButton;

        public override string Name => "Red Button";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.RedButton;

        public Vector2I? CloneMachineCoordinate = null;

        public override void OnPressed()
        {
            if(CloneMachineCoordinate.HasValue)
            {
                var cloneMachine = (CloneMachineTile)GameData.GetMapTile(CloneMachineCoordinate.Value);
                cloneMachine.CloneEntity();
                return;
            }

            Vector2I checkCoordinate = Coordinate.Clone();
            while(true)
            {
                var tile = GameData.GetMapTile(checkCoordinate);
                if(tile is CloneMachineTile cloneMachine)
                {
                    CloneMachineCoordinate = checkCoordinate;
                    cloneMachine.CloneEntity();
                    return;
                }

                checkCoordinate.X++;
                if(checkCoordinate.X >= GameData.MapDimensions.X)
                {
                    checkCoordinate.X = 0;
                    checkCoordinate.Y++;
                    if(checkCoordinate.Y >= GameData.MapDimensions.Y)
                    {
                        checkCoordinate.Y = 0;
                    }
                }
                if(checkCoordinate == Coordinate)
                {
                    // Search has wrapped the entire map
                    return;
                }
            }
        }
    }
}
