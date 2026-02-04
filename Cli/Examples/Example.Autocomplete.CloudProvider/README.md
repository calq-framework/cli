# Example: Autocomplete Cloud Provider

Demonstrates autocomplete with enums and custom completion providers.

## Install

```bash
dotnet pack
dotnet tool install --global --add-source ./bin/Release Example.Autocomplete.CloudProvider
```

## Usage

```bash
cloudprovider deploy --provider AWS
cloudprovider configure-region --region us-east-1
```

## Uninstall

```bash
dotnet tool uninstall --global Example.Autocomplete.CloudProvider
```


