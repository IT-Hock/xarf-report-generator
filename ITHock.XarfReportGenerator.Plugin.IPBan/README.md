# IPBan Plugin

The IPBan Plugin collects reports from IPBan log files.

## Configuration

### LogDirectory (string)

The path to the directory where the logfiles are located.
If this path does not exist it tries the default path for IPBan e.g. `C:\Program Files\IPBan` and `C:\Program Files\IPBanProPersonal`

## Example Configuration

```json
{
  "LogDirectory": "C:\\Program Files\\IPBanProPersonal"
}
```