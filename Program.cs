using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeSortDemoCSharp
{
    class Program
    {
        private const string ERROR_FILE_NOT_EXISTS = "\nERROR: Input file path ({0}) is not exists.";
        private const string OUTPUT_FILE_NAME = "-graded.txt";
        private const string OUTPUT_ERROR_FILE_NAME = "-error.txt";
        private const int CORRECT_VALUE_COUNT = 3;

        public class Student : IComparer
        {
            public string firstname;
            public string lastname;
            public int grade;

            public Student() : this("", "", 0)
            {

            }

            public Student(string firstname, string lastname, int grade)
            {
                this.firstname = firstname;
                this.lastname = lastname;
                this.grade = grade;
            }

            int IComparer.Compare(object x, object y)
            {
                Student a = (Student)x;
                Student b = (Student)y;
                
                if (a.grade > b.grade)
                {
                    return -1;
                }
                else if (a.grade < b.grade)
                {
                    return 1;
                }
                else
                {
                    int lastnameCompare = (new CaseInsensitiveComparer()).Compare(a.lastname, b.lastname);

                    if ((lastnameCompare < 0) || (lastnameCompare > 0))
                    {
                        return lastnameCompare;
                    }
                    else
                    {
                        return (new CaseInsensitiveComparer()).Compare(a.firstname, b.firstname);
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            ArrayList studentList = new ArrayList();
            ArrayList errorLines = new ArrayList();

            Console.WriteLine("--- Welcome to Grade Sort Demo ---");
            Console.WriteLine("----------------------------------");

            Console.Write("Please enter input file path: ");
            string inputFilePath = Console.ReadLine();
            string inputFileName = "";
            string outputFileDir = "";
            
            if (IsFilePathExists(inputFilePath))
            {
                inputFileName = Path.GetFileNameWithoutExtension(inputFilePath);
                outputFileDir = Path.GetDirectoryName(inputFilePath);

                string[] lines = File.ReadAllLines(inputFilePath);

                foreach (string line in lines)
                {
                    string[] values = line.Split(',');

                    try
                    {
                        if (values.Length == CORRECT_VALUE_COUNT)
                        {
                            string lastname = values[0].Trim();
                            string firstname = values[1].Trim();
                            int grade = Convert.ToInt16(values[2].Trim());

                            Student student = new Student(firstname, lastname, grade);
                            studentList.Add(student);
                        }
                        else
                        {
                            errorLines.Add(line);
                        }
                    }
                    catch (Exception)
                    {
                        errorLines.Add(line);
                    }
                }
                
                // Sorting
                IComparer comparer = new Student();
                studentList.Sort(comparer);

                // File writing
                string outputFilePath = (outputFileDir + "\\" + inputFileName + OUTPUT_FILE_NAME);

                if (IsFilePathExists(outputFilePath))
                {
                    File.Delete(outputFilePath);
                }

                foreach (Student student in studentList)
                {
                    using (StreamWriter writer = new StreamWriter(outputFilePath, true))
                    {
                        writer.WriteLine("{0}, {1}, {2}", student.lastname, student.firstname, student.grade);
                    }
                }

                string outputErrorFilePath = (outputFileDir + "\\" + inputFileName + OUTPUT_ERROR_FILE_NAME);

                if (IsFilePathExists(outputErrorFilePath))
                {
                    File.Delete(outputErrorFilePath);
                }

                if (errorLines.Count > 0)
                {
                    foreach (string errorLine in errorLines)
                    {
                        using (StreamWriter writer = new StreamWriter(outputErrorFilePath, true))
                        {
                            writer.WriteLine("{0}", errorLine);
                        }
                    }

                    Console.WriteLine("\nFinished with {0} error(s).", errorLines.Count);
                    Console.WriteLine("\nCreated:\t{0}", outputFilePath);
                    Console.WriteLine("Error log:\t{0}", outputErrorFilePath);
                }
                else
                {
                    Console.WriteLine("\nFinished.", errorLines.Count);
                    Console.WriteLine("\nCreated: {0}", outputFilePath);
                }
            }
            else
            {
                Console.WriteLine(ERROR_FILE_NOT_EXISTS, inputFilePath);
            }
            
            // Keep the console window open in debug mode.
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static bool IsFilePathExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
