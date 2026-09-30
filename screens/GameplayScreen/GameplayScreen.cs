using GalaxyGauntlet.scripts;
using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.SaveData;
using Godot.Collections;
using System;
using System.Linq;

public partial class GameplayScreen : Node
{
	private MapDisplay MapDisplay;

	private DialogWindow DialogWindow;

	private Label LevelName;

	private Label ChipCount;

	private Label Timer;

	private DateTime _previousSlipListProcessTime = DateTime.Parse("01/01/2000");

	private DateTime _previousProcessTime = DateTime.Parse("01/01/2000");

	private double _timer = 1;

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

			GameData.LoadCurrentLevel();
			_timer = GameData.TimeLimit + 0.99999;
			UpdateLabels();
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
			// TODO: Proper pause processing
			OnExit();
		}

		ProcessGameTick();
		CheckPlayerStatus();
		UpdateTimer(delta);
		UpdateLabels();
		if(false == DialogWindow.Visible && false == string.IsNullOrEmpty(GameData.DeathMessage))
		{
			MapDisplay.RenderMapData();
			OnPlayerDeath();
		}
	}

	#endregion

	private void UpdateLabels()
	{
		LevelName.Text = GameData.LevelName;
		ChipCount.Text = $"Chips Remaining: {Mathf.Max(GameData.ChipsRequired - GameData.ChipsCollected, 0)}";
		Timer.Text = $"Time: {(int)_timer}";
	}


	private void ProcessGameTick()
	{
		// Freeze mob lists before the process swaps mobs between lists,
		// which can cause double processing.
		var slipList = GameData.MobSlipList;
		var mobList = GameData.MobsToProcess;

		DateTime now = DateTime.Now;
		var timeSinceMobProcess = (now - _previousProcessTime).TotalSeconds;
		if (timeSinceMobProcess >= 0.2)
		{
			// Process entities that move 5 times or less per second
			if (GameData.ProcessMobs)
			{
				GameData.ProcessMobList(mobList, now);
				_previousProcessTime = now;
				GameData.PingPongStep = (GameData.PingPongStep + 1) % 2;
				GameData.SquareStep = (GameData.SquareStep + 1) % 4;
				ProcessButtonPresses();
			}
			CheckForInitializationByPlayer();
		}

		var timeSinceSlipListProcess = (now - _previousSlipListProcessTime).TotalSeconds;
		if (timeSinceSlipListProcess >= 0.1)
		{
			if (GameData.ProcessMobs)
			{
				// Process entities that move 10 times per second (slip list)
				GameData.ProcessMobList(slipList, now);
			}
			_previousSlipListProcessTime = now;
		}
	}


	private void ProcessButtonPresses()
	{
		GalaxyGauntlet.scripts.Color c = GalaxyGauntlet.scripts.Color.Green;
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
			_timer -= delta;
		}
		if(_timer < 0)
		{
			_timer = 0;
		}
		if(GameData.TimeLimit != 0 && _timer <= 0 && false == DialogWindow.Visible)
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
		if(input != Vector2.Zero)
		{
			GameData.ProcessMobs = true;
			GameData.TimerEnabled = true;

			// The player gets a free starting move
			DateTime now = DateTime.Now;
			var player = GameData.MobsToProcess.First(e => e is Player);
			GameData.ProcessMobList([player], now);
			_previousSlipListProcessTime = now;
			_previousProcessTime = now;
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
			DialogWindow.ShowDialog("Win!", DialogButtonType.OK);
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
			case GameMode.Archipelago:
				string levelSelectPath = "res://screens/LevelSelect/LevelSelect.tscn";
				GetTree().ChangeSceneToFile(levelSelectPath);
				break;
		}
	}


	private void OnGameComplete()
	{
		DialogWindow.ClearAllButtonEvents();
		DialogWindow.ShowDialog("Complete!", DialogButtonType.OK);
		SaveData.DeleteSaveSlot(SaveData.AutosaveSlotName);
		DialogWindow.OKButtonPressed += () =>
		{
			OnExit();
		};
	}


	private void RestartLevel()
	{
		GameData.LevelRestarts++;
		GameData.LoadCurrentLevel();
		_timer = GameData.TimeLimit + 0.999999;
		DialogWindow.Visible = false;
	}


	private void OnExit()
	{
		switch(GameData.GameMode)
		{
			case GameMode.Campaign:
				string titleMenuPath = "res://screens/TitleMenu/TitleMenu.tscn";
				GetTree().ChangeSceneToFile(titleMenuPath);
				break;

			case GameMode.QuickPlay:
			case GameMode.Archipelago:
				string levelSelectPath = "res://screens/LevelSelect/LevelSelect.tscn";
				GetTree().ChangeSceneToFile(levelSelectPath);
				break;
		}
	}
}
