using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace cw2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            if (args.Length < 3)
            {
                throw new ArgumentNullException();
            }

            FileStream log = new FileStream( @"log.txt", FileMode.OpenOrCreate);
            
            var pathFrom = args[0];
            var pathTo = args[1];
            var type = args[2];

            var lines = File.ReadLines(pathFrom);

            var students = new HashSet<Student>(new StudentComparer());
            var studies = new HashSet<Study>();

            foreach (var line in lines)
            {
                try
                {
                    var array = line.Split(",");

                    if (array.Length < 9) throw new NotRightData(line); ;

                    foreach (var value in array)
                    {
                        if (value.Length == 0) throw new NotFullData(line);
                    }

                    var study = new Study { Name = array[2] };

                    var student = new Student
                    {
                        Name = array[0],
                        Surname = array[1],
                        Lecture = new Lecture { Name = array[2], Type = array[3] },
                        Index = int.Parse(array[4]),
                        Birthday = DateTime.Parse(array[5]).ToString("dd.MM.yyyy"),
                        Email = array[6],
                        MothersName = array[7],
                        FathersName = array[8]
                    };

                    if (students.Contains(student)) throw new DuplicateData(line);

                    students.Add(student);

                    bool exist = false;
                    
                    foreach (var s in studies)
                    {
                        if (s.Equals(study))
                        {
                            exist = true;
                            s.CountOfStudents++;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        studies.Add(new Study { Name = array[2], CountOfStudents = 1 });
                    }

                }
                catch (Exception e)
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(e.Message + "\n");
                    log.Write(info, 0, info.Length);
                }

            }
            
            var listStudents = new List<Student>();

            foreach (var student in students)
            {
                listStudents.Add(student);
            }

            var listStudies = new List<Study>();

            foreach (var study in studies)
            {
                listStudies.Add(study);
            }

            var university = new University { Author = "Taras Kulyavtes",
                CreatedAt = DateTime.Now.ToString("dd.MM.yyyy"),
                Students = listStudents,
                ActiveStudies = listStudies
            };

            if (type.Equals("xml"))
            {
                FileStream writer = new FileStream(pathTo, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(typeof(University));
                serializer.Serialize(writer, university);
            }
            else if (type.Equals("json"))
            {
                var jsonString = JsonSerializer.Serialize(university); 
                File.WriteAllText(pathTo, jsonString);
            }
        }
    }

    class NotFullData : Exception
    {
        public NotFullData(string message) : base("NotFullData: " + message) { }
    }

    class NotRightData : Exception
    {
        public NotRightData(string message) : base("NotRightData: " + message) { }
    }

    class DuplicateData : Exception
    {
        public DuplicateData(string message) : base("DuplicateData: " + message) { }
    }

    class StudentComparer : IEqualityComparer<Student>
    {
        public bool Equals(Student x, Student y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (x.Index != y.Index)
            {
                return false;
            }

            if (!x.Name.Equals(y.Name))
            {
                return false;
            }
            
            return x.Surname.Equals(y.Surname);
        }

        public int GetHashCode(Student obj)
        {
            return obj.Index;
        }
    }

    public class Student {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string MothersName { get; set; }
        public string FathersName { get; set; }
        public int Index { get; set; }
        public string Birthday { get; set; }
        public Lecture Lecture { get; set; }
    }

    public class Lecture
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Study {
        public string Name { get; set; }
        public int CountOfStudents { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Study))
            {
                return false;
            }
            return this.Name == ((Study)obj).Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class University {
        public string CreatedAt { get; set; }
        public string Author { get; set; }
        public List<Student> Students { get; set; }
        public List<Study> ActiveStudies { get; set; }
    }
}
