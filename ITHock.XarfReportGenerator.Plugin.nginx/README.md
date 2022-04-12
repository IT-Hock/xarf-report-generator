# nginx Plugin

The nginx Plugin collects reports from nginx log files.

## Configuration

### StatusCodeFilter (string)

A regular expression that defines which status codes should be collected.

### MethodFilter (string)

A regular expression that defines which HTTP methods should be collected.

### PathFilter (string)

A regular expression that defines which paths should be collected.

### LogDirectory (string)

The path to the directory where the logfiles are located.

## Example Configuration

```json
{
  "StatusCodeFilter": "403|405|406|400",
  "MethodFilter": "POST|PUT|DELETE|HEAD|OPTIONS|TRACE|CONNECT",
  "PathFilter": null
  "LogDirectory": "C:\\nginx\\logs"
}
```