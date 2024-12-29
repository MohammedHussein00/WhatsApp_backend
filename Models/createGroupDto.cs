namespace Music_Aditor.Models
{
    public class createGroupDto
    {
        public int Id { get; set; }
        public string AdminId { get; set; }
        public string GroupName { get; set; }
        public string? GroupCaption { get; set; }
        public IFormFile? GroupIcon { get; set; }
        public List<string> GroupMembers { get; set; }
    }
}
