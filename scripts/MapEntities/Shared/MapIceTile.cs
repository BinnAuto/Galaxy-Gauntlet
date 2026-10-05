
namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
    public class MapIceTile(Vector2I coordinate, IceTileType iceType) : MapTile(coordinate)
    {
        public IceTileType IceTileType { get; private set; } = iceType;

        public override string Name
        {
            get
            {
                return IceTileType switch
                {
                    IceTileType.Standard => "Ice",
                    IceTileType.Barrier_NE => "Ice Corner (NE)",
                    IceTileType.Barrier_NW => "Ice Corner (NW)",
                    IceTileType.Barrier_SE => "Ice Corner (SE)",
                    IceTileType.Barrier_SW => "Ice Corner (SW)",
                    _ => "Ice"
                };
            }
        }

        public override int DataCode
        {
            get
            {
                return IceTileType switch
                {
                    IceTileType.Standard => Constants.ByteCodes.Entities.IceTile,
                    IceTileType.Barrier_NE => Constants.ByteCodes.Entities.Ice_NE,
                    IceTileType.Barrier_NW => Constants.ByteCodes.Entities.Ice_NW,
                    IceTileType.Barrier_SE => Constants.ByteCodes.Entities.Ice_SE,
                    IceTileType.Barrier_SW => Constants.ByteCodes.Entities.Ice_SW,
                    _ => Constants.ByteCodes.Entities.IceTile
                };
            }
        }


        public override Vector2I TextureCoordinate
        {
            get
            {
                return IceTileType switch
                {
                    IceTileType.Standard => Constants.SpriteCoordinates.IceTile,
                    IceTileType.Barrier_NE => Constants.SpriteCoordinates.Ice_NE,
                    IceTileType.Barrier_NW => Constants.SpriteCoordinates.Ice_NW,
                    IceTileType.Barrier_SE => Constants.SpriteCoordinates.Ice_SE,
                    IceTileType.Barrier_SW => Constants.SpriteCoordinates.Ice_SW,
                    _ => Constants.SpriteCoordinates.IceTile
                };
            }
        }


        public EntityOrientation SetEntityOrientation(EntityOrientation incomingOrientation)
        {
            switch(IceTileType)
            {
                case IceTileType.Standard:
                    return incomingOrientation;

                case IceTileType.Barrier_NE:
                    if(incomingOrientation == EntityOrientation.North)
                    {
                        return EntityOrientation.West;
                    }
                    if(incomingOrientation == EntityOrientation.East)
                    {
                        return EntityOrientation.South;
                    }
                    break;

                case IceTileType.Barrier_NW:
                    if(incomingOrientation == EntityOrientation.North)
                    {
                        return EntityOrientation.East;
                    }
                    if(incomingOrientation == EntityOrientation.West)
                    {
                        return EntityOrientation.South;
                    }
                    break;

                case IceTileType.Barrier_SE:
                    if(incomingOrientation == EntityOrientation.South)
                    {
                        return EntityOrientation.West;
                    }
                    if(incomingOrientation == EntityOrientation.East)
                    {
                        return EntityOrientation.North;
                    }
                    break;

                case IceTileType.Barrier_SW:
                    if (incomingOrientation == EntityOrientation.South)
                    {
                        return EntityOrientation.East;
                    }
                    if(incomingOrientation == EntityOrientation.West)
                    {
                        return EntityOrientation.North;
                    }
                    break;
            }
            return incomingOrientation;
        }
    }
}
