using System;
using System.Linq;

namespace GalaxyGauntlet.scripts
{	
	/*
	 * Contains communication methods between different properties of GameData
	 */
    public static partial class GameData
	{
        public static GameMode GameMode = GameMode.Campaign;

		public static SpriteFrames SpriteFrames;

		public static SpriteFrames ArchipelagoSpriteFrames;

		public static Godot.Color? TileTransparentColor;
        
		public static void Initialize(Vector2I windowSize)
		{
			InitialWindowResolution = windowSize; 
			Archipelago = new();
			Archipelago.OnDeathMessage += OnSetDeathMessage;
			Archipelago.OnItemReceived += OnArchipelagoItemReceived;
			Map = new();
			Level = new();
			LoadSettings();
			LoadSpriteFrames();
		}


		#region Settings

		// Audio Settings
		public static double MasterVolume = 50;

		public static double MusicVolume = 50;

		public static double WindowMagnifier = 1; 
		
		private const string GameConfigFilePath = "user://gamedata.config";

		private const string GameDataConfigSection = "GameData";

		private static ConfigFile ConfigFile = new();

		public static Vector2I? InitialWindowResolution;

		public static void LoadSettings()
		{
			var loadResult = ConfigFile.Load(GameConfigFilePath);
			if (loadResult == Error.Ok)
			{
				Archipelago.GameName = (string)ConfigFile.GetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoGameName, "Chip's Challenge");
				Archipelago.Server = (string)ConfigFile.GetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoServer, "archipelago.gg");
				Archipelago.Port = (int)ConfigFile.GetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoPort, string.Empty);
				Archipelago.SlotName = (string)ConfigFile.GetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoSlotName, string.Empty);
				Archipelago.Password = (string)ConfigFile.GetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoPassword, string.Empty);
				MasterVolume = (double)ConfigFile.GetValue(GameDataConfigSection, GGConstants.Settings.MasterVolume, 50);
				MusicVolume = (double)ConfigFile.GetValue(GameDataConfigSection, GGConstants.Settings.MusicVolume, 50);
				WindowMagnifier = (int)ConfigFile.GetValue(GameDataConfigSection, GGConstants.Settings.WindowMagnifier, 1);
			}
			SaveSettings();
		}


		public static void SaveSettings()
		{
			ConfigFile.SetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoGameName, Archipelago.GameName);
			ConfigFile.SetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoServer, Archipelago.Server);
			ConfigFile.SetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoPort, Archipelago.Port.ToString());
			ConfigFile.SetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoSlotName, Archipelago.SlotName);
			ConfigFile.SetValue(GameDataConfigSection, GGConstants.Settings.ArchipelagoPassword, Archipelago.Password);
			ConfigFile.SetValue(GameDataConfigSection, GGConstants.Settings.MasterVolume, MasterVolume);
			ConfigFile.SetValue(GameDataConfigSection, GGConstants.Settings.MusicVolume, MusicVolume);
			ConfigFile.SetValue(GameDataConfigSection, GGConstants.Settings.WindowMagnifier, WindowMagnifier);
			ConfigFile.Save(GameConfigFilePath);

			Common.Global.SetBusVolume(GGConstants.AudioBuses.Master, MasterVolume);
			Common.Global.SetBusVolume(GGConstants.AudioBuses.Music, MusicVolume);
		}

        #endregion


        #region Sprite Frame Builders

        private static void LoadSpriteFrames()
        {
            SpriteFrames spriteFrames = (SpriteFrames)GD.Load("res://sprite_frames/tile_sprites.tres");
            string customSpritePath = "./custom/images/sprites.png";
            if (File.Exists(customSpritePath))
            {
                try
                {
					Vector2I minDimensions = new(512, 1024);
                    var image = Image.LoadFromFile(customSpritePath);
                    TileTransparentColor = image.GetPixel(0, 0); // TODO: Use shader to convert nontransparent color to transparent.
					if(image.GetWidth() < minDimensions.X
						|| image.GetHeight() < minDimensions.Y)
					{
						throw new($"The custom sprite sheet is too small. It must be at least {minDimensions.X} pixels wide and {minDimensions.Y} pixels high");
					}

                    var spriteSheet = ImageTexture.CreateFromImage(image);					
                    spriteFrames = new();

                    #region Build default tile animation frames

                    if (false == spriteFrames.HasAnimation("default"))
					{
						spriteFrames.AddAnimation("default");
					}
                    spriteFrames.SetAnimationSpeed("default", 0);
                    spriteFrames.SetAnimationLoop("default", false);
                    for (int y = 0; y < 32; y++)
                    {
                        for (int x = 0; x < 16; x++)
                        {
                            AtlasTexture atlas = new()
                            {
                                Atlas = spriteSheet,
                                Region = new(32 * x, 32 * y, 32, 32)
                            };
                            spriteFrames.AddFrame("default", atlas);
                        }
                    }

					#endregion

					#region Build Thin Wall animation frames

					if(false == spriteFrames.HasAnimation("thin_wall"))
					{
						spriteFrames.AddAnimation("thin_wall");
                    }
                    spriteFrames.SetAnimationSpeed("thin_wall", 0);
                    spriteFrames.SetAnimationLoop("thin_wall", false);
					Vector2I[] wallCoordinates = [
						new(32, 320),
						new(48, 320),
						new(80, 320),
						new(80, 336),
						new(32, 336),
						new(48, 336),
						new(64, 320),
						new(64, 336)
					];
					foreach(var coordinate in wallCoordinates)
                    {
                        AtlasTexture atlas = new()
                        {
                            Atlas = spriteSheet,
                            Region = new(coordinate.X, coordinate.Y, 16, 16)
                        };
                        spriteFrames.AddFrame("thin_wall", atlas);
                    }

					#endregion

					#region Build Letter animation frames

					if(false == spriteFrames.HasAnimation("letters"))
					{
						spriteFrames.AddAnimation("letters");
                    }
                    spriteFrames.SetAnimationSpeed("letters", 0);
                    spriteFrames.SetAnimationLoop("letters", false);
                    for (int y = 0; y < 32; y++)
                    {
                        for (int x = 0; x < 2; x++)
                        {
                            AtlasTexture atlas = new()
                            {
                                Atlas = spriteSheet,
                                Region = new(16 * x, 16 * y, 16, 16)
                            };
                            spriteFrames.AddFrame("letters", atlas);
                        }
                    }

                    #endregion
                }
                catch (Exception e)
                {
                    FileLogger.LogException("Failed to load custom sprite sheet", e);
					TileTransparentColor = null;
                    spriteFrames = (SpriteFrames)GD.Load("res://sprite_frames/tile_sprites.tres");
                }
            }
            SpriteFrames = spriteFrames;

			SpriteFrames archipelagoFrames = (SpriteFrames)GD.Load("res://sprite_frames/archipelago_tile_sprites.tres");
			string archipelagoSpritePath = "./custom/images/archipelago.png";
			if(File.Exists(archipelagoSpritePath))
			{
				try
                {
                    Vector2I minDimensions = new(64, 64);
                    var image = Image.LoadFromFile(archipelagoSpritePath);
                    if (image.GetWidth() < minDimensions.X
                        || image.GetHeight() < minDimensions.Y)
                    {
                        throw new($"The custom sprite sheet is too small. It must be at least {minDimensions.X} pixels wide and {minDimensions.Y} pixels high");
                    }

                    var spriteSheet = ImageTexture.CreateFromImage(image);
                    archipelagoFrames = new();

					if(false == archipelagoFrames.HasAnimation("default"))
					{
						archipelagoFrames.AddAnimation("default");
					}
					archipelagoFrames.SetAnimationSpeed("default", 0);
					archipelagoFrames.SetAnimationLoop("default", false);
					for(int y = 0; y < 2; y++)
					{
						for(int x = 0; x < 2; x++)
						{
                            AtlasTexture atlas = new()
                            {
                                Atlas = spriteSheet,
                                Region = new(32 * x, 32 * y, 32, 32)
                            };
                            archipelagoFrames.AddFrame("default", atlas);
                        }
					}
                }
				catch(Exception e)
                {
                    FileLogger.LogException("Failed to load custom archipelago sprite sheet", e);
                    TileTransparentColor = null;
                    archipelagoFrames = (SpriteFrames)GD.Load("res://sprite_frames/archipelago_tile_sprites.tres");
                }
            }
			ArchipelagoSpriteFrames = archipelagoFrames;
        }

		#endregion


		#region Button Press Methods

		// TurnTanks
		public static void BlueButtonPressed(Vector2I sourceCoordinate)
		{
			for (int x = 0; x < Map.Dimensions.X; x++)
			{
				for (int y = 0; y < Map.Dimensions.Y; y++)
				{
					var currentTile = Map.GetTile2(new(x, y));
					if (Map.Creatures2[x, y] is BlueTankCreature2 tank)
					{
                        if (currentTile is CloneMachineTile2 || currentTile is IceTile2)
                        {
                            continue;
                        }
                        tank.AboutFace();
					}
					if (Map.LynxCreatures2[x, y] is LynxBlueTankCreature2 lynxTank)
					{
                        if (currentTile is CloneMachineTile2 || currentTile is IceTile2)
                        {
                            continue;
                        }
						lynxTank.SetCreatureState(CreatureStateFlags.Reverse);
					}
				}
			}
		}


		// ToggleWalls
		public static void GreenButtonPressed(Vector2I sourceCoordinate)
		{
			for(int x = 0; x < Map.Dimensions.X; x++)
			{
				for(int y = 0; y < Map.Dimensions.Y; y++)
				{
					if (Map.Tiles2[x, y] is GreenToggleTile2 toggle)
					{
						toggle.Toggle();
					}
				}
			}
		}


		// ActivateCloner
		public static void RedButtonPressed(Vector2I sourceCoordinate)
		{
			int x = sourceCoordinate.X;
			int y = sourceCoordinate.Y;
			while(true)
			{
				Vector2I mapCoordinate = new(x, y);
				var mapTile = Map.GetTile2(mapCoordinate);
				if(mapTile is CloneMachineTile2)
				{
					Map.CloneCreature(mapCoordinate);
					return;
				}

				// Move to next coordinate
				x++;
				if (x >= Map.Dimensions.X)
				{
					x = 0;
					y++;
				}
				if (y >= Map.Dimensions.Y)
				{
					x = 0;
					y = 0;
				}
				if(sourceCoordinate == new Vector2I(x, y))
				{
					FileLogger.QuietLogMessage("Fiddlesticks! This level has red buttons but no cloning machines!");
					return;
				}
			}
		}


		// SpringTrap
		public static void BrownButtonPressed(Vector2I sourceCoordinate)
        {
            int x = sourceCoordinate.X;
            int y = sourceCoordinate.Y;
			while(true)
			{
				Vector2I mapCoordinate = new(x, y);
				var mapTile = Map.GetTile2(mapCoordinate);
				if(mapTile is TrapTile2 trapTile)
				{
					trapTile.OpenTrap();
					return;
                }

                // Move to next coordinate
                x++;
                if (x >= Map.Dimensions.X)
                {
                    x = 0;
                    y++;
                }
                if (y >= Map.Dimensions.Y)
                {
                    x = 0;
                    y = 0;
                }
                if (sourceCoordinate == new Vector2I(x, y))
                {
                    FileLogger.QuietLogMessage("Rats! This level has brown buttons but no traps!");
                    return;
                }
            }
        }


		public static void SpringTrap(Vector2I sourceCoordinate)
		{
			int x = sourceCoordinate.X;
			int y = sourceCoordinate.Y;
			while(true)
            {
                Vector2I mapCoordinate = new(x, y);
                var mapTile = Map.GetTile2(mapCoordinate);
                if (mapTile is TrapTile2 trapTile)
                {
					bool trapWasActive = trapTile.IsActive;
					trapTile.OpenTrap();
					var trappedCreature = Map.GetLynxCreature(mapCoordinate);
					trappedCreature?.AdvanceCreature(trapWasActive);
                    return;
                }

                // Move to next coordinate
                x++;
                if (x >= Map.Dimensions.X)
                {
                    x = 0;
                    y++;
                }
                if (y >= Map.Dimensions.Y)
                {
                    x = 0;
                    y = 0;
                }
                if (sourceCoordinate == new Vector2I(x, y))
                {
                    FileLogger.QuietLogMessage("Rats! This level has brown buttons but no traps!");
                    return;
                }
            }
		}

		#endregion


		#region Global Methods

		public static void LoadCurrentLevel()
		{
			Map.Reset();
			Level.Reset();
			string mapPath = Level.LevelSet[Level.CurrentLevelIndex];
			LevelLoader.LoadLevel(mapPath);
			Map.Rerender = true;
		}


		private static void OnSetDeathMessage(string message)
		{
			Level.DeathMessage = message;
		}


		public static void OnLevelComplete()
		{
			Level.ProcessMobs = false;
			Level.AcceptingPlayerInput = false;
			Level.TimerEnabled = false;
			if (GameMode == GameMode.Archipelago)
			{
				string location = $"Complete Map {Level.CurrentLevelIndex + 1} - {Level.Name}";
				Archipelago.SendLocationCheck(location);
				if (Level.RestartCount == 0)
				{
					location += " (No Resets)";
					Archipelago.SendLocationCheck(location);
				}
			}
		}


		public static void OnChipCollected(ChipItem2 chip)
		{
			Level.ChipsCollected++;
			if (GameMode is GameMode.Archipelago)
			{
				string locationName = chip.GetArchipelagoLocationName();
				Archipelago.SendLocationCheck(locationName);
			}
			Map.RemoveItem2(chip.Coordinate);
		}


		public static void OnPlayerDeath()
		{
			Level.ProcessMobs = false;
			Level.AcceptingPlayerInput = false;
			Level.TimerEnabled = false;
			Level.PlayerAlive = false;
			Archipelago.SendDeathLinkMessage();
		}


		public static bool PlayerHasPendingItem(int entityID)
		{
			return Archipelago.PendingItems.Any(e => e.EntityID == entityID);
		}


		public static int PendingItemCount(int entityID)
		{
			return Archipelago.PendingItems.Count(e => e.EntityID == entityID);
		}


		private static void OnArchipelagoItemReceived(string itemName)
		{
			switch (itemName)
			{
				case GGConstants.Archipelago.TrapNames.ConfusionTrap:
					RandomNumberGenerator rng = new();
					Level.ConfusionTimer = rng.RandiRange(20, 40);
					break;

				case GGConstants.Archipelago.ItemNames.TimeBonus:
					if(Map.TimeLimit == 0)
					{
						if(false == Archipelago.EnableCruelTimeItems)
						{
							break;
						}
						// Turns untimed levels into timed levels (hurry up!!!)
					}

					Map.TimeLimit = 10;
					Level.Timer += 10;
					break;

				case GGConstants.Archipelago.TrapNames.TimePenalty:
					if(Map.TimeLimit == 0)
					{
						if(false == Archipelago.EnableCruelTimeItems)
						{
							break;
						}

						// Instant death on untimed levels
						Level.DeathMessage = GGConstants.DeathMessages.OutOfTime;
						break;
					}

					Level.Timer -= 10;
					break;

				case GGConstants.Archipelago.TrapNames.WalkerTrap:
					Map.AddCreatureAtRandom(new LynxWalkerCreature2(Vector2I.Zero));
					break;

				case GGConstants.Archipelago.TrapNames.BlobTrap:
					Map.AddCreatureAtRandom(new LynxBlobCreature2(Vector2I.Zero));
					break;

				case GGConstants.Archipelago.TrapNames.TeethTrap:
					Map.AddCreatureAtRandom(new LynxTeethCreature2(Vector2I.Zero));
					break;

				case GGConstants.Archipelago.ItemNames.BigMap:
					Level.MapViewportSize = Mathf.Min(15, Level.MapViewportSize + 6);
					break;

				case GGConstants.Archipelago.TrapNames.SmallMapTrap:
					Level.MapViewportSize = Mathf.Max(5, Level.MapViewportSize - 4);
					break;

				default:
					// Item is either a progression item or has already
					// been processed in the Archipelago subclass.
					break;
			}
		}

		public static Vector2I GetPlayerInput()
		{
			Vector2I input = Vector2I.Zero;
			if (Input.IsActionPressed("player_up"))
			{
				input = new(0, -1);
			}
			if (Input.IsActionPressed("player_down"))
			{
				input = new(0, 1);
			}
			if (Input.IsActionPressed("player_left"))
			{
				input = new(-1, 0);
			}
			if (Input.IsActionPressed("player_right"))
			{
				input = new(1, 0);
			}
			if(Level.PlayerIsConfused)
			{
				input *= -1;
			}
			return input;
		}

		#endregion
	}
}
