using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fixturizer {
        public class Loader
        {
                private readonly string _basePath;

                public Loader(string basePath)
                {
                        _basePath = basePath;
                }

                public dynamic Load(string fixtureName)
                {
                        var filename = fixtureName + ".json";
                        var path = Path.Combine(_basePath, filename);

                        dynamic fixture;
                        
                        using (var reader = new StreamReader(path))
                        {
                                var buffer = reader.ReadToEnd();
                                fixture = JsonConvert.DeserializeObject(buffer);
                        }

                        return fixture;
                }
        }
}
