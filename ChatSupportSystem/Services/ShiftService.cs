using ChatSupportSystem.Models;

namespace ChatSupportSystem.Services;

public class ShiftService
{
    private readonly List<Agent> _teamA;
    private readonly List<Agent> _teamB;
    private readonly List<Agent> _teamC;
    private readonly List<Agent> _overflowTeam;

    public ShiftService()
    {
        _teamA = SeedTeamA();
        _teamB = SeedTeamB();
        _teamC = SeedTeamC();
        _overflowTeam = SeedOverflowTeam();
    }

    public virtual bool IsOfficeHours()
    {
        var now = DateTime.UtcNow.TimeOfDay;
        return now >= TimeSpan.FromHours(9) && now <= TimeSpan.FromHours(17);
    }

    public virtual List<Agent> GetActiveAgents()
    {
        var now = DateTime.UtcNow;
        var hour = now.Hour;

        if (hour >= 8 && hour < 16) return _teamA;
        if (hour >= 16 && hour < 24) return _teamB;
        return _teamC; // midnight to 8am
    }

    public virtual List<Agent> GetAvailableOverflowAgents()
    {
        return IsOfficeHours() ? _overflowTeam : new List<Agent>();
    }

    public virtual List<Agent> GetAllAgents() =>
        _teamA.Concat(_teamB).Concat(_teamC).Concat(_overflowTeam).ToList();

    #region Seed Data
    private List<Agent> SeedTeamA() => new()
    {
        new Agent { Name = "Lead A", Seniority = SeniorityLevel.TeamLead, ShiftEnd = DateTime.UtcNow.AddHours(4) },
        new Agent { Name = "Mid A1", Seniority = SeniorityLevel.MidLevel, ShiftEnd = DateTime.UtcNow.AddHours(4) },
        new Agent { Name = "Mid A2", Seniority = SeniorityLevel.MidLevel, ShiftEnd = DateTime.UtcNow.AddHours(4) },
        new Agent { Name = "Junior A", Seniority = SeniorityLevel.Junior, ShiftEnd = DateTime.UtcNow.AddHours(4) }
    };

    private List<Agent> SeedTeamB() => new()
    {
        new Agent { Name = "Senior B", Seniority = SeniorityLevel.Senior, ShiftEnd = DateTime.UtcNow.AddHours(12) },
        new Agent { Name = "Mid B", Seniority = SeniorityLevel.MidLevel, ShiftEnd = DateTime.UtcNow.AddHours(12) },
        new Agent { Name = "Junior B1", Seniority = SeniorityLevel.Junior, ShiftEnd = DateTime.UtcNow.AddHours(12) },
        new Agent { Name = "Junior B2", Seniority = SeniorityLevel.Junior, ShiftEnd = DateTime.UtcNow.AddHours(12) }
    };

    private List<Agent> SeedTeamC() => new()
    {
        new Agent { Name = "Mid C1", Seniority = SeniorityLevel.MidLevel, ShiftEnd = DateTime.UtcNow.AddHours(8) },
        new Agent { Name = "Mid C2", Seniority = SeniorityLevel.MidLevel, ShiftEnd = DateTime.UtcNow.AddHours(8) }
    };

    private List<Agent> SeedOverflowTeam() => Enumerable.Range(1, 6)
        .Select(i => new Agent
        {
            Name = $"Overflow Agent {i}",
            Seniority = SeniorityLevel.Junior,
            ShiftEnd = DateTime.UtcNow.AddHours(8)
        }).ToList();
    #endregion
}
