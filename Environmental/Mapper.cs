using System.Reflection;

namespace Environmental
{
    public static class EnvMapper
    {
        public static T Map<T>()
        {
            return MapAll<T>(null);
        }

        public static T Map<T>(string filePath)
        {
            return MapAll<T>(filePath);
        }

        private static T MapAll<T>(string? filePath)
        {
            Dictionary<string, string> envvars = new();

            if (filePath != null)
            {
                envvars = ParseEnvFile(filePath);
            }

            T obj = (T)Activator.CreateInstance(typeof(T))!;

            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop == null)
                {
                    continue;
                }

                var attr = prop.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(EnvAttribute));

                if (attr == null)
                {
                    continue;
                }

                var type = obj.GetType();

                PropertyInfo? pInfo = type.GetProperty(prop.Name);

                if (pInfo == null)
                {
                    continue;
                }

                if (pInfo.GetCustomAttributes(typeof(EnvAttribute), false) is not EnvAttribute[] attrs)
                {
                    continue;
                }

                if (attrs.Length > 0)
                {
                    var val = attrs[0];
                    var required = val.Required;
                    var name = val.Name;

                    bool exists = false;
                    string? envVal = null;

                    if (filePath != null)
                    {
                        exists = envvars.TryGetValue(name, out envVal);
                    }
                    else
                    {
                        envVal = Environment.GetEnvironmentVariable(name);
                        exists = envVal != null;
                    }

                    if (required == true && !exists)
                    {
                        throw new Exception($"environment key \"{name}\" is required but not found");
                    }

                    if (envVal == null)
                    {
                        continue;
                    }

                    pInfo.SetValue(obj, ParseString(envVal, pInfo.PropertyType));
                }

            };

            return obj;
        }

        private static object ParseString(string input, Type type)
        {
            try
            {
                return type.Name switch
                {
                    "Int32" => int.Parse(input),
                    "Boolean" => bool.Parse(input),
                    "String" => input,
                    "Single" => float.Parse(input),
                    "Double" => double.Parse(input),
                    "DateTime" => DateTime.Parse(input),
                    "TimeSpan" => TimeSpan.Parse(input),
                    _ => throw new Exception($"cannot parse of type {type.Name}")
                };
            }
            catch
            {
                throw;
            }
        }

        private static Dictionary<string, string> ParseEnvFile(string filePath)
        {
            Dictionary<string, string> env = new Dictionary<string, string>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            string[] parts = line.Split("=", 2);
                            string key = parts[0];
                            string value = parts[1];
                            env[key] = value;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The env file could not be read:");
                Console.WriteLine(e.Message);
            }

            return env;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class EnvironmentalAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class EnvAttribute : Attribute
    {
        public string Name { get; set; } = null!;
        public bool Required { get; set; }
    }
}
