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
	
	
	
	
	// For enabling / disabling based on if the torso menu is open. Only internal organs can be moved around if it is open.
	public static bool IsInternalBodyPart(BodyPartEnum bodyPart)
	{
		var x = (int)bodyPart;

		// Is between the values of Heart and GenericUpgrade
		if (13 <= x && x <= 20)
		{
			return true;
		}
		
		return false;
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
			GD.Print($"index: {index}");
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
		"M", "B", "T", "Qa", "Qu", "Sx", "Sp", "Oc", "No", "Dc"
	};
	
	
	// For adding new body parts
	// File locations for body parts to load from
	public static readonly Dictionary<BodyPartEnum, string> bodyPartsDictionary = new()
	{
		{BodyPartEnum.LShoulder,"res://Scenes/Body Parts/limbs/l_shoulder.tscn"},
		{BodyPartEnum.LForearm,	"res://Scenes/Body Parts/limbs/l_forearm.tscn"}, 
		{BodyPartEnum.LHand,	"res://Scenes/Body Parts/limbs/l_hand.tscn"},
		
		{BodyPartEnum.RShoulder,"res://Scenes/Body Parts/limbs/r_shoulder.tscn"},
		{BodyPartEnum.RForearm,	"res://Scenes/Body Parts/limbs/r_forearm.tscn"},
		{BodyPartEnum.RHand,	"res://Scenes/Body Parts/limbs/r_hand.tscn"},
		
		
		{BodyPartEnum.LLeg,		"res://Scenes/Body Parts/limbs/l_leg.tscn"},
		{BodyPartEnum.LCalf,	"res://Scenes/Body Parts/limbs/l_calf.tscn"},
		{BodyPartEnum.LFoot,	"res://Scenes/Body Parts/limbs/l_foot.tscn"},
		
		{BodyPartEnum.RLeg,		"res://Scenes/Body Parts/limbs/r_leg.tscn"},
		{BodyPartEnum.RCalf,	"res://Scenes/Body Parts/limbs/r_calf.tscn"},
		{BodyPartEnum.RFoot,	"res://Scenes/Body Parts/limbs/r_foot.tscn"},
		
		
		{BodyPartEnum.Heart,	"res://Scenes/Body Parts/heart.tscn"},
		{BodyPartEnum.Lung,		"res://Scenes/Body Parts/lungs.tscn"},
		{BodyPartEnum.Liver,	"res://Scenes/Body Parts/liver.tscn"},
		
		{BodyPartEnum.Spine,			"res://Scenes/Body Parts/spine.tscn"},
		{BodyPartEnum.Kidney,			"res://Scenes/Body Parts/kidneys.tscn"},
		{BodyPartEnum.NeuralConnector,	"res://Scenes/Body Parts/neural_connector.tscn"},
		{BodyPartEnum.ImmuneReplacer,	"res://Scenes/Body Parts/immune_replacer.tscn"},
		{BodyPartEnum.GenericUpgrade,	"res://Scenes/Body Parts/generic_upgrade_slot.tscn"},
	};
	
	
	public static readonly Dictionary<ResearchTypeEnum, Color> researchTypeColorDictionary = new()
	{
		{ResearchTypeEnum.Limbs,  new Color(0.877f, 0.781f, 0.0f)},
		{ResearchTypeEnum.Organs, new Color(0.792f, 0.553f, 0.286f)},
		{ResearchTypeEnum.Exotic, new Color(0.58f,  0.478f, 0.894f)},
		{ResearchTypeEnum.Brain,  new Color(0.953f, 0.388f, 0.471f)},
		{ResearchTypeEnum.Tank,   new Color(0.349f, 1.0f,   1.0f)},
	};
	
	
	public static readonly Dictionary<ResearchTypeEnum, int> baseResearchNodeCost = new()
	{
		{ResearchTypeEnum.Limbs,  10},
		{ResearchTypeEnum.Organs, 1000},
		{ResearchTypeEnum.Exotic, 100000}, // 100k
		{ResearchTypeEnum.Brain,  10000000}, // 10m
		{ResearchTypeEnum.Tank,   1000000000}, // 1b
	};
	
	
	public enum BodyPartEnum
	{
		LShoulder,
		LForearm,
		LHand,
		
		RShoulder,
		RForearm,
		RHand,
		
		
		LLeg,
		LCalf,
		LFoot,
		
		RLeg,
		RCalf,
		RFoot,
		
		
		Torso,
		Heart,
		Lung,
		Liver,

		Spine,
		Kidney,
		NeuralConnector,
		ImmuneReplacer,
		GenericUpgrade
	}
	
	
	public enum ResearchTypeEnum
	{
		Limbs,
		Organs,
		Exotic,
		Brain,
		Tank,
	}
}
