﻿using System.IO;
using System.Net;
using Ninefold.Core;
using Ninefold.Storage.Messages;

namespace Ninefold.Storage.Queries
{
    public class GetObject : ICommand
    {
        readonly IStorageCommandBuilder _commandBuilder;
        readonly IStorageCommandAuthenticator _authenticator;
        readonly string _secret;
        readonly string _userId;

        public GetObjectRequest Parameters { get; set; }

        public GetObject(string userId,
                                        string base64Secret, 
                                        IStorageCommandBuilder commandBuilder, 
                                        IStorageCommandAuthenticator authenticator)
        {
            _userId = userId;
            _authenticator = authenticator;
            _commandBuilder = commandBuilder;
            _secret = base64Secret;
        }

        public HttpWebRequest Prepare()
        {
            var request = _commandBuilder.GenerateRequest(Parameters, _userId, WebRequestMethods.Http.Get);

            if (Parameters.LowerRange > Parameters.UpperRange)
            {
                request.AddRange(Parameters.LowerRange);
            }

            if (Parameters.UpperRange > 0)
            {
                request.AddRange(Parameters.LowerRange, Parameters.UpperRange);
            }
            _authenticator.AuthenticateRequest(request, _secret);

            return request;
        }

        public ICommandResponse ParseResponse(WebResponse response)
        {
            var getResponse = new GetObjectResponse
                                  {
                                      GroupAcl = response.Headers["x-emc-groupacl"],
                                      UserAcl = response.Headers["x-emc-useracl"],
                                      Policy = response.Headers["x-emc-policy"],
                                      Metadata = response.Headers["x-emc-meta"],
                                      ListableTags = response.Headers["x-emc-listable-meta"]
                                  };
            
            var  responseStream = response.GetResponseStream();
            if ((responseStream != null) && (responseStream.CanRead))
            {
                var reader = new StreamReader(responseStream);
                getResponse.Content = reader.CurrentEncoding.GetBytes(reader.ReadToEnd());
            }        

            return getResponse;
        }
    }
}
