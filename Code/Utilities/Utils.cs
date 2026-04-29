using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0162 // Unreachable code detected


public partial class Utils : Node
{
	// --- Debug ---
	private const string TAG = "Utils";
	public const bool DebugEnabled = true;
	
	public static void Log(string TAG, object msg)
	{
		if (!DebugEnabled) return;
		GD.Print($"[{TAG}] {msg}");
	}
	
	public static void Error(string TAG, object msg)
	{
		if (!DebugEnabled) return;
		GD.PrintErr($"[{TAG}] {msg}");
	}
	
	
	// --- Money Number Formatting ---
	
	// 8_900 -> 8,900
	// 10_500_000 -> 10.5M
	// 999_999_999_999 -> 999.99B
	public static string FormatNumber(double number)
	{
		// Less than 1 million, format with commas
		if (number < 1_000_000)
		{
			return number.ToString("#,##0");
		}
		
		
		int index = 0;
		double value = number / 1000000;
		
		// Prevent rounding
		value = Math.Floor(value * 100) / 100;
		
		// Keep dividing by 1000 until the correct suffix is reached
		while (value >= 1000 && index < shortSuffixes.Length)
		{
			value /= 1000;
			index++;
		}
		
		// Limit to 2 decimal places
		string formatted = value.ToString("0.##");
		
		return $"{formatted}{shortSuffixes[index]}";
	}
	
	private static readonly string[] suffixes =
	{
		"million", "billion", "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion", "nonillion", "decillion"
	};
	
	private static readonly string[] shortSuffixes =
	{
		"M", "B", "T", "Qa", "Qu", "Sx", "Sp", "Oc", "No", "Dc", "Udc", "Ddc", "Tdc", "Qadc", "Qidc", "Sxdc", "Spdc", "Ocdc", "Nmdc", "Vg"
	};
	
	
	
	public static string GetUpgradePartDirectory(UpgradePart upgradePart)
	{
		if (!UpgradePartsFileLocation.ContainsKey(upgradePart))
		{
			throw new ArgumentNullException($"Key: {upgradePart}, could not be found in UpgradeParts dictionary");
		}
		
		return UpgradePartsFileLocation.GetValueOrDefault(upgradePart);
	}
	
	
	public static UpgradePart? GetLowestLevelUpgradeInCategory(NodeType type)
	{
		int minimum = int.MaxValue;
		UpgradePart? lowestPart = null;
		
		foreach (var part in Enum.GetValues<UpgradePart>())
		{
			if(part == UpgradePart.Torso)
				continue;
			
			if (UpgradePartCategories[part] != type)
				continue;
				
			int currentValue = DataManager.saveData.UnlockedUpgrades[part];
			
			if (currentValue < minimum)
			{
				minimum = currentValue;
				lowestPart = part;
			}
		}
		
		return lowestPart;
	}
	
	
	// TODO: Maybe later get rid of this and replace the limbs new sprite instead of different color?
	public static readonly Dictionary<UpgradeTier, Color> UpgradePartUpgradeTierColors = new()
	{
		{UpgradeTier.Level0, new Color()},
		{UpgradeTier.Level1, Colors.White}, 	// White
		{UpgradeTier.Level2, Colors.Blue}, 		// Blue
		{UpgradeTier.Level3, Colors.Red}, 		// Red
		{UpgradeTier.Level4, Colors.Purple}, 	// Purple
		{UpgradeTier.Level5, Colors.Black},		// Black
		{UpgradeTier.Level6, Colors.PeachPuff}, // Skin
	};
	
	
	// Color of all nodes, research and clone
	public static readonly Dictionary<NodeType, Color> nodeColors = new()
	{
		{NodeType.Limbs,  new Color(0.877f, 0.781f, 0.0f)},
		{NodeType.Organs, new Color(0.792f, 0.553f, 0.286f)},
		{NodeType.Brain,  new Color(0.953f, 0.388f, 0.471f)},
		{NodeType.Tank,   new Color(0.349f, 1.0f,   1.0f)},
		{NodeType.Clone,  new Color(1.0f,   1.0f,   1.0f)},
	};
	
	
	// Value of upgrade tiers
	public static readonly Dictionary<UpgradeTier, int> UpgradePartUpgradeTierValues = new()
	{
		{UpgradeTier.Level0, 0},
		{UpgradeTier.Level1, 1},
		{UpgradeTier.Level2, 2},
		{UpgradeTier.Level3, 5},
		{UpgradeTier.Level4, 10},
		{UpgradeTier.Level5, 20},
		{UpgradeTier.Level6, 50},
	};
	
	
	
	// Base node purchase cost for all nodes, research and clone
	public static readonly Dictionary<NodeType, int> baseNodePurchaseCost = new()
	{
		{NodeType.Limbs,  10},
		{NodeType.Organs, 1_000},
		{NodeType.Brain,  100_000},   // 100K
		{NodeType.Tank,   10_000_000}, // 10M
		{NodeType.Clone,  100},
	};
	
	
	
	// Maps upgrades with their corresponding node type category
	public static readonly Dictionary<UpgradePart, NodeType> UpgradePartCategories = new()
	{
		{UpgradePart.Shoulder, NodeType.Limbs},
		{UpgradePart.Forearm,  NodeType.Limbs},
		{UpgradePart.Hand,     NodeType.Limbs},
		{UpgradePart.Thigh,    NodeType.Limbs},
		{UpgradePart.Calf,     NodeType.Limbs},
		{UpgradePart.Foot,     NodeType.Limbs},
		
		{UpgradePart.Heart,  NodeType.Organs},
		{UpgradePart.Lung,   NodeType.Organs},
		{UpgradePart.Liver,  NodeType.Organs},
		{UpgradePart.Kidney, NodeType.Organs},
		{UpgradePart.Spine,  NodeType.Organs},
		
		{UpgradePart.Processor,         NodeType.Brain},
		{UpgradePart.MemoryEnhancer,    NodeType.Brain},
		{UpgradePart.NeuralInterface,   NodeType.Brain},
		{UpgradePart.PredictionEngine,  NodeType.Brain},
		{UpgradePart.NeuronCompressor,  NodeType.Brain},
		{UpgradePart.PatternRecognizer, NodeType.Brain},
		{UpgradePart.EmotionSupressor,  NodeType.Brain},
		{UpgradePart.AICoprocessor,     NodeType.Brain},
		{UpgradePart.QuantumFluctuator, NodeType.Brain},
		
		{UpgradePart.GrowthAccelerator,      NodeType.Tank},
		{UpgradePart.TimeDilationField,      NodeType.Tank},
		{UpgradePart.MutationFilter,         NodeType.Tank},
		{UpgradePart.IncompleteCloning,      NodeType.Tank},
		{UpgradePart.WeakGenetics,           NodeType.Tank},
		{UpgradePart.ReducedLifespan,        NodeType.Tank},
		{UpgradePart.ImmuneRejection,        NodeType.Tank},
		{UpgradePart.OverclockedCellDivider, NodeType.Tank},
		{UpgradePart.MutationEngine,         NodeType.Tank},
		{UpgradePart.RandomizedGrowth,       NodeType.Tank},
		{UpgradePart.GeneticShuffler,        NodeType.Tank},
	};
	
	
	// For adding new body parts
	// File locations for body parts to load from
	public static readonly Dictionary<UpgradePart, string> UpgradePartsFileLocation = new()
	{
		{UpgradePart.Shoulder, "res://Scenes/Clone/Body Parts/shoulder.tscn"},
		{UpgradePart.Forearm,	"res://Scenes/Clone/Body Parts/forearm.tscn"}, 
		{UpgradePart.Hand,	 	"res://Scenes/Clone/Body Parts/hand.tscn"},
		{UpgradePart.Thigh,	"res://Scenes/Clone/Body Parts/thigh.tscn"},
		{UpgradePart.Calf,	 	"res://Scenes/Clone/Body Parts/calf.tscn"},
		{UpgradePart.Foot,	 	"res://Scenes/Clone/Body Parts/foot.tscn"},
	};
	
	
	// Flavour text, upgrade tier descriptions
	// TODO: Maybe replace this later, so each item has its own flavour text instead of generic "level" for all in that tier
	public static readonly Dictionary<UpgradeTier, string> upgradeTierDescriptions = new()
	{
		{UpgradeTier.Level0, "Upgrade locked"},
		{UpgradeTier.Level1, "organically grown"},
		{UpgradeTier.Level2, "A simple prosthetic replacement, increases worker efficiency by up to 20%*\n*compared to workers with no limbs."},
		{UpgradeTier.Level3, "Advanced mechanical parts are here!\nUsers may be prone to violent uncontrollable outbursts."},
		{UpgradeTier.Level4, "Synthetic"},
		{UpgradeTier.Level5, "Cybernetic may control the user in some cases."},
		{UpgradeTier.Level6, "Nanomachines"},
	};
	
	
	
	// --- Body Parts ---
	public enum Side
	{
		Left,
		Right,
		None
	}
	
	// TODO: Add more upgrades for all categories
	// All possible UpgradeParts / upgrades (anything that could be draggable / placed into an upgrade slot - excluding torso)
	public enum UpgradePart
	{
		Shoulder,
		Forearm,
		Hand,
		Thigh,
		Calf,
		Foot,
		
		Heart,
		Lung,
		Liver,
		Kidney,
		Spine,
		
		Torso,
		
		//Brain
		Processor,
		MemoryEnhancer,
		NeuralInterface,
		PredictionEngine,
		NeuronCompressor,
		PatternRecognizer,
		EmotionSupressor,
		AICoprocessor,
		QuantumFluctuator,
		
		// Clone Tank
		GrowthAccelerator,
		TimeDilationField,
		MutationFilter,
		IncompleteCloning,
		WeakGenetics,
		ReducedLifespan,
		ImmuneRejection,
		OverclockedCellDivider,
		MutationEngine,
		RandomizedGrowth,
		GeneticShuffler,
	}
	
	public enum UpgradeTier
	{
		Level0,
		Level1,
		Level2,
		Level3,
		Level4,
		Level5,
		Level6,
	}
	
	public enum NodeType
	{
		Limbs,
		Organs,
		Brain,
		Tank,
		Clone
	}
	
	// public enum UpgradePartUpgradeTier
	// {
	// 	Natural,
	// 	Prosthetic,
	// 	Mechanical,
	// 	Synthetic,
	// 	Cybernetic,
	// 	Nanomachine,
	// }
	
	// public enum BrainUpgradeTier
	// {
	// 	Natural,
	// 	Electrical,
	// 	Neural,
	// 	Molecular,
	// 	Atomic,
	// 	Quantum,
	// }
	
	// public enum CloneTankUpgradeTier
	// {
	// }
	
	
	
	
	
}
