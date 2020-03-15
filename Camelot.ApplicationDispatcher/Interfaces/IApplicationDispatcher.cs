using System;
using System.Threading.Tasks;

namespace ApplicationDispatcher.Interfaces
{
    public interface IApplicationDispatcher
    {
        void Dispatch(Action action);

        void Dispatch(Func<Task> task);
    }
}