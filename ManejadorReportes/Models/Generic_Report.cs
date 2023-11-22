using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Net.Http;
using System.Net;
using ManejadorReportes.Controllers._CustomLogic;
using System.Runtime.Remoting.Messaging;

namespace ManejadorReportes.Models
{
    public class Generic_Report
    {
        public string ReportKey { get; set; }
        public string ReportPath { get; set; }
        public string ReportExtension { get; set; }
        public string ReportDownloadName { get; set; }
        public string DbConectionKey { get; set; }
        public string DbQuery { get; set; }
        public List<SqlParameter> DbParameters { get; set; }

        
        
        /*  Funcionalidad Encapsulada   */
        protected Generic_ParameterFeature FeatureCrystalParam { get; set; }
        protected Generic_ParameterFeature FeatureSqlParam { get; set; }
        protected Generic_QueryFeature FeatureQuery { get; set; }
        protected Generic_ReportFeature FeatureXml { get; set; }

        public Generic_Report(XmlNode reporteNode)
        {
            FeatureCrystalParam = new Generic_ParameterFeature(false, new List<string>(), 0);
            FeatureSqlParam = new Generic_ParameterFeature(false, new List<string>(), 0);
            FeatureQuery = new Generic_QueryFeature(false, new List<string>(), null);
            FeatureXml = new Generic_ReportFeature(true, new List<string>());   //Se asume que la Configuracion del Reporte es Perfecta/Valida

            ReportKey = reporteNode.Attributes["key"].Value;
            ReportPath = reporteNode.Attributes["path"].Value;
            ReportDownloadName = reporteNode.SelectSingleNode("DownloadedFile").Attributes["name"].Value;
            ReportExtension = reporteNode.SelectSingleNode("DownloadedFile").Attributes["ext"].Value;
            DbQuery = reporteNode.SelectSingleNode("Dataverse").SelectSingleNode("Query").InnerText;
            DbParameters = new List<SqlParameter>();

            XmlNodeList SqlParams = reporteNode.SelectNodes("Dataverse/Params/Param");
            if(SqlParams != null && SqlParams.Count > 0)
            {
                FeatureSqlParam.SetQuantity(SqlParams.Count);

                foreach (XmlNode parametroNode in SqlParams)
                {
                    // Obtener atributos del nodo "parametro"
                    string paramName = parametroNode.Attributes["name"].Value;
                    string paramType = parametroNode.Attributes["type"].Value;
                    string defaultValue = parametroNode.Attributes["defaultValue"].Value;


                    // Crear un objeto SqlParameter con el tipo obtenido
                    SqlParameter parametro = new SqlParameter(paramName, Generic_DataType.GetSqlDbType(paramType));
                    // Obtener el tipo del parámetro
                    if (Generic_DataType.typeDictionary.TryGetValue(paramType, out Type type))
                    {
                        parametro.Value = Convert.ChangeType(defaultValue, type);

                        // Verificar y establecer atributos adicionales si están presentes
                        if (parametroNode.Attributes["size"] != null)
                            parametro.Size = int.Parse(parametroNode.Attributes["size"].Value);

                        if (parametroNode.Attributes["scale"] != null)
                            parametro.Scale = byte.Parse(parametroNode.Attributes["scale"].Value);

                        if (parametroNode.Attributes["precision"] != null)
                            parametro.Precision = byte.Parse(parametroNode.Attributes["precision"].Value);

                        if (parametroNode.Attributes["direction"] != null)
                            parametro.Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), parametroNode.Attributes["direction"].Value, true);

                        DbParameters.Add(parametro);
                    }
                    else
                    {
                        // Manejar el caso en el que el tipo no esté en el diccionario
                        FeatureXml.SetResult(false);
                        FeatureXml.AddDetail("Tipo de Dato [" + paramType + "] del Parametro @" + paramName + " no admitido");
                    }                    
                }

                //Validacion de Parametros
                FeatureSqlParam.SetResult(DbParameters.Count == FeatureSqlParam.GetQuantity());
            }          
        }

        public bool CheckFeatureResult(int ReportFeature)
        {
            bool value = false;
            switch (ReportFeature)
            {
                case Generic_ReportFeature.FEATURE_SQL_PARAMS: value = FeatureSqlParam.GetResult(); break;
                case Generic_ReportFeature.FEATURE_CRYSTAL_PARAMS: value = FeatureCrystalParam.GetResult(); ; break;
                case Generic_ReportFeature.FEATURE_QUERY_RESULT: value = FeatureQuery.GetResult(); break;
                case Generic_ReportFeature.FEATURE_XML_REPORT: value = FeatureXml.GetResult(); break; 
            };
            return value;
        }

        public List<string> CheckFeatureDetails(int ReportFeature)
        {
            List<string> value = new List<string>();
            switch (ReportFeature)
            {
                case Generic_ReportFeature.FEATURE_SQL_PARAMS: value = FeatureSqlParam.GetDetails(); break;
                case Generic_ReportFeature.FEATURE_CRYSTAL_PARAMS: value = FeatureCrystalParam.GetDetails(); break;
                case Generic_ReportFeature.FEATURE_QUERY_RESULT: value = FeatureQuery.GetDetails(); break;
                case Generic_ReportFeature.FEATURE_XML_REPORT: value= FeatureXml.GetDetails(); break;
            }
            return value;
        }        

        /// <summary>
        /// Establece los valores de un Conjunto de Parametros SQL Esperados en el Reporte
        /// </summary>
        /// <param name="potential_values">Conjunto de Clave-Valor</param>
        public bool SetUpSqlParams(Dictionary<string, object> potential_values)
        {
            if(FeatureSqlParam.AnyRecord())
            {
                /*  Checkeo de Parametros SQL para consulta */
                if (potential_values.Count != DbParameters.Count)
                {   //  Comprobacion de longitud parametrica
                    FeatureSqlParam.AddDetail("Numero de Parámetros Sql Incorrectos");
                    FeatureSqlParam.SetResult(false);
                }
                else
                {
                    //  Comprobacion de nombres Identicos
                    int coincidencias = 0;
                    int numero_exito = DbParameters.Count;

                    foreach (var parameter in DbParameters)
                    {
                        if (potential_values.ContainsKey(parameter.ParameterName))
                            coincidencias++;
                    }
                    if (numero_exito != coincidencias)
                    {
                        FeatureSqlParam.AddDetail("Nombre de Parámetros Sql Incorrectos");
                        FeatureSqlParam.SetResult(false);
                    }
                }

                if (FeatureSqlParam.GetResult())
                {
                    try
                    {
                        // Asignar valores a los parámetros
                        foreach (var parameter in DbParameters)
                        {
                            if (potential_values.ContainsKey(parameter.ParameterName))
                            {
                                parameter.Value = potential_values[parameter.ParameterName];
                            }
                        }
                        FeatureSqlParam.AddDetail("OK - Parámetros Sql correctamente asignados.");
                        FeatureSqlParam.SetResult(true);
                    }
                    catch (Exception e)
                    {
                        FeatureSqlParam.AddDetail("Excepción en Asignación de Parámetros");
                        FeatureSqlParam.AddDetails(Generic_Response.CreateExceptionDetails(e));
                        FeatureSqlParam.SetResult(false);
                    }
                }                              
            }
            else
            {
                FeatureSqlParam.AddDetail("No se esperaban Parámetros SQL.");
                FeatureSqlParam.SetResult(potential_values.Count == 0);
            }            
            return FeatureSqlParam.GetResult();
        }

        /// <summary>
        /// Establece los valores de un Conjunto de Parametros Crystal Esperados en el Reporte
        /// </summary>
        /// <param name="potential_values">Conjunto de Clave-Valor</param>
        public bool SetUpCrystalParams(Dictionary<string, object> potential_values)
        {
            /*  Pendiente de Programar  */

            return FeatureCrystalParam.GetResult();
        }

        /// <summary>
        /// Enlaza al reporte la información de la base de datos
        /// </summary>
        /// <param name="adt_connector">Conector/Conexion de base de datos que alimentará el reporte.</param>
        /// <returns>True en caso de no generarse errores, False de otro modo.</returns>
        public bool LinkData(Generic_DBConnection adt_connector)
        {
            FeatureQuery.SetDetails(new List<string>());
            if (adt_connector.OpenConnection())
            {
                try
                {
                    FeatureQuery.SetData(adt_connector.ExecuteSelectQuery(DbQuery, DbParameters.ToArray()));
                    FeatureQuery.SetResult(true);
                    FeatureQuery.AddDetail("Consulta ejecutada con éxito");
                }
                catch (Exception e)
                {
                    FeatureQuery.SetResult(false);
                    FeatureQuery.AddDetails(Generic_Response.CreateExceptionDetails(e));
                }
                finally
                {
                    if (!adt_connector.CloseConnection())
                    {
                        FeatureQuery.AddDetail("Error al cerrar Conexión a Base de Datos.");
                    }
                }
            }
            else
            {
                FeatureQuery.SetResult(false);
                FeatureQuery.AddDetail("Conexion a Base de Datos no establecida.");
            }
            return FeatureQuery.GetResult();
        }

        /// <summary>
        /// Genera un objeto en memoria que se interpreta como un archivo PDF
        /// </summary>
        /// <returns></returns>
        public byte[] CreateReport(DataTable datasource, ExportFormatType formatFile)
        {
            ReportDocument report = new ReportDocument();
            string reportFilePath = System.Web.Hosting.HostingEnvironment.MapPath(ReportPath);
            report.Load(reportFilePath);
            report.SetDataSource(datasource);

            /*  Pendiente Parametricazion de Parametros Crystal */
            //report.SetParameterValue("crys_param_01", 8);
            //report.SetParameterValue("crys_param_01", 3.1416);
            //report.SetParameterValue("crys_param_01", "cadena");
            //report.SetParameterValue("crys_param_01", '#');
            //report.SetParameterValue("crys_param_01", true);
            /*  Pendiente Parametricazion de Parametros Crystal */

            Stream pdfStream = report.ExportToStream(formatFile);   // Genera el informe en memoria en formato PDF
            byte[] pdfBytes = new byte[pdfStream.Length];           // Convierte el flujo de memoria a un arreglo de bytes
            pdfStream.Read(pdfBytes, 0, (int)pdfStream.Length);

            return pdfBytes;
        }

        /// <summary>
        /// Genera un objeto en memoria de tipo archivo (Extension configurable)
        /// </summary>
        /// <param name="formatFile">Tipo de Formato de Salida del Archivo (Extension)</param>
        /// <returns></returns>
        public byte[] CreateReport(ExportFormatType formatFile)
        {
            ReportDocument report = new ReportDocument();
            string reportFilePath = System.Web.Hosting.HostingEnvironment.MapPath(ReportPath);
            report.Load(reportFilePath);
            report.SetDataSource(FeatureQuery.GetData());


            /*  Pendiente Parametricazion de Parametros Crystal */
            //report.SetParameterValue("crys_param_01", 8);
            //report.SetParameterValue("crys_param_01", 3.1416);
            //report.SetParameterValue("crys_param_01", "cadena");
            //report.SetParameterValue("crys_param_01", '#');
            //report.SetParameterValue("crys_param_01", true);
            /*  Pendiente Parametricazion de Parametros Crystal */

            Stream genericStream = report.ExportToStream(formatFile);       // Genera el informe en memoria en formato PDF
            byte[] fileBytes = new byte[genericStream.Length];              // Convierte el flujo de memoria a un arreglo de bytes
            genericStream.Read(fileBytes, 0, (int)genericStream.Length);

            return fileBytes;
        }

        /// <summary>
        /// Genera una respuesta hacia en navegador web el formato especificado
        /// </summary>
        /// <param name="MemoryFileObject">Archivo en Memoria</param>
        /// <returns></returns>
        public HttpResponseMessage WriteResponse(byte[] MemoryFileObject, string typeHeaderValue = "application/pdf")
        {
            HttpResponseMessage webResponse = new HttpResponseMessage(HttpStatusCode.OK);          // Crea una respuesta HTTP con el archivo PDF
            webResponse.Content = new ByteArrayContent(MemoryFileObject);
            webResponse.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            webResponse.Content.Headers.ContentDisposition.FileName = ReportDownloadName + "_" + DateTime.Now.ToString() + ReportExtension;
            webResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(typeHeaderValue);
            webResponse.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");        //Exponer Nombre de Archivo en Encabezado de Respuesta
            return webResponse;
        }
        
        /// <summary>
        /// Comprueba que el DataSet contenga al menos un registro
        /// </summary>
        /// <returns>True en caso de contar con al menos un registro, False de otro modo.</returns>
        public bool AnyRecord()
        {
            return FeatureQuery.AnyRecord();
        }
    }
}