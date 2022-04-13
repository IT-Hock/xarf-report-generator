# XARF Plugin

The XARF plugin creates [XARF](http://abusix.github.io/xarf/) reports from the collected reports.

## Configuration

## EnableEmailReports (bool)

Whether or not to create *.eml Report files.

## EnableXarfReports (bool)

Whether or not to create XARF *.json Report files.

### EmailReportOutput (string)

A path to the directory where the XARF *.eml reports should be stored.

### EmailReportTemplate (string)

A path to an email report template.

### Bcc_Mail (string)

An email address that should be used as a BCC for the XARF reports.

### FromMail (string)

An email address that should be used as the From: field for the XARF reports.

### Organization (string)

The organization that should be used for the XARF reports.

### OrganizationEmail (string)

The email address of the organization that should be used for the XARF reports.

### Domain (string)

The domain that should be used for the XARF reports.

### ContactEmail (string)

The email address of the contact person that should be used for the XARF reports.

### ContactName (string)

The name of the contact person that should be used for the XARF reports.

### ContactPhone (string)

The phone number of the contact person that should be used for the XARF reports.

### OutputDirectory (string)

The path to the directory where the XARF reports should be stored.

## CombineEmlReport (bool)

When set to true, the XARF reports in EML Format will be combined into a single EML report per IP Address.

## Example Configuration

```json
{
  "EmailReportOutput": "",
  "EmailReportTemplate": null,
  "Bcc_Mail": null,
  "FromMail": "admin@sample.org",
  "Organization": "Sample Org",
  "OrganizationEmail": "admin@sample.org",
  "Domain": "sample.org",
  "ContactEmail": "john.doe@sample.org",
  "ContactName": "John Doe",
  "ContactPhone": "+1 555 555 555",
  "OutputDirectory": "xarf",
  "CombineEmlReport": false
}
```