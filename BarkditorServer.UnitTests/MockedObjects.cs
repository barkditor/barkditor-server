using System.Threading.Tasks;
using BarkditorServer.BusinessLogic;
using BarkditorServer.BusinessLogic.Services;
using BarkditorServer.UnitTests.Helpers;
using Moq;

namespace BarkditorServer.UnitTests;

public static class MockedObjects
{
    public static ProjectFilesService GetProjectFilesService()
    {
        var serviceMock = new Mock<ProjectFilesService>();

        serviceMock.Setup(x => 
            x.OpenFolder(It.IsAny<OpenFolderRequest>(),
                         It.IsAny<ServerCallContextHelper>()))
            .Returns(Task.FromResult(It.IsAny<FileTreeResponse>()));
        serviceMock.Setup(x =>
            x.GetSavedProject(It.IsAny<Google.Protobuf.WellKnownTypes.Empty>(),
                                It.IsAny<ServerCallContextHelper>()))
            .Returns(Task.FromResult(It.IsAny<FileTreeResponse>()));

        return serviceMock.Object;
    }
}