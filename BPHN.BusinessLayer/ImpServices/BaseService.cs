using BPHN.BusinessLayer.IServices;
using System.Reflection;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BaseService : IBaseService
    {
        public virtual bool ValidateModelByAttribute(object model, List<string> ignoreProperties)
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
                    if(namePropertyOfAttribute != null)
                    {
                        var nameAttribute = namePropertyOfAttribute.GetValue(attribute.TypeId, null);
                        if(nameAttribute != null)
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
            if (value == null) return false;
            if (property.PropertyType == typeof(string) && string.IsNullOrEmpty(value.ToString())) return false;
            return true;
        }

        private bool ValidateMaxLengthValue(object? value, PropertyInfo property, Attribute attribute)
        {
            var lengthProperty = attribute.GetType().GetProperty("Length");
            if(lengthProperty != null)
            {
                int maxLength = Convert.ToInt32(lengthProperty.GetValue(attribute, null));
                if (property.PropertyType == typeof(string) && maxLength >= 0)
                {
                    if (value != null && value.GetType() == typeof(string))
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
