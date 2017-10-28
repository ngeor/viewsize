using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    class CommandBus : ICommandBus
    {
        private readonly Dictionary<string, List<Action>> _subscriptions = new Dictionary<string, List<Action>>();

        public void Publish(string command)
        {
            foreach (var handler in _subscriptions[command])
            {
                handler();
            }
        }

        public void Subscribe(string command, Action handler)
        {
            var list = EnsureSubscriptions(command);
            list.Add(handler);
        }

        private List<Action> EnsureSubscriptions(string command)
        {
            if (_subscriptions.ContainsKey(command))
            {
                return _subscriptions[command];
            }

            var list = new List<Action>();
            _subscriptions.Add(command, list);
            return list;
        }
    }
}
