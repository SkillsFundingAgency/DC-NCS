using Microsoft.EntityFrameworkCore.Design;

namespace ESFA.DC.NCS.EF.Console.DesignTime
{
    public class DataPluralizer : IPluralizer
    {
        public string Pluralize(string name)
        {
            return name.Pluralize() ?? name;
        }

        public string Singularize(string name)
        {
            return name.Singularize() ?? name;
        }
    }
}
