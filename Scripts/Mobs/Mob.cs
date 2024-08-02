using Godot;
using System;
using moplestary.players;
using System.Runtime.CompilerServices;

namespace moplestary { namespace mobs {

partial class Mob : CharacterBody2D
{

    [Export] public uint MoveSpeed
    {
        get { return _MoveSpeed; }
        protected set { _MoveSpeed = value; }
    }
	[Export] public uint JumpPower
    {
        get { return _JumpPower; }
        protected set { _JumpPower = value; }
    }
    [Export] public NodePath PlayerPath;
    [Export] public NodePath SpritePath;
    [Export] public NodePath CollisionPath;

	protected readonly float GRAVITY = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    // When dropping down a platform, the Mob ignores _Terrain collision.
    // Once the Mob's Global Y-Axis becomes greater than this number,
    // The Mob will react to _Terrain collision once more.
    protected float dropdownThreshold;

    protected Player Player
    {
        get { return _Player; }
        set
        {
            if ( _Player != null )
                _Player.HMovementChanged -= OnPlayerHMovementChanged;

            _Player = value;
            _Player.HMovementChanged += OnPlayerHMovementChanged;
        }
    }

	protected AnimatedSprite2D Sprite;
	protected CollisionShape2D CollisionShape;

    protected Player _Player;
    protected uint _MoveSpeed = 50;
    protected uint _JumpPower = 150;

	public override void _Ready()
	{
        Player = GetNode<Player>( PlayerPath );
		Sprite = GetNode<AnimatedSprite2D>( SpritePath );
		CollisionShape = GetNode<CollisionShape2D>( CollisionPath );
	}

    public override void _PhysicsProcess(double delta)
	{
		var _velocity = Velocity;

        // Add the gravity.
		if ( !IsOnFloor() )
			_velocity.Y += GRAVITY * (float)delta;
        
        // Check dropdown threshold.
        if ( CollisionShape.Disabled == true )
			if ( GlobalPosition.Y > dropdownThreshold )
				CollisionShape.Disabled = false;
        
        HMovement( ref _velocity, (float)delta );

        if ( Player.Jumping && IsOnFloor() )
            if ( Player.VMovement == 1 )
                DropDown( ref _velocity );
            else
                Jump( ref _velocity );

        // Update sprite stuff.
        SpriteStuff();

        Velocity = _velocity;
        MoveAndSlide();
    }

    protected virtual void HMovement( ref Vector2 velocity, float delta )
    {
        if( Player.HMovement == 0 )
        {
            // For mobs, conserve X-axis velocity until on ground.
            // For players, automatically try to reach 0 velocity in X-axis when airbone.
            // if( IsOnFloor() )   // Keep for mobs, remove for players
			//     velocity.X = Mathf.MoveToward( velocity.X, 0, MoveSpeed*delta );
        }
            // Accelerate toward max speed
            velocity.X = Mathf.MoveToward( velocity.X, Player.HMovement * MoveSpeed, MoveSpeed*delta*10 );
    }

    protected virtual void SpriteStuff()
    {
        if ( !IsOnFloor() )
            Sprite.Play("jump");
        else
            if ( Player.HMovement != 0f )
                Sprite.Play("moving");
            else
                Sprite.Play("idle");
    }

    protected virtual void Jump( ref Vector2 velocity )
    {
		velocity.Y = -JumpPower;
    }

    protected virtual void DropDown( ref Vector2 velocity )
    {
        velocity.Y = -50;
        CollisionShape.Disabled = true;
        dropdownThreshold = GlobalPosition.Y + 2;
    }

    public void TakeDamage( int amount, Node2D attacker, Node2D inflicter )
	{
		GD.Print( Name + " took " + amount + " damage from " + attacker.Name + " using a " + inflicter.Name );
	}

    /// <returns>False if the Mob is atop of a floor collision or holding onto a ladder/rope(TODO). True otherwise.</returns>
    protected bool IsAirborne()
    {
        if ( IsOnFloor() )
            return false;
        return true;
    }

    protected void OnPlayerHMovementChanged( sbyte newVal, sbyte oldVal )
    {
        if ( newVal == -1 )
            Sprite.Scale = new Vector2( -1, 1 );
        else if ( newVal == 1 )
            Sprite.Scale = new Vector2( 1, 1 );
    }

}

} // namespace mobs
} // namespace moplestary