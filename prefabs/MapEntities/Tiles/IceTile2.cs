using GalaxyGauntlet.scripts;

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


    // TODO: Review this. It doesn't make sense.
	public Vector2I SetCreatureDirection(Vector2I inputDirection)
	{
		switch(EntityID)
		{
			case GGConstants.ByteCodes.Entities.IceTile:
				return inputDirection;

			case GGConstants.ByteCodes.Entities.Ice_NE:
				if(inputDirection == EntityOrientation.North.ToVector())
				{
					return EntityOrientation.West.ToVector();
                }
                if (inputDirection == EntityOrientation.West.ToVector())
                {
                    return EntityOrientation.South.ToVector();
                }
				break;

            case GGConstants.ByteCodes.Entities.Ice_SW:
                if (inputDirection == EntityOrientation.South.ToVector())
                {
                    return EntityOrientation.East.ToVector();
                }
                if (inputDirection == EntityOrientation.West.ToVector())
                {
                    return EntityOrientation.North.ToVector();
                }
                break;

            case GGConstants.ByteCodes.Entities.Ice_NW:
                if (inputDirection == EntityOrientation.North.ToVector())
                {
                    return EntityOrientation.East.ToVector();
                }
                if (inputDirection == EntityOrientation.West.ToVector())
                {
                    return EntityOrientation.South.ToVector();
                }
                break;

            case GGConstants.ByteCodes.Entities.Ice_SE:
                if (inputDirection == EntityOrientation.South.ToVector())
                {
                    return EntityOrientation.West.ToVector();
                }
                if (inputDirection == EntityOrientation.East.ToVector())
                {
                    return EntityOrientation.North.ToVector();
                }
                break;
        }
		return Vector2I.Zero;
	}
}
