public partial class Player2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Player";

	public override int ByteCode => GGConstants.ByteCodes.Entities.Player;


	public override void ChooseMove()
	{
		ChooseChipMove();
	}


	private void ChooseChipMove()
	{
		bool isSlipping = HasState(CreatureStateFlags.Slipping);
		// TODO
	}
}
