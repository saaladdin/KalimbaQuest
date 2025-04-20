using Godot;
using System;

public partial class StartScene : Control
{
	private Sprite2D Image1;
	private Sprite2D Image2;
	private Timer ImageSwitchTimer;
	private TextureButton StartButton;
	private TextureButton OptionButton;

	public override void _Ready()
	{
		// Get the nodes
		Image1 = GetNode<Sprite2D>("Image1");
		Image2 = GetNode<Sprite2D>("Image2");
		ImageSwitchTimer = GetNode<Timer>("ImageSwitchTimer");
		StartButton = GetNode<TextureButton>("StartButton");
		OptionButton = GetNode<TextureButton>("OptionButton");

		// Initially show the first image
		Image1.Visible = true;
		Image2.Visible = false;

		// Connect timer signal to switch images
		ImageSwitchTimer.Timeout += OnImageSwitchTimeout;
		ImageSwitchTimer.Start();

		// Connect the button's pressed signal to functions
		StartButton.Pressed += OnStartButtonPressed;
		OptionButton.Pressed += OnOptionButtonPressed;
	}

	private void OnImageSwitchTimeout()
	{
		// Toggle visibility between the two images
		Image1.Visible = !Image1.Visible;
		Image2.Visible = !Image2.Visible;
	}

	private void OnStartButtonPressed()
	{
		// Go to the game scene
		GoToGame();
	}

	private void OnOptionButtonPressed()
	{
		// Go to the options scene
		GoToOption();
	}

	private void GoToGame()
	{
		GetTree().ChangeSceneToFile("res://Sunshine.tscn");
	}

	private void GoToOption()
	{
		GetTree().ChangeSceneToFile("res://Kalimba.tscn");
	}
}
