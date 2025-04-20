using Godot;
using System;
using System.Threading.Tasks;

public partial class StartScene : Control
{
	private Sprite2D Image1;
	private Sprite2D Image2;
	private Sprite2D Jump1;  // New sprite for jump1
	private Sprite2D Jump2;  // New sprite for jump2
	private TextureButton StartButton;
	private TextureButton OptionButton;
	private Timer ImageSwitchTimer; // Timer to switch Image1 and Image2

	public override void _Ready()
	{
		// Get the nodes
		Image1 = GetNode<Sprite2D>("Image1");
		Image2 = GetNode<Sprite2D>("Image2");
		Jump1 = GetNode<Sprite2D>("Jump1"); // Reference for jump1 image
		Jump2 = GetNode<Sprite2D>("Jump2"); // Reference for jump2 image
		StartButton = GetNode<TextureButton>("StartButton");
		OptionButton = GetNode<TextureButton>("OptionButton");
		ImageSwitchTimer = GetNode<Timer>("ImageSwitchTimer"); // Timer for switching Image1/Image2

		// Initially show the first image
		Image1.Visible = true;
		Image2.Visible = false;

		// Initially hide jump1 and jump2
		Jump1.Visible = false;
		Jump2.Visible = false;

		// Connect the button's pressed signal to functions
		StartButton.Pressed += OnStartButtonPressed;
		OptionButton.Pressed += async () => await OnOptionButtonPressed(); // ✨ Make it async

		// Connect timer signal to switch images automatically
		ImageSwitchTimer.Timeout += OnImageSwitchTimeout;
		ImageSwitchTimer.Start(); // Start the automatic image switching
	}

	private async Task OnOptionButtonPressed()
	{
		// Stop the regular switching timer while we animate manually
		ImageSwitchTimer.Stop();

		// Hide Image1 and Image2, show Jump1
		Image1.Visible = false;
		Image2.Visible = false;
		Jump1.Visible = true;

		// Show the animation with jump1 and jump2
		for (int i = 0; i < 5; i++)
		{
			Jump1.Visible = false;
			Jump2.Visible = true;
			await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

			Jump1.Visible = true;
			Jump2.Visible = false;
			await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		}

		// After the animation, hide jump1 and jump2, restore Image1 and Image2
		Jump1.Visible = false;
		Jump2.Visible = false;
		Image1.Visible = true;
		Image2.Visible = false;

		// Restart the image switching timer
		ImageSwitchTimer.Start();

		// Transition back to the start screen
		GoToStartScreen();
	}

	private void OnStartButtonPressed()
	{
		// Go to the game scene
		GoToGame();
	}

	private void OnImageSwitchTimeout()
	{
		// Toggle visibility between Image1 and Image2
		Image1.Visible = !Image1.Visible;
		Image2.Visible = !Image2.Visible;
	}

	private void GoToGame()
	{
		GetTree().ChangeSceneToFile("res://Sunshine.tscn");
	}

	private void GoToStartScreen()
	{
		GetTree().ChangeSceneToFile("res://StartScreen.tscn"); // Replace with the actual start screen path
	}
}
