﻿using System;
using System.Linq;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.Encryption.AzureKeyVaultProvider;
using Microsoft.Data.Encryption.Cryptography;
using Microsoft.Data.Encryption.Cryptography.Serializers;

using static System.Text.Encoding;

namespace DataEncryptionDecryption_CustomObject {
    public class Program {
        /// <summary>
        /// TODO: Provide AKV Key URL here to run this sample.
        /// </summary>
        public const string AzureKeyVaultKeyPath = "https://yourvault.vault.azure.net:443/ExampleKey/ee1a695119e343328af6edbbd8d22093";

        // New Token Credential to authenticate to Azure interactively.
        public static readonly TokenCredential TokenCredential = new InteractiveBrowserCredential();

        // Azure Key Vault provider that allows client applications to access a key encryption key stored in Microsoft Azure Key Vault.
        public static readonly EncryptionKeyStoreProvider azureKeyProvider = new AzureKeyVaultKeyStoreProvider (TokenCredential);

        // Represents the key encryption key that encrypts and decrypts the data encryption key
        public static readonly KeyEncryptionKey keyEncryptionKey = new KeyEncryptionKey ("KEK", AzureKeyVaultKeyPath, azureKeyProvider);

        // Represents the encryption key that encrypts and decrypts the data items
        public static readonly ProtectedDataEncryptionKey encryptionKey = new ProtectedDataEncryptionKey ("DEK", keyEncryptionKey);

        public static void Main () {
            // Declare custom settings with custom serializer
            var encryptionSettings = new EncryptionSettings<Person> (
                dataEncryptionKey: encryptionKey,
                encryptionType: EncryptionType.Randomized,
                serializer: new PersonSerializer ()
            );

            Console.WriteLine ("**** Original Value ****");
            // Declare value to be encrypted
            var original = new Person (30, "Bob", "Brown");
            Console.WriteLine (original);

            Console.WriteLine ("\n**** Encrypted Value ****");
            // Encrypt value and convert to Base64 string
            var encryptedBytes = original.Encrypt (encryptionSettings);
            var encryptedHexString = encryptedBytes.ToBase64String ();
            Console.WriteLine (encryptedHexString);

            Console.WriteLine ("\n**** Decrypted Value ****");
            // Decrypt value back to original
            var bytesToDecrypt = encryptedHexString.FromBase64String ();
            var decryptedBytes = bytesToDecrypt.Decrypt (encryptionSettings);
            Console.WriteLine (decryptedBytes);

            // Set custom Person serializer as default for Person Type
            Type type = typeof (Person);
            StandardSerializerFactory.Default.RegisterSerializer (type, new PersonSerializer (), overrideDefault : true);

            var person = new Person (27, "Alice", "Anderson");
            var bytes = person.Encrypt (encryptionKey);
        }

        #region Custom Object Person
        public sealed class Person {
            public int Age { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public Person (int age, string firstName, string lastName) {
                Age = age;
                FirstName = firstName;
                LastName = lastName;
            }

            public override string ToString () => $"Person[Age: {Age}, First Name: {FirstName}, Last Name: {LastName}]";
        }

        #endregion

        #region Custom Serializer
        public class PersonSerializer : Serializer<Person> {
            private const int AgeSize = sizeof (int);
            private const int StringLengthSize = sizeof (int);
            private const int BytesPerCharacter = 2;
            private const int FirstNameIndex = AgeSize + StringLengthSize;

            public override string Identifier => "Person";

            public override Person Deserialize (byte[] bytes) {
                int age = BitConverter.ToInt32 (bytes.Take (AgeSize).ToArray ());
                int firstNameLength = BytesPerCharacter * BitConverter.ToInt32 (bytes.Skip (AgeSize).Take (StringLengthSize).ToArray ());
                string firstName = Unicode.GetString (bytes.Skip (FirstNameIndex).Take (firstNameLength).ToArray ());
                int lastNameSizeIndex = FirstNameIndex + firstNameLength;
                int lastNameLength = BytesPerCharacter * BitConverter.ToInt32 (bytes.Skip (lastNameSizeIndex).Take (StringLengthSize).ToArray ());
                int lastNameIndex = lastNameSizeIndex + StringLengthSize;
                string lastName = Unicode.GetString (bytes.Skip (lastNameIndex).Take (lastNameLength).ToArray ());

                return new Person (age, firstName, lastName);
            }

            public override byte[] Serialize (Person value) {
                byte[] ageBytes = BitConverter.GetBytes (value.Age);
                byte[] firstNameLengthBytes = BitConverter.GetBytes (value.FirstName.Length);
                byte[] firstNameBytes = Unicode.GetBytes (value.FirstName);
                byte[] lastNameLengthBytes = BitConverter.GetBytes (value.LastName.Length);
                byte[] lastNameBytes = Unicode.GetBytes (value.LastName);

                return ageBytes
                    .Concat (firstNameLengthBytes)
                    .Concat (firstNameBytes)
                    .Concat (lastNameLengthBytes)
                    .Concat (lastNameBytes)
                    .ToArray ();
            }
        }
        #endregion
    }
}
