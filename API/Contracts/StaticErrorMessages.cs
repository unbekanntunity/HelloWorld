using System.ComponentModel;

namespace API.Contracts
{
    public static class StaticErrorMessages<T>
    {
        public static readonly string NotFound = $"{typeof(T).Name} not found.";

        public static readonly object NotFoundObj = new { erros = new string[] { NotFound } };

        public static readonly string CreateOperationFailed = $"Unable to create {nameof(T)}.";

        public static readonly string UpdateOperationFailed = $"Unable to update {nameof(T)} .";

        public static readonly string DeleteOperationFailed = $"Unable to delete {nameof(T)}.";
    }

    public static class StaticErrorMessages
    {
        public static readonly string PermissionDenied = "Permission denied.";

        public static readonly object PermissionDeniedObj = new { erros = new string[] { PermissionDenied } };
    }
}
