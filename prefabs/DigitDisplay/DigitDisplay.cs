public partial class DigitDisplay : Node2D
{
	#region Child Nodes
	
	private AnimatedSprite2D[] Digits = new AnimatedSprite2D[3];
	
	#endregion
	
	public int? CurrentValue { get; private set; } = null;
		
	#region Godot Overrides
	
	public override void _Ready()
	{
		Digits[0] = (AnimatedSprite2D)GetNode("Hundreds");
		Digits[1] = (AnimatedSprite2D)GetNode("Tens");
		Digits[2] = (AnimatedSprite2D)GetNode("Ones");	
	}
	
	#endregion
	
	
	public void SetValue(int? value) 
	{
		if(value == CurrentValue)
		{
			return;
		}
		
		CurrentValue = value;
		Digits[0].Frame = 10;
		Digits[1].Frame = 10;
		Digits[2].Frame = 10;
		if(false == value.HasValue)
		{
			return;
		}
		
		string numberString = value.Value.ToString();
		if(value.Value >= 1000)
		{
			numberString = numberString[^3..];
		}
		numberString = ("   " + numberString)[^3..];
		
		for(int i = 0; i < numberString.Length; i++)
		{
			switch(numberString[i])
			{
				case ' ':
					Digits[i].Frame = 0;
					break;
					
				default:
					int digit = int.Parse(numberString[i].ToString());
					Digits[i].Frame = digit;
					break;
			}	
		}
	}
}
