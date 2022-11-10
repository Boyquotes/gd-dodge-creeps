using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGame();

    [Export]
    public bool music;

    public void ShowMessage(string text)
    {
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();
        GetNode<Timer>("MessageTimer").Start();
    }

    async public void ShowGameOver() {
        ShowMessage("Game over!");
        
        var messageTimer =  GetNode<Timer>("MessageTimer");
        await ToSignal(messageTimer, "timeout");
        GetNode<Label>("ScoreLabel").Text = "";

        var message = GetNode<Label>("Message");
        message.Text = "Dodge the Creeps!";
        message.Show();

        await ToSignal(GetTree().CreateTimer(1), "timeout");
        GetNode<Button>("StartButton").Show();
        GetNode<CheckButton>("MusicSetting").Show();
    }

    public void UpdateScore(int score, int highscore)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
        GetNode<Label>("HighscoreLabel").Text = "Highscore: " + highscore.ToString();
    }

    public void OnStartButtonPressed()
    {
        GetNode<Button>("StartButton").Hide();
        GetNode<CheckButton>("MusicSetting").Hide();
        EmitSignal("StartGame");
    }

    public void OnMessageTimerTimeout()
    {
        GetNode<Label>("Message").Hide();
    }
    public void OnMusicSettingToggled(bool button_pressed)
    {
        music = button_pressed;
    }
}