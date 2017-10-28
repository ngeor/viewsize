using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface ICommandBus
    {
        void Subscribe(string command, Action handler);
        void Publish(string command);
    }
}
