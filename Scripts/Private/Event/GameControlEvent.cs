using System;

public class GameControlEvent : IEvent
{
    private GameControlMessageEvent _message;
    public object data;

    public GameControlEvent(GameControlMessageEvent message)
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
