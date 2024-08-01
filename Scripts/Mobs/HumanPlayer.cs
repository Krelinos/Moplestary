using Godot;
using System;
using System.Collections.Generic;

namespace moplestary
{
	namespace players
	{        
/// <summary>
/// The intelligence that issues commands to a Mobile. Both human users
/// and server-controlled AIs inherit from this class.
/// </summary>
		abstract partial class Player : Node
		{
            [Signal] public delegate void HMovementChangedEventHandler( sbyte newVal, sbyte oldVal );
            [Signal] public delegate void VMovementChangedEventHandler( sbyte newVal, sbyte oldVal );
            [Signal] public delegate void JumpChangedEventHandler( bool isJumping );

			[Export] public NodePath Mobile { get; protected set; }

            public sbyte HMovement
            {
                get { return _HMovement; }
                protected set
                {
                    // Clamp between -1 and 1 inclusive
                    var newVal = Math.Max( (sbyte)-1, Math.Min( (sbyte)1, value) );
                    if ( newVal != _HMovement )
                    {
                        var oldVal = _HMovement;
                        _HMovement = newVal;
                        EmitSignal( nameof(HMovementChanged), newVal, oldVal );
                    }
                }
            }
            public sbyte VMovement
            {
                get { return _VMovement; }
                protected set
                {
                    // Clamp between -1 and 1 inclusive
                    var newVal = Math.Max( (sbyte)-1, Math.Min( (sbyte)1, value) );
                    if ( newVal != _VMovement )
                    {
                        var oldVal = _VMovement;
                        _VMovement = newVal;
                        EmitSignal( nameof(HMovementChanged), newVal, oldVal );
                    }
                }
            }
            public bool Jumping
            {
                get { return _Jumping; }
                protected set
                {
                    if ( _Jumping != value )
                    {
                        _Jumping = value;
                        EmitSignal( nameof(JumpChangedEventHandler), _Jumping );
                    }
                }
            }

            protected sbyte _HMovement;
            protected sbyte _VMovement;
            protected bool _Jumping;

			protected Player()
			{
				
			}
		}

/// <summary>
/// The interface medium in which a human user controls a Mobile. Uses Godot's Input system.
/// </summary>
        partial class HumanPlayer : Player
		{
            protected const String INPUT_MOVE_LEFT  = "MoveLeft";
            protected const String INPUT_MOVE_RIGHT = "MoveRight";
            protected const String INPUT_MOVE_UP    = "MoveUp";
            protected const String INPUT_MOVE_DOWN  = "MoveDown";
            protected const String INPUT_MOVE_JUMP  = "MoveJump";

			HumanPlayer()
			{
				// GD.Print( InputMap.GetActions() );
                
			}

            public override void _Input( InputEvent _event )
            {
                base._Input( _event );
                
                // This is a small system to prevent a character from standing still when
                // both left and right movement keys are pressed. The last pressed key will
                // be the direction the HumanPlayer will transmit. Based on "null movement
                // scripts" for Team Fortress 2.
                if ( Input.IsActionJustPressed(INPUT_MOVE_LEFT) )
                    _HMovement = -1;
                else if ( Input.IsActionJustReleased(INPUT_MOVE_LEFT) )
                    if ( Input.IsActionPressed(INPUT_MOVE_RIGHT) )
                        _HMovement = 1;
                    else
                        _HMovement = 0;
                
                if ( Input.IsActionJustPressed(INPUT_MOVE_RIGHT) )
                    _HMovement = 1;
                else if ( Input.IsActionJustReleased(INPUT_MOVE_RIGHT) )
                    if ( Input.IsActionPressed(INPUT_MOVE_LEFT) )
                        _HMovement = -1;
                    else
                        _HMovement = 0;
                
                // Vertical null movement ( Note: Up vector is (0,-1) in Godot )
                if ( Input.IsActionJustPressed(INPUT_MOVE_UP) )
                    _VMovement = -1;
                else if ( Input.IsActionJustReleased(INPUT_MOVE_UP) )
                    if ( Input.IsActionPressed(INPUT_MOVE_DOWN) )
                        _VMovement = 1;
                    else
                        _VMovement = 0;
                
                if ( Input.IsActionJustPressed(INPUT_MOVE_DOWN) )
                    _VMovement = 1;
                else if ( Input.IsActionJustReleased(INPUT_MOVE_DOWN) )
                    if ( Input.IsActionPressed(INPUT_MOVE_UP) )
                        _VMovement = -1;
                    else
                        _VMovement = 0;
                
                // Input consideration for jumping is after vertical movement as
                // players trying to dropdown can hit down and jump on the same frame.
                if ( Input.IsActionJustPressed(INPUT_MOVE_JUMP) )
                    Jumping = true;
                else if ( Input.IsActionJustReleased(INPUT_MOVE_JUMP) )
                    Jumping = false;
            }
        }

    } // namespace players
} // namespace moplestary