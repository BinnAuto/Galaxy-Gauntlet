using GalaxyGauntlet.scripts;

public partial class BugCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Bug;

	protected override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Bug_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Bug_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Bug_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Bug_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
