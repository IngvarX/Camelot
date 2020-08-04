using System;
using System.Linq;
using Camelot.Services.Windows.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsTrashCanNodeNameGenerator : IWindowsTrashCanNodeNameGenerator
    {
        private const string FileNameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int FileNameLength = 6;

        private readonly Random _random;

        public WindowsTrashCanNodeNameGenerator()
        {
            _random = new Random();
        }

        public string Generate() =>
            new string(
                Enumerable
                    .Repeat(FileNameChars, FileNameLength)
                    .Select(s => s[_random.Next(s.Length)])
                    .ToArray()
            );
    }
}