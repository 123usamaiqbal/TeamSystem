namespace TeamManageSystem.Models.Account
{
    public class Dashboard
    {

        public List<TMembers> MembersModel { get; set; }
        public List<Sprint> SprintsModel { get; set; }
        public List<Rating> RatingsModel { get; set; }
        public List<User> UsersModel { get; set; }
        public User usermodel { get; set; }

    }
}
