namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Owin.Hosting;

    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;

    using Owin;

    public class Program
    {
        static void Main(string[] args)
        {
            var options = new StartOptions
            {
                ServerFactory = "Nowin",
                Port = 9090
            };

            using (WebApp.Start<Startup>(options))
            {
                Console.WriteLine("Running a http server on port 9090");
                Console.ReadKey();
            }
        }
    }

    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {


            app.UseNancy(options => options.Bootstrapper = new MyBootstrapper());

        }
    }

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => "Hi";
        }
    }

    public class MyBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            container.Register<IBlah, GlobalBlah>();
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            var blah = container.Resolve<IBlah>();
            Console.WriteLine("This should be Blan NOT GlobalBlah");
            Console.WriteLine(blah.GetType());
        }
    }


    public class MyClass : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                return new[] { new TypeRegistration(typeof(IBlah), typeof(Blah), Lifetime.PerRequest), };
            }
        }
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; private set; }
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }
    }

    public class GlobalBlah : IBlah
    {
        public void DoSomething()
        {
            Console.WriteLine("GLOBAL BLAH");
        }
    }

    public class Blah : IBlah
    {
        public void DoSomething()
        {
            Console.WriteLine("BLAH");
        }
    }

    public interface IBlah
    {
        void DoSomething();
    }
}
