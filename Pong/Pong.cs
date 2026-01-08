using Godot;
using System;
using System.Threading.Tasks;
using System.Globalization;

public partial class Pong : Node2D
{
	[Export]
	Label scorePlayer1;
	[Export]
	Label scorePlayer2;
	[Export]
	Label speedText;

	private PackedScene ballObject = GD.Load<PackedScene>("res://Scenes/Ball.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Console.WriteLine("Game started.");
		SpawnPowerUps();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		scorePlayer1.Text = GameManager.GetPointsForPlayer(0).ToString();
		scorePlayer2.Text = GameManager.GetPointsForPlayer(1).ToString();
		speedText.Text = "x" + GameManager.SpeedMultiplierState().ToString("f2", CultureInfo.InvariantCulture);
	}

	//Asyncron, da diese parallel laufen soll. Spawnt nach zufaelliger Zeit ein Powerup
	public async void SpawnPowerUps()
	{
		Random random = new Random();

		while (true)
		{
			//Warten (10 bis 30 Sekunden)
			int waitTime = random.Next(30, 60) * 1000;
			await Task.Delay(waitTime);

			// Prüfen, ob die Node noch existiert
			if (!IsInstanceValid(this)) 
			{
				Console.WriteLine("Instance Faild Loading");
				return;
			}

			GD.Print("Spawn PowerUp");

			//Erstellen
			Ball powerUp = ballObject.Instantiate<Ball>();
			powerUp.IsPowerUpBall = true;
			powerUp.PowerUpLevel = random.Next(1, 7); // 1 - 5 oder 6

			// Hinzufügen
			AddChild(powerUp);
		}
	}
}
