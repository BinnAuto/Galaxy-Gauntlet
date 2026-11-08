using GalaxyGauntlet.scripts;

public partial class SettingsMenu : Node
{
	#region Child Nodes

	private UISlider MasterVolume;

	private UISlider MusicVolume;

	private UIButton CancelButton;

	private UIButton SaveButton;

    #endregion

    private bool _processMasterVolume = false;

	private bool _processMusicVolume = false;


	#region Godot Overrides

	public override void _Ready()
	{
		GameData.LoadSettings();

		MasterVolume = (UISlider)GetNode(nameof(MasterVolume));
		MasterVolume.Value = GameData.MasterVolume;
		MasterVolume.DragStarted += () => { _processMasterVolume = true; };
        MasterVolume.DragEnded += () => { _processMasterVolume = false; };
		MasterVolume.ValueChanged += () => { GalaxyGauntlet.Common.Global.SetBusVolume(GGConstants.AudioBuses.Master, MasterVolume.Value); };

        MusicVolume = (UISlider)GetNode(nameof(MusicVolume));
		MusicVolume.Value = GameData.MusicVolume;
        MusicVolume.DragStarted += () => { _processMusicVolume = true; };
        MusicVolume.DragEnded += () => { _processMusicVolume = false; };
        MasterVolume.ValueChanged += () => { GalaxyGauntlet.Common.Global.SetBusVolume(GGConstants.AudioBuses.Music, MusicVolume.Value); };

        CancelButton = (UIButton)GetNode(nameof(CancelButton));
		CancelButton.Pressed += OnCancelPressed;

		SaveButton = (UIButton)GetNode(nameof(SaveButton));
		SaveButton.Pressed += OnSavePressed;
    }


    public override void _Process(double delta)
    {
        if(_processMasterVolume)
		{
            GalaxyGauntlet.Common.Global.SetBusVolume(GGConstants.AudioBuses.Master, MasterVolume.Value);
        }
        if (_processMusicVolume)
        {
            GalaxyGauntlet.Common.Global.SetBusVolume(GGConstants.AudioBuses.Music, MusicVolume.Value);
        }
    }

    #endregion

    #region Event Handlers

    private void OnCancelPressed()
	{
        GalaxyGauntlet.Common.Global.SetBusVolume(GGConstants.AudioBuses.Master, GameData.MasterVolume);
        GalaxyGauntlet.Common.Global.SetBusVolume(GGConstants.AudioBuses.Music, GameData.MusicVolume);
        GetTree().ChangeSceneToFile(GGConstants.ScenePaths.TitleMenu);
    }


	private void OnSavePressed()
	{
		GameData.MasterVolume = MasterVolume.Value;
		GameData.MusicVolume = MusicVolume.Value;
		GameData.SaveSettings();
        GetTree().ChangeSceneToFile(GGConstants.ScenePaths.TitleMenu);
    }

	#endregion

}
