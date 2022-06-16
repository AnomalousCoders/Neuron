using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neuron.Core.Standalone
{
    public static class Program
    {
        /// <summary>
        /// This is an example on how to load a Neuron based application
        /// The loading order for the assemblies looks as follows:
        /// <code>
        /// 1. The assembly containing this bootstrapping code (the easy part)
        /// 2. The neuron managed assemblies, usually inside Neuron/Managed (you have to do this!)
        /// 3. Your custom platform implementation (optional)
        /// 4. Module dependencies (handled by Neuron itself)
        /// </code><br/><br/>
        ///
        /// Make sure you have the right directory, in this example, we are using the working directory,
        /// followed by Neuron, but of course you can use any directory you like. Including user
        /// directories like for example AppData//Roaming or ~/Neuron/<br/><br/>
        ///
        /// In most environments, you have to manually hook the AssemblyResolve event to a list of
        /// all your loaded dependencies, since the domain into which the assemblies are loaded
        /// mostly don't allow the respecting assemblies to resolve their dependencies, if they
        /// were loaded in the same way.<br/><br/>
        ///
        /// After all assemblies have been loaded, you can just get your entrypoint via common
        /// c# reflections and invoke your main function to start neuron.<br/><br/>
        ///
        /// To add more platform specific functionality to your neuron setup, consider writing custom
        /// modules which reference assemblies present in your environment and let them be referenced
        /// by your plugins or other modules.
        /// </summary>
        public static void Main()
        {
            Console.WriteLine("Bootstrapping via reflections");
            var assemblies = new List<Assembly>();
            var domain = AppDomain.CurrentDomain;
            foreach (var file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Neuron", "Managed"), "*.dll"))
            {
                Console.WriteLine($"Loaded Dependency {file}");
                var assembly = domain.Load(File.ReadAllBytes(file));
                assemblies.Add(assembly);
            }

            domain.AssemblyResolve += delegate(object sender, ResolveEventArgs eventArgs)
            {
                Console.WriteLine($"Requested {eventArgs.Name}");
                return assemblies.First(x => x.FullName == eventArgs.Name);
            };

            var coreAssembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == "Neuron.Core");
            var entrypoint = coreAssembly.GetType("Neuron.Core.Platform.StandaloneEntrypoint");
            if (entrypoint == null) throw new Exception("Neuron.Core.Platform.StandaloneEntrypoint not found");
            var main = entrypoint.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
            if (main == null) throw new Exception("StandaloneEntrypoint.Main() is null");
            main.Invoke(null, Array.Empty<object>());
        }
    }
}