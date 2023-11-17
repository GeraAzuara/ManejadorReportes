using System;
using System.Collections.Generic;
using System.Data;

namespace ManejadorReportes.Controllers._CustomLogic
{
    public static class Generic_DataType
    {
        // Diccionario de tipos SQL y sus equivalentes en C#
        public static readonly Dictionary<string, Type> typeDictionary = new Dictionary<string, Type>
        {
            { "bigint", typeof(long) },
            { "binary", typeof(byte[]) },
            { "bit", typeof(bool) },
            { "char", typeof(string) },
            { "date", typeof(DateTime) },
            { "datetime", typeof(DateTime) },
            { "datetime2", typeof(DateTime) },
            { "datetimeoffset", typeof(DateTimeOffset) },
            { "decimal", typeof(decimal) },
            { "float", typeof(double) },
            { "image", typeof(byte[]) },
            { "int", typeof(int) },
            { "money", typeof(decimal) },
            { "nchar", typeof(string) },
            { "ntext", typeof(string) },
            { "numeric", typeof(decimal) },
            { "nvarchar", typeof(string) },
            { "real", typeof(float) },
            { "smalldatetime", typeof(DateTime) },
            { "smallint", typeof(short) },
            { "smallmoney", typeof(decimal) },
            { "text", typeof(string) },
            { "time", typeof(TimeSpan) },
            { "timestamp", typeof(byte[]) },
            { "tinyint", typeof(byte) },
            { "uniqueidentifier", typeof(Guid) },
            { "varbinary", typeof(byte[]) },
            { "varchar", typeof(string) },
            // Otros tipos según tus necesidades
        };

        /// <summary>
        /// Método auxiliar para convertir tipos de datos de cadena a SqlDbType 
        /// </summary>
        /// <param name="type">Cadena de texto que indica el Tipo de Dato SQL a manejar en el Parámetro</param>
        /// <returns></returns>
        public static SqlDbType GetSqlDbType(string type)
        {
            switch (type.ToLower())
            {
                case "bigint":
                    return SqlDbType.BigInt;
                case "binary":
                    return SqlDbType.Binary;
                case "bit":
                    return SqlDbType.Bit;
                case "char":
                    return SqlDbType.Char;
                case "date":
                    return SqlDbType.Date;
                case "datetime":
                    return SqlDbType.DateTime;
                case "datetime2":
                    return SqlDbType.DateTime2;
                case "datetimeoffset":
                    return SqlDbType.DateTimeOffset;
                case "decimal":
                    return SqlDbType.Decimal;
                case "float":
                    return SqlDbType.Float;
                case "image":
                    return SqlDbType.Image;
                case "int":
                    return SqlDbType.Int;
                case "money":
                    return SqlDbType.Money;
                case "nchar":
                    return SqlDbType.NChar;
                case "ntext":
                    return SqlDbType.NText;
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "real":
                    return SqlDbType.Real;
                case "smalldatetime":
                    return SqlDbType.SmallDateTime;
                case "smallint":
                    return SqlDbType.SmallInt;
                case "smallmoney":
                    return SqlDbType.SmallMoney;
                case "text":
                    return SqlDbType.Text;
                case "time":
                    return SqlDbType.Time;
                case "timestamp":
                    return SqlDbType.Timestamp;
                case "tinyint":
                    return SqlDbType.TinyInt;
                case "uniqueidentifier":
                    return SqlDbType.UniqueIdentifier;
                case "varbinary":
                    return SqlDbType.VarBinary;
                case "varchar":
                    return SqlDbType.VarChar;
                case "variant":
                    return SqlDbType.Variant;
                case "xml":
                    return SqlDbType.Xml;
                default:
                    throw new ArgumentException($"Tipo de dato no compatible: {type}");
            }
        }

    }
}
