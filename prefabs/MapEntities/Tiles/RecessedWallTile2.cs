public partial class RecessedWallTile2(Vector2I coordinate) : MapTile2(coordinate)
{
    public override string MapObjectName => "Recessed Wall";

    public override int EntityID => GGConstants.ByteCodes.Entities.RecessedWall;

    public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RecessedWall;
}
