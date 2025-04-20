using Godot;
using System;

public partial class IntroScene : Control
{
	private Sprite2D image1;
	private Sprite2D image2;
	private Timer imageSwitchTimer;

	public override void _Ready()
	{
		// Get the nodes
		image1 = GetNode<Sprite2D>("Image1");
		image2 = GetNode<Sprite2D>("Image2");
		imageSwitchTimer = GetNode<Timer>("ImageSwitchTimer");

		// Initially show the first image
		image1.Visible = true;
		image2.Visible = false;

		// Connect the timer's Timeout signal to the function that will switch images
		imageSwitchTimer.Timeout += OnImageSwitchTimeout;

		// Start the timer to begin switching images
		imageSwitchTimer.Start();
	}

	private void OnImageSwitchTimeout()
	{
		// Toggle visibility between the two images
		image1.Visible = !image1.Visible;
		image2.Visible = !image2.Visible;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept"))
		{
			// Switch to the next scene
			GoToDialogue();
		}
	}

	private void GoToDialogue()
	{
		GetTree().ChangeSceneToFile("res://kalimba_bits/DialogueScene.tscn");
	}
}
