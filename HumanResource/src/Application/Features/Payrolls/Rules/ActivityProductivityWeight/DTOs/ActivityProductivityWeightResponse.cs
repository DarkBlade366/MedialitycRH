namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs
{
    public class ActivityProductivityWeightResponse
    {
        public Guid Id { get; init; }
        public int RedmineActivityId { get; init; }
        public string ActivityName { get; init; } = string.Empty;
        public decimal Weight { get; init; }
        public bool IsActive { get; init; }
    }
}
