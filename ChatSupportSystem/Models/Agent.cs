namespace ChatSupportSystem.Models;

public class Agent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public SeniorityLevel Seniority { get; set; }
    public DateTime ShiftEnd { get; set; }
    public int CurrentChats { get; set; } = 0;
    public bool IsAvailable => DateTime.UtcNow < ShiftEnd;

    public double Efficiency => Seniority switch
    {
        SeniorityLevel.Junior => 0.4,
        SeniorityLevel.MidLevel => 0.6,
        SeniorityLevel.Senior => 0.8,
        SeniorityLevel.TeamLead => 0.5,
        _ => 0.4
    };

    public int MaxChats => (int)(10 * Efficiency);
}
