using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CrypTool
{
  public class PluginLoader<T>
  {
    public List<T> Plugins; 

    public PluginLoader(Action<string,T> initAction)
    {
      LoadAssemblies(initAction, AssemblyDirectory);
    }
    public PluginLoader(Action<string, T> initAction,string path)
    {
      LoadAssemblies(initAction, path);
    }

      struct LoadedType
      {
          public Type t;
          public string Assembly;
      }
    protected void LoadAssemblies(Action<string, T> initAction,string path)
    {
      string[] dllFileNames = null;

      dllFileNames = Directory.GetFiles(path, "*.dll");
      ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
      foreach (string dllFile in dllFileNames)
      {
        
        AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
        Assembly assembly = Assembly.Load(an);
        assemblies.Add(assembly);
      }

      Type pluginType = typeof(T);
      ICollection<LoadedType> pluginTypes = new List<LoadedType>();
      foreach (Assembly assembly in assemblies)
      {
        if (assembly == null) continue;
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
          if (type.IsInterface || type.IsAbstract)
            continue;
          
          if (type.GetInterface(pluginType.FullName) != null )
          {
            pluginTypes.Add(new LoadedType() {t=type,Assembly = assembly.GetName().Name});
          }
        }
      }

      Plugins = new List<T>(pluginTypes.Count);
      foreach (LoadedType type in pluginTypes)
      {
        T plugin = (T)Activator.CreateInstance(type.t);
        Plugins.Add(plugin);
        initAction?.Invoke(type.Assembly,plugin);
      }
    }

    public static string AssemblyDirectory
    {
      get
      {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);
        return Path.GetDirectoryName(path);
      }
    }
  }
}
