using Godot;

namespace GalaxyGauntlet.Common
{
    public static class Constants
    {
        public static string GameName = "Galaxy Gauntlet";

        public static string GameVersion = "0.0.5";

        public static class Archipelago
        {
            public static class SlotDataKeys
            {
                public const string PlayerSprite = "player_sprite";
                public const string LevelHash = "level_hash";
                public const string DeathLink = "death_link";
            }


            public static class ItemNames
            {
                // Useful items
                public const string SingleUseRedKey = "Single-Use Red Key";
                public const string SingleUseBlueKey = "Single-Use Blue Key";
                public const string SingleUseYellowKey = "Single-Use Yellow Key";
                public const string SingleUseGreenKey = "Single-Use Green Key";
                public const string SingleUseIceSkates = "Single-Use Ice Skates";
                public const string SingleUseFireBoots = "Single-Use Fire Boots";
                public const string SingleUseSuctionBoots = "Single-Use Suction Boots";
                public const string SingleUseFlippers = "Single-Use Flippers";
                public const string Helmet = "Helmet";
                public const string SecretEye = "Secret Eye";
                public const string TimeBonus = "Time Bonus";

                // Traps
                public const string TimePenalty = "Time Penalty";
                public const string TeethTrap = "Teeth";
                public const string WalkerTrap = "Walker";
                public const string ConfusionTrap = "Confusion Trap";

                public const string Filler = "Filler";
            }


            public static class PlayerSpriteIndexes
            {
                public const int Player = 0;
                public const int Teeth = 1;
                public const int BlueTank = 2;
                public const int Glider = 3;
                public const int Bug = 4;
                public const int Paramecium = 5;
                public const int Fireball = 6;
            }
        }

        public static class ByteCodes
        {
            public static class Entities
            {
                public const int Floor = 0x01;
                public const int Wall = 0x02;
                public const int IceTile = 0x03;
                public const int Ice_SW = 0x04;
                public const int Ice_NW = 0x05;
                public const int Ice_NE = 0x06;
                public const int Ice_SE = 0x07;
                public const int Water = 0x08;
                public const int Fire = 0x09;
                public const int ForceFloor_N = 0x0A;
                public const int ForceFloor_E = 0x0B;
                public const int ForceFloor_S = 0x0C;
                public const int ForceFloor_W = 0x0D;
                public const int GreenToggleWall = 0x0E;
                public const int GreenToggleFloor = 0x0F;
                public const int RedTeleporter = 0x10;
                public const int BlueTeleporter = 0x11;
                public const int YellowTeleporter = 0x12;
                public const int GreenTeleporter = 0x13;
                public const int Exit = 0x14;
                public const int Slime = 0x15;
                public const int Player = 0x16;
                public const int DirtBlock = 0x17;
                public const int Walker = 0x18;
                public const int Glider = 0x19;
                public const int IceBlock = 0x1A;
                public const int ThinWall_S = 0x1B;
                public const int ThinWall_E = 0x1C;
                public const int ThinWall_SE = 0x1D;
                public const int Gravel = 0x1E;
                public const int GreenButton = 0x1F;
                public const int BlueButton = 0x20;
                public const int BlueTank = 0x21;
                public const int RedDoor = 0x22;
                public const int BlueDoor = 0x23;
                public const int YellowDoor = 0x24;
                public const int GreenDoor = 0x25;
                public const int RedKey = 0x26;
                public const int BlueKey = 0x27;
                public const int YellowKey = 0x28;
                public const int GreenKey = 0x29;
                public const int Chip = 0x2A;
                public const int ExtraChip = 0x2B;
                public const int Socket = 0x2C;
                public const int RecessedWall = 0x2D;
                public const int HiddenWall = 0x2E;
                public const int InvisibleWall = 0x2F;
                public const int BlueWall = 0x30;
                public const int FakeBlueWall = 0x31;
                public const int Dirt = 0x32;
                public const int Bug = 0x33;
                public const int Paramecium = 0x34;
                public const int Ball = 0x35;
                public const int Blob = 0x36;
                public const int Teeth = 0x37;
                public const int Fireball = 0x38;
                public const int RedButton = 0x39;
                public const int BrownButton = 0x3A;
                public const int IceSkates = 0x3B;
                public const int SuctionBoots = 0x3C;
                public const int FireBoots = 0x3D;
                public const int Flippers = 0x3E;
                public const int ToolThief = 0x3F;
                public const int RedBomb = 0x40;
                public const int OpenTrap = 0x41;
                public const int Trap = 0x42;
                public const int CloneMachine_V1 = 0x43;
                public const int CloneMachine = 0x44;
                public const int HintPanel = 0x45;
                public const int ForceFloor_R = 0x46;
                public const int GrayButton = 0x47;
                public const int SwivelDoor_SW = 0x48;
                public const int SwivelDoor_NW = 0x49;
                public const int SwivelDoor_NE = 0x4A;
                public const int SwivelDoor_SE = 0x4B;
                public const int TimeBonus = 0x4C;
                public const int Stopwatch = 0x4D;
                public const int Transmogrifier = 0x4E;
                public const int RailroadTrack = 0x4F;
                public const int SteelWall = 0x50;
                public const int TimeBomb = 0x51;
                public const int Helmet = 0x52;
                public const int Melinda = 0x56;
                public const int TimidTeeth = 0x57;
                public const int Explosion = 0x58;
                public const int HikingBoots = 0x59;
                public const int MaleOnlySign = 0x5A;
                public const int FemaleOnlySign = 0x5B;
                public const int LogicGate = 0x5C;
                public const int PinkButton = 0x5E;
                public const int FlameJet_Off = 0x5F;
                public const int FlameJet_On = 0x60;
                public const int OrangeButton = 0x61;
                public const int LightningBolt = 0x62;
                public const int YellowTank = 0x63;
                public const int YellowTankButton = 0x64;
                public const int MirrorPlayer = 0x65;
                public const int MirrorMelinda = 0x66;
                public const int BowlingBall = 0x68;
                public const int Rover = 0x69;
                public const int TimePenalty = 0x6A;
                public const int CustomFloor = 0x6B;
                public const int ThinWallOrCanopy = 0x6D;
                public const int RailroadSign = 0x6F;
                public const int CustomWall = 0x70;
                public const int LetterTile = 0x71;
                public const int PurpleToggleFloor = 0x72;
                public const int PurpleToggleWall = 0x73;
                public const int PointFlag10 = 0x7A;
                public const int PointFlag100 = 0x7B;
                public const int PointFlag1000 = 0x7C;
                public const int GreenWall = 0x7D;
                public const int FakeGreenWall = 0x7E;
                public const int NotAllowedSign = 0x7F;
                public const int DoublePointFlag = 0x80;
                public const int DirecitonalBlock = 0x81;
                public const int FloorMimic = 0x82;
                public const int GreenBomb = 0x83;
                public const int GreenChip = 0x84;
                public const int BlackButton = 0x87;
                public const int OnOffSwitch_Off = 0x88;
                public const int OnOffSwitch_On = 0x89;
                public const int KeyThief = 0x8A;
                public const int Ghost = 0x8B;
                public const int SteelFoil = 0x8C;
                public const int Turtle = 0x8D;
                public const int SecretEye = 0x8E;
                public const int ThiefBribe = 0x8F;
                public const int SpeedBoots = 0x90;
                public const int Hook = 0x92;
            }


            public static class OrientationBytes
            {
                public const int North = 0x00;
                public const int East = 0x01;
                public const int South = 0x02;
                public const int West = 0x03;
            }


            public static class ThinWallBits
            {
                public const int North = 0x01;
                public const int East = 0x02;
                public const int South = 0x04;
                public const int West = 0x08;
                public const int Canopy = 0x10;
            }
        }

        public static class SpriteCoordinates
        {
            public static Vector2I NoSprite = new(0, 0);
            public static Vector2I Floor = new(0, 0);
            public static Vector2I Wall = new(1, 0);
            public static Vector2I IceTile = new(2, 0);
            public static Vector2I Ice_SW = new(3, 0);
            public static Vector2I Ice_NW = new(4, 0);
            public static Vector2I Ice_NE = new(5, 0);
            public static Vector2I Ice_SE = new(0, 1);
            public static Vector2I Water = new(1, 1);
            public static Vector2I Fire = new(2, 1);
            public static Vector2I ForceFloor_N = new(3, 1);
            public static Vector2I ForceFloor_E = new(4, 1);
            public static Vector2I ForceFloor_S = new(5, 1);
            public static Vector2I ForceFloor_W = new(0, 2);
            public static Vector2I GreenToggleFloor = new(1, 2);
            public static Vector2I GreenToggleWall = new(2, 2);
            public static Vector2I RedTeleporter = new(3, 2);
            public static Vector2I BlueTeleporter = new(4, 2);
            public static Vector2I YellowTeleporter = new(5, 2);
            public static Vector2I GreenTeleporter = new(0, 3);
            public static Vector2I Exit = new(1, 3);
            public static Vector2I Slime = new(2, 3);
            public static Vector2I Player_N = new(3, 3);
            public static Vector2I Player_E = new(4, 3);
            public static Vector2I Player_S = new(5, 3);
            public static Vector2I Player_W = new(0, 4);
            public static Vector2I DirtBlock = new(1, 4);
            public static Vector2I Walker_N = new(2, 4);
            public static Vector2I Walker_E = new(3, 4);
            public static Vector2I Walker_S = new(4, 4);
            public static Vector2I Walker_W = new(5, 4);
            public static Vector2I Glider_N = new(0, 5);
            public static Vector2I Glider_E = new(1, 5);
            public static Vector2I Glider_S = new(2, 5);
            public static Vector2I Glider_W = new(3, 5);
            public static Vector2I IceBlock = new(4, 5);
            public static Vector2I ThinWall_None = new(5, 5);
            public static Vector2I ThinWall_S = new(0, 6);
            public static Vector2I ThinWall_E = new(1, 6);
            public static Vector2I ThinWall_W = new(2, 6);
            public static Vector2I ThinWall_N = new(3, 6);
            public static Vector2I ThinWall_SE = new(4, 6);
            public static Vector2I ThinWall_NE = new(5, 6);
            public static Vector2I ThinWall_NW = new(0, 7);
            public static Vector2I ThinWall_SW = new(1, 7);
            public static Vector2I ThinWall_EW = new(2, 7);
            public static Vector2I ThinWall_NS = new(3, 7);
            public static Vector2I ThinWall_ESW = new(4, 7);
            public static Vector2I ThinWall_NES = new(5, 7);
            public static Vector2I ThinWall_NEW = new(0, 8);
            public static Vector2I ThinWall_NSW = new(1, 8);
            public static Vector2I ThinWall_NESW = new(2, 8); // Wall Sprite
            public static Vector2I Gravel = new(3, 8);
            public static Vector2I GreenButton = new(4, 8);
            public static Vector2I BlueButton = new(5, 8);
            public static Vector2I BlueTank_N = new(0, 9);
            public static Vector2I BlueTank_E = new(1, 9);
            public static Vector2I BlueTank_S = new(2, 9);
            public static Vector2I BlueTank_W = new(3, 9);
            public static Vector2I RedDoor = new(4, 9);
            public static Vector2I BlueDoor = new(5, 9);
            public static Vector2I YellowDoor = new(0, 10);
            public static Vector2I GreenDoor = new(1, 10);
            public static Vector2I RedKey = new(2, 10);
            public static Vector2I LockedRedKey = new(3, 10);
            public static Vector2I BlueKey = new(4, 10);
            public static Vector2I LockedBlueKey = new(5, 10);
            public static Vector2I YellowKey = new(0, 11);
            public static Vector2I LockedYellowKey = new(1, 11);
            public static Vector2I GreenKey = new(2, 11);
            public static Vector2I LockedGreenKey = new(3, 11);
            public static Vector2I Chip = new(4, 11);
            public static Vector2I ProgressiveArchipelagoItem = new(5, 11);
            public static Vector2I ArchipelagoItem = new(0, 12);
            public static Vector2I ArchipelagoTrap = new(1, 12);
            public static Vector2I ArchipelagoFiller = new(2, 12);
            public static Vector2I Socket_Closed = new(3, 12);
            public static Vector2I RecessedWall = new(4, 12);
            public static Vector2I BlueWall = new(5, 12);
            public static Vector2I FakeBlueWall = new(0, 13);
            public static Vector2I Dirt = new(1, 13);
            public static Vector2I Bug_N = new(2, 13);
            public static Vector2I Bug_E = new(3, 13);
            public static Vector2I Bug_S = new(4, 13);
            public static Vector2I Bug_W = new(5, 13);
            public static Vector2I Paramecium_N = new(0, 14);
            public static Vector2I Paramecium_E = new(1, 14);
            public static Vector2I Paramecium_S = new(2, 14);
            public static Vector2I Paramecium_W = new(3, 14);
            public static Vector2I Ball = new(4, 14);
            public static Vector2I Blob = new(5, 14);
            public static Vector2I Teeth_N = new(0, 15);
            public static Vector2I Teeth_E = new(1, 15);
            public static Vector2I Teeth_S = new(2, 15);
            public static Vector2I Teeth_W = new(3, 15);
            public static Vector2I RedButton = new(4, 15);
            public static Vector2I BrownButton = new(5, 15);
            public static Vector2I Fireball_N = new(0, 16);
            public static Vector2I Fireball_E = new(1, 16);
            public static Vector2I Fireball_S = new(2, 16);
            public static Vector2I Fireball_W = new(3, 16);
            public static Vector2I FireBoots = new(4, 16);
            public static Vector2I LockedFireBoots = new(5, 16);
            public static Vector2I IceSkates = new(0, 17);
            public static Vector2I LockedIceSkates = new(1, 17);
            public static Vector2I SuctionBoots = new(2, 17);
            public static Vector2I LockedSuctionBoots = new(3, 17);
            public static Vector2I Flippers = new(4, 17);
            public static Vector2I LockedFlippers = new(5, 17);
            public static Vector2I ToolThief = new(0, 18);
            public static Vector2I RedBomb = new(1, 18);
            public static Vector2I InactiveTrap = new(2, 18);
            public static Vector2I ActiveTrap = new(3, 18);
            public static Vector2I CloneMachineTile = new(4, 18);
            public static Vector2I CloneMachineScenery = new(5, 18);
            public static Vector2I HintPanel = new(0, 19);
            public static Vector2I ForceFloor_R = new(1, 19);
            public static Vector2I GrayButton = new(2, 19);
            public static Vector2I Socket_Open = new(3, 19);
            public static Vector2I DirtBlock_XRay = new(4, 19);
            public static Vector2I IceBlock_XRay = new(5, 19);
        }

        public static class DeathMessages
        {
            public static string MovingBlocks = "Ooops! Watch out for moving blocks!";

            public static string OutOfTime = "Ooops! Out of time!";

            public static string Monsters = "Ooops! Look out for creatures!";

            public static string Bombs = "Ooops! Don't touch the bombs!";

            public static string Fire = "Ooops! Don't step in the fire without fire boots!";

            public static string Water = "Ooops! You can't swim without flippers!";
        }
    }
}
