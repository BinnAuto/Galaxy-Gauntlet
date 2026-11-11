public partial class BribeItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Bribe";

	public override int EntityID => GGConstants.ByteCodes.Entities.Bribe;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Bribe;
}
