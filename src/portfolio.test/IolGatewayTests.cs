﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using portfolio.infrastructure;
using portfolio.infrastructure.iol;
using portfolio.test.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.test;

[TestFixture]
internal class IolGatewayTests
{
    [Test]
    public async Task GetLedger_Deserialized_Ok()
    {
        var mockHttpMessageHandler = Substitute.ForPartsOf<EstadoCuentaOkMockHttpMessageHandler>();
        var httpClient = new HttpClient(mockHttpMessageHandler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var redisConfigMock = new RedisConfig
        {
            InstanceName = "Test",
            Host = "localhost",
            Port = 6379,
            User = "default",
            Password = "password",
            DefaultTtl = 3600
        };
        var redisOptionsMock = Substitute.For<IOptions<RedisConfig>>();
        redisOptionsMock.Value.Returns(redisConfigMock);

        var gateway = new IolGateway(httpClient, 
                                     Substitute.For<IDistributedCache>(),
                                     redisOptionsMock);

        var ledger = await gateway.GetLedger();

        Assert.Multiple(() =>
        {
            Assert.That(ledger, Is.Not.Null);
            Assert.That(ledger.Currencies.Count, Is.EqualTo(3));
        });
    }
}

public class EstadoCuentaOkMockHttpMessageHandler : MockHttpMessageHandler
{
    public override HttpResponseMessage MockSend(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var estadoCuentaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "iol_estadocuenta.json");
        var jsonString = File.ReadAllText(estadoCuentaFilePath);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonString, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
        return response;
    }
}