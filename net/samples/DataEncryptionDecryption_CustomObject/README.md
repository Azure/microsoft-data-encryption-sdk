# Encrypt and decrypt a custom object

This sample:
- Generates a plaintext DEK and encrypts it with the specified KEK stored in Azure Key vault.
- Creates a custom serializer for a user-defined class.
- Encrypts an object of the user-defined class using the default encryption type and the customer serializer.
- Prints the plaintext object and the encrypted object.

## Setup sample

Provide `AzureKeyVaultKeyPath` in the Program file:

The URL to your key in Azure Key Vault. The URL should include a version identifier, for example: https://example-vault.vault.azure.net:443/ExampleKey/ee1a695119e343328af6edbbd8d22093. For information on how to create a key and obtain its URL (including a key version id) using Azure Portal, see [Add a key to Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/keys/quick-create-portal#add-a-key-to-key-vault).


## Run the sample

Once configuration is updated, run the sample with below command:

```
dotnet run
```
The program will authenticate user interactively to access Azure Key Vault specified above.
