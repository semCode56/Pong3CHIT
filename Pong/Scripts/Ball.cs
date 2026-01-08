using Godot;
using System;
using System.ComponentModel.Design;
using static Godot.Image;
using static Godot.TextServer;

public partial class Ball : Area2D
{
	private PackedScene ballObject = GD.Load<PackedScene>("res://Scenes/Ball.tscn");
	
	//Sprite vom Ball
	[Export]
	private Sprite2D _sprite;
	
	
	// Konstante für die Anfangsgeschwindigkeit des Balls (Pixel pro Sekunde)
	const int INITIAL_BALL_SPEED = 100;

	// Aktuelle Geschwindigkeit des Balls (Pixel pro Sekunde)
	public float ballSpeed = INITIAL_BALL_SPEED;
	// Speichert initalBallSpeed um die Diff nach reset nicht zu verlieren
	public float initialBallSpeed = INITIAL_BALL_SPEED;


	// Größe des Viewports (Bildschirmbereich), in dem sich der Ball bewegt
	Vector2 screenSize;

	// Bewegungsrichtung des Balls (normierter Vektor)
	Vector2 direction;

	// Zufallsgenerator für kleine Zufallsabweichungen der Richtung
	Random random = new Random();

	//True, wenn der Ball als PowerUp agieren soll
	public bool IsPowerUpBall { get; set; }
	public int PowerUpLevel { get; set; }
	public bool IsDouble {get; set;}

	// Wird einmal aufgerufen, wenn der Node in die Szene eingefügt wird
	public override void _Ready()
	{
		// Aktuelle Größe des Viewports holen (Breite/Höhe)
		screenSize = GetViewportRect().Size;

		if(!IsPowerUpBall && !IsDouble){
			// Startrichtung: nach rechts (X=1) mit leicht zufälliger vertikaler Komponente (Y)
			direction = new Vector2(1f, random.NextSingle() * 0.1f);
		}
		else{
			// Startrichtung: nach rechts oder links (X=1 / -1) mit leicht zufälliger vertikaler Komponente (Y)
			direction = new Vector2(new Random().Next(0, 2) == 1 ? 1 : -1, random.NextSingle() * 0.1f);
		}
		
		// Richtung auf Länge 1 normalisieren (damit nur ballSpeed die tatsächliche Geschwindigkeit bestimmt)
		direction = direction.Normalized();
		
		if(IsPowerUpBall)
			SpawnPowerUp();
	}

	// Wird in jedem Frame aufgerufen. 'delta' ist die vergangene Zeit seit dem letzten Frame.
	public override void _Process(double delta)
	{
		if (!IsPowerUpBall)
		{			
			// Aktuelle Position des Balls zwischenspeichern
			Vector2 ballPos = Position;
			
			ballSpeed = GameManager.BallSpeed;
		
		
			// Neue Position berechnen: alte Position + Richtung * Geschwindigkeit * Zeit
			ballPos += direction * ballSpeed * (float)delta;

			// Sicher stellen, dass der Ball im Spielfeld bleibt
			ballPos.X = Mathf.Clamp(ballPos.X, 0, screenSize.X);
			ballPos.Y = Mathf.Clamp(ballPos.Y, 0, screenSize.Y);

			// Berechnete Position zurück auf den Node anwenden
			Position = ballPos;
			
			if(IsDouble)
				SetDoubleBallColour();

		}			
		else{
			// Aktuelle Position des Balls zwischenspeichern
			Vector2 ballPos = Position;
			
			ballSpeed = 100;
			
			// Neue Position berechnen: alte Position + Richtung * Geschwindigkeit * Zeit
			ballPos += direction * ballSpeed * (float)delta;

			// Sicher stellen, dass der Ball im Spielfeld bleibt
			ballPos.X = Mathf.Clamp(ballPos.X, 0, screenSize.X);
			ballPos.Y = Mathf.Clamp(ballPos.Y, 0, screenSize.Y);

			// Berechnete Position zurück auf den Node anwenden
			Position = ballPos;
			
			SetPowerUpColor(PowerUpLevel);
		}
	}

	// Wird aufgerufen, wenn der Ball ein Paddle trifft.
	// spinFactor beschreibt, wo der Ball auf dem Paddle getroffen hat
	// (z.B. -1 oben, 0 Mitte, +1 unten) und beeinflusst den Abprallwinkel.
	public void HitPaddle(float spinFactor)
	{
		if (!IsPowerUpBall)
		{
			// Neue Richtung:
			// X-Komponente wird umgedreht (Ball fliegt in die andere horizontale Richtung),
			// Y-Komponente wird aus dem spinFactor abgeleitet
			// -> Trifft der Ball das Paddle im oberen Bereich, dann wird der Ball nach oben abgelenkt
			// -> Trifft der Ball das Paddle im unteren Bereich, dann wird der Ball nach unten abgelenkt
			// -> Trifft der Ball das Paddle im mittleren Bereich, dann fliegt der Ball gerade
			Vector2 reflected = new Vector2(-direction.X, spinFactor * 0.8f);

			// Neue Richtung normalisieren, damit wir wieder einen Einheitsvektor haben
			direction = reflected.Normalized();

			// Ballgeschwindigkeit leicht erhöhen, damit das Spiel mit der Zeit schneller wird
			ballSpeed *= 1.1f;
		}
		else //Falls Ball ein Powerup ist, entferne ihn nach berührung des Paddles und führe seine Kraft aus
		{
			Vector2 reflected = new Vector2(-direction.X, spinFactor * 0.8f);

			// Neue Richtung normalisieren, damit wir wieder einen Einheitsvektor haben
			direction = reflected.Normalized();

			//Je nach PowerUpLevel --> führe verschiedene Funktionen aus (Beginnend bei 1)
			switch (PowerUpLevel)
			{
				case 1: //Langsamer Ball
					GameManager.SlowBallForTime(10, 0.5f); //Halbiert die Geschwindigkeit des Balles für 10 Sekunden
					break;

				case 2: //Doppelball
					SpawnBall(); //Spawns a Doubleball
					break;
					
				case 3: //Schneller Ball
					GameManager.SlowBallForTime(10, 2f); //Halbiert die Geschwindigkeit des Balles für 10 Sekunden
					break;

				case 4: //Crashout Ball
					GameManager.SlowBallForTime(10, 5f); //Halbiert die Geschwindigkeit des Balles für 10 Sekunden
					break;
					
				case 5: //Ragebait Ball
					GameManager.SlowBallForTime(10, 5f); //Halbiert die Geschwindigkeit des Balles für 10 Sekunden
					SpawnBall();
					break;
					
				case 6: //Eight Ball
					for(int i = 0; i < 8; i++)
						SpawnBall();
					break;

				default:
					break;
			}

			QueueFree();
		}
	   
	}

	public void Bounce(bool isHorizontalWall)
	{
		if (!IsPowerUpBall) //If the Ball is a Powerup
		{
			if (isHorizontalWall)
			{
				direction.Y = -direction.Y;
			}
			else
			{
				float x = direction.X;

				if (x > 0)
				{
					GameManager.AddPointsForPlayer(0, 1);
				}
				else if (x < 0)
				{
					GameManager.AddPointsForPlayer(1, 1);
				}

				Reset();
			}
		}
		else //If the Ball is a Powerup
		{
			QueueFree();
		}
	   
	}

	public void Reset()
	{
		if(IsDouble){
			QueueFree();
			return;
		}
		
		// Ball zurück in die Bildschirmmitte setzen
		Vector2 ballPos = screenSize * 0.5f;

		// Geschwindigkeit auf Anfangswert zurücksetzen
		ballSpeed = initialBallSpeed;

		// Startrichtung: alte horizontale Richtung mit leicht zufälliger vertikaler Komponente (Y)
		direction = new Vector2(direction.X, random.NextSingle() * 0.15f);

		// Richtung auf Länge 1 normalisieren (damit nur ballSpeed die tatsächliche Geschwindigkeit bestimmt)
		direction = direction.Normalized();

		Position = ballPos;
	}

	public void SpawnPowerUp()
	{
		// Ball zurück in die Bildschirmmitte setzen
			Vector2 ballPos = screenSize * 0.5f;

			// Geschwindigkeit auf Anfangswert zurücksetzen
			ballSpeed = 100;

			// Startrichtung: alte horizontale Richtung mit leicht zufälliger vertikaler Komponente (Y)
			Random r = new Random();
			int startDir = r.Next(0, 2) == 0 ? 1 : -1;

			direction = new Vector2(startDir * direction.X, random.NextSingle() * 0.15f);

			// Richtung auf Länge 1 normalisieren (damit nur ballSpeed die tatsächliche Geschwindigkeit bestimmt)
			direction = direction.Normalized();

			Position = ballPos;
	}
	
	public void SetPowerUpColor(int diff)
	{
		if (_sprite != null)
		{
			
			if(diff == 1)
				_sprite.Modulate = Colors.Green;
			else if(diff == 2){
				_sprite.Modulate = Colors.Yellow;
			}
			else if(diff == 3){
				_sprite.Modulate = Colors.Red;
			}	
			else if(diff == 4){
				_sprite.Modulate = Color.FromHtml("880000");
			}
			else if(diff == 5){
				_sprite.Modulate = Color.FromHtml("8800ff");
			}		
			else if(diff == 6){
				_sprite.Modulate = Color.FromHtml("ffa500");
			}
		}
	}
	
	public void SetDoubleBallColour()
	{
		if (_sprite != null)
		{
			_sprite.Modulate = Colors.Cyan;			
		}
	}
	
	public void SpawnBall()
	{
		Random random = new Random();

		// Prüfen, ob die Node noch existiert
		if (!IsInstanceValid(this)) 
		{
			Console.WriteLine("Instance Faild Loading");
			return;
		}

		GD.Print("Spawn Ball");

		//Erstellen
		Ball ball = ballObject.Instantiate<Ball>();
		ball.IsDouble = true;
		
		Vector2 ballPos = screenSize * 0.5f;
		// Geschwindigkeit auf Anfangswert zurücksetzen
		ballSpeed = GameManager.BallSpeed;
		// Startrichtung: alte horizontale Richtung mit leicht zufälliger vertikaler Komponente (Y)
		direction = new Vector2(direction.X, random.NextSingle() * 0.15f);
		
		ball.Position = ballPos;

		// Hinzufügen
		GetTree().Root.AddChild(ball);
	}
}
