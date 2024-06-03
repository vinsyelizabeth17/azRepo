using System;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;

class Program
{
    static async Task Main(string[] args)
    {
        // Set your Azure subscription ID
        string subscriptionId = "<your-subscription-id>";

        // Set the resource group name and location
        string resourceGroupName = "rg-az-tutorials";
        string location = "eastus";

        // Set the storage account name
        string storageAccountName = "newblobstorage123";

        // Authenticate
        var credential = new DefaultAzureCredential();
        var armClient = new ArmClient(credential);

        // Get the subscription
        var subscription = armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{subscriptionId}"));

        // Create a new resource group
        var resourceGroup = await subscription.GetResourceGroups().CreateOrUpdateAsync(
            WaitUntil.Completed,
            resourceGroupName,
            new ResourceGroupData(location)
        );

        Console.WriteLine($"Resource Group '{resourceGroupName}' created in '{location}'.");

        // Define the storage account parameters specifically for Blob storage
        var storageParameters = new StorageAccountCreateOrUpdateContent(
            new StorageSku(StorageSkuName.StandardLRS),
            StorageKind.BlobStorage, // Specify Blob storage
            location
        )
        {
            AccessTier = AccessTier.Hot // Set the access tier to Hot or Cool
        };

        // Create the storage account for Blob storage
        var storageAccount = await resourceGroup.Value.GetStorageAccounts().CreateOrUpdateAsync(
            WaitUntil.Completed,
            storageAccountName,
            storageParameters
        );

        Console.WriteLine($"Blob Storage Account '{storageAccountName}' created in Resource Group '{resourceGroupName}'.");
    }
}