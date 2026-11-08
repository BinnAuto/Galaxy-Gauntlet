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
                GGConstants.ByteCodes.Entities.Floor => new NoTile(coordinate),
                GGConstants.ByteCodes.Entities.Wall => new WallTile(coordinate),
                GGConstants.ByteCodes.Entities.IceTile => new MapIceTile(coordinate, IceTileType.Standard),
                GGConstants.ByteCodes.Entities.Ice_SW => new MapIceTile(coordinate, IceTileType.Barrier_SW),
                GGConstants.ByteCodes.Entities.Ice_NW => new MapIceTile(coordinate, IceTileType.Barrier_NW),
                GGConstants.ByteCodes.Entities.Ice_NE => new MapIceTile(coordinate, IceTileType.Barrier_NE),
                GGConstants.ByteCodes.Entities.Ice_SE => new MapIceTile(coordinate, IceTileType.Barrier_SE),
                GGConstants.ByteCodes.Entities.Water => new WaterTile(coordinate),
                GGConstants.ByteCodes.Entities.Fire => new FireTile(coordinate),
                GGConstants.ByteCodes.Entities.ForceFloor_N => new ForceFloorNTile(coordinate),
                GGConstants.ByteCodes.Entities.ForceFloor_E => new ForceFloorETile(coordinate),
                GGConstants.ByteCodes.Entities.ForceFloor_S => new ForceFloorSTile(coordinate),
                GGConstants.ByteCodes.Entities.ForceFloor_W => new ForceFloorWTile(coordinate),
                GGConstants.ByteCodes.Entities.GreenToggleWall => new GreenToggleTile(coordinate, true),
                GGConstants.ByteCodes.Entities.GreenToggleFloor => new GreenToggleTile(coordinate, false),
                GGConstants.ByteCodes.Entities.RedTeleporter => new RedTeleporterItem(coordinate),
                GGConstants.ByteCodes.Entities.BlueTeleporter => new BlueTeleporterItem(coordinate),
                GGConstants.ByteCodes.Entities.YellowTeleporter => new YellowTeleporterItem(coordinate),
                GGConstants.ByteCodes.Entities.GreenTeleporter => new GreenTeleporterItem(coordinate),
                GGConstants.ByteCodes.Entities.Exit => new ExitTile(coordinate),
                GGConstants.ByteCodes.Entities.Slime => new SlimeTile(coordinate),
                GGConstants.ByteCodes.Entities.Player => new Player(coordinate),
                GGConstants.ByteCodes.Entities.DirtBlock => new DirtBlockEntity(coordinate),
                GGConstants.ByteCodes.Entities.Walker => new WalkerMonster(coordinate),
                GGConstants.ByteCodes.Entities.Glider => new GliderMonster(coordinate),
                GGConstants.ByteCodes.Entities.IceBlock => new IceBlockEntity(coordinate),
                GGConstants.ByteCodes.Entities.ThinWall_S => new ThinWallOrCanopyTile(coordinate, GGConstants.ByteCodes.ThinWallBits.South),
                GGConstants.ByteCodes.Entities.ThinWall_E => new ThinWallOrCanopyTile(coordinate, GGConstants.ByteCodes.ThinWallBits.East),
                GGConstants.ByteCodes.Entities.ThinWall_SE => new ThinWallOrCanopyTile(coordinate, GGConstants.ByteCodes.ThinWallBits.South & GGConstants.ByteCodes.ThinWallBits.East),
                GGConstants.ByteCodes.Entities.Gravel => new GravelTile(coordinate),
                GGConstants.ByteCodes.Entities.GreenButton => new GreenButtonItem(coordinate),
                GGConstants.ByteCodes.Entities.BlueButton => new BlueButtonItem(coordinate),
                GGConstants.ByteCodes.Entities.BlueTank => new BlueTankMonster(coordinate),
                GGConstants.ByteCodes.Entities.RedDoor => new RedDoorItem(coordinate),
                GGConstants.ByteCodes.Entities.BlueDoor => new BlueDoorItem(coordinate),
                GGConstants.ByteCodes.Entities.YellowDoor => new YellowDoorItem(coordinate),
                GGConstants.ByteCodes.Entities.GreenDoor => new GreenDoorItem(coordinate),
                GGConstants.ByteCodes.Entities.RedKey => new RedKeyItem(coordinate),
                GGConstants.ByteCodes.Entities.BlueKey => new BlueKeyItem(coordinate),
                GGConstants.ByteCodes.Entities.YellowKey => new YellowKeyItem(coordinate),
                GGConstants.ByteCodes.Entities.GreenKey => new GreenKeyItem(coordinate),
                GGConstants.ByteCodes.Entities.Chip
                    or GGConstants.ByteCodes.Entities.ExtraChip => new ChipItem(code, coordinate),
                GGConstants.ByteCodes.Entities.Socket => new SocketItem(coordinate),
                GGConstants.ByteCodes.Entities.RecessedWall => new RecessedWallItem(coordinate),
                GGConstants.ByteCodes.Entities.HiddenWall => new HiddenWallTile(coordinate),
                GGConstants.ByteCodes.Entities.InvisibleWall => new InvisibleWallTile(coordinate),
                GGConstants.ByteCodes.Entities.BlueWall => new BlueWallTile(coordinate, true),
                GGConstants.ByteCodes.Entities.FakeBlueWall => new BlueWallTile(coordinate, false),
                GGConstants.ByteCodes.Entities.Dirt => new DirtTile(coordinate),
                GGConstants.ByteCodes.Entities.Bug => new BugMonster(coordinate),
                GGConstants.ByteCodes.Entities.Paramecium => new ParameciumMonster(coordinate),
                GGConstants.ByteCodes.Entities.Ball => new BallMonster(coordinate),
                GGConstants.ByteCodes.Entities.Blob => new BlobMonster(coordinate),
                GGConstants.ByteCodes.Entities.Teeth => new TeethMonster(coordinate),
                GGConstants.ByteCodes.Entities.Fireball => new FireballMonster(coordinate),
                GGConstants.ByteCodes.Entities.RedButton => new RedButtonItem(coordinate),
                GGConstants.ByteCodes.Entities.BrownButton => new BrownButtonItem(coordinate),
                GGConstants.ByteCodes.Entities.IceSkates => new IceSkatesItem(coordinate),
                GGConstants.ByteCodes.Entities.SuctionBoots => new SuctionBootsItem(coordinate),
                GGConstants.ByteCodes.Entities.FireBoots => new FireBootsItem(coordinate),
                GGConstants.ByteCodes.Entities.Flippers => new FlippersItem(coordinate),
                GGConstants.ByteCodes.Entities.ToolThief => new ToolThief(coordinate),
                GGConstants.ByteCodes.Entities.RedBomb => new RedBombMonster(coordinate),
                GGConstants.ByteCodes.Entities.Trap
                    or GGConstants.ByteCodes.Entities.OpenTrap => new TrapItem(coordinate),
                GGConstants.ByteCodes.Entities.CloneMachine_V1
                    or GGConstants.ByteCodes.Entities.CloneMachine => new CloneMachineTile(coordinate),
                GGConstants.ByteCodes.Entities.HintPanel => new HintPanelItem(coordinate),
                GGConstants.ByteCodes.Entities.ForceFloor_R => new ForceFloorRTile(coordinate),
                GGConstants.ByteCodes.Entities.ThinWallOrCanopy => new ThinWallOrCanopyTile(coordinate, 0),
                _ => new DebugEntity(coordinate, code, $"unknown_{code}")
            };
            if(result is DebugEntity)
            {
                FileLogger.QuietLogMessage($"WARNING: Entity code 0x{code:X2} not recognized.");
            }
            if(result.DataCode != code)
            {
                FileLogger.QuietLogMessage($"WARNING: {result.Name} data code {result.DataCode} does not match expected code of 0x{code:X2}");
            }
            return result;
        }
    }
}
