﻿// ****************************************************************
// Copyright (c) 2013 NUnit Software. All rights reserved.
// ****************************************************************

using System;

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace NUnit.VisualStudio.TestAdapter
{
    /// <summary>
    /// TestLogger wraps an IMessageLogger and adds various
    /// utility methods for sending messages. Since the
    /// IMessageLogger is only provided when the discovery
    /// and execution objects are called, we use two-phase
    /// construction. Until Initialize is called, the logger
    /// simply swallows all messages without sending them
    /// anywhere.
    /// </summary>
    public class TestLogger : IMessageLogger
    {
        private const string EXCEPTION_FORMAT = "Exception {0}, {1}";

        private IMessageLogger MessageLogger { get; set; }

        private int Verbosity { get; set; }

        public TestLogger(IMessageLogger messageLogger, int verbosity)
        {
            MessageLogger = messageLogger;
            Verbosity = verbosity;
        }

        public void AssemblyNotSupportedWarning(string sourceAssembly)
        {
            SendWarningMessage("Assembly not supported: " + sourceAssembly);
        }

        public void DependentAssemblyNotFoundWarning(string dependentAssembly, string sourceAssembly)
        {
            SendWarningMessage("Dependent Assembly " + dependentAssembly + " of " + sourceAssembly + " not found. Can be ignored if not a NUnit project.");
        }

        public void LoadingAssemblyFailedWarning(string dependentAssembly, string sourceAssembly)
        {
            SendWarningMessage("Assembly " + dependentAssembly + " loaded through " + sourceAssembly + " failed. Assembly is ignored. Correct deployment of dependencies if this is an error.");
        }

        public void NUnitLoadError(string sourceAssembly)
        {
            SendErrorMessage("NUnit failed to load " + sourceAssembly);
        }

        public void SendErrorMessage(string message)
        {
            SendMessage(TestMessageLevel.Error, message);
        }

        public void SendErrorMessage(string message, Exception ex)
        {
            switch (Verbosity)
            {
                case 0:
                    Type type = ex.GetType();
                    SendErrorMessage(string.Format(EXCEPTION_FORMAT, type, message));
                    SendErrorMessage(ex.Message);
                    break;
                default:
                    SendErrorMessage(message);
                    SendErrorMessage(ex.ToString());
                    break;
            }
        }

        public void SendWarningMessage(string message)
        {
            SendMessage(TestMessageLevel.Warning, message);
        }

        public void SendWarningMessage(string message,Exception ex)
        {
            switch (Verbosity)
            {
                case 0:
                    var type = ex.GetType();
                    SendWarningMessage(string.Format(EXCEPTION_FORMAT, type, message));
                    SendWarningMessage(ex.Message);
                    break;

                default:
                    SendWarningMessage(message);
                    SendWarningMessage(ex.ToString());
                    break;
            }

            SendMessage(TestMessageLevel.Warning, message);
        }

        public void SendInformationalMessage(string message)
        {
            SendMessage(TestMessageLevel.Informational, message);
        }

        public void SendDebugMessage(string message)
        {
#if DEBUG
            SendMessage(TestMessageLevel.Informational, message);
#endif
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            if (MessageLogger != null)
                MessageLogger.SendMessage(testMessageLevel, message);
        }
    }
}
