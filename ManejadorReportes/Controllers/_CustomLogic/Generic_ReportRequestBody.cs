using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManejadorReportes.Controllers._CustomLogic
{
    public class Generic_ReportRequestBody
    {
        /// <summary>
        /// Clave única con el que se identifica el archivo de Reporte.
        /// </summary>
        [Required(ErrorMessage = "El campo KeyReport es obligatorio.")]
        [Description("Clave única con el que se identifica el archivo de Reporte.")]
        public string KeyReport { get; set; }


        /// <summary>
        /// Clave única con la que se identifica la conexion de base de datos que utilizará un reporte.
        /// </summary>
        [Required(ErrorMessage = "El campo KeyDbConnector es obligatorio.")]
        [Description("Clave única con la que se identifica la conexion de base de datos que utilizará un reporte.")]
        public string KeyDbConnector { get; set; }

        /// <summary>
        /// Colección de Parametros SQL que puede necesitar un reporte
        /// </summary>
        [Required(ErrorMessage = "El campo SqlParams es obligatorio.")]
        [Description("Colección de Parametros SQL que puede necesitar un reporte")]
        public Dictionary<string, object> SqlParams { get; set; }

        /// <summary>
        /// Colección de Parametros Crystal que puede necesitar un reporte
        /// </summary>
        [Required(ErrorMessage = "El campo CrystalParams es obligatorio.")]
        [Description("Colección de Parametros Crystal que puede necesitar un reporte")]
        public Dictionary<string, object> CrystalParams { get; set; }
    }
}
