using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class BlueTeleporterItem(Vector2I coordinate) : MapTeleporter(coordinate)
    {
        public override bool NeedsTileSpecification => false;

        public override int DataCode => Constants.ByteCodes.Entities.BlueTeleporter;

        public override string Name => "Blue Teleporter";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.BlueTeleporter;


        public override Vector2I SearchForExit(Vector2I startingPoint)
        {
            var checkPosition = startingPoint;
            checkPosition -= new Vector2I(1, 0);
            while(checkPosition != startingPoint)
            {
                if(checkPosition == Coordinate)
                {
                    // Search has covered the entire map
                    return Coordinate;
                }

                if(checkPosition.X < 0)
                {
                    // Check has left the X bounds of the map, 
                    // wrap to row above
                    checkPosition = new(
                        GameData.MapDimensions.X - 1
                        , checkPosition.Y - 1
                    );
                    if(checkPosition.Y < 0)
                    {
                        // Check has left the Y bounds of the map,
                        // go to last row
                        checkPosition = GameData.MapDimensions - Vector2I.One;
                    }
                }

                var mapItem = GameData.GetMapItem(checkPosition);
                if (mapItem is BlueTeleporterItem)
                {
                    return checkPosition;
                }

                checkPosition -= new Vector2I(1, 0);
            }
            return startingPoint; // Is this needed?
        }
    }
}
