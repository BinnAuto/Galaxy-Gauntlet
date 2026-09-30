using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BrownButtonItem(Vector2I coordinate) : MapButtonItem(coordinate, Color.Brown)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.BrownButton;

        public override string Name => "Brown Button";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.BrownButton;

        public Vector2I? TrapCoordinate = null;

        public override void OnPressed()
        {
            if(TrapCoordinate.HasValue)
            {
                var trapItem = (TrapItem)GameData.GetMapItem(TrapCoordinate.Value);
                trapItem.SetState(false);
                return;
            }

            // Look for next trap
            Vector2I checkCoordinate = Coordinate.Clone();
            while(true)
            {
                var item = GameData.GetMapItem(checkCoordinate);
                if(item is TrapItem trapItem)
                {
                    TrapCoordinate = checkCoordinate;
                    trapItem.SetState(false);
                    return;
                }

                checkCoordinate.X++;
                if (checkCoordinate.X >= GameData.MapDimensions.X)
                {
                    checkCoordinate.X = 0;
                    checkCoordinate.Y++;
                    if (checkCoordinate.Y >= GameData.MapDimensions.Y)
                    {
                        checkCoordinate.Y = 0;
                    }
                }
                if (checkCoordinate == Coordinate)
                {
                    // Search has wrapped the entire map
                    return;
                }
            }
        }


        public override void OnUnpressed()
        {
            if (TrapCoordinate.HasValue)
            {
                var trapItem = (TrapItem)GameData.GetMapItem(TrapCoordinate.Value);
                trapItem.SetState(true);
                return;
            }

            // Look for next trap
            Vector2I checkCoordinate = Coordinate.Clone();
            while (true)
            {
                var item = GameData.GetMapItem(checkCoordinate);
                if (item is TrapItem trapItem)
                {
                    TrapCoordinate = checkCoordinate;
                    trapItem.SetState(true);
                    return;
                }

                checkCoordinate.X++;
                if (checkCoordinate.X >= GameData.MapDimensions.X)
                {
                    checkCoordinate.X = 0;
                    checkCoordinate.Y++;
                    if (checkCoordinate.Y >= GameData.MapDimensions.Y)
                    {
                        checkCoordinate.Y = 0;
                    }
                }
                if (checkCoordinate == Coordinate)
                {
                    // Search has wrapped the entire map
                    return;
                }
            }
        }
    }
}
