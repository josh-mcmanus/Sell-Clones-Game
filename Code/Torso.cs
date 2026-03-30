using Godot;
using System;

public partial class Torso : Area2D
{
	[Signal]
	public delegate void TorsoHoverEventHandler();
	
	[Signal]
	public delegate void TorsoHoverExitEventHandler();
	
	
	private bool isHovering = false;
	
	
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}
	
	// Emits a signal when the torso is hovered over
	// To be used to play the opening animation
	private void OnMouseEntered()
	{
		if (Name == "Torso")
		{
			EmitSignal(SignalName.TorsoHover);
		}
	}
	
	// To be used to play the closing animation
	private void OnMouseExited()
	{
		if (Name == "Torso")
		{
			EmitSignal(SignalName.TorsoHoverExit);
		}
	}
}
