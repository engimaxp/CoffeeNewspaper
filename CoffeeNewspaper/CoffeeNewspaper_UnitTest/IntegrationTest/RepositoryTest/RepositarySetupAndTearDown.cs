﻿using System;
using System.Threading.Tasks;
using CN_Core;
using CN_Repository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.RepositoryTest
{
    [TestFixture]
    public class RepositarySetupAndTearDown
    {
        [OneTimeSetUp]
        public void IntegretestSetup()
        {
            //Bind the Basic Ioc Container once per run
            IoC.Setup();
        }

        /// <summary>
        ///     derivative class use this function to to use the database link
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public async Task UseMemoryContextRun(Func<CNDbContext, Task> function)
        {
            //In-Memory sqlite db will vanish per connection
            using (var context = CNDbContext.GetMemorySqlDatabase())
            {
                if (context == null) return;
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                //Do that task
                await function.Invoke(context);

                context.Database.CloseConnection();
            }
        }
    }
}