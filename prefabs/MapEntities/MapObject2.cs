using GalaxyGauntlet.scripts;

public partial class MapObject2(Vector2I coordinate) : Node2D
{
	public bool IsDead = false;

	public virtual string MapObjectName => "mapobj";

	public Vector2I Coordinate = coordinate;

	public virtual bool RequiresDirectionByte => false;

	public virtual bool RequiresLowerLayerTile => false;

	public virtual bool AcceptsModifier => false;

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


	public virtual void AcceptModifier(byte[] modifierValues)
	{
		GD.Print($"Modifier logic not implemented for {MapObjectName}");
	}


	public void PlaySoundEffect(string soundName)
	{
		// TODO
	}


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
			GGConstants.ByteCodes.Entities.Fire => new FireTile2(coordinate),
			GGConstants.ByteCodes.Entities.IceTile
				or GGConstants.ByteCodes.Entities.Ice_NE
				or GGConstants.ByteCodes.Entities.Ice_NW
				or GGConstants.ByteCodes.Entities.Ice_SE
				or GGConstants.ByteCodes.Entities.Ice_SW => new IceTile2(byteCode, coordinate),
			GGConstants.ByteCodes.Entities.ForceFloor_N
				or GGConstants.ByteCodes.Entities.ForceFloor_E
				or GGConstants.ByteCodes.Entities.ForceFloor_S
				or GGConstants.ByteCodes.Entities.ForceFloor_W
				or GGConstants.ByteCodes.Entities.ForceFloor_R => new ForceFloorTile2(byteCode, coordinate),
			GGConstants.ByteCodes.Entities.BlueButton => new BlueButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.GreenButton => new GreenButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.GreenToggleFloor => new GreenToggleTile2(false, coordinate),
			GGConstants.ByteCodes.Entities.GreenToggleWall => new GreenToggleTile2(true, coordinate),
			GGConstants.ByteCodes.Entities.RedButton => new RedButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.CloneMachine
				or GGConstants.ByteCodes.Entities.CloneMachine_V1 => new CloneMachineTile2(coordinate),
			GGConstants.ByteCodes.Entities.BrownButton => new BrownButtonTile2(coordinate),
			GGConstants.ByteCodes.Entities.TrapActive => new TrapTile2(coordinate),
			GGConstants.ByteCodes.Entities.ToolThief => new ToolThiefTile2(coordinate),
			GGConstants.ByteCodes.Entities.BlueWall => new BlueWallTile2(true, coordinate),
            GGConstants.ByteCodes.Entities.FakeBlueWall => new BlueWallTile2(false, coordinate),
			GGConstants.ByteCodes.Entities.HiddenWall => new HiddenWallTile2(coordinate),
			GGConstants.ByteCodes.Entities.InvisibleWall => new InvisibleWallTile2(coordinate),
			GGConstants.ByteCodes.Entities.BlueTeleporter => new BlueTeleporterTile2(coordinate),
			GGConstants.ByteCodes.Entities.RedTeleporter => new RedTeleporterTile2(coordinate),
			GGConstants.ByteCodes.Entities.GreenTeleporter => new GreenTeleporterTile2(coordinate),
			GGConstants.ByteCodes.Entities.Gravel => new GravelTile2(coordinate),
			GGConstants.ByteCodes.Entities.Dirt => new DirtTile2(coordinate),
			GGConstants.ByteCodes.Entities.RecessedWall => new RecessedWallTile2(coordinate),

            /* Thin Wall or Canopy */
            GGConstants.ByteCodes.Entities.ThinWall_E => new ThinWallOrCanopy2(coordinate, 2),
			GGConstants.ByteCodes.Entities.ThinWall_S => new ThinWallOrCanopy2(coordinate, 4),
			GGConstants.ByteCodes.Entities.ThinWall_SE => new ThinWallOrCanopy2(coordinate, 6),
			GGConstants.ByteCodes.Entities.ThinWallOrCanopy => new ThinWallOrCanopy2(coordinate),

			/* Items */
			GGConstants.ByteCodes.Entities.Chip 
				or GGConstants.ByteCodes.Entities.ExtraChip => new ChipItem2(coordinate),
			GGConstants.ByteCodes.Entities.RedKey => new RedKeyItem2(coordinate),
			GGConstants.ByteCodes.Entities.BlueKey => new BlueKeyItem2(coordinate),
			GGConstants.ByteCodes.Entities.YellowKey => new YellowKeyItem2(coordinate),
			GGConstants.ByteCodes.Entities.GreenKey => new GreenKeyItem2(coordinate),
			GGConstants.ByteCodes.Entities.Flippers => new FlippersItem2(coordinate),
			GGConstants.ByteCodes.Entities.FireBoots => new FireBootsItem2(coordinate),
			GGConstants.ByteCodes.Entities.SuctionBoots => new SuctionBootsItem2(coordinate),
			GGConstants.ByteCodes.Entities.IceSkates => new IceSkatesItem2(coordinate),

			/* Creatures */
			GGConstants.ByteCodes.Entities.Player => new Player2(coordinate),
			GGConstants.ByteCodes.Entities.Bug => new BugCreature2(coordinate),
			GGConstants.ByteCodes.Entities.DirtBlock => new DirtBlockCreature2(coordinate),
			GGConstants.ByteCodes.Entities.RedBomb => new RedBombCreature2(coordinate),
			GGConstants.ByteCodes.Entities.BlueTank => new BlueTankCreature2(coordinate),
			GGConstants.ByteCodes.Entities.Ball => new BallCreature2(coordinate),
			GGConstants.ByteCodes.Entities.Glider => new GliderCreature2(coordinate),
			GGConstants.ByteCodes.Entities.Fireball => new FireballCreature2(coordinate),
			GGConstants.ByteCodes.Entities.Teeth => new TeethCreature2(coordinate),
			GGConstants.ByteCodes.Entities.Walker => new WalkerCreature2(coordinate),
			GGConstants.ByteCodes.Entities.Paramecium => new ParameciumCreature2(coordinate),

			/* Other */
			GGConstants.ByteCodes.Entities.Modifier_8Bit
				or GGConstants.ByteCodes.Entities.Modifier_16Bit
				or GGConstants.ByteCodes.Entities.Modifier_32Bit => null,
			_ => LogUnknownObject(byteCode, coordinate)
		};
	}


	private static MapObject2 LogUnknownObject(int byteCode, Vector2I coordinate)
	{
		FileLogger.LogMessage($"Entity with code 0x{byteCode:X2} not recognized");
		return new(coordinate);
	}
}
