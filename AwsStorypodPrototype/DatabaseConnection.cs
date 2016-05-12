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

        /// <summary>
        /// Create KeySchemaElemet list based on current attributes
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public List<KeySchemaElement> GetKeySchema(List<AttributeDefinition> attributes)
        {
            List<KeySchemaElement> schema = new List<KeySchemaElement>();

            int counter = 0;
            foreach (AttributeDefinition atr in attributes)
            {
                if (counter == 0)
                {
                    schema.Add(new KeySchemaElement(atr.AttributeName, KeyType.HASH));
                }
                else if (counter == 1)
                {
                    schema.Add(new KeySchemaElement(atr.AttributeName, KeyType.RANGE));
                }
                counter++;
            }
            Console.WriteLine(schema.Count); 
            return schema;
        }

        /// <summary>
        /// Create a list of attributes for a new table.
        /// </summary>
        /// <returns></returns>
        public List<AttributeDefinition> GetAttributes()
        {
            List<AttributeDefinition> attributes = new List<AttributeDefinition>();
            Console.WriteLine("---Let's define attribute attributes for the table---");
            string input, inputType;
            int numberOfAttributes = 0;

            do
            {
                // Clear input vars 
                input = inputType = "";
                Console.WriteLine("---Current attribute list:");
                foreach (AttributeDefinition atr in attributes)
                {
                    Console.WriteLine("Name: {0} Type: {1}", atr.AttributeName, atr.AttributeType);
                }
                Console.WriteLine("---Please input new attribute name, or 'F' if finished:");
                input = Console.ReadLine();

                // If there's some kind of name given, that isn't an F
                if (!(input == "" || input.ToUpper() == "F"))
                {
                    input = input.ToLower();
                    Console.WriteLine("Thanks, cased attribute name will be: {0}", input);

                    do
                    {
                        inputType = "";
                        Console.WriteLine("Please define attribute type...\nB - Bool\nN - Number\nS - String");
                        inputType = Console.ReadLine().ToUpper();

                    } while (!(inputType == "B" || inputType == "N" || inputType == "S"));

                    string inputConfirm = "";
                    do
                    {
                        Console.WriteLine("Confirm addition of attribute (Y or N): \nName: {0} Type: {1}", input, inputType);
                        inputConfirm = Console.ReadLine();
                    } while (!(inputConfirm.ToUpper() == "Y" || inputConfirm.ToUpper() == "N"));

                    // Finally, add new attribute to List is the user is happy
                    if (inputConfirm.ToUpper() == "Y")
                    {
                        AttributeDefinition atrb = new AttributeDefinition();
                        atrb.AttributeName = input;
                        switch (inputType)
                        {
                            case "S":
                                {
                                    atrb.AttributeType = ScalarAttributeType.S;
                                }
                                break;
                            case "N":
                                {
                                    atrb.AttributeType = ScalarAttributeType.N;
                                }
                                break;
                            case "B":
                                {
                                    atrb.AttributeType = ScalarAttributeType.B;
                                }
                                break;
                        }
                        attributes.Add(atrb);
                        numberOfAttributes++;
                    }
                }
            } while (input.ToUpper() != "F" && numberOfAttributes < 2);

            if (attributes.Count == 0)
            {
                Exception e = new Exception("You didnt define any attributes for your table!");
            }
            Console.WriteLine("Maximum number of schema attributes definted (2), creating table...");
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

                CreateTableRequest newTableReq = new CreateTableRequest();
                // Instantiate an object before applying values.
                newTableReq.TableName = proposedTableName;
                newTableReq.AttributeDefinitions = GetAttributes();
                newTableReq.KeySchema = GetKeySchema(newTableReq.AttributeDefinitions);
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
