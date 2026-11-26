using GalaxyGauntlet.scripts;

public partial class GliderCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Glider";

	public override int EntityID => GGConstants.ByteCodes.Entities.Glider;

	public override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Glider_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Glider_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Glider_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Glider_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
