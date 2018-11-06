using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tatoeba.ResourceGenerator
{
    public class CustomFolder : PCLStorage.IFolder
    {
        string DefaultPath => System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;

        public string Name => throw new NotImplementedException();
        public string Path => throw new NotImplementedException();
        public Task<PCLStorage.ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            var result = File.Exists(System.IO.Path.Combine(DefaultPath, name)) ? PCLStorage.ExistenceCheckResult.FileExists : PCLStorage.ExistenceCheckResult.NotFound;
            return Task.FromResult(result);
        }

        public Task<PCLStorage.IFile> CreateFileAsync(string desiredName, PCLStorage.CreationCollisionOption option, CancellationToken cancellationToken = default)
        {
            var file = new CustomFile
            {
                Name = desiredName,
                Path = System.IO.Path.Combine(DefaultPath, desiredName),
                Stream = File.Create(System.IO.Path.Combine(DefaultPath, desiredName)),
            };

            return Task.FromResult((PCLStorage.IFile)file);
        }
        public Task<PCLStorage.IFolder> CreateFolderAsync(string desiredName, PCLStorage.CreationCollisionOption option, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task DeleteAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PCLStorage.IFile> GetFileAsync(string name, CancellationToken cancellationToken = default)
        {
            var file = new CustomFile
            {
                Name = name,
                Path = System.IO.Path.Combine(DefaultPath, name),
                Stream = File.OpenRead(System.IO.Path.Combine(DefaultPath, name)),
            };

            return Task.FromResult((PCLStorage.IFile)file);
        }
        public Task<IList<PCLStorage.IFile>> GetFilesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PCLStorage.IFolder> GetFolderAsync(string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IList<PCLStorage.IFolder>> GetFoldersAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

    public class CustomFile : PCLStorage.IFile
    {
        public FileStream Stream { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            File.Delete(Path);
            return Task.FromResult(0);
        }
        public Task MoveAsync(string newPath, PCLStorage.NameCollisionOption collisionOption = PCLStorage.NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<Stream> OpenAsync(PCLStorage.FileAccess fileAccess, CancellationToken cancellationToken = default)
        {
            return Task.FromResult((Stream)Stream);
        }
        public Task RenameAsync(string newName, PCLStorage.NameCollisionOption collisionOption = PCLStorage.NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }



    public class CustomSettings : Plugin.Settings.Abstractions.ISettings
    {
        private string osef;

        public bool AddOrUpdateValue(string key, decimal value, string fileName = null) => throw new NotImplementedException();
        public bool AddOrUpdateValue(string key, bool value, string fileName = null) => throw new NotImplementedException();
        public bool AddOrUpdateValue(string key, long value, string fileName = null) => throw new NotImplementedException();
        public bool AddOrUpdateValue(string key, string value, string fileName = null)
        {
            osef = value;
            return true;
        }
        public bool AddOrUpdateValue(string key, int value, string fileName = null) => throw new NotImplementedException();
        public bool AddOrUpdateValue(string key, float value, string fileName = null) => throw new NotImplementedException();
        public bool AddOrUpdateValue(string key, DateTime value, string fileName = null) => throw new NotImplementedException();
        public bool AddOrUpdateValue(string key, Guid value, string fileName = null) => throw new NotImplementedException();
        public bool AddOrUpdateValue(string key, double value, string fileName = null) => throw new NotImplementedException();
        public void Clear(string fileName = null) => throw new NotImplementedException();
        public bool Contains(string key, string fileName = null) => throw new NotImplementedException();
        public decimal GetValueOrDefault(string key, decimal defaultValue, string fileName = null) => throw new NotImplementedException();
        public bool GetValueOrDefault(string key, bool defaultValue, string fileName = null) => throw new NotImplementedException();
        public long GetValueOrDefault(string key, long defaultValue, string fileName = null) => throw new NotImplementedException();
        public string GetValueOrDefault(string key, string defaultValue, string fileName = null) => osef;
        public int GetValueOrDefault(string key, int defaultValue, string fileName = null) => throw new NotImplementedException();
        public float GetValueOrDefault(string key, float defaultValue, string fileName = null) => throw new NotImplementedException();
        public DateTime GetValueOrDefault(string key, DateTime defaultValue, string fileName = null) => throw new NotImplementedException();
        public Guid GetValueOrDefault(string key, Guid defaultValue, string fileName = null) => throw new NotImplementedException();
        public double GetValueOrDefault(string key, double defaultValue, string fileName = null) => throw new NotImplementedException();
        public bool OpenAppSettings() => throw new NotImplementedException();
        public void Remove(string key, string fileName = null) => throw new NotImplementedException();
    }

}
