public partial class GreenToggleTile2(bool isWall, Vector2I coordinate) : MapTile2(coordinate)
{
	public bool IsWall => isWall;

	public override int EntityID => IsWall
		? GGConstants.ByteCodes.Entities.GreenToggleWall
		: GGConstants.ByteCodes.Entities.GreenToggleFloor;


	public override string MapObjectName => IsWall
		? "Green Toggle Wall"
		: "Green Toggle Floor";


	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.GreenToggleFloor;


	private AnimatedSprite2D Wall;


	public override void _Ready()
	{
		GD.Print("Green floor ready");
		base._Ready();
		Wall = new()
		{
			SpriteFrames = (SpriteFrames)GD.Load("res://sprite_frames/tile_sprites.tres"),
			TextureFilter = TextureFilterEnum.Nearest,
			Frame = GGConstants.SpriteFrameIndexes.ToggleWall,
			ZIndex = 1
		};
		AddChild(Wall);
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
		Wall.Visible = IsWall;
	}
}
