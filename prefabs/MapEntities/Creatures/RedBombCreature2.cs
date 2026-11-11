public partial class RedBombCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Red Bomb";

	public override int EntityID => GGConstants.ByteCodes.Entities.RedBomb;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RedBomb;
}
