using GLSPM.Domain.Dtos;
using System;

namespace GLSPM.Domain
{
    public static class ApplicationConses
    {
        public const string AppName = "Glspm";
        public static class Apis
        {
            public const string Base = "/api";
            public static class Accounts
            {
                public static string Controller = Base + "/Accounts";
                public static string Login = Controller + "/Login";
                public static string Register = Controller + "/Register";
            }
            public static class Passwords
            {
                public static string Controller = Base + "/Passwords";
                public static string GetList(GetListDto input) => Controller + $"?filter={input.Filter}&sorting={input.Sorting}&pagenumber={input.PageNumber}&pagesize={input.PageSize}";
                public static string GetOne(int id) => Controller + $"/{id}";
                public static string Update(int id) => Controller + $"/{id}";
                public static string Delete(int id) => Controller + $"/{id}";
                public static string MoveToTrash(int id) => Controller + $"/Trash/{id}";
                public static string Restore(int id) => Controller + $"/Restore/{id}";
                public static string Create = Controller + "/";
                public static string ChnageLogo = Controller + "/Logo";
                public static string GeneratePassword = Controller + "/Generate";
                public static string GetTrashed(PaginationParametersBase input) => Controller + $"/Trashed?pagenumber={input.PageNumber}&pagesize={input.PageSize}";
            }
        }

        public static class ClientConses
        {
            public const string LocalStorageUserDataKey = "userdata";
        }
    }
}
