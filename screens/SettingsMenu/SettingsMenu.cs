using GalaxyGauntlet.scripts;

public partial class SettingsMenu : Node
{
	#region Child Nodes

	private UISlider TickRate;

	private UISlider MasterVolume;

	private UISlider MusicVolume;

	private UISlider WindowSize;

	private UIButton CancelButton;

	private UIButton SaveButton;

	private DialogWindow DialogWindow;

	#endregion

	private bool _processMasterVolume = false;

	private bool _processMusicVolume = false;


	#region Godot Overrides

	public override void _Ready()
	{
		GameData.LoadSettings();
		TickRate = (UISlider)GetNode(nameof(TickRate));
		TickRate.Value = GameData.TickRate;

		MasterVolume = (UISlider)GetNode(nameof(MasterVolume));
		MasterVolume.Value = GameData.MasterVolume;
		MasterVolume.DragStarted += () => { _processMasterVolume = true; };
		MasterVolume.DragEnded += () => { _processMasterVolume = false; };
		MasterVolume.ValueChanged += () => { GGG.SetBusVolume(Constants.AudioBuses.Master, MasterVolume.Value); };

		MusicVolume = (UISlider)GetNode(nameof(MusicVolume));
		MusicVolume.Value = GameData.MusicVolume;
		MusicVolume.DragStarted += () => { _processMusicVolume = true; };
		MusicVolume.DragEnded += () => { _processMusicVolume = false; };
		MasterVolume.ValueChanged += () => { GGG.SetBusVolume(Constants.AudioBuses.Music, MusicVolume.Value); };

		WindowSize = (UISlider)GetNode(nameof(WindowSize));
		WindowSize.Value = GameData.WindowMagnifier;

		CancelButton = (UIButton)GetNode(nameof(CancelButton));
		CancelButton.Pressed += OnCancelPressed;

		SaveButton = (UIButton)GetNode(nameof(SaveButton));
		SaveButton.Pressed += OnSavePressed;

		DialogWindow = (DialogWindow)GetNode(nameof(DialogWindow));
	}


	public override void _Process(double delta)
	{
		if(_processMasterVolume)
		{
			GGG.SetBusVolume(Constants.AudioBuses.Master, MasterVolume.Value);
		}
		if (_processMusicVolume)
		{
			GGG.SetBusVolume(Constants.AudioBuses.Music, MusicVolume.Value);
		}
	}

	#endregion


	#region Event Handlers

	private void OnCancelPressed()
	{
		GGG.SetBusVolume(Constants.AudioBuses.Master, GameData.MasterVolume);
		GGG.SetBusVolume(Constants.AudioBuses.Music, GameData.MusicVolume);
		GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
	}


	private void OnSavePressed()
	{
		// Verify new screen size does not exceed monitor size
		var newWindowResolution = (int)WindowSize.Value * GameData.InitialWindowResolution;
		var size = DisplayServer.ScreenGetSize((int)DisplayServer.ScreenOfMainWindow);
		if(newWindowResolution.Value.X > size.X
			|| newWindowResolution.Value.Y > size.Y)
		{
			DialogWindow.ShowDialog("Failed to save settings. The new window dimensions would be larger than this screen!", DialogButtonType.OK);
			DialogWindow.OKButtonPressed += () =>
			{
				WindowSize.Value = 1;
				DialogWindow.Hide();
			};
			return;
		}

		GameData.TickRate = TickRate.Value;
		GameData.MasterVolume = MasterVolume.Value;
		GameData.MusicVolume = MusicVolume.Value;
		GameData.WindowMagnifier = WindowSize.Value;
		GameData.SaveSettings();
		GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
	}

	#endregion

}
