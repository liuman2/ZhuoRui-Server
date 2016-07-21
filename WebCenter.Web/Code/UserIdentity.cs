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

        public int? company_id
        {
            get
            {
                return SignInResponse.company_id;
            }
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

        public int? is_admin
        {
            get
            {
                return SignInResponse.is_admin;
            }
        }

        public string mobile
        {
            get
            {
                return SignInResponse.mobile;
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

        public string picture_url
        {
            get
            {
                return SignInResponse.picture_url;
            }
        }

        public int? status
        {
            get
            {
                return SignInResponse.status;
            }
        }
    }
}