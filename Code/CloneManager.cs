using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

public partial class CloneManager : Node2D
{
	[Export]
	private Node2D clone;
	
	// Mouse position and offset
	private Vector2 currentGlobalMousePosition = Vector2.Zero;
	private Vector2 localMouseOffset = Vector2.Zero;
	
	// Resetting, if user moves body part, but doesnt remove it
	private Vector2 initialPosition = Vector2.Zero;
	private Area2D selectedArea = null;
	private Area2D removalBoundsArea = null;
	
	// Movement checks
	private bool moveObject = false;
	private bool isAttachedPartSelected = false;
	private bool isSelectedBodyPartAttached = false;
	
	
	private Area2D currentArea = null;
	private int zIndex = 0;
	
	
	
	public override void _Ready()
	{
		// Only grab the top most object in when selection is overlapping
		GetViewport().PhysicsObjectPickingSort = true;
		GetViewport().PhysicsObjectPickingFirstOnly = true;
		
		
		// Links all children signals in BodyParts.cs
		foreach (Node node in clone.GetChildren())
		{
			if (node is BodyParts part)
			{
				part.UpgradeSiteClicked += OnUpgradeSiteClicked;
				
				part.BodyPartHover += OnBodyPartHover;
				part.BodyPartHoverExit += OnBodyPartHoverExit;
				
				part.TorsoHover += OnTorsoHover;
				part.TorsoHoverExit += OnTorsoHoverExit;
			}
		}
		
		
		// Temp testing dragging
		AddBodyPartToScene("res://Scenes/Body Parts/l_arm.tscn");
		AddBodyPartToScene("res://Scenes/Body Parts/l_arm.tscn");
		AddBodyPartToScene("res://Scenes/Body Parts/l_arm.tscn");
	}
	
	
	public override void _Process(double delta)
	{
		if (currentArea == null)
		{
			return;
		}
		if (moveObject)
		{
			currentArea.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		}
	}
	
	
	public override void _Input(InputEvent @event)
	{	
		if (@event is InputEventMouseMotion motion)
		{
			currentGlobalMousePosition = motion.Position;
		}
		
		// Mouse stops clicking on the upgrade site / attached body part
		if (@event.IsActionReleased("left_click"))
		{
			// Only trigger when object is being actively moved (clicked on), prevent unnecessary calls
			if (moveObject)
			{
				ResetAndDisableMovement();
			}
		}
	}
	
	
	// When a lone unconnected body part is clicked
	// unrelated to when a connected body part is clicked
	private void OnBodyPartClicked(Area2D area)
	{
		GD.Print($"Clicked object: {area.Name}");
		
		if (currentArea == null)
		{
			currentArea = area;
			currentArea.ZIndex = zIndex;
		}
		
		localMouseOffset = area.GetLocalMousePosition();
		currentArea = area;
		moveObject = true;
		isSelectedBodyPartAttached = false;
		
		zIndex++;
		currentArea.ZIndex = zIndex;
	}
	
	
	private void OnBodyPartReleased()
	{
		moveObject = false;
	}
	
	
	// Adds a body part as child to the scene, draggable
	// for adding onto the clone
	// Right click on body part scene e.g. "l_arm.tscn" > Copy Path > paste as sceneDirectory
	private BodyParts AddBodyPartToScene(string sceneDirectory)
	{
		// load and add body part to scene
		var bodyPartScene = GD.Load<PackedScene>(sceneDirectory);
		
		BodyParts part = bodyPartScene.Instantiate<BodyParts>();
		
		AddChild(part);
		
		// link signals to functions
		part.UpgradeSiteClicked += OnBodyPartClicked;
		// part.UpgradeSiteReleased += OnBodyPartReleased;
		
		return part;
	}
	
	
	
	
	// click appropriate type -> hover -> unclick
	// part added
	private void AddBodyPartToClone(Area2D area)
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
	
	// other checks like there needs to be higher part in higherarchy
	// cant add hand without arm
	private void IsValidBodyPartPlacement(Area2D areaToCheck, Area2D areaHolding)
	{
		// is hierarchy preserved?
		
		// i dont know how to do that
		
		// e.g. is hand < forearm < arm
	}
	
	
	
	// Called when an area is clicked
	// For removing body parts
	private void OnUpgradeSiteClicked(Area2D area)
	{
		OnBodyPartClicked(area);
		area.ZIndex = 0;
		
		isAttachedPartSelected = true;
		isSelectedBodyPartAttached = true;
		initialPosition = area.GlobalPosition;
		selectedArea = area;
		
		// Initialize new area2d, for body part removal bounds
		var removalBoundsScene = GD.Load<PackedScene>("res://Scenes/removal_bounds.tscn");
		RemovalBounds removalBounds = removalBoundsScene.Instantiate<RemovalBounds>();
		AddChild(removalBounds);
		
		// Properties
		removalBoundsArea = removalBounds;
		removalBounds.GlobalPosition = currentGlobalMousePosition;
		removalBounds.Name = area.Name;
		removalBounds.MouseExitedBounds += BodyPartRemovalBoundsExit;
		
		GD.Print(area.Name);
	}
	
	
	// Called to reset position and movment for currently selected area
	private void ResetAndDisableMovement()
	{
		moveObject = false;
		isAttachedPartSelected = false;
		
		if (!isSelectedBodyPartAttached)
		{
			return;
		}
		
		// reset position
		selectedArea.GlobalPosition = initialPosition;
		selectedArea = null;
		initialPosition = Vector2.Zero;
		
		// delete removal bounds
		removalBoundsArea.QueueFree();
		removalBoundsArea = null;
	}
	
	
	// Called when the removeBounds area is stopped hovering over with the mouse
	// Removes body part, when mouse is dragged far enough away from the body, part is disconnected
	// Creates a new identical body part seperate from the body
	private void BodyPartRemovalBoundsExit(Area2D area)
	{
		// Clicked down on body part for removal
		if (!isAttachedPartSelected)
		{
			return;
		}
		
		// TODO: some code here that saves to file
		
		
		BodyPartEnum bodyPartEnum = GetBodyPartEnumFromString(area.Name);
		
		// get directory from dictionary
		if (!bodyParts.ContainsKey(bodyPartEnum))
		{
			ResetAndDisableMovement();
			throw new ArgumentNullException($"Key: {bodyPartEnum}, could not be found in bodyParts dictionary");
		}
		
		var partDirectory = bodyParts.GetValueOrDefault(bodyPartEnum);
		
		
		// create new body part object
		BodyParts bodyPart = AddBodyPartToScene(partDirectory);
		
		// delete old object and remove bounds
		currentArea.QueueFree();
		currentArea = null;
		removalBoundsArea.QueueFree();
		removalBoundsArea = null;
		
		// move to mouse position
		bodyPart.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		
		bodyPart.BodyPartHover += OnBodyPartHover;
		bodyPart.BodyPartHoverExit += OnBodyPartHoverExit;
		
		// click onto it (follow the mouse untill unclick signal)
		currentArea = bodyPart;
		moveObject = true;
		isAttachedPartSelected = false;
		isSelectedBodyPartAttached = false;
		
		
		// create new bounds area for adding body part
		BodyParts attachmentArea = AddBodyPartToScene(partDirectory);
		attachmentArea.GlobalPosition = initialPosition;
		attachmentArea.isAttachmentArea = true;
		attachmentArea.sprite2D.Visible = false;
		
		attachmentArea.BodyPartHover += OnBodyPartHover;
		attachmentArea.BodyPartHoverExit += OnBodyPartHoverExit;
		
		GD.Print($"Stopped hovering over {area.Name}");
	}
	
	
	// Called when the mouse enters the attached body part area
	private void OnBodyPartHover(Area2D area)
	{
		// hovering over the area, while a body part is being held / clicked on
		
		// then when release left click, add it
		
		
		// when removing, need to remove the removalBounds area
		// add a new area of equal size to the area just removed, in the original position
		
		// link that areas hover to this function
		
		
	}
	
	private void OnBodyPartHoverExit(Area2D area)
	{
		
	}
	
	
	// Called when the torso is hovered over with the mouse
	private void OnTorsoHover()
	{
		GD.Print("Hovering over torso");
	}
	
	// Called when the torso is stopped hovering over with the mouse
	private void OnTorsoHoverExit()
	{
		GD.Print("Stopped hovering over torso");
	}
	
	
	
	private readonly Dictionary<BodyPartEnum, string> bodyParts = new()
	{
		{BodyPartEnum.LArm,	"res://Scenes/Body Parts/l_arm.tscn"},
		
		// remaining unfinished
		// {BodyPartEnum.LForearm,	"res://Scenes/Body Parts/"}, 
		// {BodyPartEnum.LHand,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RArm,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RForearm,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RHand,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LLeg,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LCalf,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LFoot,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RLeg,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RCalf,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RFoot,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Torso,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Heart,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Lung,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Kidney,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Liver,	"res://Scenes/Body Parts/"}
	};
	
	
	
	public static BodyPartEnum GetBodyPartEnumFromString(string input)
	{
		GD.Print(input);
		
		if (string.IsNullOrEmpty(input))
		{
			throw new ArgumentException("Input cannot be null or empty");
		}
		
		string withoutNumbers = Regex.Replace(input, "[0-9]", "");
		
		if(Enum.TryParse(withoutNumbers, out BodyPartEnum result))
		{
			return result;
		}
		
		throw new ArgumentException($"Invalid input string: {input}");
	}
	
	
	public enum BodyPartEnum
	{
		LArm,
		LForearm,
		LHand,
		
		RArm,
		RForearm,
		RHand,
		
		
		LLeg,
		LCalf,
		LFoot,
		
		RLeg,
		RCalf,
		RFoot,
		
		
		Torso,
		Heart,
		Lung,
		Kidney,
		Liver
	}
	
}
