using static Godot.Control;

public class ThinWallBitmask
{
	public const int North = 1;
	public const int East = 2;
	public const int South = 4;
	public const int West = 8;
	public const int Canopy = 16;
}

public partial class ThinWallOrCanopy2(Vector2I coordinate) : MapObject(coordinate)
{
	public override bool RequiresLowerLayerTile => true;

	public int Bitmask { get; private set; }

	public void SetWallBitmask(int bitmask)
	{
		Bitmask = bitmask;
	}


	/// <summary>
	/// Determine if a creature can leave this tile while facing the given direction
	/// </summary>
	public bool CanLeave(Vector2I exitDirection)
	{
		if((Bitmask & ThinWallBitmask.North) == ThinWallBitmask.North
			&& exitDirection.Y < 0)
		{
			return false;
		}
		if ((Bitmask & ThinWallBitmask.South) == ThinWallBitmask.South
			&& exitDirection.Y > 0)
		{
			return false;
		}
		if ((Bitmask & ThinWallBitmask.West) == ThinWallBitmask.West
			&& exitDirection.X < 0)
		{
			return false;
		}
		if ((Bitmask & ThinWallBitmask.East) == ThinWallBitmask.East
			&& exitDirection.X > 0)
		{
			return false;
		}
		return true;
	}


	/// <summary>
	/// Determine if a creature can enter this tile while facing the given direction
	/// </summary>
	public bool CanEnter(Vector2I enterDirection)
	{
		if ((Bitmask & ThinWallBitmask.North) == ThinWallBitmask.North
			&& enterDirection.Y > 0)
		{
			return false;
		}
		if ((Bitmask & ThinWallBitmask.South) == ThinWallBitmask.South
			&& enterDirection.Y < 0)
		{
			return false;
		}
		if ((Bitmask & ThinWallBitmask.West) == ThinWallBitmask.West
			&& enterDirection.X > 0)
		{
			return false;
		}
		if ((Bitmask & ThinWallBitmask.East) == ThinWallBitmask.East
			&& enterDirection.X < 0)
		{
			return false;
		}

		return true;
	}
}
