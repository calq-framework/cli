using System;

namespace CloudProvider {
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
    public abstract class ManagerBase {
        /// <summary>API Key used to auth the command.</summary>
        public string? ApiKey { get; set; } = System.IO.File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "calq.cli.example.txt"))
            ? System.IO.File.ReadAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "calq.cli.example.txt"))
            : null;
    }

    /// <summary>Everything should just work even if any class or its contents are refactored.</summary>
    public class CloudManager : ManagerBase {
        /// <summary>Permanently saves api key into calq.cli.example.txt in the user dir.</summary>
        /// <param name="ApiKey">Without shadowing enabled, throws error on use</param>
        public void Add(string ApiKey) => System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "calq.cli.example.txt"), ApiKey);
        /// <summary>Removes calq.cli.example.txt</summary>
        public void Remove() => System.IO.File.Delete(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "calq.cli.example.txt"));
        /// <summary>Compute submodule.</summary>
        public ComputeManager Compute { get; set; } = new ComputeManager();
        /// <summary>Storage submodule.</summary>
        public StorageManager Storage { get; set; } = new StorageManager();
        /// <summary>Network submodule.</summary>
        public NetworkManager Network { get; set; } = new NetworkManager();
    }

    /// <summary>Abstract compute submodule.</summary>
    public abstract class AbstractComputeManager : ManagerBase {
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
    public abstract class AbstractStorageManager : ManagerBase {
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
    public abstract class AbstractNetworkManager : ManagerBase {
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
    public class ComputeManager : AbstractComputeManager {
        /// <summary>Display name of compute module.</summary>
        public string DisplayName { get; set; } = "Compute";
        /// <summary>Runs a compute action.</summary>
        /// <param name="action">Action to perform.</param>
        /// <returns>Result of compute action.</returns>
        public string Run(string action = "default") => $"Compute executing {action}.";
        /// <summary>Virtual Machine submodule.</summary>
        public VirtualMachineManager VirtualMachine { get; set; } = new VirtualMachineManager();
        /// <summary>Container submodule.</summary>
        public ContainerManager Container { get; set; } = new ContainerManager();
    }

    /// <summary>Virtual Machine module for compute operations.</summary>
    public class VirtualMachineManager : AbstractComputeManager {
        /// <summary>Manager identifier for VM.</summary>
        public string ManagerId { get; set; } = "vm-001";
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
    public class ContainerManager : AbstractComputeManager {
        /// <summary>Manager identifier for container.</summary>
        public string ManagerId { get; set; } = "container-001";
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
    public class StorageManager : AbstractStorageManager {
        /// <summary>Display name of storage module.</summary>
        public string DisplayName { get; set; } = "Storage";
        /// <summary>Runs a storage action.</summary>
        /// <param name="action">Action to perform.</param>
        /// <returns>Result of storage action.</returns>
        public string Run(string action = "default") => $"Storage executing {action}.";
        /// <summary>Blob submodule.</summary>
        public BlobManager Blob { get; set; } = new BlobManager();
        /// <summary>File submodule.</summary>
        public FileManager File { get; set; } = new FileManager();
    }

    /// <summary>Blob module for storage operations.</summary>
    public class BlobManager : AbstractStorageManager {
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
    public class FileManager : AbstractStorageManager {
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
    public class NetworkManager : AbstractNetworkManager {
        /// <summary>Display name of network module.</summary>
        public string DisplayName { get; set; } = "Network";
        /// <summary>Runs a network action.</summary>
        /// <param name="action">Action to perform.</param>
        /// <returns>Result of network action.</returns>
        public string Run(string action = "default") => $"Network executing {action}.";
        /// <summary>Virtual network submodule.</summary>
        public VirtualNetworkManager VirtualNetwork { get; set; } = new VirtualNetworkManager();
        /// <summary>Load balancer submodule.</summary>
        public LoadBalancerManager LoadBalancer { get; set; } = new LoadBalancerManager();
        /// <summary>Firewall submodule.</summary>
        public FirewallManager Firewall { get; set; } = new FirewallManager();
    }

    /// <summary>Virtual network module for network operations.</summary>
    public class VirtualNetworkManager : AbstractNetworkManager {
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
    public class LoadBalancerManager : AbstractNetworkManager {
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
    public class FirewallManager : AbstractNetworkManager {
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
