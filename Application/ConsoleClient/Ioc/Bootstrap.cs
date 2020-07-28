using Application.ConsoleClient.Validation;
using Application.Validation.Console;
using Domain.PrimaryPort;
using Domain.SecondaryPort;
using Domain.Service;
using Infrastructure.Adapter;
using Infrastructure.Dijkstra;
using System.Collections.Generic;
using Unity;

namespace Application.ConsoleClient.Ioc
{
    public class Bootstrap
    {
        public static IUnityContainer Container;
        public static void Start()
        {
            Container = new UnityContainer();
            Register();
        }

        private static void Register()
        {
            //I Implemented singleton here by using RegisterSingleton.
            Container.RegisterSingleton<IConversionService, ConversionService>();
            Container.RegisterSingleton<IValidationService<IEnumerable<string>>, ValidationService>();
            Container.RegisterSingleton<IShortestPathService, DijkstraAdapter>();
            Container.RegisterSingleton<IDijkstraService<string>, DijkstraService<string>>();
        }
    }
}
