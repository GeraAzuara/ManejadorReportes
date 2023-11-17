using System;
using System.Collections.Generic;
using System.Net;

namespace ManejadorReportes.Controllers._CustomLogic
{
    public class Generic_Response
    {
        public bool Success { get; set; }
        public HttpStatusCode HttpStatus { get; set; }

        public string Message { get; set; }
        public string Detail { get; set; }
        public List<string> Details { get; set; }


        #region Contructores

        public Generic_Response()
        {
            Success = false;
            HttpStatus = HttpStatusCode.NotAcceptable;

            Message = "No se ha evaluado la Solicitud";
            Detail = "Valores iniciales del objeto Responser";
            Details = null;
        }
        public Generic_Response(bool success, HttpStatusCode httpStatus, string message, string detail, List<string> details = null)
        {
            Success = success;
            HttpStatus = httpStatus;

            Message = message;
            Detail = detail;
            Details = details;
        }

        #endregion

        #region Funcionalidades Basicas

        /// <summary>
        /// Genera un Objeto de Respuesta con Estatus OK (Exito)
        /// </summary>
        /// <param name="message">Mensage Generico de la Solicitud</param>
        /// <param name="detail">detail de la Solicitud</param>
        /// <param name="details">Listado de detalles de la Solicitud</param>
        /// <returns>CustomReportResponse</returns>
        public static Generic_Response Response_OK(string message, string detail, List<string> details = null)
        {
            return new Generic_Response(true, HttpStatusCode.OK, message, detail, details);
        }
        /// <summary>
        /// Genera un Objeto de Respuesta con Estatus NoContent (Sin Contenido)
        /// </summary>
        /// <param name="message">Mensage Generico de la Solicitud</param>
        /// <param name="detail">detail de la Solicitud</param>
        /// <param name="details">Listado de detalles de la Solicitud</param>
        /// <returns>CustomReportResponse</returns>
        public static Generic_Response Response_NoContent(string message, string detail, List<string> details = null)
        {
            return new Generic_Response(true, HttpStatusCode.NoContent, message, detail, details);
        }
        /// <summary>
        /// Genera un Objeto de Respuesta con Estatus NotFound (No Encontrado)
        /// </summary>
        /// <param name="message">Mensage Generico de la Solicitud</param>
        /// <param name="detail">detail de la Solicitud</param>
        /// <param name="details">Listado de detalles de la Solicitud</param>
        /// <returns>CustomReportResponse</returns>
        public static Generic_Response Response_NotFound(string message, string detail, List<string> details = null)
        {
            return new Generic_Response(false, HttpStatusCode.NotFound, message, detail, details);
        }
        /// <summary>
        /// Genera un Objeto de Respuesta con Estatus BadRequest (Solicitud Errada)
        /// </summary>
        /// <param name="message">Mensage Generico de la Solicitud</param>
        /// <param name="detail">detail de la Solicitud</param>
        /// <param name="details">Listado de detalles de la Solicitud</param>
        /// <returns>CustomReportResponse</returns>
        public static Generic_Response Response_BadRequest(string message, string detail, List<string> details = null)
        {
            return new Generic_Response(false, HttpStatusCode.BadRequest, message, detail, details);
        }
        /// <summary>
        /// Genera un Objeto de Respuesta con Estatus InternalServerError (Excepcion ocurrida en Servidor)
        /// </summary>
        /// <param name="message">Mensage Generico de la Solicitud</param>
        /// <param name="detail">detail de la Solicitud</param>
        /// <param name="details">Listado de detalles de la Solicitud</param>
        /// <returns>CustomReportResponse</returns>
        public static Generic_Response Response_InternalServerError(string message, string detail, List<string> details = null)
        {
            return new Generic_Response(false, HttpStatusCode.InternalServerError, message, detail, details);
        }
        /// <summary>
        /// Genera un Objeto de Respuesta con Estatus Unauthorized (Problema de Autenticación/Sesión)
        /// </summary>
        /// <param name="message">Mensage Generico de la Solicitud</param>
        /// <param name="detail">detail de la Solicitud</param>
        /// <param name="details">Listado de detalles de la Solicitud</param>
        /// <returns>CustomReportResponse</returns>
        public static Generic_Response Response_Unauthorized(string message, string detail, List<string> details = null)
        {
            return new Generic_Response(false, HttpStatusCode.Unauthorized, message, detail, details);
        }
        /// <summary>
        /// Genera un Objeto de Respuesta con Estatus Forbidden (Problema de Privilegios/Permisos)
        /// </summary>
        /// <param name="message">Mensage Generico de la Solicitud</param>
        /// <param name="detail">detail de la Solicitud</param>
        /// <param name="details">Listado de detalles de la Solicitud</param>
        /// <returns>CustomReportResponse</returns>
        public static Generic_Response Response_Forbidden(string message, string detail, List<string> details = null)
        {
            return new Generic_Response(false, HttpStatusCode.Forbidden, message, detail, details);
        }

        #endregion

        #region Funcionalidades Extra

        /// <summary>
        /// Construye un listado de errores con las Excepciones del objeto Exception
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static List<string> CreateExceptionDetails(Exception e)
        {
            List<string> details = new List<string>
            {
                e.Message
            };

            if (e.InnerException != null)
            {
                details.Add(e.InnerException.Message);
            }

            return details;
        }

        #endregion
    }
}