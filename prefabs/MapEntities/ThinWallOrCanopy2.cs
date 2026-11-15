using GalaxyGauntlet.scripts;

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
			if(IsNorth)
			{
				result += "N";
			}
			if (IsEast)
			{
				result += "E";
			}
			if (IsSouth)
			{
				result += "S";
			}
			if (IsWest)
			{
				result += "W";
			}
			if (IsCanopy)
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

	private bool IsNorth => (WallBitmask & 1) == 1;
	private bool IsEast => (WallBitmask & 2) == 2;
    private bool IsSouth => (WallBitmask & 4) == 4;
    private bool IsWest => (WallBitmask & 8) == 8;
	private bool IsCanopy => (WallBitmask & 16) == 16;

    public ThinWallOrCanopy2(Vector2I coordinate)
		: base(coordinate) { }


	public ThinWallOrCanopy2(Vector2I coordinate, int wallBitmask)
		: base(coordinate)
	{
		WallBitmask = wallBitmask;
	}


	private AnimatedSprite2D CanopySprite;


	public override void _Ready()
	{
		base._Ready();
		CanopySprite = new();

		#region Build wall sprites

		if(IsNorth)
		{
			var sprite = BuildWallSprite(GGConstants.ThinWallSpriteFrameIndexes.North_West);
			AddChild(sprite);
			sprite.Position = new(-16, -16);

			sprite = BuildWallSprite(GGConstants.ThinWallSpriteFrameIndexes.North_East);
			AddChild(sprite);
			sprite.Position = new(0, -16);
		}
		if(IsEast)
		{
			var sprite = BuildWallSprite(GGConstants.ThinWallSpriteFrameIndexes.East_North);
			AddChild(sprite);
			sprite.Position = new(0, -16);

            sprite = BuildWallSprite(GGConstants.ThinWallSpriteFrameIndexes.East_South);
            AddChild(sprite);
            sprite.Position = new(0, 0);
        }
		if(IsSouth)
		{
			var sprite = BuildWallSprite(GGConstants.ThinWallSpriteFrameIndexes.South_West);
			AddChild(sprite);
			sprite.Position = new(-16, 0);

			sprite = BuildWallSprite(GGConstants.ThinWallSpriteFrameIndexes.South_East);
			AddChild(sprite);
			sprite.Position = new(0, 0);
		}
		if(IsWest)
		{
			var sprite = BuildWallSprite(GGConstants.ThinWallSpriteFrameIndexes.West_North);
			AddChild(sprite);
			sprite.Position = new(-16, -16);

            sprite = BuildWallSprite(GGConstants.ThinWallSpriteFrameIndexes.West_South);
            AddChild(sprite);
            sprite.Position = new(-16, 0);
        }
		if(IsCanopy)
		{
			CanopySprite = new()
			{
				SpriteFrames = (SpriteFrames)GD.Load("res://sprite_frames/tile_sprites.tres"),
				TextureFilter = TextureFilterEnum.Nearest,
				Frame = GGConstants.SpriteFrameIndexes.Canopy,
				ZIndex = GGConstants.ZIndexes.Canopy
			};
			AddChild(CanopySprite);
        }

		#endregion
	}


    public override void _Process(double delta)
    {
        base._Process(delta);
		if(CanopySprite is not null)
		{
			CanopySprite.Frame = GameData.Level.XRayMode
				? GGConstants.SpriteFrameIndexes.Canopy_XRay
				: GGConstants.SpriteFrameIndexes.Canopy;
		}
    }


	public bool CreatureCanExit(Vector2I direction)
	{
		if(WallBitmask is null || WallBitmask == 0)
		{
			return true;
		}
		if(IsNorth && direction.Y < 0)
		{
			return false;
		}
		if(IsEast && direction.X > 0)
		{
			return false;
		}
		if(IsSouth && direction.Y > 0)
		{
			return false;
		}
		if(IsWest && direction.X < 0)
		{
			return false;
		}

		return true;
	}


	public bool CreatureCanEnter(Vector2I direction)
    {
        if (WallBitmask is null || WallBitmask == 0)
        {
            return true;
        }
        if (IsNorth && direction.Y > 0)
        {
            return false;
        }
        if (IsEast && direction.X < 0)
        {
            return false;
        }
        if (IsSouth && direction.Y < 0)
        {
            return false;
        }
        if (IsWest && direction.X > 0)
        {
            return false;
        }

		return true;
    }


	private AnimatedSprite2D BuildWallSprite(int frameIndex)
    {
        AnimatedSprite2D sprite = new()
        {
            SpriteFrames = (SpriteFrames)GD.Load("res://sprite_frames/thin_wall_sprites.tres"),
            TextureFilter = TextureFilterEnum.Nearest,
			ZIndex = ZIndex + 1,
			Centered = false,
            Frame = frameIndex
        };
		return sprite;
    }
}
