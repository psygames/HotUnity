using System.IO;
using UnityEngine;

namespace HotUnity.Editor
{
    public class HotAssemblyLoader
    {
        private string path => Path.Combine(Application.streamingAssetsPath, "HotProject.dll");
        public ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        MemoryStream fs;

        public void Load()
        {
            appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            var bytes = File.ReadAllBytes(path);
            fs = new MemoryStream(bytes);
            try
            {
                appdomain.LoadAssembly(fs, null, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            catch
            {
                Debug.LogError("加载热更DLL失败");
            }
        }

        public void Unload()
        {
            fs.Close();
            appdomain = null;
        }

        public void Reloead()
        {
            if (appdomain != null)
            {
                Unload();
            }
            Load();
        }
    }
}
