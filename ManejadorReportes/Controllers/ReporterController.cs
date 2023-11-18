using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ManejadorReportes.Controllers._CustomLogic;
using ManejadorReportes.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ManejadorReportes.Controllers
{
    public class ReporterController : ApiController
    {
        private readonly Generic_DispatcherConfiguration _dispatcher;
        public ReporterController()
        {
            _dispatcher = new Generic_DispatcherConfiguration();
        }


        // GET: api/Reporter
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Reporter/5
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Genera un reporte PDF basandose en una plantilla de reporte y su configuración respectiva.
        /// </summary>
        /// <param name="model">Conjunto de configuraciones exigibles del Reporte</param>
        /// <returns>Arhcivo PDF o Cadena JSON detallando las Excepciones/Errores</returns>
        // POST: api/Reporter
        [AllowAnonymous]
        [HttpPost]
        [ActionName("CreatePDFReport")]
        public HttpResponseMessage Post([FromBody] Generic_ReportRequestBody model)
        {
            //  Inicializacion de variables para generacion de respuesta API
            HttpResponseMessage http_response;
            Generic_Response adt_response = new Generic_Response();

            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                adt_response = Generic_Response.Response_BadRequest("Modelo inválido", "El modelo no cumple los requerimientos de la API.", errors);
            }
            else
            {
                Generic_Report adt_report = _dispatcher.SetUpReport(model.KeyReport);
                Generic_DBConnection adt_connector = _dispatcher.SetUpConnector(model.KeyDbConector);

                if (adt_report == null || adt_connector == null)
                {
                    adt_response = Generic_Response.Response_NotFound("Reporte o Conector no localizado(s).", "Revise archivos de configuracion en servidor.");
                }
                else
                {
                    //  Asociar Valores a Parametros
                    adt_report.SetUpSqlParams(model.SqlParams);
                    adt_report.SetUpCrystalParams(model.CrystalParams); /* PENDIENTE DE REVISAR ESTA PROGRAMACION  */

                    if (adt_report.CheckFeatureResult(Generic_ReportFeature.FEATURE_SQL_PARAMS))
                    {
                        adt_report.LinkData(adt_connector);
                        if (adt_report.CheckFeatureResult(Generic_ReportFeature.FEATURE_QUERY_RESULT))
                        {
                            if (adt_report.AnyRecord())
                            {
                                try
                                {
                                    return adt_report.WriteResponse(adt_report.CreateReport(ExportFormatType.PortableDocFormat), "application/pdf");
                                }
                                catch (Exception e)
                                {
                                    adt_response = Generic_Response.Response_InternalServerError("Excepción en Generación de Reporte.", e.Message, Generic_Response.CreateExceptionDetails(e));
                                }
                            }
                            else
                            {   //  Consulta sin resultados
                                adt_response = Generic_Response.Response_NoContent("Consulta del Reporte sin resultados.", "La consulta SQL no devuelve registros.");
                            }
                        }
                        else
                        {
                            string msj = "Excepción al ejecutar consulta SQL.";
                            string detail = "Excepcion al intentar realizar la consulta SQL o al establecer conexion con la Base de Datos.";
                            List<string> details = adt_report.CheckFeatureDetails(Generic_ReportFeature.FEATURE_QUERY_RESULT);
                            adt_response = Generic_Response.Response_InternalServerError(msj, detail, details);
                        }
                    }
                    else
                    {
                        string msj = string.Empty, detalle = string.Empty;
                        List<string> errores = adt_report.CheckFeatureDetails(Generic_ReportFeature.FEATURE_SQL_PARAMS);

                        if (errores.Any(e => e.Contains("Excepción")))
                        {
                            msj = "Excepción en Asociación de Parámetros SQL por valor.";
                            detalle = "Error al establecer el Valor a uno o más Parámetros SQL.";
                            adt_response = Generic_Response.Response_InternalServerError(msj, detalle, errores);
                        }
                        else
                        {
                            msj = "Error en Asociacion de Parámetros por Nombre o Número";
                            detalle = "El número de Parámetros SQL o su(s) nombre(s) no tiene(n) correspondencia.";
                            adt_response = Generic_Response.Response_BadRequest(msj, detalle, errores);
                        }
                    }
                }
            }

            //  Generar Respuesta de la Solicitud (Casos de Fallo/Error/Excepcion)
            http_response = new HttpResponseMessage(adt_response.HttpStatus);
            http_response.Content = new StringContent(JsonConvert.SerializeObject(adt_response), Encoding.UTF8, "application/json");
            return http_response;
        }

        // PUT: api/Reporter/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Reporter/5
        public void Delete(int id)
        {
        }
    }
}
