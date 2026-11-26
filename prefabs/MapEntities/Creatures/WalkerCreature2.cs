public partial class WalkerCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Walker";

	public override int EntityID => GGConstants.ByteCodes.Entities.Walker;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Walker;
}
