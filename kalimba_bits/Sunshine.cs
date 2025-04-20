using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Sunshine : Control
{
	private Dictionary<string, AudioStream> sounds = new();
	private List<string> tutorialNotes = new() { "G4", "C5", "D5", "E5", "E5" };
	private int currentNoteIndex = 0;
	private bool isPlayerTurn = false;

	// Cat face sprites
	private Sprite2D Sarah_Normal;
	private Sprite2D Sarah_Talk;
	private Sprite2D Sarah_Sad;

	private Sprite2D Bowie_Normal;
	private Sprite2D Bowie_Sad;

	// Victory Images (to mimic a GIF)
	private Sprite2D VictoryImage1;
	private Sprite2D VictoryImage2;

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

		Bowie_Normal = GetNode<Sprite2D>("Bowie_Normal");
		Bowie_Sad = GetNode<Sprite2D>("Bowie_Sad");

		// Get victory images
		VictoryImage1 = GetNode<Sprite2D>("VictoryImage1");
		VictoryImage2 = GetNode<Sprite2D>("VictoryImage2");

		// Hide victory images initially
		VictoryImage1.Visible = false;
		VictoryImage2.Visible = false;

		// Set initial visibility for cat faces
		Sarah_Normal.Visible = true;
		Sarah_Talk.Visible = false;
		Sarah_Sad.Visible = false;

		Bowie_Normal.Visible = false;
		Bowie_Sad.Visible = false;

		// Get dialogue labels
		SpeakerLabel = GetNode<Label>("DialoguePanel/SpeakerLabel");
		DialogueLabel = GetNode<Label>("DialoguePanel/DialogueLabel");

		// Dialogue before tutorial
		await ShowDialogue("Sarah", "Hello againnnn! You will play...");
		await ShowDialogue("Sarah", "You are my Sunshine <3");
		await ShowDialogue("Sarah", "Listen to this!!");

		await PlayTutorial();
		await ShowDialogue("Sarah", "Bowie you got this!");

		// Player's turn starts
		isPlayerTurn = true;
		Sarah_Normal.Visible = false;
		Bowie_Normal.Visible = true;
		await ShowDialogue("Bowie", "I will! For my sunshine <3");
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

			if (currentNoteIndex >= tutorialNotes.Count)
			{
				// Show victory images
				await ShowVictoryAnimation();

				// Proceed to next steps after victory
				Bowie_Normal.Visible = false;
				await ShowDialogue("Sarah", "You did it!!! That was perfect!");
				isPlayerTurn = false;
				await ShowDialogue("Sarah", "Let's keep goingggg!");
				GetTree().ChangeSceneToFile("res://StartScene.tscn"); // or next scene
			}
		}
		else
		{
			// Wrong note logic
			Bowie_Normal.Visible = false;
			Bowie_Sad.Visible = true;

			await ShowDialogue("Sarah", $"Nooo Bowie! {note} wasn't right!");
			await ShowDialogue("Sarah", "Try again my sunshine");

			Bowie_Sad.Visible = false;
			Bowie_Normal.Visible = true;

			currentNoteIndex = 0;
		}
	}

	private async Task ShowDialogue(string speaker, string text)
	{
		SpeakerLabel.Text = speaker;
		DialogueLabel.Text = "";

		// Show correct talking face
		if (speaker == "Sarah")
		{
			Sarah_Normal.Visible = false;
			Sarah_Sad.Visible = false;
			Sarah_Talk.Visible = true;

			Bowie_Normal.Visible = false;
			Bowie_Sad.Visible = false;
		}
		else if (speaker == "Bowie")
		{
			Bowie_Normal.Visible = true;
			Bowie_Sad.Visible = false;

			Sarah_Normal.Visible = false;
			Sarah_Talk.Visible = false;
			Sarah_Sad.Visible = false;
		}

		// Typewriter effect
		foreach (char c in text)
		{
			DialogueLabel.Text += c;
			await ToSignal(GetTree().CreateTimer(0.03f), "timeout");
		}

		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

		// Reset faces after speaking
		if (speaker == "Sarah")
		{
			Sarah_Talk.Visible = false;
			Sarah_Normal.Visible = true;
		}
	}

	private async Task ShowVictoryAnimation()
	{
		// Hide the dialogue panel during victory animation
		DialogueLabel.Visible = false;
		SpeakerLabel.Visible = false;

		// Show victory image 1 first
		VictoryImage1.Visible = true;
		VictoryImage2.Visible = false;

		// Alternate between the two victory images to mimic a GIF
		for (int i = 0; i < 5; i++) // Repeat the animation for a few cycles
		{
			await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
			VictoryImage1.Visible = false;
			VictoryImage2.Visible = true;

			await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
			VictoryImage1.Visible = true;
			VictoryImage2.Visible = false;
		}

		// Hide victory images and restore dialogue
		VictoryImage1.Visible = false;
		VictoryImage2.Visible = false;
		DialogueLabel.Visible = true;
		SpeakerLabel.Visible = true;
	}
}
