using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGame();

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
        GetNode<Label>("ScoreLabel").Text = "0";

        var message = GetNode<Label>("Message");
        message.Text = "Dodge the Creeps!";
        message.Show();

        await ToSignal(GetTree().CreateTimer(1), "timeout");
        GetNode<Button>("StartButton").Show();
    }

    public void UpdateScore(int score, int highscore)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
        GetNode<Label>("HighscoreLabel").Text = "Highscore: " + highscore.ToString();
    }

    public void OnStartButtonPressed()
    {
        GetNode<Button>("StartButton").Hide();
        EmitSignal("StartGame");
    }

    public void OnMessageTimerTimeout()
    {
        GetNode<Label>("Message").Hide();
    }
}