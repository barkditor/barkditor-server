using Grpc.Core;
using BarkditorServer.Domain.Constants;

namespace BarkditorServer.BusinessLogic.Services;

public class ProjectFilesService : ProjectFiles.ProjectFilesBase
{
    public override Task<FileTreeResponse> OpenFolder(OpenFolderRequest request, ServerCallContext ctx)
    {
        var rootProjectDirectoryInfo = new DirectoryInfo(request.Path);
        var fileTree = new FileTree();

        GetFileTree(fileTree, rootProjectDirectoryInfo);
        
        foreach(var projectFile in rootProjectDirectoryInfo.GetFiles())
        {
            var projectFileTree = new FileTree
            {
                Name = projectFile.Name,
                IsDirectory = false
            };
            fileTree.Files.Add(projectFileTree);
        }

        SaveProject(fileTree);

        var response = new FileTreeResponse
        {
            Files = fileTree
        };

        return Task.FromResult(response);
    }

    public override Task<FileTreeResponse> GetSavedProject(Google.Protobuf.WellKnownTypes.Empty empty, ServerCallContext ctx)
    {
        string jsonProjectFileTreeString = "";

        try
        {
            jsonProjectFileTreeString = File.ReadAllText(FilePaths.ProjectFilesTreeJsonPath);
        }
        catch(Exception)
        {
            var emptyResponse = new FileTreeResponse
            {
                Files = null
            };
            return Task.FromResult(emptyResponse);
        }

        var jsonParser = Google.Protobuf.JsonParser.Default;
        var projectFileTree = jsonParser.Parse<FileTree>(jsonProjectFileTreeString);

        var response = new FileTreeResponse
        {
            Files = projectFileTree
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
        var jsonFormatter = Google.Protobuf.JsonFormatter.Default; 
        var jsonProjectFileTreeString = jsonFormatter.Format(projectFileTree);
        
        File.WriteAllText(FilePaths.ProjectFilesTreeJsonPath, jsonProjectFileTreeString);
    }
}
