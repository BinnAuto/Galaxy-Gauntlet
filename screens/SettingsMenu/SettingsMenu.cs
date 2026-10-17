using GalaxyGauntlet.scripts;

public partial class SettingsMenu : Node
{
	#region Child Nodes

	private UISlider TickRate;

	private UISlider MasterVolume;

	private UISlider MusicVolume;

	private UIButton CancelButton;

	private UIButton SaveButton;

    private AudioStreamPlayer2D Music;

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

        CancelButton = (UIButton)GetNode(nameof(CancelButton));
		CancelButton.Pressed += OnCancelPressed;

		SaveButton = (UIButton)GetNode(nameof(SaveButton));
		SaveButton.Pressed += OnSavePressed;

        Music = (AudioStreamPlayer2D)GetNode(nameof(Music));
        LoadMusic();
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

    private void LoadMusic()
    {
        // Build missing directories
        if (false == Directory.Exists("./custom")
            || false == Directory.Exists("./custom/music")
            || false == Directory.Exists("./custom/music/menu"))
        {
            return;
        }

        string audioFile = GGG.GetRandomMusicFile("./custom/music/menu");
        if (string.IsNullOrEmpty(audioFile))
        {
            return;
        }

        var audioStream = GGG.BuildAudioStream(audioFile);
        Music.Stream = audioStream;
        Music.Play();
    }

    #region Event Handlers

    private void OnCancelPressed()
	{
		GGG.SetBusVolume(Constants.AudioBuses.Master, GameData.MasterVolume);
        GGG.SetBusVolume(Constants.AudioBuses.Music, GameData.MusicVolume);
        GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
    }


	private void OnSavePressed()
	{
		GameData.TickRate = TickRate.Value;
		GameData.MasterVolume = MasterVolume.Value;
		GameData.MusicVolume = MusicVolume.Value;
		GameData.SaveSettings();
        GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
    }

	#endregion

}
