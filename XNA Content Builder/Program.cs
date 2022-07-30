using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Builder;

namespace XNA_Content_Builder
{
    class Program
    {
        protected static ContentBuilder Builder;
        protected static ContentManager ContentExternal;

        static void Main(string[] args)
        {
            try
            {
                ServiceContainer Services = new ServiceContainer();

                Builder = new ContentBuilder();
                ContentExternal = new ContentManager(Services, "../Content/");

                string[] ArrayFoldersToConvert = File.ReadAllLines("XNA Folders to convert.txt");
                string[] Files;

                #region Ressources building

                foreach (string ActiveFolder in ArrayFoldersToConvert)
                {
                    if (Directory.Exists("../" + ActiveFolder))
                    {
                        Files = Directory.GetFiles("../" + ActiveFolder, "*.png", SearchOption.AllDirectories);
                        // Tell the ContentBuilder what to build.
                        for (int F = 0; F < Files.Count(); F++)
                        {
                            if (!File.Exists(Path.ChangeExtension(Files[F], ".xnb")))
                                Builder.Add(Path.GetFullPath(Files[F]), Files[F].Substring(0, Files[F].Length - 4).Remove(0, 11), "TextureImporter", "TextureProcessor");
                        }
                    }
                }

                // Build this new model data.
                string buildError = Builder.Build();
                if (!string.IsNullOrEmpty(buildError))
                {
                    throw new Exception(buildError);
                }

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
    public class ServiceContainer : IServiceProvider
    {
        Dictionary<Type, object> services = new Dictionary<Type, object>();


        /// <summary>
        /// Adds a new service to the collection.
        /// </summary>
        public void AddService<T>(T service)
        {
            services.Add(typeof(T), service);
        }


        /// <summary>
        /// Looks up the specified service.
        /// </summary>
        public object GetService(Type serviceType)
        {
            object service;

            services.TryGetValue(serviceType, out service);

            return service;
        }
    }
}
