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


	private void ToTitleScreen()
	{
		string titleMenuPath = "res://screens/TitleMenu/TitleMenu.tscn";
		GetTree().ChangeSceneToFile(titleMenuPath);
	}


	#region Event Handlers

	private void OnCancelPressed()
	{
		ToTitleScreen();
	}


	private void OnSavePressed()
	{
		GameData.TickRate = TickRate.Value;
		GameData.SaveSettings();
		ToTitleScreen();
	}

	#endregion

}
