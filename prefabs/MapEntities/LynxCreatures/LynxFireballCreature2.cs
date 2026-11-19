using GalaxyGauntlet.scripts;

public partial class LynxFireballCreature2(Vector2I coordinate) : MapLynxCreature2(coordinate)
{
	public override string MapObjectName => "Fireball";

	public override int EntityID => GGConstants.ByteCodes.Entities.Fireball;

	public override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Fireball_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Fireball_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Fireball_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Fireball_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
