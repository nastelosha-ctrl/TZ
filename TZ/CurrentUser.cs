using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZ
{
    public static class CurrentUser
    {
        public static int Id { get; set; } = -1;
        public static string Login { get; set; } = "";
        public static string FIO { get; set; } = "";
        public static string Role { get; set; } = "";

        public static bool IsAuthenticated => Id != -1 && !string.IsNullOrEmpty(Role);

        public static void Clear()
        {
            Id = -1;
            Login = "";
            FIO = "";
            Role = "";
        }

        public static string GetUserInfo()
        {
            return IsAuthenticated ? $"{FIO} ({Role})" : "Не авторизован";
        }
    }
}
