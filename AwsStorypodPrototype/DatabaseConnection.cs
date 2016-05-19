using System;
using System.Collections.Generic;
using System.Threading;

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
        AmazonDynamoDBClient _Db;
        public AmazonDynamoDBClient Db
        {
            get
            {
                return _Db;
            }

            set
            {
                _Db = value;
            }
        }

        public DatabaseConnection()
        {
            Db = new AmazonDynamoDBClient();
        }

        public void ListTables()
        {
            Console.WriteLine("\nListing all tables..");
            ListTablesResponse listOfTableNames = Db.ListTables();
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
        /// List the contents of a chosen table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="searchTerm"></param>
        /// <param name="idQuery"></param>
        public void ListTableObjects(Table table, string searchTerm = "", int idQuery = 0)
        {
            // Grab the attributes from the passed table
            List<AttributeDefinition> attrs = table.Attributes;

            // Search the second attribute in the table
            ScanFilter filter = new ScanFilter();
            Console.WriteLine(attrs[0].AttributeName);
            filter.AddCondition(attrs[0].AttributeName, ScanOperator.GreaterThan, 0); // This allows testing of the id 

            ScanOperationConfig config = new ScanOperationConfig();
            config.Filter = filter;

            // Selecting the attributes to return
            //List<string> attrTargets = new List<string>();
            //attrTargets.Add(attrs[0].AttributeName);

            Search tableSearch = table.Scan(config);
            List<Document> tableSearchResults = new List<Document>();
            Console.WriteLine("Attempting to list contents of table: {0}", table.TableName);

            do
            {
                if (tableSearch == null)
                {
                    Console.WriteLine("No search results!!");
                    break;
                }
                try
                {
                    // Search the table, store the results - THIS IS NOT ALL OF THE RESULTS, JUST ONE PAGE
                    tableSearchResults = (tableSearch.GetNextSet());
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}\n{1}", e.Message, e.StackTrace);
                    break;
                }

                foreach (Document doc in tableSearchResults)
                {
                    if (searchTerm != "")
                    {
                        if (doc["VideoName"].ToString().ToUpper().Contains(searchTerm.ToUpper()) || doc["VideoName"].ToString().ToUpper().Contains(searchTerm.ToUpper()))
                        {
                            Console.WriteLine("---");
                            Console.WriteLine("ID: {0}\nName: {1}", doc["id"], doc["VideoName"]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("---");
                        Console.WriteLine("ID: {0}\nName: {1}", doc["id"], doc["VideoName"]);
                    }
                }
                Console.WriteLine();

            } while (!tableSearch.IsDone);
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
                // First attribute is HASH, second is RANGE
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
        /// Create a list of up tp two (was originally designed for any amount, could probably reuse later) attributes for a new table.
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

        /// <summary>
        /// Create a table, simple takes a name for the time being
        /// </summary>
        /// <param name="proposedTableName"></param>
        public void CreateTable(string proposedTableName)
        {
            try
            {
                if (Db.ListTables().TableNames.Contains(proposedTableName))
                {
                    Exception e = new Exception("This table already exists!");
                    throw e;
                }
                CreateTableRequest newTableReq = new CreateTableRequest();
                // Instantiate an object before applying values.
                newTableReq.TableName = proposedTableName;
                newTableReq.AttributeDefinitions = GetAttributes();
                newTableReq.KeySchema = GetKeySchema(newTableReq.AttributeDefinitions);
                newTableReq.ProvisionedThroughput = new ProvisionedThroughput();
                newTableReq.ProvisionedThroughput.ReadCapacityUnits = 1;
                newTableReq.ProvisionedThroughput.WriteCapacityUnits = 1;

                CreateTableResponse newTableResponse = new CreateTableResponse();
                newTableResponse = Db.CreateTable(newTableReq);
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine(nre.Message);
            }
        }

        /// <summary>
        /// Get a table object based on name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>Table</returns>
        public Table GetTable(string tableName)
        {
            Table tb;
            try
            {
                tb = Table.LoadTable(Db, tableName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load table: {0}\n{1}", e.Message, e.StackTrace);
                return null;
            }

            return tb;
        }

        /// <summary>
        /// Add an object to a table
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="targetTable"></param>
        public void AddToTable(Document doc, Table targetTable)
        {
            try
            {
                Console.WriteLine("Adding Document to Table: {0}, please wait...", targetTable.TableName);
                //Thread.Sleep(000);
                targetTable.PutItem(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}\n{1}", e.Message, e.StackTrace);
            }
        }
    }
}
