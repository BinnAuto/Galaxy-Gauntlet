using GalaxyGauntlet.scripts;

public class CreatureStateFlags
{
	public const int Released = 0x01;
	public const int Cloning = 0x02;
	public const int HasMoved = 0x04;
	public const int Turning = 0x08;
	public const int Slipping = 0x10;
	public const int Sliding = 0x20;
	public const int DeferPush = 0x40;
	public const int Mutant = 0x80;

}

public partial class MapCreature2(Vector2I coordinate) : MapObject2(coordinate)
{
	public int State = 0;

	public override bool RequiresDirectionByte => true;

	public override bool RequiresLowerLayerTile => true;

	public EntityOrientation Orientation = EntityOrientation.North;

	public Vector2I TDir { get; private set; } = Vector2I.Zero;


	public virtual void ChooseMove()
	{
		if(HasState(CreatureStateFlags.Slipping))
		{
			TDir = Vector2I.Zero;
		}
		else
		{
			// TODO: ChooseCreatureMove
		}
	}


	public virtual void AdvanceCreature(Vector2I direction)
	{
		// TODO
	}


	public virtual void UpdateCreature()
	{
		// TODO
	}


	public bool HasState(int state)
	{
		return (State & state) == state;
	}
}
