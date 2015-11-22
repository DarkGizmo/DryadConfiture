using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface GameStateInterface
{
	void Activate ();
	void Update ();
	void Deactivate ();
	void OnGUI ();
}

public class GameStateManager : MonoBehaviour
{
    private class GameStateCommand
    {
        public enum CommandType
        {
            Push,
            Pop,
        }

        public CommandType m_Type;
    }

    private class PushGameStateCommand : GameStateCommand
    {
        public GameStateInterface m_State;

        public PushGameStateCommand(GameStateInterface state)
        {
            m_Type = CommandType.Push;
            m_State = state;
        }
    }

    private class PopGameStateCommand : GameStateCommand
    {
        public PopGameStateCommand()
        {
            m_Type = CommandType.Pop;
        }
    }

	private static GameStateManager instance = null;
    public static GameStateManager Instance { get { return instance; } }
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private Stack<GameStateInterface> m_States = new Stack<GameStateInterface>();
    private List<GameStateCommand> m_Commands = new List<GameStateCommand>();

    public bool HasState()
    {
        return m_States.Count > 0;
    }

    public bool HasPendingState()
    {
        int pushPopCount = 0;
        for(int i = 0; i < m_Commands.Count; ++i)
        {
            pushPopCount += (m_Commands[i].m_Type == GameStateCommand.CommandType.Push) ? 1 : -1;
        }

        return pushPopCount > 0;
    }

    public bool IsCurrentStateA<T>()
    {
        return HasState() && m_States.Peek() is T;
    }

    public void PushGameState(GameStateInterface nextState)
    {
        if (nextState != null)
        {
            m_Commands.Add(new PushGameStateCommand(nextState));
        }
    }

    public void PopGameState()
    {
        m_Commands.Add(new PopGameStateCommand());
    }

    public void ClearAllStates()
    {
        for (int i = 0; i < m_States.Count; ++i)
        {
            m_Commands.Add(new PopGameStateCommand());
        }
    }

	void Update ()
	{
        while(m_Commands.Count > 0)
		{
            if(m_Commands[0].m_Type == GameStateCommand.CommandType.Push)
            {
                PushGameStateCommand command = (PushGameStateCommand)m_Commands[0];
                if (HasState())
    			{
                    m_States.Peek().Deactivate();
    			}
                m_States.Push(command.m_State);
                m_States.Peek().Activate();
            }
            else if(m_Commands[0].m_Type == GameStateCommand.CommandType.Pop)
            {
                if (HasState())
                {
                    m_States.Peek().Deactivate();
                    m_States.Pop();
                    if (HasState())
                    {
                        m_States.Peek().Activate();
                    }
                }
            }

            m_Commands.RemoveAt(0);
		}

        if (HasState())
		{
            m_States.Peek().Update ();
		}
	}

	void OnGUI()
	{
        if (HasState())
		{
            m_States.Peek().OnGUI ();
		}
	}
}
