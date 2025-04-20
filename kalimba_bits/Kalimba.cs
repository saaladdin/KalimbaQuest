using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Kalimba : Control
{
	private Dictionary<string, AudioStream> sounds = new();
	private List<string> tutorialNotes = new() { "C4", "E4", "G4", "C5" };
	private int currentNoteIndex = 0;
	private bool isPlayerTurn = false;

	// Cat face sprites
	private Sprite2D Sarah_Normal;
	private Sprite2D Sarah_Talk;
	private Sprite2D Sarah_Sad;

	// Dialogue UI
	private Label SpeakerLabel;
	private Label DialogueLabel;

	public override async void _Ready()
	{
		// Preload all the sounds
		foreach (string note in new[] {
			"D6", "B5", "G5", "E5", "C5", "A4", "F4", "D4", "C4", "E4", "G4", "B4", "D5", "F5", "A5", "C6", "E6"
		})
		{
			sounds[note] = GD.Load<AudioStream>($"res://kalimba_sounds/{note}.mp3");
		}

		// Hook up button presses
		foreach (Node child in GetNode("KeysContainer").GetChildren())
		{
			if (child is Button button)
			{
				string note = button.Name;
				if (sounds.ContainsKey(note))
				{
					button.Pressed += () => PlayNote(note);
				}
			}
		}

		// Get cat faces
		Sarah_Normal = GetNode<Sprite2D>("Sarah_Normal");
		Sarah_Talk = GetNode<Sprite2D>("Sarah_Talk");
		Sarah_Sad = GetNode<Sprite2D>("Sarah_Sad");

		Sarah_Normal.Visible = true;
		Sarah_Talk.Visible = false;
		Sarah_Sad.Visible = false;

		// Get dialogue labels
		SpeakerLabel = GetNode<Label>("DialoguePanel/SpeakerLabel");
		DialogueLabel = GetNode<Label>("DialoguePanel/DialogueLabel");

		// Dialogue before tutorial
		await ShowDialogue("Sarah", "Hi Queers!! I'm Sarah! Let me show you how to play a part of...");
		await ShowDialogue("Sarah", "You are my Sunshine <3");
		await ShowDialogue("Sarah", "Listen nowww!!");

		await PlayTutorial();
		isPlayerTurn = true;
	}

	private async Task PlayTutorial()
	{
		foreach (string note in tutorialNotes)
		{
			await HighlightAndPlay(note);
			await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		}
	}

	private async Task HighlightAndPlay(string note)
	{
		PlayNote(note);

		var keyButton = GetNodeOrNull<Button>($"KeysContainer/{note}");
		if (keyButton != null)
		{
			var originalColor = keyButton.Modulate;
			keyButton.Modulate = new Color(1, 1, 0.5f); // yellow highlight
			await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
			keyButton.Modulate = originalColor;
		}
	}

	private void PlayNote(string note)
	{
		if (sounds.TryGetValue(note, out var sound))
		{
			var player = new AudioStreamPlayer();
			player.Stream = sound;
			AddChild(player);
			player.Play();
			player.Finished += () => player.QueueFree();
		}
		else
		{
			GD.PrintErr($"Sound for {note} not found!");
		}

		if (isPlayerTurn)
		{
			CheckPlayerInput(note);
		}
	}

	private async void CheckPlayerInput(string note)
	{
		if (note == tutorialNotes[currentNoteIndex])
		{
			currentNoteIndex++;
			GD.Print($"✅ {note}");

			if (currentNoteIndex >= tutorialNotes.Count)
			{
				await ShowDialogue("Sarah", "You played it perfectly! YAY!! <3");
				isPlayerTurn = false;
			}
		}
		else
		{
			await ShowDialogue("Sarah", $"NOO! {note} was wrong! Starting over nowww.");
			ShowSadFace();
			currentNoteIndex = 0;
		}
	}

	private async void ShowSadFace()
	{
		Sarah_Normal.Visible = false;
		Sarah_Talk.Visible = false;
		Sarah_Sad.Visible = true;

		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

		Sarah_Sad.Visible = false;
		Sarah_Normal.Visible = true;
	}

	private async Task ShowDialogue(string speaker, string text)
	{
		SpeakerLabel.Text = speaker;
		DialogueLabel.Text = "";

		// Show talking face
		Sarah_Normal.Visible = false;
		Sarah_Sad.Visible = false;
		Sarah_Talk.Visible = true;

		// Optional typewriter effect
		foreach (char c in text)
		{
			DialogueLabel.Text += c;
			await ToSignal(GetTree().CreateTimer(0.03f), "timeout");
		}

		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

		// Return to normal face
		Sarah_Talk.Visible = false;
		Sarah_Normal.Visible = true;
	}
}
