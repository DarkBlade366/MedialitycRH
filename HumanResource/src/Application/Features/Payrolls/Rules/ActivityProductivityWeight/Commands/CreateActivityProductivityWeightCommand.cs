namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Commands
{
    public class CreateActivityProductivityWeightCommand
    {
        public int RedmineActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public decimal Weight { get; set; }
    }
}
