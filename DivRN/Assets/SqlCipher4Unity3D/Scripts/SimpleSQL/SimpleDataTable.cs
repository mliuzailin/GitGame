﻿using System.Collections.Generic;
using System.Collections;

namespace SimpleSQL
{
    /// <summary>
    /// This class stores data retrieved from a SQLite database. It is a light-weight equivalent to 
    /// .NET's System.Data.DataTable that is compatible with the free versions of iOS and Android as 
    /// well as the Playmaker scripts.
    /// </summary>
    public class SimpleDataTable
    {
        /// <summary>
        /// The list of rows in this table
        /// </summary>
        public List<SimpleDataRow> rows = new List<SimpleDataRow>();

        /// <summary>
        /// The list of columns in this table
        /// </summary>
        public List<SimpleDataColumn> columns = new List<SimpleDataColumn>();

        /// <summary>
        /// Creates a new row in the table. It is important to use this function instead of directly
        /// adding to the row list because it sets up the field structure for the row.
        /// </summary>
        /// <returns></returns>
        public SimpleDataRow NewRow()
        {
            SimpleDataRow dr = new SimpleDataRow(this);
            rows.Add(dr);

            foreach (SimpleDataColumn column in columns)
            {
                dr.fields.Add(new object());
            }

            return dr;
        }
    }

    /// <summary>
    /// A simple column class for the SimpleDataTable
    /// </summary>
    public class SimpleDataColumn
    {
        /// <summary>
        /// Name of the column
        /// </summary>
        public string name;
    }

    /// <summary>
    /// A simple row class for the SimpleDataTable
    /// </summary>
    public class SimpleDataRow
    {
        private SimpleDataTable _dt;

        /// <summary>
        /// Returns a field value by its index. The index is the order that the
        /// field is listed in the SQL statement, starting at zero.
        /// </summary>
        /// <param name="fieldIndex">The numeric index of the field</param>
        /// <returns>The value of the field</returns>
        public object this[int fieldIndex] 
        { 
            get 
            { 
                if (fieldIndex >= 0 && fieldIndex < fields.Count) 
                { 
                    return fields[fieldIndex]; 
                } 
                else 
                { 
                    return null; 
                } 
            }
            set
            {
                if (fieldIndex >= 0 && fieldIndex < fields.Count)
                {
                    fields[fieldIndex] = value;
                }
            }
        }

        /// <summary>
        /// Returns a field value by its name.
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <returns>The value of the field</returns>
        public object this[string fieldName]
        {
            get
            {
                if (_dt == null || !string.IsNullOrEmpty(fieldName))
                {
                    for (int fieldIndex = 0; fieldIndex < _dt.columns.Count; fieldIndex++)
                    {
                        if (_dt.columns[fieldIndex].name == fieldName)
                        {
                            return fields[fieldIndex];
                        }
                    }
                }

                return null;
            }
            set
            {
                if (_dt == null || !string.IsNullOrEmpty(fieldName))
                {
                    for (int fieldIndex = 0; fieldIndex < _dt.columns.Count; fieldIndex++)
                    {
                        if (_dt.columns[fieldIndex].name == fieldName)
                        {
                            fields[fieldIndex] = value;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// List of fields stored in this row.
        /// </summary>
        public List<object> fields = new List<object>();

        /// <summary>
        /// The row initializer is passed the SimpleDataTable so that
        /// we can reference the column structure.
        /// </summary>
        /// <param name="dt"></param>
        public SimpleDataRow(SimpleDataTable dt)
        {
            _dt = dt;
        }
    }

}