namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries
{
    public class GetActivityProductivityWeightsPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool? IsActive { get; set; }
    }
}
