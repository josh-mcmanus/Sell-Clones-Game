using Godot;
using System;
using System.Collections.Generic;

public partial class Utils : Node
{
	private const string TAG = "Utils";
	
	public static void Log(string TAG, string msg)
	{
		GD.Print($"[{TAG}] {msg}");
	}
	
	
	
	// For adding new body parts
	// File locations for body parts to load from
	public static readonly Dictionary<BodyPartEnum, string> bodyPartsDictionary = new()
	{
		{BodyPartEnum.LShoulder,"res://Scenes/Body Parts/l_shoulder.tscn"},
		{BodyPartEnum.LForearm,	"res://Scenes/Body Parts/l_forearm.tscn"}, 
		{BodyPartEnum.LHand,	"res://Scenes/Body Parts/l_hand.tscn"},
		{BodyPartEnum.RShoulder,"res://Scenes/Body Parts/r_shoulder.tscn"},
		{BodyPartEnum.RForearm,	"res://Scenes/Body Parts/r_forearm.tscn"},
		{BodyPartEnum.RHand,	"res://Scenes/Body Parts/r_hand.tscn"},
		
		
		{BodyPartEnum.LLeg,		"res://Scenes/Body Parts/l_leg.tscn"},
		{BodyPartEnum.LCalf,	"res://Scenes/Body Parts/l_calf.tscn"},
		{BodyPartEnum.LFoot,	"res://Scenes/Body Parts/l_foot.tscn"},
		{BodyPartEnum.RLeg,		"res://Scenes/Body Parts/r_leg.tscn"},
		{BodyPartEnum.RCalf,	"res://Scenes/Body Parts/r_calf.tscn"},
		{BodyPartEnum.RFoot,	"res://Scenes/Body Parts/r_foot.tscn"},
		// {BodyPartEnum.Torso,	""},
		// {BodyPartEnum.Heart,	""},
		// {BodyPartEnum.Lung,		""},
		// {BodyPartEnum.Kidney,	""},
		// {BodyPartEnum.Liver,	""}
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
		Kidney,
		Liver
	}
	
}
