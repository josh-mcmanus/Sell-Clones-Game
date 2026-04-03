using Godot;
using System;
using System.Collections.Generic;

public partial class Utils : Node
{
	private const string TAG = "Utils";
	
	public static void Log(string TAG, object msg)
	{
		GD.Print($"[{TAG}] {msg}");
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
	
	
	// public static Vector2 GetGridPosition(int index, int rows, float marginX, float marginY, Vector2 startPosition)
	// {
	// 	int row = index % rows;
	// 	int col = index / rows;
		
	// 	return new Vector2(
	// 		startPosition.X + col * marginX,
	// 		startPosition.Y + row * marginY	
	// 	);
	// }
	
	
	
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
	
}
