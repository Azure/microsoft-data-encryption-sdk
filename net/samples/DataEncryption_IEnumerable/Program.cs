using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.Encryption.Cryptography;
using static Microsoft.Data.Encryption.Cryptography.CryptographyExtensions;
using Microsoft.Data.Encryption.AzureKeyVaultProvider;

namespace DataEncryption_IEnumerable {
    static class Program {
        /// <summary>
        /// TODO: Provide AKV configuration details here to run this sample.
        /// </summary>
        static class ClientConfiguration {
            public const string AzureKeyVaultKeyPath = "https://{KeyVaultName}.vault.azure.net/keys/{Key}/";
            public const string AzureTenantId = "{Azure_Key_Vault_Active_Directory_Tenant_Id}";
            public const string AzureClientId = "{Application_Client_ID}";
            public const string AzureClientSecret = "{Application_Client_Secret}";
        }

        // New Token Credential to authenticate to Azure
        public static readonly TokenCredential TokenCredential = new ClientSecretCredential (
            tenantId: ClientConfiguration.AzureTenantId,
            clientId: ClientConfiguration.AzureClientId,
            clientSecret: ClientConfiguration.AzureClientSecret
        );

        // Azure Key Vault provider that allows client applications to access a key encryption key stored in Microsoft Azure Key Vault.
        public static readonly EncryptionKeyStoreProvider azureKeyProvider = new AzureKeyVaultKeyStoreProvider (TokenCredential);

        // Represents the key encryption key that encrypts and decrypts the data encryption key
        public static readonly KeyEncryptionKey keyEncryptionKey = new KeyEncryptionKey ("KEK", ClientConfiguration.AzureKeyVaultKeyPath, azureKeyProvider);

        // Represents the encryption key that encrypts and decrypts the data items
        public static readonly ProtectedDataEncryptionKey encryptionKey = new ProtectedDataEncryptionKey ("DEK", keyEncryptionKey);

        public static void Main () {
            // Encrypt 25 numbers from an infinite stream and print them.
            GetIntStream ()
                .Take (25)
                .Encrypt (encryptionKey)
                .ToBase64String ()
                .ForEach (Console.WriteLine);
        }

        #region Helper Methods
        private static IEnumerable<int> GetIntStream () {
            Random random = new Random ();

            while (true) {
                yield return random.Next (0, 20);
            }
        }

        private static void ForEach<T> (this IEnumerable<T> source, Action<T> action) {
            foreach (T item in source) {
                action.Invoke (item);
            }
        }
        #endregion
    }
}
