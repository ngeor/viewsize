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
            this.commandBus = new CommandBus();
        }

        [Test]
        public void Publish_NoSubscriptions_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                this.commandBus.Publish("hello");
            });
        }

        [Test]
        public void Publish_Consume_Works()
        {
            string result = "original";
            this.commandBus.Subscribe("hello", () =>
            {
                result = "something else";
            });

            this.commandBus.Publish("hello");

            Assert.AreEqual("something else", result);
        }
    }
}
