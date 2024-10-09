
using System;
using System.Linq;
using Dissonity.Models.Builders;

namespace Dissonity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DissonityConfigAttribute : Attribute
    {
        public static _UserData GetUserConfig()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var found = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDefined(typeof(DissonityConfigAttribute), false));

            var type = found.FirstOrDefault();

            //? No classes with the attribute
            if (type == null)
            {
                throw new Exception("[Dissonity]: No class with the DissonityConfigAttribute found. You can create a config file with (Right click > Create > Dissonity > Configuration)");
            }

            var instance = Activator.CreateInstance(type);
            
            //? Not inherited
            if (instance is not ISdkConfiguration)
            {
                throw new Exception("[Dissonity]: The class with the DissonityConfigAttribute must inherit from SdkConfiguration");
            }

            //? More than one
            if (found.Count() > 1)
            {
                Utils.DissonityLogWarning("More than one classes with the DissonityConfigAttribute found. This can produce unexpected behaviors.");
            }

            //\ Get fields
            string clientId = ((ISdkConfiguration) instance).ClientId;
            bool disableLogOverride = ((ISdkConfiguration) instance).DisableConsoleLogOverride;
            string[] oauthScopes = ((ISdkConfiguration) instance).OauthScopes;
            string tokenRequestPath = ((ISdkConfiguration) instance).TokenRequestPath;
            Type requestType = ((ISdkConfiguration) instance).GetRequestType();
            Type responseType = ((ISdkConfiguration) instance).GetResponseType();
            bool disableDissonityInfoLogs = ((ISdkConfiguration) instance).DisableDissonityInfoLogs;
            MappingBuilder[] mappings = ((ISdkConfiguration) instance).Mappings;
            PatchUrlMappingsConfigBuilder patchConfig = ((ISdkConfiguration) instance).PatchUrlMappingsConfig;

            // Handle token request path
            tokenRequestPath = tokenRequestPath.StartsWith("/")
                ? tokenRequestPath
                : $"/{tokenRequestPath}";

            var data = new _UserData() {
                ClientId = clientId,
                DisableConsoleLogOverride = disableLogOverride,
                OauthScopes = oauthScopes,
                TokenRequestPath = tokenRequestPath,
                ServerTokenRequest = requestType,
                ServerTokenResponse = responseType,
                DisableDissonityInfoLogs = disableDissonityInfoLogs,
                Mappings = mappings,
                PatchUrlMappingsConfig = patchConfig,
            };

            return data;
        }
    }
}