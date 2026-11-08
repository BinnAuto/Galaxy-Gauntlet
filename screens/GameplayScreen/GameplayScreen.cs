using GalaxyGauntlet.scripts;
using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.MapEntities.Shared;
using GalaxyGauntlet.scripts.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
			HintLabel.Text = GameData.Level.Hint;
			GameData.Level.Timer = GameData.Map.TimeLimit + 0.99999;
		}
		catch (Exception e)
		{
			FileLogger.LogException("Error readying Gameplay screen", e);
		}
	}


	public override void _Process(double delta)
	{
		// Render map if necessary
		if(GameData.Map.Rerender)
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
			GameData.Level.ProcessMobs = false;
			GameData.Level.TimerEnabled = false;
			GameData.Level.AcceptingPlayerInput = false;
			// TODO: Proper pause processing
			OnExit();
		}

		GameData.Level.ConfusionTimer = Mathf.Max(GameData.Level.ConfusionTimer - delta, 0);
		ProcessGameTick();
		CheckPlayerStatus();
		UpdateTimer(delta);
		UpdateLabels();
		if(false == DialogWindow.Visible && false == string.IsNullOrEmpty(GameData.Level.DeathMessage))
		{
			MapDisplay.RenderMapData();
			OnPlayerDeath();
		}
		if(GameData.Level.ProcessMobs && GameData.Archipelago.PendingItems.Count > 0)
		{
			GameData.Level.PlayerItems.AddRange(GameData.Archipelago.PendingItems);
			GameData.Archipelago.PendingItems.Clear();
		}
	}

	#endregion

	private void UpdateLabels()
	{
		LevelName.Text = GameData.Level.Name;
		ChipCount.Text = $"Chips Remaining: {Mathf.Max(GameData.Level.ChipsRequired - GameData.Level.ChipsCollected, 0)}";
		Timer.Text = $"Time: {(int)GameData.Level.Timer}";

		var currentItem = GameData.Map.GetItem(GameData.Level.PlayerCoordinate);
		HintLabel.Visible = (currentItem != null && currentItem is HintPanelItem);
	}


	private void ProcessGameTick()
	{
		// Freeze mob lists before the process swaps mobs between lists,
		// which can cause double processing.
		var playerList = GameData.Map.PlayerProcessList;
		var playerSlipList = GameData.Map.PlayerSliplist;
		var slipList = GameData.Map.MobSlipList;
		List<MapMob> mobList = [..GameData.Map.MobProcessList];

		DateTime now = DateTime.Now;
		double regularTick = 0.2;
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
			if (GameData.Level.ProcessMobs)
			{
				GameData.ProcessMobList(mobList);
				_previousMobProcessTime = now;
				GameData.Map.PingPongStep = (GameData.Map.PingPongStep + 1) % 2;
				GameData.Map.SquareStep = (GameData.Map.SquareStep + 1) % 4;
				ProcessButtonPresses();
			}
			CheckForInitializationByPlayer();
		}

		// Process monsters that move 10 times per second (slip list)
		if (timeSinceSlipListProcess >= slipListTick)
		{
			if (GameData.Level.ProcessMobs)
			{
				GameData.ProcessMobList(slipList);
			}
			_previousSlipListProcessTime = now;
		}
	}


	private static void ProcessButtonPresses()
	{
		Dictionary<GalaxyGauntlet.scripts.Color, int> buttonPresses = [];
		foreach(var button in GameData.Map.ButtonsToProcess)
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
		if(GameData.Map.TimeLimit < 0)
		{
			return;
		}

		// TODO: Set timer display
		if(GameData.Level.TimerEnabled)
		{
			GameData.Level.Timer -= delta;
		}
		if(GameData.Level.Timer < 0)
		{
			GameData.Level.Timer = 0;
		}
		if(GameData.Map.TimeLimit != 0 && GameData.Level.Timer <= 0 && false == DialogWindow.Visible)
		{
			GameData.Level.DeathMessage = GGConstants.DeathMessages.OutOfTime;
			OnPlayerDeath();
		}
	}


	private void CheckForInitializationByPlayer()
	{
		if(GameData.Level.ProcessMobs || !GameData.Level.AcceptingPlayerInput)
		{
			return;
		}

		Vector2I input = GetPlayerInput();
		if(input != Vector2.Zero)
		{
			GameData.Level.ProcessMobs = true;
			GameData.Level.TimerEnabled = true;

			// The player gets a free starting move
			DateTime now = DateTime.Now;
			_previousSlipListProcessTime = now;
			_previousMobProcessTime = now;
		}
	}


	private void CheckPlayerStatus()
	{
		if(false == GameData.Level.ProcessMobs)
		{
			return;
		}

		var currentTile = GameData.Map.GetTile(GameData.Level.PlayerCoordinate);
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
		DialogWindow.ShowDialog(GameData.Level.DeathMessage, DialogButtonType.OK);
		DialogWindow.OKButtonPressed += RestartLevel;
		GameData.Archipelago.IsProcessingDeathLink = false;
	}


	private void OnLevelComplete()
	{
		DialogWindow.HideDialog();
		switch(GameData.Level.GameMode)
		{
			case GameMode.Campaign:
				var gameComplete = GameData.Level.OnNextLevel();
				if(gameComplete)
				{
					OnGameComplete();
				}
				break;

			case GameMode.QuickPlay:
				GetTree().ChangeSceneToFile(GGConstants.ScenePaths.LevelSelect);
				break;

			case GameMode.Archipelago:
				GameData.Archipelago.CheckForGoal();
				GetTree().ChangeSceneToFile(GGConstants.ScenePaths.LevelSelect);
				break;
		}
	}


	private void OnGameComplete()
	{
		DialogWindow.ClearAllButtonEvents();
		DialogWindow.ShowDialog("Complete!", DialogButtonType.OK);
		SaveData.DeleteSaveSlot(GGConstants.SaveSlotNames.Autosave);
		DialogWindow.OKButtonPressed += OnExit;
	}


	private void RestartLevel()
	{
		GameData.Level.RestartCount++;
		GameData.LoadCurrentLevel();
		GameData.Level.Timer = GameData.Map.TimeLimit + 0.999999;
		DialogWindow.Visible = false;
	}


	private void OnExit()
	{
		switch(GameData.Level.GameMode)
		{
			case GameMode.Campaign:
				GetTree().ChangeSceneToFile(GGConstants.ScenePaths.TitleMenu);
				break;

			case GameMode.QuickPlay:
			case GameMode.Archipelago:
				GetTree().ChangeSceneToFile(GGConstants.ScenePaths.LevelSelect);
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
		string result = GameData.Level.RestartCount switch
		{
			0 => "Yowser! First Try!",
			1 or 2 => "Go Bit Buster!",
			3 or 4 => "Finished! Good Work!",
			_ => "At last! You did it!"
		};
		int timeBonus = GameData.Map.TimeLimit == 0
			? 0 : (int)GameData.Level.Timer * 10;
		int levelBonus = 500 * (GameData.Level.CurrentLevelIndex + 1);
		levelBonus = (int)(levelBonus * Mathf.Pow(0.8f, GameData.Level.RestartCount));
		levelBonus = Mathf.Max(levelBonus, 500);

		switch(GameData.Level.GameMode)
		{
			case GameMode.Archipelago:
			case GameMode.Campaign:
				string saveSlotName = (GameData.Level.GameMode == GameMode.Archipelago)
					? GGConstants.SaveSlotNames.Archipelago
					: GGConstants.SaveSlotNames.Autosave;
				var saveSlot = SaveData.GetSaveSlot(saveSlotName);
				if(false == saveSlot.HighScores.ContainsKey(GameData.Level.CurrentLevelIndex))
				{
					saveSlot.HighScores[GameData.Level.CurrentLevelIndex] = 0;
				}
				if(timeBonus + levelBonus > saveSlot.HighScores[GameData.Level.CurrentLevelIndex])
				{
					result += $"\nYou have a new high score!";
					saveSlot.HighScores[GameData.Level.CurrentLevelIndex] = timeBonus + levelBonus;
				}
				SaveData.WriteSaveSlot(saveSlot);

				result += $"\nTime Bonus: {timeBonus}";
				result += $"\nLevel Bonus: {levelBonus}";
				result += $"\nLevel Score: {timeBonus + levelBonus}";
				result += $"\nTotal Score: {saveSlot.HighScores.Sum(e => e.Value)}";
				if(GameData.Level.GameMode == GameMode.Archipelago)
				{
					string saveSlotJson = JsonSerializer.Serialize(saveSlot);
					GameData.Archipelago.SetDataStorageKey(GGConstants.Archipelago.SlotDataKeys.SaveData, saveSlotJson);
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
