using Godot;
using System;

public class StateMachine
{
    public enum ProcessState
    {
        Idle,
        Moving,
        Airborne,
        Dying
    }

    public enum Command
    {
        Begin,
        End,
        Pause,
        Resume,
        Exit
    }
}