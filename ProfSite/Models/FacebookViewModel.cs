
namespace ProfSite.Models
{
    public class FacebookViewModel
    {
        public bool IsFacebookLinked { get; set; }
        public string UserName { get; set; }
        public string FacebookName { get; set; }
        public FacebookTimeline Timeline { get; set; }
    }
}