using GalaxyGauntlet.scripts;
using System;
using System.Threading.Tasks;

public partial class LevelSelect : Node2D
{
	private GridContainer GridContainer;

	private UIButton ReloadButton;

	private UIButton BackButton;

	private DialogWindow DialogWindow;

	private Button ButtonRef;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try
		{
			Node Control = GetNode(nameof(Control));
			Node ScrollContainer = Control.GetNode(nameof(ScrollContainer));
			GridContainer = (GridContainer)ScrollContainer.GetNode(nameof(GridContainer));
			GridContainer.Columns = (GameData.GameMode == GameMode.QuickPlay)
				? 1 : 3;
			ButtonRef = (Button)GetNode(nameof(ButtonRef));

			ReloadButton = (UIButton)GetNode(nameof(ReloadButton));
			ReloadButton.Pressed += BuildButtonList;
			
			BackButton = (UIButton)GetNode(nameof(BackButton));
			BackButton.Pressed += async () => await ToTitleScreen();

			DialogWindow = (DialogWindow)GetNode(nameof(DialogWindow));
			DialogWindow.Visible = false;

			BuildButtonList();
		}
		catch(Exception e)
		{
			FileLogger.LogException("Error readying Level Select screen", e);
		}
	}


	private void BuildButtonList()
	{
		foreach(var child in GridContainer.GetChildren())
		{
			GridContainer.RemoveChild(child);
			child.QueueFree();
		}

		string[] levelFiles = Global.GetLevelList();
		int levelIndex = 1;
		foreach(string file in levelFiles)
		{
			int tempLevel = levelIndex;
			levelIndex++;
			string levelName = LevelLoader.GetLevelName(file);
			Button button = (Button)ButtonRef.Duplicate();
			if(GameData.GameMode == GameMode.QuickPlay)
			{
				button.Text = levelName;
				button.Pressed += () => OnLevelSelect(tempLevel);
				GridContainer.AddChild(button);
				continue;
			}

			#region Archipelago logic

			if(false == GameData.PlayerHasAPItem(levelName))
			{
				continue;
			}

			button.Text = levelName;
			button.Pressed += () => OnLevelSelect(tempLevel);
			GridContainer.AddChild(button);

			// Complete label
			Label levelComplete = new()
			{
				Text = "        "
			};
			if (GameData.IsAPLocationChecked(levelName))
			{
				levelComplete.Text = "Complete";
			}
			GridContainer.AddChild(levelComplete);

			// No Resets label
			Label levelFirstTryComplete = new()
			{
				Text = "        "
			};
			if(GameData.IsAPLocationChecked($"{levelName} (No Resets)"))
			{
				levelFirstTryComplete.Text = " (No Resets)";
			}
			GridContainer.AddChild(levelFirstTryComplete);

			#endregion
		}
	}


	private void OnLevelSelect(int levelIndex)
	{
		GameData.CurrentLevelNumber = levelIndex;
		GameData.LevelRestarts = 0;
		GameData.ResetLevelData();
		string gameplayScreenPath = "res://screens/GameplayScreen/GameplayScreen.tscn";
		GetTree().ChangeSceneToFile(gameplayScreenPath);
	}


	private async Task ToTitleScreen()
	{
		if(GameData.GameMode == GameMode.QuickPlay)
		{
			string titleMenuPath = "res://screens/TitleMenu/TitleMenu.tscn";
			GetTree().ChangeSceneToFile(titleMenuPath);
			return;
		}

		// Archipelago logic
		DialogWindow.ClearAllButtonEvents();
		DialogWindow.ShowDialog("Are you sure you want to disconnect from the Archipelago server?", DialogButtonType.YesNo);
		DialogWindow.YesButtonPressed += async () =>
		{
			await GameData.DisconnectFromArchipelago();
			ToTitleMenu();
		};
		DialogWindow.NoButtonPressed += () => { DialogWindow.Visible = false; };
	}


	private void ToTitleMenu()
	{
		string titleMenuPath = "res://screens/TitleMenu/TitleMenu.tscn";
		GetTree().ChangeSceneToFile(titleMenuPath);
	}
}
