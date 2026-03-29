using Godot;
using System;

/*

Provides the dragging mouse input controls
For moving around objects with the mouse

*/
public partial class Dragging : Node2D
{
	[Export]
	private Sprite2D sprite;
	
	public Vector2 currentMousePosition = Vector2.Zero;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoveObject(delta, sprite);
	}
	
	
	public override void _Input(InputEvent @event)
	{	
		if (@event is InputEventMouseMotion motion)
		{
			GD.Print($"Motion at coordiate: {motion.Position}");
			
			currentMousePosition = motion.Position;
		}
	}
	
	public void MoveObject(double delta, Sprite2D moveable)
	{
		moveable.Position = currentMousePosition;
	}
	
	// get the position of the mouse
	// if the mouse is clicked on the object
	// move the object to the mouses position
	
	
	// use the inputevent override function signal release to activate dragging
	
	
}
