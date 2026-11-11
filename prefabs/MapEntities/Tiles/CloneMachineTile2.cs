using GalaxyGauntlet.scripts;

public partial class CloneMachineTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Clone Machine";

	public override int EntityID => GGConstants.ByteCodes.Entities.CloneMachine;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.CloneMachine;

    public EntityOrientation CreatureOrientation { get; private set; } = EntityOrientation.South;


    public override void AcceptModifier(byte[] modifierValues)
    {
        CreatureOrientation = modifierValues[0] switch
        {
            0x01 => EntityOrientation.North,
            0x02 => EntityOrientation.East,
            0x04 => EntityOrientation.South,
            0x08 => EntityOrientation.West,
            _ => EntityOrientation.South
        };
    }
}
