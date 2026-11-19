using GalaxyGauntlet.scripts;

public partial class LynxTeethCreature2(Vector2I coordinate) : MapLynxCreature2(coordinate)
{
	public override string MapObjectName => "Teeth";

	public override int EntityID => GGConstants.ByteCodes.Entities.Teeth;

	public override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Teeth_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Teeth_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Teeth_N,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Teeth_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
