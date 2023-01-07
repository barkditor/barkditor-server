using Grpc.Core;
using BarkditorServer.Domain.Constants;

namespace BarkditorServer.BusinessLogic.Services;

public class ProjectFilesService : ProjectFiles.ProjectFilesBase
{
    public override Task<GetProjectFilesResponse> GetProjectFiles(GetProjectFilesRequest request, ServerCallContext ctx)
    {
        var rootProjectDirectoryInfo = new DirectoryInfo(request.Path);
        var fileTree = new GetProjectFilesResponse.Types.FileTree();

        GetFileTree(fileTree, rootProjectDirectoryInfo);
        
        foreach(var projectFile in rootProjectDirectoryInfo.GetFiles())
        {
            var projectFileTree = new GetProjectFilesResponse.Types.FileTree
            {
                Name = projectFile.Name,
                IsDirectory = false
            };
            fileTree.Files.Add(projectFileTree);
        }

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
            if(DirectoriesToIgnore.IgnoreArray.Contains(projectFolder.Name))
            {
                continue;
            }

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
