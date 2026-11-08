using GalaxyGauntlet.scripts;

public partial class MapObject(Vector2I coordinate) : Node2D
{
	public Vector2I Coordinate { get; set; } = coordinate;

	public virtual int ByteCode => GGConstants.ByteCodes.Entities.None;

	public bool IsDead = false;

	protected virtual int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.DebugSprite;

	public virtual bool RequiresDirectionByte => false;

	public virtual bool RequiresLowerLayerTile => false;

	private AnimatedSprite2D Sprite;


	#region Godot Overrides

	public override void _Ready()
	{
		Sprite = new()
		{
			SpriteFrames = (SpriteFrames)GD.Load("res://sprite_frames/tile_sprites.tres"),
			TextureFilter = TextureFilterEnum.Nearest,
			Frame = SpriteFrameIndex
		};
		AddChild(Sprite);
	}


	public override void _Process(double delta)
	{
		if(IsDead)
		{
			var parent = GetParent();
			parent.RemoveChild(this);
			QueueFree();
			Visible = false;
			return;
		}

		SetDisplayPosition();
	}

	#endregion


	/// <summary>
	/// Sets the object's position relative to the display viewport,
	/// hiding the object if it is outside of the viewport.
	/// </summary>
	public void SetDisplayPosition()
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
		if(Coordinate.X < minX
			|| Coordinate.X > maxX
			|| Coordinate.Y < minY
			|| Coordinate.Y > maxY)
		{
			Visible = false;
			return;
		}

		Visible = true;
		int xDelta = Coordinate.X - minX;
		int yDelta = Coordinate.Y - minY;
		Position = 32 * new Vector2I(xDelta - 1, yDelta - 1);
	}


	/// <summary>
	/// Hides this node from the scene and marks it for removal.
	/// </summary>
	public void Kill()
	{
		IsDead = true;
		Visible = false;
	}


	// TODO
	public static MapObject GetMapObject(int byteCode, Vector2I coordinate)
	{
		return byteCode switch
		{
			/* Tiles */
			GGConstants.ByteCodes.Entities.Floor => new FloorTile2(coordinate),
			GGConstants.ByteCodes.Entities.Wall => new WallTile2(coordinate),
			GGConstants.ByteCodes.Entities.RedDoor
				or GGConstants.ByteCodes.Entities.BlueDoor
				or GGConstants.ByteCodes.Entities.YellowDoor
				or GGConstants.ByteCodes.Entities.GreenDoor => new KeyDoorTile2(byteCode, coordinate),
			GGConstants.ByteCodes.Entities.HintPanel => new HintPanelTile2(coordinate),
			GGConstants.ByteCodes.Entities.Socket => new SocketTile2(coordinate),
			GGConstants.ByteCodes.Entities.Exit => new ExitTile2(coordinate),
			GGConstants.ByteCodes.Entities.Water => new WaterTile2(coordinate),
			GGConstants.ByteCodes.Entities.CloneMachine
				or GGConstants.ByteCodes.Entities.CloneMachine_V1 => new CloneMachineTile2(coordinate),
			GGConstants.ByteCodes.Entities.BlueButton => new BlueButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.GreenButton => new GreenButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.RedButton => new RedButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.BrownButton => new BrownButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.GrayButton => new GrayButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.ThinWallOrCanopy => new ThinWallOrCanopy2(coordinate),
			GGConstants.ByteCodes.Entities.BlueWall
				or GGConstants.ByteCodes.Entities.FakeBlueWall => new BlueWallTile2(byteCode, coordinate),

			/* Items */
			GGConstants.ByteCodes.Entities.Chip
				or GGConstants.ByteCodes.Entities.ExtraChip => new ChipItem2(coordinate),
			GGConstants.ByteCodes.Entities.RedKey
				or GGConstants.ByteCodes.Entities.BlueKey
				or GGConstants.ByteCodes.Entities.YellowKey
				or GGConstants.ByteCodes.Entities.GreenKey => new KeyItem2(byteCode, coordinate),
			GGConstants.ByteCodes.Entities.Flippers => new FlippersItem2(coordinate),
			GGConstants.ByteCodes.Entities.FireBoots => new FireBootsItem2(coordinate),
			GGConstants.ByteCodes.Entities.SuctionBoots => new SuctionBootsItem2(coordinate),
			GGConstants.ByteCodes.Entities.IceSkates => new IceSkatesItem2(coordinate),

			/* Creatures */
			GGConstants.ByteCodes.Entities.Player => new Player2(coordinate),
			GGConstants.ByteCodes.Entities.Bug => new BugCreature2(coordinate),
			GGConstants.ByteCodes.Entities.DirtBlock => new DirtBlockCreature2(coordinate),
			GGConstants.ByteCodes.Entities.RedBomb => new RedBombCreature2(coordinate),
			_ => LogUnrecognizedByteCode(byteCode, coordinate)
		};
	}


	private static MapObject LogUnrecognizedByteCode(int byteCode, Vector2I coordinate)
	{
		GD.Print($"Byte code 0x{byteCode:X2} not recognized.");
		return new MapObject(coordinate);
	}
}
