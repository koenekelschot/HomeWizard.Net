using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HomeWizard.Net.Test
{
    [TestClass]
    public class HomeWizardClientTest
    {
        private class HomeWizardTestClient : HomeWizardClient
        {
            public string TestData { private get; set; }

            public HomeWizardTestClient(string ipAddress, string password) : base(ipAddress, password) { }

            protected override async Task<string> GetData(string command)
            {
                return TestData;
            }
        }

        [TestMethod]
        public async Task HandshakeTest()
        {
            HomeWizardTestClient testHomeWizard = new HomeWizardTestClient("127.0.0.1", "test");
            testHomeWizard.TestData = "{\"status\": \"ok\", \"version\": \"3.35\", \"request\": {\"route\": \"/handshake\" }, \"response\": {\"homewizard\": \"yes\", \"version\": \"3.35\", \"firmwareupdateavailable\": \"no\", \"appupdaterequired\": \"no\", \"serial\": \"999999999\"}}";
            Handshake handshake = await testHomeWizard.GetHandshake();

            Assert.IsTrue(handshake.HomeWizard);
            Assert.AreEqual("3.35", handshake.Version);
            Assert.IsFalse(handshake.FirmwareUpdateAvailable);
            Assert.IsFalse(handshake.AppUpdateRequired);
            Assert.AreEqual("999999999", handshake.Serial);
        }

        [TestMethod]
        public async Task SensorsTest()
        {
            HomeWizardTestClient testHomeWizard = new HomeWizardTestClient("127.0.0.1", "test");
            testHomeWizard.TestData = "{\"status\": \"ok\", \"version\": \"3.35\", \"request\": {\"route\": \"/get-sensors\" }, \"response\": {\"preset\":0,\"time\":\"2015-12-27 16:28\",\"switches\":[{\"id\":0,\"name\":\"Dimmer 1\",\"type\":\"dimmer\",\"status\":\"on\",\"dimlevel\":73,\"favorite\":\"no\"},{\"id\":1,\"name\":\"Switch 1\",\"type\":\"switch\",\"status\":\"on\",\"favorite\":\"no\"},{\"id\":2,\"name\":\"Hue 1\",\"type\":\"hue\",\"status\":\"on\",\"hue_id\":0,\"light_id\":2,\"color\":{\"hue\":60,\"sat\":28,\"bri\":100},\"favorite\":\"no\"}],\"uvmeters\":[],\"windmeters\":[],\"rainmeters\":[],\"thermometers\":[],\"weatherdisplays\":[], \"energymeters\": [], \"energylinks\": [], \"heatlinks\": [{\"id\": 0, \"favorite\": \"no\", \"name\": \"HeatLink\", \"code\": \"999999\", \"pump\": \"on\", \"heating\": \"on\", \"dhw\": \"off\", \"rte\": 19.757, \"rsp\": 20.000, \"tte\": 0.000, \"ttm\": null, \"wp\": 0.000, \"wte\": 65.437, \"ofc\": 0, \"odc\": 0, \"presets\": [{ \"id\": 0, \"te\": 20.00},{ \"id\": 1, \"te\": 12.00},{ \"id\": 2, \"te\": 21.00},{ \"id\": 3, \"te\": 16.00}]}], \"hues\": [{\"id\":0,\"name\":\"Hue Bridge\",\"username\":\"abcdefghijklmno\",\"ip\":\"127.0.0.1\"}], \"scenes\": [{\"id\": 0, \"name\": \"Scene 1\", \"favorite\": \"no\"}], \"kakusensors\": [], \"cameras\": []}}";
            Sensors sensors = await testHomeWizard.GetSensors();

            Assert.AreEqual(0, sensors.Preset);
            Assert.AreEqual(new DateTime(2015, 12, 27, 16, 28, 0), sensors.Time);

            Assert.AreEqual(3, sensors.Switches.Count);
            Assert.AreEqual(SwitchType.Dimmer, sensors.Switches[0].Type);
            Assert.IsInstanceOfType(sensors.Switches[0], typeof(Dimmer));
            Assert.IsTrue(sensors.Switches[0].IsDimmer);
            Assert.IsFalse(sensors.Switches[0].IsHue);
            Assert.AreEqual(SwitchType.Switch, sensors.Switches[1].Type);
            Assert.IsInstanceOfType(sensors.Switches[1], typeof(Switch));
            Assert.IsFalse(sensors.Switches[1].IsDimmer);
            Assert.IsFalse(sensors.Switches[1].IsHue);
            Assert.AreEqual(SwitchType.Hue, sensors.Switches[2].Type);
            Assert.IsInstanceOfType(sensors.Switches[2], typeof(HueLight));
            Assert.IsFalse(sensors.Switches[2].IsDimmer);
            Assert.IsTrue(sensors.Switches[2].IsHue);

            Assert.AreEqual(0, sensors.UvMeters.Count);
            Assert.AreEqual(0, sensors.WindMeters.Count);
            Assert.AreEqual(0, sensors.RainMeters.Count);
            Assert.AreEqual(0, sensors.ThermoMeters.Count);
            Assert.AreEqual(0, sensors.WeatherDisplays.Count);
            Assert.AreEqual(0, sensors.EnergyMeters.Count);
            Assert.AreEqual(0, sensors.EnergyLinks.Count);
            Assert.AreEqual(1, sensors.HeatLinks.Count);
            Assert.AreEqual(1, sensors.Hues.Count);
            Assert.AreEqual(1, sensors.Scenes.Count);
            Assert.AreEqual(0, sensors.KakuSensors.Count);
            Assert.AreEqual(0, sensors.Cameras.Count);
        }

        [TestMethod]
        public async Task SceneSwitchesTest()
        {
            HomeWizardTestClient testHomeWizard = new HomeWizardTestClient("127.0.0.1", "test");
            testHomeWizard.TestData = "{\"status\": \"ok\", \"version\": \"3.35\", \"request\": {\"route\": \"/gp\" }, \"response\": [{\"type\":\"dimmer\",\"id\":0,\"name\":\"Switch 1\",\"onstatus\":33,\"offstatus\":100},{\"type\":\"dimmer\",\"id\":2,\"name\":\"Switch 2\",\"onstatus\":30,\"offstatus\":100},{\"type\":\"switch\",\"id\":3,\"name\":\"Switch 3\",\"onstatus\":18,\"offstatus\":100}]}";
            IList<Switch> switches = await testHomeWizard.GetSceneSwitches(1);

            Assert.AreEqual(3, switches.Count);
            Assert.AreEqual(SwitchType.Dimmer, switches[0].Type);
            Assert.IsInstanceOfType(switches[0], typeof(Dimmer));
            Assert.IsTrue(switches[0].IsDimmer);
            Assert.IsFalse(switches[0].IsHue);
            Assert.AreEqual(SwitchType.Dimmer, switches[1].Type);
            Assert.IsInstanceOfType(switches[1], typeof(Dimmer));
            Assert.IsTrue(switches[1].IsDimmer);
            Assert.IsFalse(switches[1].IsHue);
            Assert.AreEqual(SwitchType.Switch, switches[2].Type);
            Assert.IsInstanceOfType(switches[2], typeof(Switch));
            Assert.IsFalse(switches[2].IsDimmer);
            Assert.IsFalse(switches[2].IsHue);
        }

        [TestMethod]
        public async Task TriggerTest()
        {
            HomeWizardTestClient testHomeWizard = new HomeWizardTestClient("127.0.0.1", "test");
            testHomeWizard.TestData = "{\"status\": \"ok\", \"version\": \"3.35\", \"request\": {\"route\": \"/triggers\" }, \"response\": [{\"id\":0,\"type\":\"time\",\"time\":\"17:00\",\"presets\":[0,1,2,3],\"days\":[1,2,3,4,5],\"actions\":[{\"id\":0,\"deviceType\":\"switch\",\"deviceId\":1,\"value\":\"on\",\"offTime\":0}],\"notification\":{\"receivers\":[],\"soundId\":0},\"active\":\"yes\"},{\"id\":1,\"type\":\"preset\",\"preset\":0,\"startTime\":\"r+60\",\"endTime\":\"s-60\",\"days\":[0,1,2,3,4,5,6],\"actions\":[{\"id\":0,\"deviceType\":\"dimmer\",\"deviceId\":3,\"value\":\"50\",\"offTime\":0}],\"notification\":{\"receivers\":[],\"soundId\":0},\"active\":\"yes\"}]}";
            IList<Trigger> triggers = await testHomeWizard.GetTriggers();

            Assert.AreEqual(2, triggers.Count);

            Assert.AreEqual(TriggerType.Time, triggers[0].Type);
            Assert.IsInstanceOfType(triggers[0], typeof(TimeTrigger));
            Assert.IsTrue(triggers[0].IsTimeTrigger);
            Assert.IsFalse(triggers[0].IsPresetTrigger);
            Assert.AreEqual("17:00", ((TimeTrigger)triggers[0]).Time);
            Assert.AreEqual(Preset.Home, ((TimeTrigger)triggers[0]).Presets[0]);
            Assert.AreEqual(Preset.Away, ((TimeTrigger)triggers[0]).Presets[1]);
            Assert.AreEqual(Preset.Sleep, ((TimeTrigger)triggers[0]).Presets[2]);
            Assert.AreEqual(Preset.Holiday, ((TimeTrigger)triggers[0]).Presets[3]);
            Assert.AreEqual(Day.Monday, triggers[0].Days[0]);
            Assert.AreEqual(Day.Tuesday, triggers[0].Days[1]);
            Assert.AreEqual(Day.Wednesday, triggers[0].Days[2]);
            Assert.AreEqual(Day.Thursday, triggers[0].Days[3]);
            Assert.AreEqual(Day.Friday, triggers[0].Days[4]);
            Assert.IsTrue(triggers[0].Active);
            Assert.AreEqual(1, triggers[0].Actions[0].DeviceId);
            Assert.AreEqual("on", triggers[0].Actions[0].Value);

            Assert.AreEqual(TriggerType.Preset, triggers[1].Type);
            Assert.IsInstanceOfType(triggers[1], typeof(PresetTrigger));
            Assert.IsFalse(triggers[1].IsTimeTrigger);
            Assert.IsTrue(triggers[1].IsPresetTrigger);
            Assert.AreEqual(Preset.Home, ((PresetTrigger)triggers[1]).Preset);
            Assert.AreEqual("r+60", ((PresetTrigger)triggers[1]).StartTime); //60 minutes after sunrise
            Assert.AreEqual("s-60", ((PresetTrigger)triggers[1]).EndTime); //60 minutes before sunset
            Assert.AreEqual(3, triggers[1].Actions[0].DeviceId);
            Assert.AreEqual("50", triggers[1].Actions[0].Value);
        }
    }
}
