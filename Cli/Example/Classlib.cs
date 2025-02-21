using System;
using CalqFramework.Cli.Serialization; // for CliName

namespace CloudProviderCLI {
    // Compute-specific result records
    public record StartInstanceResult(string Message, int StatusCode);
    public record StopInstanceResult(string Message, int StatusCode);
    public record ListInstancesResult(string Message, string[] Instances);
    public record ComputeStatusResult(string Message, int AvailableInstances);
    public record ResizeResourcesResult(string Message, int NewSize);

    // Storage-specific result records
    public record UploadFileResult(string Message, string FilePath);
    public record DownloadFileResult(string Message, string FileName);
    public record DeleteFileResult(string Message, string FileName);
    public record ListAccountsResult(string Message, int AccountCount);
    public record StorageStatusResult(string Message, int Capacity);

    // Network-specific result records
    public record CreateEndpointResult(string Message, string EndpointName);
    public record DeleteEndpointResult(string Message, string EndpointName);
    public record ListEndpointsResult(string Message, int EndpointCount);
    public record NetworkStatusResult(string Message, bool IsConnected);
    public record TestSpeedResult(string Message, int Bandwidth);

    /// <summary>Base class for all CLI submodules.</summary>
    public abstract class SubmoduleBase {
        /// <summary>API Key used to auth the command.</summary>
        public string? ApiKey { get; set; } = System.IO.File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "EXAMPLE_CLOUD_PROVIDER_API_KEY.txt"))
            ? System.IO.File.ReadAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "EXAMPLE_CLOUD_PROVIDER_API_KEY.txt"))
            : null;
    }

    /// <summary>Everything should just work even if any class or its contents are refactored.</summary>
    public class RootModule : SubmoduleBase {
        /// <summary>Permanently saves api key into EXAMPLE_CLOUD_PROVIDER_API_KEY.txt in the user dir.</summary>
        [CliName("SetApiKey")]
        [CliName("a")]
        public void SetApiKey(string apiKey) => System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "EXAMPLE_CLOUD_PROVIDER_API_KEY.txt"), apiKey);
        /// <summary>Removes EXAMPLE_CLOUD_PROVIDER_API_KEY.txt</summary>
        public void RemoveApiKey() => System.IO.File.Delete(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "EXAMPLE_CLOUD_PROVIDER_API_KEY.txt"));

        /// <summary>Compute submodule.</summary>
        public ComputeModule Compute { get; set; } = new ComputeModule();
        /// <summary>Storage submodule.</summary>
        public StorageModule Storage { get; set; } = new StorageModule();
        /// <summary>Network submodule.</summary>
        public NetworkModule Network { get; set; } = new NetworkModule();
    }

    /// <summary>Abstract compute submodule.</summary>
    public abstract class AbstractComputeSubmodule : SubmoduleBase {
        /// <summary>Maximum number of compute instances.</summary>
        public int MaxInstances { get; set; } = 10;
        /// <summary>Default compute region.</summary>
        public string ComputeRegion { get; set; } = "us-west";
        /// <summary>Indicates if compute is optimized.</summary>
        public bool IsOptimized { get; set; } = true;
        /// <summary>Compute operating system type.</summary>
        public string OperatingSystem { get; set; } = "Linux";
        /// <summary>Compute architecture type.</summary>
        public string Architecture { get; set; } = "x86_64";

        /// <summary>Starts a compute instance.</summary>
        /// <param name="instanceId">Instance identifier.</param>
        /// <returns>StartInstanceResult record.</returns>
        public virtual StartInstanceResult StartInstance(string instanceId = "default") =>
            new StartInstanceResult($"Started instance {instanceId} in region {ComputeRegion}", 200);

        /// <summary>Stops a compute instance.</summary>
        /// <param name="instanceId">Instance identifier.</param>
        /// <returns>StopInstanceResult record.</returns>
        public virtual StopInstanceResult StopInstance(string instanceId = "default") =>
            new StopInstanceResult($"Stopped instance {instanceId}", 200);

        /// <summary>Lists all compute instances.</summary>
        /// <returns>ListInstancesResult record.</returns>
        public virtual ListInstancesResult ListInstances() =>
            new ListInstancesResult($"Listing all instances in {ComputeRegion}", new[] { "instance1", "instance2" });

        /// <summary>Gets the status of compute resources.</summary>
        /// <returns>ComputeStatusResult record.</returns>
        public virtual ComputeStatusResult GetStatus() =>
            new ComputeStatusResult($"Compute status: {MaxInstances} instances available in {ComputeRegion}", MaxInstances);

        /// <summary>Resizes compute resources.</summary>
        /// <param name="size">New size for compute resources.</param>
        /// <returns>ResizeResourcesResult record.</returns>
        public virtual ResizeResourcesResult ResizeResources(int size) =>
            new ResizeResourcesResult($"Resized compute resources to {size}", size);
    }

    /// <summary>Abstract storage submodule.</summary>
    public abstract class AbstractStorageSubmodule : SubmoduleBase {
        /// <summary>Type of storage.</summary>
        public string StorageType { get; set; } = "Blob";
        /// <summary>Maximum storage capacity in GB.</summary>
        public int MaxCapacity { get; set; } = 500;
        /// <summary>Default storage region.</summary>
        public string StorageRegion { get; set; } = "us-east";
        /// <summary>Number of active storage accounts.</summary>
        public int ActiveAccounts { get; set; } = 5;
        /// <summary>Indicates if the storage is encrypted.</summary>
        public bool IsEncrypted { get; set; } = true;

        /// <summary>Uploads a file to storage.</summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>UploadFileResult record.</returns>
        public virtual UploadFileResult UploadFile(string filePath = "default.txt") =>
            new UploadFileResult($"Uploaded file {filePath} to {StorageType} storage", filePath);

        /// <summary>Downloads a file from storage.</summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>DownloadFileResult record.</returns>
        public virtual DownloadFileResult DownloadFile(string fileName = "default.txt") =>
            new DownloadFileResult($"Downloaded file {fileName} from {StorageType} storage", fileName);

        /// <summary>Deletes a file from storage.</summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>DeleteFileResult record.</returns>
        public virtual DeleteFileResult DeleteFile(string fileName = "default.txt") =>
            new DeleteFileResult($"Deleted file {fileName} from {StorageType} storage", fileName);

        /// <summary>Lists all storage accounts.</summary>
        /// <returns>ListAccountsResult record.</returns>
        public virtual ListAccountsResult ListAccounts() =>
            new ListAccountsResult($"Listed {ActiveAccounts} active storage accounts in {StorageRegion}", ActiveAccounts);

        /// <summary>Gets the storage status.</summary>
        /// <returns>StorageStatusResult record.</returns>
        public virtual StorageStatusResult GetStorageStatus() =>
            new StorageStatusResult($"Storage status: {MaxCapacity}GB capacity in {StorageRegion}", MaxCapacity);
    }

    /// <summary>Abstract network submodule.</summary>
    public abstract class AbstractNetworkSubmodule : SubmoduleBase {
        /// <summary>Network type .</summary>
        public string NetworkType { get; set; } = "Virtual";
        /// <summary>Default network region.</summary>
        public string NetworkRegion { get; set; } = "us-central";
        /// <summary>Indicates network connectivity status.</summary>
        public bool IsConnected { get; set; } = true;
        /// <summary>Maximum bandwidth in Mbps.</summary>
        public int MaxBandwidth { get; set; } = 1000;
        /// <summary>Number of configured endpoints.</summary>
        public int EndpointCount { get; set; } = 10;

        /// <summary>Creates a network endpoint.</summary>
        /// <param name="endpointName">Name of the endpoint.</param>
        /// <returns>CreateEndpointResult record.</returns>
        public virtual CreateEndpointResult CreateEndpoint(string endpointName = "defaultEndpoint") =>
            new CreateEndpointResult($"Created endpoint {endpointName} in {NetworkRegion}", endpointName);

        /// <summary>Deletes a network endpoint.</summary>
        /// <param name="endpointName">Name of the endpoint.</param>
        /// <returns>DeleteEndpointResult record.</returns>
        public virtual DeleteEndpointResult DeleteEndpoint(string endpointName = "defaultEndpoint") =>
            new DeleteEndpointResult($"Deleted endpoint {endpointName}", endpointName);

        /// <summary>Lists all network endpoints.</summary>
        /// <returns>ListEndpointsResult record.</returns>
        public virtual ListEndpointsResult ListEndpoints() =>
            new ListEndpointsResult($"Listed {EndpointCount} endpoints for network type {NetworkType}", EndpointCount);

        /// <summary>Gets the network status.</summary>
        /// <returns>NetworkStatusResult record.</returns>
        public virtual NetworkStatusResult GetNetworkStatus() =>
            new NetworkStatusResult($"Network status: {(IsConnected ? "Connected" : "Disconnected")} in {NetworkRegion}", IsConnected);

        /// <summary>Tests network speed.</summary>
        /// <returns>TestSpeedResult record.</returns>
        public virtual TestSpeedResult TestSpeed() =>
            new TestSpeedResult($"Network speed test: {MaxBandwidth} Mbps", MaxBandwidth);
    }

    /// <summary>Compute module for cloud operations.</summary>
    public class ComputeModule : AbstractComputeSubmodule {
        /// <summary>Display name of compute module.</summary>
        public string DisplayName { get; set; } = "Compute";
        /// <summary>Runs a compute action.</summary>
        /// <param name="action">Action to perform.</param>
        /// <returns>Result of compute action.</returns>
        public string Run(string action = "default") => $"Compute executing {action}.";
        /// <summary>Virtual Machine submodule.</summary>
        public VirtualMachineModule VirtualMachine { get; set; } = new VirtualMachineModule();
        /// <summary>Container submodule.</summary>
        public ContainerModule Container { get; set; } = new ContainerModule();
    }

    /// <summary>Virtual Machine module for compute operations.</summary>
    public class VirtualMachineModule : AbstractComputeSubmodule {
        /// <summary>Module identifier for VM.</summary>
        public string ModuleId { get; set; } = "vm-001";
        /// <summary>Deploys a virtual machine.</summary>
        /// <param name="vmName">Name of the virtual machine.</param>
        /// <returns>Deployment result string.</returns>
        public string Deploy(string vmName = "defaultVM") => $"Deploying VM: {vmName}.";
        /// <summary>Gets the status of a virtual machine.</summary>
        /// <param name="vmId">ID of the virtual machine.</param>
        /// <returns>Status string.</returns>
        public string Status(string vmId = "vm-001") => $"Status for {vmId}: Running.";
    }

    /// <summary>Container module for compute operations.</summary>
    public class ContainerModule : AbstractComputeSubmodule {
        /// <summary>Module identifier for container.</summary>
        public string ModuleId { get; set; } = "container-001";
        /// <summary>Deploys a container.</summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>Deployment result string.</returns>
        public string Deploy(string containerName = "defaultContainer") => $"Deploying container: {containerName}.";
        /// <summary>Gets the status of a container.</summary>
        /// <param name="containerId">ID of the container.</param>
        /// <returns>Status string.</returns>
        public string Status(string containerId = "container-001") => $"Status for {containerId}: Running.";
    }

    /// <summary>Storage module for cloud operations.</summary>
    public class StorageModule : AbstractStorageSubmodule {
        /// <summary>Display name of storage module.</summary>
        public string DisplayName { get; set; } = "Storage";
        /// <summary>Runs a storage action.</summary>
        /// <param name="action">Action to perform.</param>
        /// <returns>Result of storage action.</returns>
        public string Run(string action = "default") => $"Storage executing {action}.";
        /// <summary>Blob submodule.</summary>
        public BlobModule Blob { get; set; } = new BlobModule();
        /// <summary>File submodule.</summary>
        public FileModule File { get; set; } = new FileModule();
    }

    /// <summary>Blob module for storage operations.</summary>
    public class BlobModule : AbstractStorageSubmodule {
        /// <summary>Name of the blob container.</summary>
        public string ContainerName { get; set; } = "defaultBlobContainer";
        /// <summary>Uploads a file to the blob container.</summary>
        /// <param name="filePath">Path of the file to upload.</param>
        /// <returns>Upload result string.</returns>
        public string Upload(string filePath = "file.txt") => $"Uploading {filePath} to blob container {ContainerName}.";
        /// <summary>Downloads a blob from the container.</summary>
        /// <param name="blobName">Name of the blob to download.</param>
        /// <returns>Download result string.</returns>
        public string Download(string blobName = "defaultBlob") => $"Downloading blob: {blobName}.";
    }

    /// <summary>File module for storage operations.</summary>
    public class FileModule : AbstractStorageSubmodule {
        /// <summary>Name of the file directory.</summary>
        public string DirectoryName { get; set; } = "defaultFileDirectory";
        /// <summary>Uploads a file to the directory.</summary>
        /// <param name="filePath">Path of the file to upload.</param>
        /// <returns>Upload result string.</returns>
        public string Upload(string filePath = "file.txt") => $"Uploading {filePath} to directory {DirectoryName}.";
        /// <summary>Downloads a file from the directory.</summary>
        /// <param name="fileName">Name of the file to download.</param>
        /// <returns>Download result string.</returns>
        public string Download(string fileName = "defaultFile") => $"Downloading file: {fileName}.";
    }

    /// <summary>Network module for cloud operations.</summary>
    public class NetworkModule : AbstractNetworkSubmodule {
        /// <summary>Display name of network module.</summary>
        public string DisplayName { get; set; } = "Network";
        /// <summary>Runs a network action.</summary>
        /// <param name="action">Action to perform.</param>
        /// <returns>Result of network action.</returns>
        public string Run(string action = "default") => $"Network executing {action}.";
        /// <summary>Virtual network submodule.</summary>
        public VirtualNetworkModule VirtualNetwork { get; set; } = new VirtualNetworkModule();
        /// <summary>Load balancer submodule.</summary>
        public LoadBalancerModule LoadBalancer { get; set; } = new LoadBalancerModule();
        /// <summary>Firewall submodule.</summary>
        public FirewallModule Firewall { get; set; } = new FirewallModule();
    }

    /// <summary>Virtual network module for network operations.</summary>
    public class VirtualNetworkModule : AbstractNetworkSubmodule {
        /// <summary>Identifier for the virtual network.</summary>
        public string NetworkId { get; set; } = "vnet-001";
        /// <summary>Creates a virtual network.</summary>
        /// <param name="networkName">Name of the virtual network.</param>
        /// <returns>Creation result string.</returns>
        public string Create(string networkName = "defaultVNet") => $"Creating virtual network: {networkName}.";
        /// <summary>Deletes a virtual network.</summary>
        /// <param name="networkName">Name of the virtual network.</param>
        /// <returns>Deletion result string.</returns>
        public string Delete(string networkName = "defaultVNet") => $"Deleting virtual network: {networkName}.";
    }

    /// <summary>Load balancer module for network operations.</summary>
    public class LoadBalancerModule : AbstractNetworkSubmodule {
        /// <summary>Identifier for the load balancer.</summary>
        public string LBId { get; set; } = "lb-001";
        /// <summary>Creates a load balancer.</summary>
        /// <param name="lbName">Name of the load balancer.</param>
        /// <returns>Creation result string.</returns>
        public string Create(string lbName = "defaultLB") => $"Creating load balancer: {lbName}.";
        /// <summary>Gets the status of the load balancer.</summary>
        /// <param name="lbId">ID of the load balancer.</param>
        /// <returns>Status result string.</returns>
        public string Status(string lbId = "lb-001") => $"Load balancer {lbId} status: Active.";
    }

    /// <summary>Firewall module for network operations.</summary>
    public class FirewallModule : AbstractNetworkSubmodule {
        /// <summary>Identifier for the firewall.</summary>
        public string FirewallId { get; set; } = "fw-001";
        /// <summary>Enables the firewall.</summary>
        /// <param name="firewallId">ID of the firewall.</param>
        /// <returns>Enable result string.</returns>
        public string Enable(string firewallId = "fw-001") => $"Enabling firewall: {firewallId}.";
        /// <summary>Disables the firewall.</summary>
        /// <param name="firewallId">ID of the firewall.</param>
        /// <returns>Disable result string.</returns>
        public string Disable(string firewallId = "fw-001") => $"Disabling firewall: {firewallId}.";
    }
}
