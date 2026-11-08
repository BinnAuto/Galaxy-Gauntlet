using GalaxyGauntlet.scripts;

public partial class ShoeItem2(int byteCode, Vector2I coordinate) : MapItem2(coordinate)
{
	public override int ByteCode => byteCode;

	protected override int SpriteFrameIndex => ByteCode switch
	{
		GGConstants.ByteCodes.Entities.Flippers => GGConstants.SpriteFrameIndexes.Flippers,
		GGConstants.ByteCodes.Entities.FireBoots => GGConstants.SpriteFrameIndexes.FireBoots,
		GGConstants.ByteCodes.Entities.IceSkates => GGConstants.SpriteFrameIndexes.IceSkates,
		GGConstants.ByteCodes.Entities.SuctionBoots => GGConstants.SpriteFrameIndexes.SuctionBoots,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};



	public bool IsLocked => ByteCode switch
	{
		GGConstants.ByteCodes.Entities.Flippers => GameData.Archipelago.FlippersUnlocked,
		GGConstants.ByteCodes.Entities.FireBoots => GameData.Archipelago.FireBootsUnlocked,
		GGConstants.ByteCodes.Entities.IceSkates => GameData.Archipelago.IceSkatesUnlocked,
		GGConstants.ByteCodes.Entities.SuctionBoots => GameData.Archipelago.SuctionBootsUnlocked,
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
