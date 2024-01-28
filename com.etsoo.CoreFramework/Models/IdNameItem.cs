﻿using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Id / name item
    /// 编号 / 名称项目
    /// </summary>
    public record IdNameItem
    {
        [JsonRequired]
        public int Id { get; init; }

        [JsonRequired]
        public required string Name { get; init; }
    }
}
