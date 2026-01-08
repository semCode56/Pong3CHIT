using Godot;
using System;

public partial class PowerOnOff : Button
{
	[Export]
	Label PowerToggel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PowerToggel.Text=$"PowerUps:{GameManager.PowerUpsActive}";
	}
	private void OnPressed()
	{
		 GameManager.PowerUpsActive = !GameManager.PowerUpsActive;
	}
}
