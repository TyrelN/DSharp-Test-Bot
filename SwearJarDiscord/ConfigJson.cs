using Newtonsoft.Json;

namespace SwearJarDiscord
{
    //file to access properties regarding the json file so that properties can be seen in program.cs
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token {get; private set;}
        [JsonProperty("prefix")]
        public string Prefix {get; private set;}
    }
}