using GalaxyGauntlet.scripts;

public partial class SettingsMenu : Node
{
	#region Child Nodes

	private UISlider TickRate;

	private UIButton CancelButton;

	private UIButton SaveButton;

	#endregion


	#region Godot Overrides

	public override void _Ready()
	{
		GameData.LoadSettings();
		TickRate = (UISlider)GetNode(nameof(TickRate));
		TickRate.Value = GameData.TickRate;

		CancelButton = (UIButton)GetNode(nameof(CancelButton));
		CancelButton.Pressed += OnCancelPressed;

		SaveButton = (UIButton)GetNode(nameof(SaveButton));
		SaveButton.Pressed += OnSavePressed;
	}

	#endregion


	#region Event Handlers

	private void OnCancelPressed()
	{
        GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
    }


	private void OnSavePressed()
	{
		GameData.TickRate = TickRate.Value;
		GameData.SaveSettings();
        GetTree().ChangeSceneToFile(Constants.ScenePaths.TitleMenu);
    }

	#endregion

}
