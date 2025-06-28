namespace SnippetManager.Services
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using Interfaces;
    using Newtonsoft.Json;

    public class SnippetFileService : ISnippetFileService
    {
        private readonly JsonSerializerSettings _jsonSettings;

        public SnippetFileService()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                Formatting = Formatting.Indented
            };
        }

        public string SerializeSnippets(ObservableCollection<ISnippetListItemReadOnly> snippets)
        {
            try
            {
                return JsonConvert.SerializeObject(snippets, _jsonSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error serializing snippets: {ex.Message}", "Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public ObservableCollection<ISnippetListItemReadOnly> DeserializeSnippets(string jsonContent)
        {
            try
            {
                return JsonConvert.DeserializeObject<ObservableCollection<ISnippetListItemReadOnly>>(jsonContent, _jsonSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deserializing snippets: {ex.Message}", "Deserialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public bool SaveSnippetsToFile(ObservableCollection<ISnippetListItemReadOnly> snippets, string defaultFileName = "New Snippet")
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = defaultFileName,
                    DefaultExt = ".json",
                    Filter = "JSON documents (.json)|*.json"
                };

                var result = saveDialog.ShowDialog();
                if (result == true)
                {
                    return SaveSnippetsToPath(snippets, saveDialog.FileName);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing save dialog: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public ObservableCollection<ISnippetListItemReadOnly> LoadSnippetsFromFile(string defaultFileName = "New Snippet")
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog
                {
                    FileName = defaultFileName,
                    DefaultExt = ".json",
                    Filter = "JSON documents (.json)|*.json"
                };

                var result = openDialog.ShowDialog();
                if (result == true)
                {
                    return LoadSnippetsFromPath(openDialog.FileName);
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing open dialog: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public bool SaveSnippetsToPath(ObservableCollection<ISnippetListItemReadOnly> snippets, string filePath)
        {
            try
            {
                var jsonContent = SerializeSnippets(snippets);
                if (jsonContent == null)
                {
                    return false;
                }

                File.WriteAllText(filePath, jsonContent);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public ObservableCollection<ISnippetListItemReadOnly> LoadSnippetsFromPath(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File not found: {filePath}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                var jsonContent = File.ReadAllText(filePath);
                return DeserializeSnippets(jsonContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
} 