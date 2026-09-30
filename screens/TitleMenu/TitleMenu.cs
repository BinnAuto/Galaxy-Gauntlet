using GalaxyGauntlet.scripts;
using GalaxyGauntlet.scripts.SaveData;
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

    #region Godot Overrides

    public override void _Ready()
	{
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

		ExitButton = (UIButton)GetNode(nameof(ExitButton));
		ExitButton.Pressed += OnExit;

		DialogWindow = (DialogWindow)GetNode(nameof(DialogWindow));

		Archipelago = (Node2D)GetNode(nameof(Archipelago));

		ArchipelagoConnectForm = (ArchipelagoConnectForm)Archipelago.GetNode(nameof(ArchipelagoConnectForm));
        ArchipelagoConnectForm.Connected += OnArchipelagoConnected;
		ArchipelagoConnectForm.Cancelled += () => { Archipelago.Visible = false; };

        Directory.CreateDirectory("./levels");
        SaveData.Load();
		ContinueButton.Visible = SaveData.SaveSlotExists(SaveData.AutosaveSlotName);
	}

	#endregion


	#region Event Handlers

	private void OnNewGame()
	{
		if(false == SaveData.SaveSlotExists(SaveData.AutosaveSlotName))
        {
            SaveSlot autosave = new(SaveData.AutosaveSlotName);
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
			int levelCount = Global.GetLevelList().Length;
			string levelHash = GetLevelSetHash();
			if(levelCount == 0 || string.IsNullOrEmpty(levelHash))
			{
				DialogWindow.ClearAllButtonEvents();
				DialogWindow.ShowDialog("No levels found in level folder", DialogButtonType.OK);
				DialogWindow.OKButtonPressed += DialogWindow.HideDialog;
				return;
			}

            SaveSlot autosave = new(SaveData.AutosaveSlotName)
            {
                LevelCount = levelCount,
                LevelHash = levelHash
            };
            SaveData.WriteSaveSlot(autosave);
			GameData.CurrentLevelNumber = 1;
			OnGoToGameplayScreen(GameMode.Campaign);
		};
	}


	private void OnContinue()
	{
		var saveSlot = SaveData.GetSaveSlot(SaveData.AutosaveSlotName);
		string levelHash = GetLevelSetHash();
		int levelCount = Global.GetLevelList().Length;
        if (levelCount == 0 || string.IsNullOrEmpty(levelHash))
        {
            DialogWindow.ClearAllButtonEvents();
            DialogWindow.ShowDialog("No levels found in level folder", DialogButtonType.OK);
            DialogWindow.OKButtonPressed += DialogWindow.HideDialog;
            return;
        }

        if (levelHash == saveSlot.LevelHash && levelCount == saveSlot.LevelCount)
		{
			GameData.CurrentLevelNumber = saveSlot.CurrentLevelNumber;
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
			GameData.CurrentLevelNumber = saveSlot.CurrentLevelNumber;
			OnGoToGameplayScreen(GameMode.Campaign);
		};
    }


	private void OnArchipelagoConnected()
	{
        Archipelago.Visible = false;
		string apLevelHash = GameData.GetAPSlotDataKey<string>(Constants.Archipelago.SlotDataKeys.LevelHash);
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
				await GameData.DisconnectFromArchipelago();
				DialogWindow.HideDialog();
			};
			return;
		}
		string localLevelHash = GetLevelSetHash();
        GD.Print($"Local level hash: {localLevelHash}");
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
                await GameData.DisconnectFromArchipelago();
                DialogWindow.HideDialog();
            };
            return;
        }

		ToLevelSelect(GameMode.Archipelago);
	}


	private void ToLevelSelect(GameMode gameMode)
	{
		GameData.GameMode = gameMode;
		string levelSelectScreenPath = "res://screens/LevelSelect/LevelSelect.tscn";
		GetTree().ChangeSceneToFile(levelSelectScreenPath);
	}


	private void OnExit()
	{
		GetTree().Quit(0);
	}


	private void OnGoToGameplayScreen(GameMode gameMode)
    {
		GameData.GameMode = gameMode;
        string gameplayScreenPath = "res://screens/GameplayScreen/GameplayScreen.tscn";
        GetTree().ChangeSceneToFile(gameplayScreenPath);
    }

	#endregion


	private string GetLevelSetHash()
	{
		string levelPath = "./levels";
		Directory.CreateDirectory(levelPath);
		levelPath = Path.GetFullPath(levelPath);
		string[] files = Directory.GetFiles("./levels", "*.c2m");
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
}
