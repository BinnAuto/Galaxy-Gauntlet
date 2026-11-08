public partial class KeyDoorTile2(int byteCode, Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => byteCode;

	protected override int SpriteFrameIndex
	{
		get
		{
			return ByteCode switch
			{
				GGConstants.ByteCodes.Entities.RedDoor => GGConstants.SpriteFrameIndexes.RedDoor,
				GGConstants.ByteCodes.Entities.BlueDoor => GGConstants.SpriteFrameIndexes.BlueDoor,
				GGConstants.ByteCodes.Entities.YellowDoor => GGConstants.SpriteFrameIndexes.YellowDoor,
				GGConstants.ByteCodes.Entities.GreenDoor => GGConstants.SpriteFrameIndexes.GreenDoor,
				_ => GGConstants.SpriteFrameIndexes.DebugSprite
			};
		}
	}
}
