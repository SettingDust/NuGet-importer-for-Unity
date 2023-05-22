using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using kumaS.NuGetImporter.Editor.DataClasses;

namespace kumaS.NuGetImporter.Editor
{
    /// <summary>
    /// <para>Base class for resolving package paths.</para>
    /// <para>�p�b�P�[�W�̃p�X���������邽�߂̊��N���X�B</para>
    /// </summary>
    internal abstract class PackagePathSolverBase
    {
        /// <summary>
        /// <para>Get the path to the installation location.</para>
        /// <para>�C���X�g�[����̃p�X���擾�B</para>
        /// </summary>
        /// <param name="package">
        /// <para>Target package.</para>
        /// <para>�Ώۂ̃p�b�P�[�W�B</para>
        /// </param>
        /// <returns>
        /// <para>Path.</para>
        /// <para>�p�X�B</para>
        /// </returns>
        internal abstract Task<string> InstallPath(Package package);

        /// <summary>
        /// <para>Get the path to the installation location.</para>
        /// <para>�C���X�g�[����̃p�X���擾�B</para>
        /// </summary>
        /// <param name="packageName">
        /// <para>Target package name.</para>
        /// <para>�Ώۂ̃p�b�P�[�W���B</para>
        /// </param>
        /// <param name="version">
        /// <para>Target package version.</para>
        /// <para>�Ώۂ̃p�b�P�[�W�o�[�W�����B</para>
        /// </param>
        /// <returns>
        /// <para>Path.</para>
        /// <para>�p�X�B</para>
        /// </returns>
        internal abstract Task<string> InstallPath(string packageName, string version);

        /// <summary>
        /// <para>Get the path to the top directory of the analyzer.</para>
        /// <para>�A�i���C�U�[�̃g�b�v�f�B���N�g���̃p�X���擾�B</para>
        /// </summary>
        /// <param name="package">
        /// <para>Target package.</para>
        /// <para>�Ώۂ̃p�b�P�[�W�B</para>
        /// </param>
        /// <returns>
        /// <para>Path. However, if the analyzer does not exist, an empty string is returned.</para>
        /// <para>�p�X�B�������A�i���C�U�[�����݂��Ȃ��Ƃ��͋󕶎��񂪕Ԃ�B</para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// <para>Thrown when a package is not installed.</para>
        /// <para>�p�b�P�[�W���C���X�g�[������Ă��Ȃ��Ƃ��ɓ�������B</para>
        /// </exception>
        internal async Task<string> AnalyzerPath(Package package)
        {
            var installPath = await InstallPath(package);
            return AnalyzerPathInternal(installPath);
        }

        /// <summary>
        /// <para>Get the path to the top directory of the analyzer.</para>
        /// <para>�A�i���C�U�[�̃g�b�v�f�B���N�g���̃p�X���擾�B</para>
        /// </summary>
        /// <param name="packageName">
        /// <para>Target package name.</para>
        /// <para>�Ώۂ̃p�b�P�[�W���B</para>
        /// </param>
        /// <param name="version">
        /// <para>Target package version.</para>
        /// <para>�Ώۂ̃p�b�P�[�W�o�[�W�����B</para>
        /// </param>
        /// <returns>
        /// <para>Path. However, if the analyzer does not exist, an empty string is returned.</para>
        /// <para>�p�X�B�������A�i���C�U�[�����݂��Ȃ��Ƃ��͋󕶎��񂪕Ԃ�B</para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// <para>Thrown when a package is not installed.</para>
        /// <para>�p�b�P�[�W���C���X�g�[������Ă��Ȃ��Ƃ��ɓ�������B</para>
        /// </exception>
        internal async Task<string> AnalyzerPath(string packageName, string version)
        {
            var installPath = await InstallPath(packageName, version);
            return AnalyzerPathInternal(installPath);
        }

        private string AnalyzerPathInternal(string installPath)
        {
            if (!Directory.Exists(installPath))
            {
                throw new InvalidOperationException("This package is not installed.");
            }

            var analyzerRootPath = Path.Combine(installPath, "analyzers", "dotnet");
            var analyzerLanguagePath = Path.Combine(analyzerRootPath, "cs");
            if (Directory.Exists(analyzerLanguagePath))
            {
                return analyzerLanguagePath;
            }

            if (Directory.EnumerateFiles(analyzerRootPath).Where(path => !path.EndsWith(".meta")).Any())
            {
                return analyzerRootPath;
            }

            return "";
        }

        /// <summary>
        /// <para>Get the path to the top directory of the library.</para>
        /// <para>���C�u�����̃g�b�v�f�B���N�g���̃p�X���擾�B</para>
        /// </summary>
        /// <param name="package">
        /// <para>Target package.</para>
        /// <para>�Ώۂ̃p�b�P�[�W�B</para>
        /// </param>
        /// <returns>
        /// <para>Path. However, if the managed library does not exist, an empty string is returned.</para>
        /// <para>�p�X�B�������}�l�[�W�h���C�u���������݂��Ȃ��Ƃ��͋󕶎��񂪕Ԃ�B</para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// <para>Thrown when a package is not installed.</para>
        /// <para>�p�b�P�[�W���C���X�g�[������Ă��Ȃ��Ƃ��ɓ�������B</para>
        /// </exception>
        internal async Task<string> LibraryPath(Package package)
        {
            var installPath = await InstallPath(package);
            return LibraryPathInternal(installPath);
        }

        /// <summary>
        /// <para>Get the path to the top directory of the library.</para>
        /// <para>���C�u�����̃g�b�v�f�B���N�g���̃p�X���擾�B</para>
        /// </summary>
        /// <param name="packageName">
        /// <para>Target package name.</para>
        /// <para>�Ώۂ̃p�b�P�[�W���B</para>
        /// </param>
        /// <param name="version">
        /// <para>Target package version.</para>
        /// <para>�Ώۂ̃p�b�P�[�W�o�[�W�����B</para>
        /// </param>
        /// <returns>
        /// <para>Path. However, if the managed library does not exist, an empty string is returned.</para>
        /// <para>�p�X�B�������}�l�[�W�h���C�u���������݂��Ȃ��Ƃ��͋󕶎��񂪕Ԃ�B</para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// <para>Thrown when a package is not installed.</para>
        /// <para>�p�b�P�[�W���C���X�g�[������Ă��Ȃ��Ƃ��ɓ�������B</para>
        /// </exception>
        internal async Task<string> LibraryPath(string packageName, string version)
        {
            var installPath = await InstallPath(packageName, version);
            return LibraryPathInternal(installPath);
        }

        private string LibraryPathInternal(string installPath)
        {
            if (!Directory.Exists(installPath))
            {
                throw new InvalidOperationException("This package is not installed.");
            }

            var managedPath = Path.Combine(installPath, "lib");
            if (!Directory.Exists(managedPath))
            {
                return "";
            }
            var frameworkPath = Directory.EnumerateDirectories(managedPath);
            if (!frameworkPath.Any())
            {
                return "";
            }

            return frameworkPath.First();
        }


        /// <summary>
        /// <para>Get the path to the Unity Editor directory of the library.</para>
        /// <para>���C�u������Unity Editor�f�B���N�g���̃p�X���擾�B</para>
        /// </summary>
        /// <param name="package">
        /// <para>Target package.</para>
        /// <para>�Ώۂ̃p�b�P�[�W�B</para>
        /// </param>
        /// <returns>
        /// <para>Path. However, if the directory for Unity Editor does not exist, an empty string is returned.</para>
        /// <para>�p�X�B������Unity Editor�p�̃f�B���N�g�������݂��Ȃ��Ƃ��͋󕶎��񂪕Ԃ�B</para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// <para>Thrown when a package is not installed.</para>
        /// <para>�p�b�P�[�W���C���X�g�[������Ă��Ȃ��Ƃ��ɓ�������B</para>
        /// </exception>
        internal async Task<string> UnityEditorPath(Package package)
        {
            var installPath = await InstallPath(package);
            return UnityEditorPathInternal(installPath);
        }

        /// <summary>
        /// <para>Get the path to the Unity Editor directory of the library.</para>
        /// <para>���C�u������Unity Editor�f�B���N�g���̃p�X���擾�B</para>
        /// </summary>
        /// <param name="packageName">
        /// <para>Target package name.</para>
        /// <para>�Ώۂ̃p�b�P�[�W���B</para>
        /// </param>
        /// <param name="version">
        /// <para>Target package version.</para>
        /// <para>�Ώۂ̃p�b�P�[�W�o�[�W�����B</para>
        /// </param>
        /// <returns>
        /// <para>Path. However, if the directory for Unity Editor does not exist, an empty string is returned.</para>
        /// <para>�p�X�B������Unity Editor�p�̃f�B���N�g�������݂��Ȃ��Ƃ��͋󕶎��񂪕Ԃ�B</para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// <para>Thrown when a package is not installed.</para>
        /// <para>�p�b�P�[�W���C���X�g�[������Ă��Ȃ��Ƃ��ɓ�������B</para>
        /// </exception>
        internal async Task<string> UnityEditorPath(string packageName, string version)
        {
            var installPath = await InstallPath(packageName, version);
            return UnityEditorPathInternal(installPath);
        }

        private string UnityEditorPathInternal(string installPath)
        {
            if (!Directory.Exists(installPath))
            {
                throw new InvalidOperationException("This package is not installed.");
            }

            var editorPath = Path.Combine(installPath, "Unity", "Editor");
            if (!Directory.Exists(editorPath))
            {
                return "";
            }
            return editorPath;
        }
    }
}
