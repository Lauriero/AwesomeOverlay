using System;

namespace AwesomeOverlay.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DataBaseColumnAttribute : Attribute
    {
        public bool DeleteOnUnauthorize { get; private set; }
        public bool UseEncryption { get; private set; }
        public string ColumnName { get; private set; }

        /// <summary>
        /// Creates an attribute instance
        /// </summary>
        /// <param name="columnName">Name of the database column corresponding to current property</param>
        /// <param name="useEncryption">Specify whether an encryption should be user to put value into the database</param>
        /// <param name="deleteOnUnauthorize">Specify whether the value should be deleted from the database when user is unauthorized</param>
        public DataBaseColumnAttribute(string columnName, bool useEncryption = false, bool deleteOnUnauthorize = false)
        {
            ColumnName = columnName;
            UseEncryption = useEncryption;
            DeleteOnUnauthorize = deleteOnUnauthorize;
        }
    }
}
