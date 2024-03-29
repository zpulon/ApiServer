﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginCore.Plugin
{
    public class DirectoryLoader
    {

        public virtual List<Assembly> LoadFromDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return new List<Assembly>();
            }

            List<Assembly> assemblyList = new List<Assembly>();
            string[] dllFiles = Directory.GetFiles(dir, "*.dll");
            foreach (string file in dllFiles)
            {
                if (!file.EndsWith(".dll"))
                {
                    continue;
                }

                try
                {

                    Assembly assembly = Assembly.LoadFrom(file);
                    assemblyList.Add(assembly);
                }
                catch (System.Exception e)
                {
                    //Logger.Error("can not load service:{0}\r\n{1}", file, e.ToString());
                }
            }

            return assemblyList;
        }


        public virtual List<Type> GetTypes(Assembly assembly, Type baseType, Func<Type, bool> checker)
        {
            Func<Type, bool> actualCheck = (ti) =>
            {
                bool isOk = true;
                if (baseType != null)
                {
                    if (baseType.GetTypeInfo().IsAssignableFrom(ti))
                    {
                        isOk = true;
                    }
                    else
                    {
                        isOk = false;
                    }
                }

                if (isOk)
                {
                    if (checker != null)
                    {
                        isOk = checker(ti);
                    }
                }

                return isOk;
            };

            return assembly.GetTypes().Where(actualCheck).ToList();
        }
    }
}
