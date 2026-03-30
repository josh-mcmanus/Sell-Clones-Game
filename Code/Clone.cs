using System.Collections.Generic;
using Godot;


public partial class Clone : Node2D
{
	// private CollisionShape2D torso;
	private Vector2 torsoSize;
	private Vector2 currentGlobalMousePosition = Vector2.Zero;
	private Vector2 max = Vector2.Zero;
	private Vector2 min = Vector2.Zero;
	
	
	public override void _Ready()
	{
		// Links all children signals in BodyParts.cs
		// foreach (Node node in GetChildren())
		// {
		// 	if (node is BodyParts part)
		// 	{
		// 		part.UpgradeSiteClicked += OnUpgradeSiteClicked;
				
		// 		part.BodyPartHover += BodyPartHover;
		// 		part.BodyPartHoverExit += BodyPartHoverExit;
				
		// 		part.TorsoHover += OnTorsoHover;
		// 		part.TorsoHoverExit += OnTorsoHoverExit;
		// 	}
		// }
	}

	
	// Called when an area is clicked
	// private void OnUpgradeSiteClicked(Area2D area)
	// {
	// 	GD.Print(area.Name);
	// }
	
	
	// part removed
	// hover -> click -> stop hover
	
	// click appropriate type -> hover -> unclick
	// part added
	
	// other checks like there needs to be higher part in higherarchy
	// cant add hand without arm
	
	private void RemoveBodyPart(Area2D area)
	{
		// player clicks on attached body part
		
		// player moves mouse outside of area
		
		// function is called
		
		// remove body part from clone 
			// delete from file
			// create new body part object that is clicked on at same position on mouse
	}
	
	
	
	private void AddBodyPart(Area2D area)
	{
		// check if player is holding part
		
		// player hovers over area
		
		// player releases mouse
		
		// call this function
		
		// check that both parts match
		
		// if they do, add it
			// set the position to a set location
			// lerp it to that position
			// save the setup to a file
	}
	
	private void IsValidBodyPartPlacement()
	{
		// is hierarchy preserved?
		
		// i dont know how to do that
		
		// e.g. is hand < forearm < arm
	}
	
	
	// Detect when part is added
	// private void BodyPartHover(Area2D area)
	// {
	// 	GD.Print($"Hovering over {area.Name}");
	// }
	
	// Detect when part is removed
	// private void BodyPartHoverExit(Area2D area)
	// {
	// 	GD.Print($"Stopped hovering over {area.Name}");
	// }
	
	
	// Play torso opening animation, display internal organs
	// private void OnTorsoHover()
	// {
	// 	GD.Print("Hovering over torso");
	// }
	
	// Play torso closing animation, hide internal organs
	// private void OnTorsoHoverExit()
	// {
	// 	GD.Print("Stopped hovering over torso");
	// }
	
	
	
}
