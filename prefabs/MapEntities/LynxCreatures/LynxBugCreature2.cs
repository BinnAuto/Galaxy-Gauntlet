using GalaxyGauntlet.scripts;

public partial class LynxBugCreature2(Vector2I coordinate) : MapLynxCreature2(coordinate)
{
	public override string MapObjectName => "Bug";

	public override int EntityID => GGConstants.ByteCodes.Entities.Bug;

	public override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Bug_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Bug_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Bug_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Bug_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
