using Godot;


public partial class Clone : Node2D
{
	
	
	public override void _Ready()
	{
		foreach (Node node in GetChildren())
		{
			if (node is BodyParts part)
			{
				part.UpgradeSiteClicked += OnUpgradeSiteClicked;
				part.TorsoHover += OnTorsoHover;
				part.TorsoHoverExit += OnTorsoHoverExit;
			}
		}
	}
	
	// Called when an area is clicked, "area" parameter is the one that was clicked
	private void OnUpgradeSiteClicked(Area2D area)
	{
		// Temp
		GD.Print(area.Name);
	}
	
	// Play torso opening animation, display internal organs
	private void OnTorsoHover()
	{
		// Temp
		GD.Print("Hovering over torso");
	}
	
	// Play torso closing animation, hide internal organs
	private void OnTorsoHoverExit()
	{
		// Temp
		GD.Print("Stopped hovering over torso");
	}
	
	
	
}
