using Godot;
using System;

partial class WanderingMob : Mob
{
    // (x,y) offsets from the mob that will be considered when checking for possible jumps.
	// Only consider positive x-coord jumps; the code will handle negative x-coords.
	// Units are based on _Terrain grid size, so (2, -1) is a jump to reach 2 tiles
    // right and 1 tile above the mob.
	[Export] public Godot.Collections.Array<Vector2I> PotentialJumpCoords { get; set; } = new Godot.Collections.Array<Vector2I>
			{
				new Vector2I( 2, -1 ),
				new Vector2I( 1, -2 )
			};
    // The chance that the WanderingMob will consider jumps when switching from idle to wandering.
    [Export(PropertyHint.Range, "0, 100")]
    public int JumpChance = 0;

    // Only change state (moving/idle) once this procs.
	protected Timer WanderTimer;

    protected bool isJumpEnabled;

    public override void _Ready()
    {
        base._Ready();

        WanderTimer = GetNode<Timer>("WanderTimer");
        isJumpEnabled = false;
    }

    protected override void Move()
    {
        base.Move();
    
        if ( movementDirection != 0 && IsAboutToFallOffLedge() )
            if ( isJumpEnabled )
                Jump();
            else
                movementDirection *= -1;
    }

    protected bool IsAboutToFallOffLedge()  // Pure function; does not edit data
    {
        Vector2 offset = new Vector2( 12 * movementDirection, 8 );
        Vector2I possibleGapMapPos = _Terrain.LocalToMap( _Terrain.ToLocal(GlobalPosition + offset) );
        TileData possibleGap = _Terrain.GetCellTileData( 1, possibleGapMapPos );

        if ( possibleGap == null )  // No layer 1 tile means it is a ledge.
            return true;
        else
            return false;
    }

    public void _OnWanderTimerTimeout()
    {
        WanderTimer.WaitTime = 3 + GD.Randi() % 4;

        if ( movementDirection == 0 )
        {
            if ( GD.Randf() < 0.5f )
                movementDirection = -1;
            else
                movementDirection = 1;

            isJumpEnabled = ( JumpChance > 0 && GD.Randi() % 100 < JumpChance ) ? true : false;
        }
        else
            movementDirection = 0;
    }
}