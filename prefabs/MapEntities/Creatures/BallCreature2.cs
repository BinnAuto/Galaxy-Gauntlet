public partial class BallCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Ball";

	public override int EntityID => GGConstants.ByteCodes.Entities.Ball;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Ball;
}
