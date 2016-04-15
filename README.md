# Unified web service for OData V4
Connection string to DB defined in ODataRestierDynamic\ODataRestierDynamic\Web.config in block connectionStrings with name DefaultDataSource, for example:

  ```xml
  <connectionStrings>
    <add name="DefaultDataSource" connectionString="data source=MSSQL2014SRV;initial catalog=B2MML-BatchML;persist security info=False;Integrated Security=SSPI;" providerName="System.Data.SqlClient" />
  </connectionStrings>```

 Path to log folder defined in ODataRestierDynamic\ODataRestierDynamic\Web.config in block appSettings with key LogFilePath, for example:

  ```xml
  <appSettings>    
    <add key="LogFilePath" value="C:\WebServices\ODataRestierDynamic\Log\Logger.log"/>
  </appSettings>```
  