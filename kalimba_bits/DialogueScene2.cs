using Godot;
using System;
using System.Collections.Generic;

public partial class DialogueScene2 : Control
{
	private Label speakerLabel;
	private Label dialogueLabel;
	private Sprite2D Sarah;
	private Sprite2D Bowie;
	private Sprite2D Sarah_Talk;
	private Sprite2D Bowie_Talk;

	private int index = 0;

	private List<(string speaker, string text)> dialogueLines = new()
	{
		("Sarah", "HORRAAYYYAYYAYY. YOU DID IT!!!!"),
		("Bowie", "THANK YOUUUUUUUU"),
		("Sarah", "Now that you know how to play the kalimba, you can play more songs"),
		("Sarah", "You will be stronger, faster, smarter, better")
	};

	public override void _Ready()
	{
		speakerLabel = GetNode<Label>("DialoguePanel/SpeakerLabel");
		dialogueLabel = GetNode<Label>("DialoguePanel/DialogueLabel");
		
		Sarah = GetNode<Sprite2D>("DialoguePanel/Sarah");
		Bowie = GetNode<Sprite2D>("DialoguePanel/Bowie");
		Sarah_Talk = GetNode<Sprite2D>("DialoguePanel/Sarah_Talk");
		Bowie_Talk = GetNode<Sprite2D>("DialoguePanel/Bowie_Talk");
		
		SetSpeakingSprite("None");

		ShowNextLine();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept") || @event is InputEventMouseButton mouse && mouse.Pressed)
		{
			ShowNextLine();
		}
	}

	private void ShowNextLine()
	{
		if (index < dialogueLines.Count)
		{
			var line = dialogueLines[index];
			speakerLabel.Text = line.speaker;
			dialogueLabel.Text = line.text;
			
			SetSpeakingSprite(line.speaker);
			
			index++;
		}
		else
		{
			GetTree().ChangeSceneToFile("res://Kalimba.tscn");
		}
	}
	
	private void SetSpeakingSprite(string speaker)
	{
		// Default: everyone idle
		Sarah.Visible = true;
		Sarah_Talk.Visible = false;
		Bowie.Visible = true;
		Bowie_Talk.Visible = false;

		// Toggle to talking expression
		if (speaker == "Sarah")
		{
			Sarah.Visible = false;
			Sarah_Talk.Visible = true;
		}
		else if (speaker == "Bowie")
		{
			Bowie.Visible = false;
			Bowie_Talk.Visible = true;
		}
	}
}
