using Godot;
using System;

public class Main : Node
{
	public ConfigFile config = new ConfigFile();
#pragma warning disable 649
	[Export]
	public PackedScene MobScene;
#pragma warning restore 649
	public int Score;
	public int HighScore;
	public override void _Ready()
	{
		GD.Randomize();
		LoadSaveGame();
	}
	public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<HUD>("HUD").ShowGameOver();
        GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
		SaveGame();
    }

	public void NewGame()
	{
		Score = 0;

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Position2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<HUD>("HUD");
        hud.UpdateScore(Score);
        hud.UpdateHighScore(HighScore);
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
        GetNode<HUD>("HUD").UpdateScore(Score);
        GetNode<HUD>("HUD").UpdateHighScore(HighScore);

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

	public void SaveGame()
	{
		config.SetValue("Main", "HighScore", HighScore);
		GD.Print("Saving HighScore to: " + OS.GetUserDataDir() + "/savegame.dat");
        config.Save("user://savegame.dat");
	}

	public void LoadSaveGame()
	{
        config.Load("user://savegame.dat");
        HighScore = (int)config.GetValue("Main", "HighScore", HighScore);
        GD.Print("Loaded HighScore from: " + OS.GetUserDataDir() + "/savegame.dat");
		GD.Print("HighScore: " + HighScore);
        GetNode<HUD>("HUD").UpdateHighScore(HighScore);
    }
}
