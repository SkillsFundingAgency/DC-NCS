using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IExcelService
    {
        Task SaveWorkbookAsync(Workbook workbook, string fileName, string container, CancellationToken cancellationToken);

        void WriteExcelRows<TModel>(Worksheet sheet, IEnumerable<TModel> rows, int firstRow) where TModel : class;
    }
}
