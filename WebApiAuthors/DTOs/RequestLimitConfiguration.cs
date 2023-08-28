namespace WebApiAuthors.DTOs
{
    public class RequestLimitConfiguration
    {
        public int FreeRequestsPerDay { get; set; }
        public string[] RouteWhiteList { get; set; }
    }
}
