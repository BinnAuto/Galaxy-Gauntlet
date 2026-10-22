using GalaxyGauntlet.scripts;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class LevelSelect : Node2D
{
	private GridContainer GridContainer;

	private UIButton ReloadButton;

	private UIButton BackButton;

	private DialogWindow DialogWindow;

	private Button ButtonRef;

	private Node2D ArchipelagoDisplay;

	private Label TotalScore;

	private Label LevelsCompleted;

	#region Godot Overrides

	public override void _Ready()
	{
		try
		{
			Node Control = GetNode(nameof(Control));
			ScrollContainer ScrollContainer = (ScrollContainer)Control.GetNode(nameof(ScrollContainer));
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

			ArchipelagoDisplay = (Node2D)GetNode(nameof(ArchipelagoDisplay));

			TotalScore = (Label)ArchipelagoDisplay.GetNode(nameof(TotalScore));

			LevelsCompleted = (Label)ArchipelagoDisplay.GetNode(nameof(LevelsCompleted));

			BuildButtonList();

			ArchipelagoDisplay.Visible = false;
			if (GameData.GameMode == GameMode.Archipelago)
			{
				ArchipelagoDisplay.Visible = true;
				TotalScore.Text = GameData.Archipelago.CurrentTotalScore.ToString();
				if(GameData.Archipelago.ScoreGoal != 0)
				{
					TotalScore.Text += $" ({GameData.Archipelago.ScoreGoal})";
				}
				LevelsCompleted.Text = GameData.Archipelago.CurrentLevelCompletedCount.ToString();
				if(GameData.Archipelago.LevelCompletionGoal!= 0)
				{
					LevelsCompleted.Text += $" ({GameData.Archipelago.LevelCompletionGoal})";
				}
			}
		}
		catch(Exception e)
		{
			FileLogger.LogException("Error readying Level Select screen", e);
		}
	}

	#endregion


	private void BuildButtonList()
	{
		foreach(var child in GridContainer.GetChildren())
		{
			GridContainer.RemoveChild(child);
			child.QueueFree();
		}

		GameData.LevelSet = GGG.GetLevelList();
		if(GameData.GameMode == GameMode.Archipelago)
		{
			string[] levelIgnores = GameData.Archipelago.GetSlotDataKey<string>(Constants.Archipelago.SlotDataKeys.IgnoreLevelFiles).Split('|');
			GameData.LevelSet = GameData.LevelSet.Where(e => false == levelIgnores.Any(f => e.EndsWith(f, StringComparison.InvariantCultureIgnoreCase))).ToArray();
		}
		GD.Print($"{GameData.LevelSet.Length} levels found");
		int levelIndex = 0;
		foreach(string file in GameData.LevelSet)
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

			if (GameData.GameMode == GameMode.Archipelago)
			{
				string mapName = $"Map {levelIndex} - {levelName}";
				string completeLocation = $"Complete {mapName}";
				string noResetsLocation = $"{completeLocation} (No Resets)";
				if (false == GameData.Archipelago.PlayerReceivedItem(mapName))
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
				if (GameData.Archipelago.HasLocationBeenChecked(completeLocation))
				{
					levelComplete.Text = "Complete";
				}
				GridContainer.AddChild(levelComplete);

				// No Resets label
				Label levelFirstTryComplete = new()
				{
					Text = "        "
				};
				if (GameData.Archipelago.HasLocationBeenChecked(noResetsLocation))
				{
					levelFirstTryComplete.Text = " (No Resets)";
				}
				GridContainer.AddChild(levelFirstTryComplete);
			}
		}
	}


	private void OnLevelSelect(int levelIndex)
	{
		GameData.CurrentLevelIndex = levelIndex;
		GameData.LevelRestarts = 0;
		GameData.ResetLevelData();
		GetTree().ChangeSceneToFile(Constants.ScenePaths.GameplayScreen);
	}


	private async Task ToTitleScreen()
	{
		if(GameData.GameMode == GameMode.QuickPlay)
		{
			GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
			return;
		}

		// Archipelago logic
		DialogWindow.ClearAllButtonEvents();
		DialogWindow.ShowDialog("Are you sure you want to disconnect from the Archipelago server?", DialogButtonType.YesNo);
		DialogWindow.YesButtonPressed += async () =>
		{
			await GameData.Archipelago.DisconnectFromServer();
			GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
		};
		DialogWindow.NoButtonPressed += () => { DialogWindow.Visible = false; };
	}
}
