using Godot;
using System;

public class Main : Node
{
#pragma warning disable 649
	[Export]
	public PackedScene MobScene;
#pragma warning restore 649
	public int Score;
	public int HighScore;
	public override void _Ready()
	{
		GD.Randomize();
	}

	public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<HUD>("HUD").ShowGameOver();
        GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();

    }

	public void NewGame()
	{
		Score = 0;

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Position2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<HUD>("HUD");
        hud.UpdateScore(Score, HighScore);
        hud.ShowMessage("Get Ready!");
        GetTree().CallGroup("mobs", "queue_free");
		if (hud.music)
		{
        GetNode<AudioStreamPlayer>("Music").Play();
		}
    }

	public void OnScoreTimerTimeout()
	{
		Score++;
		if(Score >= HighScore) { HighScore = Score; }
        GetNode<HUD>("HUD").UpdateScore(Score, HighScore);

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
