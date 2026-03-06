namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Commands
{
    public class ChangeActivityProductivityWeightStatusCommand
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
