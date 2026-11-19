public partial class LynxBallCreature2(Vector2I coordinate) : MapLynxCreature2(coordinate)
{
	public override string MapObjectName => "Ball";

	public override int EntityID => GGConstants.ByteCodes.Entities.Ball;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Ball;
}
