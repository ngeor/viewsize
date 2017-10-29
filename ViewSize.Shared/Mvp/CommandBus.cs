// <copyright file="CommandBus.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    internal class CommandBus : ICommandBus
    {
        private readonly Dictionary<string, List<Action>> subscriptions = new Dictionary<string, List<Action>>();

        public void Publish(string command)
        {
            foreach (var handler in this.subscriptions[command])
            {
                handler();
            }
        }

        public void Subscribe(string command, Action handler)
        {
            var list = this.EnsureSubscriptions(command);
            list.Add(handler);
        }

        private List<Action> EnsureSubscriptions(string command)
        {
            if (this.subscriptions.ContainsKey(command))
            {
                return this.subscriptions[command];
            }

            var list = new List<Action>();
            this.subscriptions.Add(command, list);
            return list;
        }
    }
}
