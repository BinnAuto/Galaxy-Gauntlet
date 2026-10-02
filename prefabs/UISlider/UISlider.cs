[Tool]
public partial class UISlider : Node
{
	#region Exposed to Editor

	[Export]
	private string LabelText = "_slider_";

	[Export]
	private double InitialValue = 50;

	[Export]
	private double Minimum = 0;

	[Export]
	private double Maximum = 100;

	[Export]
	private double Step = 1;

	[Export]
	private string ValueLabelSuffix = string.Empty;

	#endregion

	#region Child Nodes

	private Label TitleLabel;

	private HSlider Slider;

	private Label ValueLabel;

	#endregion

	#region Delegates and Events

	public delegate void OnValueChangeHandler();

	public event OnValueChangeHandler ValueChanged = () => { };

	public delegate void OnDragStartHandler();

	public event OnDragStartHandler DragStarted = () => { };

	public delegate void OnDragEndHandler();

	public event OnDragEndHandler DragEnded = () => { };

	#endregion


	public double Value
	{
		get
		{
			if(Slider is null)
			{
				return 0;
			}

			return Slider.Value;
		}
		set
		{
			if(Slider is null)
			{
				return;
			}

			Slider.Value = Mathf.Min(Maximum, Mathf.Max(value, Minimum));
			OnSliderChange();
		}
	}


	#region Godot Overrides

	public override void _Ready()
	{
		TitleLabel = (Label)GetNode(nameof(TitleLabel));
		TitleLabel.Text = LabelText;

		Slider = (HSlider)GetNode(nameof(Slider));
		Slider.MinValue = Minimum;
		Slider.MaxValue = Maximum;
		Slider.Step = Step;
		Slider.Value = Mathf.Min(Maximum, Mathf.Max(InitialValue, Minimum));

		ValueLabel = (Label)GetNode(nameof(ValueLabel));
		ValueLabel.Text = $"{Slider.Value}";
	}

	
	public override void _Process(double delta)
	{
		if(Engine.IsEditorHint())
		{
			TitleLabel.Text = LabelText;
			Slider.Value = Mathf.Min(Maximum, Mathf.Max(InitialValue, Minimum));
		}
		OnSliderChange(Slider.Value);
	}

	#endregion



	#region Event Handlers

	private void OnDragStart()
	{
		DragStarted.Invoke();
	}


	private void OnDragEnded(bool valueChanged)
	{
		DragEnded.Invoke();
		if(valueChanged)
		{
			OnSliderChange();
		}
	}


	private void OnSliderChange()
	{
		OnSliderChange(Slider.Value);
		ValueChanged.Invoke();
	}


	private void OnSliderChange(double value)
	{
		ValueLabel.Text = $"{Slider.Value}{ValueLabelSuffix}";
	}

	#endregion
}
