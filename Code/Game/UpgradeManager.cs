using Godot;
using System;
using System.Linq;

public partial class UpgradeManager : Node2D
{
	[Export]
	private Node2D partsRoot;
	
	private const string TAG = "UpgradeManager";
	
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
	private bool isSelectedUpgradePartAttached = false;
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
		
		// Links all children signals in GenericParts.cs
		foreach (Node node in partsRoot.GetChildren())
		{
			if (node is GenericPart part)
			{
				if (part.isAttachmentArea)
				{
					part.sprite2D.Visible = false; // Attachment area hidden by default
				}
				
				part.UpgradeSiteClicked += DetatchPart;
			}
		}
		
		
		// Add previously attached parts
		foreach (AttachedPartData attachedPart in DataManager.saveData.AttachedParts.ToList())
		{
			var existing = FindMatchingPart(attachedPart.Part, attachedPart.Side);
			
			if(existing == null)
				continue;
			
			// Add attached part
			currentPart = AddPartToScene(
				Utils.UpgradePartsFileLocation[attachedPart.Part], 
				attachedPart.Side, 
				attachedPart.Tier
			);
			
			currentPart.part = attachedPart.Part;
			attachmentArea = existing;
			isAttachmentOk = true;
			
			AttachPart();
			
			// Delete placeholder
			existing.QueueFree();
		}
		
	}
	
	
	public override void _Process(double delta)
	{
		HandleLabelMovement();
		
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
	
	
	// Part info on hover
	private void HandleLabelMovement()
	{
		foreach (var label in GetChildren().OfType<UpgradePartHoverLabel>())
		{
			label.GlobalPosition = GetGlobalMousePosition();
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
				&& attachmentPart.part == currentPart.part
				&& attachmentPart.side == currentPart.side)
			{
				isAttachmentOk = true;
				attachmentArea = attachmentPart;
				break;
			}
		}
	}
	
	
	// Set selected held body part side based on mouse position
	public void HandlePartFlipping()
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
	private void AttachPart()
	{
		if (isAttachmentOk)
		{
			Utils.Log(TAG, "Attachment is ok");
			
			AddSaveDataAttachedPart(currentPart);
			
			// Properties
			currentPart.GlobalPosition = attachmentArea.GlobalPosition;
			currentPart.UpgradeSiteClicked += DetatchPart;
			
			// Reset
			isAttachmentOk = false;
			attachmentArea.QueueFree();
			attachmentArea = null;
		}
		
		currentPart = null;
	}
	
	
	// Activates when an attached body part is clicked
	// For removing body parts from the clone
		// NOTE: The same as MovePart, but also initializes the removal process
	private void DetatchPart(GenericPart upgradePart)
	{
		MovePart(upgradePart);
		upgradePart.ZIndex = 0;
		
		isAttachedPartSelected = true;
		isSelectedUpgradePartAttached = true;
		initialPosition = upgradePart.GlobalPosition;
		currentPart = upgradePart;
		
		// Initialize new area2d, for body part removal bounds
		var removalBoundsScene = GD.Load<PackedScene>("res://Scenes/Clone/removal_bounds.tscn");
		RemovalBounds removalBounds = removalBoundsScene.Instantiate<RemovalBounds>();
		partsRoot.AddChild(removalBounds);
		
		// Properties
		activeRemovalBounds = removalBounds;
		removalBounds.GlobalPosition = currentGlobalMousePosition;
		removalBounds.part = upgradePart.part;
		removalBounds.side = upgradePart.side;
		removalBounds.tier = upgradePart.tier;
		removalBounds.MouseExitedBounds += PartRemovalBoundsExit;
	}
	
	
	// Activates when a body part is clicked on
	// For allowing mouse dragging movement of the selected body part
		// NOTE: DetatchPart is similar, and also calls this function, but does other things relevant for ONLY the upgrade sites. 
	private void MovePart(GenericPart upgradePart)
	{
		Utils.Log(TAG, $"Clicked object type: {upgradePart.part}");
		
		// torso cant move
		if (upgradePart.part == Utils.UpgradePart.Torso)
			return;
		
		if (currentPart == null)
		{
			currentPart = upgradePart;
			currentPart.ZIndex = zIndex;
		}
		
		localMouseOffset = upgradePart.GlobalPosition - currentGlobalMousePosition;
		currentPart = upgradePart;
		moveObject = true;
		isSelectedUpgradePartAttached = false;
		currentPart.InputPickable = false;
		
		zIndex++;
		currentPart.ZIndex = zIndex;
	}
	
	
	
	// Activates when the mouse exits the removeBounds area
	// For removing body parts
	// Creates identical body part on removal, seperated from clone, deletes old attached part
	private void PartRemovalBoundsExit(RemovalBounds removalBounds)
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
			partDirectory = Utils.GetUpgradePartDirectory(removalBounds.part);
		}
		catch
		{
			ResetAndDisableMovement();
		}
		
		
		// Create new body part object
		GenericPart upgradePart = AddPartToScene(partDirectory, removalBounds.side, removalBounds.tier);
		upgradePart.part = removalBounds.part;
		upgradePart.tier = removalBounds.tier;
		
		GD.Print(removalBounds.part);
		
		// Delete old body part and removalbounds
		RemoveSaveDataAttachedPart(currentPart);
		currentPart.QueueFree();
		currentPart = null;
		activeRemovalBounds.QueueFree();
		activeRemovalBounds = null;
		
		// Move to current mouse position
		upgradePart.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		
		// Follow the mouse until unclick signal
		currentPart = upgradePart;
		moveObject = true;
		isAttachedPartSelected = false;
		isSelectedUpgradePartAttached = false;
		
		
		// Create bounds area for attaching new body part
		GenericPart attachmentArea = AddPartToScene(partDirectory, removalBounds.side, removalBounds.tier);
		attachmentArea.GlobalPosition = initialPosition;
		attachmentArea.sprite2D.Visible = false;
		
		attachmentArea.isAttachmentArea = true;
		attachmentArea.part = removalBounds.part;
		attachmentArea.side = removalBounds.side;
		attachmentArea.tier = removalBounds.tier;
		
	}
	
	
	// Adds a draggable body part to the scene
		// NOTE: To get "sceneDirectory" - right click on body part scene in the file system in Godot, e.g. "shoulder.tscn" > Copy Path > paste as sceneDirectory
	public GenericPart AddPartToScene(string sceneDirectory, Utils.Side side, Utils.UpgradeTier tier)
	{
		// Load and add to scene
		var upgradePartScene = GD.Load<PackedScene>(sceneDirectory);
		var part = upgradePartScene.Instantiate<GenericPart>();
		partsRoot.AddChild(part);
		
		// Flip on right
		if (side == Utils.Side.Right)
			part.Scale = new Vector2(-1, 1);
		
		part.side = side;
		part.tier = tier;
		
		var sprite = part.GetNode<Sprite2D>("Sprite2D");
		sprite.Modulate = Utils.UpgradePartUpgradeTierColors[tier];
		
		// link signals to functions
		part.UpgradeSiteClicked += MovePart;
		part.UpgradeSiteReleased += AttachPart;
		part.UpgradePartHoverEnter += CreateHoverLabel;
		part.UpgradePartHoverExit += DeleteHoverLabel;
		
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
		
		if (!isSelectedUpgradePartAttached)
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
	
	
	// --- Attaching Parts ---
	
	// Adds newly attached part to save data list of attached parts
	private static void AddSaveDataAttachedPart(GenericPart part)
	{
		// Remove existing part if it exists
		DataManager.saveData.AttachedParts.RemoveAll(p => p.Part == part.part && p.Side == part.side);
		
		// Add new
		DataManager.saveData.AttachedParts.Add(new AttachedPartData(part.part, part.side, part.tier));
	}
	
	
	// Removes detached parts from save data list of attached parts
	private static void RemoveSaveDataAttachedPart(GenericPart part)
	{
		DataManager.saveData.AttachedParts.RemoveAll( p 
			=> p.Part == part.part
			&& p.Side == part.side
			&& p.Tier == part.tier
		);
	}
	
	
	// Finds specified matching body part in scene, or null
	private GenericPart FindMatchingPart(Utils.UpgradePart upgradePart, Utils.Side side)
	{
		foreach (Node node in partsRoot.GetChildren())
		{
			if (node is GenericPart genericPart)
			{
				if (genericPart.part == upgradePart && genericPart.side == side)
				{
					return genericPart;
				}
			}
		}
		
		return null;
	}
	
	
	// --- Labels ---
	
	// Mostly copied from ResearchTab AddTempLabel, because it wouldnt let me put it inside of Utils
	// Or use it calling the function from here as its not static
	// TODO: Fix this not displaying the labels on top
	// TODO: Fix the scale being off, it is way bigger than it should be, look at button hover info for correct size
		// might be because the clone is scaled up 1.5x?
	private void CreateHoverLabel(GenericPart part)
	{
		// Dont display label if dragging
		if(moveObject)
			return;
		
		GD.Print("Signal go");
		
		
		PackedScene labelScene = GD.Load<PackedScene>("res://Scenes/UI/upgrade_part_hover_label.tscn");
		UpgradePartHoverLabel label = labelScene.Instantiate<UpgradePartHoverLabel>();
		
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		label.Text = "Hello";
		
		var settings = new LabelSettings
		{
			FontSize = 20,
			FontColor = Colors.White,
			
			OutlineSize = 8,
			OutlineColor = Colors.Black,
		};
		
		label.LabelSettings = settings;
		label.HoverTimerTimeout += DisplayHoverLabel;
		
		AddChild(label);
		
		label.Position = Vector2.Zero;
		label.Visible = false;
		label.ZIndex = 1;
		
		GD.Print("label created");
	}
	
	
	// Mostly Copied from ResearchTab OnMouseExitedButton
	private void DeleteHoverLabel(Area2D area)
	{
		foreach (var label in GetChildren().OfType<UpgradePartHoverLabel>())
		{
			GD.Print("label deleted");
			
			label.QueueFree();
		}
	}
	
	
	// Displays the hover label after its timer times out
	// The player has to hover for x seconds before info is displayed
	private void DisplayHoverLabel(UpgradePartHoverLabel label)
	{
		GD.Print("label displayed");
		label.Visible = true;
	}
	
	
	
}
