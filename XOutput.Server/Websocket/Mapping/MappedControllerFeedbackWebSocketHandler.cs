﻿using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using XOutput.DependencyInjection;
using XOutput.Mapping.Controller;

namespace XOutput.Websocket.Mapping
{
    class MappedControllerFeedbackWebSocketHandler : IWebSocketHandler
    {
        private static readonly Regex PathRegex = new Regex($"{IWebSocketHandler.WebsocketBasePath}/MappedController/([-A-Za-z0-9]+)");
        private readonly MappedControllers emulatedControllers;

        [ResolverMethod]
        public MappedControllerFeedbackWebSocketHandler(MappedControllers emulatedControllers)
        {
            this.emulatedControllers = emulatedControllers;
        }

        public bool CanHandle(HttpContext context)
        {
            return PathRegex.IsMatch(context.Request.Path.Value);
        }

        public IMessageHandler CreateHandler(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string id = PathRegex.Match(context.Request.Path.Value).Groups[1].Value;
            var emulatedController = emulatedControllers.Find(id);
            return new MappedControllerFeedbackHandler(closeFunction, sendFunction, emulatedController);
        }
    }
}
