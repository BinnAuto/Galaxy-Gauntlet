public partial class RedBombCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.RedBomb;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RedBomb;
}
