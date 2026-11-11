public partial class ThinWallOrCanopy2 : MapObject2
{
	public override bool RequiresLowerLayerTile => true;

	public override string MapObjectName 
	{
		get
		{
			string result = "Thin Wall";
			if(WallBitmask is null)
			{
				return result;
			}

			result += "(";
			if((WallBitmask & 1) == 1)
			{
				result += "N";
			}
			if ((WallBitmask & 2) == 2)
			{
				result += "E";
			}
			if ((WallBitmask & 4) == 4)
			{
				result += "S";
			}
			if ((WallBitmask & 8) == 8)
			{
				result += "W";
			}
			if ((WallBitmask & 16) == 16)
			{
				result += "C";
			}
			result += ")";
			return result;
		}
	}

	public override int EntityID => GGConstants.ByteCodes.Entities.ThinWallOrCanopy;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.NoSprite;

	public int? WallBitmask { get; set; }


	public ThinWallOrCanopy2(Vector2I coordinate)
		: base(coordinate) { }


	public ThinWallOrCanopy2(Vector2I coordinate, int wallBitmask)
		: base(coordinate)
	{
		WallBitmask = wallBitmask;
	}


	public override void _Ready()
	{
		base._Ready();
		// TODO: Build wall sprites
	}
}
