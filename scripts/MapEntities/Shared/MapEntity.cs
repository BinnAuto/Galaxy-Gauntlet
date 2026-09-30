
namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
    public class MapEntity(Vector2I coordinate)
    {
        public Vector2I Coordinate = coordinate;

        public virtual int DataCode { get; } = -1;

        public virtual string Name { get; } = "_none";

        public virtual Vector2I TextureCoordinate { get; } = -Vector2I.One;

        /// <summary>
        /// Indicates if this entity needs a tile defined beneath it.
        /// </summary>
        public virtual bool NeedsTileSpecification { get; } = false;

        /// <summary>
        /// Indicates if this entity needs a default orientation set.
        /// </summary>
        public virtual bool NeedsOrientation { get; } = false;


        public static MapEntity GetMapEntity(int code, Vector2I coordinate)
        {
            MapEntity result = code switch
            {
                Constants.ByteCodes.Entities.Floor => new FloorTile(coordinate),
                Constants.ByteCodes.Entities.Wall => new WallTile(coordinate),
                Constants.ByteCodes.Entities.IceTile => new MapIceTile(coordinate, IceTileType.Standard),
                Constants.ByteCodes.Entities.Ice_SW => new MapIceTile(coordinate, IceTileType.Barrier_SW),
                Constants.ByteCodes.Entities.Ice_NW => new MapIceTile(coordinate, IceTileType.Barrier_NW),
                Constants.ByteCodes.Entities.Ice_NE => new MapIceTile(coordinate, IceTileType.Barrier_NE),
                Constants.ByteCodes.Entities.Ice_SE => new MapIceTile(coordinate, IceTileType.Barrier_SE),
                Constants.ByteCodes.Entities.Water => new WaterTile(coordinate),
                Constants.ByteCodes.Entities.Fire => new FireTile(coordinate),
                Constants.ByteCodes.Entities.ForceFloor_N => new ForceFloorNTile(coordinate),
                Constants.ByteCodes.Entities.ForceFloor_E => new ForceFloorETile(coordinate),
                Constants.ByteCodes.Entities.ForceFloor_S => new ForceFloorSTile(coordinate),
                Constants.ByteCodes.Entities.ForceFloor_W => new ForceFloorWTile(coordinate),
                Constants.ByteCodes.Entities.GreenToggleWall => new GreenToggleTile(coordinate, true),
                Constants.ByteCodes.Entities.GreenToggleFloor => new GreenToggleTile(coordinate, false),
                Constants.ByteCodes.Entities.RedTeleporter => new RedTeleporterItem(coordinate),
                Constants.ByteCodes.Entities.BlueTeleporter => new BlueTeleporterItem(coordinate),
                Constants.ByteCodes.Entities.YellowTeleporter => new YellowTeleporterItem(coordinate),
                Constants.ByteCodes.Entities.GreenTeleporter => new GreenTeleporterItem(coordinate),
                Constants.ByteCodes.Entities.Exit => new ExitTile(coordinate),
                Constants.ByteCodes.Entities.Slime => new SlimeTile(coordinate),
                Constants.ByteCodes.Entities.Player => new Player(coordinate),
                Constants.ByteCodes.Entities.DirtBlock => new DirtBlockEntity(coordinate),
                Constants.ByteCodes.Entities.Walker => new WalkerMonster(coordinate),
                Constants.ByteCodes.Entities.Glider => new GliderMonster(coordinate),
                Constants.ByteCodes.Entities.IceBlock => new IceBlockEntity(coordinate),
                Constants.ByteCodes.Entities.ThinWall_S => new ThinWallOrCanopyTile(coordinate, Constants.ByteCodes.ThinWallBits.South),
                Constants.ByteCodes.Entities.ThinWall_E => new ThinWallOrCanopyTile(coordinate, Constants.ByteCodes.ThinWallBits.East),
                Constants.ByteCodes.Entities.ThinWall_SE => new ThinWallOrCanopyTile(coordinate, Constants.ByteCodes.ThinWallBits.South & Constants.ByteCodes.ThinWallBits.East),
                Constants.ByteCodes.Entities.Gravel => new GravelTile(coordinate),
                Constants.ByteCodes.Entities.GreenButton => new GreenButtonItem(coordinate),
                Constants.ByteCodes.Entities.BlueButton => new BlueButtonItem(coordinate),
                Constants.ByteCodes.Entities.BlueTank => new BlueTankMonster(coordinate),
                Constants.ByteCodes.Entities.RedDoor => new RedDoorItem(coordinate),
                Constants.ByteCodes.Entities.BlueDoor => new BlueDoorItem(coordinate),
                Constants.ByteCodes.Entities.YellowDoor => new YellowDoorItem(coordinate),
                Constants.ByteCodes.Entities.GreenDoor => new GreenDoorItem(coordinate),
                Constants.ByteCodes.Entities.RedKey => new RedKeyItem(coordinate),
                Constants.ByteCodes.Entities.BlueKey => new BlueKeyItem(coordinate),
                Constants.ByteCodes.Entities.YellowKey => new YellowKeyItem(coordinate),
                Constants.ByteCodes.Entities.GreenKey => new GreenKeyItem(coordinate),
                Constants.ByteCodes.Entities.Chip
                    or Constants.ByteCodes.Entities.ExtraChip => new ChipItem(code, coordinate),
                Constants.ByteCodes.Entities.Socket => new SocketItem(coordinate),
                Constants.ByteCodes.Entities.RecessedWall => new RecessedWallItem(coordinate),
                Constants.ByteCodes.Entities.HiddenWall => new HiddenWallTile(coordinate),
                Constants.ByteCodes.Entities.InvisibleWall => new InvisibleWallTile(coordinate),
                Constants.ByteCodes.Entities.BlueWall => new BlueWallTile(coordinate, true),
                Constants.ByteCodes.Entities.FakeBlueWall => new BlueWallTile(coordinate, false),
                Constants.ByteCodes.Entities.Dirt => new DirtTile(coordinate),
                Constants.ByteCodes.Entities.Bug => new BugMonster(coordinate),
                Constants.ByteCodes.Entities.Paramecium => new ParameciumMonster(coordinate),
                Constants.ByteCodes.Entities.Ball => new BallMonster(coordinate),
                Constants.ByteCodes.Entities.Blob => new BlobMonster(coordinate),
                Constants.ByteCodes.Entities.Teeth => new TeethMonster(coordinate),
                Constants.ByteCodes.Entities.Fireball => new FireballMonster(coordinate),
                Constants.ByteCodes.Entities.RedButton => new RedButtonItem(coordinate),
                Constants.ByteCodes.Entities.BrownButton => new BrownButtonItem(coordinate),
                Constants.ByteCodes.Entities.IceSkates => new IceSkatesItem(coordinate),
                Constants.ByteCodes.Entities.SuctionBoots => new SuctionBootsItem(coordinate),
                Constants.ByteCodes.Entities.FireBoots => new FireBootsItem(coordinate),
                Constants.ByteCodes.Entities.Flippers => new FlippersItem(coordinate),
                Constants.ByteCodes.Entities.ToolThief => new ToolThief(coordinate),
                Constants.ByteCodes.Entities.RedBomb => new RedBombMonster(coordinate),
                Constants.ByteCodes.Entities.Trap
                    or Constants.ByteCodes.Entities.OpenTrap => new TrapItem(coordinate),
                Constants.ByteCodes.Entities.CloneMachine_V1
                    or Constants.ByteCodes.Entities.CloneMachine => new CloneMachineTile(coordinate),
                Constants.ByteCodes.Entities.Hint => new HintTile(coordinate),
                Constants.ByteCodes.Entities.ThinWallOrCanopy => new ThinWallOrCanopyTile(coordinate, 0),
                _ => new DebugEntity(coordinate, code, $"unknown_{code}")
            };
            if(result is DebugEntity)
            {
                GD.Print($"WARNING: Entity code 0x{code:X2} not recognized.");
            }
            if(result.DataCode != code)
            {
                GD.Print($"WARNING: {result.Name} data code {result.DataCode} does not match expected code of 0x{code:X2}");
            }
            return result;
        }
    }
}
