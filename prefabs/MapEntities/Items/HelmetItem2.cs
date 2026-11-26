public partial class HelmetItem2(Vector2I coordinate) : MapItem2(coordinate)
{
    public override string MapObjectName => "Helmet";

    public override int EntityID => GGConstants.ByteCodes.Entities.Helmet;

    public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Helmet;
}
