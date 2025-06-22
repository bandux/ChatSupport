namespace ChatSupportSystem.Models;

public class ChatSession
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public bool IsActive { get; set; } = true;
	public int MissedPolls { get; set; } = 0;
	public Guid? AssignedAgentId { get; set; }
    public DateTime? LastPolledAt { get; set; }

}
