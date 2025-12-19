using Godot;
using System;

public partial class Pong : Node2D
{
    [Export]
    Label scorePlayer1;
    [Export]
    Label scorePlayer2;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Console.WriteLine("Game started.");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        scorePlayer1.Text = GameManager.GetPointsForPlayer(0).ToString();
        scorePlayer2.Text = GameManager.GetPointsForPlayer(1).ToString();
    }
}
