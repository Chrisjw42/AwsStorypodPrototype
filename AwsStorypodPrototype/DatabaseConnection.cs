using System;
using System.Collections.Generic;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.SecurityToken;

namespace AwsConsoleApp1
{
    class DatabaseConnection
    {
        AmazonDynamoDBClient db;
        public DatabaseConnection()
        {
            // db = new AmazonDynamoDBClient("AKIAJ65JLQAK4WBGS5NA", "EJDFXPwUb2HS90r0KGSA9UJDNFbRd7X9/Z0oWlXo");
            db = new AmazonDynamoDBClient();
        }

        public void ListCurrentTables()
        {
            Console.WriteLine("\nListing all tables..");
            ListTablesResponse listOfTableNames = db.ListTables();
            int tableCount = 1;
            foreach (string table in listOfTableNames.TableNames)
            {
                Console.Write("Table {0}:", tableCount);
                Console.WriteLine(table);
                tableCount++;
            }
            Console.WriteLine("\nFinished!\n");
        }


        public List<KeySchemaElement> CreateTableSchema()
        {
            string input = "";

            do
            {
                // Name table
                // loop add attributes 
                input = "X";

            } while (input.ToUpper() != "X");
            Console.WriteLine("It's not ready yet, default schema created");
            List<KeySchemaElement> schema = new List<KeySchemaElement>();

            KeySchemaElement schm = new KeySchemaElement("title", KeyType.HASH);

            schema.Add(new KeySchemaElement("title", KeyType.HASH));
            Console.WriteLine(schema.Count); 
            return schema;
        }

        public List<AttributeDefinition> GetAttributes()
        {
            List<AttributeDefinition> attributes = new List<AttributeDefinition>();

            AttributeDefinition name = new AttributeDefinition("title", ScalarAttributeType.S);
            attributes.Add(name);
            return attributes;
        }

        public void CreateTable(string proposedTableName)
        {
            try
            {
                if (db.ListTables().TableNames.Contains(proposedTableName))
                {
                    Exception e = new Exception("This table already exists!");
                    throw e;
                }

                // for readability
                List<KeySchemaElement> proposedSchema = CreateTableSchema();

                CreateTableRequest newTableReq = new CreateTableRequest();
                // Instantiate an object before applying values.
                newTableReq.TableName = proposedTableName;
                newTableReq.AttributeDefinitions = GetAttributes();
                newTableReq.KeySchema = proposedSchema;
                newTableReq.ProvisionedThroughput = new ProvisionedThroughput();
                newTableReq.ProvisionedThroughput.ReadCapacityUnits = 1;
                newTableReq.ProvisionedThroughput.WriteCapacityUnits = 1;

                CreateTableResponse newTableResponse = new CreateTableResponse();
                newTableResponse = db.CreateTable(newTableReq);
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine(nre.Message);
            }        
        }
    }
}
