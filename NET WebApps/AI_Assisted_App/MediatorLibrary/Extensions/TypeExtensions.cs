using System;

namespace MediatorLibrary.Extensions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the given type is assignable to the specified generic type.
        /// </summary>
        /// <param name="givenType">The given type to check.</param>
        /// <param name="genericType">The generic type to check against.</param>
        /// <returns><c>true</c> if the given type is assignable to the generic type; otherwise, <c>false</c>.</returns>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
