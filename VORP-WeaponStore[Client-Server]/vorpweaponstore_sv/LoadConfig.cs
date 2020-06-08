﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace vorpweaponstore_sv
{
    public class LoadConfig : BaseScript
    {
        public static JObject Config = new JObject();
        public static string ConfigString;
        public static string jsonWeapons;
        public static Dictionary<string, string> Langs = new Dictionary<string, string>();
        public static string resourcePath = $"{API.GetResourcePath(API.GetCurrentResourceName())}";

        public LoadConfig()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:getConfig"] += new Action<Player>(getConfig);

            EventHandlers[$"{API.GetCurrentResourceName()}:getWeapons"] += new Action<Player>(getWeapons);

            LoadConfigAndLang();
        }

        private void LoadConfigAndLang()
        {
            if (File.Exists($"{resourcePath}/Config.json"))
            {
                ConfigString = File.ReadAllText($"{resourcePath}/Config.json", Encoding.UTF8);
                Config = JObject.Parse(ConfigString);
                if (File.Exists($"{resourcePath}/{Config["defaultlang"]}.json"))
                {
                    string langstring = File.ReadAllText($"{resourcePath}/{Config["defaultlang"]}.json", Encoding.UTF8);
                    Langs = JsonConvert.DeserializeObject<Dictionary<string, string>>(langstring);
                    Debug.WriteLine($"{API.GetCurrentResourceName()}: Language {Config["defaultlang"]}.json loaded!");
                }
                else
                {
                    Debug.WriteLine($"{API.GetCurrentResourceName()}: {Config["defaultlang"]}.json Not Found");
                }
            }
            else
            {
                Debug.WriteLine($"{API.GetCurrentResourceName()}: Config.json Not Found");
            }
        }

        private void getWeapons([FromSource]Player source)
        {
            int _source = int.Parse(source.Handle);
            TriggerEvent("vorpCore:getUserWeapons", _source, new Action<dynamic>((weapons) =>
            {
                jsonWeapons = JsonConvert.SerializeObject(weapons);
                source.TriggerEvent($"{API.GetCurrentResourceName()}:SendWeapons", jsonWeapons);
            }));
            
        }

        private void getConfig([FromSource]Player source)
        {
            
            source.TriggerEvent($"{API.GetCurrentResourceName()}:SendConfig", ConfigString, Langs);
        }
    }
}
