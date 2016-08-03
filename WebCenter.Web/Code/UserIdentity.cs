using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCenter.Entities;

namespace WebCenter.Web
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(LoginResponse signInResponse)
        {
            SignInResponse = signInResponse;
        }

        public LoginResponse SignInResponse
        {
            get;
            private set;
        }

        public string AuthenticationType
        {
            get { return "Basic"; }
        }


        public int id
        {
            get
            {
                return SignInResponse.id;
            }
        }

        public bool IsAuthenticated
        {
            get { return SignInResponse != null; }
        }


        public string username
        {
            get
            {
                return SignInResponse.username;
            }
        }

        public string Name
        {
             get { return SignInResponse.name; }
        }

        public string name
        {
            get
            {
                return SignInResponse.name;
            }
        }
    }
}