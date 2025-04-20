using Godot;
using System;

public partial class IntroScene : Control
{
	public override void _Ready()
	{
		GetNode<Timer>("TransitionTimer").Timeout += OnTimeout;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept"))
		{
			GoToDialogue();
		}
	}

	private void OnTimeout()
	{
		GoToDialogue();
	}

	private void GoToDialogue()
	{
		GetTree().ChangeSceneToFile("res://DialogueScene.tscn");
		
	}
}
