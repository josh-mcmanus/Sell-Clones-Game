using Godot;
using System;

public partial class ResearchTab : Control
{
	int counter = 0;
	
	int columns = 25;
	int rows = 6;
	
	int areaX = 710;
	int areaY = 175; // this is one pixel off in some areas
	
	
	
	public override void _Process(double delta)
	{
		if (counter < (columns * rows))
		{
			AddResearchNode();
		}
	}
	
	
	private void AddResearchNode()
	{
		// gives the index for the position in the current row, 0-4
		int rowIndex = counter % rows;
		
		// gives the index for the position of the current column 0-20
		int columnIndex = counter / rows;
		
		// Load and add to scene
		var bodyPartScene = GD.Load<PackedScene>("res://Scenes/research_node.tscn");
		var part = bodyPartScene.Instantiate<Node2D>();
		AddChild(part);
		
		Sprite2D researchSprite = part.GetNode<Sprite2D>("Sprite2D");
		
		
		// calculate position
		float spriteWidth = researchSprite.Texture.GetWidth();
		float spriteHeight = researchSprite.Texture.GetHeight();
		
		float marginX = (areaX - (spriteWidth * columns)) / (columns + 1);
		float marginY = (areaY - (spriteHeight * rows)) / (rows + 1);
		
		float x = marginX + columnIndex * (spriteWidth + marginX);
		float y = marginY + rowIndex * (spriteHeight + marginY);
		
		
		// set position
		part.Position = new Vector2(x, y);
		part.Position += new Vector2(spriteWidth / 2, spriteHeight / 2);
		counter++;
	}
	
	
	// (y height - (research node height (20) * rows)) / num gaps (rows + 1)
	
	// (x height - (research node width (20) * columns)) / num gaps (columns + 1)
	
	
	
}
