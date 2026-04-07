using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class PlayerData
{
	public int Money {get; set;} = 0;
	
	[JsonInclude] // does not normally include these in the json
	public Dictionary<Utils.ResearchTypeEnum, int> ResearchPoints = new()
	{
		{Utils.ResearchTypeEnum.Limbs,  0},
		{Utils.ResearchTypeEnum.Organs, 0},
		{Utils.ResearchTypeEnum.Exotic, 0},
		{Utils.ResearchTypeEnum.Brain,  0},
		{Utils.ResearchTypeEnum.Tank,   0},
	};
	
	[JsonInclude]
	public Dictionary<Utils.ResearchTypeEnum, int> ResearchNodes = new()
	{
		{Utils.ResearchTypeEnum.Limbs,  0},
		{Utils.ResearchTypeEnum.Organs, 0},
		{Utils.ResearchTypeEnum.Exotic, 0},
		{Utils.ResearchTypeEnum.Brain,  0},
		{Utils.ResearchTypeEnum.Tank,   0},
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
