namespace BPHN.ModelLayer.Responses
{
    public sealed class AccountRespond
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid Id { get; set; }
        public RoleEnum Role { get; set; }
        public string Status { get; set; }
        public Guid? ParentId { get; set; }
        public string AvatarUrl { get; set; }
    }
}
