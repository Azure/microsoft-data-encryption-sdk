# Authenticate to Azure Key Vault interactively

This sample:
- Authenticates to Azure Key Vault using Interactive authentication.
- Generates a plaintext DEK and encrypts it with the specified KEK stored in Azure Key vault.
- Encrypts and decrypts `DateTime` data using the default serializers and the default encryption type.

## Setup sample

Provide below details in `ClientConfiguration` static class:

1. AzureKeyVaultKeyPath 

   The URL to your Key in Azure Key Vault. Example: "https://example-vault.vault.azure.net/keys/ExampleKey/", replacing "example-vault" with the name of the Azure Key Vault you created and "ExampleKey" if you used a different name for your key than the quickstart tutorials used.

## Run the sample

Once configuration is updated, run the sample with below command:

```
dotnet run
```
