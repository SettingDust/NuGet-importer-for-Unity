using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using Unity.CodeEditor;
using System.IO;
using System.Xml.Linq;

namespace kumaS.NuGetImporter.Editor
{
    /// <summary>
    /// <para>Class for configuring Roslyn Analyzer plugins.</para>
    /// <para>Roslyn Analyzer�̐ݒ�����邽�߂̃N���X�B</para>
    /// </summary>
    public class NuGetAnalyzerImportSetting : AssetPostprocessor
    {
#pragma warning disable CS0162 // ���B�ł��Ȃ��R�[�h�����o����܂���
        public static bool HasAnalyzerSupport
        {
            get
            {
#if UNITY_2020_2_OR_NEWER
                return true;
#endif
                var codeEditorType = CodeEditor.CurrentEditor.GetType();
                if (codeEditorType.Name == "VSCodeScriptEditor")
                {
#if HAS_ROSLYN_ANALZYER_SUPPORT_VSCODE
                    return true;
#endif
                }

                if (codeEditorType.Name == "RiderScriptEditor")
                {
#if HAS_ROSLYN_ANALZYER_SUPPORT_RIDER
                    return true;
#endif
                }
                return false;
            }
        }
#pragma warning restore CS0162 // ���B�ł��Ȃ��R�[�h�����o����܂���

        private readonly static Regex rx = new Regex(@"[/\\]dotnet[/\\]cs[/\\]", RegexOptions.IgnoreCase);

        private static bool IsAnalyzer(string path) => rx.IsMatch(path);

        private void OnPreprocessAsset()
        {
            if (!assetImporter.importSettingsMissing)
            {
                return;
            }

            var pluginImporter = assetImporter as PluginImporter;
            if (pluginImporter == null)
            {
                return;
            }
            if (!IsAnalyzer(assetImporter.assetPath))
            {
                return;
            }

            pluginImporter.SetCompatibleWithAnyPlatform(false);
            pluginImporter.SetCompatibleWithEditor(false);
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AssetDatabase.StartAssetEditing();
            bool isChanged = false;
            foreach(var importedAsset in importedAssets)
            {
                if (!importedAsset.EndsWith(".dll") || !IsAnalyzer(importedAsset))
                {
                    continue;
                }
                var asset = AssetDatabase.LoadMainAssetAtPath(importedAsset);
                AssetDatabase.SetLabels(asset, new[] { "RoslynAnalyzer" });
                isChanged = true;
            }

            foreach (var deletedAsset in deletedAssets)
            {
                if (deletedAsset.EndsWith(".dll") && IsAnalyzer(deletedAsset))
                {
                    isChanged = true;
                    break;
                }
            }

            // Explicitly update project files since they are not automatically updated.
            if (isChanged)
            {
                CodeEditor.CurrentEditor.SyncAll();
            }

            AssetDatabase.StopAssetEditing();
        }

        private static string OnGeneratedCSProject(string path, string content)
        {
            var packageDir = NuGetImporterSettings.Instance.InstallMethod == DataClasses.InstallMethod.AsAssets ? Path.Combine(Application.dataPath, "Packages") : Application.dataPath.Replace("Assets", "Packages");
            var analyzersPath = Directory.EnumerateFiles(packageDir, "*.dll", SearchOption.AllDirectories).Where(path => IsAnalyzer(path)).ToArray();
            var xDoc = XDocument.Parse(content);
            var nsMsbuild = (XNamespace)"http://schemas.microsoft.com/developer/msbuild/2003";
            var project = xDoc.Element(nsMsbuild + "Project");

            var baseDir = Path.GetDirectoryName(path);
            var analyzersInCsproj = new HashSet<string>(project.Descendants(nsMsbuild + "Analyzer").Select(x => x.Attribute("Include")?.Value).Where(x => x != null));
            project.Add(new XElement(nsMsbuild + "ItemGroup", analyzersPath.Where(x => !analyzersInCsproj.Contains(x)).Select(x => new XElement(nsMsbuild + "Analyzer", new XAttribute("Include", x)))));
            content = xDoc.ToString();
            return content;
        }

    }
}
