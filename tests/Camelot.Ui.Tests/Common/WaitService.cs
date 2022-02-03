using System;
using System.Threading.Tasks;

namespace Camelot.Ui.Tests.Common;

public static class WaitService
{
    public static async Task<bool> WaitForConditionAsync(Func<bool> condition, int delayMs = 50, int maxAttempts = 20)
    {
        for (var i = 0; i < maxAttempts; i++)
        {
            await Task.Delay(delayMs);

            if (condition())
            {
                return true;
            }
        }

        return false;
    }
}