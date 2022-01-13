﻿using ConsoleAppWithJsonRpcClient.RandomOrg.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tochka.JsonRpc.Client;
using Tochka.JsonRpc.Client.Services;
using Tochka.JsonRpc.Common.Serializers;

namespace ConsoleAppWithJsonRpcClient.RandomOrg;

public class RandomOrgClient : JsonRpcClientBase, IRandomOrgClient
{
    private readonly string apiKey;

    public RandomOrgClient(HttpClient client, IOptions<RandomOrgOptions> config, IJsonRpcSerializer serializer, HeaderJsonRpcSerializer headerJsonRpcSerializer, IJsonRpcIdGenerator jsonRpcIdGenerator, ILogger<RandomOrgClient> log)
        : base(client, serializer, headerJsonRpcSerializer, config.Value, jsonRpcIdGenerator, log)
    {
        if (string.IsNullOrWhiteSpace(config.Value?.Token))
        {
            throw new ArgumentNullException(nameof(config.Value.Token));
        }
        apiKey = config.Value.Token;
    }

    public async Task<GetRandomIntsResponse?> GetRandomInts(int quantityInts, int min, int max, bool replacement, CancellationToken cancellationToken)
    {
        var request = new GetRandomIntsRequest
        {
            ApiKey = apiKey,
            QuantityInts = quantityInts,
            Min = min,
            Max = max,
            Replacement = replacement
        };

        var rpcResult = await SendRequest("generateIntegers", request, cancellationToken);
        return rpcResult.GetResponseOrThrow<GetRandomIntsResponse>();
    }
}