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
	public Dictionary<Utils.BodyPart, int> UnlockedUpgrades = new()
	{
		// Unlocked by default
		{Utils.BodyPart.Shoulder, 0},
		{Utils.BodyPart.Forearm,  0},
		{Utils.BodyPart.Hand,     0},
		{Utils.BodyPart.Thigh,    0},
		{Utils.BodyPart.Calf,     0},
		{Utils.BodyPart.Foot,     0},
		
		// Not unlocked
		{Utils.BodyPart.Heart,  0},
		{Utils.BodyPart.Lung,   0},
		{Utils.BodyPart.Liver,  0},
		{Utils.BodyPart.Kidney, 0},
		{Utils.BodyPart.Spine,  0},
		
		{Utils.BodyPart.Processor,         0},
		{Utils.BodyPart.MemoryEnhancer,    0},
		{Utils.BodyPart.NeuralInterface,   0},
		{Utils.BodyPart.PredictionEngine,  0},
		{Utils.BodyPart.NeuronCompressor,  0},
		{Utils.BodyPart.PatternRecognizer, 0},
		{Utils.BodyPart.EmotionSupressor,  0},
		{Utils.BodyPart.AICoprocessor,     0},
		{Utils.BodyPart.QuantumFluctuator, 0},
		
		{Utils.BodyPart.GrowthAccelerator,      0},
		{Utils.BodyPart.TimeDilationField,      0},
		{Utils.BodyPart.MutationFilter,         0},
		{Utils.BodyPart.IncompleteCloning,      0},
		{Utils.BodyPart.WeakGenetics,           0},
		{Utils.BodyPart.ReducedLifespan,        0},
		{Utils.BodyPart.ImmuneRejection,        0},
		{Utils.BodyPart.OverclockedCellDivider, 0},
		{Utils.BodyPart.MutationEngine,         0},
		{Utils.BodyPart.RandomizedGrowth,       0},
		{Utils.BodyPart.GeneticShuffler,        0},
	};
	
	// New parts are added when attached, deleted when removed
	[JsonInclude]
	public List<AttachedPartData> AttachedParts = [];
	
	
	// Category for each upgradable thing - clone, brain, tank
		// save the currently active upgrades attached to the thing
		// do not save the positions and what is currently not attached
	
	
	// figure out saving clone data later
	// also maybe need to save the position of all active body parts
	// public Utils.BodyPartEnum shoulder {get; set;} = Utils.BodyPartEnum.LShoulder;
	
	// clone details
		// json list of part names - shoulder, forearm, hand, foot, etc.
	
	
	
	
	
	
	
	
	
	
	
}
