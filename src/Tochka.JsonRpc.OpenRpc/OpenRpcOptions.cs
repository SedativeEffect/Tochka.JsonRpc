﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Tochka.JsonRpc.OpenRpc.Models;

namespace Tochka.JsonRpc.OpenRpc;

public sealed class OpenRpcOptions
{
    public string DocumentPath { get; set; } = OpenRpcConstants.DefaultDocumentPath;
    public Dictionary<string, Info> Docs { get; set; } = new();
    public string DefaultServerName { get; set; } = OpenRpcConstants.DefaultServerName;
    public bool IgnoreObsoleteActions { get; set; }
    public Func<string, ApiDescription, bool> DocInclusionPredicate { get; set; } = static (_, _) => true;
}
