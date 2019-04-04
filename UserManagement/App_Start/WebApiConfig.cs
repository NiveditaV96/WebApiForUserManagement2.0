using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using System.Reflection;
using UserManagement.Repository;
using UserManagement.Models;
using System.Resources;
using UserManagement.Model;

namespace UserManagement
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            

            //ResourceManager rm = new ResourceManager("UsingRESX.UserControllerMessages",
            //    Assembly.GetExecutingAssembly());
            //String strWebsite = rm.GetString("Website", CultureInfo.CurrentCulture);
            //String strName = rm.GetString("Name");
            //form1.InnerText = "Website: " + strWebsite + "--Name: " + strName;



            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        public static void SetUpForAutofac(HttpConfiguration config)
        {
            // Web API configuration and services
            var builder = new ContainerBuilder();

            // Register your Web API controllers.
            //uses reflection
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // To register the components with the services offered
            builder.RegisterType<User>();
            builder.RegisterType<UserRepository>().As<IRepository<User>>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IFindUsersRepository>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUpdateUsersRepository>().SingleInstance();

            builder.RegisterType<LoginRepository>().As<ILoginRepository<User>>();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            //check this dot resolve all the contents of the container at a time
            // resolve as when its required
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
