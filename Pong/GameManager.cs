using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static partial class GameManager
{
	private static float ballSpeed = 100f;
	private static int maxBallSpeed = 1000;

	public static float BallSpeed 
	{ 
		get { return ballSpeed; }
		set
		{
			if (value < MaxBallSpeed)
				ballSpeed = value;
			else
				ballSpeed = maxBallSpeed;
		} 
	} 

	public static int MaxBallSpeed
	{
		get { return maxBallSpeed; }
		set
		{
			maxBallSpeed = value;
		}
	}

	static List<int> scores = new List<int>() { 0, 0 };

	public static void AddPointsForPlayer(int player, int score)
	{
		scores[player] += score;
	}
	
	public static float SpeedMultiplierState(){
		return BallSpeed / 100f;
	}

	public static int GetPointsForPlayer(int player)
	{
		return scores[player];
	}

	// Setzt die Geschwindigkeit des Balles fuer eine gewisse Zeit
	public static async void SlowBallForTime(float time, float multiplier)
	{
		// Aktuelle Geschwindigkeit speichern
		float originalSpeed = BallSpeed;

		// Neue Geschwindigkeit berechnen
		BallSpeed = BallSpeed * multiplier;

		// Warten (time Sekunden)
		await Task.Delay((int)(time * 1000));

		// Geschwindigkeit Zuruecksetzen
		BallSpeed = originalSpeed;
	}
}
