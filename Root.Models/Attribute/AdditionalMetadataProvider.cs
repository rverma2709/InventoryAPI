using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Attribute
{
    public class AdditionalMetadataProvider : IDisplayMetadataProvider
    {

        public AdditionalMetadataProvider() { }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            // Extract all AdditionalMetadataAttribute values and add to AdditionalValues
            // Why oh why was this omitted from MVC6????
            if (context.PropertyAttributes != null)
            {
                foreach (object propAttr in context.PropertyAttributes)
                {
                    AdditionalMetadataAttribute addMetaAttr = propAttr as AdditionalMetadataAttribute;
                    if (addMetaAttr != null)
                    {
                        context.DisplayMetadata.AdditionalValues.Add(addMetaAttr.Name, addMetaAttr.Value);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class AdditionalMetadataAttribute : System.Attribute
    {

        public AdditionalMetadataAttribute(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; private set; }
    }
}
