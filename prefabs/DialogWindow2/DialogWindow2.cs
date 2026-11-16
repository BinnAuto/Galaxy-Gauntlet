public enum DialogButtonType
{
	OK,
	YesNo
}

public partial class DialogWindow2 : Node2D
{
	private Button InputBlocker;

	private AcceptDialog OKDialog;

	private ConfirmationDialog YesNoDialog;


	#region Delegates and Events

	public delegate void OnButtonPressedHandler();

	public event OnButtonPressedHandler OKButtonPressed = () => { };

	public event OnButtonPressedHandler YesButtonPressed = () => { };

	public event OnButtonPressedHandler NoButtonPressed = () => { };

	#endregion


	#region Godot Overrides

	public override void _Ready()
	{
		InputBlocker = (Button)GetNode(nameof(InputBlocker));

		OKDialog = (AcceptDialog)GetNode(nameof(OKDialog));

		YesNoDialog = (ConfirmationDialog)GetNode(nameof(YesNoDialog));

		var normalButtonStyle = (StyleBox)GD.Load("res://styles/button_style.tres");
		var hoveredButtonStyle = (StyleBox)GD.Load("res://styles/button_hover_style.tres");
		var pressedButtonStyle = (StyleBox)GD.Load("res://styles/button_pressed_style.tres");
		var windowStyle = (StyleBox)GD.Load("res://styles/dialog_window_content_style.tres");

		OKDialog.Confirmed += OnOKButtonPressed;
		OKDialog.GetLabel().HorizontalAlignment = HorizontalAlignment.Center;
		OKDialog.GetLabel().VerticalAlignment = VerticalAlignment.Center;
		var okLabel = OKDialog.GetLabel();
		okLabel.AddThemeColorOverride("font_color", Color.Color8(0, 0, 0, 255));
		okLabel.HorizontalAlignment = HorizontalAlignment.Center;
		var okButton = OKDialog.GetOkButton();
		okButton.AddThemeStyleboxOverride("normal", normalButtonStyle);
		okButton.AddThemeStyleboxOverride("hover", hoveredButtonStyle);
		okButton.AddThemeStyleboxOverride("pressed", pressedButtonStyle);
		okButton.AddThemeColorOverride("font_color", Color.Color8(0, 0, 0, 255));

		YesNoDialog.Confirmed += OnYesButtonPressed;
		YesNoDialog.Canceled += OnNoButtonPressed;
		YesNoDialog.GetLabel().HorizontalAlignment = HorizontalAlignment.Center;
		YesNoDialog.GetLabel().VerticalAlignment = VerticalAlignment.Center;
		YesNoDialog.AddThemeStyleboxOverride("panel", windowStyle);
		var yesNoText = YesNoDialog.GetLabel();
		yesNoText.AddThemeColorOverride("font_color", Color.Color8(0, 0, 0, 255));
		yesNoText.HorizontalAlignment = HorizontalAlignment.Center;
		var yesButton = YesNoDialog.GetOkButton();
		yesButton.AddThemeStyleboxOverride("normal", normalButtonStyle);
		yesButton.AddThemeStyleboxOverride("hover", hoveredButtonStyle);
		yesButton.AddThemeStyleboxOverride("pressed", pressedButtonStyle);
		yesButton.AddThemeColorOverride("font_color", Color.Color8(0, 0, 0, 255));
		var noButton = YesNoDialog.GetCancelButton();
		noButton.AddThemeStyleboxOverride("normal", normalButtonStyle);
		noButton.AddThemeStyleboxOverride("hover", hoveredButtonStyle);
		noButton.AddThemeStyleboxOverride("pressed", pressedButtonStyle);
		noButton.AddThemeColorOverride("font_color", Color.Color8(0, 0, 0, 255));
	}


	public override void _Process(double delta)
	{
		if(false == Visible)
		{
			return;
		}

		if(Input.IsActionJustPressed("player_ok"))
		{
			if(OKDialog.Visible)
			{
				OnOKButtonPressed();
			}
			if(YesNoDialog.Visible)
			{
				OnYesButtonPressed();
			}
		}

		if(Input.IsActionJustPressed("player_no") && YesNoDialog.Visible)
		{
			OnNoButtonPressed();
		}
	}

	#endregion


	#region Show/Hide Methods

	public void ShowDialog(string content, DialogButtonType dialogButtonType)
	{
		ShowDialog(content, GGConstants.GameName, dialogButtonType);
	}


	public void ShowDialog(string content, string title, DialogButtonType dialogButtonType)
	{
		switch (dialogButtonType)
		{
			case DialogButtonType.OK:
				OKDialog.Title = title;
				OKDialog.DialogText = content;
				OKDialog.Show();
				OKDialog.MoveToCenter();
				OKDialog.GetOkButton().ReleaseFocus();
				break;

			case DialogButtonType.YesNo:
				YesNoDialog.Title = title;
				YesNoDialog.DialogText = content;
				YesNoDialog.Show();
				YesNoDialog.MoveToCenter();
				YesNoDialog.GetOkButton().ReleaseFocus();
				YesNoDialog.GetCancelButton().ReleaseFocus();
				break;
		}
		Visible = true;
	}


	public void HideDialog()
	{
		OKDialog.Hide();
		YesNoDialog.Hide();
		InputBlocker.Hide();
	}

	#endregion


	#region Event Methods

	public void ClearAllButtonEvents()
	{
		ClearOKButtonEvents();
		ClearYesButtonPressedEvents();
		ClearNoButtonPressedEvents();
	}


	public void ClearOKButtonEvents()
	{
		if (OKButtonPressed is null)
		{
			OKButtonPressed = () => { };
			return;
		}

		foreach (var @delegate in OKButtonPressed.GetInvocationList())
		{
			OKButtonPressed -= (OnButtonPressedHandler)@delegate;
		}
	}


	public void ClearYesButtonPressedEvents()
	{
		if (YesButtonPressed is null)
		{
			YesButtonPressed = () => { };
			return;
		}

		foreach (var @delegate in YesButtonPressed.GetInvocationList())
		{
			YesButtonPressed -= (OnButtonPressedHandler)@delegate;
		}
	}


	public void ClearNoButtonPressedEvents()
	{
		if (NoButtonPressed is null)
		{
			NoButtonPressed = () => { };
			return;
		}

		foreach (var @delegate in NoButtonPressed.GetInvocationList())
		{
			NoButtonPressed -= (OnButtonPressedHandler)@delegate;
		}
	}

	#endregion


	#region Event Handlers

	private void OnOKButtonPressed()
	{
		OKButtonPressed.Invoke();
	}


	private void OnYesButtonPressed()
	{
		YesButtonPressed.Invoke();
	}


	private void OnNoButtonPressed()
	{
		NoButtonPressed.Invoke();
	}

	#endregion
}
