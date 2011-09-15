using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Bar;
using Foo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foo
{
    public class MyItem
    {
        public string Name { get; set; }
    }
    public interface IRepository
    {
        MyItem GetItem();
    }
    public interface ILogger
    {
        void Log(string message);
    }
}
namespace Bar
{
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            throw new NotImplementedException();
        }
    }
    public class MyService
    {
        private readonly Logger _logger;

        public MyService(Logger logger)
        {
            _logger = logger;
        }
    }
    public class Repository : IRepository
    {
        private readonly ILogger _logger;

        public Repository(ILogger logger)
        {
            _logger = logger;
        }

        public MyItem GetItem()
        {
            throw new NotImplementedException();
        }
    }
}
namespace Alun.Autofac.Validation.Test
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void DemonstrateIntenedUsage()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Repository>().As<IRepository>();
            builder.RegisterType<MyService>();
            var container = builder.Build();

            container.AssertConfigurationIsValid(Assert.Fail);
        }

        [TestMethod]
        public void FindsMissingInterfaceDependancy()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Repository>().As<IRepository>();
            var container = builder.Build();

            var result = container.FindConfigurationProblems();
            Assert.IsTrue(result[0].Message.Contains("None of the constructors found with 'Public binding flags' on type 'Bar.Repository' can be invoked with the available services and parameters"), result[0].Message);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void FindsMissingConcreteDependancy()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyService>();
            var container = builder.Build();

            var result = container.FindConfigurationProblems();
            Assert.IsTrue(result[0].Message.Contains("None of the constructors found with 'Public binding flags' on type 'Bar.MyService' can be invoked with the available services and parameters"), result[0].Message);
            Assert.AreEqual(1, result.Count);            
        }
    }
}
