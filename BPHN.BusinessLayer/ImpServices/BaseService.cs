using AutoMapper;
using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BaseService : IBaseService
    {
        protected readonly IContextService _contextService;
        protected readonly ICacheService _cacheService;
        protected readonly IResourceService _resourceService;
        protected readonly IGlobalVariableService _globalVariableService;
        protected readonly IMapper _mapper;
        protected readonly AppSettings _appSettings;

        public BaseService(IServiceProvider provider, IOptions<AppSettings> appSettings)
        {
            _contextService = provider.GetRequiredService<IContextService>();
            _cacheService = provider.GetRequiredService<ICacheService>();
            _resourceService = provider.GetRequiredService<IResourceService>();
            _globalVariableService = provider.GetRequiredService<IGlobalVariableService>();
            _mapper = provider.GetRequiredService<IMapper>();
            _appSettings = appSettings.Value;
        }
        public virtual bool ValidateModelByAttribute(object model, params string[] ignoreProperties)
        {
            var validateProperties = model
                .GetType()
                .GetProperties()
                .Where(item => item.CustomAttributes.Count() > 0 && !ignoreProperties.Contains(item.Name)).ToList();

            for (int i = 0; i < validateProperties.Count; i++)
            {
                var property = validateProperties[i];
                var customAttributes = property.GetCustomAttributes().ToList();
                for (int j = 0; j < customAttributes.Count; j++)
                {
                    bool isValid = true;
                    var value = property.GetValue(model, null);

                    var attribute = customAttributes[j];
                    var namePropertyOfAttribute = attribute.TypeId.GetType().GetProperty("Name") ?? null;
                    if (namePropertyOfAttribute is not null)
                    {
                        var nameAttribute = namePropertyOfAttribute.GetValue(attribute.TypeId, null);
                        if (nameAttribute is not null)
                        {
                            switch (nameAttribute.ToString())
                            {
                                case "RequiredAttribute":
                                    isValid = ValidateRequiredValue(value, property, attribute);
                                    break;
                                case "MaxLengthAttribute":
                                    isValid = ValidateMaxLengthValue(value, property, attribute);
                                    break;
                                case "EmailAddressAttribute":
                                    isValid = ValidateEmailAddressValue(value, property, attribute);
                                    break;
                            }
                        }

                    }

                    if (!isValid) return false;
                }
            }

            return true;
        }

        private bool ValidateRequiredValue(object? value, PropertyInfo property, Attribute attribute)
        {
            if (value is null) return false;
            if (property.PropertyType == typeof(string) && string.IsNullOrWhiteSpace(value.ToString())) return false;
            return true;
        }

        private bool ValidateMaxLengthValue(object? value, PropertyInfo property, Attribute attribute)
        {
            var lengthProperty = attribute.GetType().GetProperty("Length");
            if (lengthProperty is not null)
            {
                int maxLength = Convert.ToInt32(lengthProperty.GetValue(attribute, null));
                if (property.PropertyType == typeof(string) && maxLength >= 0)
                {
                    if (value is not null && value.GetType() == typeof(string))
                    {
                        string strValue = (string)value;
                        if (strValue.Length > maxLength)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool ValidateEmailAddressValue(object? value, PropertyInfo property, Attribute attribute)
        {
            return true;
        }

        public virtual string BuildLinkDescription(Guid historyLogId)
        {
            var description = $"{historyLogId}";
            return description;
        }

    }
}
