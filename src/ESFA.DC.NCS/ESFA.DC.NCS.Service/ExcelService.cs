using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using Autofac.Features.AttributeFilters;
using ESFA.DC.FileService.Interface;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service
{
    public class ExcelService : IExcelService
    {
        private readonly IFileService _fileService;

        public ExcelService([KeyFilter(PersistenceStorageKeys.DctAzureStorage)] IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task SaveWorkbookAsync(Workbook workbook, string fileName, string container, CancellationToken cancellationToken)
        {
            using (Stream ms = await _fileService.OpenWriteStreamAsync(fileName, container, cancellationToken))
            {
                workbook.Save(ms, SaveFormat.Xlsx);
            }
        }

        public void WriteExcelRows<TModel>(Worksheet sheet, IEnumerable<TModel> rows, string[] columns, int firstRow)
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
