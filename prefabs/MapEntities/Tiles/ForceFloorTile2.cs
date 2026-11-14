using GalaxyGauntlet.scripts;
using System;

public partial class ForceFloorTile2(int entityId, Vector2I coordinate) : MapTile2(coordinate)
{
	public override int EntityID => entityId;

	public override string MapObjectName => entityId switch
	{
		GGConstants.ByteCodes.Entities.ForceFloor_N => "Force Floor (N)",
		GGConstants.ByteCodes.Entities.ForceFloor_E => "Force Floor (E)",
		GGConstants.ByteCodes.Entities.ForceFloor_S => "Force Floor (S)",
		GGConstants.ByteCodes.Entities.ForceFloor_W => "Force Floor (W)",
		GGConstants.ByteCodes.Entities.ForceFloor_R => "Force Floor (Random)",
		_ => base.MapObjectName
	};


	public override int SpriteFrameIndex => entityId switch
	{
		GGConstants.ByteCodes.Entities.ForceFloor_N => GGConstants.SpriteFrameIndexes.ForceFloor_N,
		GGConstants.ByteCodes.Entities.ForceFloor_E => GGConstants.SpriteFrameIndexes.ForceFloor_E,
		GGConstants.ByteCodes.Entities.ForceFloor_S => GGConstants.SpriteFrameIndexes.ForceFloor_S,
		GGConstants.ByteCodes.Entities.ForceFloor_W => GGConstants.SpriteFrameIndexes.ForceFloor_W,
		GGConstants.ByteCodes.Entities.ForceFloor_R => GGConstants.SpriteFrameIndexes.ForceFloor_R,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};


    public Vector2I SetCreatureDirection()
    {
		switch(EntityID) {
			case GGConstants.ByteCodes.Entities.ForceFloor_N:
				return EntityOrientation.North.ToVector();

            case GGConstants.ByteCodes.Entities.ForceFloor_E:
                return EntityOrientation.East.ToVector();

            case GGConstants.ByteCodes.Entities.ForceFloor_S:
                return EntityOrientation.South.ToVector();

            case GGConstants.ByteCodes.Entities.ForceFloor_W:
                return EntityOrientation.West.ToVector();

            case GGConstants.ByteCodes.Entities.ForceFloor_R:
				Vector2I[] directions = [
					EntityOrientation.North.ToVector(),
					EntityOrientation.East.ToVector(),
					EntityOrientation.South.ToVector(),
					EntityOrientation.West.ToVector()
				];
				Random.Shared.Shuffle(directions);
				return directions[0];

			default:
				return EntityOrientation.North.ToVector();
        }
    }
}
