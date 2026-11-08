using GalaxyGauntlet.scripts;

public partial class KeyItem2(int byteCode, Vector2I coordinate) : MapItem2(coordinate)
{
	public override int ByteCode => byteCode;

	protected override int SpriteFrameIndex => ByteCode switch
	{
		GGConstants.ByteCodes.Entities.RedKey => GGConstants.SpriteFrameIndexes.RedKey,
		GGConstants.ByteCodes.Entities.BlueKey => GGConstants.SpriteFrameIndexes.BlueKey,
		GGConstants.ByteCodes.Entities.YellowKey => GGConstants.SpriteFrameIndexes.YellowKey,
		GGConstants.ByteCodes.Entities.GreenKey => GGConstants.SpriteFrameIndexes.GreenKey,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};


	public bool IsLocked => ByteCode switch
	{
		GGConstants.ByteCodes.Entities.RedKey => GameData.Archipelago.RedKeyUnlocked,
		GGConstants.ByteCodes.Entities.BlueKey => GameData.Archipelago.BlueKeyUnlocked,
		GGConstants.ByteCodes.Entities.YellowKey => GameData.Archipelago.YellowKeyUnlocked,
		GGConstants.ByteCodes.Entities.GreenKey => GameData.Archipelago.GreenKeyUnlocked,
		_ => true
	};


	public override void _Process(double delta)
	{
		base._Process(delta);
		Modulate = IsLocked
			? Godot.Color.Color8(255, 255, 255, 60)
			: Godot.Color.Color8(255, 255, 255, 255);
	}
}
