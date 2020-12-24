using System;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.Encryption.AzureKeyVaultProvider;
using Microsoft.Data.Encryption.Cryptography;
using Microsoft.Data.Encryption.Cryptography.Serializers;

namespace DataEncryptionDecryption_CustomSettings {
    public class Program {
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
            // Declare custom settings
            var encryptionSettings = new EncryptionSettings<Guid> (
                dataEncryptionKey: encryptionKey,
                encryptionType: EncryptionType.Deterministic,
                serializer: StandardSerializerFactory.Default.GetDefaultSerializer<Guid> ()
            );

            Console.WriteLine ("**** Original Value ****");
            // Declare value to be encrypted
            var original = Guid.NewGuid ();
            Console.WriteLine (original);

            Console.WriteLine ("\n**** Encrypted Value ****");
            // Encrypt value and convert to HEX string
            var encryptedBytes = original.Encrypt<Guid> (encryptionSettings);
            var encryptedHexString = encryptedBytes.ToHexString ();
            Console.WriteLine (encryptedHexString);

            Console.WriteLine ("\n**** Decrypted Value ****");
            // Decrypt value back to original
            var bytesToDecrypt = encryptedHexString.FromHexString ();
            var decryptedBytes = bytesToDecrypt.Decrypt<Guid> (encryptionSettings); //Type can be inferred here
            Console.WriteLine (decryptedBytes);
        }
    }
}
