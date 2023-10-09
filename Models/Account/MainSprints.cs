namespace TeamManageSystem.Models.Account

{
    public class MainSprints
    {
        public List<Sprint> Sprints { get; set; }
        public List<SprintRating> SprintRatings { get; set; }
        public List<TMembers> TeaMembers { get; set; }
        public SprintDateModel SprintDateModel { get; set; }

    }
}
