using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            csvWriter.WriteHeader<TModel>();

            csvWriter.NextRecord();
            csvWriter.WriteRecords(records);

            csvWriter.Configuration.UnregisterClassMap();
        }

        /// <summary>
        /// Writes the data to the zip file with the specified filename.
        /// </summary>
        /// <param name="archive">Archive to write to.</param>
        /// <param name="filename">Filename to use in zip file.</param>
        /// <param name="data">Data to write.</param>
        /// <returns>Awaitable task.</returns>
        protected async Task WriteZipEntry(ZipArchive archive, string filename, string data)
        {
            if (archive == null)
            {
                return;
            }

            ZipArchiveEntry archivedFile = archive.CreateEntry(filename, CompressionLevel.Optimal);
            using (StreamWriter sw = new StreamWriter(archivedFile.Open()))
            {
                await sw.WriteAsync(data);
            }
        }
    }
}
