using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class PlayerData
{
	public double Money {get; set;} = 110;
	
	[JsonInclude]
	public Dictionary<Utils.NodeTypeEnum, double> ResearchPoints = new()
	{
		{Utils.NodeTypeEnum.Limbs,  0},
		{Utils.NodeTypeEnum.Organs, 0},
		{Utils.NodeTypeEnum.Brain,  0},
		{Utils.NodeTypeEnum.Tank,   0},
		// {Utils.NodeTypeEnum.Exotic, 0},
	};
	
	[JsonInclude]
	public Dictionary<Utils.NodeTypeEnum, int> UnlockedNodes = new()
	{
		{Utils.NodeTypeEnum.Limbs,  0},
		{Utils.NodeTypeEnum.Organs, 0},
		{Utils.NodeTypeEnum.Brain,  0},
		{Utils.NodeTypeEnum.Tank,   0},
		{Utils.NodeTypeEnum.Clone,   0},
		// {Utils.NodeTypeEnum.Exotic, 0},
	};
	
	// Category for each upgradable thing - clone, brain, tank
		// save the currently active upgrades attached to the thing
		// do not save the positions and what is currently not attached
	
	
	// figure out saving clone data later
	// also maybe need to save the position of all active body parts
	// public Utils.BodyPartEnum shoulder {get; set;} = Utils.BodyPartEnum.LShoulder;
	
	// clone details
		// json list of part names - shoulder, forearm, hand, foot, etc.
	
	
	
	
	
	
	
	
	
	
	
}
