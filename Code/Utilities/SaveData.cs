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
		{Utils.NodeType.Limbs,  1},
		{Utils.NodeType.Organs, 0},
		{Utils.NodeType.Brain,  0},
		{Utils.NodeType.Tank,   0},
		{Utils.NodeType.Clone,  1},
	};
	
	[JsonInclude]
	public Dictionary<Utils.UpgradePart, int> UnlockedUpgrades = new()
	{
		// Unlocked by default
		{Utils.UpgradePart.Shoulder, 0},
		{Utils.UpgradePart.Forearm,  0},
		{Utils.UpgradePart.Hand,     0},
		{Utils.UpgradePart.Thigh,    0},
		{Utils.UpgradePart.Calf,     0},
		{Utils.UpgradePart.Foot,     0},
		
		// Not unlocked
		{Utils.UpgradePart.Heart,  0},
		{Utils.UpgradePart.Lung,   0},
		{Utils.UpgradePart.Liver,  0},
		{Utils.UpgradePart.Kidney, 0},
		{Utils.UpgradePart.Spine,  0},
		
		{Utils.UpgradePart.Processor,         0},
		{Utils.UpgradePart.MemoryEnhancer,    0},
		{Utils.UpgradePart.NeuralInterface,   0},
		{Utils.UpgradePart.PredictionEngine,  0},
		{Utils.UpgradePart.NeuronCompressor,  0},
		{Utils.UpgradePart.PatternRecognizer, 0},
		{Utils.UpgradePart.EmotionSupressor,  0},
		{Utils.UpgradePart.AICoprocessor,     0},
		{Utils.UpgradePart.QuantumFluctuator, 0},
		
		{Utils.UpgradePart.GrowthAccelerator,      0},
		{Utils.UpgradePart.TimeDilationField,      0},
		{Utils.UpgradePart.MutationFilter,         0},
		{Utils.UpgradePart.IncompleteCloning,      0},
		{Utils.UpgradePart.WeakGenetics,           0},
		{Utils.UpgradePart.ReducedLifespan,        0},
		{Utils.UpgradePart.ImmuneRejection,        0},
		{Utils.UpgradePart.OverclockedCellDivider, 0},
		{Utils.UpgradePart.MutationEngine,         0},
		{Utils.UpgradePart.RandomizedGrowth,       0},
		{Utils.UpgradePart.GeneticShuffler,        0},
	};
	
	// New parts are added when attached, deleted when removed
	[JsonInclude]
	public List<AttachedPartData> AttachedParts = [];
	
	
	// Category for each upgradable thing - clone, brain, tank
		// save the currently active upgrades attached to the thing
		// do not save the positions and what is currently not attached
	
	
	// figure out saving clone data later
	// also maybe need to save the position of all active body parts
	// public Utils.UpgradePartEnum shoulder {get; set;} = Utils.UpgradePartEnum.LShoulder;
	
	// clone details
		// json list of part names - shoulder, forearm, hand, foot, etc.
	
	
	
	
	
	
	
	
	
	
	
}
