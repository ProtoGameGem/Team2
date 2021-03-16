using System;

public enum ActiveState
{
    None = (1<<0),
    Manual = (1<<1),
    AI = (1<<2)
}

public enum ActionState
{
    Idle = (1<<0),
    Move = (1<<1),
    Dash = (1<<2),
    Jump = (1<<3)
}

public interface ICharacter
{
    ActiveState ActiveState { get; set; }
    ActionState ActionState { get; set; }
}

public static class ICharacterExtensions
{
    public static void ConvertToManualState(this ICharacter thisCharacter)
    {
        var character = thisCharacter as Character;
        if (character == null)
        {
            throw new System.NotImplementedException();
        }
        else
        {
            thisCharacter.ActiveState = ActiveState.Manual;
        }
    }

    public static void ConvertToDisableState(this ICharacter thisCharacter)
    {
        var character = thisCharacter as Character;
        if (character == null)
        {
            throw new System.NotImplementedException();
        }
        else
        {
            thisCharacter.ActiveState = ActiveState.None;
        }
    }
}
