using Godot;
using System;

public partial class ui_inventory : Control
{

	TileMap EquipTiles;
	TileMap StorageTiles;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EquipTiles = GetNode<TileMap>("EquipTiles");
		StorageTiles = GetNode<TileMap>("StorageTiles");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
			{
			
				// Vector2 mousePos = GetViewport().GetMousePosition();
				Vector2 mousePos = mb.Position;

				Vector2 equTileLocalPos = mousePos - EquipTiles.Position;
				Vector2 stoTileLocalPos = mousePos - StorageTiles.Position;

				Vector2I equTileMapPos = EquipTiles.LocalToMap( equTileLocalPos / EquipTiles.Scale );
				Vector2I stoTileMapPos = StorageTiles.LocalToMap( stoTileLocalPos / StorageTiles.Scale );

				GD.Print(Name + "has been clicked at " + mousePos);
				GD.Print(EquipTiles.Name + " reports local pos of " + equTileLocalPos + " and map pos of " + equTileMapPos);
				GD.Print(StorageTiles.Name + "reports local pos of " + stoTileLocalPos + " and map pos of " + stoTileMapPos);
			}
		}
	}
}
