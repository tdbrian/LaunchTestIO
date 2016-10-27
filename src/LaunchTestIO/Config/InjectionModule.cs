using Autofac;

namespace LaunchTestIO.Config
{
    public class InjectionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ILaunchTestIoContext>().As<LaunchTestIoContext>().SingleInstance();
        }
    }
}