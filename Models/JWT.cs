namespace Music_Aditor.Models
{
    public class JWT
    {

        public string Key { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public double DurationInDays { get; set; }
    }
}
