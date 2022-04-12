# EventViewer Plugin

The EventViewer Plugin collects reports from the Windows EventViewer. The collected Events can be configured in the configuration file

## Configuration

### FilterEventID (int array)

A list of EventIDs that should be collected.

## Example Configuration

```json
{
  "FilterEventID": [5152]
}
```