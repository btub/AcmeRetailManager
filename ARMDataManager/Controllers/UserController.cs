﻿using AcmeDataManager.Library.DataAccess;
using AcmeDataManager.Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ARMDataManager.Controllers
{
    [Authorize]
  //  [RoutePrefix("api/User")]
    public class UserController : ApiController
    {

        // GET: User/Details/5
        public List<UserModel> GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            
            UserData data = new UserData();

            return data.GetUserById(userId);
        }
    }
}