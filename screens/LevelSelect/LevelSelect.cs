using GalaxyGauntlet.scripts;
using System;

public partial class LevelSelect : Node2D
{
	private GridContainer GridContainer;

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

			BuildButtonList();
		}
		catch(Exception e)
		{
			FileLogger.LogException("Error readying Level Select screen", e);
		}
	}


	private void BuildButtonList()
	{
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
}
