using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SM.Store.Api.Common
{
    public static class TransformAppConfigService
    {
        public static void TransformAppConfig(this IServiceCollection services, IWebHostEnvironment env)
        {
            try
            {
                //Get JSON tokens to be transformed.
                var appSettingTokens = StaticConfigs.GetWebConfigSetting("TransformJsonAppSettingTokens");
                if (string.IsNullOrEmpty(appSettingTokens)) return;
                
                var folderPath = Directory.GetCurrentDirectory();
                var baseFilePath = folderPath + @"\appsettings.json";
                var envFilePath = folderPath + $@"\appsettings.{env.EnvironmentName}.json";
                if (!File.Exists(baseFilePath) || !File.Exists(envFilePath)) return;

                var baseObj = JObject.Parse(File.ReadAllText(baseFilePath));
                var baseTokenList = baseObj.Children();

                var envObj = JObject.Parse(File.ReadAllText(envFilePath));
                var envTokenList = envObj.Children();               

                if (appSettingTokens.ToLower() == "all")
                {
                    //Iterate all tokens in envTokenList.
                    foreach (var envToken in envTokenList)
                    {                        
                        try
                        {
                            ProcessTransform(baseObj, envToken.Path, envToken.First.ToString());
                        }
                        catch (Exception ex)
                        {
                            //Nested JSON causes "Unexpected character encountered while parsing value" error.
                            //Use built-in IConfiguration injection instead.
                            continue;
                        }
                    }
                }
                else
                {
                    //Iterate tokens in appSettingTokens list.
                    var appSettingTokenList = appSettingTokens.Split(',');
                    foreach (var token in appSettingTokenList)
                    {
                        var envToken = envObj[token];
                        if (envToken != null)
                        {
                            try
                            {
                                ProcessTransform(baseObj, token, envToken.ToString());
                            }
                            catch (Exception ex)
                            {                                
                                continue;
                            }
                        }                        
                    }
                }

                string output = JsonConvert.SerializeObject(baseObj, Newtonsoft.Json.Formatting.Indented);
                //Test for server.
                //LogUtil.WriteToLogFile(LogUtil.ResolveFilePath(@"Logs\Test.log"), $"Output Json: {output}");

                File.WriteAllText(baseFilePath, output);
            }
            catch (Exception ex)
            {
                ProcessError.StartupError(ex, "Error occurred when transforming app config at startup.").Wait();
                throw ex;
            }
        }

        private static void ProcessTransform(JObject baseObj, string toDoTokenPath, string envTokenString)
        {
            var baseItem = baseObj.SelectToken(toDoTokenPath);
            if (baseItem != null)
            {
                Dictionary<string, string> envConfigList;

                //Not support nested JSON object - it's <string, string>, not <string, object>.                
                envConfigList = JsonConvert.DeserializeObject<Dictionary<string, string>>(envTokenString);

                foreach (var envItem in envConfigList)
                {
                    if (baseItem[envItem.Key] != null)
                    {
                        baseItem[envItem.Key] = envItem.Value;
                    }
                }
            }
        }
    }

}
