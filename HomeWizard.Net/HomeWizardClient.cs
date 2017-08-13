using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HomeWizard.Net.Converters;
using System.Net.Http;
using System.Threading.Tasks;

namespace HomeWizard.Net
{
    //Reference:
    //- https://github.com/depl0y/HCP/wiki/HomeWizard-API-Calls
    //- http://wiki.td-er.nl/index.php?title=Homewizard
    //- https://github.com/rvdvoorde/class.homewizard.php/blob/master/class.homewizard.php
    //- https://github.com/manuelvanrijn/homewizard-api/blob/master/homewizard-api.rb

    //Missing calls:
    //- wea/get Location?
    //Somfy
    //Graphs

    public class HomeWizardClient
    {
        private const string MinVersion = "3.35"; //HW version when this class was written
        private const string MaxVersion = "3.372"; //HW version when this class was last updated
                
        private readonly HttpClient _httpClient;
        private string _ipAddress;
        private string _password;

        public HomeWizardClient()
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        }

        /// <summary>
        /// Connect to a HomeWizard
        /// </summary>
        /// <param name="ipAddress">The IP address of the HomeWizard</param>
        /// <param name="password">The password of the HomeWizard</param>
        public HomeWizardClient(string ipAddress, string password) : base()
        {
            Connect(ipAddress, password);          
        }

        #region Connection

        /// <summary>
        /// Discover a HomeWizard on the local network
        /// </summary>
        /// <returns>Object containing the IP address. If no HomeWizard was found, the IP address is empty</returns>
        public async Task<Discovery> Discover()
        {
            var data = await DoRequest(Constants.DiscoveryUrl);
            return JsonConvert.DeserializeObject<Discovery>(data);
        }

        /// <summary>
        /// Connect to a HomeWizard
        /// </summary>
        /// <param name="ipAddress">The IP address of the HomeWizard</param>
        /// <param name="password">The password of the HomeWizard</param>
        public void Connect(string ipAddress, string password)
        {
            _ipAddress = ipAddress;
            _password = password;
        }

        /// <summary>
        /// Retrieve status information of the HomeWizard
        /// </summary>
        public async Task<Handshake> GetHandshake()
        {
            return await GetData<Handshake>("handshake");
        }

        /// <summary>
        /// Validate the password by doing a call and validating the results
        /// </summary>
        public async Task<bool> IsValidPassword()
        {
            return (await GetSunTimesForToday() != null);
        }

        #endregion

        #region Basic Data

        /// <summary>
        /// Get today's sunrise and sunset
        /// </summary>
        public async Task<SunTimes> GetSunTimesForToday()
        {
            return await GetData<SunTimes>("suntimes/today");
        }

        /// <summary>
        /// Get the sunrises and sunsets for the next seven days
        /// </summary>
        public async Task<IList<SunTimes>> GetSunTimesForWeek()
        {
            return await GetData<IList<SunTimes>>("suntimes");
        }

        /// <summary>
        /// Get a list of available switches
        /// </summary>
        public async Task<IList<Switch>> GetSwitches()
        {
            return await GetData<IList<Switch>>("swlist");
        }

        /// <summary>
        /// Get a list of available thermometers
        /// </summary>
        public async Task<IList<ThermoMeter>> GetThermoMeters()
        {
           return await GetData<IList<ThermoMeter>>("telist");
        }

        /// <summary>
        /// Get a list of available energymeters
        /// </summary>
        public async Task<IList<EnergyMeter>> GetEnergyMeters()
        {
            return await GetData<IList<EnergyMeter>>("enlist");
        }

        /// <summary>
        /// Get a list of available uv meters
        /// </summary>
        public async Task<IList<UvMeter>> GetUvMeters()
        {
            return await GetData<IList<UvMeter>>("uvlist");
        }

        /// <summary>
        /// Get a list of available windmeters
        /// </summary>
        public async Task<IList<WindMeter>> GetWindMeters()
        {
            return await GetData<IList<WindMeter>>("wilist");
        }

        /// <summary>
        /// Get a list of available rainmeters
        /// </summary>
        public async Task<IList<RainMeter>> GetRainMeters()
        {
            return await GetData<IList<RainMeter>>("ralist");
        }

        /// <summary>
        /// Get a list of available scenes
        /// </summary>
        public async Task<IList<Scene>> GetScenes()
        {
            return await GetData<IList<Scene>>("gplist");
        }

        /// <summary>
        /// Get a list of all sensors
        /// </summary>
        public async Task<Sensors> GetSensors()
        {
            return await GetData<Sensors>("get-sensors");
        }

        /// <summary>
        /// Get the status of the HomeWizard.
        /// Returns the same object as Sensors but not all properties are populated
        /// </summary>
        public async Task<Sensors> GetStatus()
        {
            return await GetData<Sensors>("get-status");
        }

        /// <summary>
        /// Get a list of triggers
        /// </summary>
        public async Task<IList<Trigger>> GetTriggers()
        {
            return await GetDataList<Trigger, TriggerConverter>("triggers");
        }

        #endregion

        #region Presets

        /// <summary>
        /// Switch to a built-in preset
        /// </summary>
        /// <param name="preset">Required: the preset to switch to</param>
        public async Task SwitchPreset(Preset preset)
        {
            await GetData("preset/" + ((int)preset));
        }

        #endregion

        #region Scenes

        /// <summary>
        /// Turn a scene on
        /// </summary>
        /// <param name="id">Required: Id of the scene</param>
        public async Task SceneOn(long id)
        {
            await GetData("gp/" + id + "/on");
        }

        /// <summary>
        /// Turn a scene off
        /// </summary>
        /// <param name="id">Required: Id of the scene</param>
        public async Task SceneOff(long id)
        {
            await GetData("gp/" + id + "/off");
        }

        /// <summary>
        /// Get a list of switches in a scene
        /// </summary>
        /// <param name="id">Required: Id of the scene</param>
        public async Task<IList<Switch>> GetSceneSwitches(long id)
        {
            return await GetDataList<Switch, SwitchConverter>("gp/get/" + id + "/switches");
        }

        //TODO: add scene
        //command: ?

        //TODO: remove scene
        //command: ?

        #endregion

        #region SwitchCodes

        internal class NewSwitch
        {
            public long Id { get; set; }
        }

        internal class SwitchCode
        {
            public string Code { get; set; }
        }

        /// <summary>
        /// Get switch codes
        /// </summary>
        /// <param name="id">Required: Id of the switch</param>
        /// <returns>List of codes associated with the switch</returns>
        public async Task<IList<string>> GetSwitchCodes(long id)
        {
            return await GetData<IList<string>>("sw/get/" + id + "/codes");
        }

        /// <summary>
        /// Remove a switch code of a switch
        /// </summary>
        /// <param name="id">Required: Id of the switch</param>
        /// <param name="code">Rquired: Code to remove</param>
        public async Task RemoveSwitchCode(long id, string code)
        {
            await GetData("sw/rc/" + id + "/" + code);
        }

        /// <summary>
        /// Edit a switch code of a switch
        /// </summary>
        /// <param name="id">Required: Id of the switch</param>
        /// <param name="oldCode">Required: The code to change</param>
        /// <param name="newCode">Required: The changed code</param>
        public async Task EditSwitchCode(long id, string oldCode, string newCode)
        {
            await GetData("sw/ec/" + id + "/" + oldCode + "/" + newCode);
        }

        /// <summary>
        /// Generate a code for a switch
        /// </summary>
        /// <returns>Code to associate with a (new) switch</returns>
        public async Task<string> GenerateSwitchCode()
        {
            SwitchCode response = await GetData<SwitchCode>("sw/generatekaku");
            return response == null ? string.Empty : response.Code;
        }

        /// <summary>
        /// Learn a code for a switch from a remote
        /// </summary>
        /// <returns>Code to associate with a (new) switch</returns>
        public async Task<string> LearnSwitchCode()
        {
            SwitchCode response = await GetData<SwitchCode>("sw/learn");
            return response == null ? string.Empty : response.Code;
        }

        #endregion

        #region Switches

        /// <summary>
        /// Turn on a switch. 
        /// </summary>
        /// <param name="id">Required: Id of the switch to turn on</param>
        /// <param name="level">Optional: The dim level (0-100)</param>
        public async Task SwitchOn(long id, int? level = null)
        {
            if (level.HasValue)
            {
                await DimSwitch(id, level.Value);
            }
            else
            {
                await GetData("sw/" + id + "/on");
            }
        }

        /// <summary>
        /// Turn a switch off
        /// </summary>
        /// <param name="id">Required: Id of the switch to turn off</param>
        public async Task SwitchOff(long id)
        {
            await GetData("sw/" + id + "/off");
        }

        /// <summary>
        /// Dim a switch (dimmers only)
        /// </summary>
        /// <param name="id">Required: Id of the switch to dim</param>
        /// <param name="level">Required: The dim level (0-100)</param>
        public async Task DimSwitch(long id, int level)
        {
            await GetData("sw/dim/" + id + "/" + NormalizeDimLevel(level));
        }

        //TODO: Hue lights
        //command: sw/<switch id>/[on/off]/[0..360]/[0..100]/[0..100]
        //Control a Hue light. Where the on or off switches the light on or off. The other values are: Hue, Saturation and Brightness in that order.

        /// <summary>
        /// Add a switch to the HomeWizard
        /// </summary>
        /// <param name="name">Required: Name of the switch (max 15 characters)</param>
        /// <param name="type">Required: Type of the switch</param>
        /// <param name="code">Required: Code of the switch</param>
        /// <returns>Id of the added switch</returns>
        public async Task<long> AddSwitch(string name, SwitchType type, string code)
        {
            string switchType = type.ToString().ToLowerInvariant();
            string urlEncodedName = WebUtility.UrlEncode(CleanName(name));
            NewSwitch added = await GetData<NewSwitch>("sw/add/" + urlEncodedName + "/" + switchType + "/" + code);
            return added.Id;
        }

        /// <summary>
        /// Edit a switch registered with the HomeWizard
        /// </summary>
        /// <param name="id">Required: Id of the switch</param>
        /// <param name="name">Required: Name of the switch (max 15 characters)</param>
        /// <param name="type">Required: Type of the switch</param>
        public async Task EditSwitch(long id, string name, SwitchType type)
        {
            string switchType = type.ToString().ToLowerInvariant();
            string urlEncodedName = WebUtility.UrlEncode(CleanName(name));
            await GetData("sw/edit/" + id + "/" + urlEncodedName + "/no/" + switchType + "/0");
        }

        /// <summary>
        /// Remove a switch from the HomeWizard
        /// </summary>
        /// <param name="id">Required: Id of the switch</param>
        public async Task RemoveSwitch(long id)
        {
            await GetData("sw/remove/" + id);
        }

        #endregion

        #region Triggers

        //TODO: add trigger
        //command: trigger/add/preset/0/r+60/s-60/0,1,2,3,4,5,6 --> {"status": "ok", "version": "3.35", "request": {"route": "/trigger" }, "response": {"id":1,"type":"preset","preset":0,"startTime":"r+60","endTime":"s-60","days":[0,1,2,3,4,5,6],"actions":[],"notification":{"receivers":[],"soundId":0},"active":"yes"}
        //command 2: trigger/1/action/add/dimmer/3/off/0 --> {"status": "ok", "version": "3.35", "request": {"route": "/trigger" }, "response": { "id":0}}

        public async Task RemoveTrigger(long id)
        {
            await GetData("trigger/remove/" + id);
        }

        #endregion

        #region HeatLink

        /// <summary>
        /// Set the target temperature of a HeatLink
        /// </summary>
        /// <param name="id">Required: Id of the HeatLink</param>
        /// <param name="temperature">Required: The target temperature (degrees Celsius). 0 to return control of temperature to the thermostat</param>
        /// <param name="minutes">Optional: Indicates the duration of the "override"</param>
        public async Task SetTargetTemperature(long id, decimal temperature, int? minutes = null)
        {
            //hl/0/settarget/<temperature>/<minutes> 
            temperature = (temperature == 0 ? 0 : NormalizeTempDegrees(temperature));
            string command = "hl/" + id + "/settarget/" + temperature;
            if (minutes.HasValue && minutes.Value > 0)
            {
                command += "/" + minutes.Value;
            }
            await GetData(command);
        }

        /// <summary>
        /// Set the target temperature of each preset
        /// </summary>
        /// <param name="id">Required: Id of the HeatLink</param>
        /// <param name="code">Required: Code used to pair the HeatLink</param>
        /// <param name="home">Required: Temperature of the "home" preset (degrees Celsius)</param>
        /// <param name="away">Required: Temperature of the "away" preset (degrees Celsius)</param>
        /// <param name="comfort">Required: Temperature of the "comfort" preset (degrees Celsius)</param>
        /// <param name="sleep">Required: Temperature of the "sleep" preset (degrees Celsius)</param>
        public async Task SetPresetTemperatures(long id, string code, decimal home, decimal away, decimal comfort, decimal sleep)
        {
            home = NormalizeTempDegrees(home);
            away = NormalizeTempDegrees(away);
            comfort = NormalizeTempDegrees(comfort);
            sleep = NormalizeTempDegrees(sleep);
            string command = "hl/edit/" + id + "/" + code + "/" + home + "/" + away + "/" + comfort + "/" + sleep;
            await GetData(command);
        }

        #endregion

        #region Internal methods

        private async Task<IList<T>> GetDataList<T, TC>(string command) where TC : JsonCreationConverter<T>, new()
        {
            //http://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm
            JObject responseObj = JObject.Parse(await GetData(command));
            IList<JToken> responseData = responseObj["response"].Children().ToList();
            IList<T> result = new List<T>();
            foreach (JToken jToken in responseData)
            {
                T item = JsonConvert.DeserializeObject<T>(jToken.ToString(), new TC());
                result.Add(item);
            }
            return result;
        }

        private async Task<T> GetData<T>(string command)
        {
            string dataString = await GetData(command);
            ApiResponse<T> data = JsonConvert.DeserializeObject<ApiResponse<T>>(dataString);
            return data.Response;
        }

        protected virtual async Task<string> GetData(string command)
        {
            string url = Constants.BaseUrl.Replace("{password}", _password);
            if ("handshake".Equals(command, StringComparison.OrdinalIgnoreCase))
            {
                url = Constants.HandshakeUrl;
            }
            url = url.Replace("{ipAddress}", _ipAddress).Replace("{command}", command);

            try
            {
                return await DoRequest(url);
            }
            catch(Exception e)
            {
                throw new HomeWizardClientException("Error sending request to HomeWizard", e);
            }
        }

        private async Task<string> DoRequest(string url)
        {
            using (HttpRequestMessage request = new HttpRequestMessage { Method = HttpMethod.Get })
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        private int NormalizeDimLevel(int dimLevel)
        {
            return (dimLevel < Constants.DimLevelMin) 
                ? Constants.DimLevelMin
                : (dimLevel > Constants.DimLevelMax) 
                    ? Constants.DimLevelMax
                    : dimLevel;
        }

        private decimal NormalizeTempDegrees(decimal targetTemp)
        {
            return (targetTemp < Constants.TemperatureMin)
                ? Constants.TemperatureMin
                : (targetTemp > Constants.TemperatureMax)
                    ? Constants.TemperatureMax
                    : targetTemp;
        }

        private string CleanName(string name)
        {
            name = name.Trim();
            if (name.Length > Constants.SwitchNameLength)
            {
                name = name.Substring(0, Constants.SwitchNameLength);
            }
            return name;
        }

        #endregion

    }
}
