﻿using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ixq.Demo.DbContext;
using Ixq.Core;
using Ixq.Core.DependencyInjection.Extensions;
using Ixq.Demo.Domain;
using Ixq.Demo.Entities;
using Ixq.DependencyInjection.Autofac;
using Ixq.Extensions;
using Ixq.Mapper;
using Ixq.Mapper.AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Ixq.Demo.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var app = AppBootProgram.Instance;
            app.Initialization()
                .RegisterAutoMappe()
                .RegisterAutofac(typeof (MvcApplication));
        }
    }
}