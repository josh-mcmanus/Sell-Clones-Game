using Godot;
using System;

public partial class MoneyLabel : Label
{
	
	public override void _Process(double delta)
	{
		// This needs to be changed to only update whenever the money is updated, instead of here in process
		
		// Update label
		double money = DataManager.saveData.Money;
		Text = $"$ {Utils.FormatNumber(money)}";
	}
	
}
