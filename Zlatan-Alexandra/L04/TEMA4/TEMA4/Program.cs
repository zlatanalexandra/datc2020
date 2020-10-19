
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace TEMA4
{
    class Program
    {
        private static CloudTable studentsTable;
        private static CloudTableClient tableClient;
        private static TableOperation tableOperation;
        private static TableResult tableResult;
        private static List<StudentEntity> students = new List<StudentEntity>();
        static void Main(string[] args)
        {
            Task.Run(async () => { await Initialize(); })
                .GetAwaiter()
                .GetResult();
        }
        static async Task Initialize()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;"
            + "AccountName=;"
            + "AccountKey=;"
            + "EndpointSuffix=core.windows.net";

            var account = CloudStorageAccount.Parse(storageConnectionString);
            tableClient = account.CreateCloudTableClient();

            studentsTable = tableClient.GetTableReference("Students");

            await studentsTable.CreateIfNotExistsAsync();

            int option = -1;
            do
            {
                System.Console.WriteLine("1.Add .");
                System.Console.WriteLine("2.Update .");
                System.Console.WriteLine("3.Delete .");
                System.Console.WriteLine("4.Show all .");
                System.Console.WriteLine("5.Exit");
                string opt = System.Console.ReadLine();
                option = Int32.Parse(opt);
                switch (option)
                {
                    case 1:
                        await AddNewStudent();
                        break;
                    case 2:
                        await EditStudent();
                        break;
                    case 3:
                        await DeleteStudent();
                        break;
                    case 4:
                        await DisplayStudents();
                        break;
                    case 5:
                        System.Console.WriteLine("Exit!");
                        break;
                }
            } while (option != 5);

        }
        private static async Task<StudentEntity> RetrieveRecordAsync(CloudTable table, string partitionKey, string rowKey)
        {
            tableOperation = TableOperation.Retrieve<StudentEntity>(partitionKey, rowKey);
            tableResult = await table.ExecuteAsync(tableOperation);
            return tableResult.Result as StudentEntity;
        }

        private static async Task EditStudent()
        {
            System.Console.WriteLine("University:");
            string university = Console.ReadLine();
            System.Console.WriteLine("CNP:");
            string cnp = Console.ReadLine();
            StudentEntity stud = await RetrieveRecordAsync(studentsTable, university, cnp);
            if (stud != null)
            {
                System.Console.WriteLine("Record exists!");
                var student = new StudentEntity(university, cnp);
                System.Console.WriteLine("FirstName:");
                string firstName = Console.ReadLine();
                System.Console.WriteLine("LastName:");
                string lastName = Console.ReadLine();
                System.Console.WriteLine("Faculty:");
                string faculty = Console.ReadLine();
                System.Console.WriteLine("Year of study:");
                string year = Console.ReadLine();
                student.FirstName = firstName;
                student.LastName = lastName;
                student.Faculty = faculty;
                student.Year = Convert.ToInt32(year);
                student.ETag = "*";
                var updateOperation = TableOperation.Replace(student);
                await studentsTable.ExecuteAsync(updateOperation);
                System.Console.WriteLine("Updated!");
            }
            else
            {
                System.Console.WriteLine("Entries does not exists!");
            }
        }
        private static async Task DeleteStudent()
        {
            System.Console.WriteLine("University:");
            string university = Console.ReadLine();
            System.Console.WriteLine("CNP:");
            string cnp = Console.ReadLine();

            StudentEntity stud = await RetrieveRecordAsync(studentsTable, university, cnp);
            if (stud != null)
            {
                var student = new StudentEntity(university, cnp);
                student.ETag = "*";
                var deleteOperation = TableOperation.Delete(student);
                await studentsTable.ExecuteAsync(deleteOperation);
                System.Console.WriteLine("Deleted!");
            }
            else
            {
                System.Console.WriteLine("Entries does not exists!");
            }
        }
        private static async Task<List<StudentEntity>> GetAllStudents()
        {
            TableQuery<StudentEntity> tableQuery = new TableQuery<StudentEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StudentEntity> result = await studentsTable.ExecuteQuerySegmentedAsync(tableQuery, token);
                token = result.ContinuationToken;
                students.AddRange(result.Results);
            } while (token != null);
            return students;
        }
        private static async Task DisplayStudents()
        {
            await GetAllStudents();

            foreach (StudentEntity std in students)
            {
                Console.WriteLine("Faculty : {0}", std.Faculty);
                Console.WriteLine("FirstName : {0}", std.FirstName);
                Console.WriteLine("LastName : {0}", std.LastName);
                Console.WriteLine("Year : {0}", std.Year);
            }
            students.Clear();

        }

        private static async Task AddNewStudent()
        {
            System.Console.WriteLine("University:");
            string university = Console.ReadLine();
            System.Console.WriteLine("CNP:");
            string cnp = Console.ReadLine();
            System.Console.WriteLine("FirstName:");
            string firstName = Console.ReadLine();
            System.Console.WriteLine("LastName:");
            string lastName = Console.ReadLine();
            System.Console.WriteLine("Faculty:");
            string faculty = Console.ReadLine();
            System.Console.WriteLine("Year:");
            string year = Console.ReadLine();

            StudentEntity stud = await RetrieveRecordAsync(studentsTable, university, cnp);
            if (stud == null)
            {
                var student = new StudentEntity(university, cnp);
                student.FirstName = firstName;
                student.LastName = lastName;
                student.Faculty = faculty;
                student.Year = Convert.ToInt32(year);
                var insertOperation = TableOperation.Insert(student);
                await studentsTable.ExecuteAsync(insertOperation);
                System.Console.WriteLine("Inserted!");
            }
            else
            {
                System.Console.WriteLine("Entries exists!");
            }
        }
    }
}
