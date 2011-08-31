﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Ninefold.API.Core;
using Ninefold.API.Storage.Messages;

namespace Ninefold.API.Storage.Commands
{
    public class GetUserMetadata : ICommand
    {
        readonly IStorageCommandBuilder _commandBuilder;
        readonly ICommandAuthenticator _authenticator;
        readonly string _secret;
        readonly string _userId;

        public GetUserMetadata(string userId, string secret, IStorageCommandBuilder commandBuilder, ICommandAuthenticator authenticator)
        {
            _commandBuilder = commandBuilder;
            _userId = userId;
            _secret = secret;
            _authenticator = authenticator;
        }

        public GetUserMetadataRequest Parameters { get; set; }

        public HttpWebRequest Prepare()
        {
            if (!Parameters.Resource.PathAndQuery.Contains("metadata/tags"))
            {
                Parameters.Resource = new Uri(Parameters.Resource, "?metadata/tags");
            }

            var request = _commandBuilder.GenerateRequest(Parameters, _userId, HttpMethod.GET);
            _authenticator.AuthenticateRequest(request, _secret);

            return request;
        }

        public ICommandResponse ParseResponse(WebResponse webResponse)
        {
            return new GetUserMetadataResponse
            {
                Policy = webResponse.Headers["x-emc-policy"],
                Tags = webResponse.Headers["x-emc-tags"],
                ListableTags = webResponse.Headers["x-emc-listable-tags"],
            };
        }
    }
}