# Etalify.SalesTax
A microservice for looking up US sales tax rates from a street address. Uses the [Washington State Sales Tax Library](http://dor.wa.gov/content/findtaxesandrates/retailsalestax/destinationbased/salestaxlibrary.aspx) for WA rate lookups and [TaxJar](https://www.taxjar.com/) for out of state lookups.

## Configuration

Out of WA state sales tax rate lookups make use of TaxJar and therefore require a [TaxJar API key](https://www.taxjar.com/api/).

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
  </startup>
  <appSettings>
    <add key="TaxService.Url" value="https://api.taxjar.com/v2/rates/" />
    <add key="TaxService.ApiKey" value="" />
  </appSettings>
</configuration>
```

## Example

**Request**

```
curl -X GET -H "Accept: application/json"
"http://localhost:1337/rates?street=1225%204th%20Ave&city=Seattle&state=WA&zip=98101"
```

**Response**

```javascript
{
  "locationCode": "1726",
  "locationName": "SEATTLE",
  "totalRate": 0.096
}
```
