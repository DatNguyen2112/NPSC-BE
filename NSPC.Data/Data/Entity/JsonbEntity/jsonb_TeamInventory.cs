namespace NSPC.Data {
    public class jsonb_TeamInventory
    {
        public int LineNo { get; set; } = 0;
        public string Name { get; set; }
        public Guid Captain { get; set; }
        public string CaptainName  { get; set; }
        public List<Guid> ParticipantsId { get; set; } = new List<Guid>();
        public List<ParticipantsViewModel> Participants { get; set; } = new List<ParticipantsViewModel>();
        public string LineNote { get; set; }
    }

    public class ParticipantsViewModel
    {
        public string ParticipantName { get; set; }
        public string ParticipantAvatarUrl { get; set; }
    }
}

