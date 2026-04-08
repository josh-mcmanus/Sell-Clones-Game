using Godot;
using System;
using System.Collections.Generic;

public partial class ResearchTab : Control
{
	
	[Export]
	public Utils.ResearchTypeEnum researchTypeEnum;
	
	[Export]
	private Control control;
	
	[Export]
	private Button button;
	
	
	private const string TAG = "ResearchTab";
	
	PackedScene bodyPartScene = GD.Load<PackedScene>("res://Scenes/research_node.tscn");
	
	double researchNodeCost = 0;
	
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
		if (researchTypeEnum == Utils.ResearchTypeEnum.Brain || researchTypeEnum == Utils.ResearchTypeEnum.Tank)
		{
			columns = 15;
		}
		
		// Set cost of research node
		researchNodeCost = Utils.baseResearchNodeCost[researchTypeEnum];
		
		
		// Set dimensions
		ColorRect parent = (ColorRect)GetParent();
		Vector2 areaSize = parent.CustomMinimumSize;
		
		areaX = (int)areaSize.X;
		areaY = (int)areaSize.Y;
		
		marginX = (areaX - buttonWidth - (spriteWidth * columns)) / (columns + 2);
		marginY = (areaY - (spriteHeight * rows)) / (rows + 1);

		button.CustomMinimumSize = new Vector2(buttonWidth, areaY - marginY * 2);

		// Set button position
		if (researchTypeEnum == Utils.ResearchTypeEnum.Limbs || researchTypeEnum == Utils.ResearchTypeEnum.Brain)
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
	}
	
	
	public override void _Process(double delta)
	{
		// Adjust labels
		foreach (Node node in GetChildren())
		{
			if (node is Label label)
			{
				// Move down
				label.Position += new Vector2(0, 0.4f);
				
				var settings = (LabelSettings)label.LabelSettings.Duplicate();
				
				float multiplier = 0.995f;
				
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
		
		
		// Add research nodes on load
		// if ((researchNodeCounter < (columns * rows)) && (researchNodeCounter < DataManager.saveData.ResearchNodes[researchTypeEnum]))
		// {
		// 	AddResearchNode();
		// }
		
		if (researchNodeCounter < (columns * rows))
		{
			AddResearchNode();
		}
	}
	
	
	// Activates on add research node button press
	// Checks if the user can add a research node
	private void OnButtonPressed()
	{
		// All nodes added
		if (researchNodeCounter > (columns * rows))
		{
			// display message, at mouse position, in blue, for 3 seconds, fading out, moving up
			// "All research nodes purchased"
			
			var text = "All research nodes purchased";
			var color = Colors.Blue;
			
			AddFadingLabel(text, color);
			
			return;
		}
		
		// Not enough money
		if (DataManager.saveData.Money < researchNodeCost)
		{
			// display message, at mouse position, in red, for 3 seconds, fading out, moving up
			// "Not enough money"
			
			var text = "Not enough money";
			var color = Colors.Red;
			
			AddFadingLabel(text, color);
			
			return;
		}
		
		AddResearchNode();
		
		// Update data
		DataManager.saveData.ResearchNodes[researchTypeEnum] += 1;
		DataManager.saveData.Money -= researchNodeCost;
	}
	
	
	// Activates on function call
	// For Adding one research unit to the research tab
	private void AddResearchNode()
	{
		// gives the index for the position in the current row, 0-4
		int rowIndex = researchNodeCounter % rows;
		
		// gives the index for the position of the current column 0-20
		int columnIndex = researchNodeCounter / rows;
		
		// Load and add to scene
		var part = bodyPartScene.Instantiate<Node2D>();
		AddChild(part);
		
		// Connect signal
		ResearchNode researchNode = (ResearchNode)part;
		researchNode.ResearchUnitComplete += AddResearchUnit;
		
		
		// Set color based on type
		if (!Utils.researchTypeColorDictionary.ContainsKey(researchTypeEnum))
		{
			throw new ArgumentNullException($"Key: {researchTypeEnum}, could not be found in researchTypeColorDictionary");
		}
		else
		{
			Color typeColor = Utils.researchTypeColorDictionary.GetValueOrDefault(researchTypeEnum);
			researchNode.nodeColor.Color = typeColor;
		}
		
		
		// calculate position
		float xOffset = isLeftSide ? buttonWidth + marginX : 0;
		
		float x = xOffset + marginX + columnIndex * (spriteWidth + marginX);
		float y = marginY + rowIndex * (spriteHeight + marginY);
		
		part.Position = new Vector2(x, y);
		part.Position += new Vector2(spriteWidth / 2, spriteHeight / 2);
		researchNodeCounter++;
		
		// Update cost
		researchNodeCost = Math.Round(researchNodeCost * 1.5, 2);
		Utils.Log(TAG, $"{researchTypeEnum} {researchNodeCost}");
	}
	
	
	// Activated when a research units timer times out
	// For adding 1 research unit to the global counter
	private void AddResearchUnit()
	{
		// Add to player data
		DataManager.saveData.ResearchPoints[researchTypeEnum] += 1;
	}
	
	
	private void AddFadingLabel(string labelContent, Color textColor)
	{
		Label label = new()
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
		
		AddChild(label);
		
		
		var rng = new Random();
		int deviation = rng.Next(-5,5);
		
		var mousePos = GetLocalMousePosition() + new Vector2(deviation, 0);
		
		label.Position = isLeftSide ? mousePos: mousePos - label.Size;
		label.ZIndex = 1;
	}
	
}
