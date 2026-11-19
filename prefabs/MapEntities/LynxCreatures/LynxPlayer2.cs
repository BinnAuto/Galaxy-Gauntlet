using GalaxyGauntlet.scripts;

public partial class LynxPlayer2(bool isRealPlayer, Vector2I coordinate) : MapLynxCreature2(coordinate)
{
	#region Global Player Properties

	/// <summary>
	/// Is the player currently pushing up against a solid wall?
	/// </summary>
	public static bool ChipPushing = false;

	public static int PutWall = -1;

	public static Vector2I ChipToPosition = -Vector2I.One;

	public static MapLynxCreature2? ChipToCreature = null;

	/// <summary>
	/// Is the player currently stuck? (What does this mean?)
	/// </summary>
	public static bool IsStuck = false;

	#endregion

	public override string MapObjectName => "Player";

	public override int EntityID => GGConstants.ByteCodes.Entities.Player;

	public override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Player_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Player_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Player_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Player_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};

	public bool CouldntMove = false;


	public bool PlayerIsStuck = false;

	public readonly bool IsRealPlayer = isRealPlayer;

	#region Godot Overrides

	public override void _Ready()
	{
		base._Ready();
		ZIndex = GGConstants.ZIndexes.Player;
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	#endregion


	public void CheckMovingTo()
	{
		Global.AssertEqual(true, IsRealPlayer, "Wuh oh! We don't care about CheckMovingTo on non-players!");
		Vector2I direction = TDir;
		bool isDiagonal = (direction.X != 0 && direction.Y != 0);
		if(direction == Vector2I.Zero || isDiagonal)
		{
			ChipToPosition = -Vector2I.One;
			ChipToCreature = null;
			return;
		}

		ChipToPosition = Coordinate + direction;
		ChipToCreature = null;
	}
}
