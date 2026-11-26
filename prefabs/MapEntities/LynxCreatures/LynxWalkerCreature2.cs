public partial class LynxWalkerCreature2(Vector2I coordinate) : MapLynxCreature2(coordinate)
{
	public override string MapObjectName => "Walker";

	public override int EntityID => GGConstants.ByteCodes.Entities.Walker;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Walker;
}
