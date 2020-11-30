using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class MimeTypesReaderTests
    {
        private readonly AutoMocker _autoMocker;

        public MimeTypesReaderTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("  ", null, new string[0])]
        [InlineData("", null, new string[0])]
        [InlineData("\t    \t\n", null, new string[0])]
        [InlineData("application\t  tests  \t\n", null, new string[0])]
        [InlineData("application/jpeg", "application/jpeg", new string[0])]
        [InlineData("application/jpeg\t  jpg jpeg  \t\n", "application/jpeg", new[] {"jpg", "jpeg"})]
        [InlineData("application/jpeg\t  jpg  \t\napplicAtion/jpEg jpeg", "application/jpeg", new[] {"jpg", "jpeg"})]
        [InlineData("1application/jpeg\t  jpg  \t", null, new string[0])]
        [InlineData("  \t  application/jpeg jpeg  ", "application/jpeg", new[] {"jpeg"})]
        public async Task TestRead(string data, string key, string[] values)
        {
            var service = _autoMocker.CreateInstance<MimeTypesReader>();

            var bytes = Encoding.UTF8.GetBytes(data);
            await using var memoryStream = new MemoryStream(bytes);
            var dictionary = await service.ReadAsync(memoryStream);

            Assert.NotNull(dictionary);
            if (key is null)
            {
                Assert.Empty(dictionary);
            }
            else
            {
                Assert.Single(dictionary.Keys);
                Assert.Equal(key, dictionary.Keys.Single());
                var actualValues = dictionary[key];
                Assert.Equal(values.Length, actualValues.Count);
                Assert.True(values.All(v => actualValues.Contains(v)));
            }
        }
    }
}