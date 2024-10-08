
Generated example with:

```bash
dotnet new console -n imuis-example
```

First example working with:
```bash
dotnet run
# hello world!

```


Adding the tools to the path:
```bash
export PATH="$PATH:/Users/pepijngriffioen/.dotnet/tools"
```

generate the proxy classes:
```bash
dotnet-svcutil https://api.kingfinance.nl/v1/ws1_xml.asmx?wsdl
```

Need to install .NET to run the app



Going to add the arguments. To input the partnerKey and omgevingscode via the command line.
```bash
export PARTNERKEY=REDACTED
export OMGEVINGSCODE=REDACTED

dotnet run $PARTNERKEY $OMGEVINGSCODE
```

Without the PdfFile it works:
```text
Calling the service...
66242/858@d1095;CSW
ordernr: 202400039 is aangemaakt.
verkooporder: <NewDataSet>
  <ORDKOP>
    <DEB>10000</DEB>
    <OPM>Dit is een test</OPM>
    <KENM>Test csw</KENM>
    <DAT>2024-10-07T00:00:00+02:00</DAT>
    <MAG>1</MAG>
    <ORDSRT>standaard</ORDSRT>
    <EXTORDNR />
    <RIT>4</RIT>
    <NRRIT>10</NRRIT>
    <NR>202400039</NR>
  </ORDKOP>
  <ORDRG>
    <AANT>1</AANT>
    <ART>9000</ART>
    <OMSCHR>Finner webshop</OMSCHR>
    <PRS>1025</PRS>
    <BEDR>1025</BEDR>
    <DATLEV>08-10-2024</DATLEV>
    <EENH>Stuk</EENH>
    <OPMEXT>De Finner 500 wordt geleverd inclusief  1 jaar diefstal verzekering. Stuur de polis direct na aflevering naar ons op.</OPMEXT>
    <OPMINT>Deze fiets wordt verpakt in een speciaal regenbestendig karton. De ketting dient altijd gesmeerd afgeleverd te worden. Het standaard meegeleverde slotnummer dient vastgelegd te worden in het computersysteem.</OPMINT>
    <ORDNR>202400039</ORDNR>
    <PERCKORT>0</PERCKORT>
    <PERCKORTKORT>0</PERCKORTKORT>
    <RG>5</RG>
    <STATUS>O</STATUS>
    <VERKOPER>groot</VERKOPER>
  </ORDRG>
  <INPUT>
    <DOMEIN>d1095</DOMEIN>
    <ADM>2118</ADM>
  </INPUT>
</NewDataSet>
Service call completed.
```

with the PDF file:
```text
Calling the service...
66242/866@d1095;CSW
mogelijke foutmelding: Kan een object van het type System.String niet converteren naar het type System.Byte[]. (CS312)
Service call completed.
```

Following the explanation: https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows&pivots=dotnet-8-0

GOing to dockerize the app:
```bash
dotnet publish -c Release
```
Now adding a Dockerfile and building it:
```bash
docker build -t imuis-demo -f Dockerfile .
```

Run the container:
```bash
docker create --name imuis-poc imuis-demo
```

start the container:
```bash
docker start imuis-poc
```

changed the entrypoint to make the container able to easily change the runs.

Now you can interactively use the container.
```bash
docker run -it -e CONTAINER_COMMAND="dotnet imuis-example.dll REPLACE_PARTNER_KEY REPLACE_OMGEVINGSCODE" imuis-demo
```

Added the pdf file via the wget in the build stage of the docker container, however that still did not fix it. Getting the error:
```text
Kan een object van het type System.String niet converteren naar het type System.Byte[]. (CS312)
```


Put the actions in a Makefile:
```bash
make build # build the container
make run # runs the container -> before running replace the keys
```


