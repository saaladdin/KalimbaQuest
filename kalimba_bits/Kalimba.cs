using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Kalimba : Control
{
	private Dictionary<string, AudioStream> sounds = new();
	
	private Sprite2D Sarah;
	private Texture2D SarahSad;
	private Texture2D sadFace;

	private List<string> tutorialNotes = new() { "C4", "E4", "G4", "C5" };
	private int currentNoteIndex = 0;
	private bool isPlayerTurn = false;

	public override async void _Ready()
	{
		// Preload all the sounds
		sounds["D6"] = GD.Load<AudioStream>("res://kalimba_sounds/D6.mp3");
		sounds["B5"] = GD.Load<AudioStream>("res://kalimba_sounds/B5.mp3");
		sounds["G5"] = GD.Load<AudioStream>("res://kalimba_sounds/G5.mp3");
		sounds["E5"] = GD.Load<AudioStream>("res://kalimba_sounds/E5.mp3");
		sounds["C5"] = GD.Load<AudioStream>("res://kalimba_sounds/C5.mp3");
		sounds["A4"] = GD.Load<AudioStream>("res://kalimba_sounds/A4.mp3");
		sounds["F4"] = GD.Load<AudioStream>("res://kalimba_sounds/F4.mp3");
		sounds["D4"] = GD.Load<AudioStream>("res://kalimba_sounds/D4.mp3");
		sounds["C4"] = GD.Load<AudioStream>("res://kalimba_sounds/C4.mp3");
		sounds["E4"] = GD.Load<AudioStream>("res://kalimba_sounds/E4.mp3");
		sounds["G4"] = GD.Load<AudioStream>("res://kalimba_sounds/G4.mp3");
		sounds["B4"] = GD.Load<AudioStream>("res://kalimba_sounds/B4.mp3");
		sounds["D5"] = GD.Load<AudioStream>("res://kalimba_sounds/D5.mp3");
		sounds["F5"] = GD.Load<AudioStream>("res://kalimba_sounds/F5.mp3");
		sounds["A5"] = GD.Load<AudioStream>("res://kalimba_sounds/A5.mp3");
		sounds["C6"] = GD.Load<AudioStream>("res://kalimba_sounds/C6.mp3");
		sounds["E6"] = GD.Load<AudioStream>("res://kalimba_sounds/E6.mp3");

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

		// Play tutorial notes
		await PlayTutorial();
		isPlayerTurn = true;
		GD.Print("🎵 Your turn! Repeat the melody.");
	}

	private async Task PlayTutorial()
	{
		GD.Print("👂 Listen to the melody...");
		foreach (string note in tutorialNotes)
		{
			await HighlightAndPlay(note);
			await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		}
	}
	
	private async Task HighlightAndPlay(string note)
	{
		// Play the sound
		PlayNote(note);

		// Try to find and "press" the button visually
		var keyButton = GetNodeOrNull<Button>($"KeysContainer/{note}");
		if (keyButton != null)
		{
			// Simulate press by changing modulate color
			var originalColor = keyButton.Modulate;
			keyButton.Modulate = new Color(1, 1, 0.5f); // yellow-ish

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

	private void CheckPlayerInput(string note)
	{
		if (note == tutorialNotes[currentNoteIndex])
		{
			currentNoteIndex++;
			GD.Print($"✅ {note}");

			if (currentNoteIndex >= tutorialNotes.Count)
			{
				GD.Print("🎉 You played it perfectly!");
				isPlayerTurn = false;
				// Optionally: advance to next level or restart tutorial
			}
		}
		else
		{
			GD.Print($"❌ {note} was wrong! Starting over.");
			currentNoteIndex = 0;
		}
	}
}
