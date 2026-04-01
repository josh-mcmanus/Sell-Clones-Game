using Godot;
using System;
using System.Collections.Generic;

public partial class CloneManager : Node2D
{
	[Export]
	private Node2D clone;
	
	private const string TAG = "CloneManager";
	
	// Mouse position and offset
	private Vector2 currentGlobalMousePosition = Vector2.Zero;
	private Vector2 localMouseOffset = Vector2.Zero;
	private Vector2 initialPosition = Vector2.Zero;
	
	// Body part removal
	private BodyParts currentBodyPart = null;
	private BodyParts attachmentArea = null;
	private RemovalBounds activeRemovalBounds = null;
	
	// Movement checks
	private bool moveObject = false;
	private bool isAttachedPartSelected = false;
	private bool isSelectedBodyPartAttached = false;
	private bool isAttachmentOk = false;
	
	private int zIndex = 0;
	
	
	
	public override void _Ready()
	{
		// Only grab the top most object in when selection is overlapping
		// NOTE: This activates for the entire game project
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
			}
		}
	}
	
	
	public override void _Process(double delta)
	{
		if (currentBodyPart == null)
		{
			return;
		}
		if (moveObject)
		{
			currentBodyPart.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
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
	
	
	
	
	// Activates when an upgrade site is clicked
	// For removing body parts from the clone
	// NOTE: The same as OnBodyPartClicked, but also initializes the removal process
	private void OnUpgradeSiteClicked(BodyParts bodyPart)
	{
		OnBodyPartClicked(bodyPart);
		bodyPart.ZIndex = 0;
		
		isAttachedPartSelected = true;
		isSelectedBodyPartAttached = true;
		initialPosition = bodyPart.GlobalPosition;
		currentBodyPart = bodyPart;
		
		// Initialize new area2d, for body part removal bounds
		var removalBoundsScene = GD.Load<PackedScene>("res://Scenes/removal_bounds.tscn");
		RemovalBounds removalBounds = removalBoundsScene.Instantiate<RemovalBounds>();
		AddChild(removalBounds);
		
		// Properties
		activeRemovalBounds = removalBounds;
		removalBounds.GlobalPosition = currentGlobalMousePosition;
		removalBounds.bodyPartEnum = bodyPart.bodyPartEnum;
		removalBounds.MouseExitedBounds += BodyPartRemovalBoundsExit;
	}
	
	
	// Activates when a body part is clicked on
	// For moving the selected body part around
	// NOTE: OnUpgradeSiteClicked is similar, and also calls this function, but also does other things relevant for ONLY the upgrade sites. e.g. the sites on the body where you can add upgrades to
	private void OnBodyPartClicked(BodyParts area)
	{
		Utils.Log(TAG, $"Clicked object type: {area.bodyPartEnum}");
		
		if (currentBodyPart == null)
		{
			currentBodyPart = area;
			currentBodyPart.ZIndex = zIndex;
		}
		
		localMouseOffset = area.GetLocalMousePosition();
		currentBodyPart = area;
		moveObject = true;
		isSelectedBodyPartAttached = false;
		currentBodyPart.InputPickable = false;
		
		zIndex++;
		currentBodyPart.ZIndex = zIndex;
	}
	
	
	// Activates when the player releases left mouse click
	// For attaching the selected body part onto the clone
	private void OnBodyPartReleased()
	{
		if (isAttachmentOk)
		{
			Utils.Log(TAG, "Attachment is ok");
			
			// Move held body part into correct position
			currentBodyPart.GlobalPosition = attachmentArea.GlobalPosition;
			
			// Set click to activate removal process
			currentBodyPart.UpgradeSiteClicked += OnUpgradeSiteClicked;
			
			// Reset for next body part
			isAttachmentOk = false;
			
			// Delete attachment area
			attachmentArea.QueueFree();
			attachmentArea = null;
		}
	}
	
	
	
	
	// Activates when the mouse enters the body part attachment area
	// For attaching new body parts
	private void OnBodyPartHover(BodyParts hoverBodyPart)
	{
		// Disable activation for attached body parts
		if (!hoverBodyPart.isAttachmentArea)
		{
			return;
		}
		
		// Disable activation if body part is not being dragged
		if (!moveObject)
		{
			return;
		}
		
		// Check both areas are the same type
		var heldArea = currentBodyPart;
		
		if (hoverBodyPart.bodyPartEnum == heldArea.bodyPartEnum)
		{
			Utils.Log(TAG, "Attachment area and held item match");
			
			isAttachmentOk = true;
			attachmentArea = hoverBodyPart;
		}
	}
	
	
	// Activates when the mouse exits any body part region
	// For disabling attachment when not hovering over a body part attachment area
	private void OnBodyPartHoverExit(BodyParts bodyPart)
	{
		// Disable activation if body part is not being dragged
		if (!moveObject)
		{
			return;
		}
		
		isAttachmentOk = false;
	}
	
	
	// Activates when the mouse exits the removeBounds area
	// For removing body parts
	// Creates identical body part on removal, seperated from clone, deletes old attached part
	private void BodyPartRemovalBoundsExit(RemovalBounds removalBounds)
	{
		// Clicked down on body part for removal
		if (!isAttachedPartSelected)
		{
			return;
		}
		
		// TODO: some code here that saves to file
		
		
		// Get file directory
		if (!Utils.bodyPartsDictionary.ContainsKey(removalBounds.bodyPartEnum))
		{
			ResetAndDisableMovement();
			throw new ArgumentNullException($"Key: {removalBounds.bodyPartEnum}, could not be found in bodyParts dictionary");
		}
		
		var partDirectory = Utils.bodyPartsDictionary.GetValueOrDefault(removalBounds.bodyPartEnum);
		
		
		// Create new body part object
		BodyParts bodyPart = AddBodyPartToScene(partDirectory);
		bodyPart.bodyPartEnum = removalBounds.bodyPartEnum;
		
		// Delete old body part and removalbounds
		currentBodyPart.QueueFree();
		currentBodyPart = null;
		activeRemovalBounds.QueueFree();
		activeRemovalBounds = null;
		
		// Move to current mouse position
		bodyPart.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		
		bodyPart.BodyPartHover += OnBodyPartHover;
		bodyPart.BodyPartHoverExit += OnBodyPartHoverExit;
		
		// Follow the mouse until unclick signal
		currentBodyPart = bodyPart;
		moveObject = true;
		isAttachedPartSelected = false;
		isSelectedBodyPartAttached = false;
		
		
		// Create bounds area for attaching new body part
		BodyParts attachmentArea = AddBodyPartToScene(partDirectory);
		attachmentArea.GlobalPosition = initialPosition;
		attachmentArea.isAttachmentArea = true;
		attachmentArea.sprite2D.Visible = false;
		
		attachmentArea.BodyPartHover += OnBodyPartHover;
		attachmentArea.BodyPartHoverExit += OnBodyPartHoverExit;
	}
	
	
	
	
	// Adds a draggable body part to the scene
	// NOTE: To get "sceneDirectory" - right click on body part scene in the file system in Godot, e.g. "l_arm.tscn" > Copy Path > paste as sceneDirectory
	private BodyParts AddBodyPartToScene(string sceneDirectory)
	{
		// Load and add to scene
		var bodyPartScene = GD.Load<PackedScene>(sceneDirectory);
		var part = bodyPartScene.Instantiate<BodyParts>();
		AddChild(part);
		
		// link signals to functions
		part.UpgradeSiteClicked += OnBodyPartClicked;
		part.UpgradeSiteReleased += OnBodyPartReleased;
		
		return part;
	}
	
	
	// Call to reset everything
	private void ResetAndDisableMovement()
	{
		moveObject = false;
		isAttachedPartSelected = false;
		
		if (currentBodyPart != null)
		{
			currentBodyPart.InputPickable = true; // restore movement
		}
		
		if (!isSelectedBodyPartAttached)
		{
			return;
		}
		
		// Position
		currentBodyPart.GlobalPosition = initialPosition;
		currentBodyPart = null;
		initialPosition = Vector2.Zero;
		
		// Mouse
		localMouseOffset = Vector2.Zero;
		
		// Delete Removal Bounds
		activeRemovalBounds.QueueFree();
		activeRemovalBounds = null;
	}
	
	
}
