using GalaxyGauntlet.scripts;

public partial class ForceFloorTile2(int byteCode, Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => byteCode;


	protected override int SpriteFrameIndex => ByteCode switch
	{
		GGConstants.ByteCodes.Entities.ForceFloor_N => GGConstants.SpriteFrameIndexes.ForceFloor_N,
		GGConstants.ByteCodes.Entities.ForceFloor_E => GGConstants.SpriteFrameIndexes.ForceFloor_E,
		GGConstants.ByteCodes.Entities.ForceFloor_S => GGConstants.SpriteFrameIndexes.ForceFloor_S,
		GGConstants.ByteCodes.Entities.ForceFloor_W => GGConstants.SpriteFrameIndexes.ForceFloor_W,
		GGConstants.ByteCodes.Entities.ForceFloor_R => GGConstants.SpriteFrameIndexes.ForceFloor_R,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};


	public EntityOrientation GetInfluenceDirection()
	{
		EntityOrientation[] directions = [
			EntityOrientation.North,
			EntityOrientation.East,
			EntityOrientation.South,
			EntityOrientation.West
		];
		int randomIndex = (new RandomNumberGenerator()).RandiRange(0, 3);
		return ByteCode switch
		{
			GGConstants.ByteCodes.Entities.ForceFloor_N => directions[0],
			GGConstants.ByteCodes.Entities.ForceFloor_E => directions[1],
			GGConstants.ByteCodes.Entities.ForceFloor_S => directions[2],
			GGConstants.ByteCodes.Entities.ForceFloor_W => directions[3],
			GGConstants.ByteCodes.Entities.ForceFloor_R => directions[randomIndex],
			_ => directions[0]
		};
	}
}
