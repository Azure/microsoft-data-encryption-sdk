using System;
using Azure.Identity;
using Microsoft.Data.Encryption.AzureKeyVaultProvider;
using Microsoft.Data.Encryption.Cryptography;

namespace DataEncryptionDecryption_AKVInteractiveAuthentication {
    public class Program {
        /// <summary>
        /// TODO: Provide AKV Key URL here to run this sample.
        /// </summary>
        class ClientConfiguration {
            public const string s_akvUrl = "https://{KeyVaultName}.vault.azure.net/keys/{Key}/";
        }

        // Azure Key Vault provider that allows client applications to access a key encryption key stored in Microsoft Azure Key Vault.
        public static readonly EncryptionKeyStoreProvider azureKeyProvider = new AzureKeyVaultKeyStoreProvider (new InteractiveBrowserCredential ());

        // Represents the key encryption key that encrypts and decrypts the data encryption key
        public static readonly KeyEncryptionKey keyEncryptionKey = new KeyEncryptionKey ("KEK", ClientConfiguration.s_akvUrl, azureKeyProvider);

        // Represents the encryption key that encrypts and decrypts the data items
        public static readonly ProtectedDataEncryptionKey encryptionKey = new ProtectedDataEncryptionKey ("DEK", keyEncryptionKey);

        public static void Main () {
            Console.WriteLine ("\n**** Original Value ****");
            // Declare value to be encrypted
            DateTime original = DateTime.Now;
            Console.WriteLine (original);

            Console.WriteLine ("\n**** Encrypted Value ****");
            // Encrypt value and convert to HEX string
            var encryptedBytes = original.Encrypt (encryptionKey);
            var encryptedHexString = encryptedBytes.ToHexString ();
            Console.WriteLine (encryptedHexString);

            Console.WriteLine ("\n**** Decrypted Value ****");
            // Decrypt value back to original
            var bytesToDecrypt = encryptedHexString.FromHexString ();
            var decryptedBytes = bytesToDecrypt.Decrypt<DateTime> (encryptionKey);
            Console.WriteLine (decryptedBytes);
        }
    }
}
