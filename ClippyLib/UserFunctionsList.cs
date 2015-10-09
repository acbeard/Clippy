using System;
using ClippyLib.Settings;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Text;

namespace ClippyLib
{
	public class UserFunctionsList : List<UserFunction>
	{
		SettingsObtainer _settings;

		public UserFunctionsList ()
		{
			_settings = SettingsObtainer.CreateInstance();
			InitFunctions();
		}

		private void InitFunctions()
		{
			GetListOfUserFunctionsFromFile();
		}

		private void GetListOfUserFunctionsFromFile()
		{
			if(!File.Exists(_settings.UdfLocation))
				throw new UndefinedFunctionException("No UDF file is set.");

			XDocument udfDoc = XDocument.Load(_settings.UdfLocation);


			XElement root = udfDoc.Root;
			if(root != null)
			{
				foreach(var command in root.Elements("command"))
				{
					this.Add(new UserFunction(command));
				}
			}
		}

		public UserFunction GetUserFunction(params string[] key)
		{
			if(key.Length < 1)
				throw new ArgumentNullException("key","No argument was passed to find function");

			return this.FirstOrDefault(f => f.Name.Equals(key[0], StringComparison.CurrentCultureIgnoreCase));
		}

		public bool CommandExists(params string[] key)
		{
			return GetUserFunction(key) != null;
		}

		public void DescribeFunctions(StringBuilder output)
		{
			foreach (UserFunction udf in this)
			{
				output.AppendLine(String.Concat(
					udf.Name,
					"  -  ",
					string.IsNullOrEmpty(udf.Description) ? "No description available" : udf.Description));
			}
		}

		public List<string> GetFunctions()
		{
			return (from func in this
			        select func.Name).ToList();
		}

	}
}

