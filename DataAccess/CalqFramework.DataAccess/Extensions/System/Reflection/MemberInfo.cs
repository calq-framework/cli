using System.Reflection;

namespace CalqFramework.Extensions.System.Reflection {

    public static class MemberInfoExtensions {

        public static Type GetUnderlyingType(this MemberInfo memberInfo) {
            if (memberInfo is FieldInfo fieldInfo) {
                return fieldInfo.FieldType;
            } else if (memberInfo is PropertyInfo propertyInfo) {
                return propertyInfo.PropertyType;
            } else if (memberInfo is MethodInfo methodInfo) {
                return methodInfo.ReturnType;
            } else if (memberInfo is EventInfo eventInfo) {
                return eventInfo.EventHandlerType ?? typeof(void);
            }

            throw new ArgumentException("MemberInfo is not a recognized type");
        }
    }
}
