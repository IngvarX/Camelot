using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Implementations;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class ArchiveTypeViewModelFactoryTests
    {
        private readonly AutoMocker _autoMocker;

        public ArchiveTypeViewModelFactoryTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestCreateForSingleFile()
        {
            var config = new ArchiveTypeViewModelFactoryConfiguration
            {
                SingleFileArchiveTypes = new Dictionary<ArchiveType, string>
                {
                    [ArchiveType.Gz] = "Gz",
                    [ArchiveType.Tar] = "Tar",
                    [ArchiveType.Zip] = "Zip"
                }
            };
            _autoMocker.Use(config);

            var factory = _autoMocker.CreateInstance<ArchiveTypeViewModelFactory>();
            var viewModels = factory.CreateForSingleFile();

            Assert.NotNull(viewModels);
            Assert.Equal(config.SingleFileArchiveTypes.Count, viewModels.Count);

            Assert.True(config.SingleFileArchiveTypes.All(kvp =>
            {
                var (key, value) = kvp;
                var viewModel = viewModels.SingleOrDefault(vm => vm.ArchiveType == key);

                return viewModel != null && viewModel.Name == value;
            }));
        }

        [Fact]
        public void TestCreateForMultipleFiles()
        {
            var config = new ArchiveTypeViewModelFactoryConfiguration
            {
                MultipleFilesArchiveTypes = new Dictionary<ArchiveType, string>
                {
                    [ArchiveType.Gz] = "Gz",
                    [ArchiveType.Tar] = "Tar",
                    [ArchiveType.Zip] = "Zip"
                }
            };
            _autoMocker.Use(config);

            var factory = _autoMocker.CreateInstance<ArchiveTypeViewModelFactory>();
            var viewModels = factory.CreateForMultipleFiles();

            Assert.NotNull(viewModels);
            Assert.Equal(config.MultipleFilesArchiveTypes.Count, viewModels.Count);

            Assert.True(config.MultipleFilesArchiveTypes.All(kvp =>
            {
                var (key, value) = kvp;
                var viewModel = viewModels.SingleOrDefault(vm => vm.ArchiveType == key);

                return viewModel != null && viewModel.Name == value;
            }));
        }
    }
}