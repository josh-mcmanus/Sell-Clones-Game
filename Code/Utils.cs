using Godot;
using System;
using System.Collections.Generic;

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
	
	
	
	public static void UnlockNextUpgrade(BodyPart part)
	{
		if (!DataManager.saveData.UnlockedUpgrades.ContainsKey(part))
		{
			DataManager.saveData.UnlockedUpgrades.Add(part, 0);
		}
		
		DataManager.saveData.UnlockedUpgrades[part] += 1;
	}
	
	
	public static bool IsUpgradeUnlocked(BodyPart part, UpgradeTier upgrade)
	{
		if (!DataManager.saveData.UnlockedUpgrades.TryGetValue(part, out int value))
		{
			return false;
		}
		
		if (value < (int)upgrade)
		{
			return false;
		}
		
		return true;
	}
	
	
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
	
	
	public static string GetBodyPartDirectory(BodyPart bodyPart)
	{
		if (!bodyPartsDictionary.ContainsKey(bodyPart))
		{
			throw new ArgumentNullException($"Key: {bodyPart}, could not be found in bodyParts dictionary");
		}
		
		return bodyPartsDictionary.GetValueOrDefault(bodyPart);
	}
	
	
	
	
	// For adding new body parts
	// File locations for body parts to load from
	public static readonly Dictionary<BodyPart, string> bodyPartsDictionary = new()
	{
		{BodyPart.Shoulder, "res://Scenes/Body Parts/shoulder.tscn"},
		{BodyPart.Forearm,	"res://Scenes/Body Parts/forearm.tscn"}, 
		{BodyPart.Hand,	 	"res://Scenes/Body Parts/hand.tscn"},
		{BodyPart.Thigh,	"res://Scenes/Body Parts/thigh.tscn"},
		{BodyPart.Calf,	 	"res://Scenes/Body Parts/calf.tscn"},
		{BodyPart.Foot,	 	"res://Scenes/Body Parts/foot.tscn"},
		
		{BodyPart.Heart,	"res://Scenes/Body Parts/heart.tscn"},
		{BodyPart.Lung,	 	"res://Scenes/Body Parts/lungs.tscn"},
		{BodyPart.Liver,	"res://Scenes/Body Parts/liver.tscn"},
		{BodyPart.Kidney,	"res://Scenes/Body Parts/kidneys.tscn"},
		{BodyPart.Spine,	"res://Scenes/Body Parts/spine.tscn"},
		
		{BodyPart.Torso, 	"res://Scenes/Body Parts/torso.tscn"},
		
		
	};
	
	
	public static readonly Dictionary<NodeType, Color> colorDictionary = new()
	{
		{NodeType.Limbs,  new Color(0.877f, 0.781f, 0.0f)},
		{NodeType.Organs, new Color(0.792f, 0.553f, 0.286f)},
		{NodeType.Brain,  new Color(0.953f, 0.388f, 0.471f)},
		{NodeType.Tank,   new Color(0.349f, 1.0f,   1.0f)},
		{NodeType.Clone,  new Color(1.0f,   1.0f,   1.0f)},
	};
	
	
	public static readonly Dictionary<NodeType, int> nodeCostDictionary = new()
	{
		{NodeType.Limbs,  10},
		{NodeType.Organs, 1000},
		{NodeType.Brain,  100000}, // 10m
		{NodeType.Tank,   10000000}, // 1b
		{NodeType.Clone,  100},
	};
	
	
	public static readonly Dictionary<UpgradeTier, float> bodyPartUpgradeMultipler = new()
	{
		{UpgradeTier.Level0, 0.0f},
		{UpgradeTier.Level1, 1.0f},
		{UpgradeTier.Level2, 1.5f},
		{UpgradeTier.Level3, 2.5f},
		{UpgradeTier.Level4, 3.0f},
		{UpgradeTier.Level5, 5.0f},
		{UpgradeTier.Level6, 10.0f},
	};
	
	
	public static readonly Dictionary<UpgradeTier, string> upgradeTierDescriptions = new()
	{
		{UpgradeTier.Level0, "Upgrade locked"},
		{UpgradeTier.Level1, "99% organically grown"},
		{UpgradeTier.Level2, "A simple prosthetic replacement, increases worker efficiency by up to 20%*\n*compared to workers with no limbs."},
		{UpgradeTier.Level3, "Advanced mechanical parts are here!\nUsers may be prone to violent uncontrollable outbursts."},
		{UpgradeTier.Level4, "Synthetic"},
		{UpgradeTier.Level5, "Cybernetic may control the user in some cases."},
		{UpgradeTier.Level6, "Nanomachines"},
		
	};
	
	
	// All possible upgrades and their category
	public static readonly Dictionary<BodyPart, NodeType> bodyPartCategories = new()
	{
		{BodyPart.Shoulder, NodeType.Limbs},
		{BodyPart.Forearm,  NodeType.Limbs},
		{BodyPart.Hand,     NodeType.Limbs},
		{BodyPart.Thigh,    NodeType.Limbs},
		{BodyPart.Calf,     NodeType.Limbs},
		{BodyPart.Foot,     NodeType.Limbs},
		
		{BodyPart.Heart,  NodeType.Organs},
		{BodyPart.Lung,   NodeType.Organs},
		{BodyPart.Liver,  NodeType.Organs},
		{BodyPart.Kidney, NodeType.Organs},
		{BodyPart.Spine,  NodeType.Organs},
		
		{BodyPart.Processor,         NodeType.Brain},
		{BodyPart.MemoryEnhancer,    NodeType.Brain},
		{BodyPart.NeuralInterface,   NodeType.Brain},
		{BodyPart.PredictionEngine,  NodeType.Brain},
		{BodyPart.NeuronCompressor,  NodeType.Brain},
		{BodyPart.PatternRecognizer, NodeType.Brain},
		{BodyPart.EmotionSupressor,  NodeType.Brain},
		{BodyPart.AICoprocessor,     NodeType.Brain},
		{BodyPart.QuantumFluctuator, NodeType.Brain},
		
		{BodyPart.GrowthAccelerator,      NodeType.Tank},
		{BodyPart.TimeDilationField,      NodeType.Tank},
		{BodyPart.MutationFilter,         NodeType.Tank},
		{BodyPart.IncompleteCloning,      NodeType.Tank},
		{BodyPart.WeakGenetics,           NodeType.Tank},
		{BodyPart.ReducedLifespan,        NodeType.Tank},
		{BodyPart.ImmuneRejection,        NodeType.Tank},
		{BodyPart.OverclockedCellDivider, NodeType.Tank},
		{BodyPart.MutationEngine,         NodeType.Tank},
		{BodyPart.RandomizedGrowth,       NodeType.Tank},
		{BodyPart.GeneticShuffler,        NodeType.Tank},
		
	};
	
	
	// --- Body Parts ---
	public enum Side
	{
		Left,
		Right,
		None
	}
	
	public enum BodyPart
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
	
	// public enum BodyPartUpgradeTier
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
