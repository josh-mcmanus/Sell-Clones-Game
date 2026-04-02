using Godot;
using System;

public partial class TorsoMenu : Control
{
	[Export]
	private Button closeTorsoMenuButton;
	
	[Signal]
	public delegate void CloseTorsoMenuEventHandler();
	
	
	private const string TAG = "TorsoMenu";
	
	
	public override void _Ready()
	{
		closeTorsoMenuButton.Pressed += Pressed;
	}
	
	
	// Activated on click of the exit button
	// For disabling visibility of Torso Menu
	public void Pressed()
	{
		EmitSignal(SignalName.CloseTorsoMenu);
	}
	
}
