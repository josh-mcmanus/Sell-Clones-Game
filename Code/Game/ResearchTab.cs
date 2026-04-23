using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ResearchTab : Control
{
	
	[Export]
	public Utils.NodeType nodeType;
	
	[Export]
	private Control control;
	
	[Export]
	private Button button;
	
	
	private const string TAG = "ResearchTab";
	
	PackedScene bodyPartScene = GD.Load<PackedScene>("res://Scenes/Game/generic_node.tscn");
	
	double nodeCost = 0;
	double nodeCostMultiplier = 1.5f;
	
	float spriteWidth = 0;
	float spriteHeight = 0;
	float marginX = 0;
	float marginY = 0;
	
	int areaX = 0;
	int areaY = 0;
	int rows = 6;
	int columns = 25;
	int buttonWidth = 60;
	
	int researchNodeCounter = 0;
	
	bool areAnyResearchNodesAdded = false;
	bool isLeftSide;
	
	
	public override void _Ready()
	{
		// Initialize research node
		var node = bodyPartScene.Instantiate<Node2D>();
		
		// Save dimensions for margin
		Sprite2D researchSprite = node.GetNode<Sprite2D>("Sprite2D");
		spriteWidth = researchSprite.Texture.GetWidth();
		spriteHeight = researchSprite.Texture.GetHeight();
		
		// Delete node
		node.QueueFree();
		
		
		
		// Set number of research nodes based on research tab type
		if (nodeType == Utils.NodeType.Brain || nodeType == Utils.NodeType.Tank)
		{
			// TODO: Remember to scale down the research cost for these two
			columns = 17;
		}
		if (nodeType == Utils.NodeType.Clone)
		{
			rows = 12;
			columns = 30;
		}
		
		nodeCost = Utils.nodeCostDictionary[nodeType];
		
		
		
		// Set dimensions
		ColorRect parent = (ColorRect)GetParent();
		Vector2 areaSize = parent.Size;
		
		areaX = (int)areaSize.X;
		areaY = (int)areaSize.Y;
		
		marginX = (areaX - buttonWidth - (spriteWidth * columns)) / (columns + 2);
		marginY = (areaY - (spriteHeight * rows)) / (rows + 1);

		button.CustomMinimumSize = new Vector2(buttonWidth, areaY - marginY * 2);
		
		
		// Set button position
		if (nodeType == Utils.NodeType.Limbs || nodeType == Utils.NodeType.Brain)
		{
			isLeftSide = true;
			button.Position = new Vector2(marginX, marginY);
		}
		else
		{
			isLeftSide = false;
			button.Position = new Vector2(areaX - buttonWidth - marginX, marginY);
		}
		
		
		
		button.Pressed += OnButtonPressed;
		button.MouseEntered += OnMouseEnteredButton;
		button.MouseExited += OnMouseExitedButton;
		
	}
	
	
	public override void _Process(double delta)
	{
		// Adjust labels
		foreach (var label in GetChildren().OfType<HoverInfoLabel>())
		{
			// Price info on hover
			if (label.labelType == HoverInfoLabel.LabelType.InfoLabel)
			{
				var mousePos = GetLocalMousePosition();
				
				var offsetX = isLeftSide ? 0 : -label.Size.X;
				var offsetY = -label.Size.Y;
				
				label.Position = mousePos + new Vector2(offsetX, offsetY);
			}
			
			// Warning on click
			if (label.labelType == HoverInfoLabel.LabelType.WarningLabel)
			{
				AddFadeOutLabel(false, label);
			}
			
			// Points added on node timout
			if (label.labelType == HoverInfoLabel.LabelType.PointsLabel)
			{
				AddFadeOutLabel(true, label);
			}
		}
		
		
		// Add research nodes on load
		if ((researchNodeCounter < (columns * rows)) && (researchNodeCounter < DataManager.saveData.UnlockedNodes[nodeType]))
		{
			AddGenericNode();
		}
	}
	
	
	// Displays hover label of research node price
	private void OnMouseEnteredButton()
	{
		// All nodes purchased
		if (researchNodeCounter >= (columns * rows))
		{
			return;
		}
		
		string msg = $"${Utils.FormatNumber(nodeCost)}";
		Color color = Colors.White;
		
		AddTempLabel(msg, color, HoverInfoLabel.LabelType.InfoLabel, null);
	}
	
	
	// Removes hover label of research node price
	private void OnMouseExitedButton()
	{
		foreach (var label in GetChildren().OfType<HoverInfoLabel>())
		{
			if (label.labelType == HoverInfoLabel.LabelType.InfoLabel)
			{
				label.QueueFree();
			}
		}
	}
	
	
	// Activates on add research node button press
	// Checks if the user can add a research node
	private void OnButtonPressed()
	{
		// All nodes added
		if (researchNodeCounter >= (columns * rows))
		{
			var text = "All research nodes purchased";
			var color = Colors.SkyBlue;
			
			AddTempLabel(text, color, HoverInfoLabel.LabelType.WarningLabel, null);
			
			return;
		}
		
		// Not enough money
		if (DataManager.saveData.Money < nodeCost)
		{
			var text = "Not enough money";
			var color = Colors.Red;
			
			AddTempLabel(text, color, HoverInfoLabel.LabelType.WarningLabel, null);
			
			return;
		}
		
		// Update data
		DataManager.saveData.UnlockedNodes[nodeType] += 1;
		DataManager.saveData.Money -= nodeCost;
		
		AddGenericNode();
	}
	
	
	// Activates on function call
	// For Adding one research unit to the research tab
	private void AddGenericNode()
	{
		// gives the index for the position in the current row, 0-4
		int rowIndex = researchNodeCounter % rows;
		
		// gives the index for the position of the current column 0-20
		int columnIndex = researchNodeCounter / rows;
		
		// Load and add body part to scene
		var part = bodyPartScene.Instantiate<Node2D>();
		AddChild(part);
		
		
		// Connect signal
		GenericNode node = (GenericNode)part;
		node.ResearchUnitComplete += AddPoints;
		
		
		// Set color based on type
		if (!Utils.colorDictionary.ContainsKey(nodeType))
		{
			throw new ArgumentNullException($"Key: {nodeType}, could not be found in colorDictionary");
		}
		else
		{
			Color typeColor = Utils.colorDictionary.GetValueOrDefault(nodeType);
			node.nodeColor.Color = typeColor;
		}
		
		
		// calculate position
		float xOffset = isLeftSide ? buttonWidth + marginX : 0;
		
		float x = xOffset + marginX + columnIndex * (spriteWidth + marginX);
		float y = marginY + rowIndex * (spriteHeight + marginY);
		
		part.Position = new Vector2(x, y);
		part.Position += new Vector2(spriteWidth / 2, spriteHeight / 2);
		researchNodeCounter++;
		
		// Update cost
		nodeCost = Math.Round(nodeCost * nodeCostMultiplier, 2);
		
		
		// Update hover label message with new price
		foreach (var label in GetChildren().OfType<HoverInfoLabel>())
		{
			if (label.labelType == HoverInfoLabel.LabelType.InfoLabel)
			{
				label.Text = $"${Utils.FormatNumber(nodeCost)}";
			}
		}
	}
	
	
	// Activated when a research units timer times out
	// For adding 1 research unit to the global counter
	private void AddPoints(GenericNode node)
	{
		double modifier = 0;
		double multiplier = 1;
		
		double cloneBase = 10;
		double researchBase = 1;
		double points;
		
		Color color;
		
		// TODO: Add calculation based on equiped upgrades
		// tank, brain
		
		
		// Add to player data
		if (nodeType == Utils.NodeType.Clone)
		{
			color = new Color(0.161f, 0.6f, 0.0f);
			points = (cloneBase + modifier) * multiplier;
			DataManager.saveData.Money += points;
		}
		else
		{
			color = Colors.SkyBlue;
			points = (researchBase + modifier) * multiplier;
			DataManager.saveData.ResearchPoints[nodeType] += points;
		}
		
		// Display point increase
		AddTempLabel($"+{points}", color, HoverInfoLabel.LabelType.PointsLabel, node);
	}
	
	
	private void AddTempLabel(string labelContent, Color textColor, HoverInfoLabel.LabelType type, GenericNode? node)
	{
		HoverInfoLabel label = new()
		{
			MouseFilter = MouseFilterEnum.Ignore,
			Text = labelContent,
		};

		var settings = new LabelSettings
		{
			FontSize = 20,
			FontColor = textColor,
			
			OutlineSize = 8,
			OutlineColor = Colors.Black,
		};
		
		label.LabelSettings = settings;
		label.labelType = type;
		
		AddChild(label);
		
		
		var rng = new Random();
		int deviation = rng.Next(-5,5);
		
		var position = Vector2.Zero;
		
		if (type == HoverInfoLabel.LabelType.WarningLabel)
		{
			position = GetLocalMousePosition() + new Vector2(deviation, 0);
			
		}
		else if (type == HoverInfoLabel.LabelType.PointsLabel && node != null)
		{
			position = node.Position;
		}
		
		label.Position = isLeftSide ? position: position - label.Size;
		label.ZIndex = 1;
	}
	
	
	// Fades out and up / down a specified label
	// Run inside of Process
	private void AddFadeOutLabel(bool floatUp, HoverInfoLabel label)
	{
		float multiplier = 0.995f;
		
		if(label.labelType == HoverInfoLabel.LabelType.PointsLabel)
		{
			multiplier = 0.95f;
		}
		
		if (floatUp)
		{
			label.Position -= new Vector2(0, 0.4f);
		}
		else
		{
			label.Position += new Vector2(0, 0.4f);
		}
		
		
		var settings = (LabelSettings)label.LabelSettings.Duplicate();
		
		Color prevFont = settings.FontColor;
		settings.FontColor = new Color(
			prevFont.R, 
			prevFont.G, 
			prevFont.B, 
			prevFont.A * multiplier
		);
		
		Color prevOutline = settings.OutlineColor;
		settings.OutlineColor = new Color(
			prevOutline.R, 
			prevOutline.G, 
			prevOutline.B, 
			prevOutline.A * multiplier
		);
		
		label.LabelSettings = settings;
		
		// Remove label
		if (prevFont.A < 0.2)
		{
			label.QueueFree();
		}
	}
	
	
}
