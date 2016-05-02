using Microsoft.Azure.Documents.Client;
using System;

namespace NBudgetCommon.Factory
{
    public class DocumentClientFactory
    {
        private const string EndpointUri = "https://nbdocumentdb.documents.azure.com:443/";
        private const string PrimaryKey = "LXFOCBF2F6DiAof2Y4aUJ14mL5zq7YX44b3cWVMaNvGz4gsKWdpf8Fi1GN6rj0J1kEwy8lGkEVUhFFlz98Suuw==";

        public static DocumentClient CreateDocumentClient()
        {

            return new DocumentClient(new Uri(EndpointUri), PrimaryKey);
        }
    }
}
