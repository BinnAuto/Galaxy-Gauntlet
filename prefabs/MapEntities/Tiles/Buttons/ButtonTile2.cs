public partial class ButtonTile2(int byteCode, Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => byteCode;

	protected override int SpriteFrameIndex => ByteCode switch
	{
		GGConstants.ByteCodes.Entities.BlueButton => GGConstants.SpriteFrameIndexes.BlueButton,
		GGConstants.ByteCodes.Entities.GreenButton => GGConstants.SpriteFrameIndexes.GreenButton,
		GGConstants.ByteCodes.Entities.RedButton => GGConstants.SpriteFrameIndexes.RedButton,
		GGConstants.ByteCodes.Entities.BrownButton => GGConstants.SpriteFrameIndexes.BrownButton,
		GGConstants.ByteCodes.Entities.GrayButton => GGConstants.SpriteFrameIndexes.GrayButton,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
