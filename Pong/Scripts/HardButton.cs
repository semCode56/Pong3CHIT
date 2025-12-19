using Godot;
using System;

public partial class HardButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPressed()
	{
        var scene = GD.Load<PackedScene>("res://Scenes/Pong.tscn");
        var game = scene.Instantiate();
        var ball = game.GetNode<Ball>("Ball");
        ball.initialBallSpeed = 300;
        GetTree().Root.AddChild(game);
        QueueFree();
    }
}
