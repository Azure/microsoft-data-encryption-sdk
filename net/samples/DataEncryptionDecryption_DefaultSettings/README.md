# Encrypt and decrypt a DateTime using default settings

This sample:
- Generates a plaintext DEK and encrypts it with the specified KEK stored in Azure Key vault.
- Encrypts and decrypts the date value.
- Prints the plaintext value (before and after encryption) and the encrypted value.

## Setup sample

Provide below details in `ClientConfiguration` static class:

1. AzureKeyVaultKeyPath 

   The URL to your Key in Azure Key Vault. Example: "https://example-vault.vault.azure.net/keys/ExampleKey/", replacing "example-vault" with the name of the Azure Key Vault you created and "ExampleKey" if you used a different name for your key than the quickstart tutorials used.

2. AzureTenantId

   A GUID value found in the Azure portal in the overview section of Azure Active Directory.

3. AzureClientId

   The application ID you configured in  [Enable client application access]().

4. AzureClientSecret

    During  [Enable client application access](), one of the authentication options available was creating an application secret. Set this value to that secret.

## Run the sample

Once configuration is updated, run the sample with below command:

```
dotnet run
```