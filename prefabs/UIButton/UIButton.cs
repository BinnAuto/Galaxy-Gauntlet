using System;

[Tool]
public partial class UIButton : Control
{
	#region Exposed to Editor

	[Export]
	public string ButtonText;

	[Export]
	public int FontSize = 15;

	[Export]
	public bool Enabled = true;

	#endregion


	#region Child Nodes

	private Button Button;

	#endregion


	#region Delegates and Events

	public delegate void OnButtonPressedHandler();

	public delegate void OnButtonHoveredHandler();

	public delegate void OnButtonExitedHandler();

	public event OnButtonPressedHandler Pressed = () => { };

	public event OnButtonHoveredHandler Hovered = () => { };

	public new event OnButtonExitedHandler MouseExited = () => { };

	#endregion


	#region Godot Overrides

	public override void _Ready()
	{
		try
		{
			Button = (Button)GetNode(nameof(Button));
			SetButtonText(ButtonText);
			Button.Pressed += OnButtonPressed;
			Button.MouseEntered += OnButtonHovered;
			Button.AddThemeFontSizeOverride("font_size", FontSize);
		}
		catch(Exception e)
		{
			FileLogger.LogException("Error readying UI Button", e);
		}
	}


	public override void _Process(double delta)
	{
		if(Engine.IsEditorHint())
		{
			SetButtonText(ButtonText);
			Button.AddThemeFontSizeOverride("font_size", FontSize);
		}
	}

	#endregion


	public void Disable()
	{
		Enabled = false;
		Button.Disabled = !Enabled;
	}


	public void Enable()
	{
		Enabled = true;
		Button.Disabled = !Enabled;
	}


	public void SetButtonText(string text)
	{
		ButtonText = text;
		Button.Size = new(50, 1);
		Button.Text = ButtonText;
	}


	private void OnButtonPressed()
	{
		if(false == Enabled)
		{
			return;
		}

		Pressed.Invoke();
	}


	private void OnButtonHovered()
	{
		if (false == Enabled)
		{
			return;
		}

		Hovered.Invoke();
	}
}
