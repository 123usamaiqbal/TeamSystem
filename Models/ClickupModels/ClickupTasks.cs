using Newtonsoft.Json;
using TeamManageSystem.Controllers.Account;


namespace TeamManageSystem.Models.ClickupModels
{
    public class ClickupTasks
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public StatusInfo status { get; set; }
        public string statusvalue { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        public string orderindex { get; set; }
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime? date_created { get; set; }
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime? date_updated { get; set; }
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime? date_closed { get; set; }
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime? date_done { get; set; }
        public string list_id { get; set; }
        public List<ClickupTaskAssignee> assignees { get; set; }
        public PriorityInfo priority { get; set; }
    }

    public class StatusInfo
    {
        public string id { get; set; }
        public string status { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        public string orderindex { get; set; }
    }
    public class PriorityInfo
    {
        public string color { get; set; }
        public string id { get; set; }
        public string orderindex { get; set; }
        public string priority { get; set; }
    }

}
