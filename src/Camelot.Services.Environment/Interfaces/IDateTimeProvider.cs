using System;

namespace Camelot.Services.Environment.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}