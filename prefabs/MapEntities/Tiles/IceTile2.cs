public partial class IceTile2(int entityId, Vector2I coordinate) : MapTile2(coordinate)
{
	public override int EntityID => entityId;

	public override string MapObjectName => entityId switch
	{
		GGConstants.ByteCodes.Entities.IceTile => "Ice",
		GGConstants.ByteCodes.Entities.Ice_NW => "Ice Wall (NW)",
		GGConstants.ByteCodes.Entities.Ice_NE => "Ice Wall (NE)",
		GGConstants.ByteCodes.Entities.Ice_SW => "Ice Wall (SW)",
		GGConstants.ByteCodes.Entities.Ice_SE => "Ice Wall (SE)",
		_ => base.MapObjectName
	};


	public override int SpriteFrameIndex => entityId switch
	{
		GGConstants.ByteCodes.Entities.IceTile => GGConstants.SpriteFrameIndexes.Ice,
		GGConstants.ByteCodes.Entities.Ice_NW => GGConstants.SpriteFrameIndexes.Ice_NW,
		GGConstants.ByteCodes.Entities.Ice_NE => GGConstants.SpriteFrameIndexes.Ice_NE,
		GGConstants.ByteCodes.Entities.Ice_SW => GGConstants.SpriteFrameIndexes.Ice_SW,
		GGConstants.ByteCodes.Entities.Ice_SE => GGConstants.SpriteFrameIndexes.Ice_SE,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};
}
