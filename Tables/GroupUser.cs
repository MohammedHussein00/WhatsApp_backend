﻿namespace WhatsApp_Clone.Tables
{
    public class GroupUser
    {
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }


    }
}
