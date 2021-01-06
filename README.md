[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/dotnet/sqlclient/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/dt/Microsoft.Data.Encryption.Cryptography?label=Microsoft.Data.Encryption.Cryptography&style=flat-square&color=green)](https://www.nuget.org/packages/Microsoft.Data.Encryption.Cryptography)
[![Nuget](https://img.shields.io/nuget/dt/Microsoft.Data.Encryption.FileEncryption?label=Microsoft.Data.Encryption.FileEncryption&style=flat-square&color=green)](https://www.nuget.org/packages/Microsoft.Data.Encryption.FileEncryption)
[![Nuget](https://img.shields.io/nuget/dt/Microsoft.Data.Encryption.AzureKeyVaultProvider?label=Microsoft.Data.Encryption.AzureKeyVaultProvider&style=flat-square&color=green)](https://www.nuget.org/packages/Microsoft.Data.Encryption.AzureKeyVaultProvider)

# Microsoft Data Encryption SDK (Preview)

The Microsoft Data Encryption SDK provides encryption support to applications. It allows developers to implement column- or field-level encryption for data stored in various data stores, including Azure data services.

## The modules of the SDK

The SDK consists of the following modules:

* Cryptography
* File Encryption
* Azure Key Vault Provider

### Cryptography

The Cryptography module provides APIs for objects like encryption keys, serializers, key store provider interfaces, and associated caches.

The module implements cryptographic operations using a two-level key hierarchy composed of:

* Data Encryption Keys (DEKs) - symmetric keys that encrypt data.
* Key Encryption Keys (KEKs) - asymmetric keys that encrypt DEKs.

The Cryptography module uses cryptographic algorithms that are fully compatible with [Always Encrypted](https://docs.microsoft.com/sql/relational-databases/security/encryption/always-encrypted-database-engine) in Azure SQL. The data encryption algorithm is AEAD_AES_256_CBC_HMAC_SHA_256 that is derived from the IETF specification draft at [https://tools.ietf.org/html/draft-mcgrew-aead-aes-cbc-hmac-sha2-05](https://tools.ietf.org/html/draft-mcgrew-aead-aes-cbc-hmac-sha2-05). The key encryption algorithm is RSA with OEAP padding. For more information, see [Always Encrypted cryptography](https://docs.microsoft.com/sql/relational-databases/security/encryption/always-encrypted-cryptography?view=sql-server-ver15).

### File Encryption

This module supports encrypting and decrypting columns in parquet files. It implements storing encryption metadata in the JSON format within the parquet file metadata footer.

### Azure Key Vault Provider

This module implements the key store provider interface using Azure Key Vault. It allows you to use KEKs stored in Azure Key Vault.

## Using the SDK for data protection in Azure

The below diagram shows an example of an Azure application and illustrates the benefits of the Microsoft Data Encryption SDK.

The sample application is a data analytics pipeline that loads data stored in parquet files on premises to Azure Data Lake. Subsequently, Azure Data Factory jobs transform and transport the data to other Azure data services, including Synapse SQL, CosmosDB and Azure SQL. Then, custom applications hosted in Azure VMs or Azure App Services, and Spark jobs in Synapse Analytics access and process the data.

![Microsoft Encryption SDK Overview Diagram](/media/SdkDiagram.jpg)

In such applications, the SDK helps ensure:

* Sensitive data gets encrypted at ingestion to Azure.
  * For example, you can achieve that by building an on-premises application that that encrypts sensitive data columns in Parquet files using the File Encryption module of the SDK, before uploading the files to Azure Data Lake.

* Sensitive data stays encrypted when it flows to other Azure data services.
  * Administrators in your organization as well Microsoft operators of the data services you are using, will not be able to access the data in plaintext, assuming your safeguard the KEKs and DEKs, your are using.
  * You can use Azure Data Factory jobs to move encrypted data to other Azure data services, while keeping the data encrypted all the time.
* Control over which Azure compute services and which users can decrypt and access the data in plaintext.
  * A service or an application, using the SDK, can access and perform computations on plaintext data only if you explicitly grant service’s/application’s identity in Azure Active Directly access to the key encryption keys in Azure Key Vault.
  * For example, if you want an Azure Data Factory job to perform transformations of encrypted sensitive data stored in Azure Data Lake, or if you want a Spark job or an application in an Azure VM or Azure App Services to compute encrypted data stored in Synapse SQL, you need to integrate the SDK into the job and grant it access to the key encryption keys in Azure Key Vault.
* Interoperability with Always Encrypted in Azure SQL
  * You can load the data, encrypted using the SDK, to databases columns configured with Always Encrypted in Azure SQL.
  * If you ensure the database columns are encrypted in the same way as the data you are inserting, your Azure Data Factory and Spark jobs as well as your applications can transparently decrypt the data stored in the database and run confidential queries using Azure SQL client drivers. This is because the SDK is compatible with Always Encrypted.
  * Similarly, you can move the data from a database in Azure SQL to another service or application without decrypting it. Then, you can use the SDK to decrypt the data where it needs to be decrypted, rather than in an application that directly talks to the database.

## Supported Platforms

The SDK currently supports the following platforms:

* .NET – for more information, see [Microsoft Data Encryption SDK for .NET](./net/)

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## License

These samples and templates are all licensed under the MIT license. See the LICENSE.txt file in the root.
