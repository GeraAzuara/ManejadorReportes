using ManejadorReportes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace ManejadorReportes.Controllers._CustomLogic
{
    public class Generic_DispatcherConfiguration
    {
        private readonly List<Generic_Report> _ListReports = new List<Generic_Report>();
        private readonly List<Generic_DBConnection> _ListDbConectors = new List<Generic_DBConnection>();

        public Generic_DispatcherConfiguration()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            //string xmlPath = Path.Combine(basePath, "API_Connectors.xml");
            ConfigConnectors(Path.Combine(basePath, "API_Connectors.xml"));
            ConfigReports(Path.Combine(basePath, "API_Reports.xml"));
        }

        public void ConfigConnectors(string xmlRelativePath)
        {
            XmlDocument xmlDoc = new XmlDocument(); // Crear una instancia de XmlDocument
            xmlDoc.Load(xmlRelativePath);           // Cargar el archivo XML
            XmlNodeList conexiones = xmlDoc.SelectNodes("/Connectors/Connector");
            foreach (XmlNode conexionNodo in conexiones)
            {
                _ListDbConectors.Add(new Generic_DBConnection(conexionNodo));
            }
        }

        public void ConfigReports(string xmlRelativePath)
        {
            XmlDocument xmlDoc = new XmlDocument(); // Crear una instancia de XmlDocument
            xmlDoc.Load(xmlRelativePath);           // Cargar el archivo XML
            XmlNodeList reportes = xmlDoc.SelectNodes("/Reports/Report");

            foreach (XmlNode conexionNodo in reportes)
            {
                _ListReports.Add(new Generic_Report(conexionNodo));
            }
        }

        public Generic_DBConnection SetUpConnector(string KeyDbConnector)
        {
            return _ListDbConectors.FirstOrDefault(c => c._keyDbConector.Equals(KeyDbConnector));
        }

        public Generic_Report SetUpReport(string KeyReport)
        {
            return _ListReports.FirstOrDefault(r => r.ReportKey.Equals(KeyReport));
        }
    }
}