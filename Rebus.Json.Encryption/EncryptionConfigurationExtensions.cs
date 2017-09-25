﻿using Newtonsoft.Json.Encryption;
using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Pipeline.Send;

namespace Rebus.Json.Encryption
{
    /// <summary>
    /// Configuration extensions for enabling encrypted message bodies
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Configures Rebus to encrypt messages.
        /// </summary>
        public static void EnableJsonEncryption(this OptionsConfigurer configurer, EncryptionFactory encryptionFactory, EncryptStateBuilder encryptStateBuilder, DecryptStateBuilder decryptStateBuilder)
        {
            Guard.AgainstNull(nameof(configurer), configurer);
            Guard.AgainstNull(nameof(encryptionFactory), encryptionFactory);
            Guard.AgainstNull(nameof(encryptStateBuilder), encryptStateBuilder);
            Guard.AgainstNull(nameof(decryptStateBuilder), decryptStateBuilder);
            configurer.Decorate<IPipeline>(c =>
            {
                var injector = new PipelineStepInjector(c.Get<IPipeline>());

                var decryptStep = new DecryptStep(encryptionFactory, decryptStateBuilder);
                injector.OnReceive(decryptStep, PipelineRelativePosition.Before, typeof(DeserializeIncomingMessageStep));

                var encryptStep = new EncryptStep(encryptionFactory, encryptStateBuilder);
                injector.OnSend(encryptStep, PipelineRelativePosition.Before, typeof(SerializeOutgoingMessageStep));

                return injector;
            });
        }
    }
}