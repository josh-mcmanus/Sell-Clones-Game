using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class PlayerData
{
	public double Money {get; set;} = 110;
	
	
	[JsonInclude]
	public Dictionary<Utils.NodeType, double> ResearchPoints = new()
	{
		{Utils.NodeType.Limbs,  0},
		{Utils.NodeType.Organs, 0},
		{Utils.NodeType.Brain,  0},
		{Utils.NodeType.Tank,   0},
	};
	
	[JsonInclude]
	public Dictionary<Utils.NodeType, int> UnlockedNodes = new()
	{
		{Utils.NodeType.Limbs,  0},
		{Utils.NodeType.Organs, 0},
		{Utils.NodeType.Brain,  0},
		{Utils.NodeType.Tank,   0},
		{Utils.NodeType.Clone,  0},
	};
	
	[JsonInclude]
	public Dictionary<Utils.BodyPart, int> UnlockedUpgrades = new();
	
	// Category for each upgradable thing - clone, brain, tank
		// save the currently active upgrades attached to the thing
		// do not save the positions and what is currently not attached
	
	
	// figure out saving clone data later
	// also maybe need to save the position of all active body parts
	// public Utils.BodyPartEnum shoulder {get; set;} = Utils.BodyPartEnum.LShoulder;
	
	// clone details
		// json list of part names - shoulder, forearm, hand, foot, etc.
	
	
	
	
	
	
	
	
	
	
	
}
