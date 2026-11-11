using GalaxyGauntlet.scripts;

public partial class MapObject2(Vector2I coordinate) : Node2D
{
	public bool IsDead = false;

	public virtual string MapObjectName => "mapobj";

	public Vector2I Coordinate = coordinate;

	public virtual bool RequiresDirectionByte => false;

	public virtual bool RequiresLowerLayerTile => false;

	public virtual int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.DebugSprite;

	public virtual int EntityID => GGConstants.ByteCodes.Entities.None;

	private AnimatedSprite2D Sprite;

	private AudioStreamPlayer2D Audio;


	#region Godot Overrides

	public override void _Ready()
	{
		// Construct Sprite object
		Name = $"GG_{MapObjectName}";
		Sprite = new()
		{
			SpriteFrames = (SpriteFrames)GD.Load("res://sprite_frames/tile_sprites.tres"),
			TextureFilter = TextureFilterEnum.Nearest,
			Frame = SpriteFrameIndex
		};
		AddChild(Sprite);

		Audio = new();
		AddChild(Audio);
	}


	public override void _Process(double delta)
	{
		Sprite.Frame = SpriteFrameIndex;
		SetDisplayPosition();
	}


	#endregion


	public void Kill()
	{
		Visible = false;
		IsDead = true;
		var parent = GetParent();
		if(parent is null)
		{
			GD.Print($"!!! No parent for {MapObjectName} at {Coordinate}");
		}
		parent?.RemoveChild(this);
		QueueFree();
	}


	public void PlaySoundEffect(string soundName)
	{
		// TODO
	}


	private void SetDisplayPosition()
	{
		var mapDimensions = GameData.Map.Dimensions;
		var playerCoordinate = GameData.Level.PlayerCoordinate;
		int viewportSize = GameData.Level.MapViewportSize;
		int offset = (viewportSize - 1) / 2;
		int minX = (mapDimensions.X - playerCoordinate.X) <= offset
			? mapDimensions.X - viewportSize
			: Mathf.Max(playerCoordinate.X - offset, 0);
		int maxX = Mathf.Min(minX + viewportSize - 1, mapDimensions.X);
		int minY = (mapDimensions.Y - playerCoordinate.Y <= offset)
			? mapDimensions.Y - viewportSize
			: Mathf.Max(playerCoordinate.Y - offset, 0);
		int maxY = Mathf.Min(minY + viewportSize - 1, mapDimensions.Y);
		if(minX > Coordinate.X || Coordinate.X > maxX
			|| minY > Coordinate.Y || Coordinate.Y > maxY)
		{
			Visible = false;
			return;
		}

		Visible = true;
		int xDelta = Coordinate.X - minX;
		int yDelta = Coordinate.Y - minY;
		Position = 32 * new Vector2I(xDelta - 1, yDelta - 1);
	}


	public static MapObject2 GetMapObject(int byteCode, Vector2I coordinate)
	{
		return byteCode switch
		{
			/* Tiles */
			GGConstants.ByteCodes.Entities.Floor => new FloorTile2(coordinate),
			GGConstants.ByteCodes.Entities.Wall => new WallTile2(coordinate),
			GGConstants.ByteCodes.Entities.Water => new WaterTile2(coordinate),
			GGConstants.ByteCodes.Entities.RedDoor => new RedDoorTile2(coordinate),
			GGConstants.ByteCodes.Entities.BlueDoor => new BlueDoorTile2(coordinate),
			GGConstants.ByteCodes.Entities.YellowDoor => new YellowDoorTile2(coordinate),
			GGConstants.ByteCodes.Entities.GreenDoor => new GreenDoorTile2(coordinate),
			GGConstants.ByteCodes.Entities.HintPanel => new HintTile2(coordinate),
			GGConstants.ByteCodes.Entities.Exit => new ExitTile2(coordinate),
			GGConstants.ByteCodes.Entities.Socket => new SocketTile2(coordinate),

			/* Items */
			GGConstants.ByteCodes.Entities.Chip => new ChipItem2(coordinate),
			GGConstants.ByteCodes.Entities.RedKey => new RedKeyItem2(coordinate),
			GGConstants.ByteCodes.Entities.BlueKey => new BlueKeyItem2(coordinate),
			GGConstants.ByteCodes.Entities.YellowKey => new YellowKeyItem2(coordinate),
			GGConstants.ByteCodes.Entities.GreenKey => new GreenKeyItem2(coordinate),

			/* Creatures */
			GGConstants.ByteCodes.Entities.Player => new Player2(coordinate),
			GGConstants.ByteCodes.Entities.Bug => new BugCreature2(coordinate),
			GGConstants.ByteCodes.Entities.DirtBlock => new DirtBlockCreature2(coordinate),
			_ => LogUnknownObject(byteCode, coordinate)
		};
	}


	private static MapObject2 LogUnknownObject(int byteCode, Vector2I coordinate)
	{
		FileLogger.LogMessage($"Entity with code 0x{byteCode:X2} not recognized");
		return new(coordinate);
	}
}
