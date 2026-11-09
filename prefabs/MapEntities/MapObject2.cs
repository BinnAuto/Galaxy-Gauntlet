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
		parent.RemoveChild(this);
		QueueFree();
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
			GGConstants.ByteCodes.Entities.Floor => new FloorTile2(coordinate),
			GGConstants.ByteCodes.Entities.Wall => new WallTile2(coordinate),
			GGConstants.ByteCodes.Entities.Player => new Player2(coordinate),
			_ => LogUnknownObject(byteCode, coordinate)
		};
	}


	private static MapObject2 LogUnknownObject(int byteCode, Vector2I coordinate)
	{
		FileLogger.LogMessage($"Entity with code 0x{byteCode:X2} not recognized");
		return new(coordinate);
	}
}
