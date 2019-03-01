using System;
using System.Collections.Generic;
using System.IO;
namespace SkylightEmulator.Core
{
	public class ConfigurationData
	{
		private Dictionary<string, string> data;

		public ConfigurationData(string filePath)
		{
			this.data = new Dictionary<string, string>();
			if (!File.Exists(filePath))
			{
                Logging.WriteLine("Unable to locate configuration file at '" + filePath + "'!", ConsoleColor.Red);
                Logging.WriteBlank();
                Logging.WriteLine("Press any key to shut down ...", ConsoleColor.Blue);
                Console.ReadKey(true);
                Skylight.Destroy();
			}
			try
			{
				using (StreamReader streamReader = new StreamReader(filePath))
				{
					string str;
					while ((str = streamReader.ReadLine()) != null)
					{
						if (str.Length >= 1 && !str.StartsWith("#"))
						{
							int num = str.IndexOf('=');
							if (num != -1)
							{
								string key = str.Substring(0, num);
								string value = str.Substring(num + 1);
								data.Add(key, value);
							}
						}
					}
					streamReader.Close();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Could not process configuration file: " + ex.Message);
			}
		}

        public string this[string key]
        {
            get
            {
                if (this.data.ContainsKey(key))
                {
                    return this.data[key];
                }
                else
                {
                    throw new Exception("Unable to find configuration file data: " + key);
                }
            }
        }
	}
}
