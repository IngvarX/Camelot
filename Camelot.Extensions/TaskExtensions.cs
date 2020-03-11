using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Camelot.Extensions
{
    public static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }
        }
    }
}