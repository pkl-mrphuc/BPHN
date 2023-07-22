namespace BPHN.BusinessLayer.IServices
{
    public interface IResourceService
    {
        string Get(string key, string lang = "");
    }
}
