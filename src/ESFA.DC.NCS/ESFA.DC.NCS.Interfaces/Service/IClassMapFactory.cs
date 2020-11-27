using CsvHelper.Configuration;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IClassMapFactory<TModel>
    {
        ClassMap<TModel> Build(INcsJobContextMessage ncsJobContextMessage);
    }
}
