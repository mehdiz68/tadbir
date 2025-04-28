using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;

namespace ahmadi.Infrastructure.Report
{
    public static class Extensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> source, string tableName = null)
        {
            if (source == null) throw new ArgumentNullException("source");

            var properties = TypeDescriptor.GetProperties(typeof(T))
                .Cast<PropertyDescriptor>()
                .ToList();

            var result = new DataTable(tableName);
            result.BeginInit();

            foreach (var prop in properties)
            {
                result.Columns.Add(prop.Name, prop.PropertyType);
            }

            result.EndInit();
            result.BeginLoadData();

            foreach (T item in source)
            {
                object[] values = properties.Select(p => p.GetValue(item)).ToArray();
                result.Rows.Add(values);
            }

            result.EndLoadData();

            return result;
        }

        public static DataSet ToDataSet<T>(this IEnumerable<T> source, string dataSetName = null, string tableName = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(dataSetName)) dataSetName = "NewDataSet";

            var result = new DataSet(dataSetName);
            result.Tables.Add(source.ToDataTable(tableName));
            return result;
        }
    }

}