using GalaxyGauntlet.scripts;
using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.MapEntities.Shared;
using GalaxyGauntlet.scripts.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
public partial class GameplayScreen : Node
{
	private MapDisplay MapDisplay;

	private DialogWindow DialogWindow;

	private Label LevelName;

	private Label ChipCount;

	private Label Timer;

	private Label HintLabel;

	private DateTime _previousPlayerProcessTime = DateTime.Parse("01/01/2000");

	private DateTime _previousPlayerSlipListProcessTime = DateTime.Parse("01/01/2000");

	private DateTime _previousSlipListProcessTime = DateTime.Parse("01/01/2000");

	private DateTime _previousMobProcessTime = DateTime.Parse("01/01/2000");

	#region Godot Overrides

	public override void _Ready()
	{
		try
		{
			MapDisplay = (MapDisplay)GetNode(nameof(MapDisplay));

			DialogWindow = (DialogWindow)GetNode(nameof(DialogWindow));

			LevelName = (Label)GetNode(nameof(LevelName));

			ChipCount = (Label)GetNode(nameof(ChipCount));

			Timer = (Label)GetNode(nameof(Timer));

			HintLabel = (Label)GetNode(nameof(HintLabel));
			HintLabel.Visible = false;
			HintLabel.Resized += OnHintLabelResized;

			GameData.LoadCurrentLevel();
			HintLabel.Text = GameData.LevelHint;
			GameData.Timer = GameData.TimeLimit + 0.99999;
		}
		catch (Exception e)
		{
			FileLogger.LogException("Error readying Gameplay screen", e);
		}
	}


	public override void _Process(double delta)
	{
		// Render map if necessary
		if(GameData.RerenderMap)
		{
			MapDisplay.RenderMapData();
		}

		if(Input.IsActionJustPressed("level_reset"))
		{
			RestartLevel();
			return;
		}
		if(Input.IsActionJustPressed("player_pause"))
		{
			GameData.ProcessMobs = false;
			GameData.TimerEnabled = false;
			GameData.AcceptingPlayerInput = false;
			// TODO: Proper pause processing
			OnExit();
		}

		GameData.ConfusionTimer = Mathf.Max(GameData.ConfusionTimer - delta, 0);
		ProcessGameTick();
		CheckPlayerStatus();
		UpdateTimer(delta);
		UpdateLabels();
		if(false == DialogWindow.Visible && false == string.IsNullOrEmpty(GameData.DeathMessage))
		{
			MapDisplay.RenderMapData();
			OnPlayerDeath();
		}
		if(GameData.ProcessMobs && GameData.PendingItems.Count > 0)
		{
			GameData.PlayerItems.AddRange(GameData.PendingItems);
			GameData.PendingItems.Clear();
			GameData.RerenderItemList = true;
		}
	}

	#endregion

	private void UpdateLabels()
	{
		LevelName.Text = GameData.LevelName;
		ChipCount.Text = $"Chips Remaining: {Mathf.Max(GameData.ChipsRequired - GameData.ChipsCollected, 0)}";
		Timer.Text = $"Time: {(int)GameData.Timer}";

		var currentItem = GameData.GetMapItem(GameData.PlayerCoordinate);
		HintLabel.Visible = (currentItem != null && currentItem is HintPanelItem);
	}


	private void ProcessGameTick()
	{
		// Freeze mob lists before the process swaps mobs between lists,
		// which can cause double processing.
		var playerList = GameData.PlayerProcessList;
		var playerSlipList = GameData.PlayerSliplist;
		var slipList = GameData.MobSlipList;
		List<MapMob> mobList = [..GameData.MobProcessList];

		DateTime now = DateTime.Now;
		double regularTick = 0.2 / GameData.TickRateNumeric;
		double slipListTick = regularTick / 2.0;

		var timeSincePlayerProcess = (now - _previousPlayerProcessTime).TotalSeconds;
		var playerInput = GetPlayerInput();
		if(playerInput != Vector2I.Zero && timeSincePlayerProcess >= regularTick)
		{
			if(playerList.Count != 0)
			{
				GameData.ProcessMobList(playerList);
				_previousPlayerProcessTime = now;
				_previousPlayerSlipListProcessTime = now;
			}
		}

		var timeSincePlayerSlipListProcess = (now - _previousPlayerSlipListProcessTime).TotalSeconds;
		if(timeSincePlayerSlipListProcess >= slipListTick)
		{
			if(playerSlipList.Count != 0)
			{
				GameData.ProcessMobList(playerSlipList);
				_previousPlayerSlipListProcessTime = now;
			}
		}

		var timeSinceMobListProcess = (now - _previousMobProcessTime).TotalSeconds;
		var timeSinceSlipListProcess = (now - _previousSlipListProcessTime).TotalSeconds;
		
		// Process monsters that move 5 times or less per second
		if (timeSinceMobListProcess >= regularTick)
		{
			if (GameData.ProcessMobs)
			{
				GameData.ProcessMobList(mobList);
				_previousMobProcessTime = now;
				GameData.PingPongStep = (GameData.PingPongStep + 1) % 2;
				GameData.SquareStep = (GameData.SquareStep + 1) % 4;
				ProcessButtonPresses();
			}
			CheckForInitializationByPlayer();
		}

		// Process monsters that move 10 times per second (slip list)
		if (timeSinceSlipListProcess >= slipListTick)
		{
			if (GameData.ProcessMobs)
			{
				GameData.ProcessMobList(slipList);
			}
			_previousSlipListProcessTime = now;
		}
	}


	private static void ProcessButtonPresses()
	{
		Dictionary<GalaxyGauntlet.scripts.Color, int> buttonPresses = [];
		foreach(var button in GameData.ButtonsToProcess)
		{
			button.ProcessTick();
			if(button.NewlyPressed)
			{
				if(false == buttonPresses.ContainsKey(button.Color))
				{
					buttonPresses[button.Color] = 0;
				}
				buttonPresses[button.Color]++;
			}
		}
	}


	private void UpdateTimer(double delta)
	{
		if(GameData.TimeLimit < 0)
		{
			return;
		}

		// TODO: Set timer display
		if(GameData.TimerEnabled)
		{
			GameData.Timer -= (delta * GameData.TickRateNumeric);
		}
		if(GameData.Timer < 0)
		{
			GameData.Timer = 0;
		}
		if(GameData.TimeLimit != 0 && GameData.Timer <= 0 && false == DialogWindow.Visible)
		{
			GameData.DeathMessage = Constants.DeathMessages.OutOfTime;
			OnPlayerDeath();
		}
	}


	private void CheckForInitializationByPlayer()
	{
		if(GameData.ProcessMobs || !GameData.AcceptingPlayerInput)
		{
			return;
		}

		Vector2I input = GetPlayerInput();
		if(input != Vector2.Zero)
		{
			GameData.ProcessMobs = true;
			GameData.TimerEnabled = true;

			// The player gets a free starting move
			DateTime now = DateTime.Now;
			_previousSlipListProcessTime = now;
			_previousMobProcessTime = now;
		}
	}


	private void CheckPlayerStatus()
	{
		if(false == GameData.ProcessMobs)
		{
			return;
		}

		var currentTile = GameData.GetMapTile(GameData.PlayerCoordinate);
		if(currentTile is ExitTile)
		{
			GameData.OnLevelComplete();
			DialogWindow.ClearAllButtonEvents();
			string dialogText = GetLevelScoreMessage();
			DialogWindow.ShowDialog(dialogText, DialogButtonType.OK);
			DialogWindow.OKButtonPressed += OnLevelComplete;
		}
	}


	private void OnPlayerDeath()
	{
		GameData.OnPlayerDeath();
		DialogWindow.ClearAllButtonEvents();
		DialogWindow.ShowDialog(GameData.DeathMessage, DialogButtonType.OK);
		DialogWindow.OKButtonPressed += RestartLevel;
		GameData.ProcessingDeathLink = false;
	}


	private void OnLevelComplete()
	{
		DialogWindow.HideDialog();
		switch(GameData.GameMode)
		{
			case GameMode.Campaign:
				var gameComplete = GameData.OnNextLevel();
				if(gameComplete)
				{
					OnGameComplete();
				}
				break;

			case GameMode.QuickPlay:
                GetTree().ChangeSceneToFile(Constants.ScenePaths.LevelSelect);
                break;

			case GameMode.Archipelago:
                // TODO: Score checks and goal condition
                GetTree().ChangeSceneToFile(Constants.ScenePaths.LevelSelect);
                break;
		}
	}


	private void OnGameComplete()
	{
		DialogWindow.ClearAllButtonEvents();
		DialogWindow.ShowDialog("Complete!", DialogButtonType.OK);
		SaveData.DeleteSaveSlot(Constants.SaveSlotNames.Autosave);
		DialogWindow.OKButtonPressed += OnExit;
	}


	private void RestartLevel()
	{
		GameData.LevelRestarts++;
		GameData.LoadCurrentLevel();
		GameData.Timer = GameData.TimeLimit + 0.999999;
		DialogWindow.Visible = false;
	}


	private void OnExit()
	{
		switch(GameData.GameMode)
		{
			case GameMode.Campaign:
				GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
				break;

			case GameMode.QuickPlay:
			case GameMode.Archipelago:
				GetTree().ChangeSceneToFile(Constants.ScenePaths.LevelSelect);
				break;
		}
	}


	private static Vector2I GetPlayerInput()
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
		return input;
	}


	private void OnHintLabelResized()
	{
		if(HintLabel.Size.X > 200)
		{
			HintLabel.SetSize(new(200, HintLabel.Size.Y));
		}
	}


	private static string GetLevelScoreMessage()
	{
		string result = GameData.LevelRestarts switch
		{
			0 => "Yowser! First Try!",
			1 or 2 => "Go Bit Buster!",
			3 or 4 => "Finished! Good Work!",
			_ => "At last! You did it!"
		};
		int timeBonus = GameData.TimeLimit == 0
			? 0 : (int)GameData.Timer * 10;
		int levelBonus = 500 * (GameData.CurrentLevelIndex + 1);
		levelBonus = (int)(levelBonus * Mathf.Pow(0.8f, GameData.LevelRestarts));
		levelBonus = Mathf.Max(levelBonus, 500);

		switch(GameData.GameMode)
		{
			case GameMode.Archipelago:
			case GameMode.Campaign:
				string saveSlotName = (GameData.GameMode == GameMode.Archipelago)
					? Constants.SaveSlotNames.Archipelago
					: Constants.SaveSlotNames.Autosave;
				var saveSlot = SaveData.GetSaveSlot(saveSlotName);
				if(false == saveSlot.HighScores.ContainsKey(GameData.CurrentLevelIndex))
				{
					saveSlot.HighScores[GameData.CurrentLevelIndex] = 0;
				}
				if(timeBonus + levelBonus > saveSlot.HighScores[GameData.CurrentLevelIndex])
				{
					result += $"\nYou have a new high score!";
					saveSlot.HighScores[GameData.CurrentLevelIndex] = timeBonus + levelBonus;
				}
				SaveData.WriteSaveSlot(saveSlot);

				result += $"\nTime Bonus: {timeBonus}";
				result += $"\nLevel Bonus: {levelBonus}";
				result += $"\nLevel Score: {timeBonus + levelBonus}";
				result += $"\nTotal Score: {saveSlot.HighScores.Sum(e => e.Value)}";
				if(GameData.GameMode == GameMode.Archipelago)
				{
					string saveSlotJson = JsonSerializer.Serialize(saveSlot);
					GD.Print($"JSON: {saveSlotJson}");
					GameData.SetDataStorage(Constants.Archipelago.SlotDataKeys.SaveData, saveSlotJson);
				}
				break;

			case GameMode.QuickPlay:
				result += $"\nTime Bonus: {timeBonus}";
				result += $"\nLevel Bonus: {levelBonus}";
				result += $"\nLevel Score: {timeBonus + levelBonus}";
				break;
		}

		return result;
	}
}
