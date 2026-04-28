using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
	private GenericPart currentPart = null;
	private GenericPart attachmentArea = null;
	private RemovalBounds activeRemovalBounds = null;
	
	// Movement checks
	private bool moveObject = false;
	private bool isAttachedPartSelected = false;
	private bool isSelectedBodyPartAttached = false;
	private bool isAttachmentOk = false;
	
	private int zIndex = 0;
	private float offsetPosition;
	private float centerPosition;
	
	
	
	public override void _Ready()
	{
		// Only grab the top most object in when selection is overlapping
		// NOTE: This activates for the entire game project
		GetViewport().PhysicsObjectPickingSort = true;
		GetViewport().PhysicsObjectPickingFirstOnly = true;
		
		centerPosition = GetViewport().GetVisibleRect().Size.X / 2;
		
		// Links all children signals in BodyParts.cs
		foreach (Node node in clone.GetChildren())
		{
			if (node is GenericPart part)
			{
				if (part.isAttachmentArea)
				{
					part.sprite2D.Visible = false; // Attachment area hidden by default
				}
				
				part.UpgradeSiteClicked += DetatchBodyPart;
			}
		}
		
		
		// Add previously attached parts
		foreach (AttachedPartData attachedPart in DataManager.saveData.AttachedParts.ToList())
		{
			var existing = FindMatchingPart(attachedPart.Part, attachedPart.Side);
			
			if(existing == null)
				continue;
			
			// Add attached part
			currentPart = AddBodyPartToScene(
				Utils.bodyPartsFileLocation[attachedPart.Part], 
				attachedPart.Side, 
				attachedPart.Tier
			);
			
			currentPart.bodyPart = attachedPart.Part;
			attachmentArea = existing;
			isAttachmentOk = true;
			
			AttachBodyPart();
			
			// Delete placeholder
			existing.QueueFree();
		}
		
	}
	
	
	public override void _Process(double delta)
	{
		if (currentPart == null)
		{
			return;
		}
		
		if (moveObject)
		{
			currentPart.GlobalPosition = currentGlobalMousePosition + localMouseOffset;
		}
		
		HandleAttachment();
		HandlePartFlipping();
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
	
	
	
	// --- Body Part Handling ---
	
	// Checks for body part attachment to clone
	private void HandleAttachment()
	{
		var overlap = currentPart.GetOverlappingAreas();
		
		isAttachmentOk = false;
		attachmentArea = null;
		
		foreach (var area in overlap)
		{
			// Valid
			if (area is GenericPart attachmentPart 
				&& attachmentPart.isAttachmentArea 
				&& attachmentPart.bodyPart == currentPart.bodyPart
				&& attachmentPart.side == currentPart.side)
			{
				isAttachmentOk = true;
				attachmentArea = attachmentPart;
				break;
			}
		}
	}
	
	
	// Set selected held body part side based on mouse position
	private void HandlePartFlipping()
	{
		if (moveObject && currentPart != null)
		{
			if (currentGlobalMousePosition.X < centerPosition) // mouse is left of center
			{
				currentPart.side = Utils.Side.Left;
				currentPart.Scale = new Vector2(1, 1);
			}
			else
			{
				currentPart.side = Utils.Side.Right;
				currentPart.Scale = new Vector2(-1, 1);
			}
		}
	}
	
	
	// Activates when releasing left click over a matching attachment area
	// Attaches the selected body part onto the matching area of the clone
	private void AttachBodyPart()
	{
		if (isAttachmentOk)
		{
			Utils.Log(TAG, "Attachment is ok");
			
			AddSaveDataAttachedPart(currentPart);
			
			// Properties
			currentPart.GlobalPosition = attachmentArea.GlobalPosition;
			currentPart.UpgradeSiteClicked += DetatchBodyPart;
			
			// Reset
			isAttachmentOk = false;
			attachmentArea.QueueFree();
			attachmentArea = null;
		}
		
		currentPart = null;
	}
	
	
	// Activates when an attached body part is clicked
	// For removing body parts from the clone
		// NOTE: The same as MoveBodyPart, but also initializes the removal process
	private void DetatchBodyPart(GenericPart bodyPart)
	{
		MoveBodyPart(bodyPart);
		bodyPart.ZIndex = 0;
		
		isAttachedPartSelected = true;
		isSelectedBodyPartAttached = true;
		initialPosition = bodyPart.GlobalPosition;
		currentPart = bodyPart;
		
		// Initialize new area2d, for body part removal bounds
		var removalBoundsScene = GD.Load<PackedScene>("res://Scenes/Clone/removal_bounds.tscn");
		RemovalBounds removalBounds = removalBoundsScene.Instantiate<RemovalBounds>();
		AddChild(removalBounds);
		
		// Properties
		activeRemovalBounds = removalBounds;
		removalBounds.GlobalPosition = currentGlobalMousePosition;
		removalBounds.bodyPart = bodyPart.bodyPart;
		removalBounds.side = bodyPart.side;
		removalBounds.tier = bodyPart.tier;
		removalBounds.MouseExitedBounds += BodyPartRemovalBoundsExit;
	}
	
	
	// Activates when a body part is clicked on
	// For allowing mouse dragging movement of the selected body part
		// NOTE: DetatchBodyPart is similar, and also calls this function, but does other things relevant for ONLY the upgrade sites. 
	private void MoveBodyPart(GenericPart area)
	{
		Utils.Log(TAG, $"Clicked object type: {area.bodyPart}");
		
		// torso cant move
		if (area.bodyPart == Utils.BodyPart.Torso)
			return;
		
		if (currentPart == null)
		{
			currentPart = area;
			currentPart.ZIndex = zIndex;
		}
		
		localMouseOffset = area.GlobalPosition - currentGlobalMousePosition;
		currentPart = area;
		moveObject = true;
		isSelectedBodyPartAttached = false;
		currentPart.InputPickable = false;
		
		zIndex++;
		currentPart.ZIndex = zIndex;
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
		
		string partDirectory = "";
		
		// Get file directory
		try
		{
			partDirectory = Utils.GetBodyPartDirectory(removalBounds.bodyPart);
		}
		catch
		{
			ResetAndDisableMovement();
		}
		
		
		// Create new body part object
		GenericPart bodyPart = AddBodyPartToScene(partDirectory, removalBounds.side, removalBounds.tier);
		bodyPart.bodyPart = removalBounds.bodyPart;
		bodyPart.tier = removalBounds.tier;
		
		GD.Print(removalBounds.bodyPart);
		
		// Delete old body part and removalbounds
		RemoveSaveDataAttachedPart(currentPart);
		currentPart.QueueFree();
		currentPart = null;
		activeRemovalBounds.QueueFree();
		activeRemovalBounds = null;
		
		// Move to current mouse position
		bodyPart.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		
		// Follow the mouse until unclick signal
		currentPart = bodyPart;
		moveObject = true;
		isAttachedPartSelected = false;
		isSelectedBodyPartAttached = false;
		
		
		// Create bounds area for attaching new body part
		GenericPart attachmentArea = AddBodyPartToScene(partDirectory, removalBounds.side, removalBounds.tier);
		attachmentArea.GlobalPosition = initialPosition;
		attachmentArea.sprite2D.Visible = false;
		
		attachmentArea.isAttachmentArea = true;
		attachmentArea.bodyPart = removalBounds.bodyPart;
		attachmentArea.side = removalBounds.side;
		attachmentArea.tier = removalBounds.tier;
		
	}
	
	
	
	
	// Adds a draggable body part to the scene
		// NOTE: To get "sceneDirectory" - right click on body part scene in the file system in Godot, e.g. "shoulder.tscn" > Copy Path > paste as sceneDirectory
	public GenericPart AddBodyPartToScene(string sceneDirectory, Utils.Side side, Utils.UpgradeTier tier)
	{
		// Load and add to scene
		var bodyPartScene = GD.Load<PackedScene>(sceneDirectory);
		var part = bodyPartScene.Instantiate<GenericPart>();
		AddChild(part);
		
		// Flip on right
		if (side == Utils.Side.Right)
			part.Scale = new Vector2(-1, 1);
		
		part.side = side;
		part.tier = tier;
		
		var sprite = part.GetNode<Sprite2D>("Sprite2D");
		sprite.Modulate = Utils.bodyPartUpgradeTierColors[tier];
		
		// link signals to functions
		part.UpgradeSiteClicked += MoveBodyPart;
		part.UpgradeSiteReleased += AttachBodyPart;
		
		zIndex++;
		part.ZIndex = zIndex;
		
		return part;
	}
	
	
	// Call to reset everything
	private void ResetAndDisableMovement()
	{
		moveObject = false;
		isAttachedPartSelected = false;
		
		if (currentPart != null)
		{
			currentPart.InputPickable = true; // restore movement
		}
		
		if (!isSelectedBodyPartAttached)
		{
			return;
		}
		
		// Position
		currentPart.GlobalPosition = initialPosition;
		currentPart = null;
		initialPosition = Vector2.Zero;
		
		// Mouse
		localMouseOffset = Vector2.Zero;
		
		// Delete Removal Bounds
		activeRemovalBounds.QueueFree();
		activeRemovalBounds = null;
	}
	
	
	// Adds newly attached part to save data list of attached parts
	private static void AddSaveDataAttachedPart(GenericPart part)
	{
		// Remove existing part if it exists
		DataManager.saveData.AttachedParts.RemoveAll(p => p.Part == part.bodyPart && p.Side == part.side);
		
		// Add new
		DataManager.saveData.AttachedParts.Add(new AttachedPartData(part.bodyPart, part.side, part.tier));
	}
	
	
	// Removes detached parts from save data list of attached parts
	private static void RemoveSaveDataAttachedPart(GenericPart part)
	{
		DataManager.saveData.AttachedParts.RemoveAll( p 
			=> p.Part == part.bodyPart
			&& p.Side == part.side
			&& p.Tier == part.tier
		);
	}
	
	
	// Finds specified matching body part in scene, or null
	private GenericPart FindMatchingPart(Utils.BodyPart part, Utils.Side side)
	{
		foreach (Node node in clone.GetChildren())
		{
			if (node is GenericPart bodyPart)
			{
				if (bodyPart.bodyPart == part && bodyPart.side == side)
				{
					return bodyPart;
				}
			}
		}
		
		return null;
	}
	
}
