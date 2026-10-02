using GalaxyGauntlet.scripts;

public partial class SettingsMenu : Node
{
	#region Child Nodes

	private UISlider TimeModifier;

	private UIButton CancelButton;

	private UIButton SaveButton;

	#endregion


	#region Godot Overrides

	public override void _Ready()
	{
		GameData.LoadSettings();
		TimeModifier = (UISlider)GetNode(nameof(TimeModifier));
		TimeModifier.Value = GameData.TimeModifier;

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
		GameData.TimeModifier = TimeModifier.Value;
		GameData.SaveSettings();
		ToTitleScreen();
	}

	#endregion

}
