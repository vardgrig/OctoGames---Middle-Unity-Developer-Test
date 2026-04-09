using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.JsonSerializers;

namespace Utilities
{
    /// <summary>
    /// Generic save/load utility. 
    /// </summary>
    public class SaveLoadUtility
    {
        public static readonly SaveLoadUtility Default = new SaveLoadUtility();
        public string SaveDirectoryName { get; set; } = "saves";
        public bool UseEncryption { get; set; } = true;

        private string _encryptionKey = DefaultEncryptionKey;
        private IJsonSerializer _serializer = new NewtonsoftJsonSerializer();

        private const string DefaultEncryptionKey = "enc-key";
        private const string BackupExtension = ".backup";
        private const string TempExtension = ".tmp";

        private string SaveDirectory =>
            Path.Combine(Application.persistentDataPath, SaveDirectoryName);

        public void Configure(string encryptionKey, bool useEncryption = true)
        {
            if (useEncryption && string.IsNullOrEmpty(encryptionKey))
            {
                throw new ArgumentException("Encryption key cannot be null or empty.", nameof(encryptionKey));
            }

            _encryptionKey = encryptionKey;
            UseEncryption = useEncryption;
        }

        public void SetSerializer(IJsonSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public bool Save<T>(T data, string fileName, bool createBackup = true)
            where T : class
        {
            if (data == null)
            {
                Debug.LogError("Save failed: Data is null");
                return false;
            }

            if (!IsValidFileName(fileName))
            {
                Debug.LogError($"Save failed: Invalid filename '{fileName}'");
                return false;
            }

            var filePath = GetFullPath(fileName);
            var tempPath = filePath + TempExtension;

            try
            {
                Directory.CreateDirectory(SaveDirectory);

                if (createBackup && File.Exists(filePath))
                {
                    CreateBackup(fileName);
                }

                var json = _serializer.Serialize(data);

                if (UseEncryption)
                {
                    json = Encrypt(json);
                }

                File.WriteAllText(tempPath, json, Encoding.UTF8);

                // Platform-safe atomic swap: File.Replace can fail on some Android devices
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.Move(tempPath, filePath);

                Debug.Log($"Successfully saved data to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Save failed for {fileName}: {ex.Message}");
                TryDeleteFile(tempPath);
                return false;
            }
        }

        public T Load<T>(string fileName, bool fallbackToBackup = true)
            where T : class
        {
            if (!IsValidFileName(fileName))
            {
                Debug.LogError($"Load failed: Invalid filename '{fileName}'");
                return null;
            }

            var filePath = GetFullPath(fileName);

            if (File.Exists(filePath))
            {
                var result = LoadFromFile<T>(filePath);
                if (result != null)
                    return result;

                Debug.LogWarning($"Main save file corrupted: {fileName}");
            }

            if (fallbackToBackup)
            {
                var backupPath = GetBackupPath(fileName);
                if (File.Exists(backupPath))
                {
                    Debug.Log($"Attempting to load from backup: {fileName}");
                    var backupResult = LoadFromFile<T>(backupPath);
                    if (backupResult != null)
                    {
                        try
                        {
                            File.Copy(backupPath, filePath, true);
                            Debug.Log($"Restored save from backup: {fileName}");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Could not restore backup for {fileName}: {ex.Message}");
                        }

                        return backupResult;
                    }
                }
            }

            Debug.LogWarning($"No valid save data found for: {fileName}");
            return null;
        }

        /// <summary>
        /// Loads save data, returning <paramref name="defaultValue"/>
        /// </summary>
        public T LoadOrDefault<T>(string fileName, T defaultValue = null, bool fallbackToBackup = true)
            where T : class, new()
        {
            return Load<T>(fileName, fallbackToBackup) ?? defaultValue ?? new T();
        }

        /// <summary>
        /// Offloads file I/O off the main thread.
        /// </summary>
        public Task<bool> SaveAsync<T>(T data, string fileName, bool createBackup = true)
            where T : class
        {
            return Task.Run(() => Save(data, fileName, createBackup));
        }

        public Task<T> LoadAsync<T>(string fileName, bool fallbackToBackup = true)
            where T : class
        {
            return Task.Run(() => Load<T>(fileName, fallbackToBackup));
        }

        public Task<T> LoadOrDefaultAsync<T>(string fileName, T defaultValue = null, bool fallbackToBackup = true)
            where T : class, new()
        {
            return Task.Run(() => LoadOrDefault(fileName, defaultValue, fallbackToBackup));
        }

        public bool SaveExists(string fileName)
        {
            return IsValidFileName(fileName) &&
                   File.Exists(GetFullPath(fileName));
        }

        public bool DeleteSave(string fileName)
        {
            if (!IsValidFileName(fileName))
            {
                Debug.LogError($"DeleteSave failed: Invalid filename '{fileName}'");
                return false;
            }

            try
            {
                TryDeleteFile(GetFullPath(fileName));
                TryDeleteFile(GetBackupPath(fileName));
                Debug.Log($"Deleted save: {fileName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete {fileName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Returns the file names (without extension) of all main save files.
        /// </summary>
        public string[] GetAllSaves(SaveSortOrder sortOrder = SaveSortOrder.NewestFirst)
        {
            if (!Directory.Exists(SaveDirectory))
                return Array.Empty<string>();

            var files = Directory.GetFiles(SaveDirectory, "*.json");
            var saves = new List<(string name, DateTime date)>(files.Length);

            foreach (var file in files)
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(file);
                if (!nameWithoutExt.EndsWith(BackupExtension, StringComparison.OrdinalIgnoreCase))
                {
                    saves.Add((nameWithoutExt, File.GetLastWriteTimeUtc(file)));
                }
            }

            switch
                (sortOrder) // We can use Strategy pattern here if we add more sort options in the future, but a simple switch is sufficient for sortOrder with only 3 options
            {
                case SaveSortOrder.NewestFirst:
                    saves.Sort((a, b) => b.date.CompareTo(a.date));
                    break;
                case SaveSortOrder.OldestFirst:
                    saves.Sort((a, b) => a.date.CompareTo(b.date));
                    break;
                case SaveSortOrder.Alphabetical:
                default:
                    saves.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.name, b.name));
                    break;
            }

            var result = new string[saves.Count];
            for (var i = 0; i < saves.Count; i++)
            {
                result[i] = saves[i].name;
            }

            return result;
        }

        public void ClearAllSaves()
        {
            try
            {
                if (!Directory.Exists(SaveDirectory))
                {
                    Debug.Log("No save directory found to clear");
                    return;
                }

                Directory.Delete(SaveDirectory, true);
                Directory.CreateDirectory(SaveDirectory);
                Debug.Log("Cleared all save data");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to clear saves: {ex.Message}");
            }
        }

        private string GetFullPath(string fileName) =>
            Path.Combine(SaveDirectory, fileName + ".json");

        private string GetBackupPath(string fileName) =>
            Path.Combine(SaveDirectory, fileName + BackupExtension + ".json");

        private static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return false;
            }

            if (fileName.Contains("..") ||
                fileName.Contains('/') ||
                fileName.Contains('\\'))
            {
                return false;
            }

            return true;
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Could not delete file '{path}': {ex.Message}");
            }
        }

        private void CreateBackup(string fileName)
        {
            var sourcePath = GetFullPath(fileName);
            var backupPath = GetBackupPath(fileName);

            try
            {
                File.Copy(sourcePath, backupPath, true);
                Debug.Log($"Created backup: {fileName}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to create backup for {fileName}: {ex.Message}");
            }
        }

        private T LoadFromFile<T>(string filePath) where T : class
        {
            try
            {
                var json = File.ReadAllText(filePath, Encoding.UTF8);

                if (UseEncryption)
                {
                    json = Decrypt(json);
                }

                var data = _serializer.Deserialize<T>(json);

                if (data == null)
                {
                    throw new InvalidOperationException("Deserialized data is null");
                }

                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load from {filePath}: {ex.Message}");
                return null;
            }
        }

        private string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            var textBytes = Encoding.UTF8.GetBytes(plainText);
            var keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);

            for (var i = 0; i < textBytes.Length; i++)
            {
                textBytes[i] ^= keyBytes[i % keyBytes.Length];
            }

            return Convert.ToBase64String(textBytes);
        }

        private string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            var cipherBytes = Convert.FromBase64String(cipherText);
            var keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);

            for (var i = 0; i < cipherBytes.Length; i++)
            {
                cipherBytes[i] ^= keyBytes[i % keyBytes.Length];
            }

            return Encoding.UTF8.GetString(cipherBytes);
        }
    }
}