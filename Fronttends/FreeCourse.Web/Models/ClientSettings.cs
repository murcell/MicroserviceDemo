namespace FreeCourse.Web.Models
{
    public class ClientSettings
    {
        public Client WebClient { get; set; }
        public Client WebClientForUsers { get; set; }

    }
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

}
