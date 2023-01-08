using Grpc.Core;
using BarkditorServer.Domain.Constants;
using System.Text.Json;

namespace BarkditorServer.BusinessLogic.Services;

public class ProjectFilesService : ProjectFiles.ProjectFilesBase
{
    public override Task<OpenFolderResponse> OpenFolder(OpenFolderRequest request, ServerCallContext ctx)
    {
        var rootProjectDirectoryInfo = new DirectoryInfo(request.Path);
        var fileTree = new FileTree();

        GetFileTree(fileTree, rootProjectDirectoryInfo);

        SaveProject(fileTree);

        var response = new OpenFolderResponse
        {
            ProjectFiles = fileTree
        };

        return Task.FromResult(response);
    }

    public override Task<GetSavedProjectResponse> GetSavedProject(Google.Protobuf.WellKnownTypes.Empty empty, ServerCallContext ctx)
    {
        var jsonProjectFileTreeString = File.ReadAllText(FilePaths.ProjectFilesTreeJsonPath);
        var projectFileTree = JsonSerializer.Deserialize<FileTree>(jsonProjectFileTreeString);

        var response = new GetSavedProjectResponse
        {
            ProjectFiles = projectFileTree
        };

        return Task.FromResult(response);
    }

    private void GetFileTree(FileTree fileTree, DirectoryInfo directoryInfo) 
    {
        foreach(var projectFolder in directoryInfo.GetDirectories()) 
        {
            if(DirectoriesToIgnore.IgnoreArray.Contains(projectFolder.Name))
            {
                continue;
            }

            var projectFolderTree = new FileTree
            {
                Name = projectFolder.Name,
                IsDirectory = true
            };

            fileTree.Files.Add(projectFolderTree);

            foreach(var projectFile in projectFolder.GetFiles())
            {
                var projectFileTree = new FileTree
                {
                    Name = projectFile.Name,
                    IsDirectory = false
                };
                projectFolderTree.Files.Add(projectFileTree);
            }

            GetFileTree(projectFolderTree, projectFolder);
        }
    }

    private void SaveProject(FileTree projectFileTree)
    {
        var jsonProjectFileTreeString = JsonSerializer.Serialize(projectFileTree);
        File.WriteAllText(FilePaths.ProjectFilesTreeJsonPath, jsonProjectFileTreeString);
    }
}
