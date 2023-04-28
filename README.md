# azure-appservice-apis
Sample code to access Azure App Service or kudu (scm) apis

## Pre-req

- Create identity (AAD App/SPN)
- Assign one of the relevant role to the idetity on the scoped appservice/function/logicapp [Website Contributor/Contributor/ Owner]
	- https://learn.microsoft.com/en-us/azure/app-service/resources-kudu#rbac-permissions-required-to-access-kudu)

## Details

- This is using AAD based auth to access apis.
- Package `Microsoft.Identity.Client` is being used to access token.