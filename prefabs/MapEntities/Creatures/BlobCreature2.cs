public partial class BlobCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
    public override string MapObjectName => "Blob";

    public override int EntityID => GGConstants.ByteCodes.Entities.Blob;

    public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Blob;
}
