using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class DirtBlockEntity(Vector2I coordinate) : MapMob(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.DirtBlock;

        public override string Name => "Dirt Block";

        public override Vector2I TextureCoordinate => Constants.SpriteCoordinates.DirtBlock;


        public override Vector2I ProposeMove(Vector2I direction)
        {
            try
            {
                if (CheckIfOnTrap(direction))
                {
                    return Coordinate;
                }

                var currentTile = GameData.GetMapTile(Coordinate);
                if (currentTile is CloneMachineTile)
                {
                    return Coordinate;   
                }

                Vector2I newCoordinate = Coordinate + direction;

                var destinationTile = GameData.GetMapTile(newCoordinate);
                if(destinationTile is WallTile
                    || destinationTile is BlueWallTile
                    || destinationTile is CloneMachineTile
                    || destinationTile is DirtTile
                    || (destinationTile is GreenToggleTile greenToggle && greenToggle.IsWall))
                {
                    return Coordinate;
                }

                if(destinationTile is WaterTile)
                {
                    GameData.SetMapTile(newCoordinate, new DirtTile(newCoordinate));
                    RemoveSelf();
                    return newCoordinate;
                }

                var destinationMob = GameData.GetMapMob(newCoordinate);
                if(destinationMob is DirtBlockEntity
                    || destinationMob is IceBlockEntity
                    || destinationMob is BugMonster
                    || destinationMob is ParameciumMonster
                    || destinationMob is WalkerMonster
                    || destinationMob is GliderMonster
                    || destinationMob is FireballMonster
                    || destinationMob is TeethMonster
                    || destinationMob is BlueTankMonster
                    || destinationMob is BallMonster
                    || destinationMob is BlobMonster)
                {
                    return Coordinate;
                }

                if(destinationMob is RedBombMonster)
                {
                    destinationMob.RemoveSelf();
                    RemoveSelf();
                    return Coordinate;
                }

                var destinationItem = GameData.GetMapItem(newCoordinate);
                if(destinationItem is SocketItem)
                {
                    return Coordinate;
                }

                return newCoordinate;
            }
            catch
            {
                return Coordinate;
            }
        }
    }
}
