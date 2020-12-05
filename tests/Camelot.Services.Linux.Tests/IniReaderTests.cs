using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class IniReaderTests
    {
        private readonly AutoMocker _autoMocker;

        public IniReaderTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("[Desktop Entry]\nType=Application\nName=System Settings\n", 
            new[] {"Desktop Entry:Type", "Desktop Entry:Name"}, new[] {"Application", "System Settings"})]
        [InlineData("[Desktop Entry]\nType=Application\n Comment\nTest", 
            new[] {"Desktop Entry:Type"}, new[] {"Application"})]
        [InlineData("[Desktop Entry]\nType= Application \t", 
            new[] {"Desktop Entry:Type"}, new[] {"Application"})]
        [InlineData("[Desktop Entry]\nType=\"Application\"", 
            new[] {"Desktop Entry:Type"}, new[] {"Application"})]
        public async Task Read(string ini, string[] keys, string[] values)
        {
            var reader = _autoMocker.CreateInstance<IniReader>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(ini));

            var dictionary = await reader.ReadAsync(stream);
            
            Assert.NotNull(dictionary);
            Assert.Equal(keys.Length, dictionary.Keys.Count());
            
            for (var i = 0; i < keys.Length; i++)
            {
                Assert.True(dictionary.ContainsKey(keys[i]));
                Assert.Equal(values[i], dictionary[keys[i]]);
            }
        }
    }
}