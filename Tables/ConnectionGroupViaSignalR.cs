namespace WhatsApp_Clone.Tables
{
    public class ConnectionGroupViaSignalR
    {
       
        public string FirstUserId { get; set; }  // FK to User table

        public string SecondUserId { get; set; } // FK to User table
    }
}
