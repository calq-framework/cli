namespace CalqFramework.DataAccess.Extensions.System.Reflection;

public static class MemberInfoExtensions {
    public static Type GetUnderlyingType(this MemberInfo memberInfo) {
        if (memberInfo is FieldInfo fieldInfo) {
            return fieldInfo.FieldType;
        }

        if (memberInfo is PropertyInfo propertyInfo) {
            return propertyInfo.PropertyType;
        }

        if (memberInfo is MethodInfo methodInfo) {
            return methodInfo.ReturnType;
        }

        if (memberInfo is EventInfo eventInfo) {
            return eventInfo.EventHandlerType ?? typeof(void);
        }

        throw DataAccessErrors.UnrecognizedMemberType();
    }
}
