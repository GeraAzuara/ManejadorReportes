using System.Collections.Generic;
using System.Data;

namespace ManejadorReportes.Controllers._CustomLogic
{
    public class Generic_ReportFeature
    {
        /// <summary>
        /// Propiedad que indica el Resultado de la Operación/Calculo de la Característica
        /// </summary>
        protected bool Success { get;set; }

        /// <summary>
        /// Propiedad que representa el Listado de Detalles relacionadas con la Operacion/Calculo de la Característica
        /// </summary>
        protected List<string> Details { get;set; }

        /*  Constantes estaticas para referenciar valores calculados    */
        public const int FEATURE_CRYSTAL_PARAMS = 200;
        public const int FEATURE_SQL_PARAMS = 300;
        public const int FEATURE_QUERY_RESULT = 400;
        public const int FEATURE_XML_REPORT = 500;

        #region Constructores

        public Generic_ReportFeature()
        {
            Success = false;
            Details = new List<string>();  
        }
        public Generic_ReportFeature(bool success, List<string> details) { 
            Success = success;
            Details = details;
        }

        #endregion

        /// <summary>
        /// Asigna un listado de Detalles relacionados a la Operación/Calculo de la Característica
        /// </summary>
        /// <param name="details">Listado de Detalles</param>
        public void SetDetails(List<string> details) { 
            Details = details;
        }

        /// <summary>
        /// Asigna un Detalle relacionado a la Operación/Calculo de la Característica
        /// </summary>
        /// <param name="detail">Nuevo Detalle por añadir al listado</param>
        public void AddDetail(string detail)
        {
            Details.Add(detail);
        }

        /// <summary>
        /// Añade al Listado de Detalle otra serie de Detalles relacionados a la Operación/Calculo de la Característica
        /// </summary>
        /// <param name="details"></param>
        public void AddDetails(List<string> details)
        {
            Details.AddRange(details);
        }

        /// <summary>
        /// Devuelve el Listado de Detalles relacionados con la Operacion/Calculo de la Característica
        /// </summary>
        /// <returns></returns>
        public List<string> GetDetails()
        {
            return Details;
        }

        /// <summary>
        /// Establece el resultado de la Operación/Calculo de la Característica
        /// </summary>
        /// <param name="result">True en caso de éxito, False de otro modo</param>
        public void SetResult(bool result)
        {
            Success = result;
        }

        /// <summary>
        /// Devuelve el resultado de la Operacion/Resultado de la Caracteristica
        /// </summary>
        /// <returns></returns>
        public bool GetResult()
        {
            return Success;
        }
    }

    public class Generic_QueryFeature: Generic_ReportFeature { 

        /// <summary>
        /// Propiedad que representa el Conjunto de Datos del Reporte
        /// </summary>
        protected DataTable DataSet { get; set; }

        #region Constructores
        
        public Generic_QueryFeature(bool success, List<string> details, DataTable ds):base(success, details)
        {
            DataSet = ds;
        }

        #endregion

        /// <summary>
        /// Devuelve el Conjunto de Datos de la Característica
        /// </summary>
        /// <returns></returns>
        public DataTable GetData() {
            return DataSet;
        }

        /// <summary>
        /// Devuelve el Numero de Registros que existen en el Conjunto de Datos
        /// </summary>
        /// <returns></returns>
        public int GetRecordsNumber()
        {
            if(DataSet != null)
                return DataSet.Rows.Count;
            return 0;
        }

        /// <summary>
        /// Devuelve si el Conjunto de Datos presenta al menos un registro.
        /// </summary>
        /// <returns></returns>
        public bool AnyRecord()
        {
            return DataSet != null && DataSet.Rows.Count > 0;
        }

        /// <summary>
        /// Establece el Conjunto de Datos perteneciente a la Caractrística
        /// </summary>
        /// <param name="ds"></param>
        public void SetData(DataTable ds) {
            DataSet = ds;
        }
    }

    public class Generic_ParameterFeature : Generic_ReportFeature
    {
        /// <summary>
        /// Propiedad que indica la Cantidad de Parametros del Reporte
        /// </summary>
        protected int RecordsNumber { get; set; }

        #region Constructores

        public Generic_ParameterFeature(bool success, List<string> details, int recordsNum) : base(success, details)
        {
            RecordsNumber = recordsNum;
        }

        #endregion

        /// <summary>
        /// Establece el Número de Parametros del Reporte
        /// </summary>
        /// <param name="recordsNumber">Numero de Parametros Esperados en el Reporte</param>
        public void SetQuantity(int recordsNumber)
        {
            RecordsNumber = recordsNumber;
        }

        /// <summary>
        /// Devuelve el Número de Parametros del Reporte
        /// </summary>
        /// <returns>Numero Actual de Parametros Esperados en el Reporte</returns>
        public int GetQuantity() { return RecordsNumber; }

        /// <summary>
        /// Devuelve si el Reporte Espera al menos un parámetro
        /// </summary>
        /// <returns></returns>
        public bool AnyRecord()
        {
            return RecordsNumber > 0;
        }
    }
}
