public partial class LynxBlobCreature2(Vector2I coordinate) : MapLynxCreature2(coordinate)
{
	public override string MapObjectName => "Blob";

	public override int EntityID => GGConstants.ByteCodes.Entities.Blob;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Blob;
}
