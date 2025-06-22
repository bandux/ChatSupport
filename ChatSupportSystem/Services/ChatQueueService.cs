using ChatSupportSystem.Models;

namespace ChatSupportSystem.Services;

public class ChatQueueService
{
    private readonly List<ChatSession> _allSessions = new(); 
    private readonly List<Agent> _agents;
    private readonly List<Agent> _overflowAgents;

    public ChatQueueService()
    {
        _agents = SeedMainTeam();
        _overflowAgents = SeedOverflowTeam();
    }

    public (bool IsAccepted, string Message, ChatSession? Session) EnqueueChatSession(Guid sessionId)
    {
        int capacity = CalculateCapacity(_agents);
        int maxQueueLength = (int)(capacity * 1.5);

        var currentWaitingCount = _allSessions.Count(s => s.IsActive && s.AssignedAgentId == null);

        if (currentWaitingCount >= maxQueueLength)
        {
            if (IsOfficeHours())
            {
                int overflowCapacity = CalculateCapacity(_overflowAgents);
                if (currentWaitingCount < maxQueueLength + (int)(overflowCapacity * 1.5))
                {
                    return AcceptSession(sessionId, useOverflow: true);
                }

                return (false, "Queue full even with overflow", null);
            }

            return (false, "Queue full", null);
        }

        return AcceptSession(sessionId);
    }

    private (bool, string, ChatSession?) AcceptSession(Guid sessionId, bool useOverflow = false)
    {
        var session = new ChatSession
        {
            Id = sessionId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _allSessions.Add(session);

        AssignToAvailableAgent(session, useOverflow ? _overflowAgents : _agents);
        return (true, "Chat accepted", session);
    }

    private void AssignToAvailableAgent(ChatSession session, List<Agent> agentPool)
    {
        foreach (var agent in agentPool)
        {
            if (!agent.IsAvailable) continue;
            if (agent.CurrentChats < agent.MaxChats)
            {
                session.AssignedAgentId = agent.Id;
                agent.CurrentChats++;
                break;
            }
        }
    }

    private int CalculateCapacity(List<Agent> team) =>
        team.Sum(a => a.IsAvailable ? a.MaxChats : 0);

    private bool IsOfficeHours()
    {
        var now = DateTime.UtcNow.TimeOfDay;
        return now >= TimeSpan.FromHours(9) && now <= TimeSpan.FromHours(17);
    }

    private List<Agent> SeedMainTeam() => new()
    {
        new Agent { Name = "Alice", Seniority = SeniorityLevel.Senior, ShiftEnd = DateTime.UtcNow.AddHours(3) },
        new Agent { Name = "Bob", Seniority = SeniorityLevel.MidLevel, ShiftEnd = DateTime.UtcNow.AddHours(2) }
    };

    private List<Agent> SeedOverflowTeam() => new()
    {
        new Agent { Name = "John (Overflow)", Seniority = SeniorityLevel.Junior, ShiftEnd = DateTime.UtcNow.AddHours(8) }
    };

    public ChatSession? GetSessionById(Guid sessionId)
    {
        return _allSessions.FirstOrDefault(s => s.Id == sessionId);
    }

    public void CheckPollTimeouts()
    {
        foreach (var session in _allSessions.Where(s => s.IsActive && s.AssignedAgentId != null))
        {
            if (session.LastPolledAt == null)
            {
                session.MissedPolls++;
            }
            else if ((DateTime.UtcNow - session.LastPolledAt.Value).TotalSeconds >= 2)
            {
                session.MissedPolls++;
                session.LastPolledAt = null;
            }

            if (session.MissedPolls >= 3)
            {
                session.IsActive = false;
                Console.WriteLine($"[Polling] Session {session.Id} marked as INACTIVE due to missed polls.");
            }
        }
    }


    public List<ChatSession> GetAllSessions() => _allSessions;
}
