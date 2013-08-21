using System;
using System.Collections.Generic;

namespace SortExpression
{
    internal class ProviderEntry
    {
        private ProviderEntry()
        {
            ProviderTypes = new Dictionary<Type, Type>();
        }

        public ProviderEntry(Type providerType)
            : this()
        {
            IsGenericSpecific = false;
            ProviderTypes.Add(typeof(object), providerType);
        }

        public ProviderEntry(Type genericType, Type providerType)
            : this()
        {
            IsGenericSpecific = true;
            AddProviderType(genericType, providerType);
        }

        public void AddProviderType(Type genericType, Type providerType)
        {
            ProviderTypes.Add(genericType, providerType);
        }

        public bool IsGenericSpecific { get; set; }

        public Dictionary<Type, Type> ProviderTypes { get; private set; }

        public Type GetOrAdd<T>(Func<Type, Type> factory)
        {
            if (IsGenericSpecific)
            {
                if (!ProviderTypes.ContainsKey(typeof(T)))
                {
                    ProviderTypes.Add(typeof(T), factory(typeof(T)));
                }
                return ProviderTypes[typeof(T)];
            }
            return ProviderTypes[typeof(object)];
        }
    }
}