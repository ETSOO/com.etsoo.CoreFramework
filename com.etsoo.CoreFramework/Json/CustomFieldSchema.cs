using Json.Schema;

namespace com.etsoo.CoreFramework.Json
{
    /// <summary>
    /// Custom field schema
    /// @see @etsoo/appscript 'CustomFieldData' for more details
    /// 自定义字段模式
    /// </summary>
    public static class CustomFieldSchema
    {
        /// <summary>
        /// Create the schema
        /// 创建模式
        /// </summary>
        /// <returns>Schema</returns>
        public static JsonSchema Create()
        {
            var builder = new JsonSchemaBuilder()
                .Type(SchemaValueType.Array)
                .Items(
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.Object)
                        .Properties(
                            ("type", new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                                .MinLength(1)
                                .MaxLength(50)
                            ),
                            ("name", new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                            ),
                            ("options", new JsonSchemaBuilder()
                                .Type(SchemaValueType.Array)
                                .Items(new JsonSchemaBuilder()
                                    .Type(SchemaValueType.Object)
                                    .Properties(
                                        ("id", new JsonSchemaBuilder()
                                            .Type(SchemaValueType.String | SchemaValueType.Number)
                                        ),
                                        ("name", new JsonSchemaBuilder()
                                            .Type(SchemaValueType.String)
                                        ),
                                        ("title", new JsonSchemaBuilder()
                                            .Type(SchemaValueType.String)
                                        ),
                                        ("label", new JsonSchemaBuilder()
                                            .Type(SchemaValueType.String)
                                        )
                                    )
                                    .Required("id")
                                    .OneOf(
                                        new JsonSchemaBuilder()
                                            .Required("name"),
                                        new JsonSchemaBuilder()
                                            .Required("title"),
                                        new JsonSchemaBuilder()
                                            .Required("label")
                                    )
                                )
                            ),
                            ("refs", new JsonSchemaBuilder()
                                .Type(SchemaValueType.Array)
                                .PrefixItems(new JsonSchemaBuilder().Type(SchemaValueType.String))
                                .Items(new JsonSchemaBuilder().Type(SchemaValueType.String | SchemaValueType.Number))
                            ),
                            ("space", new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                                .Enum("quater", "half", "half1", "full", "five", "seven")
                            ),
                            ("gridItemProps", new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                            ),
                            ("mainSlotProps", new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                            ),
                            ("label", new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                            ),
                            ("helperText", new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                            )
                        )
                        .Required("type")
                )
                .MinItems(1)
            ;

            return builder.Build();
        }
    }
}
