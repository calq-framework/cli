namespace Example.InterfaceCollections.TaskController;

/// <summary>Task priority enum.</summary>
public enum TaskPriority {
    Low,
    Medium,
    High,
    Critical
}

/// <summary>Task status enum.</summary>
public enum TaskStatus {
    Pending,
    InProgress,
    Completed,
    Blocked
}

/// <summary>Simple task model.</summary>
public class TaskItem {
    public string Name { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }

    public override string ToString() => $"{Name} [{Priority}] ({Status})";
}

/// <summary>CLI for task management demonstrating interface collection support.</summary>
public class TaskControllerManager {
    /// <summary>Default tags applied to new tasks.</summary>
    public List<string> DefaultTags { get; set; } = [
        "work"
    ];

    /// <summary>Create tasks with names (IList interface).</summary>
    /// <param name="names">List of task names to create.</param>
    /// <returns>Result message.</returns>
    public static string Create(IList<string> names) {
        var tasks = names.Select(n => new TaskItem {
            Name = n,
            Priority = TaskPriority.Medium,
            Status = TaskStatus.Pending
        })
            .ToList();

        return $"Created {tasks.Count} tasks: {string.Join(", ", tasks)}";
    }

    /// <summary>Add tags to tasks (ICollection and ISet interfaces).</summary>
    /// <param name="taskNames">Collection of task names.</param>
    /// <param name="tags">Tags to add (using ISet for uniqueness).</param>
    /// <returns>Result message.</returns>
    public static string AddTags(ICollection<string> taskNames, ISet<string> tags) => $"Added tags [{string.Join(", ", tags)}] to {taskNames.Count} tasks: {string.Join(", ", taskNames)}";

    /// <summary>List tasks by priority (IEnumerable interface).</summary>
    /// <param name="priorities">Priorities to filter by.</param>
    /// <returns>Result message.</returns>
    public static string ListByPriority(IEnumerable<TaskPriority> priorities) {
        string priorityList = string.Join(", ", priorities);
        return $"Listing tasks with priorities: {priorityList}";
    }

    /// <summary>Update task statuses (IReadOnlyList interface).</summary>
    /// <param name="taskNames">Read-only list of task names.</param>
    /// <param name="status">New status to apply.</param>
    /// <returns>Result message.</returns>
    public static string UpdateStatus(IReadOnlyList<string> taskNames, TaskStatus status) => $"Updated {taskNames.Count} tasks to status: {status}";

    /// <summary>Get tasks by tags (IReadOnlyCollection interface).</summary>
    /// <param name="tags">Read-only collection of tags to filter by.</param>
    /// <returns>Result message.</returns>
    public static string GetByTags(IReadOnlyCollection<string> tags) => $"Finding tasks with tags: {string.Join(", ", tags)} (searching {tags.Count} tags)";

    /// <summary>Bulk update with mixed interfaces.</summary>
    /// <param name="tasks">List of tasks to update.</param>
    /// <param name="addTags">Collection of tags to add.</param>
    /// <returns>Result message.</returns>
    public static string BulkUpdate(IList<string> tasks, ICollection<string> addTags) => $"Bulk updated {tasks.Count} tasks with {addTags.Count} tags";

    /// <summary>Search tasks by keywords (concrete List type).</summary>
    /// <param name="keywords">Keywords to search for.</param>
    /// <returns>Result message.</returns>
    public static string Search(List<string> keywords) => $"Searching for tasks matching: {string.Join(", ", keywords)}";

    /// <summary>Filter by statuses (concrete List with enum).</summary>
    /// <param name="statuses">Statuses to filter by.</param>
    /// <returns>Result message.</returns>
    public static string FilterByStatus(List<TaskStatus> statuses) => $"Filtering tasks by statuses: {string.Join(", ", statuses)}";

    /// <summary>Archive tasks (non-generic IList interface).</summary>
    /// <param name="taskNames">Non-generic list of task names to archive.</param>
    /// <returns>Result message.</returns>
    public static string Archive(IList taskNames) {
        IEnumerable<string> names = taskNames.Cast<object>()
            .Select(n => n?.ToString() ?? "");
        return $"Archived {taskNames.Count} tasks: {string.Join(", ", names)}";
    }

    /// <summary>Export tasks (non-generic IEnumerable interface).</summary>
    /// <param name="taskNames">Non-generic enumerable of task names to export.</param>
    /// <returns>Result message.</returns>
    public static string Export(IEnumerable taskNames) {
        var names = taskNames.Cast<object>()
            .Select(n => n?.ToString() ?? "")
            .ToList();
        return $"Exported {names.Count} tasks: {string.Join(", ", names)}";
    }
}
