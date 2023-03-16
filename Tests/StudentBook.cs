using com.etsoo.Database;
using com.etsoo.SourceGenerators.Attributes;
using Microsoft.Data.SqlClient.Server;
using System.Data;

namespace Tests
{
    public record Book : ISqlServerDataRecord
    {
        public static string TypeName => "et_book";

        public required string Name { get; init; }
        public required decimal Price { get; init; }

        public static SqlDataRecord Create()
        {
            return new SqlDataRecord(
                new SqlMetaData("Name", SqlDbType.NVarChar, 128),
                new SqlMetaData("Price", SqlDbType.Money)
            );
        }

        public void SetValues(SqlDataRecord sdr)
        {
            sdr.SetString(0, Name);
            sdr.SetDecimal(1, Price);
        }
    }

    [AutoToParameters]
    public partial record Student
    {
        [Property(TypeName = "json")]
        public IEnumerable<Book>? JsonBooks { get; init; }

        public IEnumerable<Book>? Books { get; init; }
    }
}
