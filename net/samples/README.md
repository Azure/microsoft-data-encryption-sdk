# Code Samples

This folder contains samples that demonstrate how to use the Microsoft Data Encryption SDK for .NET. Each sample includes a README file that explains how to run and use the sample.

## Getting started

### Prerequisites

* An Azure account and subscription. If you don't have one, sign up for a [free trial](https://aka.ms/azurefree).
* [Visual Studio](https://aka.ms/getvs).
* [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
* Create a key named “ExampleKey” in Azure Key Vault by following one of the [Azure Key Vault quick start tutorials](https://docs.microsoft.com/azure/key-vault/keys/quick-create-portal).

### Enable access to your key

In order to access the key in Azure Key Vault and use it for encryption and decryption operations, you need to ensure the key has the appropriate access defined. You need to ensure:

1. You have firewall access to your Azure Key Vault.
1. Your access policy grants sufficient access to perform key operations in your key vault. The samples will use user-based access to read keys from the key vault. In order to do that, the user you use to log in to Azure Key Vault requires to following Key Permissions:
   1. Get
   1. List
   1. Create
   1. Unwrap Key
   1. Wrap Key
   1. Verify
   1. Sign

## In this section

#### [DataEncryption_IEnumerable](DataEncryption_IEnumerable)
Encrypt an enumerable list of numbers

#### [DataEncryptionDecryption_CustomObject](DataEncryptionDecryption_CustomObject)
Encrypt and decrypt a custom object

#### [DataEncryptionDecryption_CustomSettings](DataEncryptionDecryption_CustomSettings)
Encrypt and decrypt a Guid using custom settings

#### [DataEncryptionDecryption_DefaultSettings](DataEncryptionDecryption_DefaultSettings)
Encrypt and decrypt a DateTime using default settings

#### [DataEncryptionDecryption_PlainTextEncryptionKey](DataEncryptionDecryption_PlainTextEncryptionKey)
Encrypt and decrypt plain text

#### [FileEncryptionDecryption_ParquetFile](FileEncryptionDecryption_ParquetFile)
Encrypt and decrypt a parquet file
