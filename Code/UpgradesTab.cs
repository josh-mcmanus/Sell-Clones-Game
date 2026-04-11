using Godot;
using System;

public partial class UpgradesTab : Control
{
	[Export]
	private Utils.NodeType areaType;
	
	[Export]
	private VBoxContainer listArea;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// check unlocked upgrades
		
		// populate upgrades tab with items
		
		// loop through unlocks
		
		
		// Add the upgrade to
			// VBox container
		
		// Add listUpgrade to
			// Foldable Container inside vbox container above
		
		
		// Set name as upgrade 1, upgrade 2, etc.
		// then move to next and add new vbox container
		
		foreach(var unlockedUpgrade in DataManager.saveData.UnlockedUpgrades)
		{
			// if (unlockedUpgrade.Key == areaType)
			// {
				
			// }
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
