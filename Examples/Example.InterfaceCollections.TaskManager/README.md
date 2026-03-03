# Example: Interface Collections Task Manager

Demonstrates interface collection support with IList, ICollection, IEnumerable, and their read-only variants.

## Features

This example showcases how CalqFramework.Cli handles various collection interface types:

- `IList<T>` - Mutable list interface
- `ICollection<T>` - Mutable collection interface  
- `IEnumerable<T>` - Basic enumerable interface
- `ISet<T>` - Mutable set interface (unique elements, automatically deduplicates)
- `IReadOnlyList<T>` - Read-only list interface
- `IReadOnlyCollection<T>` - Read-only collection interface
- `List<T>` - Concrete list type

Note: Collection parameters require repeating the parameter name for each value (e.g., `--names task1 --names task2`).

## Install

```bash
dotnet pack
dotnet tool install --global --add-source ./bin/Release Example.InterfaceCollections.TaskManager
```

## Usage Examples

### Create tasks with IList
```bash
taskmanager create --names task1 --names task2 --names task3
```

### Add tags with ICollection and ISet
```bash
# ISet automatically deduplicates - "urgent" appears twice but only added once
taskmanager add-tags --task-names task1 --task-names task2 --tags urgent --tags important --tags urgent
```

### List by priority with IEnumerable
```bash
taskmanager list-by-priority --priorities High --priorities Critical
```

### Update status with IReadOnlyList
```bash
taskmanager update-status --task-names task1 --task-names task2 --status Completed
```

### Get tasks by tags with IReadOnlyCollection
```bash
taskmanager get-by-tags --tags urgent --tags important
```

### Bulk update with mixed interfaces
```bash
taskmanager bulk-update --tasks task1 --tasks task2 --add-tags reviewed --add-tags approved
```

### Search with concrete List type
```bash
taskmanager search --keywords bug --keywords urgent --keywords backend
```

### Filter by status with List of enums
```bash
taskmanager filter-by-status --statuses Pending --statuses InProgress
```

## Uninstall

```bash
dotnet tool uninstall --global Example.InterfaceCollections.TaskManager
```
