using Godot;
using System;
using System.Collections.Generic;

public partial class Kalimba : Control
{
	private Dictionary<string, AudioStream> sounds = new();

	public override void _Ready()
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
		

		// Loop through children and auto-connect buttons by name
		foreach (Node child in GetNode("KeysContainer").GetChildren())
		{
			if (child is Button button)
			{
				string note = button.Name; // Assuming names like "C4", "D5", etc.
				if (sounds.ContainsKey(note))
				{
					button.Pressed += () => PlayNote(note);
				}
			}
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
	}
}
