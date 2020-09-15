using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Services
{
    public class CustomJobActivator : JobActivator
    {
        private IServiceProvider _container;

        public CustomJobActivator(IServiceProvider container)
        {
            _container = container;
        }

        public override object ActivateJob(Type type)
        {
            return _container.GetService(type);
        }
    }
}
