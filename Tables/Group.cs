using Music_Aditor.Tables;

namespace WhatsApp_Clone.Tables
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Caption { get; set; }
        public string? Icon { get; set; }
        public string AdminId { get; set; }
        public ICollection<GroupUser>? GroupUsers { get; set; }
        public ICollection<MessageGroup>? MessageGroups { get; set; }  // Messages that reply to this message

    }
}
