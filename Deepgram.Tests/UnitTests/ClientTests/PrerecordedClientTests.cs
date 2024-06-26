﻿// Copyright 2021-2024 Deepgram .NET SDK contributors. All Rights Reserved.
// Use of this source code is governed by a MIT license that can be found in the LICENSE file.
// SPDX-License-Identifier: MIT

using Deepgram.Models.Authenticate.v1;
using Deepgram.Models.PreRecorded.v1;
using Deepgram.Clients.PreRecorded.v1;

namespace Deepgram.Tests.UnitTests.ClientTests;

public class PreRecordedClientTests
{
    DeepgramClientOptions _options;
    string _apiKey;

    [SetUp]
    public void Setup()
    {
        _options = new DeepgramClientOptions();
        _apiKey = new Faker().Random.Guid().ToString();
    }

    [Test]
    public async Task TranscribeUrl_Should_Call_PostAsync_Returning_SyncResponse()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<SyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        prerecordedSchema.CallBack = null;
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = new AutoFaker<UrlSource>().Generate();
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        

        prerecordedClient.When(x => x.PostAsync<SyncResponse>(Arg.Any<string>(), Arg.Any<StringContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<StringContent>()).Returns(expectedResponse);

        // Act
        var result = await prerecordedClient.TranscribeUrl(source, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<StringContent>());

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<SyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeUrl_Should_Throw_ArgumentException_If_CallBack_Not_Null()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<SyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = new AutoFaker<UrlSource>().Generate();
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<SyncResponse>(Arg.Any<string>(), Arg.Any<StringContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<StringContent>()).Returns(expectedResponse);

        // Act and Assert
        await prerecordedClient.Invoking(y => y.TranscribeUrl(source, prerecordedSchema))
            .Should().ThrowAsync<ArgumentException>();

        await prerecordedClient.DidNotReceive().PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<StringContent>());
    }

    [Test]
    public async Task TranscribeUrlCallBack_Should_Call_PostAsync_Returning_SyncResponse_With_CallBack_Parameter()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var source = new AutoFaker<UrlSource>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        // prerecordedSchema is not null here as we first need to get the querystring with the callBack included
        var stringedQuery = QueryParameterUtil.GetParameters(prerecordedSchema);
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<StringContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedQuery}", Arg.Any<StringContent>()).Returns(expectedResponse);
        var callBackParameter = prerecordedSchema.CallBack;
        //before we act to test this call with the callBack parameter and not the callBack property we need to null the callBack property
        prerecordedSchema.CallBack = null;


        // Act
        var result = await prerecordedClient.TranscribeUrlCallBack(source, callBackParameter, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedQuery}", Arg.Any<StringContent>());
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AsyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeUrlCallBack_Should_Call_PostAsync_Returning_SyncResponse_With_CallBack_Property()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var source = new AutoFaker<UrlSource>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        var stringedQuery = QueryParameterUtil.GetParameters(prerecordedSchema);
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<StringContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedQuery}", Arg.Any<StringContent>()).Returns(expectedResponse);

        // Act
        var result = await prerecordedClient.TranscribeUrlCallBack(source, null, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedQuery}", Arg.Any<StringContent>());
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AsyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeUrlCallBack_Should_Throw_ArgumentException_With_CallBack_Property_And_CallBack_Parameter_Set()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var source = new AutoFaker<UrlSource>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        var stringedQuery = QueryParameterUtil.GetParameters(prerecordedSchema);

        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);
        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<StringContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedQuery}", Arg.Any<StringContent>()).Returns(expectedResponse);
        var callBackParameter = prerecordedSchema.CallBack;

        // Act Assert
        await prerecordedClient.Invoking(y => y.TranscribeUrlCallBack(source, callBackParameter, prerecordedSchema))
            .Should().ThrowAsync<ArgumentException>();

        await prerecordedClient.DidNotReceive().PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedQuery}", Arg.Any<StringContent>());
    }

    [Test]
    public async Task TranscribeUrlCallBack_Should_Throw_ArgumentException_With_No_CallBack_Set()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var source = new AutoFaker<UrlSource>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        prerecordedSchema.CallBack = null;
        var stringedQuery = QueryParameterUtil.GetParameters(prerecordedSchema);

        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);
        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<StringContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedQuery}", Arg.Any<StringContent>()).Returns(expectedResponse);


        // Act Assert
        await prerecordedClient.Invoking(y => y.TranscribeUrlCallBack(source, null, prerecordedSchema))
            .Should().ThrowAsync<ArgumentException>();

        await prerecordedClient.DidNotReceive().PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedQuery}", Arg.Any<StringContent>());
    }

    [Test]
    public async Task TranscribeFile_With_Stream_Should_Call_PostAsync_Returning_SyncResponse()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<SyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        prerecordedSchema.CallBack = null;
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = GetFakeStream(GetFakeByteArray());
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<SyncResponse>(Arg.Any<string>(), Arg.Any<HttpContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>()).Returns(expectedResponse);

        // Act
        var result = await prerecordedClient.TranscribeFile(source, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>());
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<SyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeFile_With_Bytes_Should_Call_PostAsync_Returning_SyncResponse()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<SyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        prerecordedSchema.CallBack = null;
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = GetFakeByteArray();
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<SyncResponse>(Arg.Any<string>(), Arg.Any<HttpContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>()).Returns(expectedResponse);

        // Act
        var result = await prerecordedClient.TranscribeFile(source, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<SyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>());
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<SyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeFileCallBack_With_Stream_With_CallBack_Property_Should_Call_PostAsync_Returning_SyncResponse()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = GetFakeStream(GetFakeByteArray());
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<HttpContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>()).Returns(expectedResponse);

        // Act
        var result = await prerecordedClient.TranscribeFileCallBack(source, null, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>());
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AsyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeFileCallBack_With_Bytes_With_CallBack_Property_Should_Call_PostAsync_Returning_SyncResponse()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = GetFakeByteArray();
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<HttpContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>()).Returns(expectedResponse);

        // Act
        var result = await prerecordedClient.TranscribeFileCallBack(source, null, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>());

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AsyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeFileCallBack_With_Stream_With_CallBack_Parameter_Should_Call_PostAsync_Returning_SyncResponse()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        // prerecordedSchema is not null here as we first need to get the querystring with the callBack included
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = GetFakeStream(GetFakeByteArray());
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<HttpContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>()).Returns(expectedResponse);

        var callBack = prerecordedSchema.CallBack;

        //before we act to test this call with the callBack parameter and not the callBack property we need to null the callBack property
        prerecordedSchema.CallBack = null;

        // Act
        var result = await prerecordedClient.TranscribeFileCallBack(source, callBack, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>());
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AsyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeFileCallBack_With_Bytes_With_CallBack_Parameter_Should_Call_PostAsync_Returning_SyncResponse()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        // prerecordedSchema is not null here as we first need to get the querystring with the callBack included
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = GetFakeByteArray();

        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);
        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<HttpContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>()).Returns(expectedResponse);

        var callBack = prerecordedSchema.CallBack;

        //before we act to test this call with the callBack parameter and not the callBack property we need to null the callBack property
        prerecordedSchema.CallBack = null;

        // Act
        var result = await prerecordedClient.TranscribeFileCallBack(source, callBack, prerecordedSchema);

        // Assert
        await prerecordedClient.Received().PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>());
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AsyncResponse>();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Test]
    public async Task TranscribeFileCallBack_With_Stream_Throw_ArgumentException_With_CallBack_Property_And_CallBack_Parameter_Set()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = GetFakeStream(GetFakeByteArray());
        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);

        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<HttpContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>()).Returns(expectedResponse);
        var callBack = prerecordedSchema.CallBack;


        // Act  Assert
        await prerecordedClient.Invoking(y => y.TranscribeFileCallBack(source, callBack, prerecordedSchema))
           .Should().ThrowAsync<ArgumentException>();

        await prerecordedClient.DidNotReceive().PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<StringContent>());


    }

    [Test]
    public async Task TranscribeFileCallBack_With_Bytes_Should_Throw_ArgumentException_With_No_CallBack_Set()
    {
        //Arrange 
        var expectedResponse = new AutoFaker<AsyncResponse>().Generate();
        var prerecordedSchema = new AutoFaker<PrerecordedSchema>().Generate();
        // prerecordedSchema is not null here as we first need to get the querystring with the callBack included
        var stringedOptions = QueryParameterUtil.GetParameters(prerecordedSchema);
        var source = GetFakeByteArray();

        var httpClient = MockHttpClient.CreateHttpClientWithResult(expectedResponse);
        var prerecordedClient = Substitute.For<PreRecordedClient>(_apiKey, _options);
        
        prerecordedClient.When(x => x.PostAsync<AsyncResponse>(Arg.Any<string>(), Arg.Any<HttpContent>())).DoNotCallBase();
        prerecordedClient.PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<HttpContent>()).Returns(expectedResponse);
        prerecordedSchema.CallBack = null;

        // Act  Assert
        await prerecordedClient.Invoking(y => y.TranscribeFileCallBack(source, null, prerecordedSchema))
           .Should().ThrowAsync<ArgumentException>();

        await prerecordedClient.DidNotReceive().PostAsync<AsyncResponse>($"{UriSegments.LISTEN}?{stringedOptions}", Arg.Any<StringContent>());
    }

    private static Stream GetFakeStream(byte[] source)
    {
        var stream = new MemoryStream();
        stream.Write(source, 0, source.Length);
        return stream;
    }

    private static byte[] GetFakeByteArray() => new Faker().Random.Bytes(10);

}
