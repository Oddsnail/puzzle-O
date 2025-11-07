using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace origin.IO {
    public class FileManager {
        public static readonly string root = $"{Application.dataPath}/_MAIN/Resources/";

        public static List<string> ReadTextFile(string filePath, bool includeBlackLines = false) {
            if (!filePath.StartsWith('/')) filePath = root + filePath;

            List<string> lines = new();
            try {
                using StreamReader sr = new(filePath);
                while (!sr.EndOfStream) {
                    string line = sr.ReadLine();
                    if (includeBlackLines || !string.IsNullOrWhiteSpace(line)) lines.Add(line);
                }
            }
            catch (FileNotFoundException ex) {
                Debug.LogError($"File not found: '{ex.FileName}'");
            }

            return lines;
        }

        public static List<string> ReadTextAsset(string filePath, bool includeBlackLines = false) {
            TextAsset asset = Resources.Load<TextAsset>(filePath);

            if (asset == null) {
                Debug.LogError($"Asset not found: '{filePath}'");
                return null;
            }

            return ReadTextAsset(asset, includeBlackLines);
        }

        public static List<string> ReadTextAsset(TextAsset asset, bool includeBlackLines = false) {
            List<string> lines = new();

            using (StringReader sr = new(asset.text)) {
                while (sr.Peek() > -1) {
                    string line = sr.ReadLine();
                    if (includeBlackLines || !string.IsNullOrWhiteSpace(line)) lines.Add(line);
                }
            }

            return lines;
        }
    }
}