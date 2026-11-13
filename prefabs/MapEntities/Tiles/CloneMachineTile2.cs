using GalaxyGauntlet.scripts;

public partial class CloneMachineTile2(Vector2I coordinate) : MapTile2(coordinate)
{
    public override bool AcceptsModifier => true;

	public override string MapObjectName => "Clone Machine";

	public override int EntityID => GGConstants.ByteCodes.Entities.CloneMachine;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.CloneMachine;

    public EntityOrientation? CreatureOrientation { get; private set; } = null;

    public MapCreature2 CreatureToClone = null;


    public override void _Process(double delta)
    {
        base._Process(delta);
        if(CreatureToClone is not null)
        {
            GameData.Map.Creatures2[Coordinate.X, Coordinate.Y] ??= CreatureToClone;
            return;
        }

        CreatureToClone = GameData.Map.GetCreature(Coordinate);
    }


    public override void AcceptModifier(byte[] modifierValues)
    {
        if(modifierValues.Length == 0)
        {
            return;
        }

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
