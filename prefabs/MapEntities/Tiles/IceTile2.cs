using GalaxyGauntlet.scripts;

public partial class IceTile2(int byteCode, Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => byteCode;

	public EntityOrientation GetNewCreatureDirection(EntityOrientation enterOrientation)
	{
		return ByteCode switch
		{
			GGConstants.ByteCodes.Entities.IceTile => enterOrientation,
			GGConstants.ByteCodes.Entities.Ice_SW => enterOrientation switch
			{
				EntityOrientation.South => EntityOrientation.East,
				EntityOrientation.West => EntityOrientation.North,
				_ => enterOrientation
			},
			GGConstants.ByteCodes.Entities.Ice_SE => enterOrientation switch
			{
				EntityOrientation.South => EntityOrientation.West,
				EntityOrientation.East => EntityOrientation.North,
				_ => enterOrientation
			},
			GGConstants.ByteCodes.Entities.Ice_NW => enterOrientation switch
			{
				EntityOrientation.North => EntityOrientation.East,
				EntityOrientation.West => EntityOrientation.South,
				_ => enterOrientation
			},
			GGConstants.ByteCodes.Entities.Ice_NE => enterOrientation switch
			{
				EntityOrientation.North => EntityOrientation.West,
				EntityOrientation.East => EntityOrientation.South,
				_ => enterOrientation
			},
			_ => enterOrientation,
		};
	}
}
