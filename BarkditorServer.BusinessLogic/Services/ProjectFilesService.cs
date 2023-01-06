using Grpc.Core;
using BarkditorServer.BusinessLogic;

namespace BarkditorServer.BusinessLogic.Services;

public class ProjectFilesService : ProjectFiles.ProjectFilesBase
{

    public override Task<GetProjectFilesResponse> GetProjectFiles(GetProjectFilesRequest request, ServerCallContext context)
    {
        var rootProjectDirectoryInfo = new DirectoryInfo(request.Path);
        var fileTree = new GetProjectFilesResponse.Types.FileTree();

        foreach(var projectFile in rootProjectDirectoryInfo.GetFiles())
        {
            var projectFileTree = new GetProjectFilesResponse.Types.FileTree
            {
                Name = projectFile.Name,
                IsDirectory = false
            };
            fileTree.Files.Add(projectFileTree);
        }

        GetFileTree(fileTree, rootProjectDirectoryInfo);

        var response = new GetProjectFilesResponse
        {
            ProjectFiles = fileTree
        };

        return Task.FromResult(response);
    }

    private void GetFileTree(GetProjectFilesResponse.Types.FileTree fileTree, DirectoryInfo directoryInfo) 
    {
        foreach(var projectFolder in directoryInfo.GetDirectories()) 
        {
            var projectFolderTree = new GetProjectFilesResponse.Types.FileTree
            {
                Name = projectFolder.Name,
                IsDirectory = true
            };

            fileTree.Files.Add(projectFolderTree);

            foreach(var projectFile in projectFolder.GetFiles())
            {
                var projectFileTree = new GetProjectFilesResponse.Types.FileTree
                {
                    Name = projectFile.Name,
                    IsDirectory = false
                };
                projectFolderTree.Files.Add(projectFileTree);
            }

            GetFileTree(projectFolderTree, projectFolder);
        }
    } 
}
