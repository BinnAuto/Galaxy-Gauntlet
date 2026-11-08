using GalaxyGauntlet.scripts;

public partial class BlueTankCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.BlueTank;

	protected override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.BlueTank_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.BlueTank_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.BlueTank_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.BlueTank_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
