using System;
using System.Collections.Generic;
using System.Text;

namespace AssesmentOnlineAPI.Test
{
    public static class ApiRoutes
    {
        private static readonly string _baseUrl = "https://localhost:44370/api/";

        public static class Auth
        {
            private static readonly string _authControllerUrl = string.Concat(_baseUrl, "auth");

            public static readonly string Register = string.Concat(_authControllerUrl, "/register");

            public static readonly string Login = string.Concat(_authControllerUrl, "/login");
           
        }

        public static class User
        {
            private static readonly string _usersControllerUrl = string.Concat(_baseUrl, "user");

            public static readonly string GetUser = string.Concat(_usersControllerUrl);

            public static readonly string TransferMoney = string.Concat(_usersControllerUrl, "/transfer");
        }

     
    }
}
