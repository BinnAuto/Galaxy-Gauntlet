using GalaxyGauntlet.scripts;
using GalaxyGauntlet.scripts.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public partial class TitleMenu : Node
{
	private UIButton NewGameButton;

	private UIButton ContinueButton;

	private UIButton QuickPlayButton;

	private UIButton ArchipelagoButton;

	private UIButton SettingsButton;

	private UIButton ExitButton;

	private DialogWindow DialogWindow;

	private Node2D Archipelago;

	private ArchipelagoConnectForm ArchipelagoConnectForm;

	private Label Version;


	#region Godot Overrides

	public override void _Ready()
	{
		try
		{
			BuildLocalDirectories();
			GameData.Initialize();

			NewGameButton = (UIButton)GetNode(nameof(NewGameButton));
			NewGameButton.Pressed += OnNewGame;

			ContinueButton = (UIButton)GetNode(nameof(ContinueButton));
			ContinueButton.Pressed += OnContinue;
			ContinueButton.Visible = false;

			QuickPlayButton = (UIButton)GetNode(nameof(QuickPlayButton));
			QuickPlayButton.Pressed += () => ToLevelSelect(GameMode.QuickPlay);

			ArchipelagoButton = (UIButton)GetNode(nameof(ArchipelagoButton));
			ArchipelagoButton.Pressed += () => { Archipelago.Visible = true; };

			SettingsButton = (UIButton)GetNode(nameof(SettingsButton));
			SettingsButton.Pressed += OnSettingsButtonPressed;

			ExitButton = (UIButton)GetNode(nameof(ExitButton));
			ExitButton.Pressed += OnExit;

			DialogWindow = (DialogWindow)GetNode(nameof(DialogWindow));

			Archipelago = (Node2D)GetNode(nameof(Archipelago));

			ArchipelagoConnectForm = (ArchipelagoConnectForm)Archipelago.GetNode(nameof(ArchipelagoConnectForm));
			ArchipelagoConnectForm.Connected += OnArchipelagoConnected;
			ArchipelagoConnectForm.Cancelled += () => { Archipelago.Visible = false; };

			Version = (Label)GetNode(nameof(Version));
			Version.Text = $"v{Constants.GameVersion}";

			SaveData.Load();
			ContinueButton.Visible = SaveData.SaveSlotExists(Constants.SaveSlotNames.Autosave);
			FileLogger.QuietLogMessage("Title screen _Ready complete");
		}
		catch (Exception e)
		{
			GD.Print(e.Message);
			GD.Print(e.StackTrace);
			FileLogger.LogException("Error readying Title Menu screen", e);
		}
	}

	#endregion

	private static void BuildLocalDirectories()
	{
		Directory.CreateDirectory("./levels");
		Directory.CreateDirectory("./logs");
		Directory.CreateDirectory("./custom");
		Directory.CreateDirectory("./custom/music");
		Directory.CreateDirectory("./custom/music/menu");
		Directory.CreateDirectory("./custom/music/level_select");
		Directory.CreateDirectory("./custom/music/game");
	}


	#region Event Handlers

	private void OnNewGame()
	{
		if(false == SaveData.SaveSlotExists(Constants.SaveSlotNames.Autosave))
		{
			SaveSlot autosave = new(Constants.SaveSlotNames.Autosave);
			string levelHash = GetLevelSetHash();
			autosave.LevelHash = levelHash;
			SaveData.WriteSaveSlot(autosave);
			OnGoToGameplayScreen(GameMode.Campaign);
			return;
		}

		DialogWindow.ClearAllButtonEvents();
		DialogWindow.ShowDialog("Starting a new game will delete the existing save data. Do you want to continue?", "Autosave found", DialogButtonType.YesNo);
		DialogWindow.NoButtonPressed += DialogWindow.HideDialog;
		DialogWindow.YesButtonPressed += () =>
		{
			int levelCount = GGG.GetLevelList().Length;
			string levelHash = GetLevelSetHash();
			if(levelCount == 0 || string.IsNullOrEmpty(levelHash))
			{
				DialogWindow.ClearAllButtonEvents();
				DialogWindow.ShowDialog("No levels found in level folder", DialogButtonType.OK);
				DialogWindow.OKButtonPressed += DialogWindow.HideDialog;
				return;
			}

			SaveSlot autosave = new(Constants.SaveSlotNames.Autosave)
			{
				LevelCount = levelCount,
				LevelHash = levelHash
			};
			SaveData.WriteSaveSlot(autosave);
			GameData.CurrentLevelIndex = 0;
			OnGoToGameplayScreen(GameMode.Campaign);
		};
	}


	private void OnContinue()
	{
		var saveSlot = SaveData.GetSaveSlot(Constants.SaveSlotNames.Autosave);
		string levelHash = GetLevelSetHash();
		int levelCount = GGG.GetLevelList().Length;
		if (levelCount == 0 || string.IsNullOrEmpty(levelHash))
		{
			DialogWindow.ClearAllButtonEvents();
			DialogWindow.ShowDialog("No levels found in level folder", DialogButtonType.OK);
			DialogWindow.OKButtonPressed += DialogWindow.HideDialog;
			return;
		}

		if (levelHash == saveSlot.LevelHash && levelCount == saveSlot.LevelCount)
		{
			GameData.CurrentLevelIndex = saveSlot.CurrentLevelNumber;
			OnGoToGameplayScreen(GameMode.Campaign);
			return;
		}

		DialogWindow.ClearAllButtonEvents();
		DialogWindow.ShowDialog("It looks like the level set has changed since you last saved. Do you want to continue?", DialogButtonType.YesNo);
		DialogWindow.NoButtonPressed += DialogWindow.HideDialog;
		DialogWindow.YesButtonPressed += () =>
		{
			saveSlot.LevelCount = levelCount;
			saveSlot.LevelHash = levelHash;
			SaveData.WriteSaveSlot(saveSlot);
			GameData.CurrentLevelIndex = saveSlot.CurrentLevelNumber;
			OnGoToGameplayScreen(GameMode.Campaign);
		};
	}


	private void OnArchipelagoConnected()
	{
		Archipelago.Visible = false;
		string apLevelHash = GameData.Archipelago.GetSlotDataKey<string>(Constants.Archipelago.SlotDataKeys.LevelHash);
		FileLogger.QuietLogMessage($"APWorld level hash: {apLevelHash}");
		if(string.IsNullOrEmpty(apLevelHash))
		{
			DialogWindow.ClearAllButtonEvents();
			DialogWindow.ShowDialog("The current level set cannot be determined to match the given AP world.\nWARNING: This could cause incorrect items to be sent to the server.\nDo you want to continue?", DialogButtonType.YesNo);
			DialogWindow.YesButtonPressed += () =>
			{
				ToLevelSelect(GameMode.Archipelago);
				DialogWindow.HideDialog();
			};
			DialogWindow.NoButtonPressed += async () =>
			{
				await GameData.Archipelago.DisconnectFromServer();
				DialogWindow.HideDialog();
			};
			return;
		}
		string localLevelHash = GetLevelSetHash();
		FileLogger.QuietLogMessage($"Local level hash: {localLevelHash}");
		if (localLevelHash != apLevelHash)
		{
			DialogWindow.ClearAllButtonEvents();
			DialogWindow.ShowDialog("The current level set does not seem to match the set used to generate this AP world.\nWARNING: This could cause incorrect items to be sent to the server.\nDo you want to continue?", DialogButtonType.YesNo);
			DialogWindow.YesButtonPressed += () =>
			{
				ToLevelSelect(GameMode.Archipelago);
				DialogWindow.HideDialog();
			};
			DialogWindow.NoButtonPressed += async () =>
			{
				await GameData.Archipelago.DisconnectFromServer();
				DialogWindow.HideDialog();
			};
			return;
		}

		ToLevelSelect(GameMode.Archipelago);
	}


	private void ToLevelSelect(GameMode gameMode)
	{
		GameData.GameMode = gameMode;
		GetTree().ChangeSceneToFile(Constants.ScenePaths.LevelSelect);
	}


	private void OnExit()
	{
		GetTree().Quit(0);
	}


	private void OnGoToGameplayScreen(GameMode gameMode)
	{
		GameData.LevelSet = GGG.GetLevelList();
		GameData.GameMode = gameMode;
		GetTree().ChangeSceneToFile(Constants.ScenePaths.GameplayScreen);
	}


	private void OnSettingsButtonPressed()
	{
		GetTree().ChangeSceneToFile(Constants.ScenePaths.SettingsMenu);
	}

	#endregion


	private string GetLevelSetHash()
	{
		string levelPath = "./levels";
		Directory.CreateDirectory(levelPath);
		levelPath = Path.GetFullPath(levelPath);
		string[] files = GGG.GetLevelList();
		if(files.Length == 0)
		{
			DialogWindow.ClearAllButtonEvents();
			DialogWindow.ShowDialog("ERROR: No level files found in levels folder.", DialogButtonType.OK);
			DialogWindow.OKButtonPressed += DialogWindow.HideDialog;
			return null;
		}

		List<byte> combinedFileData = [];
		foreach(var file in files)
		{
			combinedFileData.AddRange(File.ReadAllBytes(file));
		}
		if (combinedFileData.Count == 0)
		{
			DialogWindow.ClearAllButtonEvents();
			DialogWindow.ShowDialog("ERROR: No level files found in levels folder.", DialogButtonType.OK);
			DialogWindow.OKButtonPressed += DialogWindow.HideDialog;
			return null;
		}

		SHA256 sha256 = SHA256.Create();
		var hash = SHA256.HashData([..combinedFileData]);
		return hash.Select(e => $"{e:x2}").ToArray().Join("");
	}


	private static bool ValidateLevelsForArchipelago()
	{
		string[] files = GGG.GetLevelList();
		int levelNumber = 1;
		bool isValid = true;
		List<byte> combinedFileData = [];
		foreach (string file in files)
		{
			if(GameData.Archipelago.LevelFilesToIgnore.Any(e => file.EndsWith(e, StringComparison.InvariantCultureIgnoreCase)))
			{
				continue;
			}

			combinedFileData.AddRange(File.ReadAllBytes(file));
			string levelName = LevelLoader.GetLevelName(file);
			string completeLevelLocation = $"Complete Map {levelNumber} - {levelName}";
			long locationId = GameData.Archipelago.GetLocationIdByName(completeLevelLocation);
			if(locationId == -1)
			{
				isValid = false;
			}
		}
		SHA256 sha256 = SHA256.Create();
		var hash = SHA256.HashData([.. combinedFileData]);
		string localLevelHash =  hash.Select(e => $"{e:x2}").ToArray().Join("");
		string apLevelHash = GameData.Archipelago.GetSlotDataKey<string>(Constants.Archipelago.SlotDataKeys.LevelHash);
		return isValid && (apLevelHash == localLevelHash);
	}
}
