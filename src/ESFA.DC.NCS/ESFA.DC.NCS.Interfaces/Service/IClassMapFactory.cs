using CsvHelper.Configuration;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IClassMapFactory<out TClassMap, TModel>
        where TClassMap : ClassMap<TModel>
    {
        TClassMap Build(INcsJobContextMessage ncsJobContextMessage);
    }
}
