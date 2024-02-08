﻿// Copyright 2021-2024 Deepgram .NET SDK contributors. All Rights Reserved.
// Use of this source code is governed by a MIT license that can be found in the LICENSE file.
// SPDX-License-Identifier: MIT

namespace Deepgram.Encapsulations;

internal class HttpClientWrapper(HttpClient HttpClient)
{
    internal Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return HttpClient.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// this needs to be able to be set inbetween calls, as
    /// the longer the recording and more options selected the longer the transcription takes
    /// and the default timeout can be quickly exceeded
    /// </summary>
    internal void SetTimeOut(TimeSpan timeSpan)
    {
        HttpClient.Timeout = timeSpan;
    }
}