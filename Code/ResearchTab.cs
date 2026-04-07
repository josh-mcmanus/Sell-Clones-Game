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
	
	
	PackedScene bodyPartScene = GD.Load<PackedScene>("res://Scenes/research_node.tscn");
	
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
			columns = 12;
		}
		
		
		
		// Set dimensions
		ColorRect parent = (ColorRect)GetParent();
		Vector2 areaSize = parent.CustomMinimumSize;
		
		areaX = (int)areaSize.X;
		areaY = (int)areaSize.Y;
		
		marginX = (areaX - buttonWidth - (spriteWidth * columns)) / (columns + 2);
		marginY = (areaY - (spriteHeight * rows)) / (rows + 1);
		
		// Set button size and position
		button.CustomMinimumSize = new Vector2(buttonWidth, areaY - marginY * 2);
		button.Position = new Vector2(areaX - buttonWidth - marginX, marginY);
		
		button.Pressed += OnButtonPressed;
	}
	
	
	public override void _Process(double delta)
	{
		// if (researchNodeCounter < (columns * rows))
		// {
		// 	AddResearchNode();
		// }
	}
	
	
	// Activates on add research node button press
	// Checks if the user can add a research node
	private void OnButtonPressed()
	{
		// get global money
		
		// compare cost of adding node to global money
			// if enough money, subtract cost, add node, increase price for next time
			// else, display too poor red text "Not enough money" for 3 seconds, fading out and moving up
		
		if (researchNodeCounter < (columns * rows))
		{
			AddResearchNode();
		}
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
		float x = marginX + columnIndex * (spriteWidth + marginX);
		float y = marginY + rowIndex * (spriteHeight + marginY);
		
		part.Position = new Vector2(x, y);
		part.Position += new Vector2(spriteWidth / 2, spriteHeight / 2);
		researchNodeCounter++;
		
		
		// Add to player data
		DataManager.playerData.ResearchNodes[researchTypeEnum] += 1;
	}
	
	
	// Activated when a research units timer times out
	// For adding 1 research unit to the global counter
	private void AddResearchUnit()
	{
		// Add to player data
		DataManager.playerData.ResearchPoints[researchTypeEnum] += 1;
	}
	
	
	// (y height - (research node height (20) * rows)) / num gaps (rows + 1)
	
	// (x height - (research node width (20) * columns)) / num gaps (columns + 1)
	
	
	
}
