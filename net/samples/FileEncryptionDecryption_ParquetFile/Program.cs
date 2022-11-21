using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.Encryption.AzureKeyVaultProvider;
using Microsoft.Data.Encryption.Cryptography;
using Microsoft.Data.Encryption.Cryptography.Serializers;
using Microsoft.Data.Encryption.FileEncryption;

namespace FileEncryptionDecryption_ParquetFile
{
    public class Program
    {
        /// <summary>
        /// TODO: Provide AKV Key URL here to run this sample.
        /// </summary>
        public const string AzureKeyVaultKeyPath = "https://yourvault.vault.azure.net:443/ExampleKey/ee1a695119e343328af6edbbd8d22093";

        // New Token Credential to authenticate to Azure interactively.
        public static readonly TokenCredential TokenCredential = new InteractiveBrowserCredential();

        // Azure Key Vault provider that allows client applications to access a key encryption key is stored in Microsoft Azure Key Vault.
        public static readonly EncryptionKeyStoreProvider azureKeyProvider = new AzureKeyVaultKeyStoreProvider(TokenCredential);

        // Represents the key encryption key that encrypts and decrypts the data encryption key
        public static readonly KeyEncryptionKey keyEncryptionKey = new("KEK", AzureKeyVaultKeyPath, azureKeyProvider);

        // Represents the encryption key that encrypts and decrypts the data items
        public static readonly ProtectedDataEncryptionKey encryptionKey = new("DEK", keyEncryptionKey);

        public static void Main()
        {
            // open input and output file streams
            using Stream inputFileForEncryption = File.OpenRead(".\\resources\\userdata1.parquet");
            using Stream encryptedFile = File.OpenWrite(".\\resources\\encryptedUserData.parquet");
            PerformEncryption(inputFile: inputFileForEncryption, outputFile: encryptedFile);
            encryptedFile.Close();

            using Stream inputFileForDecryption = File.OpenRead(".\\resources\\encryptedUserData.parquet");
            using Stream decryptedFile = File.OpenWrite(".\\resources\\decryptedUserdata.parquet");
            PerformDecryption(inputFile: inputFileForDecryption, outputFile: decryptedFile);
        }

        private static void PerformEncryption(Stream inputFile, Stream outputFile)
        {
            // Create reader
            using ParquetFileReader reader = new(inputFile);

            // Copy source settings as target settings
            List<FileEncryptionSettings> writerSettings = reader.FileEncryptionSettings
                .Select(s => Copy(s))
                .ToList();

            // Modify a few column settings
            writerSettings[0] = new FileEncryptionSettings<DateTimeOffset?>(encryptionKey, SqlSerializerFactory.Default.GetDefaultSerializer<DateTimeOffset?>());
            writerSettings[3] = new FileEncryptionSettings<string>(encryptionKey, EncryptionType.Deterministic, new SqlVarCharSerializer(size: 255));
            writerSettings[10] = new FileEncryptionSettings<double?>(encryptionKey, StandardSerializerFactory.Default.GetDefaultSerializer<double?>());

            // Create and pass the target settings to the writer
            using ParquetFileWriter writer = new(outputFile, writerSettings);

            // Process the file
            ColumnarCryptographer cryptographer = new(reader, writer);
            cryptographer.Transform();

            Console.WriteLine($"Parquet File encrypted successfully. Verify output file contains encrypted data.");
        }

        private static void PerformDecryption(Stream inputFile, Stream outputFile)
        {
            // Create reader
            using ParquetFileReader reader = new(file: inputFile,
                encryptionKeyStoreProviders: new Dictionary<string, EncryptionKeyStoreProvider> 
                { 
                    [azureKeyProvider.ProviderName] = azureKeyProvider 
                }
            );

            // Copy source settings as target settings
            var writerSettings = reader.FileEncryptionSettings
                .Select(s => Copy(s))
                .ToList();

            var targetColumnTypes = reader.FileEncryptionSettings
                .Select(s => s.GetSerializer().GetType())
                .ToList();

            // Modify a few column settings
            writerSettings[0] = new FileEncryptionSettings<DateTimeOffset?>(encryptionKey, EncryptionType.Plaintext, SqlSerializerFactory.Default.GetDefaultSerializer<DateTimeOffset?>());
            writerSettings[3] = new FileEncryptionSettings<string>(encryptionKey, EncryptionType.Plaintext, SqlSerializerFactory.Default.GetDefaultSerializer<string?>());
            writerSettings[10] = new FileEncryptionSettings<double?>(encryptionKey, EncryptionType.Plaintext, StandardSerializerFactory.Default.GetDefaultSerializer<double?>());

            // Create and pass the target settings to the writer
            using ParquetFileWriter writer = new(outputFile, writerSettings);

            // Process the file
            ColumnarCryptographer cryptographer = new(reader, writer);
            cryptographer.Transform();

            Console.WriteLine($"Parquet File decrypted successfully. Verify output file contains the original file data.");
        }

        public static FileEncryptionSettings Copy(FileEncryptionSettings encryptionSettings)
        {
            Type genericType = encryptionSettings.GetType().GenericTypeArguments[0];
            Type settingsType = typeof(FileEncryptionSettings<>).MakeGenericType(genericType);
            return (FileEncryptionSettings)Activator.CreateInstance(
                settingsType,
                new object[] {
                    encryptionSettings.DataEncryptionKey,
                        encryptionSettings.EncryptionType,
                        encryptionSettings.GetSerializer ()
                }
            );
        }
    }
}
