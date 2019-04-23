using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Cells;
using CsvHelper;
using CsvHelper.Configuration;

namespace ESFA.DC.NCS.ReportingService
{
    public abstract class AbstractReportBuilder
    {
        protected string ReportFileName;

        /// <summary>
        /// Returns the formatted file name.
        /// </summary>
        /// <param name="submissionDateTime">DateTime to use in the file name.</param>
        /// <returns>The file name.</returns>
        protected string GetFilename(DateTime submissionDateTime)
        {
            return $"{ReportFileName}-{submissionDateTime:yyyyMMdd}-{submissionDateTime:T}";
        }

        /// <summary>
        /// Builds a CSV report using the specified mapper as the list of column names.
        /// </summary>
        /// <typeparam name="TMapper">The mapper.</typeparam>
        /// <typeparam name="TModel">The model.</typeparam>
        /// <param name="csvWriter">The memory stream to write to.</param>
        /// <param name="records">The records to persist.</param>
        /// <param name="mapperOverride">Optional override of the TMapper, for example, when needing to specify constructor parameters.</param>
        protected void WriteCsvRecords<TMapper, TModel>(CsvWriter csvWriter, IEnumerable<TModel> records, TMapper mapperOverride = null)
            where TMapper : ClassMap
            where TModel : class
        {
            if (mapperOverride == null)
            {
                csvWriter.Configuration.RegisterClassMap<TMapper>();
            }
            else
            {
                csvWriter.Configuration.RegisterClassMap(mapperOverride);
            }

            csvWriter.WriteRecords(records);

            csvWriter.Configuration.UnregisterClassMap();
        }

        /// <summary>
        /// Inserts row data for a given Excel worksheet.
        /// </summary>
        /// <typeparam name="TModel">The data model.</typeparam>
        /// <param name="sheet">The worksheet to write to.</param>
        /// <param name="rows">The records to persist.</param>
        /// <param name="columns">The columns to use from the given model.</param>
        /// <param name="firstRow">The row to begin inserting data at.</param>
        protected void WriteExcelRows<TModel>(Worksheet sheet, IEnumerable<TModel> rows, string[] columns, int firstRow)
            where TModel : class
        {
            sheet.Cells.ImportCustomObjects(
                rows.ToList(),
                columns,
                false,
                firstRow,
                0,
                rows.Count(),
                true,
                "dd/mm/yyyy",
                false);
        }
    }
}
