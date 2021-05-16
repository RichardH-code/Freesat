using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using static System.String;

namespace Freesat
{
    class Program
    {
        public static class Config
        {
            public static uint iScnServiceId = 0;
            public static uint iScnPositionEast = 0;
            public static uint iScnCarrierFrequency = 0;
            public static uint iScnPolarization = 0;
            public static uint iScnSymbolRate = 0;
            public static uint iScnOriginalNetworkId = 0;
            public static uint iScnTransportStreamId = 0;
            public static uint iScnName = 0;
            public static string strScnCsvName = "";
            public static string strHeadendId = "";
            public static string strHeadendLang = "";
            public static string strPolarizationVertical = "";
            public static string strRegion = "";
            public static string strSatName = "";
            public static string strSatPositionEast = "";
            static Config() { }

            //public static bool Initialize(string file)
            public static void Initialize(string file)
            {
                var xmlConfig = new XmlDocument();
                xmlConfig.Load(file);

                strHeadendId = xmlConfig.SelectSingleNode("//_csiId").InnerText;
                strHeadendLang = xmlConfig.SelectSingleNode("//_languageIso639").InnerText;
                strPolarizationVertical = xmlConfig.SelectSingleNode("//vertical_string").InnerText;
                strScnCsvName = xmlConfig.SelectSingleNode("//csv_name").InnerText;
                strRegion = xmlConfig.SelectSingleNode("//_isoCode").InnerText;
                strSatName = xmlConfig.SelectSingleNode("//DvbsSatellite/_name").InnerText;
                strSatPositionEast = xmlConfig.SelectSingleNode("//DvbsSatellite/_positionEast").InnerText;

                try
                {
                    iScnServiceId = uint.Parse(xmlConfig.SelectSingleNode("//_serviceId/index").InnerText) - 1;
                    iScnPositionEast = uint.Parse(xmlConfig.SelectSingleNode("//Columns/_positionEast/index").InnerText) - 1;
                    iScnCarrierFrequency = uint.Parse(xmlConfig.SelectSingleNode("//_carrierFrequency/index").InnerText) - 1;
                    iScnPolarization = uint.Parse(xmlConfig.SelectSingleNode("//_polarization/index").InnerText) - 1;
                    iScnSymbolRate = uint.Parse(xmlConfig.SelectSingleNode("//_symbolRate/index").InnerText) - 1;
                    iScnOriginalNetworkId = uint.Parse(xmlConfig.SelectSingleNode("//_originalNetworkId/index").InnerText) - 1;
                    iScnTransportStreamId = uint.Parse(xmlConfig.SelectSingleNode("//_transportStreamId/index").InnerText) - 1;
                    iScnName = uint.Parse(xmlConfig.SelectSingleNode("//_name/index").InnerText) - 1;
                }
                catch (FormatException e)
                {
                    //Console.WriteLine(e.Message);
                    //return false;
                    Console.WriteLine("Cannot parse element as integer.");
                    throw;
                }
                //return true;
            }
        }
        public class Service
        {
            private bool _isDefault;
            private ushort? _positionEast;
            private ushort? _carrierFrequency;
            private bool _polarizationH;
            private ushort? _symbolRate;
            private ushort? _originalNetworkId;
            private readonly bool _originalNetworkIdValid = true;
            private ushort? _transportStreamId;
            private readonly bool _transportStreamIdValid = true;
            private ushort? _serviceId;
            private string _name;
            private ushort? _preset;
            private bool _presetValid;

            public bool IsDefault { get => _isDefault; set => _isDefault = value; }
            public string PositionEast { get => _positionEast.ToString(); set => _positionEast = ushort.Parse(value.Trim()); }
            public string CarrierFrequency { get => _carrierFrequency.ToString(); set => _carrierFrequency = ushort.Parse(value.Trim()); }
            public string Polarization { get => _polarizationH ? "0" : "1"; set => _polarizationH = !value.Equals(Config.strPolarizationVertical); }
            public string SymbolRate { get => _symbolRate.ToString(); set => _symbolRate = ushort.Parse(value.Trim()); }
            public string OriginalNetworkId { get => _originalNetworkId.ToString(); set => _originalNetworkId = ushort.Parse(value.Trim()); }
            public string OriginalNetworkIdValid { get => _originalNetworkIdValid.ToString().ToLower(); }
            public string TransportStreamId { get => _transportStreamId.ToString(); set => _transportStreamId = ushort.Parse(value.Trim()); }
            public string TransportStreamIdValid { get => _transportStreamIdValid.ToString().ToLower(); }
            public string ServiceId { get => _serviceId.ToString(); set => _serviceId = ushort.Parse(value.Trim()); }
            public string Name { get => _name; set => _name = value; }
            public string Preset { get => _preset.ToString(); set => _preset = ushort.Parse(value.Trim()); }
            public string PresetValid { get => _presetValid.ToString().ToLower(); set => _presetValid = bool.Parse(value); }
            public string ServiceTypeValid { get => bool.TrueString.ToLower(); }
            public string IsEncryptedValid { get => bool.TrueString.ToLower(); }
            public string UidT
            {
                get => Join("!", "", "DvbsDataSet", Format("DvbsSatellite[{0}]", this.PositionEast), Format("DvbsTransponder[{0}]", Join(",", this.CarrierFrequency, "Linear" + (this._polarizationH ? "Horizontal" : "Vertical"), this.SymbolRate)));
            }
            public string UidS
            {
                get => Join("!", this.UidT, Format("DvbsService[{0}]", this.ServiceId));
            }
            public string UidC
            {
                get => Join("!", "", "DvbsDataSet", Format("DvbsHeadend[{0}]", Config.strHeadendId), Format("DvbsChannel[{0}]", string.Join(":", this.PositionEast, this.CarrierFrequency, this.OriginalNetworkId, this.TransportStreamId, this.ServiceId)));
            }
        }
        static int Main()
        {
            try
            {
                const string strSep = "\t";
                string strAppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";

                Console.WriteLine("Loading configuration.");
                Config.Initialize(strAppPath + "config.xml");

                var argsXslt = new XsltArgumentList();
                argsXslt.AddParam("satellite_name", "", Config.strSatName);
                argsXslt.AddParam("satellite_positionEast", "", Config.strSatPositionEast);
                argsXslt.AddParam("region_isoCode", "", Config.strRegion);
                argsXslt.AddParam("headend_csiId", "", Config.strHeadendId);
                argsXslt.AddParam("headend_languageIso639", "", Config.strHeadendLang);

                Console.WriteLine("Reading tuning parameter file.");
                using var srTune = new StreamReader(strAppPath + Config.strScnCsvName + ".csv", Encoding.GetEncoding(65000));
                string[] strHdrsTune = srTune.ReadLine().Split(strSep);

                Console.WriteLine(strHdrsTune[Config.iScnPositionEast] + " -> DvbsSatellite _positionEast");
                Console.WriteLine(strHdrsTune[Config.iScnCarrierFrequency] + " -> DvbsTransponder _carrierFrequency");
                Console.WriteLine(strHdrsTune[Config.iScnPolarization] + " -> DvbsTransponder _polarization");
                Console.WriteLine(strHdrsTune[Config.iScnSymbolRate] + " -> DvbsTransponder _symbolRate");
                Console.WriteLine(strHdrsTune[Config.iScnOriginalNetworkId] + " -> DvbsTransponder _originalNetworkId");
                Console.WriteLine(strHdrsTune[Config.iScnName] + " -> DvbsService _name");
                Console.WriteLine(strHdrsTune[Config.iScnServiceId] + " -> DvbsService _serviceId");
                Console.WriteLine(strHdrsTune[Config.iScnTransportStreamId] + " -> DvbsService _transportStreamId");

                var tblServices = new List<Service>();
                string[] strarr;
                while (srTune.Peek() > 0)
                {
                    strarr = srTune.ReadLine().Split(strSep);
                    if (!tblServices.Exists(s => s.ServiceId.Equals(strarr[Config.iScnServiceId])))
                    {
                        tblServices.Add(new Service()
                        {
                            PositionEast = strarr[Config.iScnPositionEast],
                            CarrierFrequency = strarr[Config.iScnCarrierFrequency],
                            Polarization = strarr[Config.iScnPolarization],
                            SymbolRate = strarr[Config.iScnSymbolRate],
                            OriginalNetworkId = strarr[Config.iScnOriginalNetworkId],
                            TransportStreamId = strarr[Config.iScnTransportStreamId],
                            ServiceId = strarr[Config.iScnServiceId],
                            Name = strarr[Config.iScnName],
                        });
                        Service service = tblServices.Last();
                        service.IsDefault = tblServices.FindAll(s => s.UidT.Equals(service.UidT)).Count == 1;
                    }
                }

                Console.WriteLine("Reading EPG123 file.");
                string file = strAppPath + "EPG123Client.csv";
                if (File.Exists(file))
                {
                    using var srEpg = new StreamReader(file, Encoding.UTF8);
                    string[] strHdrsEpg = srEpg.ReadLine().Split(strSep);
                    while (srEpg.Peek() > 0)
                    {
                        strarr = srEpg.ReadLine().Split(strSep);
                        string[] strarrMatchName = strarr[Array.IndexOf(strHdrsEpg, "MatchName")].Split(":");
                        if (strarrMatchName[0].Equals("DVBS"))
                        {
                            Service service = tblServices.Find(s => s.PositionEast.Equals(strarrMatchName[1]) && s.ServiceId.Equals(strarrMatchName[5]));
                            if (service != null)
                            {
                                service.Name = strarr[Array.IndexOf(strHdrsEpg, "Call Sign")];
                                service.Preset = strarr[Array.IndexOf(strHdrsEpg, "Number")];
                                service.PresetValid = Boolean.TrueString;
                            }
                        }
                    }
                }
                else Console.WriteLine("Nothing to read.");

                Console.WriteLine("Writing MXF to " + strAppPath + @"Target.");
                Directory.CreateDirectory(strAppPath + "Target");
                var xslt = new XslCompiledTransform();
                using Stream strmXsl = Assembly.GetExecutingAssembly().GetManifestResourceStream("Freesat.Resources.freesat.xsl");
                using var rdrXsl = XmlReader.Create(strmXsl);
                xslt.Load(rdrXsl);

                using var strmXml = new MemoryStream();
                using var wrtXml = XmlWriter.Create(strmXml);

                wrtXml.WriteStartElement("MXF");

                wrtXml.WriteStartElement("_channels");
                tblServices.FindAll(s => bool.Parse(s.PresetValid)).ForEach(service =>
                {
                    wrtXml.WriteStartElement("DvbsChannel");
                    wrtXml.WriteAttributeString("uid", service.UidC);
                    wrtXml.WriteAttributeString("_service", service.UidS);
                    wrtXml.WriteAttributeString("_presetValid", service.PresetValid);
                    wrtXml.WriteAttributeString("_preset", service.Preset);

                    wrtXml.WriteEndElement();
                });
                wrtXml.WriteEndElement();

                wrtXml.WriteStartElement("_transponders");
                tblServices.FindAll(s => s.IsDefault).ForEach(service =>
                {
                    wrtXml.WriteStartElement("DvbsTransponder");
                    wrtXml.WriteAttributeString("uid", service.UidT);
                    wrtXml.WriteAttributeString("_carrierFrequency", service.CarrierFrequency);
                    wrtXml.WriteAttributeString("_polarization", service.Polarization);
                    wrtXml.WriteAttributeString("_symbolRate", service.SymbolRate);
                    wrtXml.WriteAttributeString("_originalNetworkIdValid", service.OriginalNetworkIdValid);
                    wrtXml.WriteAttributeString("_transportStreamIdValid", service.TransportStreamIdValid);
                    wrtXml.WriteAttributeString("_originalNetworkId", service.OriginalNetworkId);
                    wrtXml.WriteAttributeString("_transportStreamId", service.TransportStreamId);
                    wrtXml.WriteEndElement();
                });
                wrtXml.WriteEndElement();

                wrtXml.WriteStartElement("_services");
                tblServices.ForEach(service =>
                {
                    wrtXml.WriteStartElement("DvbsService");
                    wrtXml.WriteAttributeString("uid", service.UidS);
                    wrtXml.WriteAttributeString("_name", service.Name);
                    ushort i = ushort.Parse(service.ServiceId);
                    wrtXml.WriteAttributeString("_serviceId", i > (ushort)short.MaxValue ? (-(short)(ushort.MaxValue + 1 - i)).ToString() : service.ServiceId);
                    wrtXml.WriteAttributeString("_serviceTypeValid", service.ServiceTypeValid);
                    wrtXml.WriteAttributeString("_isEncryptedValid", service.IsEncryptedValid);
                    wrtXml.WriteEndElement();
                });
                wrtXml.WriteEndElement();

                wrtXml.WriteEndElement();

                wrtXml.Flush();
                strmXml.Seek(0, SeekOrigin.Begin);

                var wrtSettings = xslt.OutputSettings.Clone();
                wrtSettings.NewLineOnAttributes = true;

                using var rdrXml = XmlReader.Create(strmXml);
                using var wrtMxf = XmlWriter.Create(strAppPath + @"Target\freesat.mxf", wrtSettings);
                xslt.Transform(rdrXml, argsXslt, wrtMxf);

                Console.WriteLine("Copying files to " + strAppPath + @"Target.");
                File.WriteAllText(strAppPath + @"Target\EPG.reg", Properties.Resources.EPG);
                File.WriteAllText(strAppPath + @"Target\loadMXF.cmd", Properties.Resources.loadMXF);
                File.WriteAllText(strAppPath + @"Target\Readme.txt", Properties.Resources.Readme);

                Console.WriteLine("Success.");
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail:");
                Console.WriteLine(e.Message);
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                return -1;
            }
        }
    }
}
