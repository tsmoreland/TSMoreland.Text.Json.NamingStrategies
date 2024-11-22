using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace TSMoreland.Text.Json.NamingStrategies;

public class EnumModelBinderProvider : IModelBinderProvider
{
    private readonly Hashtable _typesWithFlagsAttribute = Hashtable.Synchronized([]);

    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        return context.Metadata.ModelType.IsEnum && !HasFlagsAttribute(context.Metadata.ModelType)
            ? new BinderTypeModelBinder(typeof(EnumModelBinder))
            : null;
    }

    private bool HasFlagsAttribute(MemberInfo type)
    {
        if (_typesWithFlagsAttribute[type] is bool hasFlag)
        {
            return hasFlag;
        }

        hasFlag = type.GetCustomAttribute<FlagsAttribute>() is not null;
        _typesWithFlagsAttribute[type] = hasFlag;
        return hasFlag;
    }
}
