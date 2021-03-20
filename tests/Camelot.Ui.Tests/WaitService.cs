using System;
using System.Threading.Tasks;

namespace Camelot.Ui.Tests
{
    public static class WaitService
    {
        public static async Task WaitForConditionAsync(Func<bool> condition, int delayMs = 50, int maxAttempts = 20)
        {
            for (var i = 0; i < maxAttempts; i++)
            {
                await Task.Delay(delayMs);

                if (condition())
                {
                    break;
                }
            }
        }
    }
}