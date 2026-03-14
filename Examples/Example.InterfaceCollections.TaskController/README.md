# Example: Interface Collections Task Manager

Demonstrates collection interface support with both generic and non-generic interfaces.

## Features

This example shows various collection interface types:

- `IList<T>`, `ICollection<T>`, `IEnumerable<T>` - Generic collection interfaces
- `ISet<T>` - Set interface (automatically deduplicates)
- `IReadOnlyList<T>`, `IReadOnlyCollection<T>` - Read-only interfaces
- `List<T>` - Concrete list type
- `IList`, `IEnumerable` - Non-generic interfaces

The framework supports list-like collection types from `System.Collections` and `System.Collections.Generic` (lists, sets, arrays, etc.). Dictionary types are not supported.

Collection parameters require repeating the parameter name for each value (e.g., `--names task1 --names task2`).

## Install

```bash
dotnet pack
dotnet tool install --global --add-source ./bin/Release Example.InterfaceCollections.TaskController
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

### Archive tasks with non-generic IList
```bash
taskmanager archive --task-names task1 --task-names task2 --task-names task3
```

### Export tasks with non-generic IEnumerable
```bash
taskmanager export --task-names task1 --task-names task2
```

## Uninstall

```bash
dotnet tool uninstall --global Example.InterfaceCollections.TaskController
```
