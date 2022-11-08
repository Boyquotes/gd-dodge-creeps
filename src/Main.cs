using Godot;
using System;

public class Main : Node
{
#pragma warning disable 649
	[Export]
	public PackedScene MobScene;
#pragma warning restore 649
	public int Score;
	public override void _Ready()
	{
		GD.Randomize();
		NewGame();
	}

	public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
	}

	public void NewGame()
	{
		Score = 0;

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Position2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
	}

	public void OnScoreTimerTimeout()
	{
		Score++;
	}

	public void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	public void OnMobTimerTimeout()
	{
		var mob = (Mob)MobScene.Instance();

		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.Offset = GD.Randi();

		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2; //pi because godot doesnt use degrees for rotation it uses radians for some weird reason

		mob.Position = mobSpawnLocation.Position;

		//randomize direction
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		AddChild(mob);

	}
}
