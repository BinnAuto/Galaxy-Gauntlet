using GalaxyGauntlet.scripts;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class LevelSelect : Node2D
{
	private GridContainer GridContainer;

	private UIButton ReloadButton;

	private UIButton BackButton;

	private DialogWindow2 DialogWindow2;

	private LineEdit LevelSearch;

	private Button ButtonRef;

	private Node2D ArchipelagoDisplay;

	private Label TotalScore;

	private Label LevelsCompleted;

	private CheckButton HideCompletedLevels;


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
			LevelSearch = (LineEdit)Control.GetNode(nameof(LevelSearch));
			LevelSearch.TextChanged += OnSearchLevels;

            ButtonRef = (Button)GetNode(nameof(ButtonRef));

			ReloadButton = (UIButton)GetNode(nameof(ReloadButton));
			ReloadButton.Pressed += BuildButtonList;
			
			BackButton = (UIButton)GetNode(nameof(BackButton));
			BackButton.Pressed += async () => await ToTitleScreen();

			DialogWindow2 = (DialogWindow2)GetNode(nameof(DialogWindow2));
			DialogWindow2.Visible = false;

			ArchipelagoDisplay = (Node2D)GetNode(nameof(ArchipelagoDisplay));

			TotalScore = (Label)ArchipelagoDisplay.GetNode(nameof(TotalScore));

			LevelsCompleted = (Label)ArchipelagoDisplay.GetNode(nameof(LevelsCompleted));

			HideCompletedLevels = (CheckButton)ArchipelagoDisplay.GetNode(nameof(HideCompletedLevels));
			HideCompletedLevels.Toggled += OnHideCompletedLevels;
			HideCompletedLevels.ButtonPressed = true;

            BuildButtonList();

			ArchipelagoDisplay.Visible = false;
			if (GameData.GameMode == GameMode.Archipelago)
			{
				ArchipelagoDisplay.Visible = true;
				TotalScore.Text = GameData.Archipelago.CurrentTotalScore.ToString("N0");
				if(GameData.Archipelago.ScoreGoal != 0)
				{
					TotalScore.Text += $" ({GameData.Archipelago.ScoreGoal:N0})";
				}
				LevelsCompleted.Text = GameData.Archipelago.NumLevelsCompleted.ToString("N0");
				if(GameData.Archipelago.LevelCompletionGoal!= 0)
				{
					LevelsCompleted.Text += $" ({GameData.Archipelago.LevelCompletionGoal:N0})";
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
		GameData.Archipelago.NumLevelsCompleted = 0;
        GameData.Level.LevelSet = GalaxyGauntlet.Common.Global.GetLevelList(GameData.GameMode);
		if(GameData.GameMode == GameMode.Archipelago)
		{
			string[] levelIgnores = GameData.Archipelago.GetSlotDataKeyOrDefault(GGConstants.Archipelago.SlotDataKeys.IgnoreLevelFiles, string.Empty).Split('|');
			GameData.Level.LevelSet = GameData.Level.LevelSet.Where(e => false == levelIgnores.Any(f => e.EndsWith(f, StringComparison.InvariantCultureIgnoreCase))).ToArray();
		}
		GD.Print($"{GameData.Level.LevelSet.Length} level files found");
		int levelIndex = 0;
		foreach(string file in GameData.Level.LevelSet)
		{
			int tempLevel = levelIndex;
			string levelName = LevelLoader.GetLevelName(file);
			if(string.IsNullOrEmpty(levelName))
			{
				continue;
			}

			levelIndex++;
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
					Text = string.Empty
				};
				if (GameData.Archipelago.HasLocationBeenChecked(completeLocation))
				{
					levelComplete.Text = "Complete";
					GameData.Archipelago.NumLevelsCompleted++;
                }
				GridContainer.AddChild(levelComplete);

				// No Resets label
				Label levelFirstTryComplete = new()
				{
					Text = string.Empty
				};
				if (GameData.Archipelago.HasLocationBeenChecked(noResetsLocation))
				{
					levelFirstTryComplete.Text = " (No Resets)";
				}
				GridContainer.AddChild(levelFirstTryComplete);
			}
		}
		if(GameData.GameMode == GameMode.Archipelago)
		{
			GameData.Archipelago.CheckForGoal();
		}
		FilterLevels();
	}


	private void OnLevelSelect(int levelIndex)
	{
		GameData.Level.CurrentLevelIndex = levelIndex;
		GameData.Level.RestartCount = 0;
		GameData.Level.Reset();
		GetTree().ChangeSceneToFile(GGConstants.ScenePaths.GameplayScreen);
	}


	private void OnSearchLevels(string _)
	{
		FilterLevels();
    }


	private void OnHideCompletedLevels(bool _)
	{
		FilterLevels();
	}


	private void FilterLevels()
	{
		string searchTerm = LevelSearch.Text.Replace(" ", string.Empty);
		bool hideCompletedLevels = false;
		if(GameData.GameMode == GameMode.Archipelago)
		{
            hideCompletedLevels = HideCompletedLevels.ButtonPressed;
		}

		var children = GridContainer.GetChildren();
		for(int i = 0; i < children.Count; i++)
		{
			var child = children[i];
			if(child is not Button levelButton)
			{
				continue;
			}

			string levelName = levelButton.Text.Replace(" ", string.Empty);
			bool isVisible = (string.IsNullOrEmpty(searchTerm))
				|| levelName.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase);
			if(GameData.GameMode == GameMode.Archipelago && hideCompletedLevels)
			{
				var completedLabel = (Label)children[i + 1];
				isVisible &= string.IsNullOrWhiteSpace(completedLabel.Text);
            }

            levelButton.Visible = isVisible;
            if (GameData.GameMode == GameMode.Archipelago)
            {
                i++;
                ((Control)children[i]).Visible = isVisible;
                i++;
                ((Control)children[i]).Visible = isVisible;
            }
        }
	}


	private async Task ToTitleScreen()
	{
		if(GameData.GameMode == GameMode.QuickPlay)
		{
			GetTree().ChangeSceneToFile(GGConstants.ScenePaths.TitleMenu);
			return;
		}

		// Archipelago logic
		DialogWindow2.ClearAllButtonEvents();
		DialogWindow2.ShowDialog("Are you sure you want to disconnect from the Archipelago server?", DialogButtonType.YesNo);
		DialogWindow2.YesButtonPressed += async () =>
		{
			await GameData.Archipelago.DisconnectFromServer();
			GetTree().ChangeSceneToFile(GGConstants.ScenePaths.TitleMenu);
		};
		DialogWindow2.NoButtonPressed += DialogWindow2.HideDialog;
	}
}
