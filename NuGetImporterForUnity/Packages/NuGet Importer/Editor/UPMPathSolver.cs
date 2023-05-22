using System.IO;
using System.Linq;
using System.Threading.Tasks;

using kumaS.NuGetImporter.Editor.DataClasses;

using UnityEngine;

namespace kumaS.NuGetImporter.Editor
{
    /// <summary>
    /// <para>Class for resolving the path of packages to be installed in UPM.</para>
    /// <para>UPM�ɃC���X�g�[������p�b�P�[�W�̃p�X���������邽�߂̃N���X�B</para>
    /// </summary>
    internal class UPMPathSolver : PackagePathSolverBase
    {
        /// <inheritdoc/>
        internal override Task<string> InstallPath(Package package)
        {
            return InstallPath(package.id, package.version);
        }

        /// <inheritdoc/>
        internal override async Task<string> InstallPath(string packageName, string version)
        {
            Catalog catalog = await NuGet.GetCatalog(packageName);
            Catalogentry selectedCatalog = catalog.GetAllCatalogEntry().First(entry => entry.version == version);
            return Path.Combine(PackageManager.DataPath.Replace("Assets", "Packages"), selectedCatalog.id);
        }
    }
}
