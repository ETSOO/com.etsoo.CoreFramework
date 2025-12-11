using Json.Schema;

namespace com.etsoo.CoreFramework.Json
{
    /// <summary>
    /// Email template schema
    /// 邮件模板模式
    /// </summary>
    /// <see cref="Models.EmailTemplateDto"/>
    public static class EmailTemplateSchema
    {
        static EmailTemplateSchema()
        {
            FormatRegistry.Global.Register(new NameEmailFormat());
        }

        /// <summary>
        /// Create the schema
        /// 创建模式
        /// https://json-schema.org/draft/2020-12/json-schema-validation#name-defined-formats
        /// </summary>
        /// <returns>Schema</returns>
        public static JsonSchema Create()
        {
            var builder = new JsonSchemaBuilder()
                .Type(SchemaValueType.Object)
                .Properties(
                    ("subject", new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                    ),
                    ("template", new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                    ),
                    ("isRazor", new JsonSchemaBuilder()
                        .Type(SchemaValueType.Boolean)
                    ),
                    ("cc", new JsonSchemaBuilder()
                        .Type(SchemaValueType.Array)
                        .Items(
                            new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                                .Format("name-email")
                        )
                        .UniqueItems(true)
                    ),
                    ("bcc", new JsonSchemaBuilder()
                        .Type(SchemaValueType.Array)
                        .Items(
                            new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                                .Format("name-email")
                        )
                        .UniqueItems(true)
                    ),
                    ("successMessage", new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                    )
                )
                .Required("subject", "template")
            ;

            return builder.Build();
        }
    }
}
