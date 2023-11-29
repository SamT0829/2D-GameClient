using System;

public class GameUIEvent : IEvent
{
    private GameUIMessageEvent _message;

    public GameUIEvent(GameUIMessageEvent message)
    {
        _message = message;
    }

    public int GetMessageKey()
    {
        return Convert.ToInt32(_message);
    }

    public bool GetSendAll()
    {
        return false;
    }
}
