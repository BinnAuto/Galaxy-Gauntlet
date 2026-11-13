using GalaxyGauntlet.scripts;

public partial class ParameciumCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Paramecium";

	public override int EntityID => GGConstants.ByteCodes.Entities.Paramecium;

	public override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Paramecium_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Paramecium_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Paramecium_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Paramecium_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
