namespace UniTrack.Domain.Entities
{
    public class EventImage : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsCover { get; set; } // ana görsel mi?
        public int Order { get; set; }    // sıralama
        public Event Event { get; set; }
    }

}
