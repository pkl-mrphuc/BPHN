namespace BPHN.ModelLayer.Responses
{
    public sealed class LoginRespond
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public RoleEnum Role { get; set; }
        public Guid? ParentId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public IEnumerable<Guid> RelationIds { get; set; }
        public string AvatarUrl { get; set; }
    }
}
