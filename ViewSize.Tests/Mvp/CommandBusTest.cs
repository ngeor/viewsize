// <copyright file="CommandBusTest.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Collections.Generic;
using CRLFLabs.ViewSize.Mvp;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    public class CommandBusTest
    {
        private CommandBus commandBus;

        [SetUp]
        public void SetUp()
        {
            commandBus = new CommandBus();
        }

        [Test]
        public void Publish_NoSubscriptions_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                commandBus.Publish("hello");
            });
        }

        [Test]
        public void Publish_Consume_Works()
        {
            string result = "original";
            commandBus.Subscribe("hello", () =>
            {
                result = "something else";
            });

            commandBus.Publish("hello");

            Assert.AreEqual("something else", result);
        }
    }
}
