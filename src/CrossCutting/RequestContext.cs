namespace CrossCutting
{
    public class RequestContext
    {
        public string TenantId { get; set; }
        public string EmpresaId { get; set; }

        public string TraceId { get; set; }
    }
}