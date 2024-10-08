using System;
using System.Data;
using System.ServiceModel;
using ServiceReference;  // Replace with the namespace for the generated service reference
// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");


// var client = new Ws1ServiceClientExample();
// var result = await client.CallWs1ServiceAsync();
//

public partial class Program
{
  public static async Task Main(string[] args)
    {
        string partnerKey = "";
        string omgevingscode = "";

        // Check if any arguments were passed
        if (args.Length == 0 || args.Length > 2 )
        {
            Console.WriteLine("No command line arguments were provided or more than two.");
        }
        else
        {
            partnerKey = args[0];
            omgevingscode = args[1];

        }
        Console.WriteLine("Calling the service...");

        var clientExample = new Ws1ServiceClientExample();
        string sessionId = await clientExample.CallWs1ServiceAsync(partnerKey, omgevingscode);

        await clientExample.MaakVerkoopOrder(partnerKey, omgevingscode, sessionId);

        Console.WriteLine("Service call completed.");
    }
}


public class Ws1ServiceClientExample
{
    public ws1_xmlSoapClient CreateSoapClient() {
        // Create the binding
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.Security.Mode = BasicHttpSecurityMode.Transport;  // Use this if the service requires HTTPS
        binding.MaxReceivedMessageSize = 65536 + 65536;

        // Define the service endpoint
        EndpointAddress endpoint = new EndpointAddress("https://api.kingfinance.nl/v1/ws1_xml.asmx");

        // Create the client with the binding and endpoint
        // Ws1ServiceClient client = new Ws1ServiceClient(binding, endpoint);
        ws1_xmlSoapClient client = new ws1_xmlSoapClient(binding, endpoint);
        return client;
    }
    public async Task<string> CallWs1ServiceAsync(string partnerKey, string omgevingscode)
    {
        ws1_xmlSoapClient client = CreateSoapClient();
        string sessionId = "";

        try
        {
            // string SessionIdstring Foutmelding
            // Call the service method asynchronously
            ServiceReference.LoginResponse result = await client.LoginAsync(partnerKey, omgevingscode, "", "");
            // var result = await client.SomeMethodAsync("parameter1", "parameter2");

            // Process the result
            Console.WriteLine(result.Body.SessionId);
            sessionId = result.Body.SessionId;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        finally
        {
            // Close the client properly
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
            }
            else
            {
                client.Close();
            }
        }
        return sessionId;
    }

    public async Task MaakVerkoopOrder(string partnerKey, string omgevingscode, string sessionId) 
    {
      string foutmelding = "";

      DataSet dsVerkooporders = new DataSet();
      DataTable dtVerkoopKop = dsVerkooporders.Tables.Add();
      dtVerkoopKop.TableName = "ORDKOP";
      dtVerkoopKop.Columns.Add("DEB", typeof(System.Int32)).DefaultValue = 10000;
      dtVerkoopKop.Columns.Add("OPM", typeof(System.String)).DefaultValue = "Dit is een test"; // + ControlChars.CrLf + "over meerdere regels." + ControlChars.CrLf + "Via returns in de kop"
      dtVerkoopKop.Columns.Add("KENM", typeof(System.String)).DefaultValue = "Test csw";
      dtVerkoopKop.Columns.Add("DAT", typeof(DateTime)).DefaultValue = Convert.ToString(DateTime.Now.AddDays(-1).Day) + "-" + Convert.ToString(DateTime.Now.Month) + "-" + Convert.ToString(DateTime.Now.Year);
      dtVerkoopKop.Columns.Add("MAG", typeof(System.String)).DefaultValue = 1;
      // dtVerkoopKop.Columns.Add("ORDSRT", typeof(System.String)).DefaultValue = "spitsfactuur"; // "standaard"; // 
      dtVerkoopKop.Columns.Add("ORDSRT", typeof(System.String)).DefaultValue = "standaard"; // "standaard"; // 
      dtVerkoopKop.Columns.Add("EXTORDNR", typeof(System.String)).DefaultValue = ""; // Ordernummer uit een extern pakket.
      // dtVerkoopKop.Columns.Add("BTWPL", typeof(System.String)).DefaultValue = IMFIN.STOREVALUES.ORDKOP.BTWPL.BTWPLICHTIG;
      dtVerkoopKop.Columns.Add("RIT", typeof(System.Int32)).DefaultValue = 4; // Rit/NrRit
      dtVerkoopKop.Columns.Add("NRRIT", typeof(System.Int32)).DefaultValue = 10; // Rit/NrRit;

      DataRow drVerkoopKop = dtVerkoopKop.NewRow();
      dtVerkoopKop.Rows.Add(drVerkoopKop);

      DataTable dtVerkoopReg = dsVerkooporders.Tables.Add();
      dtVerkoopReg.TableName = "ORDRG";
      dtVerkoopReg.Columns.Add("AANT", typeof(System.String)).DefaultValue = 1;
      dtVerkoopReg.Columns.Add("ART", typeof(System.String)).DefaultValue = "9000";
      dtVerkoopReg.Columns.Add("OMSCHR", typeof(System.String)).DefaultValue = "Finner webshop";
      dtVerkoopReg.Columns.Add("PRS", typeof(System.String)).DefaultValue = 1025;
      DataRow drVerkoopReg = dtVerkoopReg.NewRow();
      dtVerkoopReg.Rows.Add(drVerkoopReg);

      bool doPdfBestand = true;
      if (doPdfBestand)
      {
        if (System.IO.File.Exists("DocumentMetPlaatje.pdf"))
        {
          DataTable dt_digdos = dsVerkooporders.Tables.Add();
          dt_digdos.TableName = "DIGDOS";
          dt_digdos.Columns.Add("FILENAME", typeof(System.String)).DefaultValue = "NAAM-ZKSL-x";
          dt_digdos.Columns.Add("FILEDESCRIPTION", typeof(System.String)).DefaultValue = "Omschrijving-x";
          dt_digdos.Columns.Add("FILE", typeof(byte[])).DefaultValue = DBNull.Value;
          DataRow dr_digdos = dt_digdos.NewRow();
          // dr_digdos["FILE"] = ReadFileToByteArray("C:\\temp\\CSW\\VoorbeeldPDF\\DocumentMetPlaatje.pdf");
          var path = Path.Combine(Directory.GetCurrentDirectory(), "DocumentMetPlaatje.pdf");
          dr_digdos["FILE"] = ReadFileToByteArray(path.ToString());

          dt_digdos.Rows.Add(dr_digdos);
        }
      }
      int OrderNr = 0;

      // ServiceReference.ws1_xmlSoapClient client = new ServiceReference.ws1_xmlSoapClient();
      //
      // ws1.ws1 ws1 = new ws1.ws1();
      // ws1.Url = "https://api.kingfinance.nl/v1/ws1.asmx";
      ws1_xmlSoapClient client = CreateSoapClient();

      dsVerkooporders.WriteXml("dsVerkooporders_input.xml");
      var result = await client.CreateVerkoopOrderAsync(partnerKey,omgevingscode,sessionId, dsVerkooporders.GetXml(), OrderNr, foutmelding); 
      // if (!client.CreateVerkoopOrderAsync(partnerKey, omgevingscode, sessionId, ref dsVerkooporders, ref OrderNr, ref foutmelding))
      if (result.Body.Foutmelding != "") {
        Console.WriteLine("mogelijke foutmelding: " + result.Body.Foutmelding);
      }
      else {
        Console.WriteLine("ordernr: " + result.Body.ORDNR + " is aangemaakt.");
        Console.WriteLine("verkooporder: " + result.Body.Verkooporder.ToString());
      }

      // {
      // }
      // else
      // {
      dsVerkooporders.WriteXml("dsVerkooporders_ouput.xml");
      // }

    }
    static byte[] ReadFileToByteArray(string path)
    {
      FileInfo oFile;
      oFile = new FileInfo(path);

      FileStream oFileStream = oFile.OpenRead();
      int lBytes = (int)oFileStream.Length;

      if (lBytes > 0)
      {
        byte[] fileData = new byte[lBytes];

        // Read the file into a byte array
        oFileStream.Read(fileData, 0, lBytes);
        oFileStream.Close();
        return fileData;
      }
      return null;
    }
}


