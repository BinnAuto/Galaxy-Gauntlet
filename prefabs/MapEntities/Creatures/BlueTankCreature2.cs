using GalaxyGauntlet.scripts;

public partial class BlueTankCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Blue Tank";

	public override int EntityID => GGConstants.ByteCodes.Entities.BlueTank;

	public override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.BlueTank_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.BlueTank_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.BlueTank_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.BlueTank_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
