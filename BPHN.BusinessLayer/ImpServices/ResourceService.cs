using BPHN.BusinessLayer.IServices;
using System.Reflection;
using System.Resources;

namespace BPHN.BusinessLayer.ImpServices
{
    public class ResourceService : IResourceService
    {
        private Dictionary<string, ResourceManager> _resourceManager = new Dictionary<string, ResourceManager>();
        private const string NAMESPACE = "BPHN.BusinessLayer.Resources.SharedResource";
        public ResourceService()
        {
            var culture = new string[] { "vi", "en" };
            for (int i = 0; i < culture.Length; i++)
            {
                _resourceManager.Add(culture[i], new ResourceManager($"{NAMESPACE}_{culture[i]}", Assembly.GetExecutingAssembly()));
            }
        }

        public string Get(string key, string lang = "")
        {
            var _currentResource =  string.IsNullOrWhiteSpace(lang) || 
                                    !_resourceManager.ContainsKey(lang) ? _resourceManager["vi"] : _resourceManager[lang];
            return _currentResource.GetString(key) ?? key;
        }
    }
}
